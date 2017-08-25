'
' CONFIDENTIAL AND PROPRIETARY
'
' The source code and other information contained herein is the
' confidential and the exclusive property of ZIH Corp.
' and is subject to the terms and conditions in your end user license
' agreement.  This source code, and any other information contained herein,
' shall not be copied, reproduced, published, displayed
' or distributed, in whole or in part, in any medium, by any means, for
' any purpose except as expressly permitted under such license agreement.
' 
' The code is offered as is. There is no explicit or implicit guarantee
' of its functionality.  Zebra Technologies is not responsible for any 
' result of using the code.  By using this code, modified or not, you
' are agreeing to the terms of use.
' 
' Copyright ZIH Corp. 2012
'
' ALL RIGHTS RESERVED
'
Imports System.Runtime.InteropServices
Imports System.Drawing.Printing
Imports System.Drawing

Public Class Form1

#Region "DLL Imports"

    <DllImport("ZBRGraphics.dll")> _
    Public Shared Function ZBRGDIInitGraphics(ByVal printerName As Byte(), ByRef myHandle As Integer, _
                                          ByRef errValue As Long) _
                                          As Integer
    End Function

    <DllImport("ZBRGraphics.dll")> _
    Public Shared Function ZBRGDICloseGraphics(ByVal myHandle As Integer, ByRef errValue As Long) _
                                            As Integer
    End Function

    <DllImport("ZBRGraphics.dll")> _
    Public Shared Function ZBRGDIPrintGraphics(ByVal myHandle As Integer, ByRef errValue As Long) _
                                            As Integer
    End Function

    <DllImport("ZBRGraphics.dll")> _
    Public Shared Function ZBRGDIDrawImageRect(ByVal fileName As Byte(), ByVal x As Integer, _
                                           ByVal y As Integer, ByVal width As Integer, _
                                           ByVal height As Integer, ByRef errValue As Long) _
                                           As Integer
    End Function

    <DllImport("ZBRGraphics.dll")> _
    Public Shared Function ZBRGDIDrawText(ByVal x As Integer, ByVal y As Integer, ByVal text As Byte(), _
                                          ByVal font As Byte(), _
                                          ByVal fontSize As Integer, ByVal fontStyle As Integer, _
                                          ByVal color As Integer, ByRef errValue As Long) _
                                          As Integer
    End Function

    <DllImport("ZBRGraphics.dll")> _
    Public Shared Function ZBRGDIDrawLine(ByVal x1 As Integer, ByVal y1 As Integer, ByVal x2 As Integer, _
                                          ByVal y2 As Integer, ByVal color As Integer, _
                                          ByVal thickness As Single, ByRef errValue As Long) _
                                          As Integer
    End Function

    <DllImport("ZBRGraphics.dll")> _
    Public Shared Function ZBRGDIPreviewGraphics(ByVal pictureBoxHandle As IntPtr, ByRef errValue As Long) _
                                        As Integer
    End Function

#End Region

#Region "WinForm Graphics Variables"

    Dim isSigning As Boolean
    Dim graphic As Graphics
    Dim bitmap As Bitmap
    Dim lastXPoint As Int16
    Dim lastYPoint As Int16
    Dim backgroundImagePath As Byte()
    Dim foregroundImagePath As Byte()
    Dim myText As Byte()
    Dim myFont As Byte()
    Dim signatureFilePath As String = "signature.jpg"
    Dim previewImage As Image
    Dim pen As New Pen(Color.Black, 2)

#End Region

#Region "ZXP 3 SDK Variables"

    Dim theHandle, errValue As Integer
    Dim printerName As Byte()

#End Region

#Region "WinForm Initialization"

    ' <summary>
    ' This call is required by the Windows Form Designer.
    ' It serves as a constructor for our Form1.
    ' </summary>
    Public Sub New()

        InitializeComponent()
        populatePrinters()

    End Sub

    ' <summary>
    ' This function populates our combobox with installed printers.
    ' </summary>
    Public Sub populatePrinters()

        ComboBox_Printers.Items.Clear()
        For Each printer In PrinterSettings.InstalledPrinters
            ComboBox_Printers.Items.Add(printer)
        Next

    End Sub

#End Region

#Region "ZXP 3 SDK Function Calls"

    ' <summary>
    ' Preview the selected images, text, and signature, and print if needed.
    ' </summary>
    ' <param name="isPrinting">Use this boolean to determine if we will print or not.</param>
    ' <remarks>
    ' This function is responsible for assembling a preview
    ' containing the selected images and signature.  It will
    ' be displayed in PictureBox_Preview.  If the passed in argument
    ' "isPrinting" is true, then we also print.
    ' </remarks>
    Private Sub processJob(ByVal isPrinting As Boolean)

        ' Check that all necessary fields have been entered
        If (TextBox_Background.Text.Length < 1 Or TextBox_Foreground.Text.Length < 1 Or TextBox_Text.Text.Length < 1) Then
            MessageBox.Show("Please fill out all of the fields.")
            Return
        End If

        ' Convert strings to byte arrays
        printerName = System.Text.Encoding.ASCII.GetBytes(ComboBox_Printers.Text)
        backgroundImagePath = System.Text.Encoding.ASCII.GetBytes(TextBox_Background.Text)
        foregroundImagePath = System.Text.Encoding.ASCII.GetBytes(TextBox_Foreground.Text)
        myText = System.Text.Encoding.ASCII.GetBytes(TextBox_Text.Text)
        myFont = System.Text.Encoding.ASCII.GetBytes("Arial")

        ' Open handle to printer
        If (ZBRGDIInitGraphics(printerName, theHandle, errValue) = 0) Then
            MessageBox.Show("Error in connecting! - Error " & errValue.ToString())
            Return
        End If

        ' Draw Background Image.
        If (ZBRGDIDrawImageRect(backgroundImagePath, 10, 10, 1054, 654, errValue) = 0) Then
            MessageBox.Show("Couldn't Draw Background Image.  Error " & errValue.ToString())
        End If

        ' Draw Foreground Image
        If (ZBRGDIDrawImageRect(foregroundImagePath, 575, 30, 400, 400, errValue) = 0) Then
            MessageBox.Show("Couldn't Draw Foregound Image.  Error " & errValue.ToString())
        End If

        ' Save the signature to file, then draw it
        SaveSignatureFile(signatureFilePath)
        Dim signatureFilePathBytes As Byte() = System.Text.Encoding.ASCII.GetBytes(signatureFilePath)
        If (ZBRGDIDrawImageRect(signatureFilePathBytes, 575, 475, 400, 150, errValue) = 0) Then
            MessageBox.Show("Couldn't Draw Signature Image.  Error " & errValue.ToString())
        End If

        ' Draw Box Behind Text
        If (ZBRGDIDrawLine(30, 75, 520, 75, 1, 75, errValue) = 0) Then
            MessageBox.Show("Couldn't Draw Box.  Error " & errValue.ToString())
        End If

        ' Draw Input Text
        If (ZBRGDIDrawText(50, 50, myText, myFont, 12, 1, 99999999, errValue) = 0) Then
            MessageBox.Show("Couldn't Draw Text.  Error " & errValue.ToString())
        End If

        ' Draw Template Text
        Dim templateText As Byte() = System.Text.Encoding.ASCII.GetBytes( _
            "Company XYZ is a proud" + Environment.NewLine + _
            "distributor of industrial" + Environment.NewLine + _
            "sized black boxes for" + Environment.NewLine + _
            "medium-sized airlines.")

        If (ZBRGDIDrawText(40, 200, templateText, myFont, 9, 1, 1, errValue) = 0) Then
            MessageBox.Show("Couldn't Draw Text.  Error " & errValue.ToString())
        End If

        'Preview Graphics Buffer
        If (ZBRGDIPreviewGraphics(PictureBox_Preview.Handle, errValue) = 0) Then
            MessageBox.Show("Couldn't Preview! Error " & errValue.ToString())
        End If

        ' If isPrinting is true, print
        If (isPrinting = True) Then
            If (ZBRGDIPrintGraphics(theHandle, errValue) = 0) Then
                MessageBox.Show("Couldn't Print! Error " & errValue.ToString())
            End If
        End If

        ' Close Graphics Buffer, drop handle on printer
        ZBRGDICloseGraphics(theHandle, errValue)

    End Sub

#End Region

#Region "Button Presses"

    ' <summary>
    ' This function executes the print preview and print out for the given design.  
    ' Since we are printing, we will pass along the true boolean "isPrinting"
    ' to processJob().
    ' </summary>
    Private Sub Button_Print_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button_Print.Click

        Dim isPrinting As Boolean = True
        processJob(isPrinting)
    End Sub

    ' <summary>
    ' This function executes the print preview for the given design.  Since
    ' we are not printing, we will pass along the false boolean "isPrinting"
    ' to processJob().
    ' </summary>
    Private Sub Button_UpdatePreview_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button_UpdatePreview.Click

        Dim isPrinting As Boolean = False
        processJob(isPrinting)
    End Sub

    ' <summary>
    ' Button press to select out background image file.
    ' </summary>
    Private Sub Button_BrowseBackground_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button_BrowseBackground.Click

        Dim dialog As OpenFileDialog = New OpenFileDialog()
        dialog.Title = "Choose Background Image..."
        dialog.RestoreDirectory = True

        If (dialog.ShowDialog() = DialogResult.OK) Then
            TextBox_Background.Text = dialog.FileName
        End If

    End Sub

    ' <summary>
    ' Button press to select out foreground image file.
    ' </summary>
    Private Sub Button_BrowseForeground_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button_BrowseForeground.Click

        Dim dialog As OpenFileDialog = New OpenFileDialog()
        dialog.Title = "Choose Foreground Image..."
        dialog.RestoreDirectory = True

        If (dialog.ShowDialog() = DialogResult.OK) Then
            TextBox_Foreground.Text = dialog.FileName
        End If

    End Sub

    ' <summary>
    ' Button press to clear the signature box.
    ' </summary>
    Private Sub Button_ClearSignature_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button_ClearSignature.Click

        Try
            graphic.Clear(Color.White)
        Catch ex As Exception
            'Do nothing
        End Try
    End Sub

#End Region

#Region "Signature Capture"

    ' <summary>
    ' This function is responsible for setting our global boolean
    ' "isSigning" to true when the mouse button is pressed over our
    ' picturebox.
    ' </summary>
    ' <remarks>
    ' Since we are drawing by creating lines in between the cursor's 
    ' current position and its previous position, we must set lastPoint 
    ' to the current position to account for the user letting go of the mouse.
    ' </remarks>
    Private Sub PictureBox_Signature_MouseDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles PictureBox_Signature.MouseDown
        ' When the user presses the mouse down inside of our picturebox,
        ' we set the isSigning boolean to true.  Since we are drawing by
        ' creating lines in between the cursor's current position and its
        ' previous position, we must set lastPoint to the current position
        ' to account for the user letting go of the mouse.

        lastXPoint = e.X
        lastYPoint = e.Y
        isSigning = True
    End Sub

    ' <summary>
    ' This function is responsible for setting our global boolean
    ' "isSigning" to false when the mouse button is released.
    ' </summary>
    Private Sub PictureBox_Signature_MouseUp(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles PictureBox_Signature.MouseUp

        isSigning = False
    End Sub

    ' <summary>
    ' With every mouse movement inside the pictureBox, this
    ' function draws a line connecting the previous point
    ' and the current cursor position.
    ' </summary>
    ' <remarks>
    ' This function allows us to draw to the Signature Picture Box.  We only
    ' draw while the boolean isSigning is true, which occurs when the 
    ' mouse event MouseDown occurs over the picturebox.
    ' </remarks>
    Private Sub PictureBox_Signature_MouseMove(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles PictureBox_Signature.MouseMove

        If isSigning = True Then
            graphic = PictureBox_Signature.CreateGraphics()
            graphic.DrawLine(pen, lastXPoint, lastYPoint, e.X, e.Y)
            lastXPoint = e.X
            lastYPoint = e.Y
        End If
    End Sub

    ' <summary>
    ' Screenshot the signature image and save to local file.
    ' </summary>
    Private Sub SaveSignatureFile(ByVal filePath As String)

        ' Calculate the screen shot position and area
        Dim pictureBoxWidth As Integer = PictureBox_Signature.Size.Width
        Dim pictureBoxHeight As Integer = PictureBox_Signature.Size.Height
        Dim pictureBoxX As Integer = PictureBox_Signature.Location.X + Me.Location.X + 9
        Dim pictureBoxY As Integer = PictureBox_Signature.Location.Y + Me.Location.Y + 31

        ' Take the screen shot and save to file
        Try
            Dim bitmapScreenShot As Bitmap = New Bitmap(pictureBoxWidth, pictureBoxHeight)
            Dim graphicScreenShot As Graphics = Graphics.FromImage(bitmapScreenShot)
            graphicScreenShot.CopyFromScreen(pictureBoxX, pictureBoxY, 0, 0, New Size(pictureBoxWidth, pictureBoxHeight))
            bitmapScreenShot.Save(filePath, Imaging.ImageFormat.Jpeg)

        Catch ex As Exception
            MessageBox.Show("Could not render updated signature.")
        End Try


    End Sub

#End Region

End Class
