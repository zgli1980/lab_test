namespace Bench_Test
{
    partial class SweepTest
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.btnTest = new System.Windows.Forms.Button();
            this.gpParameter = new System.Windows.Forms.GroupBox();
            this.lblHelp = new System.Windows.Forms.LinkLabel();
            this.tbxPin_Vramp = new System.Windows.Forms.TextBox();
            this.lblPin_Vramp = new System.Windows.Forms.Label();
            this.tbxVBAT = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnSettingSave = new System.Windows.Forms.Button();
            this.tbxFreqList = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.tbxStep = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tbxStop = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tbxStart = new System.Windows.Forms.TextBox();
            this.lblStep = new System.Windows.Forms.Label();
            this.tbxTXEN = new System.Windows.Forms.TextBox();
            this.lblTXEN = new System.Windows.Forms.Label();
            this.tbxGPCTRL = new System.Windows.Forms.TextBox();
            this.lblGPCTRL = new System.Windows.Forms.Label();
            this.tbxVCC = new System.Windows.Forms.TextBox();
            this.lblVCC = new System.Windows.Forms.Label();
            this.cbxKeepPrevious = new System.Windows.Forms.CheckBox();
            this.rbnCWLB = new System.Windows.Forms.RadioButton();
            this.rbnCWHB = new System.Windows.Forms.RadioButton();
            this.btnNext = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.dgvSweepResult = new System.Windows.Forms.DataGridView();
            this.lblError = new System.Windows.Forms.Label();
            this.rbnWCDMA = new System.Windows.Forms.RadioButton();
            this.rbnTDSCDMA = new System.Windows.Forms.RadioButton();
            this.rbnEDGELB = new System.Windows.Forms.RadioButton();
            this.cbxEnableSetting = new System.Windows.Forms.CheckBox();
            this.rbnEDGEHB = new System.Windows.Forms.RadioButton();
            this.lblCopyRight = new System.Windows.Forms.Label();
            this.btnStop = new System.Windows.Forms.Button();
            this.gbDisplay = new System.Windows.Forms.GroupBox();
            this.cbxMipi = new System.Windows.Forms.CheckBox();
            this.cbxexthar = new System.Windows.Forms.CheckBox();
            this.cbxBypassEVM = new System.Windows.Forms.CheckBox();
            this.label8 = new System.Windows.Forms.Label();
            this.rbnDisplayOFF = new System.Windows.Forms.RadioButton();
            this.rbnDisplayON = new System.Windows.Forms.RadioButton();
            this.gbMode = new System.Windows.Forms.GroupBox();
            this.rbnEVDo = new System.Windows.Forms.RadioButton();
            this.rbnCDMA = new System.Windows.Forms.RadioButton();
            this.rbnLTEFDDHB = new System.Windows.Forms.RadioButton();
            this.cbxWaveform = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.rbnLTETDD_B40 = new System.Windows.Forms.RadioButton();
            this.rbnLTEFDDLB = new System.Windows.Forms.RadioButton();
            this.rbnLTETDD_B38 = new System.Windows.Forms.RadioButton();
            this.rbnLCWLB = new System.Windows.Forms.RadioButton();
            this.rbnLCWHB = new System.Windows.Forms.RadioButton();
            this.gpParameter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSweepResult)).BeginInit();
            this.gbDisplay.SuspendLayout();
            this.gbMode.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnTest
            // 
            this.btnTest.BackColor = System.Drawing.Color.Green;
            this.btnTest.Location = new System.Drawing.Point(920, 659);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(218, 38);
            this.btnTest.TabIndex = 0;
            this.btnTest.Text = "Test";
            this.btnTest.UseVisualStyleBackColor = false;
            this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
            // 
            // gpParameter
            // 
            this.gpParameter.Controls.Add(this.lblHelp);
            this.gpParameter.Controls.Add(this.tbxPin_Vramp);
            this.gpParameter.Controls.Add(this.lblPin_Vramp);
            this.gpParameter.Controls.Add(this.tbxVBAT);
            this.gpParameter.Controls.Add(this.label2);
            this.gpParameter.Controls.Add(this.btnSettingSave);
            this.gpParameter.Controls.Add(this.tbxFreqList);
            this.gpParameter.Controls.Add(this.label6);
            this.gpParameter.Controls.Add(this.label5);
            this.gpParameter.Controls.Add(this.tbxStep);
            this.gpParameter.Controls.Add(this.label4);
            this.gpParameter.Controls.Add(this.tbxStop);
            this.gpParameter.Controls.Add(this.label3);
            this.gpParameter.Controls.Add(this.tbxStart);
            this.gpParameter.Controls.Add(this.lblStep);
            this.gpParameter.Controls.Add(this.tbxTXEN);
            this.gpParameter.Controls.Add(this.lblTXEN);
            this.gpParameter.Controls.Add(this.tbxGPCTRL);
            this.gpParameter.Controls.Add(this.lblGPCTRL);
            this.gpParameter.Controls.Add(this.tbxVCC);
            this.gpParameter.Controls.Add(this.lblVCC);
            this.gpParameter.Enabled = false;
            this.gpParameter.Location = new System.Drawing.Point(920, 13);
            this.gpParameter.Name = "gpParameter";
            this.gpParameter.Size = new System.Drawing.Size(218, 336);
            this.gpParameter.TabIndex = 1;
            this.gpParameter.TabStop = false;
            this.gpParameter.Text = "Parameter Setting";
            // 
            // lblHelp
            // 
            this.lblHelp.ActiveLinkColor = System.Drawing.Color.Black;
            this.lblHelp.AutoSize = true;
            this.lblHelp.Font = new System.Drawing.Font("SimSun", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblHelp.Location = new System.Drawing.Point(96, 311);
            this.lblHelp.Name = "lblHelp";
            this.lblHelp.Size = new System.Drawing.Size(12, 12);
            this.lblHelp.TabIndex = 111;
            this.lblHelp.TabStop = true;
            this.lblHelp.Text = "?";
            this.lblHelp.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lblHelp_LinkClicked);
            // 
            // tbxPin_Vramp
            // 
            this.tbxPin_Vramp.Location = new System.Drawing.Point(126, 109);
            this.tbxPin_Vramp.Name = "tbxPin_Vramp";
            this.tbxPin_Vramp.Size = new System.Drawing.Size(77, 20);
            this.tbxPin_Vramp.TabIndex = 105;
            this.tbxPin_Vramp.Text = "3";
            this.tbxPin_Vramp.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lblPin_Vramp
            // 
            this.lblPin_Vramp.Location = new System.Drawing.Point(8, 108);
            this.lblPin_Vramp.Name = "lblPin_Vramp";
            this.lblPin_Vramp.Size = new System.Drawing.Size(112, 23);
            this.lblPin_Vramp.TabIndex = 109;
            this.lblPin_Vramp.Text = "RF Pin(dBm)";
            this.lblPin_Vramp.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // tbxVBAT
            // 
            this.tbxVBAT.Location = new System.Drawing.Point(126, 38);
            this.tbxVBAT.Name = "tbxVBAT";
            this.tbxVBAT.Size = new System.Drawing.Size(77, 20);
            this.tbxVBAT.TabIndex = 102;
            this.tbxVBAT.Text = "1.8";
            this.tbxVBAT.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(8, 38);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(112, 23);
            this.label2.TabIndex = 109;
            this.label2.Text = "VBAT Voltage(V)";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnSettingSave
            // 
            this.btnSettingSave.Location = new System.Drawing.Point(113, 307);
            this.btnSettingSave.Name = "btnSettingSave";
            this.btnSettingSave.Size = new System.Drawing.Size(90, 23);
            this.btnSettingSave.TabIndex = 110;
            this.btnSettingSave.Text = "Save Setting";
            this.btnSettingSave.UseVisualStyleBackColor = true;
            this.btnSettingSave.Click += new System.EventHandler(this.btnSettingSave_Click);
            // 
            // tbxFreqList
            // 
            this.tbxFreqList.Location = new System.Drawing.Point(10, 244);
            this.tbxFreqList.Multiline = true;
            this.tbxFreqList.Name = "tbxFreqList";
            this.tbxFreqList.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbxFreqList.Size = new System.Drawing.Size(193, 56);
            this.tbxFreqList.TabIndex = 109;
            this.tbxFreqList.Text = "824,836,849,880,897,915";
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(6, 211);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(195, 29);
            this.label6.TabIndex = 13;
            this.label6.Text = "Frequency List (MHz)                ( Seperated by comma )";
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(81, 183);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(39, 23);
            this.label5.TabIndex = 12;
            this.label5.Text = "Step";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // tbxStep
            // 
            this.tbxStep.Location = new System.Drawing.Point(126, 184);
            this.tbxStep.Name = "tbxStep";
            this.tbxStep.Size = new System.Drawing.Size(77, 20);
            this.tbxStep.TabIndex = 108;
            this.tbxStep.Text = "0.1";
            this.tbxStep.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(81, 159);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(39, 23);
            this.label4.TabIndex = 10;
            this.label4.Text = "Stop";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // tbxStop
            // 
            this.tbxStop.Location = new System.Drawing.Point(126, 159);
            this.tbxStop.Name = "tbxStop";
            this.tbxStop.Size = new System.Drawing.Size(77, 20);
            this.tbxStop.TabIndex = 107;
            this.tbxStop.Text = "1.7";
            this.tbxStop.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(81, 135);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(39, 23);
            this.label3.TabIndex = 8;
            this.label3.Text = "Start";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // tbxStart
            // 
            this.tbxStart.Location = new System.Drawing.Point(126, 134);
            this.tbxStart.Name = "tbxStart";
            this.tbxStart.Size = new System.Drawing.Size(77, 20);
            this.tbxStart.TabIndex = 106;
            this.tbxStart.Text = "1.0";
            this.tbxStart.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lblStep
            // 
            this.lblStep.Location = new System.Drawing.Point(8, 138);
            this.lblStep.Name = "lblStep";
            this.lblStep.Size = new System.Drawing.Size(67, 68);
            this.lblStep.TabIndex = 6;
            this.lblStep.Text = "VRAMP Sweep Voltage (V)";
            this.lblStep.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tbxTXEN
            // 
            this.tbxTXEN.Location = new System.Drawing.Point(126, 86);
            this.tbxTXEN.Name = "tbxTXEN";
            this.tbxTXEN.Size = new System.Drawing.Size(77, 20);
            this.tbxTXEN.TabIndex = 104;
            this.tbxTXEN.Text = "1.8";
            this.tbxTXEN.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lblTXEN
            // 
            this.lblTXEN.Location = new System.Drawing.Point(8, 86);
            this.lblTXEN.Name = "lblTXEN";
            this.lblTXEN.Size = new System.Drawing.Size(112, 23);
            this.lblTXEN.TabIndex = 4;
            this.lblTXEN.Text = "TXEN Voltage(V)";
            this.lblTXEN.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // tbxGPCTRL
            // 
            this.tbxGPCTRL.Location = new System.Drawing.Point(126, 62);
            this.tbxGPCTRL.Name = "tbxGPCTRL";
            this.tbxGPCTRL.Size = new System.Drawing.Size(77, 20);
            this.tbxGPCTRL.TabIndex = 103;
            this.tbxGPCTRL.Text = "1.8";
            this.tbxGPCTRL.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lblGPCTRL
            // 
            this.lblGPCTRL.Location = new System.Drawing.Point(8, 62);
            this.lblGPCTRL.Name = "lblGPCTRL";
            this.lblGPCTRL.Size = new System.Drawing.Size(112, 23);
            this.lblGPCTRL.TabIndex = 2;
            this.lblGPCTRL.Text = "GPCTRL Voltage(V)";
            this.lblGPCTRL.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // tbxVCC
            // 
            this.tbxVCC.Location = new System.Drawing.Point(126, 14);
            this.tbxVCC.Name = "tbxVCC";
            this.tbxVCC.Size = new System.Drawing.Size(77, 20);
            this.tbxVCC.TabIndex = 101;
            this.tbxVCC.Text = "3.5";
            this.tbxVCC.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lblVCC
            // 
            this.lblVCC.Location = new System.Drawing.Point(8, 14);
            this.lblVCC.Name = "lblVCC";
            this.lblVCC.Size = new System.Drawing.Size(112, 23);
            this.lblVCC.TabIndex = 0;
            this.lblVCC.Text = "VCC Voltage(V)";
            this.lblVCC.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cbxKeepPrevious
            // 
            this.cbxKeepPrevious.AutoSize = true;
            this.cbxKeepPrevious.Location = new System.Drawing.Point(11, 38);
            this.cbxKeepPrevious.Name = "cbxKeepPrevious";
            this.cbxKeepPrevious.Size = new System.Drawing.Size(77, 17);
            this.cbxKeepPrevious.TabIndex = 13;
            this.cbxKeepPrevious.Text = "Keep Data";
            this.cbxKeepPrevious.UseVisualStyleBackColor = true;
            // 
            // rbnCWLB
            // 
            this.rbnCWLB.AutoSize = true;
            this.rbnCWLB.Location = new System.Drawing.Point(8, 17);
            this.rbnCWLB.Name = "rbnCWLB";
            this.rbnCWLB.Size = new System.Drawing.Size(56, 17);
            this.rbnCWLB.TabIndex = 2;
            this.rbnCWLB.Text = "CWLB";
            this.rbnCWLB.UseVisualStyleBackColor = true;
            // 
            // rbnCWHB
            // 
            this.rbnCWHB.AutoSize = true;
            this.rbnCWHB.Location = new System.Drawing.Point(8, 39);
            this.rbnCWHB.Name = "rbnCWHB";
            this.rbnCWHB.Size = new System.Drawing.Size(58, 17);
            this.rbnCWHB.TabIndex = 3;
            this.rbnCWHB.Text = "CWHB";
            this.rbnCWHB.UseVisualStyleBackColor = true;
            // 
            // btnNext
            // 
            this.btnNext.Enabled = false;
            this.btnNext.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnNext.Location = new System.Drawing.Point(920, 698);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(69, 38);
            this.btnNext.TabIndex = 4;
            this.btnNext.Text = "New";
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // btnSave
            // 
            this.btnSave.Enabled = false;
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnSave.Location = new System.Drawing.Point(1070, 698);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(68, 38);
            this.btnSave.TabIndex = 5;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // dgvSweepResult
            // 
            this.dgvSweepResult.AllowUserToAddRows = false;
            this.dgvSweepResult.AllowUserToDeleteRows = false;
            this.dgvSweepResult.AllowUserToResizeColumns = false;
            this.dgvSweepResult.AllowUserToResizeRows = false;
            this.dgvSweepResult.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("SimSun", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvSweepResult.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvSweepResult.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSweepResult.Location = new System.Drawing.Point(2, 1);
            this.dgvSweepResult.Name = "dgvSweepResult";
            this.dgvSweepResult.RowTemplate.Height = 23;
            this.dgvSweepResult.Size = new System.Drawing.Size(911, 729);
            this.dgvSweepResult.TabIndex = 6;
            // 
            // lblError
            // 
            this.lblError.Location = new System.Drawing.Point(0, 733);
            this.lblError.Name = "lblError";
            this.lblError.Size = new System.Drawing.Size(824, 23);
            this.lblError.TabIndex = 7;
            this.lblError.Text = "lblError";
            // 
            // rbnWCDMA
            // 
            this.rbnWCDMA.AutoSize = true;
            this.rbnWCDMA.Location = new System.Drawing.Point(8, 157);
            this.rbnWCDMA.Name = "rbnWCDMA";
            this.rbnWCDMA.Size = new System.Drawing.Size(67, 17);
            this.rbnWCDMA.TabIndex = 8;
            this.rbnWCDMA.Text = "WCDMA";
            this.rbnWCDMA.UseVisualStyleBackColor = true;
            // 
            // rbnTDSCDMA
            // 
            this.rbnTDSCDMA.AutoSize = true;
            this.rbnTDSCDMA.Location = new System.Drawing.Point(126, 62);
            this.rbnTDSCDMA.Name = "rbnTDSCDMA";
            this.rbnTDSCDMA.Size = new System.Drawing.Size(78, 17);
            this.rbnTDSCDMA.TabIndex = 9;
            this.rbnTDSCDMA.Text = "TDSCDMA";
            this.rbnTDSCDMA.UseVisualStyleBackColor = true;
            // 
            // rbnEDGELB
            // 
            this.rbnEDGELB.AutoSize = true;
            this.rbnEDGELB.Location = new System.Drawing.Point(8, 63);
            this.rbnEDGELB.Name = "rbnEDGELB";
            this.rbnEDGELB.Size = new System.Drawing.Size(68, 17);
            this.rbnEDGELB.TabIndex = 10;
            this.rbnEDGELB.Text = "EDGELB";
            this.rbnEDGELB.UseVisualStyleBackColor = true;
            // 
            // cbxEnableSetting
            // 
            this.cbxEnableSetting.AutoSize = true;
            this.cbxEnableSetting.Location = new System.Drawing.Point(1046, 1);
            this.cbxEnableSetting.Name = "cbxEnableSetting";
            this.cbxEnableSetting.Size = new System.Drawing.Size(95, 17);
            this.cbxEnableSetting.TabIndex = 0;
            this.cbxEnableSetting.Text = "Enable Setting";
            this.cbxEnableSetting.UseVisualStyleBackColor = true;
            this.cbxEnableSetting.CheckedChanged += new System.EventHandler(this.cbxEnableSetting_CheckedChanged);
            // 
            // rbnEDGEHB
            // 
            this.rbnEDGEHB.AutoSize = true;
            this.rbnEDGEHB.Location = new System.Drawing.Point(8, 87);
            this.rbnEDGEHB.Name = "rbnEDGEHB";
            this.rbnEDGEHB.Size = new System.Drawing.Size(70, 17);
            this.rbnEDGEHB.TabIndex = 11;
            this.rbnEDGEHB.Text = "EDGEHB";
            this.rbnEDGEHB.UseVisualStyleBackColor = true;
            // 
            // lblCopyRight
            // 
            this.lblCopyRight.Font = new System.Drawing.Font("SimSun", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblCopyRight.Location = new System.Drawing.Point(885, 740);
            this.lblCopyRight.Name = "lblCopyRight";
            this.lblCopyRight.Size = new System.Drawing.Size(257, 13);
            this.lblCopyRight.TabIndex = 12;
            this.lblCopyRight.Text = "Copyright @ 2012 Vanchip   Rev 2014-12-12";
            this.lblCopyRight.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // btnStop
            // 
            this.btnStop.BackColor = System.Drawing.SystemColors.Control;
            this.btnStop.Enabled = false;
            this.btnStop.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnStop.Location = new System.Drawing.Point(995, 698);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(69, 38);
            this.btnStop.TabIndex = 14;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = false;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // gbDisplay
            // 
            this.gbDisplay.Controls.Add(this.cbxMipi);
            this.gbDisplay.Controls.Add(this.cbxexthar);
            this.gbDisplay.Controls.Add(this.cbxBypassEVM);
            this.gbDisplay.Controls.Add(this.label8);
            this.gbDisplay.Controls.Add(this.rbnDisplayOFF);
            this.gbDisplay.Controls.Add(this.rbnDisplayON);
            this.gbDisplay.Controls.Add(this.cbxKeepPrevious);
            this.gbDisplay.Location = new System.Drawing.Point(919, 351);
            this.gbDisplay.Name = "gbDisplay";
            this.gbDisplay.Size = new System.Drawing.Size(219, 88);
            this.gbDisplay.TabIndex = 15;
            this.gbDisplay.TabStop = false;
            // 
            // cbxMipi
            // 
            this.cbxMipi.AutoSize = true;
            this.cbxMipi.Location = new System.Drawing.Point(11, 62);
            this.cbxMipi.Name = "cbxMipi";
            this.cbxMipi.Size = new System.Drawing.Size(45, 17);
            this.cbxMipi.TabIndex = 124;
            this.cbxMipi.Text = "Mipi";
            this.cbxMipi.UseVisualStyleBackColor = true;
            // 
            // cbxexthar
            // 
            this.cbxexthar.AutoSize = true;
            this.cbxexthar.Location = new System.Drawing.Point(90, 62);
            this.cbxexthar.Name = "cbxexthar";
            this.cbxexthar.Size = new System.Drawing.Size(70, 17);
            this.cbxexthar.TabIndex = 25;
            this.cbxexthar.Text = "Extra Har";
            this.cbxexthar.UseVisualStyleBackColor = true;
            this.cbxexthar.CheckedChanged += new System.EventHandler(this.cbxexthar_CheckedChanged);
            // 
            // cbxBypassEVM
            // 
            this.cbxBypassEVM.AutoSize = true;
            this.cbxBypassEVM.Checked = true;
            this.cbxBypassEVM.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbxBypassEVM.Location = new System.Drawing.Point(90, 38);
            this.cbxBypassEVM.Name = "cbxBypassEVM";
            this.cbxBypassEVM.Size = new System.Drawing.Size(86, 17);
            this.cbxBypassEVM.TabIndex = 123;
            this.cbxBypassEVM.Text = "Bypass EVM";
            this.cbxBypassEVM.UseVisualStyleBackColor = true;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(9, 16);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(65, 13);
            this.label8.TabIndex = 14;
            this.label8.Text = "VSA Display";
            // 
            // rbnDisplayOFF
            // 
            this.rbnDisplayOFF.AutoSize = true;
            this.rbnDisplayOFF.Location = new System.Drawing.Point(128, 14);
            this.rbnDisplayOFF.Name = "rbnDisplayOFF";
            this.rbnDisplayOFF.Size = new System.Drawing.Size(45, 17);
            this.rbnDisplayOFF.TabIndex = 1;
            this.rbnDisplayOFF.Text = "OFF";
            this.rbnDisplayOFF.UseVisualStyleBackColor = true;
            // 
            // rbnDisplayON
            // 
            this.rbnDisplayON.AutoSize = true;
            this.rbnDisplayON.Checked = true;
            this.rbnDisplayON.Location = new System.Drawing.Point(87, 14);
            this.rbnDisplayON.Name = "rbnDisplayON";
            this.rbnDisplayON.Size = new System.Drawing.Size(41, 17);
            this.rbnDisplayON.TabIndex = 121;
            this.rbnDisplayON.TabStop = true;
            this.rbnDisplayON.Text = "ON";
            this.rbnDisplayON.UseVisualStyleBackColor = true;
            this.rbnDisplayON.CheckedChanged += new System.EventHandler(this.rbnDisplayON_CheckedChanged);
            // 
            // gbMode
            // 
            this.gbMode.Controls.Add(this.rbnLCWHB);
            this.gbMode.Controls.Add(this.rbnLCWLB);
            this.gbMode.Controls.Add(this.rbnEVDo);
            this.gbMode.Controls.Add(this.rbnCDMA);
            this.gbMode.Controls.Add(this.rbnLTEFDDHB);
            this.gbMode.Controls.Add(this.cbxWaveform);
            this.gbMode.Controls.Add(this.label1);
            this.gbMode.Controls.Add(this.rbnLTETDD_B40);
            this.gbMode.Controls.Add(this.rbnLTEFDDLB);
            this.gbMode.Controls.Add(this.rbnLTETDD_B38);
            this.gbMode.Controls.Add(this.rbnEDGELB);
            this.gbMode.Controls.Add(this.rbnTDSCDMA);
            this.gbMode.Controls.Add(this.rbnWCDMA);
            this.gbMode.Controls.Add(this.rbnCWHB);
            this.gbMode.Controls.Add(this.rbnCWLB);
            this.gbMode.Controls.Add(this.rbnEDGEHB);
            this.gbMode.Location = new System.Drawing.Point(920, 443);
            this.gbMode.Name = "gbMode";
            this.gbMode.Size = new System.Drawing.Size(218, 220);
            this.gbMode.TabIndex = 16;
            this.gbMode.TabStop = false;
            this.gbMode.Text = "Mode";
            // 
            // rbnEVDo
            // 
            this.rbnEVDo.AutoSize = true;
            this.rbnEVDo.Location = new System.Drawing.Point(8, 134);
            this.rbnEVDo.Name = "rbnEVDo";
            this.rbnEVDo.Size = new System.Drawing.Size(55, 17);
            this.rbnEVDo.TabIndex = 24;
            this.rbnEVDo.Text = "EVDO";
            this.rbnEVDo.UseVisualStyleBackColor = true;
            // 
            // rbnCDMA
            // 
            this.rbnCDMA.AutoSize = true;
            this.rbnCDMA.Location = new System.Drawing.Point(8, 111);
            this.rbnCDMA.Name = "rbnCDMA";
            this.rbnCDMA.Size = new System.Drawing.Size(56, 17);
            this.rbnCDMA.TabIndex = 23;
            this.rbnCDMA.Text = "CDMA";
            this.rbnCDMA.UseVisualStyleBackColor = true;
            // 
            // rbnLTEFDDHB
            // 
            this.rbnLTEFDDHB.AutoSize = true;
            this.rbnLTEFDDHB.Location = new System.Drawing.Point(126, 157);
            this.rbnLTEFDDHB.Name = "rbnLTEFDDHB";
            this.rbnLTEFDDHB.Size = new System.Drawing.Size(82, 17);
            this.rbnLTEFDDHB.TabIndex = 22;
            this.rbnLTEFDDHB.Text = "LTEFDDHB";
            this.rbnLTEFDDHB.UseVisualStyleBackColor = true;
            // 
            // cbxWaveform
            // 
            this.cbxWaveform.AllowDrop = true;
            this.cbxWaveform.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxWaveform.FormattingEnabled = true;
            this.cbxWaveform.Location = new System.Drawing.Point(6, 192);
            this.cbxWaveform.Name = "cbxWaveform";
            this.cbxWaveform.Size = new System.Drawing.Size(207, 21);
            this.cbxWaveform.TabIndex = 17;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 177);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 13);
            this.label1.TabIndex = 18;
            this.label1.Text = "Select Waveform";
            // 
            // rbnLTETDD_B40
            // 
            this.rbnLTETDD_B40.AutoSize = true;
            this.rbnLTETDD_B40.Location = new System.Drawing.Point(126, 110);
            this.rbnLTETDD_B40.Name = "rbnLTETDD_B40";
            this.rbnLTETDD_B40.Size = new System.Drawing.Size(93, 17);
            this.rbnLTETDD_B40.TabIndex = 21;
            this.rbnLTETDD_B40.Text = "LTETDD(B40)";
            this.rbnLTETDD_B40.UseVisualStyleBackColor = true;
            // 
            // rbnLTEFDDLB
            // 
            this.rbnLTEFDDLB.AutoSize = true;
            this.rbnLTEFDDLB.Location = new System.Drawing.Point(126, 134);
            this.rbnLTEFDDLB.Name = "rbnLTEFDDLB";
            this.rbnLTEFDDLB.Size = new System.Drawing.Size(80, 17);
            this.rbnLTEFDDLB.TabIndex = 20;
            this.rbnLTEFDDLB.Text = "LTEFDDLB";
            this.rbnLTEFDDLB.UseVisualStyleBackColor = true;
            // 
            // rbnLTETDD_B38
            // 
            this.rbnLTETDD_B38.AutoSize = true;
            this.rbnLTETDD_B38.Location = new System.Drawing.Point(126, 86);
            this.rbnLTETDD_B38.Name = "rbnLTETDD_B38";
            this.rbnLTETDD_B38.Size = new System.Drawing.Size(93, 17);
            this.rbnLTETDD_B38.TabIndex = 19;
            this.rbnLTETDD_B38.Text = "LTETDD(B38)";
            this.rbnLTETDD_B38.UseVisualStyleBackColor = true;
            // 
            // rbnLCWLB
            // 
            this.rbnLCWLB.AutoSize = true;
            this.rbnLCWLB.Location = new System.Drawing.Point(126, 17);
            this.rbnLCWLB.Name = "rbnLCWLB";
            this.rbnLCWLB.Size = new System.Drawing.Size(62, 17);
            this.rbnLCWLB.TabIndex = 25;
            this.rbnLCWLB.Text = "LCWLB";
            this.rbnLCWLB.UseVisualStyleBackColor = true;
            // 
            // rbnLCWHB
            // 
            this.rbnLCWHB.AutoSize = true;
            this.rbnLCWHB.Location = new System.Drawing.Point(127, 39);
            this.rbnLCWHB.Name = "rbnLCWHB";
            this.rbnLCWHB.Size = new System.Drawing.Size(64, 17);
            this.rbnLCWHB.TabIndex = 26;
            this.rbnLCWHB.Text = "LCWHB";
            this.rbnLCWHB.UseVisualStyleBackColor = true;
            // 
            // SweepTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1150, 757);
            this.Controls.Add(this.gbMode);
            this.Controls.Add(this.gbDisplay);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.lblCopyRight);
            this.Controls.Add(this.cbxEnableSetting);
            this.Controls.Add(this.lblError);
            this.Controls.Add(this.dgvSweepResult);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnNext);
            this.Controls.Add(this.gpParameter);
            this.Controls.Add(this.btnTest);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "SweepTest";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SweepTest";
            this.Load += new System.EventHandler(this.SweepTest_Load);
            this.gpParameter.ResumeLayout(false);
            this.gpParameter.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSweepResult)).EndInit();
            this.gbDisplay.ResumeLayout(false);
            this.gbDisplay.PerformLayout();
            this.gbMode.ResumeLayout(false);
            this.gbMode.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnTest;
        private System.Windows.Forms.GroupBox gpParameter;
        private System.Windows.Forms.RadioButton rbnCWLB;
        private System.Windows.Forms.RadioButton rbnCWHB;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.DataGridView dgvSweepResult;
        private System.Windows.Forms.Label lblError;
        private System.Windows.Forms.RadioButton rbnWCDMA;
        private System.Windows.Forms.RadioButton rbnTDSCDMA;
        private System.Windows.Forms.RadioButton rbnEDGELB;
        private System.Windows.Forms.CheckBox cbxEnableSetting;
        private System.Windows.Forms.TextBox tbxVCC;
        private System.Windows.Forms.Label lblVCC;
        private System.Windows.Forms.TextBox tbxFreqList;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tbxStep;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tbxStop;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbxStart;
        private System.Windows.Forms.Label lblStep;
        private System.Windows.Forms.TextBox tbxTXEN;
        private System.Windows.Forms.Label lblTXEN;
        private System.Windows.Forms.TextBox tbxGPCTRL;
        private System.Windows.Forms.Label lblGPCTRL;
        private System.Windows.Forms.Button btnSettingSave;
        private System.Windows.Forms.RadioButton rbnEDGEHB;
        private System.Windows.Forms.Label lblCopyRight;
        private System.Windows.Forms.TextBox tbxPin_Vramp;
        private System.Windows.Forms.Label lblPin_Vramp;
        private System.Windows.Forms.CheckBox cbxKeepPrevious;
        private System.Windows.Forms.TextBox tbxVBAT;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.LinkLabel lblHelp;
        private System.Windows.Forms.GroupBox gbDisplay;
        private System.Windows.Forms.RadioButton rbnDisplayOFF;
        private System.Windows.Forms.RadioButton rbnDisplayON;
        private System.Windows.Forms.GroupBox gbMode;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.CheckBox cbxBypassEVM;
        private System.Windows.Forms.RadioButton rbnLTEFDDLB;
        private System.Windows.Forms.RadioButton rbnLTETDD_B38;
        private System.Windows.Forms.RadioButton rbnLTETDD_B40;
        private System.Windows.Forms.ComboBox cbxWaveform;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton rbnLTEFDDHB;
        private System.Windows.Forms.RadioButton rbnEVDo;
        private System.Windows.Forms.RadioButton rbnCDMA;
        private System.Windows.Forms.CheckBox cbxexthar;
        private System.Windows.Forms.CheckBox cbxMipi;
        private System.Windows.Forms.RadioButton rbnLCWHB;
        private System.Windows.Forms.RadioButton rbnLCWLB;
    }
}