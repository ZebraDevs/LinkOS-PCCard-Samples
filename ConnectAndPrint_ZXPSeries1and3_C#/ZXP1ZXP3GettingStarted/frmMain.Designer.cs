namespace ZXP1ZXP3GettingStarted
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
            this.cboPrinterSelection = new System.Windows.Forms.ComboBox();
            this.txtCardTextFront = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lblPrintStatus = new System.Windows.Forms.Label();
            this.btnPrint = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // cboPrinterSelection
            // 
            this.cboPrinterSelection.FormattingEnabled = true;
            this.cboPrinterSelection.Location = new System.Drawing.Point(117, 12);
            this.cboPrinterSelection.Name = "cboPrinterSelection";
            this.cboPrinterSelection.Size = new System.Drawing.Size(205, 21);
            this.cboPrinterSelection.TabIndex = 1;
            this.cboPrinterSelection.SelectedIndexChanged += new System.EventHandler(this.cboPrinterSelection_SelectedIndexChanged);
            // 
            // txtCardTextFront
            // 
            this.txtCardTextFront.Location = new System.Drawing.Point(117, 39);
            this.txtCardTextFront.Name = "txtCardTextFront";
            this.txtCardTextFront.Size = new System.Drawing.Size(205, 20);
            this.txtCardTextFront.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Card Text:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Printer:";
            // 
            // lblPrintStatus
            // 
            this.lblPrintStatus.AutoSize = true;
            this.lblPrintStatus.Location = new System.Drawing.Point(13, 77);
            this.lblPrintStatus.Name = "lblPrintStatus";
            this.lblPrintStatus.Size = new System.Drawing.Size(0, 13);
            this.lblPrintStatus.TabIndex = 6;
            // 
            // btnPrint
            // 
            this.btnPrint.Enabled = false;
            this.btnPrint.Location = new System.Drawing.Point(247, 227);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(75, 23);
            this.btnPrint.TabIndex = 7;
            this.btnPrint.Text = "Print";
            this.btnPrint.UseVisualStyleBackColor = true;
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(334, 262);
            this.Controls.Add(this.btnPrint);
            this.Controls.Add(this.lblPrintStatus);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtCardTextFront);
            this.Controls.Add(this.cboPrinterSelection);
            this.Name = "frmMain";
            this.Text = "ZXP1 ZXP3 Application";
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cboPrinterSelection;
        private System.Windows.Forms.TextBox txtCardTextFront;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblPrintStatus;
        private System.Windows.Forms.Button btnPrint;
    }
}

