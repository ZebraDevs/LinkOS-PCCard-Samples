namespace SampleApplication
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
            this.gbStatus = new System.Windows.Forms.GroupBox();
            this.lblStatus = new System.Windows.Forms.Label();
            this.btnExit = new System.Windows.Forms.Button();
            this.btnRun = new System.Windows.Forms.Button();
            this.gbPrinters = new System.Windows.Forms.GroupBox();
            this.rbEthernet = new System.Windows.Forms.RadioButton();
            this.rbUSB = new System.Windows.Forms.RadioButton();
            this.lblConnectionType = new System.Windows.Forms.Label();
            this.cboPrn = new System.Windows.Forms.ComboBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.rbSingleSide = new System.Windows.Forms.RadioButton();
            this.rbDualSided = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblVersions = new System.Windows.Forms.Label();
            this.gbStatus.SuspendLayout();
            this.gbPrinters.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbStatus
            // 
            this.gbStatus.Controls.Add(this.lblStatus);
            this.gbStatus.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbStatus.Location = new System.Drawing.Point(10, 228);
            this.gbStatus.Name = "gbStatus";
            this.gbStatus.Size = new System.Drawing.Size(567, 89);
            this.gbStatus.TabIndex = 13;
            this.gbStatus.TabStop = false;
            this.gbStatus.Text = "Job Status";
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.lblStatus.Location = new System.Drawing.Point(16, 32);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(63, 16);
            this.lblStatus.TabIndex = 5;
            this.lblStatus.Text = "lblStatus";
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(512, 323);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(65, 23);
            this.btnExit.TabIndex = 15;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnRun
            // 
            this.btnRun.Enabled = false;
            this.btnRun.Location = new System.Drawing.Point(389, 323);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(117, 23);
            this.btnRun.TabIndex = 14;
            this.btnRun.Text = "Start Printer Job";
            this.btnRun.UseVisualStyleBackColor = true;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // gbPrinters
            // 
            this.gbPrinters.Controls.Add(this.rbEthernet);
            this.gbPrinters.Controls.Add(this.rbUSB);
            this.gbPrinters.Controls.Add(this.lblConnectionType);
            this.gbPrinters.Controls.Add(this.cboPrn);
            this.gbPrinters.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbPrinters.Location = new System.Drawing.Point(10, 78);
            this.gbPrinters.Name = "gbPrinters";
            this.gbPrinters.Size = new System.Drawing.Size(567, 69);
            this.gbPrinters.TabIndex = 0;
            this.gbPrinters.TabStop = false;
            this.gbPrinters.Text = "Printers";
            // 
            // rbEthernet
            // 
            this.rbEthernet.AutoSize = true;
            this.rbEthernet.Location = new System.Drawing.Point(98, 38);
            this.rbEthernet.Name = "rbEthernet";
            this.rbEthernet.Size = new System.Drawing.Size(79, 20);
            this.rbEthernet.TabIndex = 3;
            this.rbEthernet.TabStop = true;
            this.rbEthernet.Text = "Ethernet";
            this.rbEthernet.UseVisualStyleBackColor = true;
            this.rbEthernet.CheckedChanged += new System.EventHandler(this.rbEthernet_CheckedChanged);
            // 
            // rbUSB
            // 
            this.rbUSB.AutoSize = true;
            this.rbUSB.Location = new System.Drawing.Point(98, 20);
            this.rbUSB.Name = "rbUSB";
            this.rbUSB.Size = new System.Drawing.Size(53, 20);
            this.rbUSB.TabIndex = 2;
            this.rbUSB.TabStop = true;
            this.rbUSB.Text = "USB";
            this.rbUSB.UseVisualStyleBackColor = true;
            this.rbUSB.CheckedChanged += new System.EventHandler(this.rbUSB_CheckedChanged);
            // 
            // lblConnectionType
            // 
            this.lblConnectionType.AutoSize = true;
            this.lblConnectionType.Location = new System.Drawing.Point(8, 18);
            this.lblConnectionType.Name = "lblConnectionType";
            this.lblConnectionType.Size = new System.Drawing.Size(84, 16);
            this.lblConnectionType.TabIndex = 5;
            this.lblConnectionType.Text = "Connection:";
            // 
            // cboPrn
            // 
            this.cboPrn.Enabled = false;
            this.cboPrn.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboPrn.Location = new System.Drawing.Point(268, 21);
            this.cboPrn.Name = "cboPrn";
            this.cboPrn.Size = new System.Drawing.Size(284, 22);
            this.cboPrn.TabIndex = 1;
            this.cboPrn.SelectedIndexChanged += new System.EventHandler(this.cboPrn_SelectedIndexChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.rbSingleSide);
            this.groupBox2.Controls.Add(this.rbDualSided);
            this.groupBox2.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(10, 153);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(567, 69);
            this.groupBox2.TabIndex = 8;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Printing";
            // 
            // rbSingleSide
            // 
            this.rbSingleSide.AutoSize = true;
            this.rbSingleSide.Enabled = false;
            this.rbSingleSide.Location = new System.Drawing.Point(92, 21);
            this.rbSingleSide.Name = "rbSingleSide";
            this.rbSingleSide.Size = new System.Drawing.Size(108, 20);
            this.rbSingleSide.TabIndex = 5;
            this.rbSingleSide.TabStop = true;
            this.rbSingleSide.Text = "Single Sided";
            this.rbSingleSide.UseVisualStyleBackColor = true;
            this.rbSingleSide.CheckedChanged += new System.EventHandler(this.rbSingleSide_CheckedChanged);
            // 
            // rbDualSided
            // 
            this.rbDualSided.AutoSize = true;
            this.rbDualSided.Enabled = false;
            this.rbDualSided.Location = new System.Drawing.Point(92, 39);
            this.rbDualSided.Name = "rbDualSided";
            this.rbDualSided.Size = new System.Drawing.Size(96, 20);
            this.rbDualSided.TabIndex = 6;
            this.rbDualSided.TabStop = true;
            this.rbDualSided.Text = "Dual Sided";
            this.rbDualSided.UseVisualStyleBackColor = true;
            this.rbDualSided.CheckedChanged += new System.EventHandler(this.rbDualSided_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblVersions);
            this.groupBox1.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(10, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(567, 61);
            this.groupBox1.TabIndex = 14;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "SDKs In Use:";
            // 
            // lblVersions
            // 
            this.lblVersions.AutoSize = true;
            this.lblVersions.Location = new System.Drawing.Point(16, 30);
            this.lblVersions.Name = "lblVersions";
            this.lblVersions.Size = new System.Drawing.Size(204, 16);
            this.lblVersions.TabIndex = 0;
            this.lblVersions.Text = "ZBRPrinter.dll, ZBRGraphics.dll";
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(586, 358);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnRun);
            this.Controls.Add(this.gbPrinters);
            this.Controls.Add(this.gbStatus);
            this.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ZXP 1 & ZXP 3 C# Sample Code - Printer Search";
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.gbStatus.ResumeLayout(false);
            this.gbStatus.PerformLayout();
            this.gbPrinters.ResumeLayout(false);
            this.gbPrinters.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbStatus;
        private System.Windows.Forms.GroupBox gbPrinters;
        private System.Windows.Forms.ComboBox cboPrn;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lblVersions;
        private System.Windows.Forms.RadioButton rbUSB;
        private System.Windows.Forms.RadioButton rbEthernet;
        private System.Windows.Forms.Label lblConnectionType;
        private System.Windows.Forms.RadioButton rbDualSided;
        private System.Windows.Forms.RadioButton rbSingleSide;
        private System.Windows.Forms.GroupBox groupBox2;
    }
}

