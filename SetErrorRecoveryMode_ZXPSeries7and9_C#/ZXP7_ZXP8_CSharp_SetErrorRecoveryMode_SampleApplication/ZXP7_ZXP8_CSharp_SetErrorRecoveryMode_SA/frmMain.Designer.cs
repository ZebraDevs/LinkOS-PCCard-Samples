namespace ZXP7_ZXP8_CSharp_SetErrorRecoveryMode_SA
{
    partial class frmMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.gbButtons = new System.Windows.Forms.GroupBox();
            this.lbJobStatus = new System.Windows.Forms.Label();
            this.btnExit = new System.Windows.Forms.Button();
            this.btnRun = new System.Windows.Forms.Button();
            this.lblVersions = new System.Windows.Forms.Label();
            this.gbPrinters = new System.Windows.Forms.GroupBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbEthernet = new System.Windows.Forms.RadioButton();
            this.rbUSB = new System.Windows.Forms.RadioButton();
            this.btnLocatePrinters = new System.Windows.Forms.Button();
            this.btnConnectToPrinter = new System.Windows.Forms.Button();
            this.lbStatus = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cboCardType = new System.Windows.Forms.ComboBox();
            this.cboPrn = new System.Windows.Forms.ComboBox();
            this.gbButtons.SuspendLayout();
            this.gbPrinters.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbButtons
            // 
            this.gbButtons.Controls.Add(this.lbJobStatus);
            this.gbButtons.Controls.Add(this.btnExit);
            this.gbButtons.Controls.Add(this.btnRun);
            this.gbButtons.Location = new System.Drawing.Point(8, 182);
            this.gbButtons.Name = "gbButtons";
            this.gbButtons.Size = new System.Drawing.Size(725, 49);
            this.gbButtons.TabIndex = 10;
            this.gbButtons.TabStop = false;
            // 
            // lbJobStatus
            // 
            this.lbJobStatus.AutoSize = true;
            this.lbJobStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbJobStatus.Location = new System.Drawing.Point(12, 19);
            this.lbJobStatus.Name = "lbJobStatus";
            this.lbJobStatus.Size = new System.Drawing.Size(157, 16);
            this.lbJobStatus.TabIndex = 18;
            this.lbJobStatus.Text = "Searching for Printers";
            this.lbJobStatus.Visible = false;
            // 
            // btnExit
            // 
            this.btnExit.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExit.ForeColor = System.Drawing.Color.DarkRed;
            this.btnExit.Location = new System.Drawing.Point(655, 15);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(60, 23);
            this.btnExit.TabIndex = 1;
            this.btnExit.Text = "E&xit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnRun
            // 
            this.btnRun.Enabled = false;
            this.btnRun.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRun.ForeColor = System.Drawing.Color.DarkGreen;
            this.btnRun.Location = new System.Drawing.Point(582, 15);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(60, 23);
            this.btnRun.TabIndex = 0;
            this.btnRun.Text = "&Run";
            this.btnRun.UseVisualStyleBackColor = true;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // lblVersions
            // 
            this.lblVersions.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblVersions.ForeColor = System.Drawing.Color.Navy;
            this.lblVersions.Location = new System.Drawing.Point(435, 15);
            this.lblVersions.Name = "lblVersions";
            this.lblVersions.Size = new System.Drawing.Size(298, 16);
            this.lblVersions.TabIndex = 9;
            this.lblVersions.Text = "SDKs In Use: ZMotifPrinter.dll, ZMotifGraphics.dll";
            this.lblVersions.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // gbPrinters
            // 
            this.gbPrinters.Controls.Add(this.groupBox1);
            this.gbPrinters.Controls.Add(this.btnConnectToPrinter);
            this.gbPrinters.Controls.Add(this.lbStatus);
            this.gbPrinters.Controls.Add(this.label2);
            this.gbPrinters.Controls.Add(this.label1);
            this.gbPrinters.Controls.Add(this.cboCardType);
            this.gbPrinters.Controls.Add(this.cboPrn);
            this.gbPrinters.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbPrinters.Location = new System.Drawing.Point(8, 34);
            this.gbPrinters.Name = "gbPrinters";
            this.gbPrinters.Size = new System.Drawing.Size(725, 133);
            this.gbPrinters.TabIndex = 8;
            this.gbPrinters.TabStop = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbEthernet);
            this.groupBox1.Controls.Add(this.rbUSB);
            this.groupBox1.Controls.Add(this.btnLocatePrinters);
            this.groupBox1.Location = new System.Drawing.Point(531, 20);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(182, 100);
            this.groupBox1.TabIndex = 19;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Discover Printers";
            // 
            // rbEthernet
            // 
            this.rbEthernet.AutoSize = true;
            this.rbEthernet.Location = new System.Drawing.Point(25, 45);
            this.rbEthernet.Name = "rbEthernet";
            this.rbEthernet.Size = new System.Drawing.Size(79, 20);
            this.rbEthernet.TabIndex = 15;
            this.rbEthernet.Text = "Ethernet";
            this.rbEthernet.UseVisualStyleBackColor = true;
            // 
            // rbUSB
            // 
            this.rbUSB.AutoSize = true;
            this.rbUSB.Checked = true;
            this.rbUSB.Location = new System.Drawing.Point(25, 21);
            this.rbUSB.Name = "rbUSB";
            this.rbUSB.Size = new System.Drawing.Size(57, 20);
            this.rbUSB.TabIndex = 14;
            this.rbUSB.TabStop = true;
            this.rbUSB.Text = "USB ";
            this.rbUSB.UseVisualStyleBackColor = true;
            // 
            // btnLocatePrinters
            // 
            this.btnLocatePrinters.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLocatePrinters.ForeColor = System.Drawing.Color.Black;
            this.btnLocatePrinters.Location = new System.Drawing.Point(25, 71);
            this.btnLocatePrinters.Name = "btnLocatePrinters";
            this.btnLocatePrinters.Size = new System.Drawing.Size(142, 23);
            this.btnLocatePrinters.TabIndex = 16;
            this.btnLocatePrinters.Text = "Locate Printers";
            this.btnLocatePrinters.UseVisualStyleBackColor = true;
            this.btnLocatePrinters.Click += new System.EventHandler(this.btnLocatePrinters_Click);
            // 
            // btnConnectToPrinter
            // 
            this.btnConnectToPrinter.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnConnectToPrinter.ForeColor = System.Drawing.Color.Black;
            this.btnConnectToPrinter.Location = new System.Drawing.Point(367, 23);
            this.btnConnectToPrinter.Name = "btnConnectToPrinter";
            this.btnConnectToPrinter.Size = new System.Drawing.Size(142, 23);
            this.btnConnectToPrinter.TabIndex = 18;
            this.btnConnectToPrinter.Text = "Connect to Printer";
            this.btnConnectToPrinter.UseVisualStyleBackColor = true;
            this.btnConnectToPrinter.Click += new System.EventHandler(this.btnConnectToPrinter_Click);
            // 
            // lbStatus
            // 
            this.lbStatus.AutoSize = true;
            this.lbStatus.Location = new System.Drawing.Point(97, 101);
            this.lbStatus.Name = "lbStatus";
            this.lbStatus.Size = new System.Drawing.Size(147, 16);
            this.lbStatus.TabIndex = 17;
            this.lbStatus.Text = "Searching for Printers";
            this.lbStatus.Visible = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 58);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(82, 16);
            this.label2.TabIndex = 13;
            this.label2.Text = "Card Types:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 16);
            this.label1.TabIndex = 12;
            this.label1.Text = "Printer:";
            // 
            // cboCardType
            // 
            this.cboCardType.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboCardType.Location = new System.Drawing.Point(100, 55);
            this.cboCardType.Name = "cboCardType";
            this.cboCardType.Size = new System.Drawing.Size(250, 24);
            this.cboCardType.TabIndex = 1;
            // 
            // cboPrn
            // 
            this.cboPrn.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboPrn.Location = new System.Drawing.Point(100, 20);
            this.cboPrn.Name = "cboPrn";
            this.cboPrn.Size = new System.Drawing.Size(250, 24);
            this.cboPrn.TabIndex = 0;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(741, 247);
            this.Controls.Add(this.gbButtons);
            this.Controls.Add(this.lblVersions);
            this.Controls.Add(this.gbPrinters);
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ZXP7 ZXP8 C# Set Error Recovery Mode Print Sample";
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.Shown += new System.EventHandler(this.frmMain_Shown);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            this.gbButtons.ResumeLayout(false);
            this.gbButtons.PerformLayout();
            this.gbPrinters.ResumeLayout(false);
            this.gbPrinters.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbButtons;
        private System.Windows.Forms.Label lbJobStatus;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.Label lblVersions;
        private System.Windows.Forms.GroupBox gbPrinters;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rbEthernet;
        private System.Windows.Forms.RadioButton rbUSB;
        private System.Windows.Forms.Button btnLocatePrinters;
        private System.Windows.Forms.Button btnConnectToPrinter;
        private System.Windows.Forms.Label lbStatus;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cboCardType;
        private System.Windows.Forms.ComboBox cboPrn;
    }
}

