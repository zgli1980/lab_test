///     Reversion history log
///     Rev1.0      Initial build                                                                AceLi       2012-08-09

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

namespace Bench_Test
{
    public partial class NewLossComp : Form
    {
        #region Variable define

        private static int intDelay_SigGen = TestSetting.DELAY_SIGGEN_in_ms;
        private static int intDelay_PowerMeter = TestSetting.DELAY_POWER_METER_in_ms;
        private static int intDelay_MXA = TestSetting.DELAY_VSA_in_ms;
        private static int intPowerMeter_avg = TestSetting.POWERMETER_AVERAGE_CW;

        private bool blnFinish = false;
        private int intStep = 0;


        Util _Util = new Util();
        DataTable dtLossComp = new DataTable();

        E4438C _E4438C;
        HP8665B _HP8665B;

        Arb_33522A _Arb33522A;
        Arb_33522A_USB _Arb33522A_USB;
        Arb_33120A _Arb33120A;
        Arb_33220A _Arb33220A;

        PS_66332A _PS66332A;

        PM_437B _PM_437B;
        PM_U2001A _PM_U2001A;
        PM_N1913A _PM_N1913A;
        MXA_N9020A _MXAN9020A;


        #endregion Variable define

        public NewLossComp()
        {
            InitializeComponent();

            lblInfo.Text = "Please click Next for cable loss calibration";
            lblError.Text = "";
            lblError.ForeColor = Color.Red;
            btnCal.Enabled = false;

            this.FormClosing += new FormClosingEventHandler(NewLossComp_FormClosing);
            this.dgvLossResult.CellFormatting += new DataGridViewCellFormattingEventHandler(dgvLossResult_CellFormatting);
            this.dgvLossResult.RowsAdded += new DataGridViewRowsAddedEventHandler(dgvLossResult_RowsAdded);


            #region Initialize GridView

            int intcolWidth = dgvLossResult.Width / 5;

            dtLossComp.Columns.Add(new DataColumn("Frequency", typeof(string)));
            dtLossComp.Columns.Add(new DataColumn("Result", typeof(string)));
            dtLossComp.Columns.Add(new DataColumn("LowLimit", typeof(string)));
            dtLossComp.Columns.Add(new DataColumn("UpperLimit", typeof(string)));
            dtLossComp.Columns.Add(new DataColumn("Status", typeof(string)));

            dgvLossResult.DataSource = dtLossComp;

            dgvLossResult.Columns["Frequency"].Width = 90;
            dgvLossResult.Columns["Result"].Width = 80;
            dgvLossResult.Columns["LowLimit"].Width = 80;
            dgvLossResult.Columns["UpperLimit"].Width = 80;
            dgvLossResult.Columns["Status"].Width = 80;

            dgvLossResult.AllowUserToAddRows = false;
            dgvLossResult.AllowUserToOrderColumns = false;
            dgvLossResult.AllowUserToResizeRows = false;
            dgvLossResult.ReadOnly = true;
            dgvLossResult.RowHeadersVisible = false;


            #endregion Initialize GridView

            #region Initialize instruments

            try
            {
                if (Program.Location == LocationList.BJ_1)
                {
                    _E4438C = new E4438C(Instruments_address._12);
                    _HP8665B = new HP8665B(Instruments_address._11);

                    _Arb33522A_USB = new Arb_33522A_USB(Instruments_VISA.Arb_33522A);
                    _Arb33120A = new Arb_33120A(Instruments_address._16);
                    _PS66332A = new PS_66332A(Instruments_address._05);

                    _PM_437B = new PM_437B(Instruments_address._23);
                    _PM_U2001A = new PM_U2001A();
                    _MXAN9020A = new MXA_N9020A(Instruments_address._22);

                    _E4438C.Initialize();
                    _HP8665B.Initialize();

                    _Arb33120A.Initialize();
                    _Arb33522A_USB.Initialize(208.5);
                    _PS66332A.Initialize();

                    _PM_437B.Initialize();
                    _PM_U2001A.Initialize();
                    _MXAN9020A.Initialize(true);
                    _MXAN9020A.SetAttenuattor(0);
                }
                else if (Program.Location == LocationList.SH_1)
                {
                    _PS66332A = new PS_66332A(Instruments_address._05);
                    _Arb33522A_USB = new Arb_33522A_USB(Instruments_VISA.Arb_33522A);
                    _Arb33220A = new Arb_33220A(Instruments_address._11);
                    _PM_N1913A = new PM_N1913A(Instruments_address._15);
                    _MXAN9020A = new MXA_N9020A(Instruments_address._18);
                    _E4438C = new E4438C(Instruments_address._19);

                    _E4438C.Initialize();
                    _Arb33220A.Initialize();
                    _Arb33522A_USB.Initialize(TestSetting.ARB_PULSE_FREQ_GMSK_in_khz);
                    _PS66332A.Initialize();
                    _PM_N1913A.Initialize(true);
                    _MXAN9020A.Initialize(true);
                    _MXAN9020A.SetAttenuattor(0);
                }
                else if (Program.Location == LocationList.SH_2)
                {
                    _PS66332A = new PS_66332A(Instruments_address._05);
                    _Arb33522A = new Arb_33522A(Instruments_address._10);
                    _PM_N1913A = new PM_N1913A(Instruments_address._15);
                    _MXAN9020A = new MXA_N9020A(Instruments_address._18);
                    _E4438C = new E4438C(Instruments_address._19);
                    _HP8665B = new HP8665B(Instruments_address._20);

                    _E4438C.Initialize();
                    _HP8665B.Initialize();
                    _PS66332A.Initialize();
                    _PM_N1913A.Initialize(true);
                    _Arb33522A.Initialize(TestSetting.ARB_PULSE_FREQ_GMSK_in_khz);
                    _MXAN9020A.Initialize(true);
                    _MXAN9020A.SetAttenuattor(0);
                }
                else if (Program.Location == LocationList.SH_3 || Program.Location == LocationList.SH_4)
                {
                    _PS66332A = new PS_66332A(Instruments_address._05);
                    _Arb33522A = new Arb_33522A(Instruments_address._10);
                    _Arb33220A = new Arb_33220A(Instruments_address._11);
                    _PM_N1913A = new PM_N1913A(Instruments_address._15);
                    _MXAN9020A = new MXA_N9020A(Instruments_address._18);
                    _E4438C = new E4438C(Instruments_address._19);

                    _E4438C.Initialize();
                    _PS66332A.Initialize();
                    _PM_N1913A.Initialize(true);
                    _Arb33220A.Initialize();
                    _Arb33522A.Initialize(TestSetting.ARB_PULSE_FREQ_GMSK_in_khz);
                    _MXAN9020A.Initialize(true);
                    _MXAN9020A.SetAttenuattor(0);
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

        void dgvLossResult_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            // Automatically scroll up
            if (e.RowIndex > 10)
            {
                //dgvLossResult.Rows[e.RowIndex].Selected = true;
                dgvLossResult.FirstDisplayedScrollingRowIndex = e.RowIndex;
            }
        }

        void dgvLossResult_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {

            if (e.ColumnIndex == 1)
            {
                if (e.Value == DBNull.Value)    //if DBNull
                    e.CellStyle.BackColor = Color.WhiteSmoke;
                else
                {
                    string temp = e.Value.ToString();
                    double dblCellValue = Convert.ToDouble(temp.Substring(0, temp.Length - 2));

                    temp = dgvLossResult.Rows[e.RowIndex].Cells[e.ColumnIndex + 1].Value.ToString();
                    double dblLowLimit = Convert.ToDouble(temp.Substring(0, temp.Length - 2));

                    temp = dgvLossResult.Rows[e.RowIndex].Cells[e.ColumnIndex + 2].Value.ToString();
                    double dblHighLimit = Convert.ToDouble(temp.Substring(0, temp.Length - 2));

                    if (dblCellValue < dblLowLimit || dblCellValue > dblHighLimit)
                    {
                        //e.CellStyle.BackColor = Color.Red;
                        dgvLossResult.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Red;
                    }
                    else
                    {
                        dgvLossResult.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Green;
                    }
                }// end of if DBNull
            }
            //// Automatically scroll up
            //dgvLossResult.Rows[e.RowIndex].Selected = true;
            //dgvLossResult.FirstDisplayedScrollingRowIndex = e.RowIndex;
        }

        void NewLossComp_FormClosing(object sender, FormClosingEventArgs e)
        {
            //throw new NotImplementedException();
            try
            {
                #region BJ_1
                if (Program.Location == LocationList.BJ_1)
                {
                }
                #endregion BJ_1
                #region SH_1
                else if (Program.Location == LocationList.SH_1)
                {
                    _Arb33522A_USB.Dispose();
                    _Arb33220A.Dispose();
                    _E4438C.Dispose();
                    _MXAN9020A.Dispose();
                    _PS66332A.Dispose();
                    _PM_N1913A.Dispose();
                }
                #endregion SH_1
                #region SH_2
                else if (Program.Location == LocationList.SH_2)
                {
                    _Arb33522A.Dispose();
                    _E4438C.Dispose();
                    _HP8665B.Dispose();
                    _MXAN9020A.Dispose();
                    _PS66332A.Dispose();
                    _PM_N1913A.Dispose();
                }
                #endregion SH_2
                #region SH_3 & SH4
                else if (Program.Location == LocationList.SH_3 || Program.Location == LocationList.SH_4)
                {
                    _Arb33220A.Dispose();
                    _Arb33522A.Dispose();
                    _E4438C.Dispose();
                    _MXAN9020A.Dispose();
                    _PS66332A.Dispose();
                    _PM_N1913A.Dispose();
                }
                #endregion SH_3
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

        private void btnCal_Click(object sender, EventArgs e)
        {

            int intIndex = 1;
            double dblResult = 0;
            btnCal.Enabled = false;
            dtLossComp.Clear();

            switch (intStep)
            {
                case 1:
                    #region *** Power Sensor cal ***
                    {
                        if (Program.Location == LocationList.BJ_1)
                        {   
                            lblError.Text = "CAL E437B, this will take over 35 seconds";
                            lblError.Refresh();
                            dblResult = _PM_437B.CAL();
                            //dblResult = _PM_U2001A.CAL(U2001_ZeroType.External);
                            _Util.Wait(intDelay_PowerMeter);

                            lblError.Text = "";
                            UpdateGrid(intStep, 50, dblResult, -1, 1);


                            lblError.Text = "Cal U2001A, this will take over 35 seconds";
                            lblError.Refresh();
                            //dblResult = _PM_437B.CAL();
                            dblResult = _PM_U2001A.CAL(U2001_ZeroType.External);
                            _Util.Wait(intDelay_PowerMeter);

                            lblError.Text = "";
                            UpdateGrid(intStep + 1, 50, dblResult, -1, 1);
                        }
                        else if (Program.Location == LocationList.SH_1 || 
                                 Program.Location == LocationList.SH_2 ||
                                 Program.Location == LocationList.SH_3 ||
                                 Program.Location == LocationList.SH_4)
                        {
                            lblError.Text = "This will take over 35 seconds";
                            lblError.Refresh();
                            dblResult = _PM_N1913A.CAL();
                            _Util.Wait(intDelay_PowerMeter);

                            lblError.Text = "";
                            UpdateGrid(intStep, 50, dblResult, -1, 1);
                        }
                        else
                        {
                            throw new Exception("Bad Location");
                        }
                        break;
                    }
                    #endregion *** Power Sensor cal ***

                case 2:
                    #region *** TX Source cal ***
                    {
                        #region BJ_1
                        if (Program.Location == LocationList.BJ_1)
                        {
                            _E4438C.SetFrequency(500);
                            _E4438C.SetPower(0);
                            _E4438C.SetOutput(Output.ON);

                            List<double> FreqList = new List<double>(TestSetting.LOSS_SRC.Keys);
                            FreqList.Sort();

                            foreach (double dblFreq in FreqList)
                            {
                                _E4438C.SetFrequency(dblFreq);
                                //_PM_U2001A.Configure__CW_Power(dblFreq, intPowerMeter_avg);
                                _PM_437B.Configure__CW_Power(dblFreq, 20, 3, 1);

                                _Util.Wait(Math.Max(intDelay_SigGen, intDelay_PowerMeter));

                                dblResult = 0 - _PM_437B.GetPowerResult();
                                TestSetting.LOSS_SRC[dblFreq] = dblResult;

                                if (dblFreq < 4000)
                                    UpdateGrid(intIndex, dblFreq, dblResult, 3, 5);
                                else
                                    UpdateGrid(intIndex, dblFreq, dblResult, 3.5, 6);

                                intIndex++;
                            }
                            _E4438C.SetOutput(Output.OFF);
                        }
                        #endregion BJ_1
                        #region SH_1 & SH3 & SH4
                        else if (Program.Location == LocationList.SH_1 ||
                                 Program.Location == LocationList.SH_3 ||
                                 Program.Location == LocationList.SH_4)
                        {
                            _E4438C.SetFrequency(500);
                            _E4438C.SetPower(0);
                            _E4438C.SetOutput(Output.ON);

                            //List<double> FreqList = new List<double>(TestSetting.LOSS_SRC.Keys);
                            List<double> FreqList = new List<double>(TestSetting.LOSS_SRC_ROLL.Keys);
                            FreqList.Sort();

                            foreach (double dblFreq in FreqList)
                            {
                                _E4438C.SetFrequency(dblFreq);
                                _PM_N1913A.Configure__CW_Power(dblFreq, intPowerMeter_avg);

                                _Util.Wait(Math.Max(intDelay_SigGen, intDelay_PowerMeter));

                                dblResult = 0 - _PM_N1913A.GetPowerResult();
                                if (TestSetting.LOSS_SRC.ContainsKey(dblFreq)) TestSetting.LOSS_SRC[dblFreq] = dblResult;
                                TestSetting.LOSS_SRC_ROLL[dblFreq] = dblResult;

                                if (dblFreq < 4000)
                                    UpdateGrid(intIndex, dblFreq, dblResult, 3, 5);
                                else
                                    UpdateGrid(intIndex, dblFreq, dblResult, 3.5, 6);

                                intIndex++;
                            }
                            _E4438C.SetOutput(Output.OFF);
                        }
                        #endregion SH_3
                        #region SH_2
                        else if (Program.Location == LocationList.SH_2)
                        {
                            _E4438C.SetFrequency(500);
                            _E4438C.SetPower(0);
                            _E4438C.SetOutput(Output.ON);

                            List<double> FreqList = new List<double>(TestSetting.LOSS_SRC.Keys);
                            FreqList.Sort();

                            foreach (double dblFreq in FreqList)
                            {
                                _E4438C.SetFrequency(dblFreq);
                                _PM_N1913A.Configure__CW_Power(dblFreq, intPowerMeter_avg);

                                _Util.Wait(Math.Max(intDelay_SigGen, intDelay_PowerMeter));

                                dblResult = 0 - _PM_N1913A.GetPowerResult();
                                TestSetting.LOSS_SRC[dblFreq] = dblResult;

                                if (dblFreq < 4000)
                                    UpdateGrid(intIndex, dblFreq, dblResult, 3, 5);
                                else
                                    UpdateGrid(intIndex, dblFreq, dblResult, 3.5, 6);

                                intIndex++;
                            }
                            _E4438C.SetOutput(Output.OFF);
                        }
                        #endregion SH_2
                        else
                        {
                            throw new Exception("Bad Location");
                        }
                        break;
                    }
                    #endregion *** TX Source cal ***

                case 3:
                    #region *** Roll Source cal ***
                    {
                        #region BJ_1
                        if (Program.Location == LocationList.BJ_1)
                        {
                            _HP8665B.SetFrequency(500);
                            _HP8665B.SetPower(0);
                            _HP8665B.SetOutput(Output.ON);

                            List<double> FreqList = new List<double>(TestSetting.LOSS_SRC_ROLL.Keys);
                            FreqList.Sort();

                            foreach (double dblFreq in FreqList)
                            {
                                _HP8665B.SetFrequency(dblFreq);
                                //_PM_U2001A.Configure__CW_Power(dblFreq, intPowerMeter_avg);
                                _PM_437B.Configure__CW_Power(dblFreq, 20, 3, 1);

                                _Util.Wait(Math.Max(intDelay_SigGen * 15, intDelay_PowerMeter));

                                dblResult = 0 - _PM_437B.GetPowerResult();
                                TestSetting.LOSS_SRC_ROLL[dblFreq] = dblResult;

                                //if (dblFreq < 4500)
                                //    UpdateGrid(intIndex, dblFreq, dblResult, 3, 5);
                                //else
                                UpdateGrid(intIndex, dblFreq, dblResult, 3, 7);

                                intIndex++;
                            }
                            _HP8665B.SetOutput(Output.OFF);
                        }
                        #endregion BJ_1
                        #region SH_1 & SH3 & SH4
                        else if (Program.Location == LocationList.SH_1 ||
                                 Program.Location == LocationList.SH_3 ||
                                 Program.Location == LocationList.SH_4)
                        {
                            _E4438C.SetFrequency(500);
                            _E4438C.SetPower(0);
                            _E4438C.SetOutput(Output.ON);

                            List<double> FreqList = new List<double>(TestSetting.LOSS_SRC_ROLL.Keys);
                            FreqList.Sort();

                            foreach (double dblFreq in FreqList)
                            {
                                _E4438C.SetFrequency(dblFreq);
                                _PM_N1913A.Configure__CW_Power(dblFreq, intPowerMeter_avg);

                                _Util.Wait(Math.Max(intDelay_SigGen, intDelay_PowerMeter));

                                dblResult = 0 - _PM_N1913A.GetPowerResult();
                                TestSetting.LOSS_SRC[dblFreq] = dblResult;

                                if (dblFreq < 2000)
                                    UpdateGrid(intIndex, dblFreq, dblResult, 3, 4);
                                else
                                    UpdateGrid(intIndex, dblFreq, dblResult, 3, 7);

                                intIndex++;
                            }
                            _E4438C.SetOutput(Output.OFF);
                        }
                        #endregion SH_1
                        #region SH_2
                        else if (Program.Location == LocationList.SH_2)
                        {
                            _HP8665B.SetFrequency(500);
                            _HP8665B.SetPower(0);
                            _HP8665B.SetOutput(Output.ON);

                            List<double> FreqList = new List<double>(TestSetting.LOSS_SRC_ROLL.Keys);
                            FreqList.Sort();

                            foreach (double dblFreq in FreqList)
                            {
                                _HP8665B.SetFrequency(dblFreq);
                                _PM_N1913A.Configure__CW_Power(dblFreq, intPowerMeter_avg);

                                _Util.Wait(Math.Max(intDelay_SigGen * 15, intDelay_PowerMeter));

                                dblResult = 0 - _PM_N1913A.GetPowerResult();
                                TestSetting.LOSS_SRC_ROLL[dblFreq] = dblResult;

                                //if (dblFreq < 4500)
                                //    UpdateGrid(intIndex, dblFreq, dblResult, 3, 5);
                                //else
                                    UpdateGrid(intIndex, dblFreq, dblResult, 3, 7);

                                intIndex++;
                            }
                            _HP8665B.SetOutput(Output.OFF);
                        }
                        #endregion SH_2
                        else
                        {
                            throw new Exception("Bad Location");
                        }
                        break;
                    }
                    #endregion *** Roll Source cal ***

                case 4:
                    #region *** Pout Measure cal ***
                    {
                        #region BJ_1
                        if (Program.Location == LocationList.BJ_1)
                        {
                            _E4438C.SetFrequency(500);
                            _E4438C.SetPower(0);
                            _E4438C.SetOutput(Output.ON);

                            List<double> FreqList = new List<double>(TestSetting.LOSS_MSR_POUT.Keys);
                            FreqList.Sort();

                            foreach (double dblFreq in FreqList)
                            {
                                _E4438C.SetFrequency(dblFreq);
                                _E4438C.SetPower(0 + TestSetting.LOSS_SRC[dblFreq]);
                                _PM_U2001A.Configure__CW_Power(dblFreq, intPowerMeter_avg);

                                _Util.Wait(Math.Max(intDelay_SigGen, intDelay_PowerMeter));

                                dblResult = 0 - _PM_U2001A.GetPowerResult();
                                TestSetting.LOSS_MSR_POUT[dblFreq] = dblResult;

                                UpdateGrid(intIndex, dblFreq, dblResult, 19.5, 22);

                                intIndex++;
                            }
                            _E4438C.SetOutput(Output.OFF);
                        }
                        #endregion BJ_1
                        #region SH_1 & SH3 & SH4
                        else if (Program.Location == LocationList.SH_1 ||
                                 Program.Location == LocationList.SH_3 ||
                                 Program.Location == LocationList.SH_4)
                        {
                            _E4438C.SetFrequency(500);
                            _E4438C.SetPower(0);
                            _E4438C.SetOutput(Output.ON);

                            List<double> FreqList = new List<double>(TestSetting.LOSS_MSR_POUT.Keys);
                            FreqList.Sort();

                            foreach (double dblFreq in FreqList)
                            {
                                _E4438C.SetFrequency(dblFreq);
                                _E4438C.SetPower(0 + TestSetting.LOSS_SRC[dblFreq]);
                                _PM_N1913A.Configure__CW_Power(dblFreq, intPowerMeter_avg);

                                _Util.Wait(Math.Max(intDelay_SigGen, intDelay_PowerMeter));

                                dblResult = 0 - _PM_N1913A.GetPowerResult();
                                TestSetting.LOSS_MSR_POUT[dblFreq] = dblResult;

                                UpdateGrid(intIndex, dblFreq, dblResult, 19.5, 22);

                                intIndex++;
                            }
                            _E4438C.SetOutput(Output.OFF);
                        }
                        #endregion SH_1
                        #region SH_2
                        else if (Program.Location == LocationList.SH_2)
                        {
                            _E4438C.SetFrequency(500);
                            _E4438C.SetPower(0);
                            _E4438C.SetOutput(Output.ON);

                            List<double> FreqList = new List<double>(TestSetting.LOSS_MSR_POUT.Keys);
                            FreqList.Sort();

                            foreach (double dblFreq in FreqList)
                            {
                                _E4438C.SetFrequency(dblFreq);
                                _E4438C.SetPower(0 + TestSetting.LOSS_SRC[dblFreq]);
                                _PM_N1913A.Configure__CW_Power(dblFreq, intPowerMeter_avg);

                                _Util.Wait(Math.Max(intDelay_SigGen, intDelay_PowerMeter));

                                dblResult = 0 - _PM_N1913A.GetPowerResult();
                                TestSetting.LOSS_MSR_POUT[dblFreq] = dblResult;

                                UpdateGrid(intIndex, dblFreq, dblResult, 19.5, 22);

                                intIndex++;
                            }
                            _E4438C.SetOutput(Output.OFF);
                        }
                        #endregion SH_2
                        else
                        {
                            throw new Exception("Bad Location");
                        }
                        break;
                    }
                    #endregion *** Pout Measure cal ***

                case 5:
                    #region *** VSA Through Measure cal ***
                    {

                        #region BJ_1
                        if (Program.Location == LocationList.BJ_1)
                        {
                            _E4438C.SetFrequency(500);
                            _E4438C.SetPower(0);
                            _E4438C.SetOutput(Output.ON);

                            List<double> FreqList = new List<double>(TestSetting.LOSS_MSR_THROUGH.Keys);
                            FreqList.Sort();

                            foreach (double dblFreq in FreqList)
                            {
                                _E4438C.SetFrequency(dblFreq);
                                _E4438C.SetPower(0 + TestSetting.LOSS_SRC[dblFreq]);
                                _MXAN9020A.SetFrequency(dblFreq);
                                _MXAN9020A.SetAttenuattor(10);

                                _Util.Wait(Math.Max(intDelay_SigGen, 2 * intDelay_MXA));

                                dblResult = 0 - _MXAN9020A.Get_CW_PowerResult();
                                TestSetting.LOSS_MSR_THROUGH[dblFreq] = dblResult;

                                UpdateGrid(intIndex, dblFreq, dblResult, 20, 23);

                                intIndex++;
                            }
                            _E4438C.SetOutput(Output.OFF);
                        }
                        #endregion BJ_1
                        #region SH_1 & SH3 & SH4
                        else if (Program.Location == LocationList.SH_1 ||
                                 Program.Location == LocationList.SH_3 ||
                                 Program.Location == LocationList.SH_4)
                        {
                            _E4438C.SetFrequency(500);
                            _E4438C.SetPower(0);
                            _E4438C.SetOutput(Output.ON);

                            List<double> FreqList = new List<double>(TestSetting.LOSS_MSR_THROUGH.Keys);
                            FreqList.Sort();

                            foreach (double dblFreq in FreqList)
                            {
                                _E4438C.SetFrequency(dblFreq);
                                _E4438C.SetPower(0 + TestSetting.LOSS_SRC[dblFreq]);
                                _MXAN9020A.SetFrequency(dblFreq);
                                _MXAN9020A.SetAttenuattor(10);

                                _Util.Wait(Math.Max(intDelay_SigGen, 2 * intDelay_MXA));

                                dblResult = 0 - _MXAN9020A.Get_CW_PowerResult();
                                TestSetting.LOSS_MSR_THROUGH[dblFreq] = dblResult;

                                UpdateGrid(intIndex, dblFreq, dblResult, 25, 33);

                                intIndex++;
                            }
                            _E4438C.SetOutput(Output.OFF);
                        }
                        #endregion SH1
                        #region SH_2
                        else if (Program.Location == LocationList.SH_2)
                        {
                            _E4438C.SetFrequency(500);
                            _E4438C.SetPower(0);
                            _E4438C.SetOutput(Output.ON);

                            List<double> FreqList = new List<double>(TestSetting.LOSS_MSR_THROUGH.Keys);
                            FreqList.Sort();

                            foreach (double dblFreq in FreqList)
                            {
                                _E4438C.SetFrequency(dblFreq);
                                _E4438C.SetPower(0 + TestSetting.LOSS_SRC[dblFreq]);
                                _MXAN9020A.SetFrequency(dblFreq);
                                _MXAN9020A.SetAttenuattor(10);

                                _Util.Wait(Math.Max(intDelay_SigGen, 2 * intDelay_MXA));

                                dblResult = 0 - _MXAN9020A.Get_CW_PowerResult();
                                TestSetting.LOSS_MSR_THROUGH[dblFreq] = dblResult;

                                UpdateGrid(intIndex, dblFreq, dblResult, 25, 33);

                                intIndex++;
                            }
                            _E4438C.SetOutput(Output.OFF);
                        }
                        #endregion SH_2
                        else
                        {
                            throw new Exception("Bad Location");
                        }
                        break;
                    }
                #endregion *** VSA Through Measure cal ***

                case 6:
                    #region *** LB Harmonic Measure cal ***
                    {

                        #region BJ_1
                        if (Program.Location == LocationList.BJ_1)
                        {
                            _HP8665B.SetFrequency(500);
                            _HP8665B.SetPower(0);
                            _HP8665B.SetOutput(Output.ON);

                            List<double> FreqList = new List<double>(TestSetting.LOSS_MSR_FILTER_LB.Keys);
                            FreqList.Sort();

                            foreach (double dblFreq in FreqList)
                            {
                                _HP8665B.SetFrequency(dblFreq);
                                _HP8665B.SetPower(0 + TestSetting.LOSS_SRC_ROLL[dblFreq]);
                                _MXAN9020A.SetFrequency(dblFreq);
                                _MXAN9020A.SetAttenuattor(0);

                                _Util.Wait(Math.Max(intDelay_SigGen, intDelay_MXA));

                                dblResult = 0 - _MXAN9020A.Get_CW_PowerResult();
                                TestSetting.LOSS_MSR_FILTER_LB[dblFreq] = dblResult;

                                if (dblFreq < 1200)
                                    UpdateGrid(intIndex, dblFreq, dblResult, 50, 80);
                                else if (dblFreq < 4000)
                                    UpdateGrid(intIndex, dblFreq, dblResult, 30, 34);
                                else
                                    UpdateGrid(intIndex, dblFreq, dblResult, 31, 37);

                                intIndex++;
                            }
                            _E4438C.SetOutput(Output.OFF);
                        }
                        #endregion BJ_1
                        #region SH_1 & SH3
                        else if (Program.Location == LocationList.SH_1 ||
                                 Program.Location == LocationList.SH_3 ||
                                 Program.Location == LocationList.SH_4)
                        {
                            _E4438C.SetFrequency(500);
                            _E4438C.SetPower(0);
                            _E4438C.SetOutput(Output.ON);

                            List<double> FreqList = new List<double>(TestSetting.LOSS_MSR_FILTER_LB.Keys);
                            FreqList.Sort();

                            foreach (double dblFreq in FreqList)
                            {
                                _E4438C.SetFrequency(dblFreq);
                                _E4438C.SetPower(0 + TestSetting.LOSS_SRC_ROLL[dblFreq]);
                                _MXAN9020A.SetFrequency(dblFreq);
                                _MXAN9020A.SetAttenuattor(0);

                                _Util.Wait(Math.Max(intDelay_SigGen, intDelay_MXA));

                                dblResult = 0 - _MXAN9020A.Get_CW_PowerResult();
                                TestSetting.LOSS_MSR_FILTER_LB[dblFreq] = dblResult;

                                if (dblFreq < 1200)
                                    UpdateGrid(intIndex, dblFreq, dblResult, 60, 80);
                                else if (dblFreq < 4000)
                                    UpdateGrid(intIndex, dblFreq, dblResult, 30, 34);
                                else
                                    UpdateGrid(intIndex, dblFreq, dblResult, 31, 37);

                                intIndex++;
                            }
                            _E4438C.SetOutput(Output.OFF);
                        }
                        #endregion SH1
                        #region SH_2
                        else if (Program.Location == LocationList.SH_2)
                        {
                            _HP8665B.SetFrequency(500);
                            _HP8665B.SetPower(0);
                            _HP8665B.SetOutput(Output.ON);

                            List<double> FreqList = new List<double>(TestSetting.LOSS_MSR_FILTER_LB.Keys);
                            FreqList.Sort();

                            foreach (double dblFreq in FreqList)
                            {
                                _HP8665B.SetFrequency(dblFreq);
                                _HP8665B.SetPower(0 + TestSetting.LOSS_SRC_ROLL[dblFreq]);
                                _MXAN9020A.SetFrequency(dblFreq);
                                _MXAN9020A.SetAttenuattor(0);

                                _Util.Wait(Math.Max(intDelay_SigGen, intDelay_MXA));

                                dblResult = 0 - _MXAN9020A.Get_CW_PowerResult();
                                TestSetting.LOSS_MSR_FILTER_LB[dblFreq] = dblResult;

                                if (dblFreq < 1200)
                                    UpdateGrid(intIndex, dblFreq, dblResult, 50, 80);
                                else if (dblFreq < 4000)
                                    UpdateGrid(intIndex, dblFreq, dblResult, 30, 34);
                                else
                                    UpdateGrid(intIndex, dblFreq, dblResult, 31, 37);

                                intIndex++;
                            }
                            _E4438C.SetOutput(Output.OFF);
                        }
                        #endregion SH_2
                        else
                        {
                            throw new Exception("Bad Location");
                        }
                        break;
                    }
                    #endregion *** LB Harmonic Measure cal ***
  
                case 7:
                    #region *** HB Harmonic Measure cal ***
                    {

                        #region BJ_1
                        if (Program.Location == LocationList.BJ_1)
                        {
                            _HP8665B.SetFrequency(500);
                            _HP8665B.SetPower(0);
                            _HP8665B.SetOutput(Output.ON);

                            List<double> FreqList = new List<double>(TestSetting.LOSS_MSR_FILTER_HB.Keys);
                            FreqList.Sort();

                            foreach (double dblFreq in FreqList)
                            {
                                _HP8665B.SetFrequency(dblFreq);
                                _HP8665B.SetPower(0 + TestSetting.LOSS_SRC_ROLL[dblFreq]);
                                _MXAN9020A.SetFrequency(dblFreq);
                                _MXAN9020A.SetAttenuattor(0);

                                _Util.Wait(Math.Max(intDelay_SigGen, intDelay_MXA));

                                dblResult = 0 - _MXAN9020A.Get_CW_PowerResult();
                                TestSetting.LOSS_MSR_FILTER_HB[dblFreq] = dblResult;

                                if (dblFreq < 2100)
                                    UpdateGrid(intIndex, dblFreq, dblResult, 50, 80);
                                else if (dblFreq < 4000)
                                    UpdateGrid(intIndex, dblFreq, dblResult, 30, 34);
                                else
                                    UpdateGrid(intIndex, dblFreq, dblResult, 31, 37);

                                intIndex++;
                            }
                            _HP8665B.SetOutput(Output.OFF);
                        }
                        #endregion BJ_1
                        #region SH_1 & SH3 & SH4
                        else if (Program.Location == LocationList.SH_1 ||
                                 Program.Location == LocationList.SH_3 ||
                                 Program.Location == LocationList.SH_4)
                        {
                            _E4438C.SetFrequency(500);
                            _E4438C.SetPower(0);
                            _E4438C.SetOutput(Output.ON);

                            List<double> FreqList = new List<double>(TestSetting.LOSS_MSR_FILTER_HB.Keys);
                            FreqList.Sort();

                            foreach (double dblFreq in FreqList)
                            {
                                _E4438C.SetFrequency(dblFreq);
                                _E4438C.SetPower(0 + TestSetting.LOSS_SRC_ROLL[dblFreq]);
                                _E4438C.SetFrequency(dblFreq);
                                _MXAN9020A.SetFrequency(dblFreq);
                                _MXAN9020A.SetAttenuattor(0);

                                _Util.Wait(Math.Max(intDelay_SigGen, intDelay_MXA));

                                dblResult = 0 - _MXAN9020A.Get_CW_PowerResult();
                                TestSetting.LOSS_MSR_FILTER_HB[dblFreq] = dblResult;

                                if (dblFreq < 2100)
                                    UpdateGrid(intIndex, dblFreq, dblResult, 50, 80);
                                else if (dblFreq < 4000)
                                    UpdateGrid(intIndex, dblFreq, dblResult, 30, 34);
                                else
                                    UpdateGrid(intIndex, dblFreq, dblResult, 31, 37);

                                intIndex++;
                            }
                            _E4438C.SetOutput(Output.OFF);
                        }
                        #endregion SH1
                        #region SH_2
                        else if (Program.Location == LocationList.SH_2)
                        {
                            _HP8665B.SetFrequency(500);
                            _HP8665B.SetPower(0);
                            _HP8665B.SetOutput(Output.ON);

                            List<double> FreqList = new List<double>(TestSetting.LOSS_MSR_FILTER_HB.Keys);
                            FreqList.Sort();

                            foreach (double dblFreq in FreqList)
                            {
                                _HP8665B.SetFrequency(dblFreq);
                                _HP8665B.SetPower(0 + TestSetting.LOSS_SRC_ROLL[dblFreq]);
                                _MXAN9020A.SetFrequency(dblFreq);
                                _MXAN9020A.SetAttenuattor(0);

                                _Util.Wait(Math.Max(intDelay_SigGen, intDelay_MXA));

                                dblResult = 0 - _MXAN9020A.Get_CW_PowerResult();
                                TestSetting.LOSS_MSR_FILTER_HB[dblFreq] = dblResult;

                                if (dblFreq < 2100)
                                    UpdateGrid(intIndex, dblFreq, dblResult, 50, 80);
                                else if (dblFreq < 4000)
                                    UpdateGrid(intIndex, dblFreq, dblResult, 30, 34);
                                else
                                    UpdateGrid(intIndex, dblFreq, dblResult, 31, 37);

                                intIndex++;
                            }
                            _HP8665B.SetOutput(Output.OFF);
                        }
                        #endregion SH_2
                        else
                        {
                            throw new Exception("Bad Location");
                        }
                        break;
                    }
                    #endregion *** HB Harmonic Measure cal ***
            }



            btnCal.Text = "Retry";
            btnNext.Enabled = true;
            btnCal.Enabled = true;
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            intStep++;
            if (Program.Location == LocationList.SH_1)
            {
                if (intStep == 3) intStep++;    // bypass step 3 so far
            }
            btnCal.Text = "Cal";
            btnCal.Enabled = true;
            btnNext.Enabled = false;

            #region "Cal Steps description"

            switch (intStep)
            {
                case 1:
                    {
                        lblInfo.Text = @"Step 1 of 7: Cal Power Sensor 

Connect Power Sensor to Ref port or leave it open for USB power sensor";
                        break;
                    }
                case 2:
                    {
                        lblInfo.Text = @"Step 2 of 7: Cal TX SigGen(E4438C or N5182A)) RF IN path 

Connect TX SigGen RF IN cable to power sensor";
                        break;
                    }
                case 3:
                    {
                        lblInfo.Text = @"Step 3 of 7: Cal Rolled SigGen(HP8665B) RF IN path 

Connect Roll RF IN cable to power sensor";
                        break;
                    }
                case 4:
                    {
                        lblInfo.Text = @"Step 4 of 7: Cal RF OUT path 

Connect TX SigGen(E4438C or N5182A)) RF IN and RF OUT cable use a through
And Connect the power sensor back coupler";
                        break;
                    }
                case 5:
                    {
                        lblInfo.Text = @"Step 5 of 7: Cal VSA Through path 

And connect MXA to coupler out without filter";
                        break;
                    }
                case 6:
                    {
                        if (Program.Location == LocationList.BJ_1)
                        {
                            lblInfo.Text = @"Step 6 of 7: Cal LB Harmonic path 

Connect Rolled SigGen(HP8665B) RF IN and RF OUT cable use a through
and connect MXA to coupler out with LB filter";
                        }
                        else if (Program.Location == LocationList.SH_1 ||
                                 Program.Location == LocationList.SH_3 ||
                                 Program.Location == LocationList.SH_4)
                        {
                            lblInfo.Text = @"Step 6 of 7: Cal LB Harmonic path 

Connect TX SigGen(E4438C) RF IN and RF OUT cable use a through
and connect MXA to coupler out with LB filter";
                        }
                        else if (Program.Location == LocationList.SH_2)
                        {
                            lblInfo.Text = @"Step 6 of 7: Cal LB Harmonic path 

Connect Rolled SigGen(HP8665B) RF IN and RF OUT cable use a through
and connect MXA to coupler out with LB filter";
                        }
                        else
                        {
                            throw new Exception("Bad Location");
                        }
                        break;
                    }
                case 7:
                    {
                        lblInfo.Text = @"Step 7 of 7: Cal HB Harmonic path 

Chnage LB filter to HB filter";
                        break;
                    }
                default:
                    break;
            }

            #endregion "Cal Steps description"

            dtLossComp.Clear();

            if (blnFinish)
            {
                #region Save loss comp file
                //Save cal result to txt file
                StringBuilder sbLC = new StringBuilder();
                sbLC.AppendLine(DateTime.Now.ToString());

                // Source loss
                sbLC.AppendLine("--- Source ---");
                foreach (var tmpLoss in TestSetting.LOSS_SRC)
                {
                    sbLC.AppendLine(tmpLoss.Key + "," + tmpLoss.Value);
                }
                // Power Meter Measure loss
                sbLC.AppendLine("--- Power Meter Measure ---");
                foreach (var tmpLoss in TestSetting.LOSS_MSR_POUT)
                {
                    sbLC.AppendLine(tmpLoss.Key + "," + tmpLoss.Value);
                }
                // VSA Through measure loss
                sbLC.AppendLine("--- VSA Through Measure ---");
                foreach (var tmpLoss in TestSetting.LOSS_MSR_THROUGH)
                {
                    sbLC.AppendLine(tmpLoss.Key + "," + tmpLoss.Value);
                }
                //LB Harmonic measure loss
                sbLC.AppendLine("--- VSA LB Filter Measure ---");
                foreach (var tmpLoss in TestSetting.LOSS_MSR_FILTER_LB)
                {
                    sbLC.AppendLine(tmpLoss.Key + "," + tmpLoss.Value);
                }
                //HB Harmonic measure loss
                sbLC.AppendLine("--- VSA HB Filter Measure ---");
                foreach (var tmpLoss in TestSetting.LOSS_MSR_FILTER_HB)
                {
                    sbLC.AppendLine(tmpLoss.Key + "," + tmpLoss.Value);
                }

                sbLC.AppendLine("--- The End ---");

                //Write Loss Comp file
                FileInfo file = new FileInfo(Program.strSweepCableLoss_FileName);
                if (file.Exists)
                {
                    file.Delete();
                }

                StreamWriter swLC = new StreamWriter(Program.strSweepCableLoss_FileName, true);
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

        private void dgvLossResult_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

    }
}
