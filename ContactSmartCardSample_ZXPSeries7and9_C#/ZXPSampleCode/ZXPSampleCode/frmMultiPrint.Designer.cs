namespace ZXPSampleCode
{
    partial class frmMultiPrint
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
            this.gb = new System.Windows.Forms.GroupBox();
            this.tbCount = new System.Windows.Forms.TextBox();
            this.lblCount = new System.Windows.Forms.Label();
            this.tbResults = new System.Windows.Forms.TextBox();
            this.gbButtons = new System.Windows.Forms.GroupBox();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnRun = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.gb.SuspendLayout();
            this.gbButtons.SuspendLayout();
            this.SuspendLayout();
            // 
            // gb
            // 
            this.gb.Controls.Add(this.tbResults);
            this.gb.Controls.Add(this.tbCount);
            this.gb.Controls.Add(this.lblCount);
            this.gb.Location = new System.Drawing.Point(10, 10);
            this.gb.Name = "gb";
            this.gb.Size = new System.Drawing.Size(559, 202);
            this.gb.TabIndex = 2;
            this.gb.TabStop = false;
            // 
            // tbCount
            // 
            this.tbCount.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbCount.Location = new System.Drawing.Point(70, 20);
            this.tbCount.Name = "tbCount";
            this.tbCount.Size = new System.Drawing.Size(50, 20);
            this.tbCount.TabIndex = 3;
            this.tbCount.Text = "1";
            // 
            // lblCount
            // 
            this.lblCount.AutoSize = true;
            this.lblCount.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCount.Location = new System.Drawing.Point(10, 20);
            this.lblCount.Name = "lblCount";
            this.lblCount.Size = new System.Drawing.Size(54, 16);
            this.lblCount.TabIndex = 2;
            this.lblCount.Text = "Count:";
            // 
            // tbResults
            // 
            this.tbResults.Location = new System.Drawing.Point(126, 20);
            this.tbResults.Multiline = true;
            this.tbResults.Name = "tbResults";
            this.tbResults.Size = new System.Drawing.Size(418, 176);
            this.tbResults.TabIndex = 4;
            // 
            // gbButtons
            // 
            this.gbButtons.Controls.Add(this.btnClose);
            this.gbButtons.Controls.Add(this.btnRun);
            this.gbButtons.Controls.Add(this.btnClear);
            this.gbButtons.Location = new System.Drawing.Point(10, 218);
            this.gbButtons.Name = "gbButtons";
            this.gbButtons.Size = new System.Drawing.Size(559, 50);
            this.gbButtons.TabIndex = 3;
            this.gbButtons.TabStop = false;
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(374, 15);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(50, 25);
            this.btnClear.TabIndex = 0;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnRun
            // 
            this.btnRun.Location = new System.Drawing.Point(434, 15);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(50, 25);
            this.btnRun.TabIndex = 1;
            this.btnRun.Text = "Run";
            this.btnRun.UseVisualStyleBackColor = true;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(494, 15);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(50, 25);
            this.btnClose.TabIndex = 2;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // frmMultiPrint
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(578, 274);
            this.ControlBox = false;
            this.Controls.Add(this.gbButtons);
            this.Controls.Add(this.gb);
            this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "frmMultiPrint";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Mutiple Print Form";
            this.Load += new System.EventHandler(this.frmMultiPrint_Load);
            this.gb.ResumeLayout(false);
            this.gb.PerformLayout();
            this.gbButtons.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gb;
        private System.Windows.Forms.TextBox tbResults;
        private System.Windows.Forms.TextBox tbCount;
        private System.Windows.Forms.Label lblCount;
        private System.Windows.Forms.GroupBox gbButtons;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnRun;
    }
}