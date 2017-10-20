<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class MainForm
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.btnExit = New System.Windows.Forms.Button
        Me.Label1 = New System.Windows.Forms.Label
        Me.gbPortType = New System.Windows.Forms.GroupBox
        Me.btnFindPrinters = New System.Windows.Forms.Button
        Me.lblStatus = New System.Windows.Forms.Label
        Me.rbUSB = New System.Windows.Forms.RadioButton
        Me.tvTests = New System.Windows.Forms.TreeView
        Me.Label2 = New System.Windows.Forms.Label
        Me.cboCardTypes = New System.Windows.Forms.ComboBox
        Me.lblCardTypes = New System.Windows.Forms.Label
        Me.lblTestChild = New System.Windows.Forms.Label
        Me.lblTest = New System.Windows.Forms.Label
        Me.tbLog = New System.Windows.Forms.TextBox
        Me.btnRun = New System.Windows.Forms.Button
        Me.btnCancelJob = New System.Windows.Forms.Button
        Me.cboPrn = New System.Windows.Forms.ComboBox
        Me.gbPortType.SuspendLayout()
        Me.SuspendLayout()
        '
        'btnExit
        '
        Me.btnExit.Location = New System.Drawing.Point(493, 478)
        Me.btnExit.Name = "btnExit"
        Me.btnExit.Size = New System.Drawing.Size(63, 29)
        Me.btnExit.TabIndex = 0
        Me.btnExit.Text = "Exit"
        Me.btnExit.UseVisualStyleBackColor = True
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.Location = New System.Drawing.Point(242, 87)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(142, 15)
        Me.Label1.TabIndex = 6
        Me.Label1.Text = "ZMotif Printers - USB"
        '
        'gbPortType
        '
        Me.gbPortType.Controls.Add(Me.btnFindPrinters)
        Me.gbPortType.Controls.Add(Me.lblStatus)
        Me.gbPortType.Controls.Add(Me.rbUSB)
        Me.gbPortType.Font = New System.Drawing.Font("Arial", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.gbPortType.Location = New System.Drawing.Point(29, 26)
        Me.gbPortType.Name = "gbPortType"
        Me.gbPortType.Size = New System.Drawing.Size(517, 44)
        Me.gbPortType.TabIndex = 7
        Me.gbPortType.TabStop = False
        Me.gbPortType.Text = "Printer Ports:"
        '
        'btnFindPrinters
        '
        Me.btnFindPrinters.Location = New System.Drawing.Point(142, 16)
        Me.btnFindPrinters.Name = "btnFindPrinters"
        Me.btnFindPrinters.Size = New System.Drawing.Size(97, 23)
        Me.btnFindPrinters.TabIndex = 24
        Me.btnFindPrinters.Text = "Find Printers"
        Me.btnFindPrinters.UseVisualStyleBackColor = True
        '
        'lblStatus
        '
        Me.lblStatus.ForeColor = System.Drawing.Color.Navy
        Me.lblStatus.Location = New System.Drawing.Point(293, 16)
        Me.lblStatus.Name = "lblStatus"
        Me.lblStatus.Size = New System.Drawing.Size(218, 16)
        Me.lblStatus.TabIndex = 18
        Me.lblStatus.Text = "Status"
        Me.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.lblStatus.Visible = False
        '
        'rbUSB
        '
        Me.rbUSB.AutoSize = True
        Me.rbUSB.Checked = True
        Me.rbUSB.Location = New System.Drawing.Point(15, 14)
        Me.rbUSB.Name = "rbUSB"
        Me.rbUSB.Size = New System.Drawing.Size(53, 20)
        Me.rbUSB.TabIndex = 1
        Me.rbUSB.TabStop = True
        Me.rbUSB.Text = "USB"
        Me.rbUSB.UseVisualStyleBackColor = True
        '
        'tvTests
        '
        Me.tvTests.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.tvTests.Font = New System.Drawing.Font("Arial", 10.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.tvTests.Location = New System.Drawing.Point(26, 105)
        Me.tvTests.Name = "tvTests"
        Me.tvTests.Size = New System.Drawing.Size(139, 344)
        Me.tvTests.TabIndex = 8
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.Location = New System.Drawing.Point(26, 87)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(41, 15)
        Me.Label2.TabIndex = 9
        Me.Label2.Text = "Tests"
        '
        'cboCardTypes
        '
        Me.cboCardTypes.Font = New System.Drawing.Font("Arial", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cboCardTypes.Location = New System.Drawing.Point(346, 147)
        Me.cboCardTypes.Name = "cboCardTypes"
        Me.cboCardTypes.Size = New System.Drawing.Size(200, 24)
        Me.cboCardTypes.TabIndex = 18
        '
        'lblCardTypes
        '
        Me.lblCardTypes.AutoSize = True
        Me.lblCardTypes.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblCardTypes.Location = New System.Drawing.Point(254, 150)
        Me.lblCardTypes.Name = "lblCardTypes"
        Me.lblCardTypes.Size = New System.Drawing.Size(82, 15)
        Me.lblCardTypes.TabIndex = 19
        Me.lblCardTypes.Text = "Card Types:"
        '
        'lblTestChild
        '
        Me.lblTestChild.AutoSize = True
        Me.lblTestChild.Font = New System.Drawing.Font("Arial", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblTestChild.ForeColor = System.Drawing.Color.Navy
        Me.lblTestChild.Location = New System.Drawing.Point(26, 491)
        Me.lblTestChild.Name = "lblTestChild"
        Me.lblTestChild.Size = New System.Drawing.Size(82, 16)
        Me.lblTestChild.TabIndex = 21
        Me.lblTestChild.Text = "lblTestChild"
        '
        'lblTest
        '
        Me.lblTest.AutoSize = True
        Me.lblTest.Font = New System.Drawing.Font("Arial", 10.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblTest.ForeColor = System.Drawing.Color.Navy
        Me.lblTest.Location = New System.Drawing.Point(26, 465)
        Me.lblTest.Name = "lblTest"
        Me.lblTest.Size = New System.Drawing.Size(54, 16)
        Me.lblTest.TabIndex = 20
        Me.lblTest.Text = "lblTest"
        '
        'tbLog
        '
        Me.tbLog.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.tbLog.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.tbLog.Location = New System.Drawing.Point(181, 191)
        Me.tbLog.Multiline = True
        Me.tbLog.Name = "tbLog"
        Me.tbLog.ScrollBars = System.Windows.Forms.ScrollBars.Both
        Me.tbLog.Size = New System.Drawing.Size(384, 258)
        Me.tbLog.TabIndex = 22
        Me.tbLog.WordWrap = False
        '
        'btnRun
        '
        Me.btnRun.Enabled = False
        Me.btnRun.Location = New System.Drawing.Point(424, 478)
        Me.btnRun.Name = "btnRun"
        Me.btnRun.Size = New System.Drawing.Size(63, 29)
        Me.btnRun.TabIndex = 23
        Me.btnRun.Text = "Run"
        Me.btnRun.UseVisualStyleBackColor = True
        '
        'btnCancelJob
        '
        Me.btnCancelJob.Location = New System.Drawing.Point(177, 477)
        Me.btnCancelJob.Name = "btnCancelJob"
        Me.btnCancelJob.Size = New System.Drawing.Size(91, 29)
        Me.btnCancelJob.TabIndex = 24
        Me.btnCancelJob.Text = "Cancel Job"
        Me.btnCancelJob.UseVisualStyleBackColor = True
        '
        'cboPrn
        '
        Me.cboPrn.FormattingEnabled = True
        Me.cboPrn.Location = New System.Drawing.Point(245, 105)
        Me.cboPrn.Name = "cboPrn"
        Me.cboPrn.Size = New System.Drawing.Size(302, 21)
        Me.cboPrn.TabIndex = 25
        '
        'MainForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(577, 520)
        Me.ControlBox = False
        Me.Controls.Add(Me.cboPrn)
        Me.Controls.Add(Me.btnCancelJob)
        Me.Controls.Add(Me.btnRun)
        Me.Controls.Add(Me.tbLog)
        Me.Controls.Add(Me.lblTestChild)
        Me.Controls.Add(Me.lblTest)
        Me.Controls.Add(Me.cboCardTypes)
        Me.Controls.Add(Me.lblCardTypes)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.tvTests)
        Me.Controls.Add(Me.gbPortType)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.btnExit)
        Me.Name = "MainForm"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "ZXP7 ZXP8 VB.Net Print, Magnetic Encode, Contactless Smartcard Encode Sample"
        Me.gbPortType.ResumeLayout(False)
        Me.gbPortType.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents btnExit As System.Windows.Forms.Button
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Private WithEvents gbPortType As System.Windows.Forms.GroupBox
    Private WithEvents lblStatus As System.Windows.Forms.Label
    Private WithEvents rbUSB As System.Windows.Forms.RadioButton
    Private WithEvents tvTests As System.Windows.Forms.TreeView
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Private WithEvents cboCardTypes As System.Windows.Forms.ComboBox
    Private WithEvents lblCardTypes As System.Windows.Forms.Label
    Private WithEvents lblTestChild As System.Windows.Forms.Label
    Private WithEvents lblTest As System.Windows.Forms.Label
    Private WithEvents tbLog As System.Windows.Forms.TextBox
    Friend WithEvents btnRun As System.Windows.Forms.Button
    Friend WithEvents btnCancelJob As System.Windows.Forms.Button
    Friend WithEvents btnFindPrinters As System.Windows.Forms.Button
    Friend WithEvents cboPrn As System.Windows.Forms.ComboBox

End Class
