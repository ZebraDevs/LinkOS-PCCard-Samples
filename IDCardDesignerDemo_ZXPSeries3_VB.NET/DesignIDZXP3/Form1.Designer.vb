<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox
        Me.PictureBox_Preview = New System.Windows.Forms.PictureBox
        Me.Button_Print = New System.Windows.Forms.Button
        Me.Button_UpdatePreview = New System.Windows.Forms.Button
        Me.Label1 = New System.Windows.Forms.Label
        Me.Label2 = New System.Windows.Forms.Label
        Me.Label3 = New System.Windows.Forms.Label
        Me.TextBox_Background = New System.Windows.Forms.TextBox
        Me.TextBox_Foreground = New System.Windows.Forms.TextBox
        Me.TextBox_Text = New System.Windows.Forms.TextBox
        Me.Button_BrowseBackground = New System.Windows.Forms.Button
        Me.Button_BrowseForeground = New System.Windows.Forms.Button
        Me.Label5 = New System.Windows.Forms.Label
        Me.PictureBox_Signature = New System.Windows.Forms.PictureBox
        Me.Button_ClearSignature = New System.Windows.Forms.Button
        Me.Label4 = New System.Windows.Forms.Label
        Me.ComboBox_Printers = New System.Windows.Forms.ComboBox
        Me.GroupBox1.SuspendLayout()
        CType(Me.PictureBox_Preview, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.PictureBox_Signature, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.PictureBox_Preview)
        Me.GroupBox1.Controls.Add(Me.Button_Print)
        Me.GroupBox1.Location = New System.Drawing.Point(450, 12)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(365, 257)
        Me.GroupBox1.TabIndex = 0
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Preview"
        '
        'PictureBox_Preview
        '
        Me.PictureBox_Preview.BackColor = System.Drawing.Color.White
        Me.PictureBox_Preview.Location = New System.Drawing.Point(16, 19)
        Me.PictureBox_Preview.Name = "PictureBox_Preview"
        Me.PictureBox_Preview.Size = New System.Drawing.Size(335, 206)
        Me.PictureBox_Preview.TabIndex = 0
        Me.PictureBox_Preview.TabStop = False
        '
        'Button_Print
        '
        Me.Button_Print.Location = New System.Drawing.Point(288, 228)
        Me.Button_Print.Name = "Button_Print"
        Me.Button_Print.Size = New System.Drawing.Size(65, 23)
        Me.Button_Print.TabIndex = 9
        Me.Button_Print.Text = "Print"
        Me.Button_Print.UseVisualStyleBackColor = True
        '
        'Button_UpdatePreview
        '
        Me.Button_UpdatePreview.Location = New System.Drawing.Point(360, 215)
        Me.Button_UpdatePreview.Name = "Button_UpdatePreview"
        Me.Button_UpdatePreview.Size = New System.Drawing.Size(75, 54)
        Me.Button_UpdatePreview.TabIndex = 16
        Me.Button_UpdatePreview.Text = "Update"
        Me.Button_UpdatePreview.UseVisualStyleBackColor = True
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(13, 42)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(97, 13)
        Me.Label1.TabIndex = 1
        Me.Label1.Text = "Background Image"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(17, 74)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(93, 13)
        Me.Label2.TabIndex = 2
        Me.Label2.Text = "Foreground Image"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(77, 104)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(28, 13)
        Me.Label3.TabIndex = 3
        Me.Label3.Text = "Text"
        '
        'TextBox_Background
        '
        Me.TextBox_Background.Location = New System.Drawing.Point(126, 42)
        Me.TextBox_Background.Name = "TextBox_Background"
        Me.TextBox_Background.Size = New System.Drawing.Size(220, 20)
        Me.TextBox_Background.TabIndex = 4
        Me.TextBox_Background.Text = "background.jpg"
        '
        'TextBox_Foreground
        '
        Me.TextBox_Foreground.Location = New System.Drawing.Point(126, 72)
        Me.TextBox_Foreground.Name = "TextBox_Foreground"
        Me.TextBox_Foreground.Size = New System.Drawing.Size(220, 20)
        Me.TextBox_Foreground.TabIndex = 5
        Me.TextBox_Foreground.Text = "johndoe.bmp"
        '
        'TextBox_Text
        '
        Me.TextBox_Text.Location = New System.Drawing.Point(126, 102)
        Me.TextBox_Text.Name = "TextBox_Text"
        Me.TextBox_Text.Size = New System.Drawing.Size(220, 20)
        Me.TextBox_Text.TabIndex = 6
        Me.TextBox_Text.Text = "John Doe"
        '
        'Button_BrowseBackground
        '
        Me.Button_BrowseBackground.Location = New System.Drawing.Point(360, 42)
        Me.Button_BrowseBackground.Name = "Button_BrowseBackground"
        Me.Button_BrowseBackground.Size = New System.Drawing.Size(75, 23)
        Me.Button_BrowseBackground.TabIndex = 7
        Me.Button_BrowseBackground.Text = "Browse"
        Me.Button_BrowseBackground.UseVisualStyleBackColor = True
        '
        'Button_BrowseForeground
        '
        Me.Button_BrowseForeground.Location = New System.Drawing.Point(360, 73)
        Me.Button_BrowseForeground.Name = "Button_BrowseForeground"
        Me.Button_BrowseForeground.Size = New System.Drawing.Size(74, 23)
        Me.Button_BrowseForeground.TabIndex = 8
        Me.Button_BrowseForeground.Text = "Browse"
        Me.Button_BrowseForeground.UseVisualStyleBackColor = True
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(20, 123)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(52, 13)
        Me.Label5.TabIndex = 11
        Me.Label5.Text = "Signature"
        '
        'PictureBox_Signature
        '
        Me.PictureBox_Signature.BackColor = System.Drawing.Color.White
        Me.PictureBox_Signature.Location = New System.Drawing.Point(23, 139)
        Me.PictureBox_Signature.Name = "PictureBox_Signature"
        Me.PictureBox_Signature.Size = New System.Drawing.Size(323, 130)
        Me.PictureBox_Signature.TabIndex = 12
        Me.PictureBox_Signature.TabStop = False
        '
        'Button_ClearSignature
        '
        Me.Button_ClearSignature.Location = New System.Drawing.Point(360, 139)
        Me.Button_ClearSignature.Name = "Button_ClearSignature"
        Me.Button_ClearSignature.Size = New System.Drawing.Size(75, 35)
        Me.Button_ClearSignature.TabIndex = 13
        Me.Button_ClearSignature.Text = "Clear Signature"
        Me.Button_ClearSignature.UseVisualStyleBackColor = True
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(68, 15)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(37, 13)
        Me.Label4.TabIndex = 14
        Me.Label4.Text = "Printer"
        '
        'ComboBox_Printers
        '
        Me.ComboBox_Printers.FormattingEnabled = True
        Me.ComboBox_Printers.Location = New System.Drawing.Point(126, 12)
        Me.ComboBox_Printers.Name = "ComboBox_Printers"
        Me.ComboBox_Printers.Size = New System.Drawing.Size(220, 21)
        Me.ComboBox_Printers.TabIndex = 15
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(842, 280)
        Me.Controls.Add(Me.Button_UpdatePreview)
        Me.Controls.Add(Me.ComboBox_Printers)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.Button_ClearSignature)
        Me.Controls.Add(Me.PictureBox_Signature)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.Button_BrowseForeground)
        Me.Controls.Add(Me.Button_BrowseBackground)
        Me.Controls.Add(Me.TextBox_Text)
        Me.Controls.Add(Me.TextBox_Foreground)
        Me.Controls.Add(Me.TextBox_Background)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.GroupBox1)
        Me.Name = "Form1"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "ZXP1 ZXP3 - ID Designer"
        Me.GroupBox1.ResumeLayout(False)
        CType(Me.PictureBox_Preview, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.PictureBox_Signature, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents PictureBox_Preview As System.Windows.Forms.PictureBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents TextBox_Background As System.Windows.Forms.TextBox
    Friend WithEvents TextBox_Foreground As System.Windows.Forms.TextBox
    Friend WithEvents TextBox_Text As System.Windows.Forms.TextBox
    Friend WithEvents Button_BrowseBackground As System.Windows.Forms.Button
    Friend WithEvents Button_BrowseForeground As System.Windows.Forms.Button
    Friend WithEvents Button_Print As System.Windows.Forms.Button
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents PictureBox_Signature As System.Windows.Forms.PictureBox
    Friend WithEvents Button_ClearSignature As System.Windows.Forms.Button
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents ComboBox_Printers As System.Windows.Forms.ComboBox
    Friend WithEvents Button_UpdatePreview As System.Windows.Forms.Button

End Class
