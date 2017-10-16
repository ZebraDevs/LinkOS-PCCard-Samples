namespace CSharpContactSmartCardSample
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
            this.btnSubmit = new System.Windows.Forms.Button();
            this.gbPrinters = new System.Windows.Forms.GroupBox();
            this.cboPrn = new System.Windows.Forms.ComboBox();
            this.gbPrint = new System.Windows.Forms.GroupBox();
            this.cbBack = new System.Windows.Forms.CheckBox();
            this.cbFront = new System.Windows.Forms.CheckBox();
            this.gbMag = new System.Windows.Forms.GroupBox();
            this.cbMag = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblVersions = new System.Windows.Forms.Label();
            this.gbSmartCard = new System.Windows.Forms.GroupBox();
            this.cbContact = new System.Windows.Forms.CheckBox();
            this.gbStatus.SuspendLayout();
            this.gbPrinters.SuspendLayout();
            this.gbPrint.SuspendLayout();
            this.gbMag.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.gbSmartCard.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbStatus
            // 
            this.gbStatus.Controls.Add(this.lblStatus);
            this.gbStatus.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.gbStatus.Location = new System.Drawing.Point(10, 351);
            this.gbStatus.Name = "gbStatus";
            this.gbStatus.Size = new System.Drawing.Size(570, 85);
            this.gbStatus.TabIndex = 13;
            this.gbStatus.TabStop = false;
            this.gbStatus.Text = "Status:";
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.lblStatus.Location = new System.Drawing.Point(16, 21);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(63, 16);
            this.lblStatus.TabIndex = 5;
            this.lblStatus.Text = "lblStatus";
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(497, 442);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(65, 23);
            this.btnExit.TabIndex = 15;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnSubmit
            // 
            this.btnSubmit.Location = new System.Drawing.Point(420, 442);
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
            this.gbPrinters.Location = new System.Drawing.Point(10, 78);
            this.gbPrinters.Name = "gbPrinters";
            this.gbPrinters.Size = new System.Drawing.Size(570, 61);
            this.gbPrinters.TabIndex = 0;
            this.gbPrinters.TabStop = false;
            this.gbPrinters.Text = "Printers";
            // 
            // cboPrn
            // 
            this.cboPrn.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboPrn.Location = new System.Drawing.Point(40, 23);
            this.cboPrn.Name = "cboPrn";
            this.cboPrn.Size = new System.Drawing.Size(284, 22);
            this.cboPrn.TabIndex = 1;
            this.cboPrn.SelectedIndexChanged += new System.EventHandler(this.cboPrn_SelectedIndexChanged);
            // 
            // gbPrint
            // 
            this.gbPrint.Controls.Add(this.cbBack);
            this.gbPrint.Controls.Add(this.cbFront);
            this.gbPrint.Enabled = false;
            this.gbPrint.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbPrint.Location = new System.Drawing.Point(10, 144);
            this.gbPrint.Name = "gbPrint";
            this.gbPrint.Size = new System.Drawing.Size(570, 64);
            this.gbPrint.TabIndex = 2;
            this.gbPrint.TabStop = false;
            this.gbPrint.Text = "Print Selections:";
            // 
            // cbBack
            // 
            this.cbBack.AutoSize = true;
            this.cbBack.Enabled = false;
            this.cbBack.Location = new System.Drawing.Point(150, 30);
            this.cbBack.Name = "cbBack";
            this.cbBack.Size = new System.Drawing.Size(58, 20);
            this.cbBack.TabIndex = 4;
            this.cbBack.Text = "Back";
            this.cbBack.UseVisualStyleBackColor = true;
            this.cbBack.CheckedChanged += new System.EventHandler(this.cbBack_CheckedChanged);
            // 
            // cbFront
            // 
            this.cbFront.AutoSize = true;
            this.cbFront.Enabled = false;
            this.cbFront.Location = new System.Drawing.Point(50, 30);
            this.cbFront.Name = "cbFront";
            this.cbFront.Size = new System.Drawing.Size(60, 20);
            this.cbFront.TabIndex = 3;
            this.cbFront.Text = "Front";
            this.cbFront.UseVisualStyleBackColor = true;
            this.cbFront.CheckedChanged += new System.EventHandler(this.cbFront_CheckedChanged);
            // 
            // gbMag
            // 
            this.gbMag.Controls.Add(this.cbMag);
            this.gbMag.Enabled = false;
            this.gbMag.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbMag.Location = new System.Drawing.Point(10, 213);
            this.gbMag.Name = "gbMag";
            this.gbMag.Size = new System.Drawing.Size(570, 64);
            this.gbMag.TabIndex = 5;
            this.gbMag.TabStop = false;
            this.gbMag.Text = "Magnetic Encoder:";
            // 
            // cbMag
            // 
            this.cbMag.AutoSize = true;
            this.cbMag.Enabled = false;
            this.cbMag.Location = new System.Drawing.Point(50, 30);
            this.cbMag.Name = "cbMag";
            this.cbMag.Size = new System.Drawing.Size(66, 20);
            this.cbMag.TabIndex = 6;
            this.cbMag.Text = "Check";
            this.cbMag.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblVersions);
            this.groupBox1.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(10, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(570, 61);
            this.groupBox1.TabIndex = 14;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "SDKS In Use:";
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
            // gbSmartCard
            // 
            this.gbSmartCard.Controls.Add(this.cbContact);
            this.gbSmartCard.Enabled = false;
            this.gbSmartCard.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbSmartCard.Location = new System.Drawing.Point(10, 283);
            this.gbSmartCard.Name = "gbSmartCard";
            this.gbSmartCard.Size = new System.Drawing.Size(570, 64);
            this.gbSmartCard.TabIndex = 16;
            this.gbSmartCard.TabStop = false;
            this.gbSmartCard.Text = "Smart Card Selection:";
            // 
            // cbContact
            // 
            this.cbContact.AutoSize = true;
            this.cbContact.Enabled = false;
            this.cbContact.Location = new System.Drawing.Point(50, 29);
            this.cbContact.Name = "cbContact";
            this.cbContact.Size = new System.Drawing.Size(75, 20);
            this.cbContact.TabIndex = 7;
            this.cbContact.Text = "Contact";
            this.cbContact.UseVisualStyleBackColor = true;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(590, 477);
            this.Controls.Add(this.gbSmartCard);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnSubmit);
            this.Controls.Add(this.gbMag);
            this.Controls.Add(this.gbPrint);
            this.Controls.Add(this.gbPrinters);
            this.Controls.Add(this.gbStatus);
            this.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ZXP 3 C# Sample Code - Printing, Magnetic Encoding and Contact RFID";
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.gbStatus.ResumeLayout(false);
            this.gbStatus.PerformLayout();
            this.gbPrinters.ResumeLayout(false);
            this.gbPrint.ResumeLayout(false);
            this.gbPrint.PerformLayout();
            this.gbMag.ResumeLayout(false);
            this.gbMag.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.gbSmartCard.ResumeLayout(false);
            this.gbSmartCard.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbStatus;
        private System.Windows.Forms.GroupBox gbPrinters;
        private System.Windows.Forms.ComboBox cboPrn;
        private System.Windows.Forms.GroupBox gbPrint;
        private System.Windows.Forms.CheckBox cbBack;
        private System.Windows.Forms.CheckBox cbFront;
        private System.Windows.Forms.GroupBox gbMag;
        private System.Windows.Forms.CheckBox cbMag;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Button btnSubmit;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lblVersions;
        private System.Windows.Forms.GroupBox gbSmartCard;
        private System.Windows.Forms.CheckBox cbContact;
    }
}

