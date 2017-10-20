Imports System.Threading 'Thread.Sleep
Imports System.Runtime.InteropServices ' for release interface code 
Imports ZMOTIFPRINTERLib ' ZMotifPrinter SDK
Imports ZMTGraphics ' ZMotifGraphics SDK
Imports SmartCardOps ' WinSCard functions

Public Class MainForm

    Private Const CARD_NOT_DETECTED As Integer = 4014
    Private Const OUT_OF_CARDS As Integer = 4016
    Private Const EP_SCRIPT_ERROR As Integer = 7014
    Private Const ACTION_ID_NOT_FOUND As Integer = 13003

    Private _job As Job ' ZMotifPrinterSDK Job Object
    Private _graphics As ZMotifGraphics 'ZMotifGraphics SDK object
    Private _contactlessReader, _contactReader As String
    Private _abort As Boolean = False
    Private _status As String
    Private _alarm As Integer
    Private _lastActionID As Integer

    Private _startup As Boolean

    Private _bmpBack As Byte() = Nothing
    Private _bmpFront As Byte() = Nothing

    Public Enum TEST_TYPE
        PRINTING
        PRINTINGwithEIN
        PRINTINGwithBARCODE
        MAG
        MAGwithEIN
        MAGwithBARCODE
        CONTACTLESS
        CONTACTLESSwithBARCODE
    End Enum

    Private SmartCardTestType As TEST_TYPE
    Private _cardTypes As ArrayList = Nothing

    Private _test As String
    Private _testChild As String

    Public Sub New()

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        _job = New Job()
        _graphics = New ZMotifGraphics()
        _cardTypes = New ArrayList()
    End Sub

    ' This method is required to prevent memory leaks with the ZMotif Printer SDK
    Private Sub ReleaseInterface()

        Do While (Marshal.FinalReleaseComObject(_job) > 0)

            Thread.Sleep(10)
        Loop
    End Sub

#Region "Properties"
    Private _connectionType As String
    Public ReadOnly Property ConnectionType() As String
        Get
            _connectionType = cboPrn.Text
            Return _connectionType
        End Get
    End Property

#End Region

#Region "Main Form Methods & Events"
    Private Sub MainForm_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            'uncomment these lines to retrieve SDK version information
            'Text += GetPrinterSDKVersion()
            'Text += GetGraphicsSDKVersion()

            PerformPrinterFind()

            LoadPrintImages()

            _startup = True

        Catch ex As Exception
            MessageBox.Show(ex.Message, "MainForm_Load threw exception")
        Finally
            rbUSB.Checked = True
            cboPrn.Enabled = True
        End Try
    End Sub

    Private Sub MainForm_FormClosing(ByVal sender As System.Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles MyBase.FormClosing
        Try
            CloseConnectionToPrinter()
        Catch

        Finally
            ReleaseInterface()

            _job = Nothing
            _graphics = Nothing
            _cardTypes = Nothing
        End Try
    End Sub

    Private Sub cbIPAddr_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Try
            If (Not _startup) Then
                GetSelectedPrinterConfiguration()
            End If
        Finally
            _startup = False
        End Try
    End Sub

    Private Sub cboPrn_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Try
            If (Not _startup) Then
                GetSelectedPrinterConfiguration()
            End If
        Finally
            _startup = False
        End Try
    End Sub

    Private Sub btnFindPrinters_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnFindPrinters.Click
        PerformPrinterFind()
    End Sub

    Private Sub btnRun_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnRun.Click
        Try
            Cursor = Cursors.WaitCursor
            btnRun.Enabled = False

            PerformSelectedTest(_test, RetrieveCardSelection())

        Finally
            Cursor = Cursors.Default
            btnRun.Enabled = True
        End Try
    End Sub

    Private Sub btnExit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnExit.Click
        Close()
    End Sub

    Private Sub btnCancelJob_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCancelJob.Click
        Try
            CancelJob(0)
        Finally
            btnCancelJob.Enabled = False
        End Try
    End Sub

    Private Sub rbUSB_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rbUSB.CheckedChanged
        cboPrn.Enabled = rbUSB.Checked
    End Sub

    Private Sub rbEthernet_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs)
        cboPrn.Enabled = False
    End Sub

    Private Sub EnableCommandButtons()
        btnRun.Enabled = True
        btnExit.Enabled = True
        rbUSB.Enabled = True
    End Sub

    Private Sub EnableCancelButton()
        btnCancelJob.Enabled = True
    End Sub

    Private Sub SetStatusLabelVisible()
        lblStatus.Visible = True
    End Sub

    Private Sub LoadPrintImages()
        Try
            Dim StartupPath As String = Application.StartupPath
            Dim len As Integer = StartupPath.Length

            If (StartupPath.LastIndexOf("\") <> len - 1) Then
                StartupPath += "\"
            End If

            _bmpFront = ImageToByteArray(StartupPath + "ZXPFront.bmp")
            _bmpBack = ImageToByteArray(StartupPath + "ZXPBack.bmp")

        Catch ex As Exception
            MessageBox.Show(ex.Message, "LoadPrintImages threw exception")
        End Try
    End Sub

    Private Function ImageToByteArray(ByVal filename As String) As Byte()

        Dim ms As System.IO.MemoryStream = Nothing
        Try
            Dim img As Image = System.Drawing.Image.FromFile(filename)

            ms = New System.IO.MemoryStream()

            img.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp)

            Return ms.ToArray()

        Catch nEx As System.ArgumentNullException
            MessageBox.Show(nEx.Message, "ImageByteArray null argument exception")

        Catch Eex As System.Runtime.InteropServices.ExternalException
            MessageBox.Show(Eex.Message, "ImageByteArray ExternalException exception")

        Catch ex As Exception
            MessageBox.Show(ex.Message, "ImageToByteArray threw exception ")

        Finally
            ms = Nothing
        End Try

        Return Nothing
    End Function

    Private Sub PerformPrinterFind()
        Try
            cboPrn.Items.Clear()

            If (LocatePrinters()) Then
                GetSelectedPrinterConfiguration()
            Else
                cboPrn.Items.Clear()
                MessageBox.Show("No ZMotif Printers found on local USB port")
            End If

                RefreshTheForm()
        Catch ex As Exception
            MessageBox.Show(ex.Message, "PeformPrinterFind exception")
        End Try
    End Sub

    Private Sub GetSelectedPrinterConfiguration()
        Try
            Dim isContactless, isMag As Boolean
            Dim errMsg As String = String.Empty

            If (GetPrinterConfiguration(isContactless, isMag)) Then
                If (isContactless AndAlso rbUSB.Checked) Then
                    WinSCard.GetPCSCReaders(_contactlessReader, _contactReader, errMsg)
                End If

                If (Not ConfigureApplication(isMag)) Then
                    MessageBox.Show("Failed to configure application")
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message, "GetSelectedPrinterConfiguration")
        End Try
    End Sub

    Private Function LocatePrinters() As Boolean
        Try
            cboPrnInit()

            If (rbUSB.Checked) Then
                If cboPrn.Text.Length > 0 Then
                    Return True
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message, "LocatePrinters exception")
        End Try
        Return False
    End Function

    Private Sub cboPrnInit()
        Try
            Cursor = Cursors.WaitCursor

            cboPrn.Text = String.Empty
            cboPrn.Items.Clear()
            cboPrn.Refresh()

            Dim portType As ZMOTIFPRINTERLib.ConnectionTypeEnum = GetPrinterPortType()

            Dim objList As Object = GetPrinters(portType)

            If (Not (objList Is Nothing) AndAlso (Not String.IsNullOrEmpty(Convert.ToString(objList)))) Then
                Dim arrlstPrinters As ArrayList = ArrayList.Adapter(objList)

                If (rbUSB.Checked) Then

                    For Each obj As Object In arrlstPrinters
                        cboPrn.Items.Add(Convert.ToString(obj))
                    Next
                    cboPrn.Text = Convert.ToString(cboPrn.Items(0).ToString())
                End If
            End If
        Catch Aex As System.ArgumentException
            MessageBox.Show(Aex.Message, "cboPrnInit threw exception: Could not locate printer serial no")
            cboPrn.Items.Clear()
        Catch
            cboPrn.Items.Clear()
        Finally
            Cursor = Cursors.Default
        End Try
    End Sub

    Private Function GetPrinterConfiguration(ByRef isContactless As Boolean, ByRef isMag As Boolean) As Boolean
        Try
            Dim errMsg As String = String.Empty

            Dim deviceName As String = ConnectionType

            GetPrinterConfiguration(deviceName, isContactless, isMag, errMsg)
            If (String.IsNullOrEmpty(errMsg)) Then
                Return True
            Else
                MessageBox.Show("Get Printer Configuration Error: " + errMsg)
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message, "GetPrinterConfiguration threw exception")
        End Try
        Return False
    End Function

    Private Function ConfigureApplication(ByVal isMag As Boolean) As Boolean
        Try
            tvTestsInit(_contactlessReader, _contactReader, isMag)

            Dim result As Integer = CardTypesInit(ConnectionType)

            If (result = 0) Then 'ZXP8
                cboCardTypesInit(TEST_TYPE.PRINTING)
            End If

            Return True
        Catch ex As Exception
            MessageBox.Show(ex.Message, "ConfigureZGT threw exception")
        End Try
        Return False
    End Function

    Private Sub tvTestsInit(ByVal contactlessReader As String, ByVal contactReader As String, _
                            ByVal isMag As Boolean)

        tvTests.Nodes.Clear()

        Dim iNode As Byte = 0
        Dim bContactContactlessSupported As Boolean = False

        tvTests.Nodes.Add("Print")
        tvTests.Nodes(iNode).Nodes.Add("Single Side")
        tvTests.Nodes(iNode).Nodes.Add("Dual Side")
        iNode += 1

        If (isMag) Then
            tvTests.Nodes.Add("Magnetic")
            tvTests.Nodes(iNode).Nodes.Add("Single Side")
            tvTests.Nodes(iNode).Nodes.Add("Dual Side")
            tvTests.Nodes(iNode).Nodes.Add("Encode Only")
            iNode += 1
        End If

        If (rbUSB.Checked) Then
            If (Not String.IsNullOrEmpty(contactlessReader)) Then
                tvTests.Nodes.Add("Contactless")
                tvTests.Nodes(iNode).Nodes.Add("Single Side")
                tvTests.Nodes(iNode).Nodes.Add("Dual Side")
                tvTests.Nodes(iNode).Nodes.Add("Encode Only")
                iNode += 1
            End If
        End If
    End Sub

    Private Sub tvTests_AfterSelect(ByVal sender As Object, ByVal e As System.Windows.Forms.TreeViewEventArgs) Handles tvTests.AfterSelect
        Try
            Dim fp As String = tvTests.SelectedNode.FullPath

            Select Case (fp)
                Case "Print", "Print\Dual Side"
                    If (_test <> "Print") Then
                        cboCardTypesInit(TEST_TYPE.PRINTING)
                        _test = "Print"
                    End If
                    _testChild = "Dual Side"
                    LblTestSet("Print", "Dual Side")

                Case "Print\Single Side"
                    If (_test <> "Print") Then
                        cboCardTypesInit(TEST_TYPE.PRINTING)
                        _test = "Print"
                    End If
                    _testChild = "Single Side"
                    LblTestSet("Print", "Single Side")

                Case "Magnetic", "Magnetic\Encode Only"
                    If (_test <> "Magnetic") Then
                        cboCardTypesInit(TEST_TYPE.MAG)
                        _test = "Magnetic"
                    End If
                    _testChild = "Encode Only"
                    LblTestSet("Magnetic", "Encode Only")

                Case "Magnetic\Single Side"
                    If (_test <> "Magnetic") Then
                        cboCardTypesInit(TEST_TYPE.MAG)
                        _test = "Magnetic"
                    End If
                    _testChild = "Single Side"
                    LblTestSet("Magnetic", "Single Side")

                Case "Magnetic\Dual Side"
                    If (_test <> "Magnetic") Then
                        cboCardTypesInit(TEST_TYPE.MAG)
                        _test = "Magnetic"
                    End If
                    _testChild = "Dual Side"
                    LblTestSet("Magnetic", "Dual Side")

                Case "Contactless", "Contactless\Encode Only"
                    If (_test <> "Contactless") Then
                        cboCardTypesInit(TEST_TYPE.CONTACTLESS)
                        _test = "Contactless"
                    End If
                    _testChild = "Encode Only"
                    LblTestSet("Contactless", "Encode Only")

                Case "Contactless\Single Side"
                    If (_test <> "Contactless") Then
                        cboCardTypesInit(TEST_TYPE.CONTACTLESS)
                        _test = "Contactless"
                    End If
                    _testChild = "Single Side"
                    LblTestSet("Contactless", "Single Side")

                Case "Contactless\Dual Side"
                    If (_test <> "Contactless") Then
                        cboCardTypesInit(TEST_TYPE.CONTACTLESS)
                        _test = "Contactless"
                    End If
                    _testChild = "Dual Side"
                    LblTestSet("Contactless", "Dual Side")
            End Select
        Catch ex As Exception
        Finally
            btnRun.Enabled = True
        End Try
    End Sub

    Private Sub LblTestSet(ByVal test As String, ByVal testChild As String)
        lblTest.Text = test
        lblTest.Refresh()

        lblTestChild.Text = testChild
        lblTestChild.Refresh()
    End Sub

    Private Function CardTypesInit(ByVal deviceName As String) As Integer
        Try
            _cardTypes.Clear()

            If (Not OpenConnectionToPrinter()) Then
                Cursor = Cursors.Default
                MessageBox.Show("Unable to open device [" + deviceName + "]")
                Return 0
            End If

            If (IsZXP7()) Then
                Return 1
            End If

            Dim objList As Object = GetAvailableCards()

            If (Not objList Is Nothing) Then
                Dim array As Array = CType(objList, Array)

                For Each obj As Object In array
                    _cardTypes.Add(Convert.ToString(obj))
                Next
                Return _cardTypes.Count
            Else
                MessageBox.Show("No ZMotif devices found")
                RefreshTheForm()
            End If
        Catch ex As Exception
            MessageBox.Show("cboCardTypesInit Exception: " + ex.Message)
        Finally
            CloseConnectionToPrinter()
        End Try
        Return 0
    End Function

    Private Sub cboCardTypesInit(ByVal testType As TEST_TYPE)
        Try
            Cursor = Cursors.WaitCursor

            cboCardTypes.Items.Clear()

            Dim first As Boolean = True
            Dim cardType As String = String.Empty

            For i As Integer = 0 To _cardTypes.Count - 1

                cardType = _cardTypes(i).ToString()

                If (cardType.ToLower().Contains("custom")) Then
                    cboCardTypes.Items.Add(cardType)
                Else
                    Select Case (testType)

                        Case TEST_TYPE.CONTACTLESS, TEST_TYPE.CONTACTLESSwithBARCODE
                            If (cardType.ToLower().Contains("mifare")) Then
                                cboCardTypes.Items.Add(cardType)
                                If (first) Then
                                    cboCardTypes.Text = cardType
                                    first = False
                                End If
                            End If

                        Case TEST_TYPE.MAGwithBARCODE, TEST_TYPE.MAGwithEIN, TEST_TYPE.MAG
                            If (cardType.ToLower().Contains("loco") OrElse cardType.ToLower().Contains("hico")) Then
                                cboCardTypes.Items.Add(cardType)
                                If (first) Then
                                    cboCardTypes.Text = cardType
                                    first = False
                                End If
                            End If

                        Case TEST_TYPE.PRINTINGwithBARCODE, TEST_TYPE.PRINTINGwithEIN, TEST_TYPE.PRINTING

                            Select Case (cardType.ToLower())
                                Case "pvc", "polycarbonate", "pvc,composite", "pvc,composite,z6", _
                                      "teslin,composite", "pet", "petg", "abs"
                                    cboCardTypes.Items.Add(cardType)
                                    If (first) Then
                                        cboCardTypes.Text = cardType
                                        first = False
                                    End If
                            End Select
                    End Select
                End If
            Next i

        Catch ex As Exception

        Finally
            Cursor = Cursors.Default
        End Try
    End Sub

    Private Sub RefreshTheForm()
        Try
            Refresh()
        Catch ex As Exception
            MessageBox.Show(ex.Message, "RefreshTheForm threw exception")
        End Try
    End Sub

    Private Function RetrieveCardSelection() As String
        Try
            Return cboCardTypes.Text
        Catch ex As Exception
            LogAppend("RetrieveCardSelection threw exception: " + ex.Message)
        End Try
        Return ""
    End Function

    Private Sub LogAppend(ByVal logData As String)
        Try
            Dim tm As String = DisplayDateTime()

            logData = RemoveCRLF(logData)

            tbLog.AppendText(tm + " : " + logData + vbCrLf)

        Catch ex As Exception
            MessageBox.Show(ex.Message, "LogAppend threw exception")
        End Try
    End Sub

    Private Function DisplayDateTime() As String

        Dim dt As DateTime = DateTime.Now
        Dim d As String = dt.ToString("MM/dd/yyyy")
        Dim t As String = dt.ToString("HH:mm:ss")

        Return d + "  " + t
    End Function

    Private Function RemoveCRLF(ByVal strIn As String) As String

        Dim strOut As String = String.Empty
        Try
            Dim buffer As Char() = strIn.ToCharArray

            For Each item As Char In buffer
                If ((item <> vbCr) AndAlso (item <> vbLf)) Then
                    strOut += Convert.ToChar(item)
                End If
            Next
        Catch ex As Exception
            MessageBox.Show(ex.Message, "RemoveCRLF threw exception")
        End Try
        Return strOut

    End Function

    Private Sub lblStatusUpdate(ByVal status As String)
        Select Case (status)

            Case "alarm_handling"
                UpdateStatusLabel("alarm handling")

            Case "idle"
                UpdateStatusLabel("ready")

            Case "mag_ops"
                UpdateStatusLabel("magnetic operation")

            Case "printing_cooling", "xfer_rollers_cooling"
                UpdateStatusLabel("cooling")

            Case "printing_heating", "xfer_rollers_heating"
                UpdateStatusLabel("warming")

            Case Else
                UpdateStatusLabel(status)
        End Select
    End Sub

    Private Sub UpdateStatusLabel(ByVal status As String)
        Try
            lblStatus.Text = status
            If (String.IsNullOrEmpty(status)) Then
                lblStatus.Visible = False
            Else
                lblStatus.Visible = True
                lblStatus.Refresh()
            End If
        Catch ex As Exception
            LogAppend("UpdateStatusLabel threw exception: " + ex.Message)
        End Try
    End Sub

    Private Sub PerformSelectedTest(ByVal testType As String, ByVal cardType As String)
        Try
            btnCancelJob.Enabled = True

            Select Case (testType)
                Case "Print"
                    PerformPrintTest(cardType, 1)

                Case "Magnetic"
                    PerformMagneticTest(cardType, 1)

                Case "Contactless"
                    PerformContactlessTest(cardType, 1)

                Case Else
                    LogAppend("Test [" + testType + "] Not Supported")
            End Select
        Catch ex As Exception
            MessageBox.Show(ex.Message, "PerformSelectedTest threw exception")
        Finally
            btnCancelJob.Enabled = False
            CloseConnectionToPrinter()
        End Try
    End Sub

    Private Function CheckAlarm() As Short
        Dim alarm As Integer = 0
        Dim result As Short
        Try
            If (alarm <> _alarm) Then
                alarm = GetAlarm()
                result = CType(alarm, Short)
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message, "CheckAlarm threw exception. Defaulting to no alarm condition")
            result = 0
        End Try
        Return result
    End Function

    Private Sub PrepareToStartTest(ByVal test As String, ByVal testType As String)
        Try
            _abort = False

            LogAppend("Start " + test + " " + testType)

            lblStatusUpdate("")
            SetStatusLabelVisible()

        Catch ex As Exception
            MessageBox.Show(ex.Message, "PrepareToStartTest threw exception")
        End Try
    End Sub

#Region "Print Tests"

    Private Sub PerformPrintTest(ByVal CardType As String, ByVal continueCount As String)
        Try
            Dim errCount As Integer = 0
            Dim cardCount As Integer = 1
            Dim actionID As Integer = 0
            Dim alarm As Short = 0
            Dim printerStatus As String = String.Empty
            Dim errMsg As String = String.Empty
            Dim TestInProgress As Boolean = False

            PrepareToStartTest(_test, _testChild)

            If Not (OpenConnectionToPrinter()) Then
                Return
            End If

            If Not (AssignSourceDestinationLocationsToJob()) Then
                Return
            End If

            While (True)
                If (_abort) Then
                    Exit While
                End If

                If (IsPrinterReady(printerStatus, errMsg, alarm)) Then

                    alarm = CheckAlarm()

                    If (_alarm = 0) Then
                        DeterminePrintTestType(_testChild, CardType, actionID, errMsg, alarm)

                        TestInProgress = True

                        cardCount = DetermineNextStepPrintTest(cardCount, CardType, errMsg, _
                                                               actionID, alarm, _testChild, _
                                                               errCount)
                    Else
                        alarm = CheckAlarmValue(alarm, cardCount)
                    End If

                ElseIf (PrinterInAlarmState()) Then

                    If (Not TestInProgress) Then
                        EnableCommandButtons()
                        Exit While
                    End If
                End If

                If (cardCount > continueCount) Then

                    Exit While

                ElseIf (errMsg.Contains("User chose")) Then

                    LogAppend("User chose to end " + _test + " " + _testChild + " test at Cards = " + cardCount.ToString() _
                               + " due to printer alarm condition")
                    Exit While
                End If
                Application.DoEvents()
                Thread.Sleep(500)
            End While

            cardCount -= 1
            LogAppend("End " + _test + " " + _testChild + ": Cards = " + cardCount.ToString() _
                      + " Errors = " + errCount.ToString())

        Catch ex As Exception
            MessageBox.Show(ex.Message, "PerformPrintTest threw exception")
        Finally
            lblStatusUpdate("")
            CloseConnectionToPrinter()
        End Try
    End Sub

    Private Sub DeterminePrintTestType(ByVal testType As String, ByVal cardType As String, _
                                       ByRef actionID As Integer, ByRef errMsg As String, _
                                       ByRef alarm As Short)
        Try
            If (testType = "Single Side") Then
                PrintJob(actionID, 1, cardType, _bmpFront, Nothing, errMsg, alarm)
            Else 'Dual Side
                PrintJob(actionID, 1, cardType, _bmpFront, _bmpBack, errMsg, alarm)
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message, "DeterminePrintTestType threw exception")
        End Try
    End Sub

    Private Function DetermineNextStepPrintTest(ByVal cardCount As Integer, ByVal smartCardType As String, _
                                                ByRef errMsg As String, ByVal actionID As Integer, ByVal alarm As Short, _
                                                ByVal type As String, ByRef errCount As Integer) As Integer
        Try
            Dim logMsg As String = String.Empty

            If (String.IsNullOrEmpty(errMsg)) Then

                JobWait(actionID, TEST_TYPE.PRINTING, 60, errMsg, alarm, (type = "Encode Only"))

                If ((String.IsNullOrEmpty(errMsg) OrElse errMsg = "done_ok" OrElse errMsg = "card exiting to eject bin" _
                    OrElse errMsg = "card exiting to reject bin" OrElse errMsg = "card exiting to feeder") _
                    AndAlso ((alarm = 0) OrElse (alarm = 4016))) Then

                    logMsg = "Printing Passed: Card " + cardCount.ToString()
                    LogAppend(logMsg)


                ElseIf (alarm = CARD_NOT_DETECTED) Then
                    CancelJob(actionID)
                    logMsg = "Printing Error: " + errMsg + ": Card " + cardCount.ToString()
                    LogAppend(logMsg)
                    errCount += 1
                End If
            Else
                CancelJob(actionID)
                logMsg = "Printing Error: " + errMsg + ": Card " + cardCount.ToString()
                LogAppend(logMsg)
                TerminateJob()
                EnableCommandButtons()
                errCount += 1
            End If

        Catch ex As Exception
            errCount += 1
            LogAppend("DetermineNextStepPrintTest threw exception: " + ex.Message)
        Finally
            cardCount += 1
        End Try

        Return cardCount
    End Function

    Private Function CheckAlarmValue(ByVal alarm As Short, ByRef cardCount As Integer) As Short
        Try
            If ((alarm <> 0) AndAlso (alarm <> _alarm)) Then

                Dim logMsg As String = "Alarm (" + alarm.ToString() + ") Card " + cardCount.ToString()

                LogAppend(logMsg)
                _alarm = CType(GetAlarm(), Short)

            End If

        Catch ex As Exception
            MessageBox.Show(ex.Message, "CheckAlarmValue threw exception. Defaulting to no alarm condition")
            alarm = 0
        End Try

        Return alarm
    End Function

    Private Function PrinterInAlarmState() As Boolean
        Try

            GetAlarm()

            If ((_alarm = 0) OrElse (_alarm = OUT_OF_CARDS)) Then
                Return False
            End If

            MessageBox.Show("Cannot Run Test, Printer is in Alarm Condition")
            RefreshTheForm()

        Catch ex As Exception
            MessageBox.Show(ex.Message, "PrinterInAlarmState threw exception")
        End Try

        Return True
    End Function

#End Region 'Print tests

#Region "Magnetic Tests"

    Private Sub PerformMagneticTest(ByVal CardType As String, ByVal continueCount As Integer)
        Try
            Dim cardCount As Integer = 1
            Dim errCount As Integer = 0
            Dim actionID As Integer = 0
            Dim alarm As Short = 0
            Dim errMsg As String = String.Empty
            Dim printerStatus As String = String.Empty
            Dim TestInProgress As Boolean = False

            PrepareToStartTest(_test, _testChild)

            If Not (OpenConnectionToPrinter()) Then
                Return
            End If

            If Not (AssignSourceDestinationLocationsToJob()) Then
                Return
            End If

            While (True)
                If (_abort) Then
                    Exit While
                End If

                If (IsPrinterReady(printerStatus, errMsg, alarm) _
                    OrElse _testChild = "Encode Only") Then

                    alarm = CheckAlarm()

                    If (_alarm = 0) Then

                        DetermineMagneticTestType(_testChild, CardType, actionID, errMsg, alarm)

                        TestInProgress = True
                        If (String.IsNullOrEmpty(errMsg)) Then

                            cardCount = DetermineNextStepMagneticTest(cardCount, CardType, errMsg, actionID, _
                                                                      alarm, _testChild, errCount)
                        Else
                            ProcessMagneticTestError(cardCount, errMsg, errCount)
                        End If
                    Else
                        alarm = CheckAlarmValue(alarm, cardCount)
                    End If

                ElseIf (PrinterInAlarmState()) Then
                    If (Not TestInProgress) Then
                        EnableCommandButtons()
                        Exit While
                    End If

                    If (errMsg.Contains("User chose")) Then
                        LogAppend("User chose to end " + _test + " " + _testChild + " test at Cards = " _
                                  + cardCount.ToString() + " due to printer alarm condition")
                        Exit While
                    End If
                End If

                If (cardCount > continueCount) Then
                    Exit While
                End If

                Application.DoEvents()
                Thread.Sleep(500)
            End While

            cardCount -= 1
            LogAppend("End " + _test + " " + _testChild + ": Cards = " + cardCount.ToString() _
                      + " Errors = " + errCount.ToString())

        Catch ex As Exception
            MessageBox.Show(ex.Message, "PerformMagneticTest threw exception")
        Finally
            lblStatusUpdate("")
            CloseConnectionToPrinter()
        End Try
    End Sub

    Private Sub DetermineMagneticTestType(ByVal testType As String, ByVal cardType As String, _
                                          ByRef actionID As Integer, ByRef errMsg As String, _
                                          ByRef alarm As Short)
        Try
            Dim magheadType As MagEncodingTypeEnum = MagEncodingTypeEnum.ISO
            Dim Track1 As String = "ABCDEFGHIJKLM"
            Dim Track2 As String = "0123456789"
            Dim Track3 As String = "9876543210"


            Select Case (testType)

                Case "Single Side"
                    PrintWithMagJob(actionID, cardType, Track1, _
                                         Track2, Track3, _bmpFront, Nothing, _
                                         errMsg, alarm, magheadType)

                Case "Dual Side"
                    PrintWithMagJob(actionID, cardType, Track1, _
                                             Track2, Track3, _bmpFront, _bmpBack, _
                                             errMsg, alarm, magheadType)

                Case Else 'Encode Only
                    PrintWithMagJob(actionID, cardType, Track1, _
                                    Track2, Track3, Nothing, Nothing, errMsg, _
                                    alarm, magheadType)
            End Select

        Catch ex As Exception
            LogAppend("DetermineMagneticTestType threw exception: " + ex.Message)
        End Try
    End Sub

    Private Function DetermineNextStepMagneticTest(ByVal cardCount As Integer, ByVal smartCardType As String, _
                                                   ByRef errMsg As String, ByVal actionID As Integer, ByVal alarm As Short, _
                                                   ByRef type As String, ByRef errCount As Integer) As Integer
        Try
            Dim logMsg As String = String.Empty

            JobWait(actionID, TEST_TYPE.MAG, 120, errMsg, alarm, (type = "Encode Only"))

            If ((String.IsNullOrEmpty(errMsg) OrElse errMsg = "done_ok" OrElse errMsg = "card exiting to eject bin" _
                OrElse errMsg = "card exiting to reject bin" OrElse errMsg = "card exiting to feeder") _
                AndAlso ((alarm = 0) OrElse (alarm = 4016))) Then

                logMsg = "Magnetic Job Passed: Card " + cardCount.ToString()
                LogAppend(logMsg)

            ElseIf (alarm = CARD_NOT_DETECTED OrElse alarm = EP_SCRIPT_ERROR) Then
                logMsg = "Printing Error: " + errMsg + ": Card " + cardCount.ToString()
                LogAppend(logMsg)
                errCount += 1
                EnableCancelButton()

            ElseIf (alarm <> 0) Then
                logMsg = "Alarm (" + alarm.ToString() + ") Card " + cardCount.ToString()
                LogAppend(logMsg)

                GetAlarm()
                If (alarm >= 9000 AndAlso alarm <= 9999) Then

                    TerminateJob()
                    errCount += 1

                ElseIf (alarm = 13003) Then 'job id not found
                    errCount += 1
                Else
                    logMsg = "Magnetic Job Wait Error: " + errMsg + ": Card " + cardCount.ToString()
                    LogAppend(logMsg)

                    TerminateJob()
                    errCount += 1
                End If
            End If

        Catch ex As Exception
            errCount += 1
            LogAppend("DetermineNextStepMagneticTest threw exception: " + ex.Message)
        Finally
            cardCount += 1
        End Try

        Return cardCount
    End Function

    Private Sub ProcessMagneticTestError(ByVal cardCount As Integer, ByVal errMsg As String, _
                                         ByRef errCount As Integer)
        Try
            Dim logMsg As String = "Magnetic Encoding Error: " + errMsg + ": Card " _
                                    + cardCount.ToString()
            LogAppend(logMsg)

            TerminateJob()

            errCount += 1

        Catch ex As Exception
            MessageBox.Show(ex.Message, "ProcessMagneticTestError threw exception")
        End Try
    End Sub

#End Region 'Magnetic Tests

#Region "Contactless Tests"

    Private Sub PerformContactlessTest(ByVal CardType As String, ByVal continueCount As Integer)
        Dim contactless As ZMotifOpsContactless = Nothing
        Try
            Dim cardCount As Integer = 1
            Dim errCount As Integer = 0
            Dim actionID As Integer = 0
            Dim alarm As Short = 0
            Dim errMsg As String = String.Empty
            Dim printerStatus As String = String.Empty
            Dim smartCardType As String = String.Empty
            Dim TestInProgress As Boolean = False

            Dim key As Byte() = {&HFF, &HFF, &HFF, &HFF, &HFF, &HFF}

            PrepareToStartTest(_test, _testChild)

            If Not (OpenConnectionToPrinter()) Then
                Return
            End If

            If Not (AssignSourceDestinationLocationsToJob()) Then
                Return
            End If

            contactless = New ZMotifOpsContactless()

            While (True)
                If (_abort) Then
                    Exit While
                End If

                If (IsPrinterReady(printerStatus, errMsg, alarm) _
                    OrElse _testChild = "Encode Only") Then

                    alarm = CheckAlarm()

                    If (_alarm = 0) Then
                        DetermineContactlessTestType(_testChild, CardType, actionID, alarm, errMsg)

                        If (String.IsNullOrEmpty(errMsg)) Then 'encode the contactless card
                            contactless.MifareTest(smartCardType, _contactlessReader, key, _
                                                   True, errMsg)
                            TestInProgress = True

                            If (String.IsNullOrEmpty(errMsg)) Then
                                DetermineNextStepContactlessTest(cardCount, smartCardType, _
                                                                 errMsg, actionID, alarm, _
                                                                 _testChild, errCount)
                            Else
                                Dim logMsg As String = "Mifare Test Error: " + errMsg + ": Card " + cardCount.ToString()
                                LogAppend(logMsg)
                                errCount += 1
                                TerminateJob()
                            End If

                            cardCount += 1
                        End If
                        alarm = CheckAlarmValue(alarm, cardCount)

                    ElseIf (PrinterInAlarmState()) Then

                        If (Not TestInProgress) Then
                            EnableCommandButtons()
                            Exit While
                        End If
                    Else
                        alarm = CheckAlarm()
                    End If

                    If (cardCount > continueCount) Then
                        Exit While
                    ElseIf (errMsg.Contains("User chose")) Then
                        LogAppend("User chose to end " + _test + " " + _testChild _
                                   + " test at Cards = " + cardCount.ToString() _
                                   + " due to printer alarm condition")
                        Exit While
                    End If

                    Application.DoEvents()
                    Thread.Sleep(1000)
                End If
            End While

            cardCount -= 1
            LogAppend("End " + _test + " " + _testChild + ": Cards = " + cardCount.ToString() _
                      + " Errors = " + errCount.ToString())

            lblStatusUpdate("")

        Catch ex As Exception
            MessageBox.Show(ex.Message, "PerformContactlesTests threw exception")
        Finally
            CloseConnectionToPrinter()
            contactless = Nothing
        End Try
    End Sub

    Private Sub DetermineContactlessTestType(ByVal testType As String, ByVal cardType As String, _
                                             ByRef actionID As Integer, ByRef alarm As Short, _
                                             ByRef errMsg As String)
        Try
            Select Case (testType)

                Case "Single Side"
                    ContactlessJob(actionID, cardType, _bmpFront, Nothing, errMsg, alarm)

                Case "Dual Side"
                    ContactlessJob(actionID, cardType, _bmpFront, _bmpBack, errMsg, alarm)

                Case Else 'Encode Only
                    ContactlessJob(actionID, cardType, Nothing, Nothing, errMsg, alarm)
            End Select

        Catch ex As Exception
            MessageBox.Show(ex.Message, "DetermineContactlessTestType threw exception")
        End Try
    End Sub

    Private Function DetermineNextStepContactlessTest(ByVal cardCount As Integer, ByVal smartCardType As String, _
                                                      ByRef errMsg As String, ByVal actionID As Integer, _
                                                      ByVal alarm As Short, ByVal type As String, _
                                                      ByRef errCount As Integer) As Integer
        Try
            Dim logMsg As String = String.Empty

            ResumeJob()
            JobWait(actionID, TEST_TYPE.CONTACTLESS, 60, errMsg, alarm, (type = "Encode Only"))

            If ((String.IsNullOrEmpty(errMsg) OrElse errMsg = "done_ok" OrElse errMsg = "card exiting to eject bin" _
                OrElse errMsg = "card exiting to reject bin" OrElse errMsg = "card exiting to feeder") _
                AndAlso ((alarm = 0) OrElse (alarm = 4016))) Then

                logMsg = smartCardType + " Passed: Card " + cardCount.ToString()
                LogAppend(logMsg)

            ElseIf (alarm = CARD_NOT_DETECTED) Then
                logMsg = "Printing Error: " + errMsg + ": Card " + cardCount.ToString()
                LogAppend(logMsg)
                errCount += 1

            Else
                logMsg = smartCardType + " Job Wait Error: " + errMsg + ": Card " + cardCount.ToString()
                LogAppend(logMsg)
                errCount += 1
            End If

        Catch ex As Exception
            errCount += 1
            LogAppend("DetermineNextStepContactlessTest threw exception: " + ex.Message)
        Finally
            cardCount += 1
        End Try

        Return cardCount
    End Function

#End Region 'Contactless Tests

#End Region 'Main Form Methods & Events

#Region "ZMotifPrinter SDK"

    '***************************************************
    '  Name: CancelJob
    '  Purpose: Demonstrates how to cancel all pending jobs
    '
    '  Inputs:  None
    '  Outputs: None 
    '***************************************************
    Private Sub CancelJob(ByVal actionID As Integer)
        Try
            _job.JobCancel(actionID) '0 = cancel all jobs

        Catch ex As Exception
            MessageBox.Show(ex.Message, "CancelJob threw exception")
        End Try
    End Sub

    '***************************************************
    '  Name: TerminateJob
    '  Purpose: Demonstrates how to terminate a job,
    '           eject the card, and clear any error
    '           messages from the OCP panel
    '
    '  Inputs:  None
    '  Outputs: None 
    '***************************************************
    Private Sub TerminateJob()
        Try
            _job.JobAbort(True)
            '_job.ClearError()

        Catch ex As Exception
            MessageBox.Show(ex.Message, "TerminateJob threw exception")
        End Try
    End Sub

    '***************************************************
    '  Name: ResumeJob
    '  Purpose: Demonstrates how to resume a suspended job,
    '           after smart card encoding
    '
    '  Inputs:  None
    '  Outputs: None 
    '***************************************************
    Private Sub ResumeJob()
        Try
            _job.JobResume()

        Catch ex As Exception
            MessageBox.Show(ex.Message, "ResumeJob threw exception")
        End Try
    End Sub

    '***************************************************
    '  Name: GetPrinterSDKVersion
    '  Purpose: Demonstrates how to retrieve SDK version
    '           from ZMotifPrinter.dll
    '
    '  Inputs:  None
    '  Outputs: Version of SDK returned as string 
    '***************************************************
    Private Function GetPrinterSDKVersion() As String
        Try
            Dim major As Byte
            Dim minor As Byte
            Dim build As Byte
            Dim revision As Byte

            _job.GetSDKVersion(major, minor, build, revision)

            Dim temp As String = "         ZMotifPrinterSDK version: "

            temp += Convert.ToString(major)
            temp += "."
            temp += Convert.ToString(minor)
            temp += "."
            temp += Convert.ToString(build)
            temp += "."
            temp += Convert.ToString(revision)

            Return temp

        Catch ex As Exception

            MessageBox.Show(ex.Message, "GetPrinterSDKVersion threw exception")
        End Try

        Return ""
    End Function

    '***************************************************
    '  Name: OpenConnectionToPrinter
    '  Purpose: Demonstrates how to open connection to
    '           printer
    '
    '  Inputs:  None
    '  Outputs: True  - connection open
    '           False - failed to open connection
    '***************************************************
    Private Function OpenConnectionToPrinter() As Boolean
        Try
            If (_job.IsOpen) Then
                Return True
            End If
            Dim alarm As Short = _job.Open(ConnectionType)
            If (alarm = 0) OrElse (alarm = OUT_OF_CARDS) Then
                Return True
            ElseIf (Not _job.IsOpen) Then
                MessageBox.Show("Unable to open device [" + ConnectionType + "] alarm = " + Convert.ToString(alarm))
            Else
                Return True
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message, "OPenConnectionToPrinter threw exception: Unable to open device")
        End Try
        Return False
    End Function

    '***************************************************
    '  Name: CloseConnectionToPrinter
    '  Purpose: Demonstrates how to close connection to
    '           printer
    '
    '  Inputs:  None
    '  Outputs: True  - connection closed
    '           False - failed to close connection
    '***************************************************
    Private Function CloseConnectionToPrinter() As Boolean
        Try
            If (_job.IsOpen) Then
                _job.Close()
                Return True
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message, "CloseConnectionToPrinter threw exception")
        End Try
        Return False
    End Function

    '***************************************************
    '  Name: GetPrinters
    '  Purpose: Demonstrates how to retrieve a list of 
    '           available printers
    '
    '  Inputs:  None
    '  Outputs: Success - list of connected printers
    '           Failure - nothing
    '***************************************************
    Private Function GetPrinters(ByVal portType As ConnectionTypeEnum) As Object
        Try
            Dim objList As Object = Nothing

            _job.GetPrinters(portType, objList)

            Return objList
        Catch ex As Exception
            MessageBox.Show(ex.Message, "GetPrinters threw exception")
        End Try

        Return Nothing
    End Function

    '***************************************************
    '  Name: GetAvailableCards
    '  Purpose: Demonstrates how to retrieve supported 
    '           card types from printer
    '
    '  Inputs:  None
    '  Outputs: Success - list of supported cards
    '           Failure - nothing
    '***************************************************
    Private Function GetAvailableCards() As Object
        Try
            Dim objList As Object = Nothing



            _job.JobControl.GetAvailableCardTypes(objList)

            Return objList
        Catch ex As Exception
            MessageBox.Show(ex.Message, "GetAvailableCards threw exception")
        End Try

        Return Nothing
    End Function

    '***************************************************
    '  Name: GetPrinterConfiguration
    '  Purpose: Demonstrates how to retrieve a printer's 
    '           configuration
    '
    '  Inputs:  deviceName    - Serial Number or IP Address of printer
    '           isContactless - flag indicating contactless reader installed
    '           isMag         - flag indicating magnetic encoding print head
    '           errMsg        - returns error message if error occurs
    '  Outputs: None 
    '***************************************************
    Private Sub GetPrinterConfiguration(ByVal deviceName As String, ByRef isContactless As Boolean, _
                                        ByRef isMag As Boolean, ByRef errMsg As String)
        Try
            Dim commChannel As String = String.Empty
            Dim contactEncoder As String = String.Empty
            Dim contactlessEncoder As String = String.Empty

            If (OpenConnectionToPrinter()) Then

                isMag = IsMagEncoder(errMsg)

                If (String.IsNullOrEmpty(errMsg)) Then
                    _job.Device.GetSmartCardConfiguration(commChannel, contactEncoder, contactlessEncoder)

                    If (Not String.IsNullOrEmpty(contactlessEncoder)) Then
                        isContactless = True
                    End If
                End If

            Else
                errMsg = "Unable to open device [" + deviceName + "]"
            End If

        Catch ex As Exception
            MessageBox.Show(ex.Message, "GetPrinterConfiguration threw exception")
        Finally
            CloseConnectionToPrinter()
        End Try
    End Sub

    Private Function IsZXP7() As Boolean

        Dim result As Boolean = False
        Try
            Dim vendor As String = String.Empty
            Dim model As String = String.Empty
            Dim serialNo As String = String.Empty
            Dim MAC As String = String.Empty
            Dim headSerialNo As String = String.Empty
            Dim OemCode As String = String.Empty
            Dim fwVersion As String = String.Empty
            Dim mediaVersion As String = String.Empty
            Dim heaterVersion As String = String.Empty
            Dim zmotifVer As String = String.Empty

            _job.Device.GetDeviceInfo(vendor, model, serialNo, MAC, headSerialNo, OemCode, fwVersion, mediaVersion, heaterVersion, zmotifVer)

            If (model.Contains("7")) Then
                result = True
            End If
        Catch
            result = False
        End Try

        Return result
    End Function

    '***************************************************
    '  Name: GetPrinterPortType
    '  Purpose: Demonstrates how to assign & return printer 
    '           connection in SDK
    '
    '  Inputs:  None
    '  Outputs: None 
    '***************************************************
    Private Function GetPrinterPortType() As ZMOTIFPRINTERLib.ConnectionTypeEnum
        Dim portType As ZMOTIFPRINTERLib.ConnectionTypeEnum
        Try
            portType = ZMOTIFPRINTERLib.ConnectionTypeEnum.USB
        Catch ex As Exception
            portType = ConnectionTypeEnum.All
            MessageBox.Show(ex.Message, "GetPrinterPortType threw exception: defaulting to All")
        End Try
        Return portType
    End Function

    '***************************************************
    '  Name: IsMagEncoder
    '  Purpose: Demonstrates how to check printer 
    '           configuration for magnetic encoding support
    '
    '  Inputs:  errMsg - holds error message if exception 
    '                    occurs
    '  Outputs: True -  Printer supports mag encoding
    '           False - Printer does not support mag encoding
    '***************************************************
    Private Function IsMagEncoder(ByRef errMsg As String) As Boolean
        Dim headType As String = String.Empty
        Dim stripeLocation As String = String.Empty

        errMsg = String.Empty
        Try
            _job.Device.GetMagneticEncoderConfiguration(headType, stripeLocation)

            If (headType = "none" OrElse String.IsNullOrEmpty(headType)) Then
                Return False
            Else
                Return True
            End If
        Catch ex As Exception
            errMsg = "Is Mag Encoder Error: " + ex.Message.ToString()
        End Try
        Return False
    End Function

    '***************************************************
    '  Name: AssignSourceDestinationLocationsToJob
    '  Purpose: Demonstrates how to assign a card's 
    '           source and destination locations for a job
    '
    '  Inputs:  None
    '  Outputs: True  - successfully set locations
    '           False - failed to set locations
    '***************************************************
    Private Function AssignSourceDestinationLocationsToJob() As Boolean
        Try
            _job.JobControl.FeederSource = FeederSourceEnum.CardFeeder
            _job.JobControl.Destination = DestinationTypeEnum.Eject
            _job.JobControl.DeleteAfter = False

            Return True

        Catch ex As Exception
            LogAppend("AssignSourceDestinationLocationsToJob threw exception: " + ex.Message _
                       + " assigned default source and destination locations to job")
        End Try

        Return False
    End Function

    '***************************************************
    '  Name: IsPrinterReady
    '  Purpose: Demonstrates how to check printer's current 
    '           status
    '
    '  Inputs:  status - returns printer's current status
    '           errMsg - returns error message if error occurs 
    '           alarm  - returns error code 
    '  Outputs: True  - Printer is ready for a new job
    '           False - Printer is not ready for a new job
    '***************************************************
    Private Function IsPrinterReady(ByRef status As String, ByRef errMsg As String, ByRef alarm As Short) As Boolean
        alarm = 0
        errMsg = status = String.Empty

        Try
            Dim errorCode, jobsPending, jobsActive, jobsComplete As Integer
            Dim jobErrors, jobsTotal, nextActionID As Integer

            alarm = _job.Device.GetPrinterStatus(status, errorCode, jobsPending, jobsActive, jobsComplete, _
                                                 jobErrors, jobsTotal, nextActionID)
            If (status <> _status) Then
                _status = status
                lblStatusUpdate(status)
            End If

            If (alarm = 0) Then
                If (status = "idle" OrElse status = "standby" OrElse status = "xfer_rollers_heating" _
                    OrElse status = "xfer_rollers_cooling") Then
                    Return True
                End If
            End If
        Catch e As Exception
            errMsg = "Is Printer Ready Exception: " + e.Message
        End Try
        Return False
    End Function

    '***************************************************
    '  Name:    PrinterJob
    '  Purpose: Demonstrates how to perform single and 
    '               dual sided printing 
    '
    '  Inputs:  actionID - returns print job ID
    '           copies   - how many cards to print
    '           cardType - type of card being used
    '           bmpFront - image to be printed on front 
    '                      of card
    '           bmpBack  - image to be printed on back
    '                      of card (can be set to Nothing)
    '           errMsg   - returns error message if any
    '           alarm    - returns error code 
    '
    '  Outputs: True  - Job successful
    '           False - Job failed
    '***************************************************
    Private Function PrintJob(ByRef actionID As Integer, ByVal copies As Integer, ByVal cardType As String, _
                              ByVal bmpFront As Byte(), ByVal bmpBack As Byte(), ByRef errMsg As String, _
                              ByRef alarm As Short) As Boolean

        actionID = alarm = 0
        errMsg = String.Empty

        If ((bmpFront Is Nothing) AndAlso (bmpBack Is Nothing)) Then
            errMsg = "Print Error: No Images to Print"
            Return False
        End If

        Try
            _job.JobControl.CardType = cardType

            If Not (bmpFront Is Nothing) Then
                bmpFront = BuildImage(bmpFront, ZMotifGraphics.ImageOrientationEnum.Landscape, _
                                      ZMotifGraphics.RibbonTypeEnum.Color, errMsg)

                If Not (bmpFront Is Nothing) Then
                    _job.BuildGraphicsLayers(SideEnum.Front, PrintTypeEnum.Color, 0, 0, 0, -1, _
                                             GraphicTypeEnum.BMP, bmpFront)
                End If
            End If

            If Not (bmpBack Is Nothing) Then
                bmpBack = BuildImage(bmpBack, ZMotifGraphics.ImageOrientationEnum.Landscape, _
                                    ZMotifGraphics.RibbonTypeEnum.MonoK, errMsg)

                If Not (bmpBack Is Nothing) Then
                    _job.BuildGraphicsLayers(SideEnum.Back, PrintTypeEnum.MonoK, 0, 0, 0, -1, _
                                             GraphicTypeEnum.BMP, bmpBack)
                End If
            End If

            _job.PrintGraphicsLayers(copies, actionID)
            _lastActionID = actionID

            _job.ClearGraphicsLayers()

            Return True

        Catch e As Exception
            errMsg = e.Message
            _job.JobAbort(True)
        End Try
        Return False
    End Function

    '***************************************************
    '  Name:    PrinteWithMagJob
    '  Purpose: Demonstrates how to perform single and 
    '           dual sided printing with magnetic encoding 
    '
    '  Inputs:  actionID - returns print job ID
    '           cardType - type of card being used
    '           track1   - data to encode to track one
    '           track2   - data to encode to track two
    '           track3   - data to encode to track three
    '           bmpFront - image to be printed on front 
    '                      of card
    '           bmpBack  - image to be printed on back
    '                      of card (can be set to Nothing)
    '           errMsg   - returns error message if any
    '           alarm    - returns error code 
    '           MagHeadType - Type of magnetic encoding head
    '           installed in printer.
    '
    '  Outputs: True  - Job successful
    '           False - Job failed
    '***************************************************
    Private Function PrintWithMagJob(ByRef actionID As Integer, ByVal cardType As String, _
                                     ByVal track1 As String, ByVal track2 As String, _
                                     ByVal track3 As String, ByVal bmpFront As Byte(), _
                                     ByVal bmpBack As Byte(), ByRef errMsg As String, _
                                     ByRef alarm As Short, ByVal magHeadType As MagEncodingTypeEnum) As Boolean
        actionID = alarm = 0
        errMsg = String.Empty
        Try
            _job.JobControl.CardType = cardType
            _job.JobControl.MagEncodingType = magHeadType

            If (bmpBack Is Nothing AndAlso bmpFront Is Nothing) Then
                alarm = _job.MagDataOnly(1, track1, track2, track3, actionID)
                _lastActionID = actionID
            Else
                If Not (bmpFront Is Nothing) Then
                    bmpFront = BuildImage(bmpFront, ZMotifGraphics.ImageOrientationEnum.Landscape, _
                                          ZMotifGraphics.RibbonTypeEnum.Color, errMsg)

                    If Not (bmpFront Is Nothing) Then
                        _job.BuildGraphicsLayers(SideEnum.Front, PrintTypeEnum.Color, 0, 0, 0, -1, _
                                                 GraphicTypeEnum.BMP, bmpFront)
                    End If

                    If Not (bmpBack Is Nothing) Then
                        bmpBack = BuildImage(bmpBack, ZMotifGraphics.ImageOrientationEnum.Landscape, _
                                             ZMotifGraphics.RibbonTypeEnum.MonoK, errMsg)

                        If Not (bmpBack Is Nothing) Then
                            _job.BuildGraphicsLayers(SideEnum.Back, PrintTypeEnum.MonoK, 0, 0, 0, -1, _
                                                     GraphicTypeEnum.BMP, bmpBack)
                        End If
                    End If
                End If
                alarm = _job.PrintGraphicsLayersWithMagData(1, track1, track2, track3, actionID)
                _lastActionID = actionID
            End If

            If ((alarm = 0) OrElse (alarm = 4016)) Then 'no error or out of cards
                Return True
            End If

        Catch e As Exception
            errMsg = e.Message
            _job.JobAbort(True)
        Finally
            _job.ClearGraphicsLayers()
        End Try

        Return False
    End Function

    '***************************************************
    '  Name:    ContactlessJob
    '  Purpose: Demonstrates how to perform single and 
    '           dual sided printing with contactless
    '           smart card
    '
    '  Inputs:  actionID - returns print job ID
    '           cardType - type of card being used
    '           bmpFront - image to be printed on front 
    '                      of card
    '           bmpBack  - image to be printed on back
    '                      of card (can be set to Nothing)
    '           errMsg   - returns error message if any
    '           alarm    - returns error code 
    '
    '  Outputs: True  - Job successful
    '           False - Job failed
    '***************************************************
    Public Function ContactlessJob(ByRef actionID As Integer, ByRef cardType As String, _
                                   ByVal bmpFront As Byte(), ByVal bmpBack As Byte(), _
                                   ByRef errMsg As String, ByRef alarm As Short) As Boolean
        actionID = alarm = 0
        errMsg = String.Empty
        Try
            _job.JobControl.CardType = cardType
            _job.JobControl.SmartCardConfiguration(SideEnum.Front, SmartCardTypeEnum.MIFARE, True)

            If (bmpFront Is Nothing AndAlso bmpBack Is Nothing) Then
                _job.SmartCardDataOnly(1, actionID)
                _lastActionID = actionID

            Else
                If Not (bmpFront Is Nothing) Then
                    bmpFront = BuildImage(bmpFront, ZMotifGraphics.ImageOrientationEnum.Landscape, _
                                          ZMotifGraphics.RibbonTypeEnum.Color, errMsg)

                    If Not (bmpFront Is Nothing) Then
                        _job.BuildGraphicsLayers(SideEnum.Front, PrintTypeEnum.Color, 0, 0, 0, -1, _
                                                 GraphicTypeEnum.BMP, bmpFront)
                    End If
                End If

                If Not (bmpBack Is Nothing) Then
                    bmpBack = BuildImage(bmpBack, ZMotifGraphics.ImageOrientationEnum.Landscape, _
                                         ZMotifGraphics.RibbonTypeEnum.MonoK, errMsg)

                    If Not (bmpBack Is Nothing) Then
                        _job.BuildGraphicsLayers(SideEnum.Back, PrintTypeEnum.MonoK, 0, 0, 0, -1, _
                                                 GraphicTypeEnum.BMP, bmpBack)
                    End If

                    _job.PrintGraphicsLayers(1, actionID)
                    _lastActionID = actionID

                End If
            End If

            AtStation(actionID, 30, errMsg, alarm)

            Return True

        Catch e As Exception
            errMsg = "Job: Smart Card Data Only Error: " + e.Message
        Finally
            _job.ClearGraphicsLayers()
            GC.Collect()
        End Try

        Return False
    End Function

    '***************************************************
    '  Name:    JobWait
    '  Purpose: Demonstrates how to poll printer for the
    '           status of a job, and printer status
    '
    '  Inputs:  actionID   - job ID
    '           testType   - type of print job
    '           loops      - max number of times to poll
    '           status     - returns printer status
    '           alarm      - returns error code
    '           encodeOnly - flag to indicate card encode only job
    '                        (no printing)
    '  Outputs: None
    '***************************************************
    Private Sub JobWait(ByVal actionID As Integer, ByVal testType As TEST_TYPE, ByVal loops As Integer, _
                        ByRef status As String, ByRef alarm As Short, ByVal encodeOnly As Boolean)

        Dim copiesCompleted = 0, _
            copiesRequested = 0, _
            errorCode As Integer = 0

        Dim contactStatus = String.Empty, _
            contactlessStatus = String.Empty, _
            errMsg = String.Empty, _
            magStatus = String.Empty, _
            printerStatus = String.Empty, _
            printingStatus = String.Empty, _
            cardPosition = String.Empty, _
            uuidJob As String = String.Empty

        alarm = 0
        status = String.Empty

        Dim jobStatusGood As Boolean = False

        While (loops > 0)
            Try
                If (_abort) Then
                    status = "Job Wait, Escape key pressed"
                    Exit While
                End If

                If (Not encodeOnly) Then
                    IsPrinterReady(printerStatus, errMsg, alarm)
                    Thread.Sleep(250)
                End If

                alarm = _job.GetJobStatus(actionID, uuidJob, printingStatus, cardPosition, _
                                          errorCode, copiesCompleted, copiesRequested, _
                                          magStatus, contactStatus, contactlessStatus)
                If (alarm <> _alarm) Then
                    GetAlarm()
                End If

                If (alarm <> 0) Then
                    If (alarm = ACTION_ID_NOT_FOUND) Then
                        status = "Action ID not found"
                        Exit While

                    ElseIf (alarm <> OUT_OF_CARDS) Then
                        status = _job.Device.GetStatusMessageString(alarm)
                        Exit While
                    End If
                End If

            Catch e As Exception
                status = "Job Wait Exception: " + e.Message
                Exit While
            End Try

            If (printingStatus = "done_ok") Then
                jobStatusGood = True
                Exit While
            End If

            If (printingStatus.ToLower().Contains("error")) Then
                status = "Printing Status Error"
                Exit While
            End If

            If (cardPosition = "ejecting_eject") Then
                status = "card exiting to eject bin"
                Exit While
            End If

            If (cardPosition = "ejecting_reject") Then
                status = "card exiting to reject bin"
                Exit While
            End If

            If (cardPosition = "ejecting_feeder") Then
                status = "card exiting to feeder"
                Exit While
            End If

            Select Case (testType)

                Case TEST_TYPE.CONTACTLESS
                Case TEST_TYPE.CONTACTLESSwithBARCODE
                    If (contactlessStatus.ToLower().Contains("error")) Then
                        status = contactlessStatus
                        Exit While
                    End If

                Case TEST_TYPE.MAGwithBARCODE
                Case TEST_TYPE.MAGwithEIN
                Case TEST_TYPE.MAG
                    If (magStatus.ToLower().Contains("error")) Then
                        status = magStatus
                        Exit While
                    End If
            End Select

            Thread.Sleep(1000)
            Application.DoEvents()

            If (Not encodeOnly) Then
                If (alarm = 0) Then
                    loops -= 1
                End If
            Else
                If (alarm = 0 AndAlso Not printerStatus.Contains("cooling") AndAlso Not printerStatus.Contains("heating")) Then
                    loops -= 1
                End If
            End If
        End While

        If (Not jobStatusGood AndAlso String.IsNullOrEmpty(status)) Then
            status = "Job Status Timedout"
        End If
    End Sub

    '***************************************************
    '  Name:    AtStation
    '  Purpose: Demonstrates how to poll printer for the
    '           status of at_station which indicates a
    '               smart card has been positioned for encoding
    '
    '  Inputs:  actionID   - job ID
    '           loops      - max number of times to poll
    '           status     - returns printer status
    '           alarm      - returns error code
    '  Outputs: None
    '***************************************************
    Private Sub AtStation(ByVal actionID As Integer, ByVal loops As Integer, ByRef status As String, _
                          ByRef alarm As Short)
        alarm = 0
        Try
            status = String.Empty

            Dim copiesCompleted As Integer = 0
            Dim copiesRequested As Integer = 0
            Dim errorCode As Integer = 0

            Dim contactStatus As String = String.Empty
            Dim contactlessStatus As String = String.Empty
            Dim magStatus As String = String.Empty
            Dim printingStatus As String = String.Empty
            Dim cardPosition As String = String.Empty
            Dim uuidJob As String = String.Empty

            Dim timedOut As Boolean = True

            For i As Integer = 0 To loops - 1

                alarm = _job.GetJobStatus(actionID, uuidJob, printingStatus, cardPosition, _
                                          errorCode, copiesCompleted, copiesRequested, _
                                          magStatus, contactStatus, contactlessStatus)

                If (printingStatus.Contains("error") OrElse printingStatus = "at_station" _
                    OrElse contactStatus = "at_station" OrElse contactlessStatus = "at_station") Then
                    timedOut = False
                    Exit For
                End If
                Thread.Sleep(1000)
            Next

            If (timedOut) Then
                status = "At Station Timed Out"
            End If

        Catch e As Exception
            status = "At Station Exception: " + e.Message
        End Try
    End Sub

    '***************************************************
    '  Name:    GetAlarm
    '  Purpose: Demonstrates how to poll printer for its
    '           current error status
    '
    '  Inputs:  none
    '           
    '  Outputs: alarm - current error status
    '***************************************************
    Private Function GetAlarm() As Integer

        Dim alarm As Short = 0
        Dim errMsg As String = String.Empty
        Dim temp As String = String.Empty
        Dim newErr As Boolean = False
        Try
            alarm = _job.Device.GetDeviceInfo(temp, temp, temp, temp, temp, _
                                              temp, temp, temp, temp, temp)
            If (alarm <> 0) Then
                errMsg = "Alarm (" + alarm.ToString() + ") " + _job.Device.GetStatusMessageString(alarm)
                If (alarm = OUT_OF_CARDS) Then
                    lblStatusUpdate("Out of Cards")
                End If
            End If

            If (alarm = 0 AndAlso _alarm <> 0) Then
                _alarm = 0
                LogAppend("Alarm has been cleared")
                Return alarm
            End If

            If (_alarm <> alarm) Then
                _alarm = alarm
                If (_alarm <> 0) Then
                    newErr = True
                    LogAppend(errMsg)
                End If
            End If

        Catch e As Exception
            errMsg = e.Message
        End Try

        Return alarm
    End Function


#End Region 'ZMotifPrinter SDK

#Region "ZMotifGraphics SDK"
    '***************************************************
    '  Name: GetGraphicsSDKVersion
    '  Purpose: Demonstrates how to retrieve SDK version
    '           from ZGraphicsPrinter.dll
    '
    '  Inputs:  None
    '  Outputs: Version of SDK returned as string 
    '***************************************************
    Private Function GetGraphicsSDKVersion() As String
        Try
            Dim major As Byte
            Dim minor As Byte
            Dim build As Byte
            Dim revision As Byte

            _graphics.GetSDKVersion(major, minor, build, revision)

            Dim temp As String = "         ZMotifGraphicsSDK version: "

            temp += Convert.ToString(major)
            temp += "."
            temp += Convert.ToString(minor)
            temp += "."
            temp += Convert.ToString(build)
            temp += "."
            temp += Convert.ToString(revision)

            Return temp

        Catch ex As Exception

            MessageBox.Show(ex.Message, "GetGraphicsSDKVersion threw exception")
        End Try

        Return ""
    End Function

    '***************************************************
    '  Name:    BuildImage
    '  Purpose: Demonstrates how to build an image using
    '           the graphics SDK, set the image's print 
    '           orientation, and set the ribbon type
    '
    '  Inputs:  image - raw bitmap image
    '           ImageOrientation - print orientation of 
    '           image: landscape or portrait
    '  Outputs: configured image for print job 
    '***************************************************
    Private Function BuildImage(ByVal image As Byte(), ByVal ImageOrientation As ZMotifGraphics.ImageOrientationEnum, _
                                ByVal RibbonType As ZMotifGraphics.RibbonTypeEnum, ByRef errMsg As String) As Byte()
        Try
            errMsg = String.Empty

            Dim dataLen As Integer = 0
            Dim TheImage As Byte() = Nothing

            _graphics.InitGraphics(0, 0, ImageOrientation, RibbonType)

            _graphics.DrawImage(image, ZMotifGraphics.ImagePositionEnum.Centered, 1024, 653, 0)

            If Not (image Is Nothing) Then
                TheImage = _graphics.CreateBitmap(dataLen)

                Return TheImage
            End If

        Catch ex As Exception
            errMsg = ex.Message
        Finally
            _graphics.ClearGraphics()
        End Try

        Return Nothing
    End Function
#End Region 'ZMotifGraphics SDK

End Class
