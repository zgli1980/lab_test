namespace Bench_Test
{
    partial class Start
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
            this.lbxProduct = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnStart = new System.Windows.Forms.Button();
            this.cbxDUT = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // lbxProduct
            // 
            this.lbxProduct.FormattingEnabled = true;
            this.lbxProduct.ItemHeight = 12;
            this.lbxProduct.Items.AddRange(new object[] {
            "Manual"});
            this.lbxProduct.Location = new System.Drawing.Point(23, 35);
            this.lbxProduct.Name = "lbxProduct";
            this.lbxProduct.Size = new System.Drawing.Size(118, 148);
            this.lbxProduct.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(185, 12);
            this.label1.TabIndex = 3;
            this.label1.Text = "Select a test program to start";
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(182, 81);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(66, 58);
            this.btnStart.TabIndex = 4;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // cbxDUT
            // 
            this.cbxDUT.AutoSize = true;
            this.cbxDUT.Location = new System.Drawing.Point(12, 196);
            this.cbxDUT.Name = "cbxDUT";
            this.cbxDUT.Size = new System.Drawing.Size(144, 16);
            this.cbxDUT.TabIndex = 5;
            this.cbxDUT.Text = "Use production board";
            this.cbxDUT.UseVisualStyleBackColor = true;
            // 
            // Start
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 224);
            this.Controls.Add(this.cbxDUT);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lbxProduct);
            this.Name = "Start";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Start";
            this.Load += new System.EventHandler(this.Start_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox lbxProduct;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.CheckBox cbxDUT;
    }
}