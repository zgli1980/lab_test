namespace Bench_Test
{
    partial class NewLossComp
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
            this.btnCal = new System.Windows.Forms.Button();
            this.btnNext = new System.Windows.Forms.Button();
            this.dgvLossResult = new System.Windows.Forms.DataGridView();
            this.lblInfo = new System.Windows.Forms.Label();
            this.lblError = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLossResult)).BeginInit();
            this.SuspendLayout();
            // 
            // btnCal
            // 
            this.btnCal.Location = new System.Drawing.Point(388, 286);
            this.btnCal.Name = "btnCal";
            this.btnCal.Size = new System.Drawing.Size(75, 40);
            this.btnCal.TabIndex = 0;
            this.btnCal.Text = "Cal";
            this.btnCal.UseVisualStyleBackColor = true;
            this.btnCal.Click += new System.EventHandler(this.btnCal_Click);
            // 
            // btnNext
            // 
            this.btnNext.Location = new System.Drawing.Point(388, 332);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(75, 40);
            this.btnNext.TabIndex = 1;
            this.btnNext.Text = "Next";
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // dgvLossResult
            // 
            this.dgvLossResult.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvLossResult.Location = new System.Drawing.Point(1, 1);
            this.dgvLossResult.Name = "dgvLossResult";
            this.dgvLossResult.RowTemplate.Height = 23;
            this.dgvLossResult.Size = new System.Drawing.Size(473, 278);
            this.dgvLossResult.TabIndex = 2;
            this.dgvLossResult.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvLossResult_CellContentClick);
            // 
            // lblInfo
            // 
            this.lblInfo.Location = new System.Drawing.Point(13, 286);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(369, 72);
            this.lblInfo.TabIndex = 3;
            this.lblInfo.Text = "label1";
            // 
            // lblError
            // 
            this.lblError.AutoSize = true;
            this.lblError.Location = new System.Drawing.Point(13, 362);
            this.lblError.Name = "lblError";
            this.lblError.Size = new System.Drawing.Size(41, 12);
            this.lblError.TabIndex = 4;
            this.lblError.Text = "label1";
            // 
            // NewLossComp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(475, 380);
            this.Controls.Add(this.lblError);
            this.Controls.Add(this.lblInfo);
            this.Controls.Add(this.dgvLossResult);
            this.Controls.Add(this.btnNext);
            this.Controls.Add(this.btnCal);
            this.Name = "NewLossComp";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "NewLossComp";
            ((System.ComponentModel.ISupportInitialize)(this.dgvLossResult)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCal;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.DataGridView dgvLossResult;
        private System.Windows.Forms.Label lblInfo;
        private System.Windows.Forms.Label lblError;
    }
}