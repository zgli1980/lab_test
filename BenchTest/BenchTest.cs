/// Reversion history
/// 20150306        Change according to the BJ new bench setup              Ace Li
/// 20150316        Add Mipi support for all mode                           Ace Li
/// 20150609        Add LTE FDD Mode                                        Ace Li
/// 

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
using Vanchip.Common;


namespace Bench_Test
{
    public partial class BenchTest : Form
    {
        #region *** Define Variable ***

        private static double dblMaxPout = -99;
        private static double dblRatedVramp = -99;
        private static double WCDMA_Pin = -99;
        private static double TDSCDMA_Pin = -99;
        private static double EDGE_Pin = -99;
        private static double TDD_Pin = -99;
        private static double LTE_Pin = -99;

        private static double dblVbat_Current_Limit = 3;

        private static int LB_Test_Count;
        private static bool first_run = true;
        private static double last_vcc;
        private static double last_txen;
        private static double last_vramp;
        private static double last_srcpwr = -99;
        private static double last_freq = 0;

        private static double dblPulse_Freq = 216.638; // 208.5;

        private static int intDelay_SigGen = TestSetting.DELAY_SIGGEN_in_ms;
        //private static int intDelay_SigGen_Old = TestSetting.DELAY_SIGGEN_OLD_in_ms;
        private static int intDelay_PowerMeter = TestSetting.DELAY_POWER_METER_in_ms;
        private static int intDelay_MXA = TestSetting.DELAY_VSA_in_ms;
        private static int intDelay_PowerSupply = TestSetting.DELAY_POWER_SUPPLLY_in_ms;
        private static int intDelay_Arb = TestSetting.DELAY_ARB_in_ms;
        private static int intDelay_N1913A_Count = 10;
        
        private string[] Array_HBData_Row = new string[6];
        private string[] Array_LBData_Row = new string[6];
        private string[] Array_Limit = new string[100];
        private string[] Array_Limit_Row = new string[4];


        private DataTable dtResult = new DataTable();
        private DataTable dtLBData = new DataTable();
        private DataTable dtHBData = new DataTable();
        private DataTable dtRXData = new DataTable();
        private DataTable dtWCDMAData = new DataTable();
        private DataTable dtTDSCDMAData = new DataTable();
        private DataTable dtEDGELBData = new DataTable();
        private DataTable dtEDGEHBData = new DataTable();
        private DataTable dtTDDLBData = new DataTable();
        private DataTable dtTDDHBData = new DataTable();
        private DataTable dtLTELBData = new DataTable();

        private bool isDataSaved = false;
        private bool isLBTested = false;
        private bool isHBTested = false;
        private bool isRXTested = false;
        private bool isWCDMATested = false;
        private bool isTDSCDMATested = false;
        private bool isEDGELBTested = false;
        private bool isEDGEHBTested = false;
        private bool isTDDLBTested = false;
        private bool isTDDHBTested = false;
        private bool isLTELBTested = false;
        //private bool RX_Isolation = false;
        private bool Datalog = false;

        private string strProduct;
        private string strErr;
        private bool cw_initialized = false;
        private bool wcdma_initialized = false;
        private bool tdscdma_initialized = false;
        private bool edge_initialized = false;
        private bool tdd_initialized = false;
        private bool lte_initialized = false;


        Mipi frmMipi = new Mipi();

        //private LossComp LossComp = new LossComp();
        Util util = new Util();


        E4438C _SRC_3G;
        E4438C _SRC_4G;

        Arb_33522A _Arb_Ramp;
        Arb_33522A_USB _Arb_Mipi;
        Arb_33120A _Arb_Ctrl;

        PS_66332A _PS_VCC;
        PS_66319B _PS_VBAT;
        //PS_E3631A _PS_E3631A;

        //PM_N1913A _PM_N1913A;
        //PM_U2001A _PM_U2001A;
        MXA_N9020A _MSR_3G;
        MXA_N9020A _MSR_4G;

        #endregion *** Define variable ***


        #region *** Initialize ***

        public BenchTest()
        {
            InitializeComponent();
            this.FormClosing += new FormClosingEventHandler(BenchTest_FormClosing);
            
            #region Initialize  Datatable
            
            this.dtResult.Columns.Add(new DataColumn("#", typeof(int)));
            this.dtResult.Columns.Add(new DataColumn("Test Description", typeof(string)));
            this.dtResult.Columns.Add(new DataColumn("Units", typeof(string)));
            this.dtResult.Columns.Add(new DataColumn("Result", typeof(string)));
            this.dtResult.Columns.Add(new DataColumn("LowLimit", typeof(string)));
            this.dtResult.Columns.Add(new DataColumn("UpperLimit", typeof(string)));
            this.dtResult.Columns.Add(new DataColumn("Status", typeof(string)));
            this.dtResult.PrimaryKey = new DataColumn[] { dtResult.Columns["Test Description"] };

            #endregion Initialize  Datatable

            #region Initialize GridView

            this.dgvResult.DataSource = this.dtResult;

            this.dgvResult.Columns["#"].Width = 35;
            this.dgvResult.Columns["Test Description"].Width = 177;
            this.dgvResult.Columns["Units"].Width = 60;
            this.dgvResult.Columns["Result"].Width = 90;
            this.dgvResult.Columns["LowLimit"].Width = 80;
            this.dgvResult.Columns["UpperLimit"].Width = 80;
            this.dgvResult.Columns["Status"].Width = 65;

            this.dgvResult.AllowUserToAddRows = false;
            this.dgvResult.RowHeadersVisible = false;
            this.dgvResult.ReadOnly = true;
            

            #endregion Initialize GridView

            #region Initialize instruments

            try
            {
                #region BJ_1
                if (Program.Location == LocationList.BJ_1)
                {
                    _SRC_3G = new Vanchip.Testing.E4438C(Instruments_address._19);
                    _SRC_4G = new Vanchip.Testing.E4438C(Instruments_address._20);

                    _Arb_Mipi = new Arb_33522A_USB(Instruments_VISA.Arb_33522A);
                    _Arb_Ramp = new Arb_33522A(Instruments_address._10);
                    _Arb_Ctrl = new Vanchip.Testing.Arb_33120A(Instruments_address._11);

                    _PS_VCC = new Vanchip.Testing.PS_66332A(Instruments_address._05);
                    _PS_VBAT = new Vanchip.Testing.PS_66319B(Instruments_address._06);

                    _MSR_3G = new Vanchip.Testing.MXA_N9020A(Instruments_address._18);
                    _MSR_4G = new Vanchip.Testing.MXA_N9020A(Instruments_address._17);

                    //Initialize
                    _SRC_3G.Initialize();
                    _SRC_4G.Initialize();

                    _Arb_Ctrl.Initialize();
                    _Arb_Mipi.Initialize(dblPulse_Freq);  //208.5KHz  
                    _Arb_Ramp.Initialize(dblPulse_Freq);  //208.5KHz  

                    _PS_VCC.Initialize();
                    _PS_VBAT.Initialize();

                    _MSR_3G.Initialize(true);
                    _MSR_4G.Initialize(true);
                }
                #endregion BJ_1

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

        private void BenchTest_Load(object sender, EventArgs e)
        {
            lblRev.Text= "Bench Test\r\n3.1.0.0\r\nJun/09/2015";

            #region Build Limit file

            this.Check_Band();
            this.Limit_Fill();

            #endregion Read Limit file
            
            strProduct = Program.strFilePath_Product;
            btnTest.BackColor = Color.Green;
            rbnDisplayON.Checked = true;

            //Show date time on title bar for last time perform loss comp
            this.Text = Program.tp + " Bench Test    ###Last Loss Comp date " + Program.lastCalDate;

            lblErr.Text = "Loss Comp value loaded";
        }

        #endregion *** Initialize ***


        #region *** Main Function ***

        private void btnSave_Click(object sender, EventArgs e)
        {
            this.SaveData();
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            //Check if data has been saved
            if (isWCDMATested || isLBTested || isHBTested || isEDGELBTested || isEDGEHBTested || isTDSCDMATested || isRXTested || isTDDLBTested || isTDDHBTested)
            {
                if (!isDataSaved)
                {
                    if ((MessageBox.Show("Data is not been saved yet, do you want to save the data?"
                        , "Data is not saved", MessageBoxButtons.YesNo, MessageBoxIcon.Question)) == DialogResult.Yes)
                    {
                        this.SaveData();
                    }
                }
            }

            dtResult.Clear();
            dtWCDMAData.Clear();
            dtLBData.Clear();
            dtHBData.Clear();
            dtEDGELBData.Clear();
            dtEDGEHBData.Clear();
            dtTDSCDMAData.Clear();
            dtRXData.Clear();
            dtTDDLBData.Clear();
            dtTDDHBData.Clear();
            dtLTELBData.Clear();

            isWCDMATested = false;
            isLBTested = false;
            isHBTested = false;
            isEDGELBTested = false;
            isEDGEHBTested = false;
            isTDSCDMATested = false;
            isRXTested = false;
            isTDDLBTested = false;
            isTDDHBTested = false;
            isLTELBTested = false;

            isDataSaved = false;

            this.Limit_Fill();

            this.Refresh();

        }

        private void RadioButton_CheckChanged(object sender, EventArgs e)
        {
            this.Limit_Fill();
        }

        private void BenchTest_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                #region BJ_1
                if (Program.Location == LocationList.BJ_1)
                {
                    _SRC_3G.SetOutput(Output.OFF);
                    _SRC_4G.SetOutput(Output.OFF);

                    _PS_VCC.SetOutput(Output.OFF);
                    _PS_VBAT.SetOutput(PS_66319B_Channel.Channel_1, Output.OFF);
                    _PS_VBAT.SetOutput(PS_66319B_Channel.Channel_2, Output.OFF);

                    _Arb_Mipi.SetOutput(Output.OFF, Arb_Channel.Channel_1);
                    _Arb_Mipi.SetOutput(Output.OFF, Arb_Channel.Channel_2);
                    _Arb_Ramp.SetOutput(Output.OFF, Arb_Channel.Channel_1);
                    _Arb_Ramp.SetOutput(Output.OFF, Arb_Channel.Channel_2);

                    _Arb_Ctrl.Dispose();
                    _Arb_Mipi.Dispose();
                    _Arb_Ramp.Dispose();
                    _SRC_3G.Dispose();
                    _SRC_4G.Dispose();

                    _MSR_3G.Dispose();
                    _MSR_4G.Dispose();

                    _PS_VCC.Dispose();
                    _PS_VBAT.Dispose();
                }
                #endregion BJ_1


                else
                {
                    throw new Exception("Bad Location");
                }
            }
            catch (Exception ex)
            {
                lblErr.Text = ex.Message.ToString();
            }
        }

        #endregion *** Main Function ***


        #region *** Sub Function ***

        private void Limit_Fill()
        {
            this.dtResult.Clear();

            #region Fill Limit
            #region CW_LB
            if (this.rbnLB.Checked)
            {
                if (this.isLBTested)
                {
                    foreach (DataRow drTemp in dtLBData.Rows)
                    {
                        dtResult.ImportRow(drTemp);
                    }
                }
                else
                {
                    for (int i = 1; i < TestSetting.MaxTestItem; i++)
                    {
                        if (Program.ProductTest[i].TestItem == null) break;
                        if (Program.ProductTest[i].Description.ToUpper().Contains("CW") && Program.ProductTest[i].Description.ToUpper().Contains("LB"))
                        {
                            DataRow dr = this.dtResult.NewRow();
                            dr["#"] = Program.ProductTest[i].Item;
                            dr["Test Description"] = Program.ProductTest[i].TestItem;
                            dr["Units"] = Program.ProductTest[i].Units;
                            dr["LowLimit"] = Program.ProductTest[i].LowLimit;
                            dr["UpperLimit"] = Program.ProductTest[i].HighLimit;
                            this.dtResult.Rows.Add(dr);
                        }
                    }
                }
            }
            #endregion CW_LB

            #region CW_HB
            else if (this.rbnHB.Checked)
            {
                if (this.isHBTested)
                {
                    foreach (DataRow drTemp in dtHBData.Rows)
                    {
                        dtResult.ImportRow(drTemp);
                    }
                }
                else
                {
                    for (int i = 1; i < TestSetting.MaxTestItem; i++)
                    {
                        if (Program.ProductTest[i].TestItem == null) break;

                        if (Program.ProductTest[i].Description.ToUpper().Contains("CW") && Program.ProductTest[i].Description.ToUpper().Contains("HB"))
                        {
                            DataRow dr = this.dtResult.NewRow();
                            dr["#"] = Program.ProductTest[i].Item;
                            dr["Test Description"] = Program.ProductTest[i].TestItem;
                            dr["Units"] = Program.ProductTest[i].Units;
                            dr["LowLimit"] = Program.ProductTest[i].LowLimit;
                            dr["UpperLimit"] = Program.ProductTest[i].HighLimit;
                            this.dtResult.Rows.Add(dr);
                        }
                    }
                }
            }
            #endregion CW_HB

            #region RX
            else if (this.rbnRX.Checked)
            {
                if (this.isRXTested)
                {
                    foreach (DataRow drTemp in dtRXData.Rows)
                    {
                        dtResult.ImportRow(drTemp);
                    }
                }
                else
                {
                    for (int i = 1; i < TestSetting.MaxTestItem; i++)
                    {
                        if (Program.ProductTest[i].TestItem == null) break;

                        if (Program.ProductTest[i].Description.ToUpper().Contains("RX"))
                        {
                            DataRow dr = this.dtResult.NewRow();
                            dr["#"] = Program.ProductTest[i].Item;
                            dr["Test Description"] = Program.ProductTest[i].TestItem;
                            dr["Units"] = Program.ProductTest[i].Units;
                            dr["LowLimit"] = Program.ProductTest[i].LowLimit;
                            dr["UpperLimit"] = Program.ProductTest[i].HighLimit;
                            this.dtResult.Rows.Add(dr);
                        }
                    }
                }
            }
            #endregion RX

            #region WCDMA
            else if (this.rbnWCDMA.Checked)
            {
                if (this.isWCDMATested)
                {
                    foreach (DataRow drTemp in dtWCDMAData.Rows)
                    {
                        dtResult.ImportRow(drTemp);
                    }
                }
                else
                {
                    for (int i = 1; i < TestSetting.MaxTestItem; i++)
                    {
                        if (Program.ProductTest[i].TestItem == null) break;

                        if (Program.ProductTest[i].Description.ToUpper().Contains("WCDMA"))
                        {
                            DataRow dr = this.dtResult.NewRow();
                            dr["#"] = Program.ProductTest[i].Item;
                            dr["Test Description"] = Program.ProductTest[i].TestItem;
                            dr["Units"] = Program.ProductTest[i].Units;
                            dr["LowLimit"] = Program.ProductTest[i].LowLimit;
                            dr["UpperLimit"] = Program.ProductTest[i].HighLimit;
                            this.dtResult.Rows.Add(dr);
                        }
                    }
                }
            }
            #endregion WCDMA

            #region TDSCDMA
            else if (this.rbnTD.Checked)
            {
                if (this.isTDSCDMATested)
                {
                    foreach (DataRow drTemp in dtTDSCDMAData.Rows)
                    {
                        dtResult.ImportRow(drTemp);
                    }
                }
                else
                {
                    for (int i = 1; i < TestSetting.MaxTestItem; i++)
                    {
                        if (Program.ProductTest[i].TestItem == null) break;

                        if (Program.ProductTest[i].Description.ToUpper().Contains("TDSCDMA"))
                        {
                            DataRow dr = this.dtResult.NewRow();
                            dr["#"] = Program.ProductTest[i].Item;
                            dr["Test Description"] = Program.ProductTest[i].TestItem;
                            dr["Units"] = Program.ProductTest[i].Units;
                            dr["LowLimit"] = Program.ProductTest[i].LowLimit;
                            dr["UpperLimit"] = Program.ProductTest[i].HighLimit;
                            this.dtResult.Rows.Add(dr);
                        }
                    }
                }
            }
            #endregion TDSCDMA

            #region EDGE LB
            else if (this.rbnEDGELB.Checked)
            {
                if (this.isEDGELBTested)
                {
                    foreach (DataRow drTemp in dtEDGELBData.Rows)
                    {
                        dtResult.ImportRow(drTemp);
                    }
                }
                else
                {
                    for (int i = 1; i < TestSetting.MaxTestItem; i++)
                    {
                        if (Program.ProductTest[i].TestItem == null) break;

                        if (Program.ProductTest[i].Description.ToUpper().Contains("EDGE") && Program.ProductTest[i].Description.ToUpper().Contains("LB"))
                        {
                            DataRow dr = this.dtResult.NewRow();
                            dr["#"] = Program.ProductTest[i].Item;
                            dr["Test Description"] = Program.ProductTest[i].TestItem;
                            dr["Units"] = Program.ProductTest[i].Units;
                            dr["LowLimit"] = Program.ProductTest[i].LowLimit;
                            dr["UpperLimit"] = Program.ProductTest[i].HighLimit;
                            this.dtResult.Rows.Add(dr);
                        }
                    }
                }
            }
            #endregion EDGE LB

            #region EDGE HB
            else if (this.rbnEDGEHB.Checked)
            {
                if (this.isEDGEHBTested)
                {
                    foreach (DataRow drTemp in dtEDGEHBData.Rows)
                    {
                        dtResult.ImportRow(drTemp);
                    }
                }
                else
                {
                    for (int i = 1; i < TestSetting.MaxTestItem; i++)
                    {
                        if (Program.ProductTest[i].TestItem == null) break;

                        if (Program.ProductTest[i].Description.ToUpper().Contains("EDGE") && Program.ProductTest[i].Description.ToUpper().Contains("HB"))
                        {
                            DataRow dr = this.dtResult.NewRow();
                            dr["#"] = Program.ProductTest[i].Item;
                            dr["Test Description"] = Program.ProductTest[i].TestItem;
                            dr["Units"] = Program.ProductTest[i].Units;
                            dr["LowLimit"] = Program.ProductTest[i].LowLimit;
                            dr["UpperLimit"] = Program.ProductTest[i].HighLimit;
                            this.dtResult.Rows.Add(dr);
                        }
                    }
                }
            }
            #endregion EDGE LB

            #region TDD LB
            else if (this.rbnTDDLB.Checked)
            {
                if (this.isTDDLBTested)
                {
                    foreach (DataRow drTemp in dtTDDLBData.Rows)
                    {
                        dtResult.ImportRow(drTemp);
                    }
                }
                else
                {
                    for (int i = 1; i < TestSetting.MaxTestItem; i++)
                    {
                        if (Program.ProductTest[i].TestItem == null) break;

                        if (Program.ProductTest[i].Description.ToUpper().Contains("TDD") && Program.ProductTest[i].Description.ToUpper().Contains("LB"))
                        {
                            DataRow dr = this.dtResult.NewRow();
                            dr["#"] = Program.ProductTest[i].Item;
                            dr["Test Description"] = Program.ProductTest[i].TestItem;
                            dr["Units"] = Program.ProductTest[i].Units;
                            dr["LowLimit"] = Program.ProductTest[i].LowLimit;
                            dr["UpperLimit"] = Program.ProductTest[i].HighLimit;
                            this.dtResult.Rows.Add(dr);
                        }
                    }
                }
            }
            #endregion TDD LB

            #region TDD HB
            else if (this.rbnTDDHB.Checked)
            {
                if (this.isTDDHBTested)
                {
                    foreach (DataRow drTemp in dtTDDHBData.Rows)
                    {
                        dtResult.ImportRow(drTemp);
                    }
                }
                else
                {
                    for (int i = 1; i < TestSetting.MaxTestItem; i++)
                    {
                        if (Program.ProductTest[i].TestItem == null) break;

                        if (Program.ProductTest[i].Description.ToUpper().Contains("TDD") && Program.ProductTest[i].Description.ToUpper().Contains("HB"))
                        {
                            DataRow dr = this.dtResult.NewRow();
                            dr["#"] = Program.ProductTest[i].Item;
                            dr["Test Description"] = Program.ProductTest[i].TestItem;
                            dr["Units"] = Program.ProductTest[i].Units;
                            dr["LowLimit"] = Program.ProductTest[i].LowLimit;
                            dr["UpperLimit"] = Program.ProductTest[i].HighLimit;
                            this.dtResult.Rows.Add(dr);
                        }
                    }
                }
            }
            #endregion TDD HB

            #region LTE LB
            else if (this.rbnLTELB.Checked)
            {
                if (this.isLTELBTested)
                {
                    foreach (DataRow drTemp in dtLTELBData.Rows)
                    {
                        dtResult.ImportRow(drTemp);
                    }
                }
                else
                {
                    for (int i = 1; i < TestSetting.MaxTestItem; i++)
                    {
                        if (Program.ProductTest[i].TestItem == null) break;

                        if (Program.ProductTest[i].Description.ToUpper().Contains("LTE") && Program.ProductTest[i].Description.ToUpper().Contains("LB"))
                        {
                            DataRow dr = this.dtResult.NewRow();
                            dr["#"] = Program.ProductTest[i].Item;
                            dr["Test Description"] = Program.ProductTest[i].TestItem;
                            dr["Units"] = Program.ProductTest[i].Units;
                            dr["LowLimit"] = Program.ProductTest[i].LowLimit;
                            dr["UpperLimit"] = Program.ProductTest[i].HighLimit;
                            this.dtResult.Rows.Add(dr);
                        }
                    }
                }
            }
            #endregion LTE LB

            #endregion Fill Limit
        }

        private void Check_Band()
        {
            bool isCheckSet = false;

            rbnLB.Enabled = false;
            rbnHB.Enabled = false;
            rbnEDGELB.Enabled = false;
            rbnEDGEHB.Enabled = false;
            rbnRX.Enabled = false;
            rbnTD.Enabled = false;
            rbnWCDMA.Enabled = false;
            rbnCDMA.Enabled = false;
            rbnEVDO.Enabled = false;
            rbnLTEHB.Enabled = false;
            rbnLTELB.Enabled = false;
            rbnTDDHB.Enabled = false;
            rbnTDDLB.Enabled = false;

            for (int i = 1; i < TestSetting.MaxTestItem; i++)
            {
                if (Program.ProductTest[i].TestItem == null) break;

                if (Program.ProductTest[i].Description.ToUpper().Contains("CW") && Program.ProductTest[i].Description.ToUpper().Contains("LB"))
                {
                    rbnLB.Enabled = true;
                    if (!isCheckSet)
                    {
                        rbnLB.Checked = true;
                        isCheckSet = true;
                    }
                }
                else if (Program.ProductTest[i].Description.ToUpper().Contains("CW") && Program.ProductTest[i].Description.ToUpper().Contains("HB"))
                {
                    rbnHB.Enabled = true;
                    if (!isCheckSet)
                    {
                        rbnHB.Checked = true;
                        isCheckSet = true;
                    }
                }
                else if (Program.ProductTest[i].Description.ToUpper().Contains("EDGE") && Program.ProductTest[i].Description.ToUpper().Contains("LB"))
                {
                    rbnEDGELB.Enabled = true;
                    if (!isCheckSet)
                    {
                        rbnEDGELB.Checked = true;
                        isCheckSet = true;
                    }
                }
                else if (Program.ProductTest[i].Description.ToUpper().Contains("EDGE") && Program.ProductTest[i].Description.ToUpper().Contains("HB"))
                {
                    rbnEDGEHB.Enabled = true;
                    if (!isCheckSet)
                    {
                        rbnEDGEHB.Checked = true;
                        isCheckSet = true;
                    }
                }
                else if (Program.ProductTest[i].Description.ToUpper().Contains("WCDMA"))
                {
                    rbnWCDMA.Enabled = true;
                    if (!isCheckSet)
                    {
                        rbnWCDMA.Checked = true;
                        isCheckSet = true;
                    }

                    //if (_PS_E3631A==null || !_PS_E3631A.isInitialized)
                    //{
                    //    try
                    //    {
                    //        _PS_E3631A = new PS_E3631A(Instruments_address._06);
                    //        _PS_E3631A.Initialize();
                    //    }
                    //    catch (Exception e)
                    //    {
                    //        throw new Exception("Cannot find E3661A\r\n" + e.Message);
                    //    }
                    //}
                }
                else if (Program.ProductTest[i].Description.ToUpper().Contains("TDSCDMA"))
                {
                    rbnTD.Enabled = true;
                    if (!isCheckSet)
                    {
                        rbnTD.Checked = true;
                        isCheckSet = true;
                    }
                }
                else if (Program.ProductTest[i].Description.ToUpper().Contains("TDD") && Program.ProductTest[i].Description.ToUpper().Contains("LB"))
                {
                    rbnTDDLB.Enabled = true;
                    if (!isCheckSet)
                    {
                        rbnTDDLB.Checked = true;
                        isCheckSet = true;
                    }
                }
                else if (Program.ProductTest[i].Description.ToUpper().Contains("TDD") && Program.ProductTest[i].Description.ToUpper().Contains("HB"))
                {
                    rbnTDDHB.Enabled = true;
                    if (!isCheckSet)
                    {
                        rbnTDDHB.Checked = true;
                        isCheckSet = true;
                    }
                }
                else if (Program.ProductTest[i].Description.ToUpper().Contains("LTE") && Program.ProductTest[i].Description.ToUpper().Contains("LB"))
                {
                    rbnLTELB.Enabled = true;
                    if (!isCheckSet)
                    {
                        rbnLTELB.Checked = true;
                        isCheckSet = true;
                    }
                }
                else if (Program.ProductTest[i].Description.ToUpper().Contains("LTE") && Program.ProductTest[i].Description.ToUpper().Contains("HB"))
                {
                    rbnLTEHB.Enabled = true;
                    if (!isCheckSet)
                    {
                        rbnLTEHB.Checked = true;
                        isCheckSet = true;
                    }
                }
                else if (Program.ProductTest[i].Description.ToUpper().Contains("RX"))
                {
                    rbnRX.Enabled = true;
                    if (!isCheckSet)
                    {
                        rbnRX.Checked = true;
                        isCheckSet = true;
                    }
                }
            }
        }

        private void SaveData()
        {
            if (!isLBTested && !isHBTested && !isRXTested && !isWCDMATested && isEDGELBTested && !isEDGEHBTested && !isTDSCDMATested && !isTDDLBTested && !isTDDHBTested && !isLTELBTested)
            {
                MessageBox.Show("No any data need to be saved!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Refresh();
                return;
            }
            //Save Data
            try
            {
                // Displays a SaveFileDialog so the user can save the file
                // Get full path for save data file
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();

                saveFileDialog1.Filter = "CSV File|*.csv|Text|*.txt";
                saveFileDialog1.Title = "Save as";
                //DialogResult dr = saveFileDialog1.ShowDialog();

                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    string strSaveDataPath = saveFileDialog1.FileName;
                    bool titleSaved = false;

                    StreamWriter swData = new StreamWriter(strSaveDataPath);
                    StringBuilder sbTitle = new StringBuilder();
                    //Build test data title
                    sbTitle.AppendLine(strProduct + "," + DateTime.Now.ToString());
                    sbTitle.AppendLine();
                    sbTitle.AppendLine();

                    //if (!titleSaved)
                    //{
                    //    for (int i = 0; i < dtLBData.Columns.Count; i++)
                    //    {
                    //        if (i != 0)
                    //            sbTitle.Append(',');
                    //        sbTitle.Append(dtLBData.Columns[i].ColumnName);
                    //    }
                    //    titleSaved = true;
                    //}
                    for (int i = 0; i < dgvResult.Columns.Count; i++)
                    {
                        if (i != 0)
                            sbTitle.Append(',');
                        sbTitle.Append(dgvResult.Columns[i].Name);
                    }

                    //Save test data title
                    swData.WriteLine(sbTitle.ToString());

                    //Build and save LB test data 
                    foreach (DataRow drTemp in dtLBData.Rows)
                    {
                        StringBuilder sbData = new StringBuilder();
                        for (int i = 0; i < dtLBData.Columns.Count; i++)
                        {
                            if (i != 0)
                                sbData.Append(',');
                            sbData.Append(drTemp[i].ToString());
                        }
                        swData.WriteLine(sbData.ToString());
                    }
                    //Build and save HB test data 
                    foreach (DataRow drTemp in dtHBData.Rows)
                    {
                        StringBuilder sbData = new StringBuilder();
                        for (int i = 0; i < dtHBData.Columns.Count; i++)
                        {
                            if (i != 0)
                                sbData.Append(',');
                            sbData.Append(drTemp[i].ToString());
                        }
                        swData.WriteLine(sbData.ToString());
                    }
                    //Build and save WCDMA test data 
                    foreach (DataRow drTemp in dtWCDMAData.Rows)
                    {
                        StringBuilder sbData = new StringBuilder();
                        for (int i = 0; i < dtWCDMAData.Columns.Count; i++)
                        {
                            if (i != 0)
                                sbData.Append(',');
                            sbData.Append(drTemp[i].ToString());
                        }
                        swData.WriteLine(sbData.ToString());
                    }
                    //Build and save EDGELB test data 
                    foreach (DataRow drTemp in dtEDGELBData.Rows)
                    {
                        StringBuilder sbData = new StringBuilder();
                        for (int i = 0; i < dtEDGELBData.Columns.Count; i++)
                        {
                            if (i != 0)
                                sbData.Append(',');
                            sbData.Append(drTemp[i].ToString());
                        }
                        swData.WriteLine(sbData.ToString());
                    }
                    //Build and save EDGEHB test data 
                    foreach (DataRow drTemp in dtEDGEHBData.Rows)
                    {
                        StringBuilder sbData = new StringBuilder();
                        for (int i = 0; i < dtEDGEHBData.Columns.Count; i++)
                        {
                            if (i != 0)
                                sbData.Append(',');
                            sbData.Append(drTemp[i].ToString());
                        }
                        swData.WriteLine(sbData.ToString());
                    }
                    //Build and save TDSCDMA test data 
                    foreach (DataRow drTemp in dtTDSCDMAData.Rows)
                    {
                        StringBuilder sbData = new StringBuilder();
                        for (int i = 0; i < dtTDSCDMAData.Columns.Count; i++)
                        {
                            if (i != 0)
                                sbData.Append(',');
                            sbData.Append(drTemp[i].ToString());
                        }
                        swData.WriteLine(sbData.ToString());
                    }
                    //Build and save RX test data 
                    foreach (DataRow drTemp in dtRXData.Rows)
                    {
                        StringBuilder sbData = new StringBuilder();
                        for (int i = 0; i < dtRXData.Columns.Count; i++)
                        {
                            if (i != 0)
                                sbData.Append(',');
                            sbData.Append(drTemp[i].ToString());
                        }
                        swData.WriteLine(sbData.ToString());
                    }
                    //Build and save TDD LB test data 
                    foreach (DataRow drTemp in dtTDDLBData.Rows)
                    {
                        StringBuilder sbData = new StringBuilder();
                        for (int i = 0; i < dtTDDLBData.Columns.Count; i++)
                        {
                            if (i != 0)
                                sbData.Append(',');
                            sbData.Append(drTemp[i].ToString());
                        }
                        swData.WriteLine(sbData.ToString());
                    }
                    //Build and save TDD HB test data 
                    foreach (DataRow drTemp in dtTDDHBData.Rows)
                    {
                        StringBuilder sbData = new StringBuilder();
                        for (int i = 0; i < dtTDDHBData.Columns.Count; i++)
                        {
                            if (i != 0)
                                sbData.Append(',');
                            sbData.Append(drTemp[i].ToString());
                        }
                        swData.WriteLine(sbData.ToString());
                    }
                    //Build and save LTE LB test data 
                    foreach (DataRow drTemp in dtLTELBData.Rows)
                    {
                        StringBuilder sbData = new StringBuilder();
                        for (int i = 0; i < dtLTELBData.Columns.Count; i++)
                        {
                            if (i != 0)
                                sbData.Append(',');
                            sbData.Append(drTemp[i].ToString());
                        }
                        swData.WriteLine(sbData.ToString());
                    }


                    swData.Close();
                    isDataSaved = true;
                    MessageBox.Show("Data have been saved to " + strSaveDataPath);
                    this.Refresh();
                }
            }
            catch (Exception ex)
            {
                lblErr.Text = ex.Message.ToString();
            }
        }
        
        private void UpdateGrid(string strTest_Descriotion, double dblResult)
        {
            //Update grid data
            DataRow drTemp;
            drTemp = dtResult.Rows.Find(strTest_Descriotion);

            if (drTemp != null)
            {
                drTemp.BeginEdit();
                drTemp["Result"] = dblResult;
                if (dblResult > Convert.ToDouble(drTemp["LowLimit"]) && dblResult < Convert.ToDouble(drTemp["UpperLimit"]))
                    drTemp["Status"] = "Pass";
                else
                    drTemp["Status"] = "Fail";

                drTemp.EndEdit();
            }
            else
            {
                throw new Exception(strTest_Descriotion.ToString() + " is not find");
            }
            dgvResult.Refresh();
        }

        private void UpdateProductionTestResult(string strTest_Descriotion, double dblResult)
        {
            int ItemCount = Program.ProductTest.Count();
            for (int i = 1; i < ItemCount; i++)
            {
                if (Program.ProductTest[i].TestItem == null) break;
                if (Program.ProductTest[i].TestItem == strTest_Descriotion)
                {
                    Program.ProductTest[i].Result = dblResult;
                }
            }
        }

        private void gvResult_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (this.dgvResult.Columns[e.ColumnIndex].Name == "Status")
            {
                if (e.Value != null)
                {
                    // Check for the string "Fail" in the cell.
                    string stringValue = Convert.ToString(e.Value);
                    stringValue = stringValue.ToLower();
                    if (stringValue == "fail")
                    {
                        //e.CellStyle.BackColor = Color.Pink;
                        this.dgvResult.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Red;
                    }
                    else if (stringValue == "pass")
                    {
                        this.dgvResult.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Green;
                    }

                }
            }

        }

        private int Set_CW_Condition(ProductTest TestCondition)
        {
            int delaytime = 0;

            if (first_run)
            {
                SetVCC(Convert.ToDouble(TestCondition.VCC));
                SetVramp(Convert.ToDouble(TestCondition.Vramp));
                SetTXEnable(Convert.ToDouble(TestCondition.Txen_Ven));

                util.Wait(intDelay_PowerSupply);
                first_run = false;
            }
            else
            {
                if (Convert.ToDouble(TestCondition.VCC) != last_vcc)
                {
                    SetVCC(Convert.ToDouble(TestCondition.VCC));
                    delaytime = intDelay_PowerSupply;
                }

                if (Convert.ToDouble(TestCondition.Vramp) != last_vramp)
                {
                    SetVramp(Convert.ToDouble(TestCondition.Vramp));
                    if (intDelay_Arb > delaytime)
                        delaytime = intDelay_Arb;
                }

                if (Convert.ToDouble(TestCondition.Txen_Ven) != last_txen)
                {
                    SetTXEnable(Convert.ToDouble(TestCondition.Txen_Ven));
                    if (intDelay_Arb > delaytime)
                        delaytime = intDelay_Arb;
                }
            }
            last_vcc = Convert.ToDouble(TestCondition.VCC);
            last_txen = Convert.ToDouble(TestCondition.Txen_Ven);
            last_vramp = Convert.ToDouble(TestCondition.Vramp);

            return delaytime;
        }
        private void Set_EDGE_Condition(ProductTest TestCondition)
        {
            SetVCC(Convert.ToDouble(TestCondition.VCC));
            SetVbat(Convert.ToDouble(TestCondition.VBAT), 1.0);

            SetVEN_NEW(Arb_Channel.Channel_1, dblPulse_Freq, 25, 2, 0, TestCondition.Txen_Ven, false);
            SetVEN(Arb_Channel.Channel_2, TestCondition.Vramp, true);

            util.Wait(intDelay_PowerSupply);

            //if (is_Mipi) frmMipi.ShowDialog();


        }
        private void Set_WCDMA_Condition(ProductTest TestCondition)
        {
            SetVCC(Convert.ToDouble(TestCondition.VCC));
            SetVbat(Convert.ToDouble(TestCondition.VBAT), 1.0);

            SetVEN(Arb_Channel.Channel_1, TestCondition.Txen_Ven, true);
            SetVEN(Arb_Channel.Channel_2, TestCondition.Vramp, true);

            util.Wait(intDelay_PowerSupply);

        }
        private void Set_TD_Condition(ProductTest TestCondition)
        {
            SetVCC(Convert.ToDouble(TestCondition.VCC));
            SetVbat(Convert.ToDouble(TestCondition.VBAT), 1.0);

            SetVEN(Arb_Channel.Channel_1, TestCondition.Txen_Ven, true);
            SetVEN(Arb_Channel.Channel_2, TestCondition.Vramp, true);

            util.Wait(intDelay_PowerSupply);

            //if (is_Mipi) frmMipi.ShowDialog();
            

        }

        private void SetVCC(double dblValue_in_Volts)
        {
            //Enbale Power Supply
            _PS_VCC.SetCurrentRange(dblVbat_Current_Limit);
            _PS_VCC.SetVoltage(dblValue_in_Volts);
            _PS_VCC.SetOutput(Output.ON);
        }
        private void SetVbat(double Voltage_in_Volts, double Current_in_Amps)
        {
            //Enbale Power Supply
            _PS_VBAT.SetVoltage(PS_66319B_Channel.Channel_1, Voltage_in_Volts);
            _PS_VBAT.SetCurrentRange(PS_66319B_Channel.Channel_1, Current_in_Amps);

            if (Voltage_in_Volts > 0.2)
            {
                _PS_VBAT.SetOutput(PS_66319B_Channel.Channel_1, Output.ON);
            }
            else
            {
                _PS_VBAT.SetOutput(PS_66319B_Channel.Channel_1, Output.OFF);
            }

        }
        private void SetVramp(double dblVrampValue_in_Volts)
        {
            double dblDCOffset = 0;// dblVrampValue_in_Volts / 2;
            try
            {
                #region BJ_1
                if (Program.Location == LocationList.BJ_1)
                {
                    _Arb_Ramp.SetArbOut(Arb_Waveform_Type.Pulse, Arb_Channel.Channel_2, dblPulse_Freq, dblVrampValue_in_Volts, dblDCOffset, 25);
                }
                #endregion BJ_1

                else
                {
                    throw new Exception("Bad Location");
                }
            }
            catch (Exception ex)
            {
                lblErr.Text = ex.Message.ToString();
            }
        }
        private void SetTXEnable(double dblTxEnableValue_in_Volts)
        {
            double dblDCOffset = 0;// dblTxEnableValue_in_Volts / 2;
            try
            {
                #region BJ_1
                if (Program.Location == LocationList.BJ_1)
                {
                    _Arb_Ramp.SetArbOut(Arb_Waveform_Type.Pulse, Arb_Channel.Channel_1, dblPulse_Freq, dblTxEnableValue_in_Volts, dblDCOffset, 26);
                }
                #endregion BJ_1

                else
                {
                    throw new Exception("Bad Location");
                }
            }
            catch (Exception ex)
            {
                lblErr.Text = ex.Message.ToString();
            }

        }
        private void SetVEN(Arb_Channel Channel, double dblValue_in_Volts, bool isDC)
        {
            //double dblDCOffset = dblTxEnableValue_in_Volts / 2;
            double dblDCOffset = 0;
            try
            {
                #region BJ_1
                if (Program.Location == LocationList.BJ_1)
                {

                    if (isDC)
                    {
                        util.Wait(10);
                        _Arb_Ramp.SetDC(Channel, dblValue_in_Volts);
                        util.Wait(10);
                    }
                    else
                    {
                        _Arb_Ramp.SetArbOut(Arb_Waveform_Type.Pulse, Arb_Channel.Channel_1, 200, dblValue_in_Volts, dblDCOffset, 25, false);
                        util.Wait(10);
                        _Arb_Ramp.SetBurstTrig(Arb_Channel.Channel_1, 0, 2);
                        util.Wait(20);
                    }
                }
                #endregion BJ_1

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }

        }
        private void SetVEN_NEW(Arb_Channel Channel, double dblFreq_in_Hz, int intDutyCycle, int intBurstCount, double dblTrigDelay_in_us, double dblValue_in_Volts, bool isDC)
        {
            //double dblDCOffset = dblTxEnableValue_in_Volts / 2;
            double dblDCOffset = 0;
            try
            {
                #region BJ_1 
                if (Program.Location == LocationList.BJ_1)
                {

                    if (isDC)
                    {
                        util.Wait(10);
                        _Arb_Ramp.SetDC(Channel, dblValue_in_Volts);
                        util.Wait(10);
                    }
                    else
                    {
                        _Arb_Ramp.SetArbOut(Arb_Waveform_Type.Pulse, Arb_Channel.Channel_1, dblFreq_in_Hz, dblValue_in_Volts, dblDCOffset, intDutyCycle, false);
                        util.Wait(10);
                        _Arb_Ramp.SetBurstTrig(Arb_Channel.Channel_1, dblTrigDelay_in_us, intBurstCount);
                        util.Wait(20);
                    }
                }
                #endregion BJ_1

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }

        }


        private void chxDataFormat_CheckedChanged(object sender, EventArgs e)
        {
            if (chxDataFormat.Checked)
            {
                Datalog frmDatalog = new Bench_Test.Datalog();
                frmDatalog.ShowDialog();

                Datalog = true;
            }
            else
            {
                Datalog = false;
            }
        }

        private void rbnVSAON_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (rbnDisplayON.Checked)
                {
                    _MSR_3G.Display_Enable(true);
                    _MSR_4G.Display_Enable(true);
                    intDelay_N1913A_Count = 10;
                }
                else
                {
                    _MSR_3G.Display_Enable(false);
                    _MSR_4G.Display_Enable(false);
                    intDelay_N1913A_Count = 5;
                }
            }
            catch
            { }
        }

        private void lblDataTool_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            DataMergeTool frmDataTool = new DataMergeTool();
            frmDataTool.Visible = true;
        }

        #endregion *** Sub Function ***

        private void btnTest_Click(object sender, EventArgs e)
        {
            #region *** Variable Define ***
            isDataSaved = false;

            btnTest.Text = "Testing";
            btnTest.BackColor = Color.Yellow;
            btnTest.Enabled = false;

            #endregion *** Variable Define ***

            if (rbnLB.Checked)
            {
                #region *** Clear Display ***
                dtResult.Clear();
                dtLBData.Clear();

                isLBTested = false;
                isDataSaved = false;

                this.Limit_Fill();
                #endregion *** Clear Display ***

                #region *** Initialize
                wcdma_initialized = false;
                tdscdma_initialized = false;
                edge_initialized = false;
                tdd_initialized = false;
                lte_initialized = false;

                if (!cw_initialized)
                {
                    _SRC_3G.SetModOutput(Output.OFF);
                    _MSR_3G.Initialize(!rbnVSAOFF.Checked);

                    util.Wait(2000);
                    cw_initialized = true;
                }
                #endregion *** Initialize

                #region *** MessageBox ***
                int index = 0;
                string msgString = "";

                foreach (ProductTest Test in Program.ProductTest)
                {
                    if (Test.Description != null)
                    {
                        if (Test.Description.ToUpper().Contains("CW") && Test.Description.ToUpper().Contains("LB"))
                        {
                            index = Array.IndexOf(Program.ProductTest, Test);
                            break;
                        }
                    }
                }
                if (cbxMIPI.Checked)
                {
                    SetVCC(Program.ProductTest[1].VCC);
                    msgString = "Make sure everything is setup for GMSK LB testing\r\n" +
                                "   1) Connect tx source cable to TX RF IN(SRC_3G)\r\n" +
                                "   2) Set Mipi control to GMSK LB mode\r\n" +
                                "   3) Disconnect highpass filter";

                    MessageBox.Show(msgString, "GMSK LB Testing", MessageBoxButtons.OK);
                    this.Refresh();
                    frmMipi.ShowDialog();
                    util.Wait(1000);
                }
                else
                {
                    msgString = "Make sure everything is setup for GMSK LB testing\r\n" +
                                "   1) Connect tx source cable to TX RF IN(SRC_3G)\r\n" +
                                "   2) Set Control box to GMSK LB mode\r\n" +
                                "       GPCtrl0 : " + Program.ProductTest[index].Gpctrl0_Vmode0 + "\r\n" +
                                "       GPCtrl1 : " + Program.ProductTest[index].Gpctrl1_Vmode1 + "\r\n" +
                                "       GPCtrl2 : " + Program.ProductTest[index].Gpctrl2_Vmode2 + "\r\n" +
                                "   3) Connect / Change LB highpass filter";

                    MessageBox.Show(msgString, "GMSK LB Testing", MessageBoxButtons.OK);
                }
                this.Refresh();

                #endregion MessageBox

                # region *** CW LB Electronic Test ***
                //
                //
                for (int i = 1; i < TestSetting.MaxTestItem; i++)
                {
                    if (Program.ProductTest[i].TestItem == null) break;
                    if (Program.ProductTest[i].Description.ToUpper().Contains("CW"))
                    {
                        if (Program.ProductTest[i].Description.ToUpper().Contains("LB"))
                        {
                            DeviceTest_CW(Program.ProductTest[i]);
                        }
                    }
                }
                #endregion CW LB Electronic Test

                #region *** Transfer Test result to datatable ***

                dtLBData.Clear();
                dtLBData = dtResult.Clone();
                foreach (DataRow drTemp in dtResult.Rows)
                {
                    dtLBData.ImportRow(drTemp);
                }
                isLBTested = true;

                #endregion Transfer Test result to datatable

            }
            else if (rbnHB.Checked)
            {
                #region *** Clear Display ***
                dtResult.Clear();
                dtHBData.Clear();

                isHBTested = false;
                isDataSaved = false;

                this.Limit_Fill();
                #endregion *** Clear Display ***

                #region *** Initialize
                wcdma_initialized = false;
                tdscdma_initialized = false;
                edge_initialized = false;
                tdd_initialized = false;
                lte_initialized = false;

                if (!cw_initialized)
                {
                    _SRC_3G.SetModOutput(Output.OFF);
                    _MSR_3G.Initialize(!rbnVSAOFF.Checked);

                    util.Wait(2000);
                    cw_initialized = true;
                }
                #endregion *** Initialize

                #region *** MessageBox ***
                int index = 0;
                string msgString = "";

                foreach (ProductTest Test in Program.ProductTest)
                {
                    if (Test.Description != null)
                    {
                        if (Test.Description.ToUpper().Contains("CW") && Test.Description.ToUpper().Contains("HB"))
                        {
                            index = Array.IndexOf(Program.ProductTest, Test);
                            break;
                        }
                    }
                }
                if (cbxMIPI.Checked)
                {
                    SetVCC(Program.ProductTest[1].VCC);
                    msgString = "Make sure everything is setup for GMSK HB testing\r\n" +
                                "   1) Connect tx source cable to TX RF IN(SRC_3G)\r\n" +
                                "   2) Set Mipi control to GMSK HB mode\r\n" +
                                "   3) Disconnect highpass filter";

                    MessageBox.Show(msgString, "GMSK HB Testing", MessageBoxButtons.OK);
                    this.Refresh();
                    frmMipi.ShowDialog();
                    util.Wait(1000);
                }
                else
                {
                    msgString = "Make sure everything is setup for GMSK HB testing\r\n" +
                                "   1) Connect tx source cable to TX RF IN(SRC_3G)\r\n" +
                                "   2) Set Control box to GMSK HB mode\r\n" +
                                "       GPCtrl0 : " + Program.ProductTest[index].Gpctrl0_Vmode0 + "\r\n" +
                                "       GPCtrl1 : " + Program.ProductTest[index].Gpctrl1_Vmode1 + "\r\n" +
                                "       GPCtrl2 : " + Program.ProductTest[index].Gpctrl2_Vmode2 + "\r\n" +
                                "   3) Connect / Change HB highpass filter";

                    MessageBox.Show(msgString, "GMSK HB Testing", MessageBoxButtons.OK);
                }
                this.Refresh();

                #endregion MessageBox

                # region *** CW HB Electronic Test ***
                //
                //
                for (int i = 1; i < TestSetting.MaxTestItem; i++)
                {
                    if (Program.ProductTest[i].TestItem == null) break;
                    if (Program.ProductTest[i].Description.ToUpper().Contains("CW"))
                    {
                        if (Program.ProductTest[i].Description.ToUpper().Contains("HB"))
                        {
                            DeviceTest_CW(Program.ProductTest[i]);
                        }
                    }
                }
                #endregion CW HB Electronic Test

                #region *** Transfer Test result to datatable ***

                dtHBData.Clear();
                dtHBData = dtResult.Clone();
                foreach (DataRow drTemp in dtResult.Rows)
                {
                    dtHBData.ImportRow(drTemp);
                }
                isHBTested = true;

                #endregion Transfer Test result to datatable

            }
            else if (rbnRX.Checked)
            {
                #region *** Clear Display ***
                dtResult.Clear();
                dtRXData.Clear();

                isRXTested = false;
                isDataSaved = false;

                this.Limit_Fill();
                #endregion *** Clear Display ***

                #region *** Initialize
                wcdma_initialized = false;
                tdscdma_initialized = false;
                edge_initialized = false;
                tdd_initialized = false;
                lte_initialized = false;

                if (!cw_initialized)
                {
                    _SRC_3G.SetModOutput(Output.OFF);
                    _MSR_3G.Initialize(!rbnVSAOFF.Checked);

                    util.Wait(2000);
                    cw_initialized = true;
                }
                #endregion *** Initialize

                #region *** MessageBox ***
                //                string msgString = "";
                //                msgString = @"Make sure everything is setup for LB testing
                //    1) Connect tx source cable to TX HB
                //    2) Set Control box to HB mode
                //    3) Connect / Change highpass filter";

                //                MessageBox.Show(msgString, "HB Testing", MessageBoxButtons.OK);
                //                this.Refresh();

                #endregion MessageBox

                # region *** RX Electronic Test ***
                //
                //
                for (int i = 1; i < TestSetting.MaxTestItem; i++)
                {
                    if (Program.ProductTest[i].TestItem == null) break;
                    if (Program.ProductTest[i].Description.ToUpper().Contains("RX"))
                    {
                        DeviceTest_CW(Program.ProductTest[i]);
                    }
                }
                #endregion RX Electronic Test

                #region *** Transfer Test result to datatable ***

                dtRXData.Clear();
                dtRXData = dtResult.Clone();
                foreach (DataRow drTemp in dtResult.Rows)
                {
                    dtRXData.ImportRow(drTemp);
                }
                isRXTested = true;

                #endregion Transfer Test result to datatable

            }
            else if (rbnWCDMA.Checked)
            {
                #region *** Clear Display ***
                dtResult.Clear();
                dtWCDMAData.Clear();

                isWCDMATested = false;
                isDataSaved = false;

                this.Limit_Fill();
                #endregion *** Clear Display ***

                #region *** Initialize
                cw_initialized = false;
                tdscdma_initialized = false;
                edge_initialized = false;
                tdd_initialized = false;
                lte_initialized = false;
                if (!wcdma_initialized)
                {
                    btnTest.Text = "Initializing";
                    btnTest.Refresh();

                    _SRC_3G.Mode_Initialize(Modulation.WCDMA);
                    _MSR_3G.Mod_Initialize(Modulation.WCDMA);
                    //_MXA_N9020A.Initialize(rbnDisplayON.Checked);
                    util.Wait(2000);
                    wcdma_initialized = true;

                    btnTest.Text = "Testing";
                    btnTest.Refresh();
                }
                #endregion *** Initialize

                #region *** MessageBox ***
                int index = 0;
                string msgString = "";

                foreach (ProductTest Test in Program.ProductTest)
                {
                    if (Test.Description != null)
                    {
                        if (Test.Description.ToUpper().Contains("WCDMA"))
                        {
                            index = Array.IndexOf(Program.ProductTest, Test);
                            break;
                        }
                    }
                }
                if (cbxMIPI.Checked)
                {
                    SetVCC(Program.ProductTest[1].VCC);
                    msgString = "Make sure everything is setup for WCDMA testing\r\n" +
                                "   1) Connect tx source cable to TX RF IN(SRC_3G)\r\n" +
                                "   2) Set Mipi control to WCDMA mode\r\n" +
                                "   3) Disconnect highpass filter";

                    MessageBox.Show(msgString, "WCDMA Testing", MessageBoxButtons.OK);
                    this.Refresh();
                    frmMipi.ShowDialog();
                    util.Wait(1000);
                }
                else
                {
                    msgString = "Make sure everything is setup for WCDMA testing\r\n" +
                                "   1) Connect tx source cable to TX RF IN(SRC_3G)\r\n" +
                                "   2) Set Control box to WCDMA mode\r\n" +
                                "       GPCtrl0 : " + Program.ProductTest[index].Gpctrl0_Vmode0 + "\r\n" +
                                "       GPCtrl1 : " + Program.ProductTest[index].Gpctrl1_Vmode1 + "\r\n" +
                                "       GPCtrl2 : " + Program.ProductTest[index].Gpctrl2_Vmode2 + "\r\n" +
                                "   3) Disconnect highpass filter";

                    MessageBox.Show(msgString, "WCDMA Testing", MessageBoxButtons.OK);
                }
                this.Refresh();

                #endregion MessageBox

                # region *** WCDMA Electronic Test ***
                //
                for (int i = 1; i < TestSetting.MaxTestItem; i++)
                {
                    if (Program.ProductTest[i].TestItem == null) break;

                    if (Program.ProductTest[i].Description.ToUpper().Contains("WCDMA"))
                    {
                        DeviceTest_WCDMA(Program.ProductTest[i]);
                    }
                }
                #endregion WCDMA Electronic Test

                #region *** Transfer Test result to datatable ***

                dtWCDMAData.Clear();
                dtWCDMAData = dtResult.Clone();
                foreach (DataRow drTemp in dtResult.Rows)
                {
                    dtWCDMAData.ImportRow(drTemp);
                }
                isWCDMATested = true;

                #endregion Transfer Test result to datatable

            }
            else if (rbnTD.Checked)
            {
                #region *** Clear Display ***
                dtResult.Clear();
                dtTDSCDMAData.Clear();

                isTDSCDMATested = false;
                isDataSaved = false;

                this.Limit_Fill();
                #endregion *** Clear Display ***

                #region *** Initialize
                cw_initialized = false;
                wcdma_initialized = false;
                edge_initialized = false;
                tdd_initialized = false;
                lte_initialized = false;
                if (!tdscdma_initialized)
                {
                    btnTest.Text = "Initializing";
                    btnTest.Refresh();

                    _SRC_3G.Mode_Initialize(Modulation.TDSCDMA);
                    _SRC_3G.SetArbTrig(Triger_Type.Continous_Reset, Triger_Source.Bus, 0);
                    _SRC_3G.TrigerBus();

                    _MSR_3G.Mod_Initialize(Modulation.TDSCDMA);
                    util.Wait(2000);
                    tdscdma_initialized = true;

                    btnTest.Text = "Testing";
                    btnTest.Refresh();
                }
                #endregion *** Initialize

                #region *** MessageBox ***
                int index = 0;
                string msgString = "";

                foreach (ProductTest Test in Program.ProductTest)
                {
                    if (Test.Description != null)
                    {
                        if (Test.Description.ToUpper().Contains("TDSCDMA"))
                        {
                            index = Array.IndexOf(Program.ProductTest, Test);
                            break;
                        }
                    }
                }
                if (cbxMIPI.Checked)
                {
                    SetVCC(Program.ProductTest[1].VCC);
                    msgString = "Make sure everything is setup for TDSCDMA testing\r\n" +
                                "   1) Connect tx source cable to TX RF IN(SRC_3G)\r\n" +
                                "   2) Set Mipi control to TDSCDMA mode\r\n" +
                                "   3) Disconnect highpass filter";

                    MessageBox.Show(msgString, "TDSCDMA Testing", MessageBoxButtons.OK);
                    this.Refresh();
                    frmMipi.ShowDialog();
                    util.Wait(1000);
                }
                else
                {
                    msgString = "Make sure everything is setup for TDSCDMA testing\r\n" +
                                "   1) Connect tx source cable to TX RF IN(SRC_3G)\r\n" +
                                "   2) Set Control box to TDSCDMA mode\r\n" +
                                "       GPCtrl0 : " + Program.ProductTest[index].Gpctrl0_Vmode0 + "\r\n" +
                                "       GPCtrl1 : " + Program.ProductTest[index].Gpctrl1_Vmode1 + "\r\n" +
                                "       GPCtrl2 : " + Program.ProductTest[index].Gpctrl2_Vmode2 + "\r\n" +
                                "   3) Disconnect highpass filter";

                    MessageBox.Show(msgString, "TDSCDMA Testing", MessageBoxButtons.OK);
                }
                this.Refresh();

                #endregion MessageBox

                # region *** TDSCDMA Electronic Test ***
                //
                for (int i = 1; i < TestSetting.MaxTestItem; i++)
                {
                    if (Program.ProductTest[i].TestItem == null) break;

                    if (Program.ProductTest[i].Description.ToUpper().Contains("TDSCDMA"))
                    {
                        DeviceTest_TDSCDMA(Program.ProductTest[i]);
                    }
                }
                #endregion TDSCDMA Electronic Test

                #region *** Transfer Test result to datatable ***

                dtTDSCDMAData.Clear();
                dtTDSCDMAData = dtResult.Clone();
                foreach (DataRow drTemp in dtResult.Rows)
                {
                    dtTDSCDMAData.ImportRow(drTemp);
                }
                isTDSCDMATested = true;

                #endregion Transfer Test result to datatable

            }
            else if (rbnEDGELB.Checked)
            {
                #region *** Clear Display ***
                dtResult.Clear();
                dtEDGELBData.Clear();

                isEDGELBTested = false;
                isDataSaved = false;

                this.Limit_Fill();
                #endregion *** Clear Display ***

                #region *** Initialize
                cw_initialized = false;
                wcdma_initialized = false;
                tdscdma_initialized = false;
                tdd_initialized = false;
                lte_initialized = false;
                if (!edge_initialized)
                {
                    btnTest.Text = "Initializing";
                    btnTest.Refresh();

                    _SRC_3G.Mode_Initialize(Modulation.EDGE, Mod_Waveform_Name.EDGE_CONTINOUS);
                    _SRC_3G.SetArbTrig(Triger_Type.Continous_Reset, Triger_Source.Bus, 0);
                    _SRC_3G.TrigerBus();

                    _MSR_3G.Mod_Initialize(Modulation.EDGE_CONTINOUS);
                    util.Wait(1000);
                    edge_initialized = true;

                    btnTest.Text = "Testing";
                    btnTest.Refresh();
                }
                #endregion *** Initialize

                #region *** MessageBox ***
                int index = 0;
                string msgString = "";

                foreach (ProductTest Test in Program.ProductTest)
                {
                    if (Test.Description != null)
                    {
                        if (Test.Description.ToUpper().Contains("EDGE") && Test.Description.ToUpper().Contains("LB"))
                        {
                            index = Array.IndexOf(Program.ProductTest, Test);
                            break;
                        }
                    }
                }
                if (cbxMIPI.Checked)
                {
                    SetVCC(Program.ProductTest[1].VCC);
                    msgString = "Make sure everything is setup for EDGE LB testing\r\n" +
                                "   1) Connect tx source cable to TX RF IN(SRC_3G)\r\n" +
                                "   2) Set Mipi control to EDGE LB mode\r\n" +
                                "   3) Disconnect highpass filter";

                    MessageBox.Show(msgString, "EDGE LB Testing", MessageBoxButtons.OK);
                    this.Refresh();
                    frmMipi.ShowDialog();
                    util.Wait(1000);
                }
                else
                {
                    msgString = "Make sure everything is setup for EDGE LB testing\r\n" +
                                "   1) Connect tx source cable to TX RF IN(SRC_3G)\r\n" +
                                "   2) Set Control box to EDGE LB mode\r\n" +
                                "       GPCtrl0 : " + Program.ProductTest[index].Gpctrl0_Vmode0 + "\r\n" +
                                "       GPCtrl1 : " + Program.ProductTest[index].Gpctrl1_Vmode1 + "\r\n" +
                                "       GPCtrl2 : " + Program.ProductTest[index].Gpctrl2_Vmode2 + "\r\n" +
                                "   3) Disconnect highpass filter";

                    MessageBox.Show(msgString, "EDGE LB Testing", MessageBoxButtons.OK);
                }
                this.Refresh();

                #endregion MessageBox

                # region *** EDGE LB Electronic Test ***
                //
                for (int i = 1; i < TestSetting.MaxTestItem; i++)
                {
                    if (Program.ProductTest[i].TestItem == null) break;

                    if (Program.ProductTest[i].Description.ToUpper().Contains("EDGE") && Program.ProductTest[i].Description.ToUpper().Contains("LB"))
                    {
                        DeviceTest_EDGE(Program.ProductTest[i]);
                    }
                }
                #endregion EDGE LB Electronic Test

                #region *** Transfer Test result to datatable ***

                dtEDGELBData.Clear();
                dtEDGELBData = dtResult.Clone();
                foreach (DataRow drTemp in dtResult.Rows)
                {
                    dtEDGELBData.ImportRow(drTemp);
                }
                isEDGELBTested = true;

                #endregion Transfer Test result to datatable

            }
            else if (rbnEDGEHB.Checked)
            {
                #region *** Clear Display ***
                dtResult.Clear();
                dtEDGEHBData.Clear();

                isEDGEHBTested = false;
                isDataSaved = false;

                this.Limit_Fill();
                #endregion *** Clear Display ***

                #region *** Initialize
                cw_initialized = false;
                wcdma_initialized = false;
                tdscdma_initialized = false;
                tdd_initialized = false;
                lte_initialized = false;
                if (!edge_initialized)
                {
                    btnTest.Text = "Initializing";
                    btnTest.Refresh();

                    _SRC_3G.Mode_Initialize(Modulation.EDGE, Mod_Waveform_Name.EDGE_CONTINOUS);
                    _SRC_3G.SetArbTrig(Triger_Type.Continous_Reset, Triger_Source.Bus, 0);
                    _SRC_3G.TrigerBus();

                    _MSR_3G.Mod_Initialize(Modulation.EDGE);
                    util.Wait(1000);
                    //_E4438C.SetOutput(Output.OFF);
                    edge_initialized = true;

                    btnTest.Text = "Testing";
                    btnTest.Refresh();
                }
                #endregion *** Initialize

                #region *** MessageBox ***
                int index = 0;
                string msgString = "";

                foreach (ProductTest Test in Program.ProductTest)
                {
                    if (Test.Description != null)
                    {
                        if (Test.Description.ToUpper().Contains("EDGE") && Test.Description.ToUpper().Contains("HB"))
                        {
                            index = Array.IndexOf(Program.ProductTest, Test);
                            break;
                        }
                    }
                }
                if (cbxMIPI.Checked)
                {
                    SetVCC(Program.ProductTest[1].VCC);
                    msgString = "Make sure everything is setup for EDGE HB testing\r\n" +
                                "   1) Connect tx source cable to TX RF IN(SRC_3G)\r\n" +
                                "   2) Set Mipi control to EDGE HB mode\r\n" +
                                "   3) Disconnect highpass filter";

                    MessageBox.Show(msgString, "EDGE HB Testing", MessageBoxButtons.OK);
                    this.Refresh();
                    frmMipi.ShowDialog();
                    util.Wait(1000);
                }
                else
                {
                    msgString = "Make sure everything is setup for EDGE HB testing\r\n" +
                                "   1) Connect tx source cable to TX RF IN(SRC_3G)\r\n" +
                                "   2) Set Control box to EDGE HB mode\r\n" +
                                "       GPCtrl0 : " + Program.ProductTest[index].Gpctrl0_Vmode0 + "\r\n" +
                                "       GPCtrl1 : " + Program.ProductTest[index].Gpctrl1_Vmode1 + "\r\n" +
                                "       GPCtrl2 : " + Program.ProductTest[index].Gpctrl2_Vmode2 + "\r\n" +
                                "   3) Disconnect highpass filter";

                    MessageBox.Show(msgString, "EDGE HB Testing", MessageBoxButtons.OK);
                }
                this.Refresh();

                #endregion MessageBox

                # region *** EDGE HB Electronic Test ***
                //
                for (int i = 1; i < TestSetting.MaxTestItem; i++)
                {
                    if (Program.ProductTest[i].TestItem == null) break;

                    if (Program.ProductTest[i].Description.ToUpper().Contains("EDGE") && Program.ProductTest[i].Description.ToUpper().Contains("HB"))
                    {
                        DeviceTest_EDGE(Program.ProductTest[i]);
                    }
                }
                #endregion EDGE HB Electronic Test

                #region *** Transfer Test result to datatable ***

                dtEDGEHBData.Clear();
                dtEDGEHBData = dtResult.Clone();
                foreach (DataRow drTemp in dtResult.Rows)
                {
                    dtEDGEHBData.ImportRow(drTemp);
                }
                isEDGEHBTested = true;

                #endregion Transfer Test result to datatable

            }
            else if (rbnTDDLB.Checked)
            {
                #region *** Clear Display ***
                dtResult.Clear();
                dtTDDLBData.Clear();

                isTDDLBTested = false;
                isDataSaved = false;

                this.Limit_Fill();
                #endregion *** Clear Display ***

                #region *** Initialize
                cw_initialized = false;
                wcdma_initialized = false;
                tdscdma_initialized = false;
                edge_initialized = false;
                lte_initialized = false;
                if (!tdd_initialized)
                {
                    btnTest.Text = "Initializing";
                    btnTest.Refresh();

                    _SRC_4G.Mode_Initialize(Modulation.LTETDD, false);
                    _SRC_4G.SetArbTrig(Triger_Type.Continous_Reset, Triger_Source.Bus, 0); // Set Trigger to Free Run
                    _SRC_4G.TrigerBus();
                    _MSR_4G.Mod_Initialize(Modulation.LTETDD, false);
                    util.Wait(2000);
                    //_E4438C.SetOutput(Output.OFF);
                    tdd_initialized = true;

                    btnTest.Text = "Testing";
                    btnTest.Refresh();
                }
                #endregion *** Initialize

                #region *** MessageBox ***
                int index = 0;
                string msgString = "";

                foreach (ProductTest Test in Program.ProductTest)
                {
                    if (Test.Description != null)
                    {
                        if (Test.Description.ToUpper().Contains("TDD") && Test.Description.ToUpper().Contains("LB"))
                        {
                            index = Array.IndexOf(Program.ProductTest, Test);
                            break;
                        }
                    }
                }

                if (cbxMIPI.Checked)
                {
                    SetVCC(Program.ProductTest[1].VCC);
                    msgString = "Make sure everything is setup for TDD LB testing\r\n" +
                                "   1) Connect tx source cable to TX RF IN(SRC_4G)\r\n" +
                                "   2) Set Mipi control to TDD LB mode\r\n" +
                                "   3) Disconnect highpass filter";
                    MessageBox.Show(msgString, "TDD LB Testing", MessageBoxButtons.OK);
                    this.Refresh();
                    frmMipi.ShowDialog();
                    util.Wait(1000);
                }
                else
                {
                    msgString = "Make sure everything is setup for TDD LB testing\r\n" +
                                "   1) Connect tx source cable to TX RF IN(SRC_4G)\r\n" +
                                "   2) Set Control box to TDD LB mode\r\n" +
                                "       GPCtrl0 : " + Program.ProductTest[index].Gpctrl0_Vmode0 + "\r\n" +
                                "       GPCtrl1 : " + Program.ProductTest[index].Gpctrl1_Vmode1 + "\r\n" +
                                "       GPCtrl2 : " + Program.ProductTest[index].Gpctrl2_Vmode2 + "\r\n" +
                                "   3) Disconnect highpass filter";
                    MessageBox.Show(msgString, "TDD LB Testing", MessageBoxButtons.OK);

                }
                this.Refresh();

                #endregion MessageBox

                # region *** TDD LB Electronic Test ***
                //
                for (int i = 1; i < TestSetting.MaxTestItem; i++)
                {
                    if (Program.ProductTest[i].TestItem == null) break;

                    if (Program.ProductTest[i].Description.ToUpper().Contains("TDD") && Program.ProductTest[i].Description.ToUpper().Contains("LB"))
                    {
                        DeviceTest_TDD(Program.ProductTest[i]);
                    }
                }
                #endregion TDD LB Electronic Test

                #region *** Transfer Test result to datatable ***

                dtTDDLBData.Clear();
                dtTDDLBData = dtResult.Clone();
                foreach (DataRow drTemp in dtResult.Rows)
                {
                    dtTDDLBData.ImportRow(drTemp);
                }
                isTDDLBTested = true;

                #endregion Transfer Test result to datatable

            }
            else if (rbnTDDHB.Checked)
            {
                #region *** Clear Display ***
                dtResult.Clear();
                dtTDDHBData.Clear();

                isTDDHBTested = false;
                isDataSaved = false;

                this.Limit_Fill();
                #endregion *** Clear Display ***

                #region *** Initialize
                cw_initialized = false;
                wcdma_initialized = false;
                tdscdma_initialized = false;
                edge_initialized = false;
                lte_initialized = false;
                if (!tdd_initialized)
                {
                    btnTest.Text = "Initializing";
                    btnTest.Refresh();

                    _SRC_4G.Mode_Initialize(Modulation.LTETDD, false);
                    _SRC_4G.SetArbTrig(Triger_Type.Continous_Reset, Triger_Source.Bus, 0); // Set Trigger to Free Run
                    _SRC_4G.TrigerBus();

                    _MSR_4G.Mod_Initialize(Modulation.LTETDD, false);
                    _PS_VCC.SetVoltage(Program.ProductTest[1].VCC);
                    util.Wait(2000);
                    //_E4438C.SetOutput(Output.OFF);
                    tdd_initialized = true;

                    btnTest.Text = "Testing";
                    btnTest.Refresh();
                }
                #endregion *** Initialize

                #region *** MessageBox ***
                int index = 0;
                string msgString = "";

                foreach (ProductTest Test in Program.ProductTest)
                {
                    if (Test.Description != null)
                    {
                        if (Test.Description.ToUpper().Contains("TDD") && Test.Description.ToUpper().Contains("HB"))
                        {
                            index = Array.IndexOf(Program.ProductTest, Test);
                            break;
                        }
                    }
                }
                if (cbxMIPI.Checked)
                {
                    SetVCC(Program.ProductTest[1].VCC);
                    msgString = "Make sure everything is setup for TDD HB testing\r\n" +
                                "   1) Connect tx source cable to TX RF IN(SRC_4G)\r\n" +
                                "   2) Set Mipi control to TDD HB mode\r\n" +
                                "   3) Disconnect highpass filter";
                    MessageBox.Show(msgString, "TDD LB Testing", MessageBoxButtons.OK);
                    this.Refresh();
                    frmMipi.ShowDialog();
                    util.Wait(1000);
                }
                else
                {
                    msgString = "Make sure everything is setup for TDD HB testing\r\n" +
                                "   1) Connect tx source cable to TX RF IN(SRC_4G)\r\n" +
                                "   2) Set Control box to TDD HB mode\r\n" +
                                "       GPCtrl0 : " + Program.ProductTest[index].Gpctrl0_Vmode0 + "\r\n" +
                                "       GPCtrl1 : " + Program.ProductTest[index].Gpctrl1_Vmode1 + "\r\n" +
                                "       GPCtrl2 : " + Program.ProductTest[index].Gpctrl2_Vmode2 + "\r\n" +
                                "   3) Disconnect highpass filter";
                    MessageBox.Show(msgString, "TDD HB Testing", MessageBoxButtons.OK);

                }
                this.Refresh();

                #endregion MessageBox

                # region *** TDD HB Electronic Test ***
                //
                for (int i = 1; i < TestSetting.MaxTestItem; i++)
                {
                    if (Program.ProductTest[i].TestItem == null) break;

                    if (Program.ProductTest[i].Description.ToUpper().Contains("TDD") && Program.ProductTest[i].Description.ToUpper().Contains("HB"))
                    {
                        DeviceTest_TDD(Program.ProductTest[i]);
                    }
                }
                #endregion TDD HB Electronic Test

                #region *** Transfer Test result to datatable ***

                dtTDDHBData.Clear();
                dtTDDHBData = dtResult.Clone();
                foreach (DataRow drTemp in dtResult.Rows)
                {
                    dtTDDHBData.ImportRow(drTemp);
                }
                isTDDHBTested = true;

                #endregion Transfer Test result to datatable


            }
            else if (rbnLTELB.Checked)
            {
                #region *** Clear Display ***
                dtResult.Clear();
                dtLTELBData.Clear();

                isLTELBTested = false;
                isDataSaved = false;

                this.Limit_Fill();
                #endregion *** Clear Display ***

                #region *** Initialize
                cw_initialized = false;
                wcdma_initialized = false;
                tdscdma_initialized = false;
                edge_initialized = false;
                tdd_initialized = false;
                if (!lte_initialized)
                {
                    btnTest.Text = "Initializing";
                    btnTest.Refresh();

                    _SRC_4G.Mode_Initialize(Modulation.LTEFDD, Mod_Waveform_Name.LTEFDD);
                    _SRC_4G.SetArbTrig(Triger_Type.Continous_Reset, Triger_Source.Bus, 0); // Set Trigger to Free Run
                    _SRC_4G.TrigerBus();

                    _MSR_4G.Mod_Initialize(Modulation.LTEFDD, false);
                    _PS_VCC.SetVoltage(Program.ProductTest[1].VCC);
                    util.Wait(2000);
                    //_E4438C.SetOutput(Output.OFF);
                    lte_initialized = true;

                    btnTest.Text = "Testing";
                    btnTest.Refresh();
                }
                #endregion *** Initialize

                #region *** MessageBox ***
                int index = 0;
                string msgString = "";

                foreach (ProductTest Test in Program.ProductTest)
                {
                    if (Test.Description != null)
                    {
                        if (Test.Description.ToUpper().Contains("LTE") && Test.Description.ToUpper().Contains("LB"))
                        {
                            index = Array.IndexOf(Program.ProductTest, Test);
                            break;
                        }
                    }
                }
                if (cbxMIPI.Checked)
                {
                    SetVCC(Program.ProductTest[1].VCC);
                    msgString = "Make sure everything is setup for LTE testing\r\n" +
                                "   1) Connect tx source cable to TX RF IN(SRC_4G)\r\n" +
                                "   2) Set Mipi control to LTE mode\r\n" +
                                "   3) Disconnect highpass filter";
                    MessageBox.Show(msgString, "LTE Testing", MessageBoxButtons.OK);
                    this.Refresh();
                    frmMipi.ShowDialog();
                    util.Wait(1000);
                }
                else
                {
                    msgString = "Make sure everything is setup for LTE testing\r\n" +
                                "   1) Connect tx source cable to TX RF IN(SRC_4G)\r\n" +
                                "   2) Set Control box to LTE mode\r\n" +
                                "       GPCtrl0 : " + Program.ProductTest[index].Gpctrl0_Vmode0 + "\r\n" +
                                "       GPCtrl1 : " + Program.ProductTest[index].Gpctrl1_Vmode1 + "\r\n" +
                                "       GPCtrl2 : " + Program.ProductTest[index].Gpctrl2_Vmode2 + "\r\n" +
                                "   3) Disconnect highpass filter";
                    MessageBox.Show(msgString, "LTE Testing", MessageBoxButtons.OK);

                }
                this.Refresh();

                #endregion MessageBox

                # region *** LTE LB Electronic Test ***
                //
                for (int i = 1; i < TestSetting.MaxTestItem; i++)
                {
                    if (Program.ProductTest[i].TestItem == null) break;

                    if (Program.ProductTest[i].Description.ToUpper().Contains("LTE") && Program.ProductTest[i].Description.ToUpper().Contains("LB"))
                    {
                        DeviceTest_FDD(Program.ProductTest[i]);
                    }
                }
                #endregion LTE LB Electronic Test

                #region *** Transfer Test result to datatable ***

                dtLTELBData.Clear();
                dtLTELBData = dtResult.Clone();
                foreach (DataRow drTemp in dtResult.Rows)
                {
                    dtLTELBData.ImportRow(drTemp);
                }
                isLTELBTested = true;

                #endregion Transfer Test result to datatable


            }

            #region *** After Test ***
            #region BJ_1
            if (Program.Location == LocationList.BJ_1)
            {
                _SRC_3G.SetOutput(Output.OFF);
                _SRC_4G.SetOutput(Output.OFF);
                _PS_VCC.Initialize();
                _PS_VCC.SetOutput(Output.OFF);
                _Arb_Mipi.SetOutput(Output.OFF, Arb_Channel.Channel_1);
                _Arb_Mipi.SetOutput(Output.OFF, Arb_Channel.Channel_2);
                _Arb_Ramp.SetBurstModeOFF();
                _Arb_Ramp.Initialize(200);
                _Arb_Ramp.SetOutput(Output.OFF, Arb_Channel.Channel_1);
                _Arb_Ramp.SetOutput(Output.OFF, Arb_Channel.Channel_2);
            }
            #endregion BJ_1


            else
            {
                throw new Exception("Bad Location");
            }

            btnTest.Text = "Test";
            btnTest.BackColor = Color.Green;
            btnTest.Enabled = true;

            first_run = true;

            #endregion *** After Test ***

        }

        private void DeviceTest_CW(ProductTest Test)
        {
            double dblTestResult = 0;
            double dblReportResult = 0;
            double LoopResult = 0;
            double LoopResult_Vramp_Low = 0;
            double LoopResult_Vramo_High = 0;
            double Slope_mV = 0;

            int intDelay = 60;

            //Seach for current Test Item
            int index = Array.IndexOf(Program.ProductTest, Test);

            #region -- Idle Current
            if (Test.Description.ToUpper().Contains("IDLE"))
            {
                #region BJ_1
                if (Program.Location == LocationList.BJ_1)
                {
                    intDelay = Set_CW_Condition(Test);

                    _SRC_3G.SetOutput(Output.OFF);

                    util.Wait(intDelay);

                    //dblTestResult = _PS_VCC.High_Current();
                    double[] dblTemp = _PS_VCC.Meas_Current_Trig(Triger_Source.Bus, 0.1, 25, 200, 0);
                    dblTestResult = dblTemp[2];
                    dblReportResult = Math.Round((dblTestResult * 1000), 3);

                    this.UpdateGrid(Test.TestItem.ToString(), dblReportResult);
                    this.UpdateProductionTestResult(Test.TestItem.ToString(), dblReportResult);
                }
                #endregion BJ_1

                else
                {
                    throw new Exception("Bad Location");
                }
            }
            #endregion
   
            #region -- Pout
            if (Test.Description.ToUpper().Contains("POUT"))
            {
                #region BJ_1
                if (Program.Location == LocationList.BJ_1)
                {
                    intDelay = Set_CW_Condition(Test);

                    _SRC_3G.SetOutput(Output.ON);
                    _SRC_3G.SetFrequency(Test.FreqIn);
                    _SRC_3G.SetPower(Test.Pin + Test.LossIn);

                    _MSR_3G.Config__CW_Power(Test.FreqOut, 10, 30, 368, 0.7, 10);
                    _MSR_3G.SetAttenuattor(10);
                    _MSR_3G.SetFrequency(Test.FreqOut);

                    util.Wait(Math.Max(intDelay_MXA, intDelay_SigGen));

                    dblTestResult = _MSR_3G.Get_CW_PowerResult();
                    dblReportResult = Math.Round((dblTestResult + Test.LossOut), 3);
                    this.UpdateGrid(Test.TestItem, dblReportResult);
                    this.UpdateProductionTestResult(Test.TestItem.ToString(), dblReportResult);
                }
                #endregion BJ_1

                else
                {
                    throw new Exception("Bad Location");
                }
            }
            #endregion
 
            #region -- PowerPAE
            if (Test.Description.ToUpper().Contains("POWERPAE"))
            {
                #region BJ_1
                if (Program.Location == LocationList.BJ_1)
                {
                    intDelay = Set_CW_Condition(Test);

                    _SRC_3G.SetOutput(Output.ON);
                    _SRC_3G.SetFrequency(Test.FreqIn);
                    _SRC_3G.SetPower(Test.Pin + Test.LossIn);

                    _MSR_3G.Config__CW_Power(Test.FreqOut, 10, 30, 368, 0.7, 10);
                    _MSR_3G.SetAttenuattor(10);
                    _MSR_3G.SetFrequency(Test.FreqOut);

                    util.Wait(Math.Max(intDelay_MXA, intDelay_SigGen));
                    //Pout
                    dblTestResult = _MSR_3G.Get_CW_PowerResult();
                    dblReportResult = dblMaxPout = Math.Round((dblTestResult + Test.LossOut), 3);
                    this.UpdateGrid(Test.TestItem, dblReportResult);
                    this.UpdateProductionTestResult(Test.TestItem.ToString(), dblReportResult);
                    //Icc
                    //dblTestResult = _PS_VCC.High_Current();
                    double[] dblTemp = _PS_VCC.Meas_Current_Trig(Triger_Source.Bus, 0.1, 25, 200, 0);
                    dblTestResult = dblTemp[2];
                    dblReportResult = dblTestResult * 1000;
                    this.UpdateGrid(Program.ProductTest[index + 1].TestItem, dblReportResult);
                    this.UpdateProductionTestResult(Program.ProductTest[index + 1].TestItem, dblReportResult);
                    //PAE
                    dblTestResult = (Math.Pow(10, ((dblMaxPout - 30) / 10))) / Test.VCC / dblTestResult;
                    dblReportResult = Math.Round(dblTestResult * 100, 3);
                    this.UpdateGrid(Program.ProductTest[index + 2].TestItem, dblReportResult);
                    this.UpdateProductionTestResult(Program.ProductTest[index + 2].TestItem, dblReportResult);
                }
                #endregion BJ_1

                else
                {
                    throw new Exception("Bad Location");
                }
            }
            #endregion

            #region -- Power Servo
            if (Test.Description.ToUpper().Contains("SERVO"))
            {
                if (dblMaxPout < Test.Pout)
                {
                    dblRatedVramp = 1.8;
                    return;
                }

                #region BJ_1
                if (Program.Location == LocationList.BJ_1)
                {
                    int Count = 0;
                    intDelay = Set_CW_Condition(Test);

                    _SRC_3G.SetOutput(Output.ON);
                    _SRC_3G.SetFrequency(Test.FreqIn);
                    _SRC_3G.SetPower(Test.Pin + Test.LossIn);

                    this.SetVramp(1.2);
                    _MSR_3G.Config__CW_Power(Test.FreqOut, 10, 30, 368, 0.7, 10);
                    _MSR_3G.SetAttenuattor(10);
                    _MSR_3G.SetFrequency(Test.FreqOut);

                    util.Wait(Math.Max(intDelay_MXA, intDelay_SigGen));

                    LoopResult_Vramp_Low = _MSR_3G.Get_CW_PowerResult();

                    this.SetVramp(1.6);
                    util.Wait(intDelay_MXA);
                    LoopResult_Vramo_High = _MSR_3G.Get_CW_PowerResult();

                    Slope_mV = (LoopResult_Vramo_High - LoopResult_Vramp_Low) / 400;

                    LoopResult = LoopResult_Vramp_Low + Test.LossOut;
                    dblRatedVramp = 1.2 + (Test.Pout - LoopResult) / Slope_mV / 1000;

                    if (dblRatedVramp < 0.8 || dblRatedVramp > 1.8) dblRatedVramp = 1.8;

                    while (Math.Abs(LoopResult - Test.Pout) > 0.05 && dblRatedVramp < 1.8 && dblRatedVramp > 0.8)
                    {
                        this.SetVramp(dblRatedVramp);
                        util.Wait(intDelay_PowerMeter);
                        LoopResult = _MSR_3G.Get_CW_PowerResult() + Test.LossOut;
                        dblRatedVramp = dblRatedVramp + (Test.Pout - LoopResult) / Slope_mV / 1000;
                        Count++;
                        if (Count > 20) break;
                    }
                    //Rated Pout
                    dblTestResult = LoopResult;
                    dblReportResult = Math.Round(dblTestResult, 3);
                    this.UpdateGrid(Test.TestItem, dblReportResult);
                    this.UpdateProductionTestResult(Test.TestItem, dblReportResult);
                    //Rated Vramp
                    dblTestResult = dblRatedVramp;
                    dblReportResult = Math.Round(dblTestResult, 3);
                    this.UpdateGrid(Program.ProductTest[index + 1].TestItem, dblReportResult);
                    this.UpdateProductionTestResult(Program.ProductTest[index + 1].TestItem, dblReportResult);
                    //Rated Icc
                    //dblTestResult = _PS_VCC.High_Current();
                    double[] dblTemp = _PS_VCC.Meas_Current_Trig(Triger_Source.Bus, 0.1, 25, 200, 0);
                    dblTestResult = dblTemp[2];
                    dblReportResult = dblTestResult * 1000;
                    this.UpdateGrid(Program.ProductTest[index + 2].TestItem, dblReportResult);
                    this.UpdateProductionTestResult(Program.ProductTest[index + 2].TestItem, dblReportResult);
                    //Rated PAE
                    dblTestResult = (Math.Pow(10, ((LoopResult - 30) / 10))) / Test.VCC / dblTestResult;
                    dblReportResult = Math.Round(dblTestResult * 100, 3);
                    this.UpdateGrid(Program.ProductTest[index + 3].TestItem, dblReportResult);
                    this.UpdateProductionTestResult(Program.ProductTest[index + 3].TestItem, dblReportResult);
                }
                #endregion BJ_1

                else
                {
                    throw new Exception("Bad Location");
                }

            }
            #endregion -- Power Servo

            #region -- Harmonic
            if (Test.Description.ToUpper().Contains("HARMONIC"))
            {
                if (Test.Vramp == 0.01)
                {
                    if (dblRatedVramp == 1.8) return;
                    Test.Vramp = dblRatedVramp;
                }

                #region BJ_1
                if (Program.Location == LocationList.BJ_1)
                {
                    intDelay = Set_CW_Condition(Test);

                    _SRC_3G.SetOutput(Output.ON);
                    _SRC_3G.SetFrequency(Test.FreqIn);
                    _SRC_3G.SetPower(Test.Pin + Test.LossIn);

                    _MSR_4G.Config__CW_Power(Test.FreqOut, 0, 30, 368, 0.7, 10);
                    _MSR_4G.SetAttenuattor(0);
                    _MSR_4G.SetFrequency(Test.FreqOut);

                    util.Wait(Math.Max(intDelay_SigGen, intDelay_MXA));

                    dblTestResult = _MSR_4G.Get_CW_PowerResult();
                    dblReportResult = Math.Round(dblTestResult + Test.LossOut, 3);
                    this.UpdateGrid(Test.TestItem, dblReportResult);
                    this.UpdateProductionTestResult(Test.TestItem, dblReportResult);
                }
                #endregion BJ_1

                else
                {
                    throw new Exception("Bad Location");
                }

            }
            #endregion -- Harmonic

            #region -- Stress
            if (Test.Description.ToUpper().Contains("STRESS"))
            {
                double dbl2ndMaxPout = 0;
                double VCCNormal = Program.ProductTest[1].VCC;
                double VCCStress = Test.VCC;

                #region BJ_1
                if (Program.Location == LocationList.BJ_1)
                {
                    _SRC_3G.SetOutput(Output.ON);
                    _SRC_3G.SetFrequency(Test.FreqIn);
                    _SRC_3G.SetPower(Test.Pin + Test.LossIn);

                    // Test 1st Normal Max Pout if not tested before
                    if (dblMaxPout == 0)
                    {
                        Test.VCC = VCCNormal;
                        intDelay = Math.Max(intDelay, Set_CW_Condition(Test));
                        _MSR_3G.Config__CW_Power(Test.FreqOut, 10, 30, 368, 0.7, 10);
                        _MSR_3G.SetAttenuattor(10);
                        _MSR_3G.SetFrequency(Test.FreqOut);

                        util.Wait(Math.Max(intDelay_MXA, intDelay_SigGen));

                        dblTestResult = _MSR_3G.Get_CW_PowerResult();

                        dblMaxPout = Math.Round((dblTestResult + Test.LossOut), 3);
                    }
                    //Stress
                    Test.VCC = VCCStress;
                    intDelay = Math.Max(intDelay, Set_CW_Condition(Test));
                    util.Wait(intDelay + 5);   //Stress one time frame
                    //Test 2nd Normal Max Pout
                    Test.VCC = VCCNormal;
                    intDelay = Math.Max(intDelay, Set_CW_Condition(Test));
                    _MSR_3G.Config__CW_Power(Test.FreqOut, 0, 30, 368, 0.7, 10);
                    _MSR_3G.SetAttenuattor(10);
                    _MSR_3G.SetFrequency(Test.FreqOut);

                    util.Wait(Math.Max(intDelay_MXA, intDelay_SigGen));

                    dblTestResult = _MSR_3G.Get_CW_PowerResult();
                    dbl2ndMaxPout = Math.Round((dblTestResult + Test.LossOut), 3);
                    dblReportResult = Math.Round((dbl2ndMaxPout - dblMaxPout), 3);
                    //Stress delta
                    this.UpdateGrid(Test.TestItem, dblReportResult);
                    this.UpdateProductionTestResult(Test.TestItem, dblReportResult);
                }
                #endregion BJ_1

                else
                {
                    throw new Exception("Bad Location");
                }
            }
            #endregion -- Stress
   
            #region -- RX
            if (Test.Description.ToUpper().Contains("RX"))
            {
                string msgString = "";
                if (cbxMIPI.Checked)
                {
                    SetVCC(Program.ProductTest[1].VCC);
                    msgString = "Make sure everything is setup for testing  " + Test.Description + "\r\n" +
                                "   1) Connect rx source cable to " + Test.Description + "\r\n" +
                                "   2) Set Mipi control to " + Test.TestItem + " mode\r\n" +
                                "   3) Disconnect highpass filter";

                    MessageBox.Show(msgString, Test.TestItem + " Testing", MessageBoxButtons.OK);
                    this.Refresh();
                    frmMipi.ShowDialog();
                    util.Wait(1000);
                }
                else
                {
                    msgString = "Make sure everything is setup for testing  " + Test.Description +"\r\n"+
                                "   1) Connect rx source cable to " + Test.Description + "\r\n" +
                                "   2) Set Control box to " + Test.Description + " mode" + "\r\n" +
                                "       GPCtrl0 : " + Test.Gpctrl0_Vmode0 + "\r\n" +
                                "       GPCtrl1 : " + Test.Gpctrl1_Vmode1 + "\r\n" +
                                "       GPCtrl2 : " + Test.Gpctrl2_Vmode2;

                    MessageBox.Show(msgString, Test.TestItem + " Testing", MessageBoxButtons.OK);
             
                }
                this.Refresh();

                #region BJ_1
                if (Program.Location == LocationList.BJ_1)
                {
                    intDelay = Set_CW_Condition(Test);
                    _SRC_3G.SetOutput(Output.ON);

                    _SRC_3G.SetFrequency(Test.FreqIn);
                    _SRC_3G.SetPower(Test.Pin + Test.LossIn);

                    _MSR_3G.Config__CW_Power(Test.FreqOut, 10, 30, 368, 0.7, 10);
                    _MSR_3G.SetAttenuattor(10);
                    _MSR_3G.SetFrequency(Test.FreqOut);

                    util.Wait(Math.Max(intDelay_MXA, intDelay_SigGen));

                    dblTestResult = _MSR_3G.Get_CW_PowerResult();
                    dblReportResult = Math.Round((dblTestResult + Test.LossOut), 3);
                    this.UpdateGrid(Test.TestItem, dblReportResult);
                    this.UpdateProductionTestResult(Test.TestItem, dblReportResult);
                }
                #endregion BJ_1

                else
                {
                    throw new Exception("Bad Location");
                }
            }
            #endregion -- RX

        }

        private void DeviceTest_EDGE(ProductTest Test)
        {
            double dblTestResult = 0;
            double dblReportResult = 0;

            int intDelay = 60;

            //Seach for current Test Item
            int index = Array.IndexOf(Program.ProductTest, Test);

            #region -- Idle Current
            if (Test.Description.ToUpper().Contains("IDLE"))
            {
                double I_Vcc;

                #region BJ_1
                if (Program.Location == LocationList.BJ_1)
                {
                    Set_EDGE_Condition(Test);

                    _SRC_3G.SetOutput(Output.OFF);

                    util.Wait(intDelay);

                    //Idle VCC
                    //dblTestResult = I_Vcc = _PS_VCC.High_Current();
                    //double[] dblTemp = _PS_VCC.Meas_Current_Trig(Triger_Source.Int, 0.1, 25, 200, 0);
                    double[] dblTemp = _PS_VCC.Meas_Current_Trig(Triger_Source.Int, 0.1, 15, 80, 0);
                    dblTestResult = I_Vcc = dblTemp[2];
                    dblReportResult = Math.Round((dblTestResult * 1000), 3);
                    this.UpdateGrid(Program.ProductTest[index].TestItem.ToString(), dblReportResult);
                    this.UpdateProductionTestResult(Program.ProductTest[index].TestItem.ToString(), dblReportResult);

                }
                #endregion BJ_1

                else
                {
                    throw new Exception("Bad Location");
                }
            }
            #endregion -- Idle Current

            #region -- Gain
            if (Test.Description.ToUpper().Contains("GAIN"))
            {
                double I_Vcc = 0.0;
                double I_Vbat = 0.0;
                double dblPout = 0.0;
                double dblPin = Test.Pin;
                double PoutLL = Test.Pout - 0.05;
                double PoutUL = Test.Pout + 0.05;
                EDGE_Pin = dblPin;

                #region BJ_1
                if (Program.Location == LocationList.BJ_1)
                {
                    Set_EDGE_Condition(Test);

                    _SRC_3G.SetFrequency(Test.FreqIn);
                    _SRC_3G.SetPower(dblPin + Test.LossIn);
                    _SRC_3G.SetModOutput(Output.ON);
                    _SRC_3G.SetOutput(Output.ON);
                    _MSR_3G.Config__EDGE_CONTINOUS_Burst_Power(Test.FreqOut);
                    intDelay = Math.Max(intDelay_SigGen, intDelay_MXA);
                    util.Wait(intDelay);

                    dblPout = _MSR_3G.Get_EDGE_CONTINOUS_Burst_Power_Result();
                    dblPout += +Test.LossOut;
                    while (dblPout <= PoutLL || dblPout >= PoutUL)
                    {
                        dblPin = dblPin + Test.Pout - dblPout;
                        if (dblPin > 0) break;

                        _SRC_3G.SetPower(dblPin + Test.LossIn);
                        util.Wait(intDelay);
                        dblPout = _MSR_3G.Get_EDGE_CONTINOUS_Burst_Power_Result();
                        dblPout += +Test.LossOut;
                        EDGE_Pin = dblPin;
                    }

                    //Pout
                    dblTestResult = dblPout;
                    dblReportResult = Math.Round(dblTestResult, 3);
                    this.UpdateGrid(Test.TestItem.ToString(), dblReportResult);
                    this.UpdateProductionTestResult(Test.TestItem.ToString(), dblReportResult);

                    //Pin
                    dblTestResult = EDGE_Pin;
                    dblReportResult = Math.Round(dblTestResult, 3);
                    this.UpdateGrid(Program.ProductTest[index + 1].TestItem.ToString(), dblReportResult);
                    this.UpdateProductionTestResult(Program.ProductTest[index + 1].TestItem.ToString(), dblReportResult);

                    //Icc VCC
                    //dblTestResult = I_Vcc = _PS_VCC.High_Current();
                    double[] dblTemp = _PS_VCC.Meas_Current_Trig(Triger_Source.Int, 0.1, 15, 80, 0);
                    dblTestResult = I_Vcc = dblTemp[2];
                    dblReportResult = Math.Round((dblTestResult * 1000), 3);
                    this.UpdateGrid(Program.ProductTest[index + 2].TestItem.ToString(), dblReportResult);
                    this.UpdateProductionTestResult(Program.ProductTest[index + 2].TestItem.ToString(), dblReportResult);

                    //PAE
                    dblTestResult = Math.Pow(10, ((dblPout - 30) / 10)) / Test.VCC / (I_Vcc + I_Vbat);
                    dblReportResult = Math.Round((dblTestResult * 100), 3);
                    this.UpdateGrid(Program.ProductTest[index + 3].TestItem.ToString(), dblReportResult);
                    this.UpdateProductionTestResult(Program.ProductTest[index + 3].TestItem.ToString(), dblReportResult);

                    //Gain
                    dblTestResult = dblPout - EDGE_Pin;
                    dblReportResult = Math.Round(dblTestResult, 3);
                    this.UpdateGrid(Program.ProductTest[index + 4].TestItem.ToString(), dblReportResult);
                    this.UpdateProductionTestResult(Program.ProductTest[index + 4].TestItem.ToString(), dblReportResult);

                    _SRC_3G.SetModOutput(Output.ON);
                    util.Wait(1000);
                }
                #endregion BJ_1

                else
                {
                    throw new Exception("Bad Location");
                }
            }
            #endregion -- GAIN

            #region -- ACP
            if (Test.Description.ToUpper().Contains("ACP"))
            {
                double dblPin;
                double[] dblResult;

                if (EDGE_Pin == -99)
                    dblPin = Test.Pin;
                else
                    dblPin = EDGE_Pin;

                #region BJ_1
                if (Program.Location == LocationList.BJ_1)
                {
                    Set_EDGE_Condition(Test);

                    _SRC_3G.SetFrequency(Test.FreqIn);
                    _SRC_3G.SetPower(dblPin + Test.LossIn);
                    _SRC_3G.SetOutput(Output.ON);

                    //_MSR_3G.Config__EDGE_CONTINOUS_ACP(Test.FreqOut);
                    _MSR_3G.Config__EDGE_CONTINOUS_ORFS(Test.FreqOut);
                    intDelay = Math.Max(intDelay_SigGen, intDelay_MXA);
                    util.Wait(intDelay);

                    dblResult = _MSR_3G.Get_EDGE_CONTINOUS_ORFS_Result();

                    //ACP -400kHz
                    dblTestResult = dblResult[6];
                    dblReportResult = Math.Round(dblTestResult, 3);
                    this.UpdateGrid(Test.TestItem.ToString(), dblReportResult);
                    this.UpdateProductionTestResult(Test.TestItem.ToString(), dblReportResult);

                    //ACP +400kMHz
                    dblTestResult = dblResult[7];
                    dblReportResult = Math.Round(dblTestResult, 3);
                    this.UpdateGrid(Program.ProductTest[index + 1].TestItem.ToString(), dblReportResult);
                    this.UpdateProductionTestResult(Program.ProductTest[index + 1].TestItem.ToString(), dblReportResult);

                }
                #endregion BJ_1

                else
                {
                    throw new Exception("Bad Location");
                }
            }
            #endregion -- ACP

            #region -- EVM
            if (Test.Description.ToUpper().Contains("EVM"))
            {
                double dblPin;

                if (EDGE_Pin == -99)
                    dblPin = Test.Pin;
                else
                    dblPin = EDGE_Pin;

                #region BJ_1
                if (Program.Location == LocationList.BJ_1)
                {
                    Set_EDGE_Condition(Test);

                    _SRC_3G.SetFrequency(Test.FreqIn);
                    _SRC_3G.SetPower(dblPin + Test.LossIn);
                    _SRC_3G.SetOutput(Output.ON);

                    _MSR_3G.Config__EDGE_CONTINOUS_EVM(Test.FreqOut);
                    intDelay = Math.Max(intDelay_SigGen, intDelay_MXA);
                    util.Wait(intDelay);

                    dblTestResult = _MSR_3G.Get_EDGE_CONTINOUS_EVM_Result();

                    //EVM
                    dblReportResult = Math.Round(dblTestResult, 3);
                    this.UpdateGrid(Test.TestItem.ToString(), dblReportResult);
                    this.UpdateProductionTestResult(Test.TestItem.ToString(), dblReportResult);

                }
                #endregion BJ_1

                else
                {
                    throw new Exception("Bad Location");
                }
            }
            #endregion -- EVM
        }

        private void DeviceTest_WCDMA(ProductTest Test)
        {
            double dblTestResult = 0;
            double dblReportResult = 0;

            int intDelay = 60;

            //Seach for current Test Item
            int index = Array.IndexOf(Program.ProductTest, Test);

            #region -- Idle Current
            if (Test.Description.ToUpper().Contains("IDLE"))
            {
                double I_Vcc;
                double I_Vbat;

                #region BJ_1
                if (Program.Location == LocationList.BJ_1)
                {
                    Set_WCDMA_Condition(Test);

                    _SRC_3G.SetOutput(Output.OFF);

                    util.Wait(intDelay);

                    //Idle Vbat
                    dblTestResult = I_Vbat = _PS_VBAT.RMS_Current(PS_66319B_Channel.Channel_1);
                    if (Program.evb)
                        dblReportResult = Math.Round((dblTestResult * 1000), 3);
                    else
                        dblReportResult = Math.Round((dblTestResult * 1000), 3) + Test.SocketOffset;

                    this.UpdateGrid(Test.TestItem.ToString(), dblReportResult);
                    this.UpdateProductionTestResult(Test.TestItem.ToString(), dblReportResult);

                    //Idle VCC
                    //dblTestResult = I_Vcc = _PS_VCC.High_Current();
                    double[] dblTemp = _PS_VCC.Meas_Current_Trig(Triger_Source.Bus, 0.1, 25, 200, 0);
                    dblTestResult = I_Vcc = dblTemp[0];
                    if (Program.evb)
                        dblReportResult = Math.Round((dblTestResult * 1000), 3);
                    else
                        dblReportResult = Math.Round((dblTestResult * 1000), 3) + Program.ProductTest[index + 1].SocketOffset;
                    this.UpdateGrid(Program.ProductTest[index + 1].TestItem.ToString(), dblReportResult);
                    this.UpdateProductionTestResult(Program.ProductTest[index + 1].TestItem.ToString(), dblReportResult);

                    //Idle Total
                    dblTestResult = I_Vbat + I_Vcc;
                    if (Program.evb)
                        dblReportResult = Math.Round((dblTestResult * 1000), 3);
                    else
                        dblReportResult = Math.Round((dblTestResult * 1000), 3) + Program.ProductTest[index + 2].SocketOffset;
                    this.UpdateGrid(Program.ProductTest[index + 2].TestItem.ToString(), dblReportResult);
                    this.UpdateProductionTestResult(Program.ProductTest[index + 2].TestItem.ToString(), dblReportResult);
                }
                #endregion BJ_1

                else
                {
                    throw new Exception("Bad Location");
                }
            }
            #endregion -- Idle Current

            #region -- Gain
            if (Test.Description.ToUpper().Contains("GAIN"))
            {
                double I_Vcc = 0.0;
                double I_Vbat = 0.0;
                double dblPout = 0.0;
                double dblPin = Test.Pin;
                double PoutLL = Test.Pout - 0.09;
                double PoutUL = Test.Pout + 0.09;
                WCDMA_Pin = dblPin;

                #region BJ_1
                if (Program.Location == LocationList.BJ_1)
                {
                    Set_WCDMA_Condition(Test);

                    //_E4438C.SetModOutput(Output.OFF);
                    _SRC_3G.SetFrequency(Test.FreqIn);
                    _SRC_3G.SetPower(dblPin + Test.LossIn);
                    _SRC_3G.SetOutput(Output.ON);

                    //_MXA_N9020A.Config__CW_Power(Test.FreqOut, 20, 30, 368, 20, 10);
                    _MSR_3G.Config__WCDMA_CHP(Test.FreqOut);
                    intDelay = Math.Max(intDelay_SigGen, intDelay_MXA);
                    util.Wait(intDelay );

                    dblPout = _MSR_3G.Get_WCDMA_CHP_Result();
                    dblPout += +Test.LossOut;
                    while (dblPout <= PoutLL || dblPout >= PoutUL)
                    {
                        dblPin = dblPin + Test.Pout - dblPout;
                        if (dblPin > 4) break;

                        _SRC_3G.SetPower(dblPin + Test.LossIn);
                        util.Wait(intDelay * 1);
                        dblPout = _MSR_3G.Get_WCDMA_CHP_Result();
                        dblPout += +Test.LossOut;
                        WCDMA_Pin = dblPin;
                    }
                    //_E4438C.SetModOutput(Output.ON);

                    //Pin
                    dblTestResult = WCDMA_Pin;
                    if (Program.evb)
                        dblReportResult = Math.Round(dblTestResult, 3);
                    else
                        dblReportResult = Math.Round(dblTestResult, 3) + Test.SocketOffset;
                    this.UpdateGrid(Test.TestItem.ToString(), dblReportResult);
                    this.UpdateProductionTestResult(Test.TestItem.ToString(), dblReportResult);

                    //Pout
                    dblTestResult = dblPout;
                    dblReportResult = Math.Round(dblTestResult, 3);
                    this.UpdateGrid(Program.ProductTest[index + 1].TestItem.ToString(), dblReportResult);
                    this.UpdateProductionTestResult(Program.ProductTest[index + 1].TestItem.ToString(), dblReportResult);

                    //Gain
                    dblTestResult = dblPout - WCDMA_Pin;
                    if (Program.evb)
                        dblReportResult = Math.Round(dblTestResult, 3);
                    else
                        dblReportResult = Math.Round(dblTestResult, 3) + Program.ProductTest[index + 2].SocketOffset;
                    this.UpdateGrid(Program.ProductTest[index + 2].TestItem.ToString(), dblReportResult);
                    this.UpdateProductionTestResult(Program.ProductTest[index + 2].TestItem.ToString(), dblReportResult);

                    //Icc Vbat
                    dblTestResult = I_Vbat = _PS_VBAT.RMS_Current(PS_66319B_Channel.Channel_1);
                    if (Program.evb)
                        dblReportResult = Math.Round((dblTestResult * 1000), 3);
                    else
                        dblReportResult = Math.Round((dblTestResult * 1000), 3) + Program.ProductTest[index + 3].SocketOffset;
                    this.UpdateGrid(Program.ProductTest[index + 3].TestItem.ToString(), dblReportResult);
                    this.UpdateProductionTestResult(Program.ProductTest[index + 3].TestItem.ToString(), dblReportResult);

                    //Icc VCC
                    //dblTestResult = I_Vcc = _PS_VCC.High_Current();
                    double[] dblTemp = _PS_VCC.Meas_Current_Trig(Triger_Source.Bus, 0.1, 25, 200, 0);
                    dblTestResult = I_Vcc = dblTemp[0];
                    if (Program.evb)
                        dblReportResult = Math.Round((dblTestResult * 1000), 3);
                    else
                        dblReportResult = Math.Round((dblTestResult * 1000), 3) + Program.ProductTest[index + 4].SocketOffset;
                    this.UpdateGrid(Program.ProductTest[index + 4].TestItem.ToString(), dblReportResult);
                    this.UpdateProductionTestResult(Program.ProductTest[index + 4].TestItem.ToString(), dblReportResult);

                    //Icc Total
                    dblTestResult = I_Vcc + I_Vbat;
                    if (Program.evb)
                        dblReportResult = Math.Round((dblTestResult * 1000), 3);
                    else
                        dblReportResult = Math.Round((dblTestResult * 1000), 3) + Program.ProductTest[index + 5].SocketOffset;
                    this.UpdateGrid(Program.ProductTest[index + 5].TestItem.ToString(), dblReportResult);
                    this.UpdateProductionTestResult(Program.ProductTest[index + 5].TestItem.ToString(), dblReportResult);

                    //PAE
                    dblTestResult = Math.Pow(10, ((dblPout - 30) / 10)) / Test.VCC / (I_Vcc + I_Vbat);
                    dblReportResult = Math.Round((dblTestResult * 100), 3);
                    this.UpdateGrid(Program.ProductTest[index + 6].TestItem.ToString(), dblReportResult);
                    this.UpdateProductionTestResult(Program.ProductTest[index + 6].TestItem.ToString(), dblReportResult);
                }
                #endregion BJ_1

                else
                {
                    throw new Exception("Bad Location");
                }
            }
            #endregion -- GAIN

            #region -- ACP
            if (Test.Description.ToUpper().Contains("ACP"))
            {
                double dblPin;
                double[] dblResult;

                if (WCDMA_Pin == -99)
                    dblPin = Test.Pin;
                else
                    dblPin = WCDMA_Pin;

                #region BJ_1
                if (Program.Location == LocationList.BJ_1)
                {
                    Set_WCDMA_Condition(Test);

                    _SRC_3G.SetFrequency(Test.FreqIn);
                    _SRC_3G.SetPower(dblPin + Test.LossIn);
                    _SRC_3G.SetOutput(Output.ON);

                    _MSR_3G.Config__WCDMA_ACP(Test.FreqOut);
                    intDelay = Math.Max(intDelay_SigGen, intDelay_MXA);
                    util.Wait(intDelay * 1);

                    dblResult = _MSR_3G.Get_WCDMA_ACP_Result();

                    //ACP -5MHz
                    dblTestResult = dblResult[1];
                    if (Program.evb)
                        dblReportResult = Math.Round(dblTestResult, 3);
                    else
                        dblReportResult = Math.Round(dblTestResult, 3) + Test.SocketOffset;
                    this.UpdateGrid(Test.TestItem.ToString(), dblReportResult);
                    this.UpdateProductionTestResult(Test.TestItem.ToString(), dblReportResult);

                    //ACP +5MHz
                    dblTestResult = dblResult[2];
                    if (Program.evb)
                        dblReportResult = Math.Round(dblTestResult, 3);
                    else
                        dblReportResult = Math.Round(dblTestResult, 3) + Program.ProductTest[index + 1].SocketOffset;
                    this.UpdateGrid(Program.ProductTest[index + 1].TestItem.ToString(), dblReportResult);
                    this.UpdateProductionTestResult(Program.ProductTest[index + 1].TestItem.ToString(), dblReportResult);

                    //ACP -10MHz
                    dblTestResult = dblResult[3];
                    if (Program.evb)
                        dblReportResult = Math.Round(dblTestResult, 3);
                    else
                        dblReportResult = Math.Round(dblTestResult, 3) + Program.ProductTest[index + 2].SocketOffset;
                    this.UpdateGrid(Program.ProductTest[index + 2].TestItem.ToString(), dblReportResult);
                    this.UpdateProductionTestResult(Program.ProductTest[index + 2].TestItem.ToString(), dblReportResult);

                    //ACP +10MHz
                    dblTestResult = dblResult[4];
                    if (Program.evb)
                        dblReportResult = Math.Round(dblTestResult, 3);
                    else
                        dblReportResult = Math.Round(dblTestResult, 3) + Program.ProductTest[index + 3].SocketOffset;
                    this.UpdateGrid(Program.ProductTest[index + 3].TestItem.ToString(), dblReportResult);
                    this.UpdateProductionTestResult(Program.ProductTest[index + 3].TestItem.ToString(), dblReportResult);

                }
                #endregion BJ_1

                else
                {
                    throw new Exception("Bad Location");
                }
            }
            #endregion -- ACP

            #region -- EVM
            if (Test.Description.ToUpper().Contains("EVM"))
            {
                double dblPin;

                if (WCDMA_Pin == -99)
                    dblPin = Test.Pin;
                else
                    dblPin = WCDMA_Pin;

                #region BJ_1
                if (Program.Location == LocationList.BJ_1)
                {
                    Set_WCDMA_Condition(Test);

                    _SRC_3G.SetFrequency(Test.FreqIn);
                    _SRC_3G.SetPower(dblPin + Test.LossIn);
                    _SRC_3G.SetOutput(Output.ON);

                    _MSR_3G.Config__WCDMA_EVM(Test.FreqOut);
                    intDelay = Math.Max(intDelay_SigGen, intDelay_MXA);
                    util.Wait(intDelay * 1);

                    dblTestResult = _MSR_3G.Get_WCDMA_EVM_Result();

                    //EVM
                    if (Program.evb)
                        dblReportResult = Math.Round(dblTestResult, 3);
                    else
                        dblReportResult = Math.Round(dblTestResult, 3) + Test.SocketOffset;

                    this.UpdateGrid(Test.TestItem.ToString(), dblReportResult);
                    this.UpdateProductionTestResult(Test.TestItem.ToString(), dblReportResult);

                }
                #endregion BJ_1

                else
                {
                    throw new Exception("Bad Location");
                }
            }
            #endregion -- EVM

            #region -- Harmonic
            if (Test.Description.ToUpper().Contains("HARMONIC"))
            {
                double dblPin;

                if (WCDMA_Pin == -99)
                    dblPin = Test.Pin;
                else
                    dblPin = WCDMA_Pin;

                #region BJ_1
                if (Program.Location == LocationList.BJ_1)
                {
                    Set_WCDMA_Condition(Test);

                    _SRC_3G.SetModOutput(Output.OFF);
                    _SRC_3G.SetFrequency(Test.FreqIn);
                    _SRC_3G.SetPower(dblPin + Test.LossIn);
                    _SRC_3G.SetOutput(Output.ON);

                    _MSR_4G.Config__CW_Power_FreeRun(Test.FreqOut, 30, 0, 2, 10);
                    intDelay = Math.Max(intDelay_SigGen, intDelay_MXA);
                    util.Wait(intDelay);

                    MessageBox.Show("Connect highpass filter", "WCDMA Testing", MessageBoxButtons.OK);

                    dblTestResult = _MSR_4G.Get_CW_PowerResult();
                    dblTestResult += Test.LossOut;

                    _SRC_3G.SetModOutput(Output.ON);

                    //Harmonic
                    if (Program.evb)
                        dblReportResult = Math.Round(dblTestResult, 3);
                    else
                        dblReportResult = Math.Round(dblTestResult, 3) + Test.SocketOffset;
                    this.UpdateGrid(Test.TestItem.ToString(), dblReportResult);
                    this.UpdateProductionTestResult(Test.TestItem.ToString(), dblReportResult);


                    MessageBox.Show("Disconnect highpass filter", "WCDMA Testing", MessageBoxButtons.OK);

                }
                #endregion BJ_1

                else
                {
                    throw new Exception("Bad Location");
                }
            }
            #endregion -- Harmonic

        }

        private void DeviceTest_TDSCDMA(ProductTest Test)
        {
            double dblTestResult = 0;
            double dblReportResult = 0;

            int intDelay = 60;

            //Seach for current Test Item
            int index = Array.IndexOf(Program.ProductTest, Test);

            #region -- Idle Current
            if (Test.Description.ToUpper().Contains("IDLE"))
            {
                double I_Vcc;

                #region BJ_1
                if (Program.Location == LocationList.BJ_1)
                {
                    _SRC_3G.SetOutput(Output.OFF);

                    Set_TD_Condition(Test);

                    util.Wait(intDelay);

                    //Idle VCC
                    //dblTestResult = I_Vcc = _PS_VCC.High_Current();
                    double[] dblTemp = _PS_VCC.Meas_Current_Trig(Triger_Source.Bus, 0.1, 25, 200, 0);
                    dblTestResult = I_Vcc = dblTemp[2];
                    dblReportResult = Math.Round((dblTestResult * 1000), 3);
                    this.UpdateGrid(Test.TestItem.ToString(), dblReportResult);
                    this.UpdateProductionTestResult(Test.TestItem.ToString(), dblReportResult);

                }
                #endregion BJ_1

                else
                {
                    throw new Exception("Bad Location");
                }
            }
            #endregion -- Idle Current

            #region -- Gain
            if (Test.Description.ToUpper().Contains("GAIN"))
            {
                double I_Vcc = 0.0;
                double I_Vbat = 0.0;
                double dblPout = 0.0;
                double dblPin = Test.Pin;
                double PoutLL = Test.Pout - 0.05;
                double PoutUL = Test.Pout + 0.05;
                TDSCDMA_Pin = dblPin;

                #region BJ_1
                if (Program.Location == LocationList.BJ_1)
                {
                    Set_TD_Condition(Test);

                    _SRC_3G.SetFrequency(Test.FreqIn);
                    _SRC_3G.SetPower(dblPin + Test.LossIn);
                    _SRC_3G.SetOutput(Output.ON);

                    _MSR_3G.Config__TDSCDMA_CHP(Test.FreqOut);
                    intDelay = Math.Max(intDelay_SigGen, intDelay_MXA);
                    util.Wait(intDelay * 10);

                    dblPout = _MSR_3G.Get_TDSCDMA_CHP_Result();
                    dblPout += +Test.LossOut;
                    while (dblPout <= PoutLL || dblPout >= PoutUL)
                    {
                        dblPin = dblPin + Test.Pout - dblPout;
                        if (dblPin > 2) break;

                        _SRC_3G.SetPower(dblPin + Test.LossIn);
                        util.Wait(intDelay * 3);
                        dblPout = _MSR_3G.Get_TDSCDMA_CHP_Result();
                        dblPout += +Test.LossOut;
                        TDSCDMA_Pin = dblPin;
                    }

                    //Pout
                    dblTestResult = dblPout;
                    dblReportResult = Math.Round(dblTestResult, 3);
                    this.UpdateGrid(Test.TestItem.ToString(), dblReportResult);
                    this.UpdateProductionTestResult(Test.TestItem.ToString(), dblReportResult);

                    //Pin
                    dblTestResult = TDSCDMA_Pin;
                    dblReportResult = Math.Round(dblTestResult, 3);
                    this.UpdateGrid(Program.ProductTest[index + 1].TestItem.ToString(), dblReportResult);
                    this.UpdateProductionTestResult(Program.ProductTest[index + 1].TestItem.ToString(), dblReportResult);

                    //Icc VCC
                    //dblTestResult = I_Vcc = _PS_VCC.High_Current();
                    double[] dblTemp = _PS_VCC.Meas_Current_Trig(Triger_Source.Bus, 0.1, 25, 200, 0);
                    dblTestResult = I_Vcc = dblTemp[2];
                    dblReportResult = Math.Round(dblTestResult * 1000, 3);
                    this.UpdateGrid(Program.ProductTest[index + 2].TestItem.ToString(), dblReportResult);
                    this.UpdateProductionTestResult(Program.ProductTest[index + 2].TestItem.ToString(), dblReportResult);

                    //PAE
                    dblTestResult = Math.Pow(10, ((dblPout - 30) / 10)) / Test.VCC / (I_Vcc + I_Vbat);
                    dblReportResult = Math.Round((dblTestResult * 100), 3);
                    this.UpdateGrid(Program.ProductTest[index + 3].TestItem.ToString(), dblReportResult);
                    this.UpdateProductionTestResult(Program.ProductTest[index + 3].TestItem.ToString(), dblReportResult);

                    //Gain
                    dblTestResult = dblPout - TDSCDMA_Pin;
                    dblReportResult = Math.Round((dblTestResult), 3);
                    this.UpdateGrid(Program.ProductTest[index + 4].TestItem.ToString(), dblReportResult);
                    this.UpdateProductionTestResult(Program.ProductTest[index + 4].TestItem.ToString(), dblReportResult);

                }
                #endregion BJ_1

                else
                {
                    throw new Exception("Bad Location");
                }
            }
            #endregion -- GAIN

            #region -- ACP
            if (Test.Description.ToUpper().Contains("ACP"))
            {
                double dblPin;
                double[] dblResult;

                if (TDSCDMA_Pin == -99)
                    dblPin = Test.Pin;
                else
                    dblPin = TDSCDMA_Pin;

                #region BJ_1
                if (Program.Location == LocationList.BJ_1)
                {
                    Set_TD_Condition(Test);

                    _SRC_3G.SetFrequency(Test.FreqIn);
                    _SRC_3G.SetPower(dblPin + Test.LossIn);
                    _SRC_3G.SetOutput(Output.ON);

                    _MSR_3G.Config__TDSCDMA_ACP(Test.FreqOut, 5);
                    intDelay = Math.Max(intDelay_SigGen, intDelay_MXA);
                    util.Wait(intDelay * 1);

                    dblResult = _MSR_3G.Get_TDSCDMA_ACP_Result();

                    //ACP -1.6MHz
                    dblTestResult = dblResult[1];
                    dblReportResult = Math.Round(dblTestResult, 3);
                    this.UpdateGrid(Test.TestItem.ToString(), dblReportResult);
                    this.UpdateProductionTestResult(Test.TestItem.ToString(), dblReportResult);

                    //ACP +1.6MHz
                    dblTestResult = dblResult[2];
                    dblReportResult = Math.Round(dblTestResult, 3);
                    this.UpdateGrid(Program.ProductTest[index + 1].TestItem.ToString(), dblReportResult);
                    this.UpdateProductionTestResult(Program.ProductTest[index + 1].TestItem.ToString(), dblReportResult);

                    //ACP -3.2MHz
                    dblTestResult = dblResult[3];
                    dblReportResult = Math.Round(dblTestResult, 3);
                    this.UpdateGrid(Program.ProductTest[index + 2].TestItem.ToString(), dblReportResult);
                    this.UpdateProductionTestResult(Program.ProductTest[index + 2].TestItem.ToString(), dblReportResult);

                    //ACP +3.2MHz
                    dblTestResult = dblResult[4];
                    dblReportResult = Math.Round(dblTestResult, 3);
                    this.UpdateGrid(Program.ProductTest[index + 3].TestItem.ToString(), dblReportResult);
                    this.UpdateProductionTestResult(Program.ProductTest[index + 3].TestItem.ToString(), dblReportResult);

                }
                #endregion BJ_1

                else
                {
                    throw new Exception("Bad Location");
                }
            }
            #endregion -- ACP

            #region -- EVM
            if (Test.Description.ToUpper().Contains("EVM"))
            {
                double dblPin;

                if (TDSCDMA_Pin == -99)
                    dblPin = Test.Pin;
                else
                    dblPin = TDSCDMA_Pin;

                #region BJ_1
                if (Program.Location == LocationList.BJ_1)
                {
                    Set_TD_Condition(Test);

                    _SRC_3G.SetFrequency(Test.FreqIn);
                    _SRC_3G.SetPower(dblPin + Test.LossIn);
                    _SRC_3G.SetOutput(Output.ON);

                    _MSR_3G.Config__TDSCDMA_EVM(Test.FreqOut);
                    intDelay = Math.Max(intDelay_SigGen, intDelay_MXA);
                    util.Wait(intDelay * 1);

                    dblTestResult = _MSR_3G.Get_TDSCDMA_EVM_Result();

                    //EVM
                    dblReportResult = Math.Round(dblTestResult, 3);
                    this.UpdateGrid(Test.TestItem.ToString(), dblReportResult);
                    this.UpdateProductionTestResult(Test.TestItem.ToString(), dblReportResult);

                }
                #endregion BJ_1

                else
                {
                    throw new Exception("Bad Location");
                }
            }
            #endregion -- EVM

        }

        private void DeviceTest_TDD(ProductTest Test)
        {
            double dblTestResult = 0;
            double dblReportResult = 0;

            int intDelay = 60;

            //Seach for current Test Item
            int index = Array.IndexOf(Program.ProductTest, Test);

            #region -- Idle Current
            if (Test.Description.ToUpper().Contains("IDLE"))
            {
                double I_Vcc;

                #region BJ_1
                if (Program.Location == LocationList.BJ_1)
                {
                    Set_TD_Condition(Test);

                    _SRC_4G.SetOutput(Output.OFF);

                    util.Wait(intDelay);


                    //dblTestResult = I_Vcc = _PS_VCC.High_Current();
                    double[] dblTemp = _PS_VCC.Meas_Current_Trig(Triger_Source.Bus, 0.1, 25, 200, 0);
                    dblTestResult = I_Vcc = dblTemp[2];
                    dblReportResult = Math.Round((dblTestResult * 1000), 3);

                    //Idle Vbat
                    this.UpdateGrid(Program.ProductTest[index].TestItem.ToString(), 0.1);
                    this.UpdateProductionTestResult(Program.ProductTest[index].TestItem.ToString(), 0.1);

                    //Idle VCC
                    this.UpdateGrid(Program.ProductTest[index + 1].TestItem.ToString(), dblReportResult);
                    this.UpdateProductionTestResult(Program.ProductTest[index].TestItem.ToString(), dblReportResult);

                    //Idle Total
                    this.UpdateGrid(Program.ProductTest[index + 2].TestItem.ToString(), dblReportResult);
                    this.UpdateProductionTestResult(Program.ProductTest[index].TestItem.ToString(), dblReportResult);

                }
                #endregion BJ_1

                else
                {
                    throw new Exception("Bad Location");
                }
            }
            #endregion -- Idle Current

            #region -- Gain
            if (Test.Description.ToUpper().Contains("GAIN"))
            {
                double I_Vcc = 0.0;
                double I_Vbat = 0.0;
                double dblPout = 0.0;
                double dblPin = Test.Pin;
                double PoutLL = Test.Pout - 0.05;
                double PoutUL = Test.Pout + 0.05;
                TDD_Pin = dblPin;

                #region BJ_1
                if (Program.Location == LocationList.BJ_1)
                {
                    Set_TD_Condition(Test);

                    _SRC_4G.SetFrequency(Test.FreqIn);
                    _SRC_4G.SetPower(dblPin + Test.LossIn);

                    _SRC_4G.SetOutput(Output.ON);

                    _MSR_4G.Config__LTETDD_CHP(Test.FreqOut);                    
                    _MSR_4G.SetAttenuattor(10);
                    intDelay = Math.Max(intDelay_SigGen, intDelay_MXA);
                    util.Wait(intDelay);

                    dblPout = _MSR_4G.Get_LTETDD_CHP_Result();
                    dblPout += +Test.LossOut;
                    while (dblPout <= PoutLL || dblPout >= PoutUL)
                    {
                        dblPin = dblPin + Test.Pout - dblPout;
                        if (dblPin > 7) break;

                        _SRC_4G.SetPower(dblPin + Test.LossIn);
                        util.Wait(intDelay);
                        dblPout = _MSR_4G.Get_LTETDD_CHP_Result();
                        dblPout += +Test.LossOut;
                        TDD_Pin = dblPin;
                    }

                    //Pin
                    dblTestResult = TDD_Pin;
                    dblReportResult = Math.Round(dblTestResult, 3);
                    this.UpdateGrid(Test.TestItem.ToString(), dblReportResult);
                    this.UpdateProductionTestResult(Test.TestItem.ToString(), dblReportResult);

                    //Pout
                    dblTestResult = dblPout;
                    dblReportResult = Math.Round(dblTestResult, 3);
                    this.UpdateGrid(Program.ProductTest[index + 1].TestItem.ToString(), dblReportResult);
                    this.UpdateProductionTestResult(Program.ProductTest[index + 1].TestItem.ToString(), dblReportResult);

                    //Gain
                    dblTestResult = dblPout - TDD_Pin;
                    dblReportResult = Math.Round(dblTestResult, 3);
                    this.UpdateGrid(Program.ProductTest[index + 2].TestItem.ToString(), dblReportResult);
                    this.UpdateProductionTestResult(Program.ProductTest[index + 2].TestItem.ToString(), dblReportResult);

                    //Icc Vbat
                    this.UpdateGrid(Program.ProductTest[index + 3].TestItem.ToString(), 0.1);
                    this.UpdateProductionTestResult(Program.ProductTest[index + 3].TestItem.ToString(), 0.1);

                    //Icc Vcc
                    //dblTestResult = I_Vcc = _PS_VCC.High_Current();
                    double[] dblTemp = _PS_VCC.Meas_Current_Trig(Triger_Source.Bus, 0.1, 25, 200, 0);
                    dblTestResult = I_Vcc = dblTemp[2];
                    dblReportResult = Math.Round((dblTestResult * 1000), 3);
                    this.UpdateGrid(Program.ProductTest[index + 4].TestItem.ToString(), dblReportResult);
                    this.UpdateProductionTestResult(Program.ProductTest[index + 3].TestItem.ToString(), dblReportResult);

                    //Icc Total
                    this.UpdateGrid(Program.ProductTest[index + 5].TestItem.ToString(), dblReportResult);
                    this.UpdateProductionTestResult(Program.ProductTest[index + 3].TestItem.ToString(), dblReportResult);

                    //PAE
                    dblTestResult = Math.Pow(10, ((dblPout - 30) / 10)) / Test.VCC / (I_Vcc + I_Vbat);
                    dblReportResult = Math.Round((dblTestResult * 100), 3);
                    this.UpdateGrid(Program.ProductTest[index + 6].TestItem.ToString(), dblReportResult);
                    this.UpdateProductionTestResult(Program.ProductTest[index + 4].TestItem.ToString(), dblReportResult);

                    //_E4438C.SetModOutput(Output.ON);
                    util.Wait(1000);
                }
                #endregion BJ_1

                else
                {
                    throw new Exception("Bad Location");
                }
            }
            #endregion -- GAIN

            #region -- ACP
            if (Test.Description.ToUpper().Contains("ACP"))
            {
                double dblPin;
                double[] dblResult;

                if (TDD_Pin == -99)
                    dblPin = Test.Pin;
                else
                    dblPin = TDD_Pin;

                #region BJ_1
                if (Program.Location == LocationList.BJ_1)
                {

                    Set_TD_Condition(Test);

                    _SRC_4G.SetFrequency(Test.FreqIn);
                    _SRC_4G.SetPower(dblPin + Test.LossIn);
                    _SRC_4G.SetOutput(Output.ON);

                    _MSR_4G.Config__LTETDD_ACP_EULTRA(Test.FreqOut);
                    intDelay = Math.Max(intDelay_SigGen, intDelay_MXA);
                    util.Wait(intDelay * 1);

                    dblResult = _MSR_4G.Get_LTETDD_ACP_EULTRA_Result();

                    // EUTRA ACP
                    dblTestResult = dblResult[1];
                    dblReportResult = Math.Round(dblTestResult, 3);
                    this.UpdateGrid(Test.TestItem.ToString(), dblReportResult);
                    this.UpdateProductionTestResult(Test.TestItem.ToString(), dblReportResult);

                    _MSR_4G.Config__LTETDD_ACP_ULTRA(Test.FreqOut);
                    intDelay = Math.Max(intDelay_SigGen, intDelay_MXA);
                    util.Wait(intDelay * 1);

                    dblResult = _MSR_4G.Get_LTETDD_ACP_ULTRA_Result();

                    // UTRA ACP1
                    dblTestResult = dblResult[1];
                    dblReportResult = Math.Round(dblTestResult, 3);
                    this.UpdateGrid(Program.ProductTest[index + 1].TestItem.ToString(), dblReportResult);
                    this.UpdateProductionTestResult(Program.ProductTest[index + 1].TestItem.ToString(), dblReportResult);

                    // UTRA ACP2
                    dblTestResult = dblResult[3];
                    dblReportResult = Math.Round(dblTestResult, 3);
                    this.UpdateGrid(Program.ProductTest[index + 2].TestItem.ToString(), dblReportResult);
                    this.UpdateProductionTestResult(Program.ProductTest[index + 1].TestItem.ToString(), dblReportResult);
                }
                #endregion BJ_1

                else
                {
                    throw new Exception("Bad Location");
                }
            }
            #endregion -- ACP

            #region -- EVM
            if (Test.Description.ToUpper().Contains("EVM"))
            {
                double dblPin;

                if (TDD_Pin == -99)
                    dblPin = Test.Pin;
                else
                    dblPin = TDD_Pin;

                #region BJ_1
                if (Program.Location == LocationList.BJ_1)
                {
                    Set_TD_Condition(Test);

                    _SRC_4G.SetFrequency(Test.FreqIn);
                    _SRC_4G.SetPower(dblPin + Test.LossIn);
                    _SRC_4G.SetOutput(Output.ON);

                    _MSR_4G.Config__LTETDD_EVM(Test.FreqOut);
                    intDelay = Math.Max(intDelay_SigGen, intDelay_MXA);
                    util.Wait(intDelay * 1);

                    dblTestResult = _MSR_4G.Get_LTETDD_EVM_Result();

                    //EVM
                    dblReportResult = Math.Round(dblTestResult, 3);
                    this.UpdateGrid(Test.TestItem.ToString(), dblReportResult);
                    this.UpdateProductionTestResult(Test.TestItem.ToString(), dblReportResult);

                }
                #endregion BJ_1

                else
                {
                    throw new Exception("Bad Location");
                }
            }
            #endregion -- EVM
        }

        private void DeviceTest_FDD(ProductTest Test)
        {
            double dblTestResult = 0;
            double dblReportResult = 0;

            int intDelay = 60;

            //Seach for current Test Item
            int index = Array.IndexOf(Program.ProductTest, Test);

            #region -- Idle Current
            if (Test.Description.ToUpper().Contains("IDLE"))
            {
                double I_Vcc;
                double I_Vbat;

                #region BJ_1
                if (Program.Location == LocationList.BJ_1)
                {
                    Set_WCDMA_Condition(Test);

                    _SRC_4G.SetOutput(Output.OFF);

                    util.Wait(intDelay);


                    //Idle Vbat
                    dblTestResult = I_Vbat = _PS_VBAT.RMS_Current(PS_66319B_Channel.Channel_1);
                    if (Program.evb)
                        dblReportResult = Math.Round((dblTestResult * 1000), 3);
                    else
                        dblReportResult = Math.Round((dblTestResult * 1000), 3) + Test.SocketOffset;

                    //this.UpdateGrid(Test.TestItem.ToString(), dblReportResult);
                    //this.UpdateProductionTestResult(Test.TestItem.ToString(), dblReportResult);

                    this.UpdateGrid(Test.TestItem.ToString(), 0.1);
                    this.UpdateProductionTestResult(Test.TestItem.ToString(), 0.1);

                    //Idle VCC
                    //dblTestResult = I_Vcc = _PS_VCC.High_Current();
                    double[] dblTemp = _PS_VCC.Meas_Current_Trig(Triger_Source.Bus, 0.1, 25, 200, 0);
                    dblTestResult = I_Vcc = dblTemp[0];
                    if (Program.evb)
                        dblReportResult = Math.Round((dblTestResult * 1000), 3);
                    else
                        dblReportResult = Math.Round((dblTestResult * 1000), 3) + Program.ProductTest[index + 1].SocketOffset;
                    this.UpdateGrid(Program.ProductTest[index + 1].TestItem.ToString(), dblReportResult);
                    this.UpdateProductionTestResult(Program.ProductTest[index + 1].TestItem.ToString(), dblReportResult);


                    //Idle Total
                    this.UpdateGrid(Program.ProductTest[index + 2].TestItem.ToString(), dblReportResult);
                    this.UpdateProductionTestResult(Program.ProductTest[index + 2].TestItem.ToString(), dblReportResult);

                }
                #endregion BJ_1

                else
                {
                    throw new Exception("Bad Location");
                }
            }
            #endregion -- Idle Current

            #region -- Gain
            if (Test.Description.ToUpper().Contains("GAIN"))
            {
                double I_Vcc = 0.0;
                double I_Vbat = 0.0;
                double dblPout = 0.0;
                double dblPin = Test.Pin;
                double PoutLL = Test.Pout - 0.05;
                double PoutUL = Test.Pout + 0.05;
                LTE_Pin = dblPin;

                #region BJ_1
                if (Program.Location == LocationList.BJ_1)
                {
                    Set_WCDMA_Condition(Test);

                    _SRC_4G.SetFrequency(Test.FreqIn);
                    _SRC_4G.SetPower(dblPin + Test.LossIn);

                    _SRC_4G.SetOutput(Output.ON);

                    _MSR_4G.Config__LTEFDD_CHP(Test.FreqOut);
                    _MSR_4G.SetAttenuattor(10);
                    intDelay = Math.Max(intDelay_SigGen, intDelay_MXA);
                    util.Wait(intDelay);

                    dblPout = _MSR_4G.Get_LTEFDD_CHP_Result();
                    dblPout += +Test.LossOut;
                    while (dblPout <= PoutLL || dblPout >= PoutUL)
                    {
                        dblPin = dblPin + Test.Pout - dblPout;
                        if (dblPin > 7) break;

                        _SRC_4G.SetPower(dblPin + Test.LossIn);
                        util.Wait(intDelay);
                        dblPout = _MSR_4G.Get_LTEFDD_CHP_Result();
                        dblPout += +Test.LossOut;
                        LTE_Pin = dblPin;
                    }

                    //Pin
                    dblTestResult = LTE_Pin;
                    dblReportResult = Math.Round(dblTestResult, 3);
                    this.UpdateGrid(Test.TestItem.ToString(), dblReportResult);
                    this.UpdateProductionTestResult(Test.TestItem.ToString(), dblReportResult);

                    //Pout
                    dblTestResult = dblPout;
                    dblReportResult = Math.Round(dblTestResult, 3);
                    this.UpdateGrid(Program.ProductTest[index + 1].TestItem.ToString(), dblReportResult);
                    this.UpdateProductionTestResult(Program.ProductTest[index + 1].TestItem.ToString(), dblReportResult);

                    //Gain
                    dblTestResult = dblPout - LTE_Pin;
                    dblReportResult = Math.Round(dblTestResult, 3);
                    this.UpdateGrid(Program.ProductTest[index + 2].TestItem.ToString(), dblReportResult);
                    this.UpdateProductionTestResult(Program.ProductTest[index + 2].TestItem.ToString(), dblReportResult);

                    //Icc Vbat
                    this.UpdateGrid(Program.ProductTest[index + 3].TestItem.ToString(), 0.1);
                    this.UpdateProductionTestResult(Program.ProductTest[index + 3].TestItem.ToString(), 0.1);

                    //Icc Vcc
                    //dblTestResult = I_Vcc = _PS_VCC.High_Current();
                    double[] dblTemp = _PS_VCC.Meas_Current_Trig(Triger_Source.Bus, 0.1, 25, 200, 0);
                    dblTestResult = I_Vcc = dblTemp[2];
                    dblReportResult = Math.Round((dblTestResult * 1000), 3);
                    this.UpdateGrid(Program.ProductTest[index + 4].TestItem.ToString(), dblReportResult);
                    this.UpdateProductionTestResult(Program.ProductTest[index + 3].TestItem.ToString(), dblReportResult);

                    //Icc Total
                    this.UpdateGrid(Program.ProductTest[index + 5].TestItem.ToString(), dblReportResult);
                    this.UpdateProductionTestResult(Program.ProductTest[index + 3].TestItem.ToString(), dblReportResult);

                    //PAE
                    dblTestResult = Math.Pow(10, ((dblPout - 30) / 10)) / Test.VCC / (I_Vcc + I_Vbat);
                    dblReportResult = Math.Round((dblTestResult * 100), 3);
                    this.UpdateGrid(Program.ProductTest[index + 6].TestItem.ToString(), dblReportResult);
                    this.UpdateProductionTestResult(Program.ProductTest[index + 4].TestItem.ToString(), dblReportResult);

                    //_E4438C.SetModOutput(Output.ON);
                    util.Wait(1000);
                }
                #endregion BJ_1

                else
                {
                    throw new Exception("Bad Location");
                }
            }
            #endregion -- GAIN

            #region -- ACP
            if (Test.Description.ToUpper().Contains("ACP"))
            {
                double dblPin;
                double[] dblResult;

                if (LTE_Pin == -99)
                    dblPin = Test.Pin;
                else
                    dblPin = LTE_Pin;

                #region BJ_1
                if (Program.Location == LocationList.BJ_1)
                {

                    Set_WCDMA_Condition(Test);

                    _SRC_4G.SetFrequency(Test.FreqIn);
                    _SRC_4G.SetPower(dblPin + Test.LossIn);
                    _SRC_4G.SetOutput(Output.ON);

                    _MSR_4G.Config__LTEFDD_ACP_EULTRA(Test.FreqOut);
                    intDelay = Math.Max(intDelay_SigGen, intDelay_MXA);
                    util.Wait(intDelay * 1);

                    dblResult = _MSR_4G.Get_LTEFDD_ACP_EULTRA_Result();

                    // EUTRA ACP
                    dblTestResult = dblResult[1];
                    dblReportResult = Math.Round(dblTestResult, 3);
                    this.UpdateGrid(Test.TestItem.ToString(), dblReportResult);
                    this.UpdateProductionTestResult(Test.TestItem.ToString(), dblReportResult);

                    _MSR_4G.Config__LTEFDD_ACP_ULTRA(Test.FreqOut);
                    intDelay = Math.Max(intDelay_SigGen, intDelay_MXA);
                    util.Wait(intDelay * 1);

                    dblResult = _MSR_4G.Get_LTEFDD_ACP_ULTRA_Result();

                    // UTRA ACP1
                    dblTestResult = dblResult[1];
                    dblReportResult = Math.Round(dblTestResult, 3);
                    this.UpdateGrid(Program.ProductTest[index + 1].TestItem.ToString(), dblReportResult);
                    this.UpdateProductionTestResult(Program.ProductTest[index + 1].TestItem.ToString(), dblReportResult);

                    // UTRA ACP2
                    dblTestResult = dblResult[3];
                    dblReportResult = Math.Round(dblTestResult, 3);
                    this.UpdateGrid(Program.ProductTest[index + 2].TestItem.ToString(), dblReportResult);
                    this.UpdateProductionTestResult(Program.ProductTest[index + 1].TestItem.ToString(), dblReportResult);
                }
                #endregion BJ_1

                else
                {
                    throw new Exception("Bad Location");
                }
            }
            #endregion -- ACP

            #region -- EVM
            if (Test.Description.ToUpper().Contains("EVM"))
            {
                double dblPin;

                if (LTE_Pin == -99)
                    dblPin = Test.Pin;
                else
                    dblPin = LTE_Pin;

                #region BJ_1
                if (Program.Location == LocationList.BJ_1)
                {
                    Set_WCDMA_Condition(Test);

                    _SRC_4G.SetFrequency(Test.FreqIn);
                    _SRC_4G.SetPower(dblPin + Test.LossIn);
                    _SRC_4G.SetOutput(Output.ON);

                    _MSR_4G.Config__LTEFDD_EVM(Test.FreqOut);
                    intDelay = Math.Max(intDelay_SigGen, intDelay_MXA);
                    util.Wait(intDelay * 1);

                    dblTestResult = _MSR_4G.Get_LTEFDD_EVM_Result();

                    //EVM
                    dblReportResult = Math.Round(dblTestResult, 3);
                    this.UpdateGrid(Test.TestItem.ToString(), dblReportResult);
                    this.UpdateProductionTestResult(Test.TestItem.ToString(), dblReportResult);

                }
                #endregion BJ_1

                else
                {
                    throw new Exception("Bad Location");
                }
            }
            #endregion -- EVM
        }


    }
}
