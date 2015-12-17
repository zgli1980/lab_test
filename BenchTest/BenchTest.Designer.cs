using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using Vanchip.Testing;


namespace Bench_Test
{
    partial class BenchTest
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
            this.btnTest = new System.Windows.Forms.Button();
            this.lblErr = new System.Windows.Forms.Label();
            this.dgvResult = new System.Windows.Forms.DataGridView();
            this.btnSave = new System.Windows.Forms.Button();
            this.rbnLB = new System.Windows.Forms.RadioButton();
            this.rbnHB = new System.Windows.Forms.RadioButton();
            this.btnNext = new System.Windows.Forms.Button();
            this.chxDataFormat = new System.Windows.Forms.CheckBox();
            this.rbnWCDMA = new System.Windows.Forms.RadioButton();
            this.rbnTD = new System.Windows.Forms.RadioButton();
            this.rbnEDGELB = new System.Windows.Forms.RadioButton();
            this.rbnEDGEHB = new System.Windows.Forms.RadioButton();
            this.rbnRX = new System.Windows.Forms.RadioButton();
            this.gbxDisplay = new System.Windows.Forms.GroupBox();
            this.rbnVSAOFF = new System.Windows.Forms.RadioButton();
            this.rbnDisplayON = new System.Windows.Forms.RadioButton();
            this.lblRev = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbnEVDO = new System.Windows.Forms.RadioButton();
            this.rbnCDMA = new System.Windows.Forms.RadioButton();
            this.rbnLTEHB = new System.Windows.Forms.RadioButton();
            this.rbnTDDHB = new System.Windows.Forms.RadioButton();
            this.rbnTDDLB = new System.Windows.Forms.RadioButton();
            this.rbnLTELB = new System.Windows.Forms.RadioButton();
            this.lblDataTool = new System.Windows.Forms.LinkLabel();
            this.btnStop = new System.Windows.Forms.Button();
            this.cbxMIPI = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgvResult)).BeginInit();
            this.gbxDisplay.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnTest
            // 
            this.btnTest.Location = new System.Drawing.Point(594, 491);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(160, 56);
            this.btnTest.TabIndex = 0;
            this.btnTest.Text = "Test";
            this.btnTest.UseVisualStyleBackColor = true;
            this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
            // 
            // lblErr
            // 
            this.lblErr.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblErr.ForeColor = System.Drawing.Color.Red;
            this.lblErr.Location = new System.Drawing.Point(0, 590);
            this.lblErr.Name = "lblErr";
            this.lblErr.Size = new System.Drawing.Size(763, 13);
            this.lblErr.TabIndex = 1;
            this.lblErr.Text = "lblErr";
            // 
            // dgvResult
            // 
            this.dgvResult.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvResult.Location = new System.Drawing.Point(1, 1);
            this.dgvResult.Name = "dgvResult";
            this.dgvResult.RowTemplate.Height = 23;
            this.dgvResult.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dgvResult.Size = new System.Drawing.Size(587, 586);
            this.dgvResult.TabIndex = 2;
            this.dgvResult.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.gvResult_CellFormatting);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(705, 553);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(49, 34);
            this.btnSave.TabIndex = 4;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // rbnLB
            // 
            this.rbnLB.AutoSize = true;
            this.rbnLB.Checked = true;
            this.rbnLB.Location = new System.Drawing.Point(14, 41);
            this.rbnLB.Name = "rbnLB";
            this.rbnLB.Size = new System.Drawing.Size(65, 16);
            this.rbnLB.TabIndex = 5;
            this.rbnLB.TabStop = true;
            this.rbnLB.Text = "GMSK_LB";
            this.rbnLB.UseVisualStyleBackColor = true;
            this.rbnLB.CheckedChanged += new System.EventHandler(this.RadioButton_CheckChanged);
            // 
            // rbnHB
            // 
            this.rbnHB.AutoSize = true;
            this.rbnHB.Location = new System.Drawing.Point(92, 41);
            this.rbnHB.Name = "rbnHB";
            this.rbnHB.Size = new System.Drawing.Size(65, 16);
            this.rbnHB.TabIndex = 6;
            this.rbnHB.Text = "GMSK_HB";
            this.rbnHB.UseVisualStyleBackColor = true;
            this.rbnHB.CheckedChanged += new System.EventHandler(this.RadioButton_CheckChanged);
            // 
            // btnNext
            // 
            this.btnNext.Location = new System.Drawing.Point(595, 553);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(49, 34);
            this.btnNext.TabIndex = 8;
            this.btnNext.Text = "Next";
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // chxDataFormat
            // 
            this.chxDataFormat.AutoSize = true;
            this.chxDataFormat.Location = new System.Drawing.Point(661, 80);
            this.chxDataFormat.Name = "chxDataFormat";
            this.chxDataFormat.Size = new System.Drawing.Size(102, 16);
            this.chxDataFormat.TabIndex = 9;
            this.chxDataFormat.Text = "Show Data Log";
            this.chxDataFormat.UseVisualStyleBackColor = true;
            this.chxDataFormat.Visible = false;
            this.chxDataFormat.CheckedChanged += new System.EventHandler(this.chxDataFormat_CheckedChanged);
            // 
            // rbnWCDMA
            // 
            this.rbnWCDMA.AutoSize = true;
            this.rbnWCDMA.Location = new System.Drawing.Point(14, 85);
            this.rbnWCDMA.Name = "rbnWCDMA";
            this.rbnWCDMA.Size = new System.Drawing.Size(53, 16);
            this.rbnWCDMA.TabIndex = 11;
            this.rbnWCDMA.Text = "WCDMA";
            this.rbnWCDMA.UseVisualStyleBackColor = true;
            this.rbnWCDMA.CheckedChanged += new System.EventHandler(this.RadioButton_CheckChanged);
            // 
            // rbnTD
            // 
            this.rbnTD.AutoSize = true;
            this.rbnTD.Location = new System.Drawing.Point(14, 107);
            this.rbnTD.Name = "rbnTD";
            this.rbnTD.Size = new System.Drawing.Size(65, 16);
            this.rbnTD.TabIndex = 12;
            this.rbnTD.Text = "TDSCDMA";
            this.rbnTD.UseVisualStyleBackColor = true;
            this.rbnTD.CheckedChanged += new System.EventHandler(this.RadioButton_CheckChanged);
            // 
            // rbnEDGELB
            // 
            this.rbnEDGELB.AutoSize = true;
            this.rbnEDGELB.Location = new System.Drawing.Point(14, 63);
            this.rbnEDGELB.Name = "rbnEDGELB";
            this.rbnEDGELB.Size = new System.Drawing.Size(65, 16);
            this.rbnEDGELB.TabIndex = 13;
            this.rbnEDGELB.Text = "EDGE_LB";
            this.rbnEDGELB.UseVisualStyleBackColor = true;
            this.rbnEDGELB.CheckedChanged += new System.EventHandler(this.RadioButton_CheckChanged);
            // 
            // rbnEDGEHB
            // 
            this.rbnEDGEHB.AutoSize = true;
            this.rbnEDGEHB.Location = new System.Drawing.Point(92, 63);
            this.rbnEDGEHB.Name = "rbnEDGEHB";
            this.rbnEDGEHB.Size = new System.Drawing.Size(65, 16);
            this.rbnEDGEHB.TabIndex = 14;
            this.rbnEDGEHB.Text = "EDGE_HB";
            this.rbnEDGEHB.UseVisualStyleBackColor = true;
            this.rbnEDGEHB.CheckedChanged += new System.EventHandler(this.RadioButton_CheckChanged);
            // 
            // rbnRX
            // 
            this.rbnRX.AutoSize = true;
            this.rbnRX.Location = new System.Drawing.Point(14, 20);
            this.rbnRX.Name = "rbnRX";
            this.rbnRX.Size = new System.Drawing.Size(107, 16);
            this.rbnRX.TabIndex = 15;
            this.rbnRX.Text = "INSERTION LOSS";
            this.rbnRX.UseVisualStyleBackColor = true;
            this.rbnRX.CheckedChanged += new System.EventHandler(this.RadioButton_CheckChanged);
            // 
            // gbxDisplay
            // 
            this.gbxDisplay.Controls.Add(this.rbnVSAOFF);
            this.gbxDisplay.Controls.Add(this.rbnDisplayON);
            this.gbxDisplay.Location = new System.Drawing.Point(595, 12);
            this.gbxDisplay.Name = "gbxDisplay";
            this.gbxDisplay.Size = new System.Drawing.Size(163, 46);
            this.gbxDisplay.TabIndex = 16;
            this.gbxDisplay.TabStop = false;
            this.gbxDisplay.Text = "Display";
            // 
            // rbnVSAOFF
            // 
            this.rbnVSAOFF.AutoSize = true;
            this.rbnVSAOFF.Checked = true;
            this.rbnVSAOFF.Location = new System.Drawing.Point(92, 20);
            this.rbnVSAOFF.Name = "rbnVSAOFF";
            this.rbnVSAOFF.Size = new System.Drawing.Size(41, 16);
            this.rbnVSAOFF.TabIndex = 1;
            this.rbnVSAOFF.TabStop = true;
            this.rbnVSAOFF.Text = "OFF";
            this.rbnVSAOFF.UseVisualStyleBackColor = true;
            // 
            // rbnDisplayON
            // 
            this.rbnDisplayON.AutoSize = true;
            this.rbnDisplayON.Location = new System.Drawing.Point(32, 20);
            this.rbnDisplayON.Name = "rbnDisplayON";
            this.rbnDisplayON.Size = new System.Drawing.Size(35, 16);
            this.rbnDisplayON.TabIndex = 0;
            this.rbnDisplayON.TabStop = true;
            this.rbnDisplayON.Text = "ON";
            this.rbnDisplayON.UseVisualStyleBackColor = true;
            this.rbnDisplayON.CheckedChanged += new System.EventHandler(this.rbnVSAON_CheckedChanged);
            // 
            // lblRev
            // 
            this.lblRev.AutoSize = true;
            this.lblRev.Location = new System.Drawing.Point(639, 109);
            this.lblRev.Name = "lblRev";
            this.lblRev.Size = new System.Drawing.Size(41, 12);
            this.lblRev.TabIndex = 17;
            this.lblRev.Text = "lblRev";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cbxMIPI);
            this.groupBox1.Controls.Add(this.rbnEVDO);
            this.groupBox1.Controls.Add(this.rbnCDMA);
            this.groupBox1.Controls.Add(this.rbnLTEHB);
            this.groupBox1.Controls.Add(this.rbnTDDHB);
            this.groupBox1.Controls.Add(this.rbnTDDLB);
            this.groupBox1.Controls.Add(this.rbnLTELB);
            this.groupBox1.Controls.Add(this.rbnLB);
            this.groupBox1.Controls.Add(this.rbnHB);
            this.groupBox1.Controls.Add(this.rbnWCDMA);
            this.groupBox1.Controls.Add(this.rbnRX);
            this.groupBox1.Controls.Add(this.rbnTD);
            this.groupBox1.Controls.Add(this.rbnEDGEHB);
            this.groupBox1.Controls.Add(this.rbnEDGELB);
            this.groupBox1.Location = new System.Drawing.Point(591, 188);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(163, 297);
            this.groupBox1.TabIndex = 18;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Test";
            // 
            // rbnEVDO
            // 
            this.rbnEVDO.AutoSize = true;
            this.rbnEVDO.Location = new System.Drawing.Point(92, 173);
            this.rbnEVDO.Name = "rbnEVDO";
            this.rbnEVDO.Size = new System.Drawing.Size(47, 16);
            this.rbnEVDO.TabIndex = 21;
            this.rbnEVDO.Text = "EVDO";
            this.rbnEVDO.UseVisualStyleBackColor = true;
            this.rbnEVDO.CheckedChanged += new System.EventHandler(this.RadioButton_CheckChanged);
            // 
            // rbnCDMA
            // 
            this.rbnCDMA.AutoSize = true;
            this.rbnCDMA.Location = new System.Drawing.Point(14, 173);
            this.rbnCDMA.Name = "rbnCDMA";
            this.rbnCDMA.Size = new System.Drawing.Size(47, 16);
            this.rbnCDMA.TabIndex = 20;
            this.rbnCDMA.Text = "CDMA";
            this.rbnCDMA.UseVisualStyleBackColor = true;
            this.rbnCDMA.CheckedChanged += new System.EventHandler(this.RadioButton_CheckChanged);
            // 
            // rbnLTEHB
            // 
            this.rbnLTEHB.AutoSize = true;
            this.rbnLTEHB.Location = new System.Drawing.Point(92, 129);
            this.rbnLTEHB.Name = "rbnLTEHB";
            this.rbnLTEHB.Size = new System.Drawing.Size(59, 16);
            this.rbnLTEHB.TabIndex = 19;
            this.rbnLTEHB.Text = "LTE_HB";
            this.rbnLTEHB.UseVisualStyleBackColor = true;
            this.rbnLTEHB.CheckedChanged += new System.EventHandler(this.RadioButton_CheckChanged);
            // 
            // rbnTDDHB
            // 
            this.rbnTDDHB.AutoSize = true;
            this.rbnTDDHB.Location = new System.Drawing.Point(92, 151);
            this.rbnTDDHB.Name = "rbnTDDHB";
            this.rbnTDDHB.Size = new System.Drawing.Size(59, 16);
            this.rbnTDDHB.TabIndex = 18;
            this.rbnTDDHB.Text = "TDD_HB";
            this.rbnTDDHB.UseVisualStyleBackColor = true;
            this.rbnTDDHB.CheckedChanged += new System.EventHandler(this.RadioButton_CheckChanged);
            // 
            // rbnTDDLB
            // 
            this.rbnTDDLB.AutoSize = true;
            this.rbnTDDLB.Location = new System.Drawing.Point(14, 151);
            this.rbnTDDLB.Name = "rbnTDDLB";
            this.rbnTDDLB.Size = new System.Drawing.Size(59, 16);
            this.rbnTDDLB.TabIndex = 17;
            this.rbnTDDLB.Text = "TDD_LB";
            this.rbnTDDLB.UseVisualStyleBackColor = true;
            this.rbnTDDLB.CheckedChanged += new System.EventHandler(this.RadioButton_CheckChanged);
            // 
            // rbnLTELB
            // 
            this.rbnLTELB.AutoSize = true;
            this.rbnLTELB.Location = new System.Drawing.Point(14, 129);
            this.rbnLTELB.Name = "rbnLTELB";
            this.rbnLTELB.Size = new System.Drawing.Size(59, 16);
            this.rbnLTELB.TabIndex = 16;
            this.rbnLTELB.Text = "LTE_LB";
            this.rbnLTELB.UseVisualStyleBackColor = true;
            this.rbnLTELB.CheckedChanged += new System.EventHandler(this.RadioButton_CheckChanged);
            // 
            // lblDataTool
            // 
            this.lblDataTool.AutoSize = true;
            this.lblDataTool.Location = new System.Drawing.Point(705, 65);
            this.lblDataTool.Name = "lblDataTool";
            this.lblDataTool.Size = new System.Drawing.Size(53, 12);
            this.lblDataTool.TabIndex = 19;
            this.lblDataTool.TabStop = true;
            this.lblDataTool.Text = "DataTool";
            this.lblDataTool.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lblDataTool_LinkClicked);
            // 
            // btnStop
            // 
            this.btnStop.Enabled = false;
            this.btnStop.Location = new System.Drawing.Point(650, 553);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(49, 34);
            this.btnStop.TabIndex = 20;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            // 
            // cbxMIPI
            // 
            this.cbxMIPI.AutoSize = true;
            this.cbxMIPI.Location = new System.Drawing.Point(14, 275);
            this.cbxMIPI.Name = "cbxMIPI";
            this.cbxMIPI.Size = new System.Drawing.Size(48, 16);
            this.cbxMIPI.TabIndex = 22;
            this.cbxMIPI.Text = "MIPI";
            this.cbxMIPI.UseVisualStyleBackColor = true;
            // 
            // BenchTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(763, 603);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.lblDataTool);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.lblRev);
            this.Controls.Add(this.gbxDisplay);
            this.Controls.Add(this.chxDataFormat);
            this.Controls.Add(this.dgvResult);
            this.Controls.Add(this.lblErr);
            this.Controls.Add(this.btnTest);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnNext);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "BenchTest";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Vanchip Bench Test";
            this.Load += new System.EventHandler(this.BenchTest_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvResult)).EndInit();
            this.gbxDisplay.ResumeLayout(false);
            this.gbxDisplay.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnTest;
        private System.Windows.Forms.DataGridView dgvResult;
        private System.Windows.Forms.Label lblErr;
        private System.Windows.Forms.RadioButton rbnHB;
        private System.Windows.Forms.RadioButton rbnLB;
        private CheckBox chxDataFormat;
        private RadioButton rbnWCDMA;
        private RadioButton rbnTD;
        private RadioButton rbnEDGELB;
        private RadioButton rbnEDGEHB;
        private RadioButton rbnRX;
        private GroupBox gbxDisplay;
        private RadioButton rbnVSAOFF;
        private RadioButton rbnDisplayON;
        private Label lblRev;
        private GroupBox groupBox1;
        private LinkLabel lblDataTool;
        private RadioButton rbnEVDO;
        private RadioButton rbnCDMA;
        private RadioButton rbnLTEHB;
        private RadioButton rbnTDDHB;
        private RadioButton rbnTDDLB;
        private RadioButton rbnLTELB;
        private Button btnStop;
        private CheckBox cbxMIPI;
    }
}