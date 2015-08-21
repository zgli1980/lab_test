/// Reversion history
/// 20150306        Change according to the BJ new bench setup              Ace Li
/// 

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Vanchip.Testing;
using Vanchip.Common;

namespace Bench_Test
{
    public partial class LossComp : Form
    {
        #region Variable define

        private static int intDelay_SigGen = TestSetting.DELAY_SIGGEN_in_ms;
        private static int intDelay_PowerMeter = TestSetting.DELAY_POWER_METER_in_ms;
        private static int intDelay_MXA = TestSetting.DELAY_VSA_in_ms;

        private bool blnFinish = false;
        private int intStep = 0;


        Dictionary<double, double> SRC_3G = new Dictionary<double, double>();
        Dictionary<double, double> SRC_4G = new Dictionary<double, double>();
        Dictionary<double, double> MSR_THROUGH_3G = new Dictionary<double, double>();
        Dictionary<double, double> MSR_THROUGH_4G = new Dictionary<double, double>();
        Dictionary<double, double> MSR_HAR_L = new Dictionary<double, double>();
        Dictionary<double, double> MSR_HAR_H = new Dictionary<double, double>();


        Util _Util = new Util();
        DataTable dtLossComp = new DataTable();

        E4438C _SRC_3G;
        E4438C _SRC_4G;

        PM_N1913A _PM_CAL;
        MXA_N9020A _MSR_3G;
        MXA_N9020A _MSR_4G;


        #endregion Variable define

        //Initialize
        public LossComp()
        {
            InitializeComponent();
            this.FormClosing += new FormClosingEventHandler(LossComp_FormClosing);

            #region Build Frequency List

            for (int i = 1; i < TestSetting.MaxTestItem; i++)
            {
                if (Program.ProductTest[i].FreqIn != 0.0 || Program.ProductTest[i].FreqOut != 0.0)
                {
                    // Add Freq in and Freq Out (non Harmonic)
                    if (!SRC_3G.ContainsKey(Program.ProductTest[i].FreqIn))
                    {
                        SRC_3G.Add(Program.ProductTest[i].FreqIn, 0.0);
                        SRC_4G.Add(Program.ProductTest[i].FreqIn, 0.0);
                        MSR_THROUGH_3G.Add(Program.ProductTest[i].FreqIn, 0.0);
                        MSR_THROUGH_4G.Add(Program.ProductTest[i].FreqIn, 0.0);
                    }
                    
                    // Add Freq Out (Harmonic)
                    if (!SRC_4G.ContainsKey(Program.ProductTest[i].FreqOut))
                    {
                        SRC_4G.Add(Program.ProductTest[i].FreqOut, 0.0);
                        MSR_HAR_L.Add(Program.ProductTest[i].FreqOut, 0.0);
                        MSR_HAR_H.Add(Program.ProductTest[i].FreqOut, 0.0);
                    }
   
                }
            }
            #endregion Build Frequency List

            #region Initialize GridView

            int intcolWidth = dgvResult.Width / 5;

            dtLossComp.Columns.Add(new DataColumn("Frequency", typeof(string)));
            dtLossComp.Columns.Add(new DataColumn("Result", typeof(string)));
            dtLossComp.Columns.Add(new DataColumn("LowLimit", typeof(string)));
            dtLossComp.Columns.Add(new DataColumn("UpperLimit", typeof(string)));
            dtLossComp.Columns.Add(new DataColumn("Status", typeof(string)));

            dgvResult.DataSource = dtLossComp;

            dgvResult.Columns["Frequency"].Width = 90;
            dgvResult.Columns["Result"].Width = 80;
            dgvResult.Columns["LowLimit"].Width = 80;
            dgvResult.Columns["UpperLimit"].Width = 80;
            dgvResult.Columns["Status"].Width = 80;

            dgvResult.AllowUserToAddRows = false;
            dgvResult.AllowUserToOrderColumns = false;
            dgvResult.AllowUserToResizeRows = false;
            dgvResult.ReadOnly = true;
            dgvResult.RowHeadersVisible = false;
            

            #endregion Initialize GridView

            #region Initialize instruments

            try
            {
                if (Program.Location == LocationList.BJ_1)
                {
                    _SRC_3G = new E4438C(Instruments_address._19);
                    _SRC_4G = new E4438C(Instruments_address._20);

                    _MSR_3G = new MXA_N9020A(Instruments_address._18);
                    _MSR_4G = new MXA_N9020A(Instruments_address._17);

                    _PM_CAL = new PM_N1913A(Instruments_address._15);

                    _SRC_3G.Initialize();
                    _SRC_4G.Initialize();

                    _MSR_3G.Initialize(true);
                    _MSR_3G.SetAttenuattor(0);
                    _MSR_4G.Initialize(true);
                    _MSR_4G.SetAttenuattor(0);

                    _PM_CAL.Initialize(true);
                }
                else if (Program.Location == LocationList.SH_1)
                {
                }
                else if (Program.Location == LocationList.SH_2)
                {
                }
                else if (Program.Location == LocationList.BJ_2 || Program.Location == LocationList.SH_3)
                {
                }
                else
                {
                    throw new Exception("Bad Location");
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Refresh();
            }

            #endregion Intialize instruments

        }

        private void LossComp_Load(object sender, EventArgs e)
        {
            //btnNext.Enabled = true;
            btnMeasure.Enabled = false;

            this.Text = Program.tp + " Loss Compensation";

            lblStep.Text = "This is loss cal program for "+ Program.tp +",\r\nClick Next button to start calibration";
        }

        private void LossComp_FormClosing(object sender, FormClosingEventArgs e)
        {
            //throw new NotImplementedException();
            try
            {
                #region BJ_1
                if (Program.Location == LocationList.BJ_1)
                {
                    _SRC_3G.Dispose();
                    _SRC_4G.Dispose();
                    _MSR_3G.Dispose();
                    _MSR_4G.Dispose();
                    _PM_CAL.Dispose();
                }
                #endregion BJ_1

                #region SH_1
                else if (Program.Location == LocationList.SH_1)
                { }
                #endregion SH1

                #region SH_2
                else if (Program.Location == LocationList.SH_2)
                {
                }
                #endregion SH_2

                #region BJ_2 & SH_2 & SH_3
                else if (Program.Location == LocationList.BJ_2 || Program.Location == LocationList.SH_3)
                {
                }
                #endregion BJ_2 & SH_2 & SH_3

                else
                {
                    throw new Exception("Bad Location");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnMeasure_Click(object sender, EventArgs e)
        {
            int intIndex = 1;
            double dblResult = 0;
            btnMeasure.Enabled = false;
            dtLossComp.Clear();

            switch (intStep)
            {
                // Power Sensor cal   
                case 1:
                    #region *** Power Sensor cal ***
                    {
                        #region BJ_1
                        if (Program.Location == LocationList.BJ_1)
                        {
                            lblTime.Text = "CAL Power Meter, this will take over 35 seconds";
                            lblTime.Refresh();
                            dblResult = _PM_CAL.CAL();
                            _Util.Wait(intDelay_PowerMeter);

                            lblTime.Text = "";
                            UpdateGrid(intStep, 50, dblResult, -1, 1);

                        }
                        #endregion BJ_1

                        else
                        {
                            throw new Exception("Bad Location");
                        }
                        break;
                    }
                    #endregion *** Power Sensor cal ***
                // TX(3G) Source cal
                case 2:
                    #region *** TX(3G) Source cal ***
                    {
                        #region BJ_1
                        if (Program.Location == LocationList.BJ_1)
                        {
                            _SRC_3G.SetFrequency(500);
                            _SRC_3G.SetPower(0);
                            _SRC_3G.SetOutput(Output.ON);
                            List<double> FreqList = new List<double>(SRC_3G.Keys);
                            FreqList.Sort();
                            foreach (double dblFreq in FreqList)
                            {
                                _SRC_3G.SetFrequency(dblFreq);
                                _PM_CAL.Configure__CW_Power(dblFreq, 20);

                                _Util.Wait(Math.Max(intDelay_SigGen, intDelay_PowerMeter));

                                dblResult = 0 - _PM_CAL.GetPowerResult();
                                SRC_3G[dblFreq] = dblResult;
                                UpdateGrid(intIndex, dblFreq, dblResult, 3, 6);
                                intIndex++;
                            }
                            _SRC_3G.SetOutput(Output.OFF);
                        }
                        #endregion BJ_1

                        else
                        {
                            throw new Exception("Bad Location");
                        }
                        break;
                    }
                #endregion *** TX(3G) Source cal ***
                // TX(4G) Source cal
                case 3:
                    #region *** TX(4G) Source cal ***
                    {
                        #region BJ_1
                        if (Program.Location == LocationList.BJ_1)
                        {
                            _SRC_4G.SetFrequency(500);
                            _SRC_4G.SetPower(0);
                            _SRC_4G.SetOutput(Output.ON);

                            List<double> FreqList = new List<double>(SRC_4G.Keys);
                            FreqList.Sort();

                            foreach (double dblFreq in FreqList)
                            {
                                _SRC_4G.SetFrequency(dblFreq);
                                _PM_CAL.Configure__CW_Power(dblFreq, 20);

                                _Util.Wait(Math.Max(intDelay_SigGen, intDelay_PowerMeter));

                                dblResult = 0 - _PM_CAL.GetPowerResult();
                                SRC_4G[dblFreq] = dblResult;

                                UpdateGrid(intIndex, dblFreq, dblResult, 3, 7);

                                intIndex++;
                            }
                            _SRC_4G.SetOutput(Output.OFF);
                        }
                        #endregion BJ_1

                        else
                        {
                            throw new Exception("Bad Location");
                        }
                        break;
                    }
                    #endregion *** TX(4G) Source cal ***
                // MSR(3G) Measure cal
                case 4:
                    #region *** MSR(3G) Measure cal ***
                    {
                        #region BJ_1
                        if (Program.Location == LocationList.BJ_1)
                        {
                            _SRC_3G.SetFrequency(500);
                            _SRC_3G.SetPower(0);
                            _SRC_3G.SetOutput(Output.ON);
                            List<double> FreqList = new List<double>(MSR_THROUGH_3G.Keys);
                            FreqList.Sort();
                            foreach (double dblFreq in FreqList)
                            {
                                _SRC_3G.SetFrequency(dblFreq);
                                _SRC_3G.SetPower(0 + SRC_3G[dblFreq]);
                                _MSR_3G.Config__CW_Power_FreeRun(dblFreq, 30, 0, 10, 10);
                                _MSR_3G.SetAttenuattor(10);
                                _MSR_3G.SetFrequency(dblFreq);

                                _Util.Wait(Math.Max(intDelay_SigGen, intDelay_MXA));

                                dblResult = 0 - _MSR_3G.Get_CW_PowerResult();
                                MSR_THROUGH_3G[dblFreq] = dblResult;
                                UpdateGrid(intIndex, dblFreq, dblResult, 26, 33);
                                intIndex++;
                            }
                            _SRC_3G.SetOutput(Output.OFF);
                        }
                        #endregion BJ_1

                        else
                        {
                            throw new Exception("Bad Location");
                        }
                        break;
                    }
                    #endregion *** MSR(3G) Measure cal ***
                // MSR(4G) Measure cal
                case 5:
                    #region *** MSR(4G) Measure cal ***
                    {
                        #region BJ_1
                        if (Program.Location == LocationList.BJ_1)
                        {
                            _SRC_4G.SetFrequency(500);
                            _SRC_4G.SetPower(0);
                            _SRC_4G.SetOutput(Output.ON);
                            List<double> FreqList = new List<double>(MSR_THROUGH_4G.Keys);
                            FreqList.Sort();
                            foreach (double dblFreq in FreqList)
                            {
                                _SRC_4G.SetFrequency(dblFreq);
                                _SRC_4G.SetPower(0 + SRC_3G[dblFreq]);
                                _MSR_4G.Config__CW_Power_FreeRun(dblFreq, 30, 0, 10, 10);
                                _MSR_4G.SetAttenuattor(10);
                                _MSR_4G.SetFrequency(dblFreq);

                                _Util.Wait(Math.Max(intDelay_SigGen, intDelay_MXA));

                                dblResult = 0 - _MSR_4G.Get_CW_PowerResult();
                                MSR_THROUGH_4G[dblFreq] = dblResult;
                                UpdateGrid(intIndex, dblFreq, dblResult, 26, 33);
                                intIndex++;
                            }
                            _SRC_4G.SetOutput(Output.OFF);
                        }
                        #endregion BJ_1

                        else
                        {
                            throw new Exception("Bad Location");
                        }
                        break;
                    }
                    #endregion *** MSR(3G) Measure cal ***
                // LB Harmonic Measure cal
                case 6:
                    #region *** LB Harmonic Measure cal ***
                    {
                        #region BJ_1
                        if (Program.Location == LocationList.BJ_1)
                        {
                            _SRC_4G.SetFrequency(500);
                            _SRC_4G.SetPower(0);
                            _SRC_4G.SetOutput(Output.ON);
                            List<double> FreqList = new List<double>(MSR_HAR_L.Keys);
                            FreqList.Sort();
                            foreach (double dblFreq in FreqList)
                            {
                                _SRC_4G.SetFrequency(dblFreq);
                                _SRC_4G.SetPower(0 + SRC_4G[dblFreq]);
                                _MSR_4G.Config__CW_Power_FreeRun(dblFreq, 30, 0, 10, 10);
                                _MSR_4G.SetAttenuattor(10);
                                _MSR_4G.SetFrequency(dblFreq);

                                _Util.Wait(Math.Max(intDelay_SigGen*3, intDelay_MXA));

                                dblResult = 0 - _MSR_4G.Get_CW_PowerResult();
                                MSR_HAR_L[dblFreq] = dblResult;
                                UpdateGrid(intIndex, dblFreq, dblResult, 30, 36);
                                intIndex++;
                            }
                            _SRC_4G.SetOutput(Output.OFF);
                        }
                        #endregion BJ_1

                        else
                        {
                            throw new Exception("Bad Location");
                        }
                        break;
                    }
                    #endregion *** LB Harmonic Measure cal ***
                // HB Harmonic Measure cal
                case 7:
                    #region *** HB Harmonic Measure cal ***
                    {
                        #region BJ_1
                        if (Program.Location == LocationList.BJ_1)
                        {
                            _SRC_4G.SetFrequency(500);
                            _SRC_4G.SetPower(0);
                            _SRC_4G.SetOutput(Output.ON);
                            List<double> FreqList = new List<double>(MSR_HAR_H.Keys);
                            FreqList.Sort();
                            foreach (double dblFreq in FreqList)
                            {
                                _SRC_4G.SetFrequency(dblFreq);
                                _SRC_4G.SetPower(0 + SRC_4G[dblFreq]);
                                _MSR_4G.Config__CW_Power_FreeRun(dblFreq, 30, 0, 10, 10);
                                _MSR_4G.SetAttenuattor(10);
                                _MSR_4G.SetFrequency(dblFreq);

                                _Util.Wait(Math.Max(intDelay_SigGen, intDelay_MXA));

                                dblResult = 0 - _MSR_4G.Get_CW_PowerResult();
                                MSR_HAR_H[dblFreq] = dblResult;

                                if (dblFreq < 2400)
                                    UpdateGrid(intIndex, dblFreq, dblResult, 50, 80);
                                else
                                    UpdateGrid(intIndex, dblFreq, dblResult, 30, 37);

                                intIndex++;
                            }
                            _SRC_4G.SetOutput(Output.OFF);
                        }
                        #endregion BJ_1

                        else
                        {
                            throw new Exception("Bad Location");
                        }
                        break;
                    }
                    #endregion *** HB Harmonic Measure cal ***
            }



            btnMeasure.Text = "Retry";
            btnNext.Enabled = true;
            btnMeasure.Enabled = true;
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            intStep++;
            //if (intStep == 2) intStep++;    // bypass step 2 so far
            btnMeasure.Text = "Cal";
            btnMeasure.Enabled = true;
            btnNext.Enabled = false;

            #region "Cal Steps description"

            switch (intStep)
            {
                case 1:
                    {
                        lblStep.Text = @"Step 1 of 7: Cal Power Sensor 

Connect Power Sensor to Ref port";
                        break;
                    }
                case 2:
                    {
                        lblStep.Text = @"Step 2 of 7: Cal SRC_3G RF IN path 

Connect SigGen(E4438C[:19:]) RF IN cable(with 3dB pad) to power sensor";
                        break;
                    }
                case 3:
                    {
                        lblStep.Text = @"Step 3 of 7: Cal SRC_4G RF IN path 

Connect SigGen(E4438C[:20:]) RF IN cable(with 3dB pad) to power sensor";
                        break;
                    }
                case 4:
                    {
                        lblStep.Text = @"Step 4 of 7: Cal MSR_3G RF OUT path 

Connect SigGen(E4438C[:19:]) RF IN and RF OUT cable(with total 30dB pad) use a through
And Connect VSA(N90x0A[:18:]) to coupler output";
                       
                        break;
                    }
                case 5:
                    {
                        lblStep.Text = @"Step 5 of 7: Cal MSR_4G RF OUT path 

Connect SigGen(E4438C[:20:]) RF IN and RF OUT cable(with total 30dB pad) use a through
And connect VSA(N90x0A[:17:]) to coupler 10dB out without filter";
                        break;
                    }
                case 6:
                    {
                        lblStep.Text = @"Step 6 of 7: Cal LB Harmonic path 


Connect VSA(N90x0A[:17:]) to coupler 10dB out with LB filter";
                        break;

                    }
                case 7:
                    {
                        lblStep.Text = @"Step 7 of 7: Cal HB Harmonic path 

Change LB filter with HB filter";
                        break;
                    }
                default:
                    break;
            }

            #endregion "Cal Steps description"

            dtLossComp.Clear();

            if (blnFinish)
            {
                #region Save loss comp result
                for (int i = 1; i < TestSetting.MaxTestItem; i++)
                {
                    if (Program.ProductTest[i].Description != null && Program.ProductTest[i].FreqIn != 0.0)
                    {
                        if (Program.ProductTest[i].Description.ToUpper().Contains("CW"))
                        {
                            Program.ProductTest[i].LossIn = SRC_3G[Program.ProductTest[i].FreqIn];

                            if (Program.ProductTest[i].Description.ToUpper().Contains("HARMONIC"))
                            {
                                if (Program.ProductTest[i].Description.ToUpper().Contains("LB"))
                                    Program.ProductTest[i].LossOut = MSR_HAR_L[Program.ProductTest[i].FreqOut];
                                else if (Program.ProductTest[i].Description.ToUpper().Contains("HB"))
                                    Program.ProductTest[i].LossOut = MSR_HAR_H[Program.ProductTest[i].FreqOut];
                            }
                            else
                            {
                                Program.ProductTest[i].LossOut = MSR_THROUGH_3G[Program.ProductTest[i].FreqOut];
                            }
                        }
                        else if (Program.ProductTest[i].Description.ToUpper().Contains("RX"))
                        {
                            Program.ProductTest[i].LossIn = SRC_3G[Program.ProductTest[i].FreqIn];
                            Program.ProductTest[i].LossOut = MSR_THROUGH_3G[Program.ProductTest[i].FreqOut];
                        }
                        else if (Program.ProductTest[i].Description.ToUpper().Contains("EDGE"))
                        {
                            Program.ProductTest[i].LossIn = SRC_3G[Program.ProductTest[i].FreqIn];
                            Program.ProductTest[i].LossOut = MSR_THROUGH_3G[Program.ProductTest[i].FreqOut];
                        }
                        else if (Program.ProductTest[i].Description.ToUpper().Contains("TDSCDMA"))
                        {
                            Program.ProductTest[i].LossIn = SRC_3G[Program.ProductTest[i].FreqIn];
                            Program.ProductTest[i].LossOut = MSR_THROUGH_3G[Program.ProductTest[i].FreqOut];
                        }
                        else if (Program.ProductTest[i].Description.ToUpper().Contains("WCDMA"))
                        {
                            Program.ProductTest[i].LossIn = SRC_3G[Program.ProductTest[i].FreqIn];
                            if (Program.ProductTest[i].Description.ToUpper().Contains("HARMONIC"))
                            {
                                Program.ProductTest[i].LossOut = MSR_HAR_H[Program.ProductTest[i].FreqOut];
                            }
                            else
                            {
                                Program.ProductTest[i].LossOut = MSR_THROUGH_3G[Program.ProductTest[i].FreqOut];
                            }
                        }
                        else if (Program.ProductTest[i].Description.ToUpper().Contains("TDD"))
                        {
                            Program.ProductTest[i].LossIn = SRC_4G[Program.ProductTest[i].FreqIn];
                            Program.ProductTest[i].LossOut = MSR_THROUGH_4G[Program.ProductTest[i].FreqOut];
                        }
                        else if (Program.ProductTest[i].Description.ToUpper().Contains("LTE"))
                        {
                            Program.ProductTest[i].LossIn = SRC_4G[Program.ProductTest[i].FreqIn];
                            Program.ProductTest[i].LossOut = MSR_THROUGH_4G[Program.ProductTest[i].FreqOut];
                        }

                    }
                }
                #endregion Save loss comp result

                #region Save loss comp file
                //Save cal result to txt file
                StringBuilder sbLC = new StringBuilder();
                sbLC.AppendLine(DateTime.Now.ToString() + ",,,,,,,,,,,,,,,,,,");
                sbLC.AppendLine(Program.csvHeader);
                for (int i = 1; i < TestSetting.MaxTestItem; i++)
                {
                    if (Program.ProductTest[i].Description != null)
                    {
                        sbLC.Append(Program.ProductTest[i].Item.ToString() + ",");
                        sbLC.Append(Program.ProductTest[i].TestItem.ToString() + ",");
                        sbLC.Append(Program.ProductTest[i].Description.ToString() + ",");
                        sbLC.Append(Program.ProductTest[i].Units.ToString() + ",");
                        sbLC.Append(Program.ProductTest[i].LowLimit.ToString() + ",");
                        sbLC.Append(Program.ProductTest[i].HighLimit.ToString() + ",");
                        sbLC.Append(Program.ProductTest[i].VCC.ToString() + ",");

                        if (Program.ProductTest[i].VBAT == 0.01)
                            sbLC.Append("0,");
                        else
                            sbLC.Append(Program.ProductTest[i].VBAT.ToString() + ",");

                        if (Program.ProductTest[i].Vramp == 0.01)
                            sbLC.Append("0,");
                        else
                            sbLC.Append(Program.ProductTest[i].Vramp.ToString() + ",");

                        if (Program.ProductTest[i].Txen_Ven == 0.01)
                            sbLC.Append("0,");
                        else
                            sbLC.Append(Program.ProductTest[i].Txen_Ven.ToString() + ",");

                        if (Program.ProductTest[i].Gpctrl0_Vmode0 == 0.01)
                            sbLC.Append("0,");
                        else
                            sbLC.Append(Program.ProductTest[i].Gpctrl0_Vmode0.ToString() + ",");

                        if (Program.ProductTest[i].Gpctrl1_Vmode1 == 0.01)
                            sbLC.Append("0,");
                        else
                            sbLC.Append(Program.ProductTest[i].Gpctrl1_Vmode1.ToString() + ",");

                        if (Program.ProductTest[i].Gpctrl2_Vmode2 == 0.01)
                            sbLC.Append("0,");
                        else
                            sbLC.Append(Program.ProductTest[i].Gpctrl2_Vmode2.ToString() + ",");

                        sbLC.Append(Program.ProductTest[i].Pin.ToString() + ",");
                        sbLC.Append(Program.ProductTest[i].Pout.ToString() + ",");
                        sbLC.Append(Program.ProductTest[i].FreqIn.ToString() + ",");
                        sbLC.Append(Program.ProductTest[i].FreqOut.ToString() + ",");
                        sbLC.Append(Program.ProductTest[i].LossIn.ToString() + ",");
                        sbLC.Append(Program.ProductTest[i].LossOut.ToString() + ",");
                        sbLC.AppendLine(Program.ProductTest[i].SocketOffset.ToString());
                    }
                }


                //Write Loss Comp file
                FileInfo file = new FileInfo(Program.strFilePath_Testcfg);
                if (file.Exists)
                {
                    file.Delete();
                }

                StreamWriter swLC = new StreamWriter(Program.strFilePath_Testcfg, true);
                swLC.WriteLine(sbLC.ToString());
                swLC.Close();

                #endregion Save loss comp file

                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                this.Close();
            }

            if (intStep == 7)
            {
                blnFinish = true;
                btnNext.Text = "Finish";
            }
        }

        private void UpdateGrid(int intIndex, double dblFrequncy_MHz, double dblResult_dBm, double dblLL, double dblUL)
        {            
            if (intIndex != 0)
            {
                DataRow dr = dtLossComp.NewRow();

                dr[0] = Convert.ToString(dblFrequncy_MHz) + "MHz";
                dr[1] = Convert.ToString(Math.Round(dblResult_dBm, 2)) + "dB";
                dr[2] = Convert.ToString(dblLL) + "dB";
                dr[3] = Convert.ToString(dblUL) + "dB";

                if ((dblResult_dBm >= dblLL) && (dblResult_dBm <= dblUL))
                {
                    dr[4] = "Pass";
                }
                else
                {
                    dr[4] = "Fail";
                }

                dtLossComp.Rows.Add(dr);
                //gvResult.DataSource = dtLossComp;
                this.Refresh();
            }
        }

        private void dgvResult_CellFormatting(object sender, System.Windows.Forms.DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == 1)
            {
                if (e.Value == DBNull.Value)    //if DBNull
                    e.CellStyle.BackColor = Color.WhiteSmoke;
                else
                {
                    string temp = e.Value.ToString();
                    double dblCellValue = Convert.ToDouble(temp.Substring(0, temp.Length - 2));

                    temp = dgvResult.Rows[e.RowIndex].Cells[e.ColumnIndex + 1].Value.ToString();
                    double dblLowLimit = Convert.ToDouble(temp.Substring(0, temp.Length - 2));

                    temp = dgvResult.Rows[e.RowIndex].Cells[e.ColumnIndex + 2].Value.ToString();
                    double dblHighLimit = Convert.ToDouble(temp.Substring(0, temp.Length - 2));

                    if (dblCellValue < dblLowLimit || dblCellValue > dblHighLimit)
                    {
                        //e.CellStyle.BackColor = Color.Red;
                        dgvResult.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Red;
                    }
                    else
                    {
                        dgvResult.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Green;
                    }
                }// end of if DBNull
            }
        }

    }
}
