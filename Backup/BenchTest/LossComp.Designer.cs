namespace Bench_Test
{
    partial class LossComp
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
            this.lblStep = new System.Windows.Forms.Label();
            this.btnMeasure = new System.Windows.Forms.Button();
            this.btnNext = new System.Windows.Forms.Button();
            this.dgvResult = new System.Windows.Forms.DataGridView();
            this.lblTime = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvResult)).BeginInit();
            this.SuspendLayout();
            // 
            // lblStep
            // 
            this.lblStep.Location = new System.Drawing.Point(12, 245);
            this.lblStep.Name = "lblStep";
            this.lblStep.Size = new System.Drawing.Size(402, 62);
            this.lblStep.TabIndex = 0;
            this.lblStep.Text = "Step";
            // 
            // btnMeasure
            // 
            this.btnMeasure.Location = new System.Drawing.Point(288, 309);
            this.btnMeasure.Name = "btnMeasure";
            this.btnMeasure.Size = new System.Drawing.Size(60, 23);
            this.btnMeasure.TabIndex = 1;
            this.btnMeasure.Text = "Cal";
            this.btnMeasure.UseVisualStyleBackColor = true;
            this.btnMeasure.Click += new System.EventHandler(this.btnMeasure_Click);
            // 
            // btnNext
            // 
            this.btnNext.Location = new System.Drawing.Point(354, 309);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(60, 23);
            this.btnNext.TabIndex = 2;
            this.btnNext.Text = "Next";
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // dgvResult
            // 
            this.dgvResult.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvResult.Location = new System.Drawing.Point(-2, -1);
            this.dgvResult.Name = "dgvResult";
            this.dgvResult.RowTemplate.Height = 23;
            this.dgvResult.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dgvResult.Size = new System.Drawing.Size(427, 237);
            this.dgvResult.TabIndex = 3;
            this.dgvResult.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dgvResult_CellFormatting);
            // 
            // lblTime
            // 
            this.lblTime.AutoSize = true;
            this.lblTime.Location = new System.Drawing.Point(14, 311);
            this.lblTime.Name = "lblTime";
            this.lblTime.Size = new System.Drawing.Size(0, 12);
            this.lblTime.TabIndex = 4;
            // 
            // LossCompensation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(424, 338);
            this.Controls.Add(this.lblTime);
            this.Controls.Add(this.dgvResult);
            this.Controls.Add(this.btnNext);
            this.Controls.Add(this.btnMeasure);
            this.Controls.Add(this.lblStep);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LossCompensation";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "LossComp";
            this.Load += new System.EventHandler(this.LossComp_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvResult)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblStep;
        private System.Windows.Forms.Button btnMeasure;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.DataGridView dgvResult;
        private System.Windows.Forms.Label lblTime;
    }
}