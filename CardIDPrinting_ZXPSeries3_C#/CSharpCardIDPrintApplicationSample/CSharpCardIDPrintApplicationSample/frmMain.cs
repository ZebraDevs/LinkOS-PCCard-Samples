/**********************************************
* CONFIDENTIAL AND PROPRIETARY
*
* The source code and other information contained herein is the confidential and the exclusive property of
* ZIH Corp. and is subject to the terms and conditions in your end user license agreement.
* This source code, and any other information contained herein, shall not be copied, reproduced, published,
* displayed or distributed, in whole or in part, in any medium, by any means, for any purpose except as
* expressly permitted under such license agreement.
*
* Copyright ZIH Corp. 2011
*
* ALL RIGHTS RESERVED
***********************************************
File: frmMain.cs
Description: Form allowing users to select printing and image overlay options.
$Revision: 1 $
$Date: 2011/08/10 $
*******************************************************************************/

using System;
using System.Windows.Forms;
using System.Drawing.Printing;
using System.Threading;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace CSharpCardIDPrintApplicationSample
{
    public partial class frmMain : Form
    {
        #region Local Variables

        // Versions -----------------------------------------------------------------------------------------

        private string _graphicsSDKVersion ;
        #endregion


        ZBRPrinter _thePrinterSDK = null;
        private Graphics graphics;
        private Graphics previewGraphics;
        private Bitmap bitmap;
        private Bitmap previewBitmap;
        private bool hasCapture;
        private int oldX;
        private int oldY;
        private int SigMoves;
        private int SigCounter; 
        
        Font drawFont = new Font("Arial", 16);
        SolidBrush drawBrush = new SolidBrush(Color.Black);
        SolidBrush eraseBrush = new SolidBrush(Color.Transparent);
        SolidBrush redBrush = new SolidBrush(Color.Red);
        SolidBrush blueBrush = new SolidBrush(Color.Blue);
        SolidBrush greenBrush = new SolidBrush(Color.Green);
        SolidBrush whiteBrush = new SolidBrush(Color.White);
        SolidBrush blackBrush = new SolidBrush(Color.Black); 

        private int[] SigArrayX = new int[501];
        private int[] SigArrayY = new int[501];
        private string SigData;
        private Pen blackPen = new Pen(Color.Black);
        private System.Windows.Forms.Panel pDrawWindow = null;
        private System.Windows.Forms.Button Clear;


        #region Local Functions

      
        // Configures the Form based on present dlls --------------------------------------------------------
        private void FormConfig()
        {
            string msg = "";
            try
            {
                this.lblStatus.Text = "";

                // Printing (Graphics)
                if (_graphicsSDKVersion != "")
                { 
                    msg = "Graphics: " + _graphicsSDKVersion + "; ";
                } 
 
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "FormConfig threw exception");
            }
            finally
            {
                this.lblVersions.Text = msg;
            }
        }

        // Gets the versions of the SDK's DLLs
        //     if the version == "" then the supporting dll is not present ----------------------------------
        private void GetSDKVersions()
        {
            SampleCodeGraphics g;  
            try
            {
                g = new SampleCodeGraphics();
                _graphicsSDKVersion = g.GetSDKVersion();
                  
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "GetSDKVersions threw exception");
            }
            finally
            {
                g = null;  
            }
        }
         
        #endregion

        #region Form

        // Form Initialization ------------------------------------------------------------------------------

        public frmMain()
        { 
            //This call is required by the Windows Form Designer.
            InitializeComponent();

            //Add any initialization after the InitializeComponent() call
            lblStatus.Text = "";

            // Create offscreen drawing surface
            this.bitmap = new Bitmap(this.pDrawWindow.Size.Width, this.pDrawWindow.Size.Height);
            this.previewBitmap = new Bitmap(this.panePreview.Size.Width, this.panePreview.Size.Height);
            this.graphics = Graphics.FromImage(this.bitmap);
            this.graphics.FillRectangle(new SolidBrush(Color.Yellow), 0, 0, this.Size.Width, this.Size.Height);
            this.graphics.DrawString("SIGN HERE", new Font("Arial", 20, FontStyle.Bold), new SolidBrush(Color.Salmon), 40, 20);

            this.previewGraphics = Graphics.FromImage(this.previewBitmap); 
             
         }


        private void InitializeComponent()
        {
            this.gbStatus = new System.Windows.Forms.GroupBox();
            this.lblStatus = new System.Windows.Forms.Label();
            this.btnExit = new System.Windows.Forms.Button();
            this.btnSubmit = new System.Windows.Forms.Button();
            this.gbPrinters = new System.Windows.Forms.GroupBox();
            this.cboPrn = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblVersions = new System.Windows.Forms.Label();
            this.pDrawWindow = new System.Windows.Forms.Panel();
            this.Clear = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.panePreview = new System.Windows.Forms.Panel();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.openFileDialogBackground = new System.Windows.Forms.OpenFileDialog();
            this.button1 = new System.Windows.Forms.Button();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtTextInput = new System.Windows.Forms.TextBox();
            this.gbStatus.SuspendLayout();
            this.gbPrinters.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbStatus
            // 
            this.gbStatus.Controls.Add(this.lblStatus);
            this.gbStatus.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.gbStatus.Location = new System.Drawing.Point(19, 444);
            this.gbStatus.Name = "gbStatus";
            this.gbStatus.Size = new System.Drawing.Size(986, 63);
            this.gbStatus.TabIndex = 13;
            this.gbStatus.TabStop = false;
            this.gbStatus.Text = "Status:";
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.lblStatus.Location = new System.Drawing.Point(16, 21);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(55, 14);
            this.lblStatus.TabIndex = 5;
            this.lblStatus.Text = "lblStatus";
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(940, 526);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(65, 23);
            this.btnExit.TabIndex = 15;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnSubmit
            // 
            this.btnSubmit.Location = new System.Drawing.Point(863, 526);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(65, 23);
            this.btnSubmit.TabIndex = 14;
            this.btnSubmit.Text = "Submit";
            this.btnSubmit.UseVisualStyleBackColor = true;
            this.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click);
            // 
            // gbPrinters
            // 
            this.gbPrinters.Controls.Add(this.cboPrn);
            this.gbPrinters.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbPrinters.Location = new System.Drawing.Point(10, 12);
            this.gbPrinters.Name = "gbPrinters";
            this.gbPrinters.Size = new System.Drawing.Size(417, 61);
            this.gbPrinters.TabIndex = 0;
            this.gbPrinters.TabStop = false;
            this.gbPrinters.Text = "Printers";
            // 
            // cboPrn
            // 
            this.cboPrn.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboPrn.Location = new System.Drawing.Point(34, 27);
            this.cboPrn.Name = "cboPrn";
            this.cboPrn.Size = new System.Drawing.Size(313, 22);
            this.cboPrn.TabIndex = 1;
            this.cboPrn.SelectedIndexChanged += new System.EventHandler(this.cboPrn_SelectedIndexChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblVersions);
            this.groupBox1.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(447, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(558, 61);
            this.groupBox1.TabIndex = 14;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "SDKs in Use:";
            // 
            // lblVersions
            // 
            this.lblVersions.AutoSize = true;
            this.lblVersions.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblVersions.Location = new System.Drawing.Point(16, 30);
            this.lblVersions.Name = "lblVersions";
            this.lblVersions.Size = new System.Drawing.Size(204, 16);
            this.lblVersions.TabIndex = 0;
            this.lblVersions.Text = "ZBRPrinter.dll, ZBRGraphics.dll";
            // 
            // pDrawWindow
            // 
            this.pDrawWindow.BackColor = System.Drawing.Color.Yellow;
            this.pDrawWindow.Location = new System.Drawing.Point(149, 215);
            this.pDrawWindow.Name = "pDrawWindow";
            this.pDrawWindow.Size = new System.Drawing.Size(240, 80);
            this.pDrawWindow.TabIndex = 3;
            this.pDrawWindow.Paint += new System.Windows.Forms.PaintEventHandler(this.pDrawWindow_Paint);
            this.pDrawWindow.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pDrawWindow_MouseMove);
            this.pDrawWindow.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pDrawWindow_MouseDown);
            this.pDrawWindow.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pDrawWindow_MouseUp);
            // 
            // Clear
            // 
            this.Clear.Location = new System.Drawing.Point(765, 526);
            this.Clear.Name = "Clear";
            this.Clear.Size = new System.Drawing.Size(75, 23);
            this.Clear.TabIndex = 0;
            this.Clear.Text = "Clear";
            this.Clear.Click += new System.EventHandler(this.Clear_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.panePreview);
            this.groupBox2.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(447, 90);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(558, 357);
            this.groupBox2.TabIndex = 16;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Preview";
            // 
            // panePreview
            // 
            this.panePreview.Location = new System.Drawing.Point(23, 19);
            this.panePreview.Name = "panePreview";
            this.panePreview.Size = new System.Drawing.Size(512, 324);
            this.panePreview.TabIndex = 0;
            this.panePreview.Paint += new System.Windows.Forms.PaintEventHandler(this.panePreview_Paint);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // openFileDialogBackground
            // 
            this.openFileDialogBackground.FileName = "openFileDialog1";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(149, 110);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(242, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Select Photo ID Image";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.label2);
            this.groupBox5.Controls.Add(this.pDrawWindow);
            this.groupBox5.Controls.Add(this.button2);
            this.groupBox5.Controls.Add(this.label3);
            this.groupBox5.Controls.Add(this.label1);
            this.groupBox5.Controls.Add(this.button1);
            this.groupBox5.Controls.Add(this.txtTextInput);
            this.groupBox5.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox5.Location = new System.Drawing.Point(10, 90);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(417, 357);
            this.groupBox5.TabIndex = 4;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Card Configuration";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 60);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(128, 16);
            this.label2.TabIndex = 3;
            this.label2.Text = "Background Image";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(149, 57);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(242, 23);
            this.button2.TabIndex = 2;
            this.button2.Text = "Select Background Image";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 169);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 16);
            this.label3.TabIndex = 4;
            this.label3.Text = "Text";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 117);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(106, 16);
            this.label1.TabIndex = 1;
            this.label1.Text = "Photo ID Image";
            // 
            // txtTextInput
            // 
            this.txtTextInput.Location = new System.Drawing.Point(149, 166);
            this.txtTextInput.Name = "txtTextInput";
            this.txtTextInput.Size = new System.Drawing.Size(240, 22);
            this.txtTextInput.TabIndex = 0;
            this.txtTextInput.TextChanged += new System.EventHandler(this.txtTextInput_TextChanged);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1020, 561);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnSubmit);
            this.Controls.Add(this.gbPrinters);
            this.Controls.Add(this.gbStatus);
            this.Controls.Add(this.Clear);
            this.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Card ID Print Application Sample";
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.gbStatus.ResumeLayout(false);
            this.gbStatus.PerformLayout();
            this.gbPrinters.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.ResumeLayout(false);

        }

        private void pDrawWindow_Paint(System.Object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImage(this.bitmap, 0, 0);
        }

        private void pDrawWindow_MouseDown(System.Object sender, MouseEventArgs e)
        {
            this.hasCapture = true;
            this.oldX = e.X;
            this.oldY = e.Y;
            if (SigMoves + 1 > 500)
            {
                Array.Resize(ref SigArrayX, SigMoves + 2);
                Array.Resize(ref SigArrayY, SigMoves + 2);
            } 
            SigArrayX[SigMoves] = e.X;
            SigArrayY[SigMoves] = e.Y;// +128;
            
            SigMoves = SigMoves + 1;
        }
 
        private void pDrawWindow_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.hasCapture)
            {
                if (e.X >= 0 & e.X <= 239 & e.Y >= 0 & e.Y <= 79)
                {
                    this.graphics.DrawLine(blackPen, this.oldX, this.oldY, e.X, e.Y);
                    this.pDrawWindow.Invalidate(new Rectangle(Math.Min(e.X, this.oldX), Math.Min(e.Y, this.oldY), Math.Abs(this.oldX - e.X) + 1, Math.Abs(this.oldY - e.Y) + 1));
                   // this.previewGraphics.DrawLine(blackPen, this.oldX, this.oldY + 140, e.X, e.Y + 140);
                   // this.panePreview.Invalidate(new Rectangle(Math.Min(e.X, this.oldX), Math.Min(e.Y + 140, this.oldY + 140), Math.Abs(this.oldX - e.X) + 1, Math.Abs(this.oldY - e.Y + 140) + 1));
                    this.oldX = e.X;
                    this.oldY = e.Y;
                    SigArrayX[SigMoves] = e.X;
                    SigArrayY[SigMoves] = e.Y;
                    if (SigMoves + 1 > 500)
                    {
                        Array.Resize(ref SigArrayX, SigMoves + 2);
                        Array.Resize(ref SigArrayY, SigMoves + 2);
                    }
                    SigMoves = SigMoves + 1;
                    resetCanvas();
                }
            }
        }

        private void pDrawWindow_MouseUp(object sender, MouseEventArgs e)
        {
            this.hasCapture = false;
        } 
 
        private void Clear_Sig()
        {
            this.previewGraphics.Clear(Color.Transparent);
            this.panePreview.Invalidate();
            
            this.graphics.FillRectangle(new SolidBrush(Color.Yellow), 0, 0, this.pDrawWindow.Size.Width, this.pDrawWindow.Size.Height);
            this.pDrawWindow.Invalidate();
            this.graphics.DrawString("SIGN HERE", new Font("Arial", 20, FontStyle.Bold), new SolidBrush(Color.Salmon), 40, 20);
            txtTextInput.Text = "";
        }

        private void Clear_Click(System.Object sender, System.EventArgs e)
        {
            
            SigArrayX = new int[501];
            SigArrayY = new int[501];
            Clear_Sig();
            openFileDialog1.FileName = ""; 
            openFileDialogBackground.FileName = "";
            previewGraphics.Clear(Color.White);
        } 

        private void pDrawWindow_GotFocus(System.Object sender, System.EventArgs e)
        {
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        // Form Load Event ----------------------------------------------------------------------------------

        private void frmMain_Load(object sender, EventArgs e)
        {
            _thePrinterSDK = new ZBRPrinter();
            LocatePrinters();
            cboPrn.Focus();

            //GetSDKVersions();
           // FormConfig();
            resetCanvas();
        }


        #endregion

        #region Buttons

        // Exits the Application ----------------------------------------------------------------------------

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        // Submit Button
        //     Starts the example code based on Form selections ---------------------------------------------

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            #region Variables

            bool eject = false;
            string msg = "";
 
            SampleCodeGraphics prn = null; 

            #endregion

            #region Check Selections

            // Verifies that a printer has been selected
            try
            {
                if (cboPrn.SelectedIndex < 0)
                {
                    msg = "Error: A Printer has not been selected";
                    return;
                }
 
            #endregion
  

                #region Printing

                // Initialize the Print Side Class

                prn = new SampleCodeGraphics();

                // Determines the printing type

                SampleCodeDrawConfiguration config = new SampleCodeDrawConfiguration();
                config.StringLabelText = txtTextInput.Text;
                config.LabelLocation = new Point(50, 465); 
                
                if (openFileDialogBackground.FileName.EndsWith(".bmp") || openFileDialogBackground.FileName.EndsWith(".png"))
                {
                    config.BackgroundImageData =
                        new ZBRGraphics().AsciiEncoder.GetBytes(openFileDialogBackground.FileName);

                    config.BackgroundImageRect = new Rectangle(0,0,1024,648);
                }

                if (openFileDialog1.FileName.EndsWith(".bmp") || openFileDialog1.FileName.EndsWith(".png"))
                {
                    config.ImageData =
                        new ZBRGraphics().AsciiEncoder.GetBytes(openFileDialog1.FileName);

                    config.ImageLocationRect = new Rectangle(50, 50, 400, 400);
                }
                if( SigMoves >0)
                {
                    Bitmap signbitmap = new Bitmap(240, 80, PixelFormat.Format32bppArgb); //Format32bppArgb for composite
                    Graphics g = Graphics.FromImage(signbitmap);
                    g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
                    for (int i = 1; i < SigMoves; i++)
                    {
                        g.DrawLine(blackPen, this.SigArrayX[i - 1], this.SigArrayY[i - 1], this.SigArrayX[i], this.SigArrayY[i]);
                    }
                    signbitmap.Save(Application.StartupPath + "signature.png", ImageFormat.Png);
 
                    config.SignatureImageData =
                        new ZBRGraphics().AsciiEncoder.GetBytes(Application.StartupPath + "signature.png");
                    config.SignatureImageRect = new Rectangle(10, 500, 240, 80); 
                }                 
                 
                prn.PrintFrontSideOnly(this.cboPrn.Text, 
                     config,
                     out msg);
                    if (msg == "") this.lblStatus.Text = "No Errors";
                  
            }
            catch (Exception ex)
            {
                msg += ex.Message;
                MessageBox.Show(ex.ToString(), "btnSubmit_Click threw exception");
            }
            finally
            {
                if (msg != "")
                    this.lblStatus.Text = msg;
                 
                prn = null; 
            }

                #endregion
        }

        #endregion
 



        #region Printer Setup

        /**************************************************************************************************
        * Function Name: ConfigureForEthernetPrinter
        * 
        * Purpose: Configures the program for an ethernet-connected printer. 
        * 
        * Parameters: None 
        * 
        * Returns: None 
        * 
        * History:
        * Date             Who             Comment
        * 12/9/2010        ACT             Function creation.
        ***************************************************************************************************/
        private void ConfigurePrinter()
        {
            try
            {
                string deviceName = string.Empty;
                if (!GetPrinterDesignation(ref deviceName))
                    return;

                if (!RefreshConnectionToPrinter())
                    return;
                 

                cboPrn.Visible = true;

                ConfigureApp(deviceName);

                Refresh();
                Application.DoEvents();
                Thread.Sleep(100); 
            }
            catch (Exception ex)
            {
                MessageBox.Show("ConfigureForEthernetPrinter threw exception: " + ex.Message);
            }
            finally
            {
                CloseConnectionToPrinter();
            }
        }

        /**************************************************************************************************
        * Function Name: DecipherConfigurationCode
        * 
        * Purpose: Determine a printer's configuration from its configuration code. 
        * 
        * Parameters: options = string value containing the configuration code
        *                 usb = boolean value which defines the type of printer interface
        *                        True = usb-connected printer
        *                       False = ethernet-connected printer   
        * 
        * Returns: None 
        * 
        * History:
        * Date             Who             Comment
        * 12/9/2010        ACT             Function creation.
        * 01/17/2011       ACT             Added enabling magnetic encoder checkbox if printer includes mag encoder.
        ***************************************************************************************************/
        private void DecipherConfigurationCode(ref byte[] options)
        {
            string errMsg = string.Empty;
            string config = string.Empty;
            try
            { 

                char sides = Convert.ToChar(options[4]);
                char SCOption = Convert.ToChar(options[5]);
                char MagOption = Convert.ToChar(options[6]);
                char Interface = Convert.ToChar(options[8]);

                config = Convert.ToString(sides); 

            }
            catch (Exception ex)
            {
                MessageBox.Show("DecipherConfigurationCode threw exception: " + ex.Message);
            }
        }

        /**************************************************************************************************
        * Function Name: ConfigureApp
        * 
        * Purpose: Configures the application's option based upon the selected printer's configuration. 
        * 
        * Parameters: deviceName = string value containing the selected printer's name
        *                 
        * Returns: None 
        * 
        * History:
        * Date             Who             Comment
        * 12/9/2010        ACT             Function creation.
        ***************************************************************************************************/
        private void ConfigureApp(string deviceName)
        {
            byte[] options = null;
            try
            {
                string errMsg = string.Empty;

                options = new byte[50];

                _thePrinterSDK.GetPrinterConfiguration(options, out errMsg);
                if (errMsg == "")
                {
                    DecipherConfigurationCode(ref options);

                }
                else MessageBox.Show("ConfigureApp failed: " + errMsg);
            }
            catch (Exception ex)
            {
                MessageBox.Show("ConfigureApp threw exception: " + ex.Message);
            }
            finally
            {
                options = null;
            }
        }

        #region Locate Printers

        /**************************************************************************************************
        * Function Name: GetPrinterDesignation
        * 
        * Purpose: Retrieve the name of the currently selected printer. 
        * 
        * Parameters: None
        * 
        * Returns: String value containing printer name
        *  
        * History:
        * Date             Who             Comment
        * 12/9/2010        ACT             Function creation.
        ***************************************************************************************************/
        private String GetPrinterDesignation()
        {
            string temp = cboPrn.Text;
            if (temp.Contains(","))
            {
                int index = temp.IndexOf(",");
                temp = temp.Substring(index + 1);
            }
            temp = temp.Trim();
            return temp;
        }

        /**************************************************************************************************
        * Function Name: GetPrinterDesignation
        * 
        * Purpose: Retrieve the name of the currently selected printer. 
        * 
        * Parameters: deviceName = string to hold printer name 
        * 
        * Returns:  True = Printer name retrieved successfully
        *          False = failed to retrieve printer name 
        * 
        * History:
        * Date             Who             Comment
        * 12/9/2010        ACT             Function creation.
        ***************************************************************************************************/
        private bool GetPrinterDesignation(ref string deviceName)
        {
            deviceName = GetPrinterDesignation();
            if (deviceName.Length == 0)
            {
                MessageBox.Show("Printer ID cannot be blank");
                return false;
            }
            return true;
        }

        /**************************************************************************************************
        * Function Name: LocatePrinters
        * 
        * Purpose: Determines how to search for installed printer(s): via usb or ethernet, 
        *           initialize printer combo box with installed printer name(s). 
        * 
        * Parameters: usbConnection = boolean which defines type of search to conduct.
        *                             True = usb search
        *                            False = ethernet search 
        * 
        * Returns:  True = Installed printer(s) located
        *          False = failed to locate installed printer(s) 
        * 
        * History:
        * Date             Who             Comment
        * 12/9/2010        ACT             Function creation.
        ***************************************************************************************************/
        private bool LocatePrinters()
        {
            Cursor.Current = Cursors.WaitCursor;
            List<string> printers = null;
            try
            {
                printers = new List<string>();

                
                LocateUSBPrinters(ref printers);
            
                LocateEthernetPrinters(ref printers);
                

                cboPrnInit(ref printers);
                if (cboPrn.Items.Count > 0)
                {
                    cboPrn.Enabled = true;
                    return true;
                }
                else
                    cboPrn.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("LocatePrinters threw exception:" + ex.Message);
            }
            finally
            {
                if (printers != null)
                    printers.Clear();
                printers = null;
                Cursor.Current = Cursors.Default;
            }
            return false;
        }

        /**************************************************************************************************
        * Function Name: LocateUSBPrinters
        * 
        * Purpose: Determines how to search for installed printer(s) via usb, 
        *           initialize printer list control with installed printer name(s). 
        * 
        * Parameters: printers = string list to hold names of installed printers 
        * 
        * Returns:  True = Installed printer(s) located
        *          False = failed to locate installed printer(s) 
        * 
        * History:
        * Date             Who             Comment
        * 12/9/2010        ACT             Function creation.
        ***************************************************************************************************/
        private bool LocateUSBPrinters(ref List<string> printers)
        {
            try
            {
                foreach (String strPrn in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
                {
                    string name = strPrn.ToUpper();

                    if (name.Contains("ZEBRA"))
                        if ((name.Contains("ZXP SERIES 3") && !name.Contains("NETWORK"))
                            || (name.Contains("ZXP S3") && !name.Contains("ZXP S3 NETWORK")))
                            printers.Add(strPrn);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("LocateUSBPrinters threw exception:" + ex.Message);
            }
            return false;
        }

        /**************************************************************************************************
        * Function Name: LocateEthernetPrinters
        * 
        * Purpose: Determines how to search for installed printer(s) via ethernet, 
        *           initialize printer list control with installed printer name(s). 
        * 
        * Parameters: printers = string list to hold names of installed printers 
        * 
        * Returns:  True = Installed printer(s) located
        *          False = failed to locate installed printer(s) 
        * 
        * History:
        * Date             Who             Comment
        * 12/9/2010        ACT             Function creation.
        ***************************************************************************************************/
        private bool LocateEthernetPrinters(ref List<string> printers)
        {
            try
            {
                foreach (String strPrn in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
                {
                    string name = strPrn.ToUpper();

                    if (name.Contains("ZEBRA"))
                        if (name.Contains("ZXP SERIES 3 NETWORK")
                            || name.Contains("ZXP S3 NETWORK"))
                            printers.Add(strPrn);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("LocateEthernetPrinters threw exception:" + ex.Message);
            }
            return false;
        }

        /**************************************************************************************************
        * Function Name: RefreshConnectionToPrinter
        * 
        * Purpose: Releases current handle and gets a new handle to the printer driver for the selected printer. 
        * 
        * Parameters: None
        *                 
        * Returns:  True = New handle retrieved
        *          False = Failed to open a new handle to the printer driver  
        * 
        * History:
        * Date             Who             Comment
        * 12/9/2010        ACT             Function creation.
        ***************************************************************************************************/
        private bool RefreshConnectionToPrinter()
        {
            try
            {
                CloseConnectionToPrinter();

                return OpenConnectionToPrinter();
            }
            catch (Exception ex)
            {
                MessageBox.Show("RefreshConnectionToPrinter threw exception: " + ex.Message);
            }
            return false;
        }



        /**************************************************************************************************
        * Function Name: cboPrnInit
        * 
        * Purpose: Initialize printer combo box with installed printer name(s). 
        * 
        * Parameters: printers = string list of installed printer name(s) 
        * 
        * Returns: None 
        * 
        * History:
        * Date             Who             Comment
        * 12/9/2010        ACT             Function creation.
        ***************************************************************************************************/
        private void cboPrnInit(ref List<string> printers)
        {
            try
            {
                cboPrn.Text = "";
                cboPrn.Items.Clear();
                cboPrn.Refresh();

                foreach (string printer in printers)
                {
                    cboPrn.Items.Add(printer);
                }

                cboPrn.Focus();
                cboPrn.SelectedIndex = -1;

                Refresh();
            }
            catch (System.Exception ex)
            {
                cboPrn.Items.Clear();
                MessageBox.Show("cboPrnInit threw exception: Could not locate printers on USB port: " + ex.Message);
            }
        }



        /**************************************************************************************************
        * Function Name: OpenConnectionToPrinter
        * 
        * Purpose: Gets a handle to the printer driver for the selected printer. 
        * 
        * Parameters: None
        *                 
        * Returns:  True = Handle retrieved
        *          False = Failed to open a handle to the printer driver  
        * 
        * History:
        * Date             Who             Comment
        * 12/9/2010        ACT             Function creation.
        ***************************************************************************************************/
        private bool OpenConnectionToPrinter()
        {
            try
            {
                string errMsg = string.Empty;

                if (cboPrn.Text.Length <= 0)
                {
                    MessageBox.Show("No printer selected");
                    return false;
                }

                _thePrinterSDK.Open(cboPrn.Text, out errMsg);

                if (errMsg == string.Empty)
                {
                    return true;
                }
                MessageBox.Show("Unable to open device [" + cboPrn.Text + "]. " + errMsg);
            }
            catch (Exception ex)
            {
                MessageBox.Show("OPenConnectionToPrinter threw exception; Unable to open device: " + ex.Message);
            }
            return false;
        }

        /**************************************************************************************************
        * Function Name: CloseConnectionToPrinter
        * 
        * Purpose: Releases a handle to the printer driver for the selected printer. 
        * 
        * Parameters: None
        *                 
        * Returns:  True = Handle released
        *          False = Failed to release a handle to the printer driver  
        * 
        * History:
        * Date             Who             Comment
        * 12/9/2010        ACT             Function creation.
        ***************************************************************************************************/
        private bool CloseConnectionToPrinter()
        {
            try
            {
                string errMsg = string.Empty;

                _thePrinterSDK.Close(out errMsg);

                if (errMsg == string.Empty)
                    return true;
                else
                    MessageBox.Show("Unable to close device [" + cboPrn.Text + "]. " + errMsg);
            }
            catch (Exception ex)
            {
                MessageBox.Show("CloseConnectionToPrinter threw exception: " + ex.Message);
            }
            return false;
        }


        #endregion //Locate Printers 

       
        /**************************************************************************************************
        * Function Name: cboPrn_SelectedIndexChanged
        * 
        * Purpose: Event handler for installed printers combo box selection event. 
        * 
        * Parameters: sender = object which caused the event to be called
        *                  e = arguments related to the event
        * 
        * Returns: None
        *   
        * History:
        * Date             Who             Comment
        * 12/8/2010        ACT             Function creation.
        * 01/17/2011       ACT             Added call to function ResetMagneticEncoderControls(). 
        ***************************************************************************************************/
        private void cboPrn_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;
 
                  ConfigurePrinter(); 
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "cboPrn_SelectedIndexChanged threw exception");
            }
            finally
            {
                Cursor = Cursors.Arrow;
            }
        }



        #endregion //Configure application for selected printer

        private void txtTextInput_TextChanged(object sender, EventArgs e)
        {
            resetCanvas();
        }

        public void resetCanvas()
        {
            previewGraphics.Clear(Color.White);

            Brush currentBrush = blackBrush;

            if (openFileDialogBackground.FileName.EndsWith(".bmp") || openFileDialogBackground.FileName.EndsWith(".png"))
            {
                Image image = Image.FromFile(openFileDialogBackground.FileName);
                //previewGraphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;

                // selected Image will show in the pictureBox
                this.previewGraphics.DrawImage(image,
                    0, 0, panePreview.Width, panePreview.Height);
            }

            if (openFileDialog1.FileName != null && openFileDialog1.FileName.Length > 0 && openFileDialog1.FileName != "openFileDialog1")
            {
                Image image = Image.FromFile(openFileDialog1.FileName);
                //previewGraphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;

                // selected Image will show in the pictureBox
                this.previewGraphics.DrawImage(image,
                    25, 25, 200, 200);
            }

            //Graphics g = Graphics.FromImage(signbitmap);
            //g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;

            currentBrush = blackBrush;

            if (txtTextInput.Text.Length > 0)
            {
                // previewGraphics.Clear(Color.Transparent); 
                previewGraphics.DrawString(txtTextInput.Text, drawFont, drawBrush, 0, 225);
            }
            
            
            if (SigMoves > 0)
            {
                for (int i = 1; i < SigMoves; i++)
                {
                    previewGraphics.DrawLine(blackPen, this.SigArrayX[i - 1], this.SigArrayY[i - 1] + 250, this.SigArrayX[i], this.SigArrayY[i] + 250);
                } 
            }
            
            panePreview.Invalidate();

        }
        private void rdo_CheckedChanged(object sender, EventArgs e)
        {
            resetCanvas();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Bitmap files (*.bmp)|*.bmp|Png files (*.png)|*.png";
            
            openFileDialog1.ShowDialog();
            resetCanvas();
        }

        private void panePreview_Paint(object sender, PaintEventArgs e)
        {

            e.Graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
            e.Graphics.DrawImage(this.previewBitmap, 0, 0);
        }

        private void button2_Click(object sender, EventArgs e) 
        {
            openFileDialogBackground.Filter = "Bitmap files (*.bmp)|*.bmp|Png files (*.png)|*.png";
            
            openFileDialogBackground.ShowDialog();
            resetCanvas();
        }
 

    }
}