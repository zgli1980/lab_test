/// Reversion history
/// 20150226        Fix mipi read error             Ace Li
/// 20150226_2      Add Visa control for mipi       Ace Li
/// 20151106        Can change clock dutycycle      Ace Li
/// 20151106        add loop, trigger function      Ace Li
/// 


using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Vanchip.Testing;
using Vanchip.Common;
//using NationalInstruments.VisaNS;

namespace Bench_Test
{
    public partial class Mipi : Form
    {
        bool simulated = false;
        bool bln_loop = false;

        #region ### Variable
        bool isgpib = true;
        double dblFreq = 1.0;
        double dblVolt = 1.8;
        int intPoint = 10;
        double dblShift = 0;
        double dblDuty = 0.5;

        bool is_Vadilate = false;

        int GPIB_Addr = Instruments_address._12;
        string VISA_str = Instruments_VISA.Arb_33522A;

        //MessageBasedSession AWG_33522A_USB;
        GPIB gpib = null;
        VISA visa = null;
        Util util = new Util();

        static int Max_SRate_MHz = 240;

        #endregion ### Variable

        public Mipi()
        {
            InitializeComponent();

            lbxMipi.SelectedIndexChanged += new EventHandler(lbxMipi_SelectedIndexChanged);
        }

        private void Mipi_Load(object sender, EventArgs e)
        {
            this.Text = "MIPI - 20151217";
            if (Program.debug_mipi)
            {
                cbx_P1.Enabled = true;
                cbx_P2.Enabled = true;
                btnSend.Enabled = false;
            }
            else
            {
                cbx_P1.Enabled = false;
                cbx_P2.Enabled = false;
            }
            ResetMipiPanel();

            if (!simulated)
            {
                try
                {
                    if (gpib == null || !gpib.isInit) gpib = new GPIB();
                    gpib.Send(GPIB_Addr, "*RST");
                }
                catch
                {
                    if (visa == null || !visa.isInit) visa = new VISA(VISA_str);
                    visa.Send("*RST");
                    isgpib = false;
                }
                finally
                {
                    SendCmd("*RST");
                    util.Wait(1000);
                }
            }


        }

        private void lbxMipi_SelectedIndexChanged(object sender, EventArgs e)
        {
            #region Reset
            foreach (Control control in this.Mipi_Panel.Controls)
            {
                if (control is TextBox)
                {
                    TextBox t = (TextBox)control;
                    t.Text = "";
                }
                else if (control is CheckBox)
                {
                    CheckBox c = (CheckBox)control;
                    c.Checked = false;
                }
            }

            cbxReg0.Enabled = false;
            cbxReg0.Checked = true;
            cbbMethod.SelectedIndex = 0;
            cbx_P1.Checked = false;
            cbx_P2.Checked = false;
            tbxShift.Text = "0";
            #endregion Reset

            string line = "";
            string[] line_content;
            string[] strTemp;
            if (lbxMipi.SelectedItem == null) return;
            StreamReader srMipi = new StreamReader(Program.strMipi_FilePath + lbxMipi.SelectedItem.ToString() + ".ini");

            tbxDuty.Text = "0.5";

            line = srMipi.ReadLine();
            do
            {
                line_content = line.Split(',');
                strTemp = line.Split(',');

                switch (line_content[0])
                {
                    case "Product":
                        tbxProduct.Text = line_content[1];
                        break;
                    case "Mode":
                        tbxMode.Text = line_content[1];
                        break;
                    case "USID":
                        tbxUSID.Text = line_content[1];
                        break;
                    case "Freq":
                        tbxFreq.Text = line_content[1];
                        break;
                    case "Volt":
                        tbxVolt.Text = line_content[1];
                        break;
                    case "Point":
                        tbxPoint.Text = line_content[1];
                        break;
                    case "Shift":
                        tbxShift.Text = line_content[1];
                        break;
                    case "DutyCycle":
                        tbxDuty.Text = line_content[1];
                        break;
                    case "reg0":
                        cbxReg0.Checked = true;
                        strTemp = line_content[1].Split('_');
                        tbxAddr_0.Text = strTemp[0];
                        tbxData_0.Text = strTemp[1];
                        break;
                    case "reg1":
                        cbxReg1.Checked = true;
                        strTemp = line_content[1].Split('_');
                        tbxAddr_1.Text = strTemp[0];
                        tbxData_1.Text = strTemp[1];
                        break;
                    case "reg2":
                        cbxReg2.Checked = true;
                        strTemp = line_content[1].Split('_');
                        tbxAddr_2.Text = strTemp[0];
                        tbxData_2.Text = strTemp[1];
                        break;
                    case "reg3":
                        cbxReg3.Checked = true;
                        strTemp = line_content[1].Split('_');
                        tbxAddr_3.Text = strTemp[0];
                        tbxData_3.Text = strTemp[1];
                        break;
                    case "reg4":
                        cbxReg4.Checked = true;
                        strTemp = line_content[1].Split('_');
                        tbxAddr_4.Text = strTemp[0];
                        tbxData_4.Text = strTemp[1];
                        break;
                    case "reg5":
                        cbxReg5.Checked = true;
                        strTemp = line_content[1].Split('_');
                        tbxAddr_5.Text = strTemp[0];
                        tbxData_5.Text = strTemp[1];
                        break;
                    case "Method":
                        strTemp = line_content[1].Split('_');
                        cbbMethod.SelectedIndex = Convert.ToInt16(strTemp[0]);
                        break;
                    case "mipi_loop":
                        strTemp = line_content[1].Split('_');
                        cbx_loop.Checked = true;
                        tbx_burst.Text = strTemp[0];
                        tbx_width.Text = strTemp[1];
                        break;
                }
                line = srMipi.ReadLine();           
            }while (line != null);

            srMipi.Close();
        }
        private void cbxP2_CheckedChanged(object sender, EventArgs e)
        {

            if (cbx_P2.Checked)
            {
                tbx_P2_0.Text = "0";
                tbx_P2_0.Enabled = true;
                tbx_P2_1.Text = "0";
                tbx_P2_1.Enabled = true;
                tbx_P2_2.Text = "0";
                tbx_P2_2.Enabled = true;
                tbx_P2_3.Text = "0";
                tbx_P2_3.Enabled = true;
                tbx_P2_4.Text = "0";
                tbx_P2_4.Enabled = true;
                tbx_P2_5.Text = "0";
                tbx_P2_5.Enabled = true;
            }
            else
            {
                tbx_P2_0.Text = "";
                tbx_P2_0.Enabled = false;
                tbx_P2_1.Text = "";
                tbx_P2_1.Enabled = false;
                tbx_P2_2.Text = "";
                tbx_P2_2.Enabled = false;
                tbx_P2_3.Text = "";
                tbx_P2_3.Enabled = false;
                tbx_P2_4.Text = "";
                tbx_P2_4.Enabled = false;
                tbx_P2_5.Text = "";
                tbx_P2_5.Enabled = false;
            }
        }
        private void cbx_P1_CheckedChanged(object sender, EventArgs e)
        {
            if (cbx_P1.Checked)
            {
                tbx_P1_0.Text = "0";
                tbx_P1_0.Enabled = true;
                tbx_P1_1.Text = "0";
                tbx_P1_1.Enabled = true;
                tbx_P1_2.Text = "0";
                tbx_P1_2.Enabled = true;
                tbx_P1_3.Text = "0";
                tbx_P1_3.Enabled = true;
                tbx_P1_4.Text = "0";
                tbx_P1_4.Enabled = true;
                tbx_P1_5.Text = "0";
                tbx_P1_5.Enabled = true;
            }
            else
            {
                tbx_P1_0.Text = "";
                tbx_P1_0.Enabled = false;
                tbx_P1_1.Text = "";
                tbx_P1_1.Enabled = false;
                tbx_P1_2.Text = "";
                tbx_P1_2.Enabled = false;
                tbx_P1_3.Text = "";
                tbx_P1_3.Enabled = false;
                tbx_P1_4.Text = "";
                tbx_P1_4.Enabled = false;
                tbx_P1_5.Text = "";
                tbx_P1_5.Enabled = false;
            }
        }
        
        private string Mipi_Parity(string strData)
        {
            string strParityResult = "";
            int intSum = 0;

            for (int i = 0; i < strData.Length; i++)
            {
                intSum += Convert.ToInt16(strData[i]);
            }
            int intTemp = (intSum + 1) % 2;
            strParityResult = intTemp.ToString();

            return strParityResult;

        }
        private string Dec2Binary(int intData)
        {
            string result = Convert.ToString(intData, 2);
            return result;
        }
        private string ParseBinaryFromText(string strData, int NumofDigits)
        {
            string result = "";
            string suffix = "";
            try
            {
                suffix = strData.Substring(0, 2);
            }
            catch(Exception ex)
            {
                string ace = ex.Message;
            }

            if (suffix.ToLower() == "0x")
            {
                string strTemp = "";
                int intTemp = 0;

                strTemp = strData.Substring(2);
                // Hex to Dec
                intTemp = Convert.ToInt32(strTemp, 16);
                // Dec to Binary
                result = Convert.ToString(intTemp, 2);
            }
            else if (suffix.ToLower() == "0b")
            { 
                result = strData.Substring(2);
            }
            else
            {
                result = strData;
            }

            result = CheckNumDigits(result, true, NumofDigits);
            return result;
        }
        private string CheckNumDigits(string strData ,bool issuffix ,int intExpectNumDigits)
        {
            //补足位,前面补0
            if (issuffix)
            {
                while (strData.Length < intExpectNumDigits)
                {
                    strData = "0" + strData;
                }
            }
            else //补足位,后面补0
            {
                while (strData.Length < intExpectNumDigits)
                {
                    strData = strData + "0";
                }
            }

            return strData;
        }

        private void ResetMipiPanel()
        {
            foreach (Control control in this.Mipi_Panel.Controls)
            {
                if (control is TextBox)
                {
                    TextBox t = (TextBox)control;
                    t.Text = "";
                }
                else if (control is CheckBox)
                {
                    CheckBox c = (CheckBox)control;
                    c.Checked = false;
                }
            }

            cbxReg0.Enabled = false;
            cbxReg0.Checked = true;
            cbbMethod.SelectedIndex = 0;
            cbx_P1.Checked = false;
            cbx_P2.Checked = false;

            tbxFreq.Text = "1";
            tbxVolt.Text = "1.8";
            tbxPoint.Text = "10";
            tbxShift.Text = "0";
            tbxDuty.Text = "0.5";
            tbxProduct.Text = "VC7778";
            tbxMode.Text = "OFF";
            tbxUSID.Text = "1111";
            tbxAddr_0.Text = "0x0";
            tbxData_0.Text = "0x00";

            FillListBox();
        }
        private void FillListBox()
        {
            lbxMipi.Items.Clear();
            string[] strFiles = Directory.GetFiles(Program.strMipi_FilePath,"*.ini");

            for (int i = 0; i < strFiles.Length; i++)
            {
                FileInfo fi = new FileInfo(strFiles[i]);
                if (fi.Name.Substring(1, 1) != ".")
                    lbxMipi.Items.Add(fi.Name.Substring(0, fi.Name.Length - 4));
            }

        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string filename = Program.strMipi_FilePath + lbxMipi.SelectedItem.ToString() + ".ini";

            FileInfo fi = new FileInfo(filename);
            fi.Delete();

            lbxMipi.SelectedIndex = 0;
            lbxMipi.Items.Remove(lbxMipi.SelectedItem);

            ResetMipiPanel();
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            string FileName = Program.strMipi_FilePath + tbxProduct.Text + "_" + tbxMode.Text + ".ini";

            // 判断配置文件是否存在
            if (File.Exists(FileName))
            {
                if (MessageBox.Show(@"The same configuration file is exist, are you sure to overwrite the current file?

    Yes -- Overwrite 
    No  -- Return(nothing saved)", "Overwrite file?", MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    return;
                }

            }

            #region --- Save file ---
            StreamWriter swmipi = new StreamWriter(FileName);
            StringBuilder sbmipi = new StringBuilder();

            sbmipi.AppendLine("for Mipi(RFFE) control, please make the change if you are sure what are you doing here.......Ace");
            sbmipi.AppendLine("--- Start ---");
            

            // Product Info
            sbmipi.AppendLine("Product," + tbxProduct.Text);
            sbmipi.AppendLine("Mode," + tbxMode.Text);
            sbmipi.AppendLine("USID," + tbxUSID.Text);

            // Common Setting
            sbmipi.AppendLine("Freq," + tbxFreq.Text);
            sbmipi.AppendLine("Volt," + tbxVolt.Text);
            sbmipi.AppendLine("Point," + tbxPoint.Text);
            sbmipi.AppendLine("Shift," + tbxShift.Text);
            sbmipi.AppendLine("DutyCycle," + tbxDuty.Text);

            // Register Info
            sbmipi.AppendLine("reg0," + tbxAddr_0.Text + "_" + tbxData_0.Text);
            if (cbxReg1.Checked) sbmipi.AppendLine("reg1," + tbxAddr_1.Text + "_" + tbxData_1.Text);
            if (cbxReg2.Checked) sbmipi.AppendLine("reg2," + tbxAddr_2.Text + "_" + tbxData_2.Text);
            if (cbxReg3.Checked) sbmipi.AppendLine("reg3," + tbxAddr_3.Text + "_" + tbxData_3.Text);
            if (cbxReg4.Checked) sbmipi.AppendLine("reg4," + tbxAddr_4.Text + "_" + tbxData_4.Text);
            if (cbxReg5.Checked) sbmipi.AppendLine("reg5," + tbxAddr_5.Text + "_" + tbxData_5.Text);
            sbmipi.AppendLine("Method," + cbbMethod.SelectedIndex.ToString() + "_" + cbbMethod.SelectedItem.ToString());

            // Other
            if (bln_loop) sbmipi.AppendLine("mipi_loop," + tbx_burst.Text + "_" + tbx_width.Text);



            sbmipi.AppendLine("--- The End ---");

            swmipi.Write(sbmipi.ToString());
            swmipi.Close();

            lbxMipi.Items.Add(tbxProduct.Text + "_" + tbxMode.Text);
            MessageBox.Show("Configuration saved!");
            #endregion --- Save file ---

            ResetMipiPanel();

        }
        private void btnReset_Click(object sender, EventArgs e)
        {
            ResetMipiPanel();
        }
        private void btnTest_Click(object sender, EventArgs e)
        {
            btnTest.Enabled = false;
            btnTest.Text = "Sending mipi";
            btnTest.Refresh();

            Build_Send_Mipi();

            btnTest.Enabled = true;
            btnTest.Text = "Test Mipi";
        }
        private void btnDebug_Click(object sender, EventArgs e)
        {
            btnDebug.Enabled = false;

            string cmdString = tbxDebug.Text;
            string strCmd = cmdString.Replace(" ", "");
            RegisterWrite(GPIB_Addr, "Debug", strCmd);

            btnDebug.Enabled = true;
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            btnSend.Enabled = false;

            Build_Send_Mipi();

            Datalog frmDatalog = new Datalog();
            
            //frmDatalog.ShowDialog();
            btnSend.Enabled = true;
            this.Hide();
            //util.Wait(2000);
            //this.ShowDialog();
        }
        private void Build_Send_Mipi()
        {
            #region Variable Define

            string SSC = "10";
            string USID = "";
            string CMD = "010";
            string Addr = "";
            string P1 = "";
            string Data = "";
            string P2 = "";
            string cmdString = "";

            string extCMD = "0000";
            string extAddr = "";
            string extP = "";
            string extBC = "";
            StringBuilder extcmd = new StringBuilder();
            StringBuilder extdata = new StringBuilder();

            string Product = "";
            string Mode = "";

            int intTotalBytes = -1; // 0;

            bool extWrite = false;

            TestSetting.MIPI_BURST = int.Parse(tbx_burst.Text);
            TestSetting.MIPI_WIDTH = int.Parse(tbx_width.Text);

            #endregion Variable Define

            if (!is_Vadilate) Textbox_Vadilate();

            dblFreq = Convert.ToDouble(tbxFreq.Text);
            dblVolt = Convert.ToDouble(tbxVolt.Text);
            intPoint = Convert.ToInt16(tbxPoint.Text);
            dblShift = Convert.ToDouble(tbxShift.Text);
            dblDuty = Convert.ToDouble(tbxDuty.Text);
            USID = tbxUSID.Text;

            // Enable Extended Register Write
            if (cbbMethod.SelectedIndex == 1) extWrite = true;

            #region Register 1
            if (cbxReg1.Checked)
            {
                Addr = ParseBinaryFromText(tbxAddr_1.Text, 5);
                Data = ParseBinaryFromText(tbxData_1.Text, 8);

                tbx_P1_0.Text = P1 = Mipi_Parity(USID + CMD + Addr);
                tbx_P2_1.Text = P2 = Mipi_Parity(Data);

                cmdString = SSC + USID + CMD + Addr + P1 + Data + P2;

                //Extended Register Write
                intTotalBytes += 1;
                extdata.Append(Data);
                extdata.Append(P2);

                if (!extWrite)
                    RegisterWrite(GPIB_Addr, "reg1", cmdString);

            }
            #endregion Register 1

            #region Register 2
            if (cbxReg2.Checked)
            {
                Addr = ParseBinaryFromText(tbxAddr_2.Text, 5);
                Data = ParseBinaryFromText(tbxData_2.Text, 8);

                tbx_P1_0.Text = P1 = Mipi_Parity(USID + CMD + Addr);
                tbx_P2_2.Text = P2 = Mipi_Parity(Data);

                cmdString = SSC + USID + CMD + Addr + P1 + Data + P2;

                //Extended Register Write
                intTotalBytes += 1;
                extdata.Append(Data);
                extdata.Append(P2);

                if (!extWrite)
                    RegisterWrite(GPIB_Addr, "reg2", cmdString);

            }
            #endregion Register 1

            #region Register 3
            if (cbxReg3.Checked)
            {
                Addr = ParseBinaryFromText(tbxAddr_3.Text, 5);
                Data = ParseBinaryFromText(tbxData_3.Text, 8);

                tbx_P1_0.Text = P1 = Mipi_Parity(USID + CMD + Addr);
                tbx_P2_3.Text = P2 = Mipi_Parity(Data);

                cmdString = SSC + USID + CMD + Addr + P1 + Data + P2;

                //Extended Register Write
                intTotalBytes += 1;
                extdata.Append(Data);
                extdata.Append(P2);

                if (!extWrite)
                    RegisterWrite(GPIB_Addr, "reg3", cmdString);

            }
            #endregion Register 3

            #region Register 4
            if (cbxReg4.Checked)
            {
                Addr = ParseBinaryFromText(tbxAddr_4.Text, 5);
                Data = ParseBinaryFromText(tbxData_4.Text, 8);

                tbx_P1_0.Text = P1 = Mipi_Parity(USID + CMD + Addr);
                tbx_P2_4.Text = P2 = Mipi_Parity(Data);

                cmdString = SSC + USID + CMD + Addr + P1 + Data + P2;

                //Extended Register Write
                intTotalBytes += 1;
                extdata.Append(Data);
                extdata.Append(P2);

                if (!extWrite)
                    RegisterWrite(GPIB_Addr, "reg4", cmdString);

            }
            #endregion Register 4

            #region Register 5
            if (cbxReg5.Checked)
            {
                Addr = ParseBinaryFromText(tbxAddr_5.Text, 5);
                Data = ParseBinaryFromText(tbxData_5.Text, 8);

                tbx_P1_0.Text = P1 = Mipi_Parity(USID + CMD + Addr);
                tbx_P2_5.Text = P2 = Mipi_Parity(Data);

                cmdString = SSC + USID + CMD + Addr + P1 + Data + P2;

                //Extended Register Write
                intTotalBytes += 1;
                extdata.Append(Data);
                extdata.Append(P2);

                if (!extWrite)
                    RegisterWrite(GPIB_Addr, "reg5", cmdString);

            }
            #endregion Register 5

            #region Register 0 ---### Must be the last one ###---
            if (cbxReg0.Checked)
            {
                Addr = ParseBinaryFromText(tbxAddr_0.Text, 5);
                Data = ParseBinaryFromText(tbxData_0.Text, 8);

                tbx_P1_0.Text = P1 = Mipi_Parity(USID + CMD + Addr);
                tbx_P2_0.Text = P2 = Mipi_Parity(Data);

                cmdString = SSC + USID + CMD + Addr + P1 + Data + P2;
                tbxDebug.Text = cmdString;

                //Extended Register Write
                intTotalBytes += 1;
                extAddr = CheckNumDigits(Addr, true, 8);
                extdata.Append(extAddr);
                extP = Mipi_Parity(extAddr);
                extdata.Append(extP);
                extdata.Append(Data);
                extdata.Append(P2);

                if (bln_loop)
                {
                    int burst_width = int.Parse(tbx_burst.Text);
                    cmdString = cmdString + "#";

                    for (int i = 1; i <= burst_width; i++)
                    {
                        cmdString = cmdString + "0";
                    }
                    cmdString = cmdString + "#";
                    cmdString = cmdString + SSC + USID + CMD + Addr + P1 + "000000001"; // +P2; // "00000000" turn off the PA enable

                    RegisterWrite(GPIB_Addr, "reg0", cmdString, Triger_Source.Ext);
                }
                else if (!extWrite)
                    RegisterWrite(GPIB_Addr, "reg0", cmdString);

            }
            #endregion Register 0

            #region Extended Write
            extBC = Dec2Binary(intTotalBytes);
            extBC = CheckNumDigits(extBC, true, 8);

            extcmd.Append(SSC);
            extcmd.Append(USID);
            extcmd.Append(extBC);

            extP = Mipi_Parity(USID + extBC);
            extcmd.Append(extP);
            extcmd.Append(extdata.ToString());

            if (extWrite)
            {
                tbxDebug.Text = extcmd.ToString();
                RegisterWrite(GPIB_Addr, "Extended", extcmd.ToString());
            }
            #endregion Extended Write
        }

        private void btnRead_Click(object sender, EventArgs e)
        {
            Build_Read_Mipi();
        }
        private void Build_Read_Mipi()
        {
            #region Variable Define

            string SSC = "10";
            string USID = "";
            string CMD = "011";
            string Addr = "";
            string P1 = "";
            string Data = "";
            string P2 = "";
            string cmdString = "";

            string extCMD = "0000";
            string extAddr = "";
            string extP = "";
            string extBC = "";
            StringBuilder extcmd = new StringBuilder();
            StringBuilder extdata = new StringBuilder();

            string Product = "";
            string Mode = "";

            #endregion Variable Define

            dblFreq = Convert.ToDouble(tbxFreq.Text);
            dblVolt = Convert.ToDouble(tbxVolt.Text);
            intPoint = Convert.ToInt16(tbxPoint.Text);
            USID = tbxUSID.Text;


            #region Register 0
            if (cbxReg0.Checked)
            {
                Addr = ParseBinaryFromText(tbxAddr_0.Text, 5);
                Data = ParseBinaryFromText(tbxData_0.Text, 8);

                P1 = Mipi_Parity(CMD + Addr);
                P2 = Mipi_Parity(Data);

                cmdString = SSC + USID + CMD + Addr + P1;
                RegisterWrite(GPIB_Addr, "reg0", cmdString, true);

            }
            #endregion Register 0

        }

        private void RegisterWrite(int GPIB_Addr, string arb_name, string strCmd)
        {
            //int shift_point = (int)Math.Ceiling((double)intPoint / 5) + intShift;

            double SRate = dblFreq * intPoint * 2E6;
            int shift_point = (int)Math.Ceiling((double)(dblShift*10));
            int total_point = (strCmd.Length + 4) * intPoint;
            int intHigh = Convert.ToInt32(intPoint * 2 * dblDuty);
            int intLow = intPoint * 2 - intHigh;

            #region Build Mipi Array
            StringBuilder sbClock = new StringBuilder();
            StringBuilder sbData = new StringBuilder();

            #region //// Prefix 2*point  一个周期
            for (int i = 1; i <= intPoint * 2; i++)
            {
                if (shift_point > 0)
                {
                    if (i > shift_point)
                    {
                        sbClock.Append(",");
                        sbClock.Append(0);
                    }

                    sbData.Append(",");
                    sbData.Append(0);
                }
                else if (shift_point < 0)
                {
                    if (i > Math.Abs( shift_point))
                    {
                        sbData.Append(",");
                        sbData.Append(0);
                    }

                    sbClock.Append(",");
                    sbClock.Append(0);
                }

            }
            #endregion 

            #region //// Data
            for (int j = 0; j < strCmd.Length; j++)
            {
                for (int i = 1; i <= intPoint * 2; i++)
                {
                    // SSC no clock
                    if (j <= 1)
                    {
                        sbClock.Append(",");
                        sbClock.Append(0);
                    }
                    else if (i <= intHigh)
                    {
                        sbClock.Append(",");
                        sbClock.Append(1);
                    }
                    else if (i > intHigh)
                    {
                        sbClock.Append(",");
                        sbClock.Append(0);
                    }

                    sbData.Append(",");
                    sbData.Append(strCmd[j]);
                }
            }
            #endregion

            #region //// BP signal
            for (int i = 1; i <= intPoint * 2; i++)
            {
                if (i <= intHigh)
                {
                    sbClock.Append(",");
                    sbClock.Append(1);
                }
                else if (i > intHigh)
                {
                    sbClock.Append(",");
                    sbClock.Append(0);
                }
                sbData.Append(",");
                sbData.Append(0);
            }
            #endregion

            #region //// Suffix 2*point
            for (int i = 1; i <= intPoint * 2 + shift_point; i++)
            {
                sbClock.Append(",");
                sbClock.Append(0);
                if (i <= intPoint * 2)
                {
                    sbData.Append(",");
                    sbData.Append(0);
                }
            }
            #endregion

            #endregion Build Mipi Array

            if (!simulated)
            {
                #region Send to Arb
                SendCmd("*CLS");

                SendCmd("SOUR1:FUNC ARB");
                SendCmd("SOUR1:DATA:VOL:CLE");
                SendCmd("SOUR1:DATA:ARB SCLK" + sbClock.ToString());
                SendCmd("SOUR1:FUNC:ARB SCLK");

                SendCmd("SOUR1:FUNC:ARB:SRATE " + SRate.ToString());
                //SendCmd("SOUR1:FUNC:ARB:FILTER OFF");
                SendCmd("SOUR1:FUNC:ARB:FILTER STEP");
                SendCmd("OUTP1:LOAD INF");
                SendCmd("SOUR1:VOLT " + dblVolt.ToString());
                SendCmd("SOUR1:VOLT:OFFS 0");

                SendCmd("SOUR1:BURS:STAT ON");
                SendCmd("SOUR1:BURS:MODE TRIG");
                SendCmd("SOUR1:BURS:NCYC 1");
                SendCmd("TRIG1:SOUR BUS");

                SendCmd("SOUR2:FUNC ARB");
                SendCmd("SOUR2:DATA:VOL:CLE");
                SendCmd("SOUR2:DATA:ARB " + arb_name + sbData.ToString());
                SendCmd("SOUR2:FUNC:ARB " + arb_name);

                SendCmd("SOUR2:FUNC:ARB:SRATE " + SRate.ToString());
                //SendCmd("SOUR2:FUNC:ARB:FILTER OFF");
                SendCmd("SOUR2:FUNC:ARB:FILTER STEP");
                SendCmd("OUTP2:LOAD INF");
                SendCmd("SOUR2:VOLT " + dblVolt.ToString());
                SendCmd("SOUR2:VOLT:OFFS 0");

                SendCmd("SOUR2:BURS:STAT ON");
                SendCmd("SOUR2:BURS:MODE TRIG");
                SendCmd("SOUR2:BURS:NCYC 1");
                SendCmd("TRIG2:SOUR BUS");

                SendCmd("OUTP1 ON");
                SendCmd("OUTP2 ON");

                util.Wait(1000);
                SendCmd("*TRG");
                #endregion Send to Arb
            }
            //SendCmd("*RST");

        }
        private void RegisterWrite(int GPIB_Addr, string arb_name, string strCmd, bool Extra_Clock)
        {
            int shift_point = (int)Math.Ceiling((double)intPoint / 5);
            int total_point = (strCmd.Length + 4) * intPoint;
            double SRate = dblFreq * intPoint * 2E6;


            #region Build Mipi Array
            StringBuilder sbClock = new StringBuilder();
            StringBuilder sbData = new StringBuilder();

            //// Prefix 2*point
            for (int i = 1; i <= intPoint * 2; i++)
            {
                if (i > shift_point)
                {
                    sbClock.Append(",");
                    sbClock.Append(0);
                }

                sbData.Append(",");
                sbData.Append(0);
            }
            //// Data
            for (int j = 0; j < strCmd.Length; j++)
            {
                for (int i = 1; i <= intPoint * 2; i++)
                {
                    // SSC no clock
                    if (j <= 1)
                    {
                        sbClock.Append(",");
                        sbClock.Append(0);
                    }
                    else if (i <= intPoint)
                    {
                        sbClock.Append(",");
                        sbClock.Append(1);
                    }
                    else if (i > intPoint)
                    {
                        sbClock.Append(",");
                        sbClock.Append(0);
                    }

                    sbData.Append(",");
                    sbData.Append(strCmd[j]);
                }
            }
            //// BP signal 
            for (int i = 1; i <= intPoint * 2; i++)
            {
                if (i <= intPoint)
                {
                    sbClock.Append(",");
                    sbClock.Append(1);
                }
                else if (i > intPoint)
                {
                    sbClock.Append(",");
                    sbClock.Append(0);
                }
                sbData.Append(",");
                sbData.Append(0);
            }
            //// Extra Clock
            if (Extra_Clock)
            {
                for (int a = 0; a < 20; a++)
                {
                    for (int i = 1; i <= intPoint * 2; i++)
                    {
                        if (i <= intPoint)
                        {
                            sbClock.Append(",");
                            sbClock.Append(1);
                        }
                        else if (i > intPoint)
                        {
                            sbClock.Append(",");
                            sbClock.Append(0);
                        }
                        sbData.Append(",");
                        sbData.Append(0);
                    }
                }
            }
            //// Suffix 2*point
            for (int i = 1; i <= intPoint * 2 + shift_point; i++)
            {
                sbClock.Append(",");
                sbClock.Append(0);
                if (i <= intPoint * 2)
                {
                    sbData.Append(",");
                    sbData.Append(0);
                }
            }

            #endregion Build Mipi Array

            if (!simulated)
            {
                #region Send to Arb
                SendCmd("*CLS");

                SendCmd("SOUR1:FUNC ARB");
                SendCmd("SOUR1:DATA:VOL:CLE");
                SendCmd("SOUR1:DATA:ARB SCLK" + sbClock.ToString());
                SendCmd("SOUR1:FUNC:ARB SCLK");

                SendCmd("SOUR1:FUNC:ARB:SRATE " + SRate.ToString());
                //SendCmd("SOUR1:FUNC:ARB:FILTER OFF");
                SendCmd("OUTP1:LOAD INF");
                SendCmd("SOUR1:VOLT " + dblVolt.ToString());
                SendCmd("SOUR1:VOLT:OFFS 0");

                SendCmd("SOUR1:BURS:STAT ON");
                SendCmd("SOUR1:BURS:MODE TRIG");
                SendCmd("SOUR1:BURS:NCYC 1");
                SendCmd("TRIG1:SOUR BUS");

                SendCmd("SOUR2:FUNC ARB");
                SendCmd("SOUR2:DATA:VOL:CLE");
                SendCmd("SOUR2:DATA:ARB " + arb_name + sbData.ToString());
                SendCmd("SOUR2:FUNC:ARB " + arb_name);

                SendCmd("SOUR2:FUNC:ARB:SRATE " + SRate.ToString());
                //SendCmd("SOUR2:FUNC:ARB:FILTER OFF");
                SendCmd("OUTP2:LOAD INF");
                SendCmd("SOUR2:VOLT " + dblVolt.ToString());
                SendCmd("SOUR2:VOLT:OFFS 0");

                SendCmd("SOUR2:BURS:STAT ON");
                SendCmd("SOUR2:BURS:MODE TRIG");
                SendCmd("SOUR2:BURS:NCYC 1");
                SendCmd("TRIG2:SOUR BUS");

                SendCmd("OUTP1 ON");
                SendCmd("OUTP2 ON");

                util.Wait(1000);
                MessageBox.Show("Set oscilloscope to trigger mode");
                SendCmd("*TRG");
                #endregion Send to Arb
            }
            //SendCmd("*RST");

        }
        private void RegisterWrite(int GPIB_Addr, string arb_name, string strCmd, Triger_Source trigger)
        {
            //int shift_point = (int)Math.Ceiling((double)intPoint / 5) + intShift;

            double SRate = dblFreq * intPoint * 2E6;
            int shift_point = (int)Math.Ceiling((double)(dblShift * 10));
            int total_point = (strCmd.Length + 4) * intPoint;
            int intHigh = Convert.ToInt32(intPoint * 2 * dblDuty);
            int intLow = intPoint * 2 - intHigh;

            #region Build Mipi Array
            StringBuilder sbClock = new StringBuilder();
            StringBuilder sbData = new StringBuilder();

            if (bln_loop)
            {
                string[] strCmdLst = strCmd.Split('#');

                #region //// First cmd
                #region //// Prefix 2*point  一个周期
                for (int i = 1; i <= intPoint * 2; i++)
                {
                    if (shift_point > 0)
                    {
                        if (i > shift_point)
                        {
                            sbClock.Append(",");
                            sbClock.Append(0);
                        }

                        sbData.Append(",");
                        sbData.Append(0);
                    }
                    else if (shift_point < 0)
                    {
                        if (i > Math.Abs(shift_point))
                        {
                            sbData.Append(",");
                            sbData.Append(0);
                        }

                        sbClock.Append(",");
                        sbClock.Append(0);
                    }

                }
                #endregion

                #region //// Data
                for (int j = 0; j < strCmdLst[0].Length; j++)
                {
                    for (int i = 1; i <= intPoint * 2; i++)
                    {
                        // SSC no clock
                        if (j <= 1)
                        {
                            sbClock.Append(",");
                            sbClock.Append(0);
                        }
                        else if (i <= intHigh)
                        {
                            sbClock.Append(",");
                            sbClock.Append(1);
                        }
                        else if (i > intHigh)
                        {
                            sbClock.Append(",");
                            sbClock.Append(0);
                        }

                        sbData.Append(",");
                        sbData.Append(strCmdLst[0][j]);
                    }
                }
                #endregion

                #region //// BP signal
                for (int i = 1; i <= intPoint * 2; i++)
                {
                    if (i <= intHigh)
                    {
                        sbClock.Append(",");
                        sbClock.Append(1);
                    }
                    else if (i > intHigh)
                    {
                        sbClock.Append(",");
                        sbClock.Append(0);
                    }
                    sbData.Append(",");
                    sbData.Append(0);
                }
                #endregion

                #region //// Suffix 2*point
                for (int i = 1; i <= intPoint * 2 + shift_point; i++)
                {
                    sbClock.Append(",");
                    sbClock.Append(0);
                    if (i <= intPoint * 2)
                    {
                        sbData.Append(",");
                        sbData.Append(0);
                    }
                }
                #endregion

                #endregion

                #region //// Burst width
                for (int j = 0; j < strCmdLst[1].Length; j++)
                {
                    for (int i = 1; i <= intPoint * 2; i++)
                    {
                        sbClock.Append(",");
                        sbClock.Append(0);
                        sbData.Append(",");
                        sbData.Append(strCmdLst[1][j]);
                    }
                }
                #endregion

                #region //// 2nd cmd
                #region //// Prefix 2*point  一个周期
                for (int i = 1; i <= intPoint * 2; i++)
                {
                    if (shift_point > 0)
                    {
                        if (i > shift_point)
                        {
                            sbClock.Append(",");
                            sbClock.Append(0);
                        }

                        sbData.Append(",");
                        sbData.Append(0);
                    }
                    else if (shift_point < 0)
                    {
                        if (i > Math.Abs(shift_point))
                        {
                            sbData.Append(",");
                            sbData.Append(0);
                        }

                        sbClock.Append(",");
                        sbClock.Append(0);
                    }

                }
                #endregion

                #region //// Data
                for (int j = 0; j < strCmdLst[2].Length; j++)
                {
                    for (int i = 1; i <= intPoint * 2; i++)
                    {
                        // SSC no clock
                        if (j <= 1)
                        {
                            sbClock.Append(",");
                            sbClock.Append(0);
                        }
                        else if (i <= intHigh)
                        {
                            sbClock.Append(",");
                            sbClock.Append(1);
                        }
                        else if (i > intHigh)
                        {
                            sbClock.Append(",");
                            sbClock.Append(0);
                        }

                        sbData.Append(",");
                        sbData.Append(strCmdLst[2][j]);
                    }
                }
                #endregion

                #region //// BP signal
                for (int i = 1; i <= intPoint * 2; i++)
                {
                    if (i <= intHigh)
                    {
                        sbClock.Append(",");
                        sbClock.Append(1);
                    }
                    else if (i > intHigh)
                    {
                        sbClock.Append(",");
                        sbClock.Append(0);
                    }
                    sbData.Append(",");
                    sbData.Append(0);
                }
                #endregion

                #region //// Suffix 2*point
                for (int i = 1; i <= intPoint * 2 + shift_point; i++)
                {
                    sbClock.Append(",");
                    sbClock.Append(0);
                    if (i <= intPoint * 2)
                    {
                        sbData.Append(",");
                        sbData.Append(0);
                    }
                }
                #endregion

                #endregion

            }

            else
            {
                #region //// Prefix 2*point  一个周期
                for (int i = 1; i <= intPoint * 2; i++)
                {
                    if (shift_point > 0)
                    {
                        if (i > shift_point)
                        {
                            sbClock.Append(",");
                            sbClock.Append(0);
                        }

                        sbData.Append(",");
                        sbData.Append(0);
                    }
                    else if (shift_point < 0)
                    {
                        if (i > Math.Abs(shift_point))
                        {
                            sbData.Append(",");
                            sbData.Append(0);
                        }

                        sbClock.Append(",");
                        sbClock.Append(0);
                    }

                }
                #endregion

                #region //// Data
                for (int j = 0; j < strCmd.Length; j++)
                {
                    for (int i = 1; i <= intPoint * 2; i++)
                    {
                        // SSC no clock
                        if (j <= 1)
                        {
                            sbClock.Append(",");
                            sbClock.Append(0);
                        }
                        else if (i <= intHigh)
                        {
                            sbClock.Append(",");
                            sbClock.Append(1);
                        }
                        else if (i > intHigh)
                        {
                            sbClock.Append(",");
                            sbClock.Append(0);
                        }

                        sbData.Append(",");
                        sbData.Append(strCmd[j]);
                    }
                }
                #endregion

                #region //// BP signal
                for (int i = 1; i <= intPoint * 2; i++)
                {
                    if (i <= intHigh)
                    {
                        sbClock.Append(",");
                        sbClock.Append(1);
                    }
                    else if (i > intHigh)
                    {
                        sbClock.Append(",");
                        sbClock.Append(0);
                    }
                    sbData.Append(",");
                    sbData.Append(0);
                }
                #endregion

                #region //// Suffix 2*point
                for (int i = 1; i <= intPoint * 2 + shift_point; i++)
                {
                    sbClock.Append(",");
                    sbClock.Append(0);
                    if (i <= intPoint * 2)
                    {
                        sbData.Append(",");
                        sbData.Append(0);
                    }
                }
                #endregion }
            }
            #endregion Build Mipi Array

            if (!simulated)
            {
                #region Send to Arb
                SendCmd("*CLS");

                SendCmd("SOUR1:FUNC ARB");
                SendCmd("SOUR1:DATA:VOL:CLE");
                SendCmd("SOUR1:DATA:ARB SCLK" + sbClock.ToString());
                SendCmd("SOUR1:FUNC:ARB SCLK");

                SendCmd("SOUR1:FUNC:ARB:SRATE " + SRate.ToString());
                //SendCmd("SOUR1:FUNC:ARB:FILTER OFF");
                SendCmd("SOUR1:FUNC:ARB:FILTER STEP");
                SendCmd("OUTP1:LOAD INF");
                SendCmd("SOUR1:VOLT " + dblVolt.ToString());
                SendCmd("SOUR1:VOLT:OFFS 0");

                SendCmd("SOUR1:BURS:STAT ON");
                SendCmd("SOUR1:BURS:MODE TRIG");
                SendCmd("SOUR1:BURS:NCYC 1");
                SendCmd("TRIG1:SOUR BUS");

                SendCmd("SOUR2:FUNC ARB");
                SendCmd("SOUR2:DATA:VOL:CLE");
                SendCmd("SOUR2:DATA:ARB " + arb_name + sbData.ToString());
                SendCmd("SOUR2:FUNC:ARB " + arb_name);

                SendCmd("SOUR2:FUNC:ARB:SRATE " + SRate.ToString());
                //SendCmd("SOUR2:FUNC:ARB:FILTER OFF");
                SendCmd("SOUR2:FUNC:ARB:FILTER STEP");
                SendCmd("OUTP2:LOAD INF");
                SendCmd("SOUR2:VOLT " + dblVolt.ToString());
                SendCmd("SOUR2:VOLT:OFFS 0");

                SendCmd("SOUR2:BURS:STAT ON");
                SendCmd("SOUR2:BURS:MODE TRIG");
                SendCmd("SOUR2:BURS:NCYC 1");
                SendCmd("TRIG2:SOUR BUS");

                SendCmd("OUTP1 ON");
                SendCmd("OUTP2 ON");

                // set ext trigger
                if (trigger == Triger_Source.Ext)
                {
                    SendCmd("TRIG1:SOUR EXT");
                    SendCmd("TRIG2:SOUR EXT");
                }
                else
                {
                    util.Wait(1000);
                    SendCmd("*TRG");
                }
                #endregion Send to Arb
            }
            //SendCmd("*RST");

        }

        private void SendCmd(string strCmd)
        {
            if (simulated) return;

            if (isgpib)
                gpib.Send(GPIB_Addr, strCmd);
            else
                visa.Send(strCmd);
        }

        private void Textbox_Vadilate()
        {
            Freq_verify();
            Point_verify();

            is_Vadilate = true;

        }
        private void Point_verify()
        {
            try
            {
                dblFreq = Convert.ToDouble(tbxFreq.Text);

                int intp = Convert.ToInt32(tbxPoint.Text);
                int intmaxp = Convert.ToInt32(Max_SRate_MHz / dblFreq / 2);

                if (intp < 10)
                {
                    MessageBox.Show("Point less than 10 may cause the mipi does not work ");
                    //tbxPoint.Text = "10";
                }
                else if (intp > intmaxp)
                {
                    MessageBox.Show("Point can not exceed " + intmaxp.ToString() + ", if Freq is set to " + dblFreq.ToString());
                    intp = Convert.ToInt32(Max_SRate_MHz / dblFreq / 2);
                    tbxPoint.Text = intp.ToString();
                }
            }
            catch
            {
                tbxPoint.Text = "10";
                tbxFreq.Text = "1";
            }

        }
        private void Freq_verify()
        {
            try
            {
                intPoint = Convert.ToInt16(tbxPoint.Text);

                double dblf = Convert.ToDouble(tbxFreq.Text);
                double dblMaxf = Max_SRate_MHz / 10 / 2;

                if (dblf < 0.1)
                {
                    MessageBox.Show("Freq can not less than 0.1 MHz ");
                    tbxFreq.Text = "0.1";
                }
                else if (dblf > dblMaxf)
                {
                    MessageBox.Show("Freq exceed " + dblMaxf.ToString() + "MHz may cause mipi does not work");
                    //tbxFreq.Text = dblMaxf.ToString();
                }

            }
            catch
            {
                tbxPoint.Text = "10";
                tbxFreq.Text = "1";
            }
        }

        private void btnCheck_Click(object sender, EventArgs e)
        {
            if (!is_Vadilate) Textbox_Vadilate();
        }
        private void TextBox_TextChanged(object sender, EventArgs e)
        {
            is_Vadilate = false;
        }

        private void cbxTrgigger_CheckedChanged(object sender, EventArgs e)
        {
            if (cbx_loop.Checked)
            {
                bln_loop = true;
                tbx_burst.Enabled = true;
                tbx_width.Enabled = true;
                tbx_burst.Text = "1154";
                tbx_width.Text = "4616";
                cbbMethod.SelectedIndex = 0;
                cbbMethod.Enabled = false;


            }
            else
            {
                bln_loop = false;
                cbbMethod.Enabled = true;
                tbx_burst.Enabled = false;
                tbx_width.Enabled = false;
            }
        }



    }
}
