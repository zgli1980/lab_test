///Reversion history log
///Rev1.0      Initial build                                                               AceLi       2012-08-09
///Rev1.1      Add TDHSDPA test                                                            AceLi       2013-01-21
///Rev1.2      Add BJ Sweep test                                                           AceLi       2013-04-24
///Rev1.3      Add LTE TDD test                                                            AceLi       2013-10-31
///Rev1.3      Add LTE CDMA EVDO test                                                      AceLi       2014-01-24
///Rev2.5      Add up to 13.6GHz Harmonic test(14fo for LB, 7fo for HB)                    AceLi       2014-06-11
///Rev2.6      Add Mipi                                                                    AceLi       2014-08-29
///Rev:2014-12-12      Add SH4 and change CDMA/EVDO to VSA measure, some error fix `       AceLi       2014-12-12
///Rev:2014-12-12      Fix CDMA/EVDO gain issue `                                          AceLi       2014-12-31
///Rev:2015-02-06      Add Mipi for All Mode change Mod trigger mode`                      AceLi       2015-02-06
///Rev:2015-04-23      Change GMSK power servo function                                    AceLi       2015-04-23
///Rev:2015-06-24      Fix SH1 Edge power issue                                            AceLi       2015-06-24
     

using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using Vanchip.Testing;
using Vanchip.Common;

namespace Bench_Test
{
    public partial class SweepTest : Form
    {
        #region --- Define Variable ---

        private static double dblMaxPout = 0;
        private static double dblRatedVramp = 0;

        private static Dictionary<double, double> dicVramp = new Dictionary<double, double>();

        private static double dblVbat_Current_Limit = 3;

        private static int LB_Test_Count;
        private static bool first_run = true;

        private static double dblPulse_Freq_Gmsk = TestSetting.ARB_PULSE_FREQ_GMSK_in_khz;
        private static double dblPulse_Freq_LTETDD = TestSetting.ARB_PULSE_FREQ_LTETDD_in_khz;
        //private static double dblPulse_Freq_Gmsk = TestSetting.ARB_PULSE_FREQ_GMSK_in_khz;

        private static int intDelay_SigGen = TestSetting.DELAY_SIGGEN_in_ms;
        private static int intDelay_PowerMeter = TestSetting.DELAY_POWER_METER_in_ms;
        private static int intDelay_MXA = TestSetting.DELAY_VSA_in_ms;
        private static int intDelay_PowerSupply = TestSetting.DELAY_POWER_SUPPLLY_in_ms;
        private static int intDelay_Arb = TestSetting.DELAY_ARB_in_ms;
        private static int intDelay_N1913A_Count = 4;


        private string[] Array_HBData_Row = new string[6];
        private string[] Array_LBData_Row = new string[6];
        private string[] Array_Limit = new string[100];
        private string[] Array_Limit_Row = new string[4];


        private DataTable dtCWHB = new DataTable();
        private DataTable dtCWLB = new DataTable();
        private DataTable dtEDGEHB = new DataTable();
        private DataTable dtEDGELB = new DataTable();
        private DataTable dtTDSCDMA = new DataTable();
        private DataTable dtWCDMA = new DataTable();
        private DataTable dtLTETDD_B38 = new DataTable();
        private DataTable dtLTETDD_B40 = new DataTable();
        private DataTable dtLTEFDD_B1 = new DataTable();
        private DataTable dtLTEFDD_B2 = new DataTable();
        private DataTable dtCDMA = new DataTable();
        private DataTable dtEVDO = new DataTable();

        private bool isDataSaved = false;
        private bool isCWHBTested = false;
        private bool isCWLBTested = false;
        private bool isEDGELBTested = false;
        private bool isEDGEHBTested = false;
        private bool isTDSCDMATested = false;
        private bool isWCDMATested = false;
        private bool isLTETDD_B38_Tested = false;
        private bool isLTETDD_B40_Tested = false;
        private bool isLTEFDD_B1_Tested = false;
        private bool isLTEFDD_B2_Tested = false;
        private bool isCDMA_Tested = false;
        private bool isEVDO_Tested = false;

        private bool td_hsdpa = false;
        private bool ext_har = false;
        private bool RX_Isolation = false;
        private bool SweepData = false;
        private bool StopTest = false;

        private string strProduct;
        private string strErr;
        private bool src_initialized = false;

        private NewLossComp LossComp = new NewLossComp();
        private Mipi frmMipi = new Mipi();
        Util util = new Util();

        E4438C _E4438C;
        HP8665B _HP8665B;

        Arb_33522A _Arb_33522A;
        Arb_33522A_USB _Arb_33522A_USB;
        Arb_33120A _Arb_33120A;
        Arb_33220A _Arb_33220A;

        PS_66332A _PS_66332A;
        PS_66319B _PS_66319B;
        PS_E3631A _PS_E3631A;

        PM_U2001A _PM_U2001A;
        PM_N1913A _PM_N1913A;
        MXA_N9020A _MXA_N9020A;

        #endregion *** Define variable ***

        public SweepTest()
        {
            InitializeComponent();
            this.FormClosing += new FormClosingEventHandler(SweepTest_FormClosing);
            this.dgvSweepResult.RowsAdded += new DataGridViewRowsAddedEventHandler(dgvSweepResult_RowsAdded);
            this.cbxWaveform.SelectedIndexChanged += new EventHandler(cbxWaveform_SelectedIndexChanged);

            #region Initialize  Datatable

            #region --- GMSK LB ---
            this.dtCWLB.Columns.Add(new DataColumn("#", typeof(int)));
            this.dtCWLB.Columns.Add(new DataColumn("Frequency(MHz)", typeof(double)));
            this.dtCWLB.Columns.Add(new DataColumn("Vramp(V)", typeof(string)));
            this.dtCWLB.Columns.Add(new DataColumn("Pout(dBm)", typeof(double)));
            this.dtCWLB.Columns.Add(new DataColumn("ICC(mA)", typeof(double)));
            this.dtCWLB.Columns.Add(new DataColumn("PAE(%)", typeof(double)));
            this.dtCWLB.Columns.Add(new DataColumn("2fo(dBm)", typeof(double)));
            this.dtCWLB.Columns.Add(new DataColumn("3fo(dBm)", typeof(double)));
            this.dtCWLB.Columns.Add(new DataColumn("4fo(dBm)", typeof(double)));
            this.dtCWLB.Columns.Add(new DataColumn("5fo(dBm)", typeof(double)));
            this.dtCWLB.Columns.Add(new DataColumn("6fo(dBm)", typeof(double)));
            this.dtCWLB.Columns.Add(new DataColumn("7fo(dBm)", typeof(double)));
            this.dtCWLB.Columns.Add(new DataColumn("8fo(dBm)", typeof(double)));
            this.dtCWLB.Columns.Add(new DataColumn("9fo(dBm)", typeof(double)));
            this.dtCWLB.Columns.Add(new DataColumn("10fo(dBm)", typeof(double)));
            this.dtCWLB.Columns.Add(new DataColumn("11fo(dBm)", typeof(double)));
            this.dtCWLB.Columns.Add(new DataColumn("12fo(dBm)", typeof(double)));
            this.dtCWLB.Columns.Add(new DataColumn("13fo(dBm)", typeof(double)));
            this.dtCWLB.Columns.Add(new DataColumn("14fo(dBm)", typeof(double)));
            //this.dtCWLB.PrimaryKey = new DataColumn[] { dtCWLB.Columns["#"] };
            //this.dtCWLBData.Columns["#"].AutoIncrement = true;
            //this.dtCWLBData.Columns["#"].AutoIncrementSeed = 1;
            //this.dtCWLBData.Columns["#"].AutoIncrementStep = 1;
            #endregion --- GMSK LB ---

            #region --- GMSK HB ---
            this.dtCWHB.Columns.Add(new DataColumn("#", typeof(int)));
            this.dtCWHB.Columns.Add(new DataColumn("Frequency(MHz)", typeof(double)));
            this.dtCWHB.Columns.Add(new DataColumn("Vramp(V)", typeof(string)));
            this.dtCWHB.Columns.Add(new DataColumn("Pout(dBm)", typeof(double)));
            this.dtCWHB.Columns.Add(new DataColumn("ICC(mA)", typeof(double)));
            this.dtCWHB.Columns.Add(new DataColumn("PAE(%)", typeof(double)));
            this.dtCWHB.Columns.Add(new DataColumn("2fo(dBm)", typeof(double)));
            this.dtCWHB.Columns.Add(new DataColumn("3fo(dBm)", typeof(double)));
            this.dtCWHB.Columns.Add(new DataColumn("4fo(dBm)", typeof(double)));
            this.dtCWHB.Columns.Add(new DataColumn("5fo(dBm)", typeof(double)));
            this.dtCWHB.Columns.Add(new DataColumn("6fo(dBm)", typeof(double)));
            this.dtCWHB.Columns.Add(new DataColumn("7fo(dBm)", typeof(double)));
            //this.dtCWHB.PrimaryKey = new DataColumn[] { dtCWHB.Columns["#"] };
            //this.dtCWHBData.Columns["#"].AutoIncrement = true;
            //this.dtCWHBData.Columns["#"].AutoIncrementSeed = 1;
            //this.dtCWHBData.Columns["#"].AutoIncrementStep = 1;
            #endregion --- GMSK HB ---

            #region --- EDGE LB ---

            this.dtEDGELB.Columns.Add(new DataColumn("#", typeof(int)));
            this.dtEDGELB.Columns.Add(new DataColumn("Frequency (MHz)", typeof(double)));
            this.dtEDGELB.Columns.Add(new DataColumn("Target Pout(dBm)", typeof(double)));
            this.dtEDGELB.Columns.Add(new DataColumn("Pout(dBm)", typeof(double)));
            this.dtEDGELB.Columns.Add(new DataColumn("Pin(dBm)", typeof(double)));
            this.dtEDGELB.Columns.Add(new DataColumn("Gain(dB)", typeof(double)));
            this.dtEDGELB.Columns.Add(new DataColumn("Icc(mA)", typeof(double)));
            this.dtEDGELB.Columns.Add(new DataColumn("PAE(%)", typeof(double)));
            this.dtEDGELB.Columns.Add(new DataColumn("ACP -400kHz(dB)", typeof(double)));
            this.dtEDGELB.Columns.Add(new DataColumn("ACP +400kHz(dB)", typeof(double)));
            this.dtEDGELB.Columns.Add(new DataColumn("EVM(%)", typeof(double)));


            #endregion --- EDGE LB ---

            #region --- EDGE HB ---

            this.dtEDGEHB = this.dtEDGELB.Clone();

            //this.dtEDGEHB.Columns.Add(new DataColumn("#", typeof(int)));
            //this.dtEDGEHB.Columns.Add(new DataColumn("Frequency (MHz)", typeof(double)));
            //this.dtEDGEHB.Columns.Add(new DataColumn("Target Pout(dBm)", typeof(double)));
            //this.dtEDGEHB.Columns.Add(new DataColumn("Pout(dBm)", typeof(double)));
            //this.dtEDGEHB.Columns.Add(new DataColumn("Pin(dBm)", typeof(double)));
            //this.dtEDGEHB.Columns.Add(new DataColumn("Gain(dB)", typeof(double)));
            //this.dtEDGEHB.Columns.Add(new DataColumn("Icc(mA)", typeof(double)));
            //this.dtEDGEHB.Columns.Add(new DataColumn("PAE(%)", typeof(double)));
            //this.dtEDGEHB.Columns.Add(new DataColumn("ACP -400kHz(dB)", typeof(double)));
            //this.dtEDGEHB.Columns.Add(new DataColumn("ACP +400kHz(dB)", typeof(double)));
            //this.dtEDGEHB.Columns.Add(new DataColumn("EVM(%)", typeof(double)));

            #endregion --- EDGE HB ---

            #region --- TDSCDMA ---

            this.dtTDSCDMA.Columns.Add(new DataColumn("#", typeof(int)));
            this.dtTDSCDMA.Columns.Add(new DataColumn("Frequency (MHz)", typeof(double)));
            this.dtTDSCDMA.Columns.Add(new DataColumn("Target Pout(dBm)", typeof(double)));
            this.dtTDSCDMA.Columns.Add(new DataColumn("Pout(dBm)", typeof(double)));
            this.dtTDSCDMA.Columns.Add(new DataColumn("Pin(dBm)", typeof(double)));
            this.dtTDSCDMA.Columns.Add(new DataColumn("Gain(dB)", typeof(double)));
            this.dtTDSCDMA.Columns.Add(new DataColumn("Icc(mA)", typeof(double)));
            this.dtTDSCDMA.Columns.Add(new DataColumn("PAE(%)", typeof(double)));
            this.dtTDSCDMA.Columns.Add(new DataColumn("ACP -1.6MHz(dB)", typeof(double)));
            this.dtTDSCDMA.Columns.Add(new DataColumn("ACP +1.6MHz(dB)", typeof(double)));
            this.dtTDSCDMA.Columns.Add(new DataColumn("ACP -3.2MHz(dB)", typeof(double)));
            this.dtTDSCDMA.Columns.Add(new DataColumn("ACP +3.2MHz(dB)", typeof(double)));
            this.dtTDSCDMA.Columns.Add(new DataColumn("EVM(%)", typeof(double)));
            //this.dtWCDMA.PrimaryKey = new DataColumn[] { dtWCDMA.Columns["#"] };

            #endregion --- TDSCDMA ---

            #region --- WCDMA ---

            this.dtWCDMA.Columns.Add(new DataColumn("#", typeof(int)));
            this.dtWCDMA.Columns.Add(new DataColumn("Frequency (MHz)", typeof(double)));
            this.dtWCDMA.Columns.Add(new DataColumn("Target Pout(dBm)", typeof(double)));
            this.dtWCDMA.Columns.Add(new DataColumn("Pout(dBm)", typeof(double)));
            this.dtWCDMA.Columns.Add(new DataColumn("Pin(dBm)", typeof(double)));
            this.dtWCDMA.Columns.Add(new DataColumn("Gain(dB)", typeof(double)));
            this.dtWCDMA.Columns.Add(new DataColumn("Icc(mA)", typeof(double)));
            this.dtWCDMA.Columns.Add(new DataColumn("PAE(%)", typeof(double)));
            this.dtWCDMA.Columns.Add(new DataColumn("ACP -5MHz(dB)", typeof(double)));
            this.dtWCDMA.Columns.Add(new DataColumn("ACP +5MHz(dB)", typeof(double)));
            this.dtWCDMA.Columns.Add(new DataColumn("ACP -10MHz(dB)", typeof(double)));
            this.dtWCDMA.Columns.Add(new DataColumn("ACP +10MHz(dB)", typeof(double)));
            this.dtWCDMA.Columns.Add(new DataColumn("EVM(%)", typeof(double)));
            //this.dtWCDMA.PrimaryKey = new DataColumn[] { dtWCDMA.Columns["#"] };

            #endregion --- WCDMA ---

            #region --- LTETDD_B38 ---

            this.dtLTETDD_B38.Columns.Add(new DataColumn("#", typeof(int)));
            this.dtLTETDD_B38.Columns.Add(new DataColumn("Frequency (MHz)", typeof(double)));
            this.dtLTETDD_B38.Columns.Add(new DataColumn("Target Pout(dBm)", typeof(double)));
            this.dtLTETDD_B38.Columns.Add(new DataColumn("Pout(dBm)", typeof(double)));
            this.dtLTETDD_B38.Columns.Add(new DataColumn("Pin(dBm)", typeof(double)));
            this.dtLTETDD_B38.Columns.Add(new DataColumn("Gain(dB)", typeof(double)));
            this.dtLTETDD_B38.Columns.Add(new DataColumn("Icc(mA)", typeof(double)));
            this.dtLTETDD_B38.Columns.Add(new DataColumn("PAE(%)", typeof(double)));
            this.dtLTETDD_B38.Columns.Add(new DataColumn("ACP_EULTRA -10MHz(dB)", typeof(double)));
            this.dtLTETDD_B38.Columns.Add(new DataColumn("ACP_EULTRA +10MHz(dB)", typeof(double)));
            this.dtLTETDD_B38.Columns.Add(new DataColumn("ACP_ULTRA -0.8MHz(dB)", typeof(double)));
            this.dtLTETDD_B38.Columns.Add(new DataColumn("ACP_ULTRA +0.8MHz(dB)", typeof(double)));
            this.dtLTETDD_B38.Columns.Add(new DataColumn("ACP_ULTRA -2.4MHz(dB)", typeof(double)));
            this.dtLTETDD_B38.Columns.Add(new DataColumn("ACP_ULTRA +2.4MHz(dB)", typeof(double)));
            this.dtLTETDD_B38.Columns.Add(new DataColumn("EVM(%)", typeof(double)));
            //this.dtWCDMA.PrimaryKey = new DataColumn[] { dtWCDMA.Columns["#"] };

            #endregion 

            #region --- LTETDD_B40 ---
            this.dtLTETDD_B40 = this.dtLTETDD_B38.Clone();

            //this.dtLTETDD_B40.Columns.Add(new DataColumn("#", typeof(int)));
            //this.dtLTETDD_B40.Columns.Add(new DataColumn("Frequency (MHz)", typeof(double)));
            //this.dtLTETDD_B40.Columns.Add(new DataColumn("Target Pout(dBm)", typeof(double)));
            //this.dtLTETDD_B40.Columns.Add(new DataColumn("Pout(dBm)", typeof(double)));
            //this.dtLTETDD_B40.Columns.Add(new DataColumn("Pin(dBm)", typeof(double)));
            //this.dtLTETDD_B40.Columns.Add(new DataColumn("Gain(dB)", typeof(double)));
            //this.dtLTETDD_B40.Columns.Add(new DataColumn("Icc(mA)", typeof(double)));
            //this.dtLTETDD_B40.Columns.Add(new DataColumn("PAE(%)", typeof(double)));
            //this.dtLTETDD_B40.Columns.Add(new DataColumn("ACP_EULTRA -10MHz(dB)", typeof(double)));
            //this.dtLTETDD_B40.Columns.Add(new DataColumn("ACP_EULTRA +10MHz(dB)", typeof(double)));
            //this.dtLTETDD_B40.Columns.Add(new DataColumn("ACP_ULTRA -0.8MHz(dB)", typeof(double)));
            //this.dtLTETDD_B40.Columns.Add(new DataColumn("ACP_ULTRA +0.8MHz(dB)", typeof(double)));
            //this.dtLTETDD_B40.Columns.Add(new DataColumn("ACP_ULTRA -2.4MHz(dB)", typeof(double)));
            //this.dtLTETDD_B40.Columns.Add(new DataColumn("ACP_ULTRA +2.4MHz(dB)", typeof(double)));
            //this.dtLTETDD_B40.Columns.Add(new DataColumn("EVM(%)", typeof(double)));
            ////this.dtWCDMA.PrimaryKey = new DataColumn[] { dtWCDMA.Columns["#"] };

            #endregion --- LTETDD_B40 ---

            #region --- LTEFDD_B1 ---

            this.dtLTEFDD_B1.Columns.Add(new DataColumn("#", typeof(int)));
            this.dtLTEFDD_B1.Columns.Add(new DataColumn("Frequency (MHz)", typeof(double)));
            this.dtLTEFDD_B1.Columns.Add(new DataColumn("Target Pout(dBm)", typeof(double)));
            this.dtLTEFDD_B1.Columns.Add(new DataColumn("Pout(dBm)", typeof(double)));
            this.dtLTEFDD_B1.Columns.Add(new DataColumn("Pin(dBm)", typeof(double)));
            this.dtLTEFDD_B1.Columns.Add(new DataColumn("Gain(dB)", typeof(double)));
            this.dtLTEFDD_B1.Columns.Add(new DataColumn("Icc(mA)", typeof(double)));
            this.dtLTEFDD_B1.Columns.Add(new DataColumn("PAE(%)", typeof(double)));
            this.dtLTEFDD_B1.Columns.Add(new DataColumn("ACP_EULTRA -10MHz(dB)", typeof(double)));
            this.dtLTEFDD_B1.Columns.Add(new DataColumn("ACP_EULTRA +10MHz(dB)", typeof(double)));
            this.dtLTEFDD_B1.Columns.Add(new DataColumn("ACP_ULTRA -2.5MHz(dB)", typeof(double)));
            this.dtLTEFDD_B1.Columns.Add(new DataColumn("ACP_ULTRA +2.5MHz(dB)", typeof(double)));
            this.dtLTEFDD_B1.Columns.Add(new DataColumn("ACP_ULTRA -7.5MHz(dB)", typeof(double)));
            this.dtLTEFDD_B1.Columns.Add(new DataColumn("ACP_ULTRA +7.5MHz(dB)", typeof(double)));
            this.dtLTEFDD_B1.Columns.Add(new DataColumn("EVM(%)", typeof(double)));
            //this.dtWCDMA.PrimaryKey = new DataColumn[] { dtWCDMA.Columns["#"] };

            #endregion --- LTEFDD_B1 ---

            #region --- LTEFDD_B2 ---

            this.dtLTEFDD_B2.Columns.Add(new DataColumn("#", typeof(int)));
            this.dtLTEFDD_B2.Columns.Add(new DataColumn("Frequency (MHz)", typeof(double)));
            this.dtLTEFDD_B2.Columns.Add(new DataColumn("Target Pout(dBm)", typeof(double)));
            this.dtLTEFDD_B2.Columns.Add(new DataColumn("Pout(dBm)", typeof(double)));
            this.dtLTEFDD_B2.Columns.Add(new DataColumn("Pin(dBm)", typeof(double)));
            this.dtLTEFDD_B2.Columns.Add(new DataColumn("Gain(dB)", typeof(double)));
            this.dtLTEFDD_B2.Columns.Add(new DataColumn("Icc(mA)", typeof(double)));
            this.dtLTEFDD_B2.Columns.Add(new DataColumn("PAE(%)", typeof(double)));
            this.dtLTEFDD_B2.Columns.Add(new DataColumn("ACP_EULTRA -10MHz(dB)", typeof(double)));
            this.dtLTEFDD_B2.Columns.Add(new DataColumn("ACP_EULTRA +10MHz(dB)", typeof(double)));
            this.dtLTEFDD_B2.Columns.Add(new DataColumn("ACP_ULTRA -2.5MHz(dB)", typeof(double)));
            this.dtLTEFDD_B2.Columns.Add(new DataColumn("ACP_ULTRA +2.5MHz(dB)", typeof(double)));
            this.dtLTEFDD_B2.Columns.Add(new DataColumn("ACP_ULTRA -7.5MHz(dB)", typeof(double)));
            this.dtLTEFDD_B2.Columns.Add(new DataColumn("ACP_ULTRA +7.5MHz(dB)", typeof(double)));
            this.dtLTEFDD_B2.Columns.Add(new DataColumn("EVM(%)", typeof(double)));
            //this.dtWCDMA.PrimaryKey = new DataColumn[] { dtWCDMA.Columns["#"] };

            #endregion --- LTEFDD_B2 ---

            #region --- CDMA & EVDO ---

            dtCDMA = dtWCDMA.Clone();
            //dtCDMA.Columns[8].ColumnName = "ACP -1.6MHz(dB)";
            dtEVDO = dtWCDMA.Clone();

            #endregion --- CDMA & EVDO ---

            #endregion Initialize  Datatable

            #region Intialize Radio Button
            foreach (Control control in this.gbMode.Controls)
            {
                RadioButton radioButton = control as RadioButton;

                if (radioButton != null)
                {
                    radioButton.CheckedChanged += new EventHandler(radioButton_CheckedChanged);
                    radioButton.Enabled = false;
                }
            }
            rbnCWLB.Text = TestSetting.MODE_CW_LB;
            rbnCWHB.Text = TestSetting.MODE_CW_HB;
            rbnEDGELB.Text = TestSetting.MODE_EDGE_LB;
            rbnEDGEHB.Text = TestSetting.MODE_EDGE_HB;
            rbnTDSCDMA.Text = TestSetting.MODE_TDSCDMA;
            rbnWCDMA.Text = TestSetting.MODE_WCDMA;
            rbnLTETDD_B38.Text = TestSetting.MODE_LTETDD_B38;
            rbnLTETDD_B40.Text = TestSetting.MODE_LTETDD_B40;
            rbnLTEFDDLB.Text = TestSetting.MODE_LTEFDD_LB;
            rbnLTEFDDHB.Text = TestSetting.MODE_LTEFDD_HB;
            rbnCDMA.Text = TestSetting.MODE_CDMA;
            rbnEVDo.Text = TestSetting.MODE_EVDO;

            rbnCWLB.Checked = true;

            #endregion Intialize Radio Button

            #region Intialize Setting TextBox Change
            foreach (Control control in this.gpParameter.Controls)
            {
                TextBox textBox = control as TextBox;

                if (textBox != null)
                {
                    //textBox.TextChanged += new EventHandler(textBox_TextChanged);
                    textBox.LostFocus += new EventHandler(textBox_LostFocus);
                    textBox.KeyDown += new KeyEventHandler(textBox_KeyDown);
                }
            }
            #endregion Intialize Setting TextBox Change

            #region Initialize instruments

            try
            {
                if (Program.Location == LocationList.BJ_1)
                {
                    _PS_66332A = new Vanchip.Testing.PS_66332A(Instruments_address._05);

                    _Arb_33522A_USB = new Arb_33522A_USB(Instruments_VISA.Arb_33522A);
                    _Arb_33220A = new Vanchip.Testing.Arb_33220A(Instruments_address._16);

                    _PM_U2001A = new Vanchip.Testing.PM_U2001A();
                    _MXA_N9020A = new Vanchip.Testing.MXA_N9020A(Instruments_address._22);
                    _E4438C = new Vanchip.Testing.E4438C(Instruments_address._12);

                    //Initialize
                    _E4438C.Initialize();
                    _Arb_33220A.Initialize();
                    _Arb_33522A_USB.Initialize(dblPulse_Freq_Gmsk);  //208.5KHz  

                    _PS_66332A.Initialize();

                    _PM_U2001A.Initialize();
                    _MXA_N9020A.Initialize(rbnDisplayON.Checked);
                }
                else if (Program.Location == LocationList.SH_1)
                {
                    _PS_66332A = new PS_66332A(Instruments_address._05);

                    _Arb_33522A_USB = new Arb_33522A_USB(Instruments_VISA.Arb_33522A);
                    _Arb_33220A = new Arb_33220A(Instruments_address._11);

                    _PM_N1913A = new PM_N1913A(Instruments_address._15);
                    _MXA_N9020A = new MXA_N9020A(Instruments_address._18);
                    _E4438C = new Vanchip.Testing.E4438C(Instruments_address._19);

                    _E4438C.Initialize();
                    _Arb_33220A.Initialize();
                    _Arb_33522A_USB.Initialize(dblPulse_Freq_Gmsk);  //208.5KHz

                    _PS_66332A.Initialize();

                    _PM_N1913A.Initialize(rbnDisplayON.Checked);
                    _MXA_N9020A.Initialize(rbnDisplayON.Checked);
                }
                else if (Program.Location == LocationList.SH_2)
                {
                    _PS_66332A = new PS_66332A(Instruments_address._05);
                    _PS_66319B = new PS_66319B(Instruments_address._05);
                    _Arb_33522A = new Arb_33522A(Instruments_address._10);

                    _PM_N1913A = new PM_N1913A(Instruments_address._15);
                    _MXA_N9020A = new MXA_N9020A(Instruments_address._18);
                    _E4438C = new Vanchip.Testing.E4438C(Instruments_address._19);

                    _E4438C.Initialize();
                    _Arb_33522A.Initialize(dblPulse_Freq_Gmsk);  //208.5KHz

                    _PS_66332A.Initialize();
                    _PS_66319B.Initialize();

                    _PM_N1913A.Initialize(rbnDisplayON.Checked);
                    _MXA_N9020A.Initialize(rbnDisplayON.Checked);
                }
                else if (Program.Location == LocationList.SH_3 ||
                                 Program.Location == LocationList.SH_4)
                {
                    _PS_66332A = new PS_66332A(Instruments_address._05);
                    _PS_66319B = new PS_66319B(Instruments_address._05);

                    _Arb_33522A = new Arb_33522A(Instruments_address._10);
                    _Arb_33220A = new Arb_33220A(Instruments_address._11);

                    _PM_N1913A = new PM_N1913A(Instruments_address._15);
                    _MXA_N9020A = new MXA_N9020A(Instruments_address._18);
                    _E4438C = new Vanchip.Testing.E4438C(Instruments_address._19);

                    _E4438C.Initialize();
                    _Arb_33220A.Initialize();
                    _Arb_33522A.Initialize(dblPulse_Freq_Gmsk);  //208.5KHz

                    _PS_66332A.Initialize();
                    _PS_66319B.Initialize();

                    _PM_N1913A.Initialize(rbnDisplayON.Checked);
                    _MXA_N9020A.Initialize(rbnDisplayON.Checked);
                }
                else if (Program.Location == LocationList.Simulation)
                {
                    this.Text = this.Text + "  --- Simulation ";
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
  
        private void SweepTest_Load(object sender, EventArgs e)
        {
            lblCopyRight.Text = "Copyright @ 2011 Vanchip Rev:2015-06-24";
            this.Text = "SweepTest *** " + Program.Location.ToString();

            #region *** Read Loss Comp Data ***
            int LossCompIndentify = 5;
            string line;
            string[] content;
            FileInfo fileLossComp = new FileInfo(Program.strSweepCableLoss_FileName);

            if (!fileLossComp.Exists)
            {
                MessageBox.Show("Cable loss file is not exist in " + Program.strSweepCableLoss_FileName
                                    + " \r\nPlease restart the test program ans perform the Loss Compensation",
                                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }

            StreamReader srLossComp = new StreamReader(Program.strSweepCableLoss_FileName);
            //Last Cal Date
            line = srLossComp.ReadLine();
            content = line.Split(',');
            Program.lastCalDate = content[0];

            while (line != null)
            {
                #region Power meter measure loss data
                if (line.Contains("--- Power Meter Measure ---"))
                {
                    while (!(line = srLossComp.ReadLine()).Contains("---"))
                    {
                        content = line.Split(',');
                        TestSetting.LOSS_MSR_POUT[double.Parse(content[0])] = double.Parse(content[1]);
                    }
                    LossCompIndentify--;
                }
                #endregion

                #region  VSA through measure loss data
                if (line.Contains("--- VSA Through Measure ---"))
                {
                    while (!(line = srLossComp.ReadLine()).Contains("---"))
                    {
                        content = line.Split(',');
                        TestSetting.LOSS_MSR_THROUGH[double.Parse(content[0])] = double.Parse(content[1]);
                    }
                    LossCompIndentify--;
                }
                #endregion

                #region VSA LB filter measure loss data
                if (line.Contains("--- VSA LB Filter Measure ---"))
                {
                    while (!(line = srLossComp.ReadLine()).Contains("---"))
                    {
                        content = line.Split(',');
                        TestSetting.LOSS_MSR_FILTER_LB[double.Parse(content[0])] = double.Parse(content[1]);
                    }
                    LossCompIndentify--;
                }
                #endregion

                #region VSA HB filter measure loss data
                if (line.Contains("--- VSA HB Filter Measure ---"))
                {
                    while (!(line = srLossComp.ReadLine()).Contains("---"))
                    {
                        content = line.Split(',');
                        TestSetting.LOSS_MSR_FILTER_HB[double.Parse(content[0])] = double.Parse(content[1]);
                    }
                    LossCompIndentify--;
                }
                #endregion

                #region  Source loss data
                if (line.Contains("--- Source ---"))
                {
                    while (!(line = srLossComp.ReadLine()).Contains("---"))
                    {
                        content = line.Split(',');
                        TestSetting.LOSS_SRC[double.Parse(content[0])] = double.Parse(content[1]);
                    }
                    LossCompIndentify--;
                }
                #endregion

                if (!line.Contains("---"))
                    line = srLossComp.ReadLine();
                else if (line.Contains("--- The End ---"))
                    break;
            }

            srLossComp.Close();

            if (LossCompIndentify != 0)
            {
                MessageBox.Show("Cable loss file is interrupt in " + Program.strSweepCableLoss_FileName
                                    + " \r\nPlease restart the test program ans perform the Loss Compensation",
                                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }

            #endregion *** Read Loss Comp Data ***

            #region *** Control Enable ***

            #region BJ_1
            if (Program.Location == LocationList.BJ_1)
            {
                rbnCWLB.Enabled = true;
                rbnCWHB.Enabled = true;
                rbnEDGELB.Enabled = true;
                rbnEDGEHB.Enabled = true;
                rbnTDSCDMA.Enabled = true;
                rbnWCDMA.Enabled = true;
            }
            #endregion BJ_1

            #region SH_1
            else if (Program.Location == LocationList.SH_1)
            {
                rbnCWLB.Enabled = true;
                rbnCWHB.Enabled = true;
                rbnEDGELB.Enabled = true;
                rbnEDGEHB.Enabled = true;
                rbnTDSCDMA.Enabled = true;

            }
            #endregion SH1

            #region SH_2
            else if (Program.Location == LocationList.SH_2)
            {
                MessageBox.Show(this, "Make sure DG1022 is set to DC 1.2V", "Loading Program", MessageBoxButtons.OK);

                rbnCWLB.Enabled = true;
                rbnCWHB.Enabled = true;
                rbnEDGELB.Enabled = true;
                rbnEDGEHB.Enabled = true;
                rbnTDSCDMA.Enabled = true;
                rbnWCDMA.Enabled = true;
                rbnCDMA.Enabled = true;
                rbnEVDo.Enabled = true;
                rbnLTETDD_B38.Enabled = true;
                rbnLTETDD_B40.Enabled = true;
                rbnLTEFDDLB.Enabled = true;
                rbnLTEFDDHB.Enabled = true;

            }
            #endregion SH_2

            #region SH_3
            else if (Program.Location == LocationList.SH_3)
            {
                rbnCWLB.Enabled = true;
                rbnCWHB.Enabled = true;
                rbnLTETDD_B38.Enabled = true;
                rbnLTETDD_B40.Enabled = true;
                rbnLTEFDDLB.Enabled = true;
                rbnLTEFDDHB.Enabled = true;
                //rbnCDMA.Enabled = true;
                //rbnEVDo.Enabled = true;
                rbnWCDMA.Enabled = true;

            }
            #endregion SH_3

            #region SH_4
            else if (Program.Location == LocationList.SH_4)
            {
                rbnCWLB.Enabled = true;
                rbnCWHB.Enabled = true;
                rbnEDGELB.Enabled = true;
                rbnEDGEHB.Enabled = true;
                rbnTDSCDMA.Enabled = true;
                rbnWCDMA.Enabled = true;
                rbnCDMA.Enabled = true;
                rbnEVDo.Enabled = true;
                rbnLTETDD_B38.Enabled = true;
                rbnLTETDD_B40.Enabled = true;
                rbnLTEFDDLB.Enabled = true;
                rbnLTEFDDHB.Enabled = true;

            }
            #endregion SH_3
            else
            {
                throw new Exception("Bad Location");
            }
            #endregion *** Control Enable ***

        }


        private void dgvSweepResult_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            if (e.RowIndex > 10)
            {
                //dgvLossResult.Rows[e.RowIndex].Selected = true;
                dgvSweepResult.FirstDisplayedScrollingRowIndex = e.RowIndex;
            }
        }

        private void textBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;

            TextBox tbxParameter = sender as TextBox;
            foreach (Control cl in this.gpParameter.Controls)
            {
                if (cl.TabIndex == tbxParameter.TabIndex + 1)
                    cl.Focus();
            }
        }
        private void textBox_LostFocus(object sender, EventArgs e)
        {
            TextBox tbxParameter = sender as TextBox;
            try
            {
                if (tbxParameter.Name == "tbxVCC")
                    TestSetting.LEVEL_VCC = double.Parse(tbxParameter.Text);
                else if (tbxParameter.Name == "tbxVBAT")
                    TestSetting.LEVEL_VBAT = double.Parse(tbxParameter.Text);
                else if (tbxParameter.Name == "tbxTXEN")
                    TestSetting.LEVEL_TXEN = double.Parse(tbxParameter.Text);
                else if (tbxParameter.Name == "tbxGPCTRL")
                    TestSetting.LEVEL_GPCTRL = double.Parse(tbxParameter.Text);
                else if (tbxParameter.Name == "tbxStart")
                    TestSetting.LEVEL_START = double.Parse(tbxParameter.Text);
                else if (tbxParameter.Name == "tbxStep")
                    TestSetting.LEVEL_STEP = double.Parse(tbxParameter.Text);
                else if (tbxParameter.Name == "tbxStop")
                    TestSetting.LEVEL_STOP = double.Parse(tbxParameter.Text);
                else if (tbxParameter.Name == "tbxPin_Vramp")
                    TestSetting.LEVEL_PIN_VRAMP = double.Parse(tbxParameter.Text);
                else if (tbxParameter.Name == "tbxFreqList")
                {
                    string[] content = tbxParameter.Text.Split(',');
                    double[] tmp = new double[content.Count()];
                    for (int i = 0; i < content.Count(); i++)
                    {
                        tmp[i] = double.Parse(content[i]);
                    }
                    TestSetting.FREQLIST = new List<double>(tmp);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "oops", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void cbxWaveform_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxWaveform.SelectedItem.ToString() == Mod_Waveform_Name.LTETDD_FULL)
            {
                this.dgvSweepResult.Columns["ACP_EULTRA +10MHz(dB)"].Visible = true;
                this.dgvSweepResult.Columns["ACP_ULTRA +0.8MHz(dB)"].Visible = true;
                this.dgvSweepResult.Columns["ACP_ULTRA +2.4MHz(dB)"].Visible = true;
                this.dgvSweepResult.ScrollBars = ScrollBars.Both;
            }
            else if (cbxWaveform.SelectedItem.ToString() == Mod_Waveform_Name.LTETDD)
            {
                this.dgvSweepResult.Columns["ACP_EULTRA +10MHz(dB)"].Visible = false;
                this.dgvSweepResult.Columns["ACP_ULTRA +0.8MHz(dB)"].Visible = false;
                this.dgvSweepResult.Columns["ACP_ULTRA +2.4MHz(dB)"].Visible = false;
                this.dgvSweepResult.ScrollBars = ScrollBars.Vertical;
            }
            else if (cbxWaveform.SelectedItem.ToString() == Mod_Waveform_Name.LTEFDD_FULL)
            {
                this.dgvSweepResult.Columns["ACP_EULTRA +10MHz(dB)"].Visible = true;
                this.dgvSweepResult.Columns["ACP_ULTRA +2.5MHz(dB)"].Visible = true;
                this.dgvSweepResult.Columns["ACP_ULTRA +7.5MHz(dB)"].Visible = true;
                this.dgvSweepResult.ScrollBars = ScrollBars.Both;
            }
            else if (cbxWaveform.SelectedItem.ToString() == Mod_Waveform_Name.LTEFDD)
            {
                this.dgvSweepResult.Columns["ACP_EULTRA +10MHz(dB)"].Visible = false;
                this.dgvSweepResult.Columns["ACP_ULTRA +2.5MHz(dB)"].Visible = false;
                this.dgvSweepResult.Columns["ACP_ULTRA +7.5MHz(dB)"].Visible = false;
                this.dgvSweepResult.ScrollBars = ScrollBars.Vertical;
            }

        }
        private void SweepTest_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.DoEvents();
            this.Hide();
            try
            {
                if (Program.Location == LocationList.BJ_1)
                {
                    _E4438C.SetOutput(Output.OFF);
                    _PS_66332A.SetOutput(Output.OFF);
                    _Arb_33522A_USB.SetOutput(Output.OFF, Arb_Channel.Channel_1);
                    _Arb_33522A_USB.SetOutput(Output.OFF, Arb_Channel.Channel_2);

                    _Arb_33522A_USB.Dispose();
                    _Arb_33220A.Dispose();
                    _E4438C.Dispose();
                    _MXA_N9020A.Dispose();
                    _PS_66332A.Dispose();
                    _PM_U2001A.Dispose();
                }
                else if (Program.Location == LocationList.SH_1)
                {
                    _E4438C.SetOutput(Output.OFF);
                    _PS_66332A.SetOutput(Output.OFF);
                    _Arb_33522A_USB.SetOutput(Output.OFF, Arb_Channel.Channel_1);
                    _Arb_33522A_USB.SetOutput(Output.OFF, Arb_Channel.Channel_2);

                    _Arb_33522A_USB.Dispose();
                    _Arb_33220A.Dispose();
                    _E4438C.Dispose();
                    _MXA_N9020A.Dispose();
                    _PS_66332A.Dispose();
                    _PM_N1913A.Dispose();
                }
                else if (Program.Location == LocationList.SH_2)
                {
                    _E4438C.SetOutput(Output.OFF);
                    _PS_66332A.SetOutput(Output.OFF);
                    _Arb_33522A.SetOutput(Output.OFF, Arb_Channel.Channel_1);
                    _Arb_33522A.SetOutput(Output.OFF, Arb_Channel.Channel_2);

                    _Arb_33522A.Dispose();
                    _E4438C.Dispose();
                    _MXA_N9020A.Dispose();
                    _PS_66332A.Dispose();
                    _PM_N1913A.Dispose();
                }
                else if (Program.Location == LocationList.SH_3)
                {
                    _E4438C.SetOutput(Output.OFF);
                    _PS_66332A.SetOutput(Output.OFF);
                    _Arb_33522A.SetOutput(Output.OFF, Arb_Channel.Channel_1);
                    _Arb_33522A.SetOutput(Output.OFF, Arb_Channel.Channel_2);

                    _Arb_33522A.Dispose();
                    _Arb_33220A.Dispose();
                    _E4438C.Dispose();
                    _MXA_N9020A.Dispose();
                    _PS_66332A.Dispose();
                    _PM_N1913A.Dispose();
                }
                else if (Program.Location == LocationList.Simulation)
                {
                    Application.DoEvents();
                }
                else
                {
                    throw new Exception("Bad Location");
                }

            }
            catch (Exception ex)
            {
                lblError.Text = ex.Message.ToString();
            }
        }
        private void lblHelp_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MessageBox.Show("Change the setting will effect the current test immediately\r\n" +
                            "if you want use it for next time, please save the setting\r\n" +
                            "Only the current mode setting is saved", "Setting help");
        }

        private void radioButton_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rbnMode = sender as RadioButton;
            if (rbnMode.Checked == false) return;

            if (rbnMode.Text != TestSetting.MODE_CW_LB && rbnMode.Text != TestSetting.MODE_CW_HB)
            {
                cbxexthar.Visible = false;
            }
            else
            {
                cbxexthar.Visible = true;
            }
            switch (rbnMode.Text)
            {
                case TestSetting.MODE_CW_LB:
                    {
                        #region *** Assign CW LB Parameter Setting ***
                        bool firstfreq = true;
                        TestSetting.LEVEL_VCC = TestSetting.SETTING_GMSK_LB[TestSetting.NAME_VCC];
                        TestSetting.LEVEL_TXEN = TestSetting.SETTING_GMSK_LB[TestSetting.NAME_TXEN];
                        TestSetting.LEVEL_GPCTRL = TestSetting.SETTING_GMSK_LB[TestSetting.NAME_GPCTRL];
                        TestSetting.LEVEL_START = TestSetting.SETTING_GMSK_LB[TestSetting.NAME_START];
                        TestSetting.LEVEL_STOP = TestSetting.SETTING_GMSK_LB[TestSetting.NAME_STOP];
                        TestSetting.LEVEL_STEP = TestSetting.SETTING_GMSK_LB[TestSetting.NAME_STEP];
                        TestSetting.LEVEL_PIN_VRAMP = TestSetting.SETTING_GMSK_LB[TestSetting.NAME_PIN_VRAMP];


                        lblPin_Vramp.Text = "RF Pin(dBm)";
                        lblStep.Text = "VRAMP Sweep Voltage (V)";

                        tbxVCC.Text = TestSetting.LEVEL_VCC.ToString();
                        tbxVBAT.Text = TestSetting.NA;
                        tbxTXEN.Text = TestSetting.LEVEL_TXEN.ToString();
                        tbxGPCTRL.Text = TestSetting.LEVEL_GPCTRL.ToString();
                        tbxStart.Text = TestSetting.LEVEL_START.ToString();
                        tbxStop.Text = TestSetting.LEVEL_STOP.ToString();
                        tbxStep.Text = TestSetting.LEVEL_STEP.ToString();
                        tbxPin_Vramp.Text = TestSetting.LEVEL_PIN_VRAMP.ToString();

                        tbxVBAT.Enabled = false;
                        tbxPin_Vramp.Enabled = true;
                        tbxTXEN.Enabled = true;

                        TestSetting.FREQLIST = new List<double>(TestSetting.FREQ_CW_LB.Keys);

                        foreach (var freq in TestSetting.FREQLIST)
                        {
                            if (firstfreq)
                            {
                                tbxFreqList.Text = freq.ToString();
                                firstfreq = false;
                            }
                            else
                            {
                                tbxFreqList.Text += ",";
                                tbxFreqList.Text += freq.ToString();
                            }
                        }

                        #endregion

                        #region *** Initialize GridView ***

                        this.dgvSweepResult.DataSource = null;
                        this.dgvSweepResult.DataSource = this.dtCWLB;

                        this.dgvSweepResult.Columns["#"].Width = 35;
                        this.dgvSweepResult.Columns["Frequency(MHz)"].Width = 110;
                        this.dgvSweepResult.Columns["Vramp(V)"].Width = 75;
                        this.dgvSweepResult.Columns["Pout(dBm)"].Width = 75;
                        this.dgvSweepResult.Columns["ICC(mA)"].Width = 75;
                        this.dgvSweepResult.Columns["PAE(%)"].Width = 75;
                        this.dgvSweepResult.Columns["2fo(dBm)"].Width = 75;
                        this.dgvSweepResult.Columns["3fo(dBm)"].Width = 75;
                        this.dgvSweepResult.Columns["4fo(dBm)"].Width = 75;
                        this.dgvSweepResult.Columns["5fo(dBm)"].Width = 75;
                        this.dgvSweepResult.Columns["6fo(dBm)"].Width = 75;
                        this.dgvSweepResult.Columns["7fo(dBm)"].Width = 75;
                        this.dgvSweepResult.Columns["8fo(dBm)"].Width = 75;
                        this.dgvSweepResult.Columns["9fo(dBm)"].Width = 75;
                        this.dgvSweepResult.Columns["10fo(dBm)"].Width = 75;
                        this.dgvSweepResult.Columns["11fo(dBm)"].Width = 75;
                        this.dgvSweepResult.Columns["12fo(dBm)"].Width = 75;
                        this.dgvSweepResult.Columns["13fo(dBm)"].Width = 75;
                        this.dgvSweepResult.Columns["14fo(dBm)"].Width = 75;

                        //dgvSweepResult.ScrollBars = ScrollBars.Vertical;
                        dgvSweepResult.ScrollBars = ScrollBars.Both;
                        dgvSweepResult.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                        foreach (DataGridViewTextBoxColumn dcTmp in dgvSweepResult.Columns)
                        {
                            dcTmp.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        }

                        this.dgvSweepResult.AllowUserToAddRows = false;
                        this.dgvSweepResult.RowHeadersVisible = false;
                        this.dgvSweepResult.ReadOnly = true;


                        #endregion Initialize GridView

                        #region *** Waveform ***
                        cbxWaveform.Enabled = false;
                        cbxWaveform.Items.Clear();
                        cbxWaveform.Items.Add("N/A");
                        cbxWaveform.SelectedIndex = 0;
                        #endregion *** Waveform ***
                        break;
                    }
                case TestSetting.MODE_CW_HB:
                    {
                        #region *** Assign CW HB Parameter Setting ***
                        bool firstfreq = true;
                        TestSetting.LEVEL_VCC = TestSetting.SETTING_GMSK_HB[TestSetting.NAME_VCC];
                        TestSetting.LEVEL_TXEN = TestSetting.SETTING_GMSK_HB[TestSetting.NAME_TXEN];
                        TestSetting.LEVEL_GPCTRL = TestSetting.SETTING_GMSK_HB[TestSetting.NAME_GPCTRL];
                        TestSetting.LEVEL_START = TestSetting.SETTING_GMSK_HB[TestSetting.NAME_START];
                        TestSetting.LEVEL_STOP = TestSetting.SETTING_GMSK_HB[TestSetting.NAME_STOP];
                        TestSetting.LEVEL_STEP = TestSetting.SETTING_GMSK_HB[TestSetting.NAME_STEP];
                        TestSetting.LEVEL_PIN_VRAMP = TestSetting.SETTING_GMSK_HB[TestSetting.NAME_PIN_VRAMP];


                        lblPin_Vramp.Text = "RF Pin(dBm)";
                        lblStep.Text = "VRAMP Sweep Voltage (V)";

                        tbxVCC.Text = TestSetting.LEVEL_VCC.ToString();
                        tbxVBAT.Text = TestSetting.NA;
                        tbxTXEN.Text = TestSetting.LEVEL_TXEN.ToString();
                        tbxGPCTRL.Text = TestSetting.LEVEL_GPCTRL.ToString();
                        tbxStart.Text = TestSetting.LEVEL_START.ToString();
                        tbxStop.Text = TestSetting.LEVEL_STOP.ToString();
                        tbxStep.Text = TestSetting.LEVEL_STEP.ToString();
                        tbxPin_Vramp.Text = TestSetting.LEVEL_PIN_VRAMP.ToString();

                        tbxVBAT.Enabled = false;
                        tbxPin_Vramp.Enabled = true;
                        tbxTXEN.Enabled = true;

                        TestSetting.FREQLIST = new List<double>(TestSetting.FREQ_CW_HB.Keys);

                        foreach (var freq in TestSetting.FREQLIST)
                        {
                            if (firstfreq)
                            {
                                tbxFreqList.Text = freq.ToString();
                                firstfreq = false;
                            }
                            else
                            {
                                tbxFreqList.Text += ",";
                                tbxFreqList.Text += freq.ToString();
                            }
                        }
                        #endregion

                        #region *** Initialize GridView ***

                        this.dgvSweepResult.DataSource = null;
                        this.dgvSweepResult.DataSource = this.dtCWHB;

                        this.dgvSweepResult.Columns["#"].Width = 35;
                        this.dgvSweepResult.Columns["Frequency(MHz)"].Width = 110;
                        this.dgvSweepResult.Columns["Vramp(V)"].Width = 85;
                        this.dgvSweepResult.Columns["Pout(dBm)"].Width = 85;
                        this.dgvSweepResult.Columns["ICC(mA)"].Width = 75;
                        this.dgvSweepResult.Columns["PAE(%)"].Width = 75;
                        this.dgvSweepResult.Columns["2fo(dBm)"].Width = 75;
                        this.dgvSweepResult.Columns["3fo(dBm)"].Width = 75;
                        this.dgvSweepResult.Columns["4fo(dBm)"].Width = 75;
                        this.dgvSweepResult.Columns["5fo(dBm)"].Width = 75;
                        this.dgvSweepResult.Columns["6fo(dBm)"].Width = 75;
                        this.dgvSweepResult.Columns["7fo(dBm)"].Width = 75;

                        dgvSweepResult.ScrollBars = ScrollBars.Both;
                        dgvSweepResult.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                        foreach (DataGridViewTextBoxColumn dcTmp in dgvSweepResult.Columns)
                        {
                            dcTmp.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        }

                        this.dgvSweepResult.AllowUserToAddRows = false;
                        this.dgvSweepResult.RowHeadersVisible = false;
                        this.dgvSweepResult.ReadOnly = true;


                        #endregion Initialize GridView

                        #region *** Waveform ***
                        cbxWaveform.Enabled = false;
                        cbxWaveform.Items.Clear();
                        cbxWaveform.Items.Add("N/A");
                        cbxWaveform.SelectedIndex = 0;
                        #endregion *** Waveform ***
                        break;
                    }
                case TestSetting.MODE_EDGE_LB:
                    {
                        #region *** Assign EDGE LB Parameter Setting ***
                        bool firstfreq = true;
                        TestSetting.LEVEL_VCC = TestSetting.SETTING_EDGE_LB[TestSetting.NAME_VCC];
                        TestSetting.LEVEL_TXEN = TestSetting.SETTING_EDGE_LB[TestSetting.NAME_TXEN];
                        TestSetting.LEVEL_GPCTRL = TestSetting.SETTING_EDGE_LB[TestSetting.NAME_GPCTRL];
                        TestSetting.LEVEL_START = TestSetting.SETTING_EDGE_LB[TestSetting.NAME_START];
                        TestSetting.LEVEL_STOP = TestSetting.SETTING_EDGE_LB[TestSetting.NAME_STOP];
                        TestSetting.LEVEL_STEP = TestSetting.SETTING_EDGE_LB[TestSetting.NAME_STEP];
                        TestSetting.LEVEL_PIN_VRAMP = TestSetting.SETTING_EDGE_LB[TestSetting.NAME_PIN_VRAMP];

                        tbxVCC.Text = TestSetting.LEVEL_VCC.ToString();
                        tbxTXEN.Text = TestSetting.LEVEL_TXEN.ToString();
                        tbxGPCTRL.Text = TestSetting.LEVEL_GPCTRL.ToString();
                        tbxStart.Text = TestSetting.LEVEL_START.ToString();
                        tbxStop.Text = TestSetting.LEVEL_STOP.ToString();
                        tbxStep.Text = TestSetting.LEVEL_STEP.ToString();
                        tbxPin_Vramp.Text = TestSetting.LEVEL_PIN_VRAMP.ToString();
                        tbxVBAT.Text = TestSetting.NA;

                        lblPin_Vramp.Text = "Vramp Voltage (V)";
                        lblStep.Text = "EDGE_LB Target Power (dBm)";
                        tbxPin_Vramp.Enabled = true;
                        tbxTXEN.Enabled = true;
                        tbxVBAT.Enabled = false;

                        TestSetting.FREQLIST = new List<double>(TestSetting.FREQ_EDGE_LB.Keys);

                        foreach (var freq in TestSetting.FREQLIST)
                        {
                            if (firstfreq)
                            {
                                tbxFreqList.Text = freq.ToString();
                                firstfreq = false;
                            }
                            else
                            {
                                tbxFreqList.Text += ",";
                                tbxFreqList.Text += freq.ToString();
                            }
                        }
                        #endregion

                        #region *** Initialize GridView ***

                        this.dgvSweepResult.DataSource = null;
                        this.dgvSweepResult.DataSource = this.dtEDGELB;


                        this.dgvSweepResult.Columns["#"].Width = 35;
                        this.dgvSweepResult.Columns["Frequency (MHz)"].Width = 85;
                        this.dgvSweepResult.Columns["Target Pout(dBm)"].Width = 85;
                        this.dgvSweepResult.Columns["Pout(dBm)"].Width = 85;
                        this.dgvSweepResult.Columns["Pin(dBm)"].Width = 85;
                        this.dgvSweepResult.Columns["Gain(dB)"].Width = 85; ;
                        this.dgvSweepResult.Columns["Icc(mA)"].Width = 85;
                        this.dgvSweepResult.Columns["PAE(%)"].Width = 75;
                        this.dgvSweepResult.Columns["ACP -400kHz(dB)"].Width = 95;
                        this.dgvSweepResult.Columns["ACP +400kHz(dB)"].Width = 95;
                        this.dgvSweepResult.Columns["EVM(%)"].Width = 85;


                        dgvSweepResult.ScrollBars = ScrollBars.Both;
                        dgvSweepResult.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                        foreach (DataGridViewTextBoxColumn dcTmp in dgvSweepResult.Columns)
                        {
                            dcTmp.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        }

                        this.dgvSweepResult.AllowUserToAddRows = false;
                        this.dgvSweepResult.RowHeadersVisible = false;
                        this.dgvSweepResult.ReadOnly = true;


                        #endregion Initialize GridView

                        #region *** Waveform ***
                        cbxWaveform.Enabled = false;
                        cbxWaveform.Items.Clear();
                        cbxWaveform.Items.Add(Mod_Waveform_Name.EDGE);
                        cbxWaveform.SelectedIndex = 0;
                        #endregion *** Waveform ***
                        break;
                    }
                case TestSetting.MODE_EDGE_HB:
                    {
                        #region *** Assign EDGE HB Parameter Setting ***
                        bool firstfreq = true;
                        TestSetting.LEVEL_VCC = TestSetting.SETTING_EDGE_HB[TestSetting.NAME_VCC];
                        TestSetting.LEVEL_TXEN = TestSetting.SETTING_EDGE_HB[TestSetting.NAME_TXEN];
                        TestSetting.LEVEL_GPCTRL = TestSetting.SETTING_EDGE_HB[TestSetting.NAME_GPCTRL];
                        TestSetting.LEVEL_START = TestSetting.SETTING_EDGE_HB[TestSetting.NAME_START];
                        TestSetting.LEVEL_STOP = TestSetting.SETTING_EDGE_HB[TestSetting.NAME_STOP];
                        TestSetting.LEVEL_STEP = TestSetting.SETTING_EDGE_HB[TestSetting.NAME_STEP];
                        TestSetting.LEVEL_PIN_VRAMP = TestSetting.SETTING_EDGE_HB[TestSetting.NAME_PIN_VRAMP];

                        tbxVCC.Text = TestSetting.LEVEL_VCC.ToString();
                        tbxTXEN.Text = TestSetting.LEVEL_TXEN.ToString();
                        tbxGPCTRL.Text = TestSetting.LEVEL_GPCTRL.ToString();
                        tbxStart.Text = TestSetting.LEVEL_START.ToString();
                        tbxStop.Text = TestSetting.LEVEL_STOP.ToString();
                        tbxStep.Text = TestSetting.LEVEL_STEP.ToString();
                        tbxPin_Vramp.Text = TestSetting.LEVEL_PIN_VRAMP.ToString();
                        tbxVBAT.Text = TestSetting.NA;

                        lblPin_Vramp.Text = "Vramp Voltage (V)";
                        lblStep.Text = "EDGE_HB Target Power (dBm)";
                        tbxPin_Vramp.Enabled = true;
                        tbxTXEN.Enabled = true;
                        tbxVBAT.Enabled = false;

                        TestSetting.FREQLIST = new List<double>(TestSetting.FREQ_EDGE_HB.Keys);

                        foreach (var freq in TestSetting.FREQLIST)
                        {
                            if (firstfreq)
                            {
                                tbxFreqList.Text = freq.ToString();
                                firstfreq = false;
                            }
                            else
                            {
                                tbxFreqList.Text += ",";
                                tbxFreqList.Text += freq.ToString();
                            }
                        }
                        #endregion

                        #region *** Initialize GridView ***

                        this.dgvSweepResult.DataSource = null;
                        this.dgvSweepResult.DataSource = this.dtEDGEHB;

                        this.dgvSweepResult.Columns["#"].Width = 35;
                        this.dgvSweepResult.Columns["Frequency (MHz)"].Width = 85;
                        this.dgvSweepResult.Columns["Target Pout(dBm)"].Width = 85;
                        this.dgvSweepResult.Columns["Pout(dBm)"].Width = 85;
                        this.dgvSweepResult.Columns["Pin(dBm)"].Width = 85;
                        this.dgvSweepResult.Columns["Gain(dB)"].Width = 85; ;
                        this.dgvSweepResult.Columns["Icc(mA)"].Width = 85;
                        this.dgvSweepResult.Columns["PAE(%)"].Width = 75;
                        this.dgvSweepResult.Columns["ACP -400kHz(dB)"].Width = 95;
                        this.dgvSweepResult.Columns["ACP +400kHz(dB)"].Width = 95;
                        this.dgvSweepResult.Columns["EVM(%)"].Width = 85;

                        dgvSweepResult.ScrollBars = ScrollBars.Both;
                        dgvSweepResult.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                        foreach (DataGridViewTextBoxColumn dcTmp in dgvSweepResult.Columns)
                        {
                            dcTmp.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        }

                        this.dgvSweepResult.AllowUserToAddRows = false;
                        this.dgvSweepResult.RowHeadersVisible = false;
                        this.dgvSweepResult.ReadOnly = true;


                        #endregion Initialize GridView

                        #region *** Waveform ***
                        cbxWaveform.Enabled = false;
                        cbxWaveform.Items.Clear();
                        cbxWaveform.Items.Add(Mod_Waveform_Name.EDGE);
                        cbxWaveform.SelectedIndex = 0;
                        #endregion *** Waveform ***
                        break;
                    }
                case TestSetting.MODE_TDSCDMA:
                    {
                        #region *** Assign TDSCDMA Parameter Setting ***
                        bool firstfreq = true;
                        TestSetting.LEVEL_VCC = TestSetting.SETTING_TDSCDMA[TestSetting.NAME_VCC];
                        TestSetting.LEVEL_TXEN = TestSetting.SETTING_TDSCDMA[TestSetting.NAME_TXEN];
                        TestSetting.LEVEL_GPCTRL = TestSetting.SETTING_TDSCDMA[TestSetting.NAME_GPCTRL];
                        TestSetting.LEVEL_START = TestSetting.SETTING_TDSCDMA[TestSetting.NAME_START];
                        TestSetting.LEVEL_STOP = TestSetting.SETTING_TDSCDMA[TestSetting.NAME_STOP];
                        TestSetting.LEVEL_STEP = TestSetting.SETTING_TDSCDMA[TestSetting.NAME_STEP];
                        TestSetting.LEVEL_PIN_VRAMP = TestSetting.SETTING_TDSCDMA[TestSetting.NAME_PIN_VRAMP];

                        tbxVCC.Text = TestSetting.SETTING_TDSCDMA[TestSetting.NAME_VCC].ToString();
                        tbxTXEN.Text = TestSetting.SETTING_TDSCDMA[TestSetting.NAME_TXEN].ToString();
                        tbxGPCTRL.Text = TestSetting.SETTING_TDSCDMA[TestSetting.NAME_GPCTRL].ToString();
                        tbxStart.Text = TestSetting.SETTING_TDSCDMA[TestSetting.NAME_START].ToString();
                        tbxStop.Text = TestSetting.SETTING_TDSCDMA[TestSetting.NAME_STOP].ToString();
                        tbxStep.Text = TestSetting.SETTING_TDSCDMA[TestSetting.NAME_STEP].ToString();
                        tbxPin_Vramp.Text = TestSetting.SETTING_TDSCDMA[TestSetting.NAME_PIN_VRAMP].ToString();
                        tbxVBAT.Text = TestSetting.NA;

                        lblPin_Vramp.Text = "Vramp Voltage (V)";
                        lblStep.Text = "TDSCDMA Target Power (dBm)";
                        tbxPin_Vramp.Enabled = true;
                        tbxTXEN.Enabled = true;
                        tbxVBAT.Enabled = false;

                        TestSetting.FREQLIST = new List<double>(TestSetting.FREQ_TDSCDMA.Keys);

                        foreach (var freq in TestSetting.FREQLIST)
                        {
                            if (firstfreq)
                            {
                                tbxFreqList.Text = freq.ToString();
                                firstfreq = false;
                            }
                            else
                            {
                                tbxFreqList.Text += ",";
                                tbxFreqList.Text += freq.ToString();
                            }
                        }
                        #endregion

                        #region *** Initialize GridView ***
                        this.dgvSweepResult.DataSource = null;
                        this.dgvSweepResult.DataSource = this.dtTDSCDMA;

                        this.dgvSweepResult.Columns["#"].Width = 25;
                        this.dgvSweepResult.Columns["Frequency (MHz)"].Width = 70;
                        this.dgvSweepResult.Columns["Target Pout(dBm)"].Width = 70;
                        this.dgvSweepResult.Columns["Pout(dBm)"].Width = 70;
                        this.dgvSweepResult.Columns["Pin(dBm)"].Width = 70;
                        this.dgvSweepResult.Columns["Gain(dB)"].Width = 70; ;
                        this.dgvSweepResult.Columns["Icc(mA)"].Width = 70;
                        this.dgvSweepResult.Columns["PAE(%)"].Width = 60;
                        this.dgvSweepResult.Columns["ACP -1.6MHz(dB)"].Width = 80;
                        this.dgvSweepResult.Columns["ACP +1.6MHz(dB)"].Width = 80;
                        this.dgvSweepResult.Columns["ACP -3.2MHz(dB)"].Width = 80;
                        this.dgvSweepResult.Columns["ACP +3.2MHz(dB)"].Width = 80;
                        this.dgvSweepResult.Columns["EVM(%)"].Width = 70;

                        dgvSweepResult.ScrollBars = ScrollBars.Both;
                        dgvSweepResult.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        dgvSweepResult.AllowUserToOrderColumns = false;

                        foreach (DataGridViewTextBoxColumn dcTmp in dgvSweepResult.Columns)
                        {
                            dcTmp.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                            dcTmp.SortMode = DataGridViewColumnSortMode.NotSortable;
                        }
                        this.dgvSweepResult.AllowUserToAddRows = false;
                        this.dgvSweepResult.RowHeadersVisible = false;
                        this.dgvSweepResult.ReadOnly = true;


                        #endregion Initialize GridView

                        #region *** Waveform ***
                        cbxWaveform.Enabled = true;
                        cbxWaveform.Items.Clear();
                        cbxWaveform.Items.Add(Mod_Waveform_Name.TDSCDMA);
                        cbxWaveform.Items.Add(Mod_Waveform_Name.TDHSDPA);
                        cbxWaveform.SelectedIndex = 0;
                        #endregion *** Waveform ***
                        break;
                    }
                case TestSetting.MODE_WCDMA:
                    {
                        #region *** Assign WCDMA Parameter Setting ***
                        bool firstfreq = true;
                        TestSetting.LEVEL_VCC = TestSetting.SETTING_WCDMA[TestSetting.NAME_VCC];
                        TestSetting.LEVEL_VBAT = TestSetting.SETTING_WCDMA[TestSetting.NAME_VBAT];
                        //TestSetting.LEVEL_TXEN = TestSetting.SETTING_WCDMA[TestSetting.NAME_TXEN];
                        TestSetting.LEVEL_GPCTRL = TestSetting.SETTING_WCDMA[TestSetting.NAME_GPCTRL];
                        TestSetting.LEVEL_START = TestSetting.SETTING_WCDMA[TestSetting.NAME_START];
                        TestSetting.LEVEL_STOP = TestSetting.SETTING_WCDMA[TestSetting.NAME_STOP];
                        TestSetting.LEVEL_STEP = TestSetting.SETTING_WCDMA[TestSetting.NAME_STEP];
                        TestSetting.LEVEL_PIN_VRAMP = TestSetting.SETTING_WCDMA[TestSetting.NAME_PIN_VRAMP];


                        lblPin_Vramp.Text = "RF Pin(dBm)";
                        lblPin_Vramp.Text = "Vramp Voltage (V)";
                        lblStep.Text = "WCDMA Target Power (dBm)";

                        tbxVCC.Text = TestSetting.LEVEL_VCC.ToString();
                        tbxVBAT.Text = TestSetting.LEVEL_VBAT.ToString();
                        tbxTXEN.Text = TestSetting.NA;
                        tbxPin_Vramp.Text = TestSetting.LEVEL_PIN_VRAMP.ToString();
                        tbxGPCTRL.Text = TestSetting.LEVEL_GPCTRL.ToString();
                        tbxStart.Text = TestSetting.LEVEL_START.ToString();
                        tbxStop.Text = TestSetting.LEVEL_STOP.ToString();
                        tbxStep.Text = TestSetting.LEVEL_STEP.ToString();

                        tbxVBAT.Enabled = true;
                        tbxTXEN.Enabled = false;
                        tbxPin_Vramp.Enabled = true;

                        TestSetting.FREQLIST = new List<double>(TestSetting.FREQ_WCDMA.Keys);

                        foreach (var freq in TestSetting.FREQLIST)
                        {
                            if (firstfreq)
                            {
                                tbxFreqList.Text = freq.ToString();
                                firstfreq = false;
                            }
                            else
                            {
                                tbxFreqList.Text += ",";
                                tbxFreqList.Text += freq.ToString();
                            }
                        }
                        #endregion

                        #region *** Initialize GridView ***
                        this.dgvSweepResult.DataSource = null;
                        this.dgvSweepResult.DataSource = this.dtWCDMA;

                        this.dgvSweepResult.Columns["#"].Width = 25;
                        this.dgvSweepResult.Columns["Frequency (MHz)"].Width = 70;
                        this.dgvSweepResult.Columns["Target Pout(dBm)"].Width = 70;
                        this.dgvSweepResult.Columns["Pout(dBm)"].Width = 70;
                        this.dgvSweepResult.Columns["Pin(dBm)"].Width = 70;
                        this.dgvSweepResult.Columns["Gain(dB)"].Width = 70; ;
                        this.dgvSweepResult.Columns["Icc(mA)"].Width = 70;
                        this.dgvSweepResult.Columns["PAE(%)"].Width = 60;
                        this.dgvSweepResult.Columns["ACP -5MHz(dB)"].Width = 80;
                        this.dgvSweepResult.Columns["ACP +5MHz(dB)"].Width = 80;
                        this.dgvSweepResult.Columns["ACP -10MHz(dB)"].Width = 80;
                        this.dgvSweepResult.Columns["ACP +10MHz(dB)"].Width = 80;
                        this.dgvSweepResult.Columns["EVM(%)"].Width = 70;

                        dgvSweepResult.ScrollBars = ScrollBars.Both;
                        dgvSweepResult.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        dgvSweepResult.AllowUserToOrderColumns = false;

                        //for (int i = 0; i < dgvSweepResult.Columns.Count; i++)
                        //{
                        //    dgvSweepResult.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                        //}

                        foreach (DataGridViewTextBoxColumn dcTmp in dgvSweepResult.Columns)
                        {
                            dcTmp.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                            dcTmp.SortMode = DataGridViewColumnSortMode.NotSortable;
                        }
                        this.dgvSweepResult.AllowUserToAddRows = false;
                        this.dgvSweepResult.RowHeadersVisible = false;
                        this.dgvSweepResult.ReadOnly = true;


                        #endregion Initialize GridView

                        #region *** Waveform ***
                        cbxWaveform.Enabled = false;
                        cbxWaveform.Items.Clear();
                        cbxWaveform.Items.Add(Mod_Waveform_Name.WCDMA);
                        cbxWaveform.SelectedIndex = 0;
                        #endregion *** Waveform ***
                        break;
                    }
                case TestSetting.MODE_LTETDD_B38:
                    {
                        #region *** Assign LTE TDD Parameter Setting ***
                        bool firstfreq = true;
                        TestSetting.LEVEL_VCC = TestSetting.SETTING_LTETDD_B38[TestSetting.NAME_VCC];
                        TestSetting.LEVEL_TXEN = TestSetting.SETTING_LTETDD_B38[TestSetting.NAME_VEN];
                        TestSetting.LEVEL_GPCTRL = TestSetting.SETTING_LTETDD_B38[TestSetting.NAME_MODE];
                        TestSetting.LEVEL_START = TestSetting.SETTING_LTETDD_B38[TestSetting.NAME_START];
                        TestSetting.LEVEL_STOP = TestSetting.SETTING_LTETDD_B38[TestSetting.NAME_STOP];
                        TestSetting.LEVEL_STEP = TestSetting.SETTING_LTETDD_B38[TestSetting.NAME_STEP];
                        TestSetting.LEVEL_PIN_VRAMP = TestSetting.SETTING_LTETDD_B38[TestSetting.NAME_PIN_VRAMP];

                        tbxVCC.Text = TestSetting.LEVEL_VCC.ToString();
                        tbxTXEN.Text = TestSetting.LEVEL_TXEN.ToString();
                        tbxPin_Vramp.Text = TestSetting.LEVEL_PIN_VRAMP.ToString();
                        tbxGPCTRL.Text = TestSetting.LEVEL_GPCTRL.ToString();
                        tbxStart.Text = TestSetting.LEVEL_START.ToString();
                        tbxStop.Text = TestSetting.LEVEL_STOP.ToString();
                        tbxStep.Text = TestSetting.LEVEL_STEP.ToString();
                        //tbxPin_Vramp.Text = TestSetting.SETTING_LTETDD[TestSetting.NAME_PIN_VRAMP].ToString();
                        tbxVBAT.Text = TestSetting.NA;

                        //lblTXEN.Text = "VEN Voltage (V)";
                        //lblGPCTRL.Text = "MODE Voltage (V)";
                        lblPin_Vramp.Text = "Vramp Voltage (V)";
                        lblStep.Text = "LTE TDD Target Power (dBm)";
                        tbxPin_Vramp.Enabled = true;
                        //tbxTXEN.Enabled = true;
                        tbxVBAT.Enabled = false;

                        TestSetting.FREQLIST = new List<double>(TestSetting.FREQ_LTETDD_B38.Keys);

                        foreach (var freq in TestSetting.FREQLIST)
                        {
                            if (firstfreq)
                            {
                                tbxFreqList.Text = freq.ToString();
                                firstfreq = false;
                            }
                            else
                            {
                                tbxFreqList.Text += ",";
                                tbxFreqList.Text += freq.ToString();
                            }
                        }
                        #endregion

                        #region *** Initialize GridView ***
                        this.dgvSweepResult.DataSource = null;
                        this.dgvSweepResult.DataSource = this.dtLTETDD_B38;

                        this.dgvSweepResult.Columns["#"].Width = 25;
                        this.dgvSweepResult.Columns["Frequency (MHz)"].Width = 70;
                        this.dgvSweepResult.Columns["Target Pout(dBm)"].Width = 70;
                        this.dgvSweepResult.Columns["Pout(dBm)"].Width = 70;
                        this.dgvSweepResult.Columns["Pin(dBm)"].Width = 70;
                        this.dgvSweepResult.Columns["Gain(dB)"].Width = 70; ;
                        this.dgvSweepResult.Columns["Icc(mA)"].Width = 70;
                        this.dgvSweepResult.Columns["PAE(%)"].Width = 60;
                        this.dgvSweepResult.Columns["ACP_EULTRA -10MHz(dB)"].Width = 80;
                        this.dgvSweepResult.Columns["ACP_EULTRA +10MHz(dB)"].Width = 80;
                        this.dgvSweepResult.Columns["ACP_ULTRA -0.8MHz(dB)"].Width = 80;
                        this.dgvSweepResult.Columns["ACP_ULTRA +0.8MHz(dB)"].Width = 80;
                        this.dgvSweepResult.Columns["ACP_ULTRA -2.4MHz(dB)"].Width = 80;
                        this.dgvSweepResult.Columns["ACP_ULTRA +2.4MHz(dB)"].Width = 80;
                        this.dgvSweepResult.Columns["EVM(%)"].Width = 70;

                        dgvSweepResult.ScrollBars = ScrollBars.Both;
                        dgvSweepResult.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        dgvSweepResult.AllowUserToOrderColumns = false;

                        foreach (DataGridViewTextBoxColumn dcTmp in dgvSweepResult.Columns)
                        {
                            dcTmp.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                            dcTmp.SortMode = DataGridViewColumnSortMode.NotSortable;
                        }
                        this.dgvSweepResult.AllowUserToAddRows = false;
                        this.dgvSweepResult.RowHeadersVisible = false;
                        this.dgvSweepResult.ReadOnly = true;


                        #endregion Initialize GridView

                        #region *** Waveform ***
                        cbxWaveform.Enabled = true;
                        cbxWaveform.Items.Clear();
                        cbxWaveform.Items.Add(Mod_Waveform_Name.LTETDD);
                        cbxWaveform.Items.Add(Mod_Waveform_Name.LTETDD_FULL);
                        cbxWaveform.SelectedIndex = 0;
                        #endregion *** Waveform ***
                        break;
                    }
                case TestSetting.MODE_LTETDD_B40:
                    {
                        #region *** Assign LTE TDD Parameter Setting ***
                        bool firstfreq = true;
                        TestSetting.LEVEL_VCC = TestSetting.SETTING_LTETDD_B40[TestSetting.NAME_VCC];
                        TestSetting.LEVEL_TXEN = TestSetting.SETTING_LTETDD_B40[TestSetting.NAME_VEN];
                        TestSetting.LEVEL_GPCTRL = TestSetting.SETTING_LTETDD_B40[TestSetting.NAME_MODE];
                        TestSetting.LEVEL_START = TestSetting.SETTING_LTETDD_B40[TestSetting.NAME_START];
                        TestSetting.LEVEL_STOP = TestSetting.SETTING_LTETDD_B40[TestSetting.NAME_STOP];
                        TestSetting.LEVEL_STEP = TestSetting.SETTING_LTETDD_B40[TestSetting.NAME_STEP];
                        TestSetting.LEVEL_PIN_VRAMP = TestSetting.SETTING_LTETDD_B40[TestSetting.NAME_PIN_VRAMP];

                        tbxVCC.Text = TestSetting.LEVEL_VCC.ToString();
                        tbxTXEN.Text = TestSetting.LEVEL_TXEN.ToString();
                        tbxPin_Vramp.Text = TestSetting.LEVEL_PIN_VRAMP.ToString();
                        tbxGPCTRL.Text = TestSetting.LEVEL_GPCTRL.ToString();
                        tbxStart.Text = TestSetting.LEVEL_START.ToString();
                        tbxStop.Text = TestSetting.LEVEL_STOP.ToString();
                        tbxStep.Text = TestSetting.LEVEL_STEP.ToString();
                        tbxVBAT.Text = TestSetting.NA;

                        //lblTXEN.Text = "VEN Voltage (V)";
                        //lblGPCTRL.Text = "MODE Voltage (V)";
                        lblPin_Vramp.Text = "Vramp Voltage (V)";
                        lblStep.Text = "LTE TDD Target Power (dBm)";
                        tbxPin_Vramp.Enabled = true;
                        //tbxTXEN.Enabled = true;
                        tbxVBAT.Enabled = false;

                        TestSetting.FREQLIST = new List<double>(TestSetting.FREQ_LTETDD_B40.Keys);

                        foreach (var freq in TestSetting.FREQLIST)
                        {
                            if (firstfreq)
                            {
                                tbxFreqList.Text = freq.ToString();
                                firstfreq = false;
                            }
                            else
                            {
                                tbxFreqList.Text += ",";
                                tbxFreqList.Text += freq.ToString();
                            }
                        }
                        #endregion

                        #region *** Initialize GridView ***
                        this.dgvSweepResult.DataSource = null;
                        this.dgvSweepResult.DataSource = this.dtLTETDD_B40;

                        this.dgvSweepResult.Columns["#"].Width = 25;
                        this.dgvSweepResult.Columns["Frequency (MHz)"].Width = 70;
                        this.dgvSweepResult.Columns["Target Pout(dBm)"].Width = 70;
                        this.dgvSweepResult.Columns["Pout(dBm)"].Width = 70;
                        this.dgvSweepResult.Columns["Pin(dBm)"].Width = 70;
                        this.dgvSweepResult.Columns["Gain(dB)"].Width = 70; ;
                        this.dgvSweepResult.Columns["Icc(mA)"].Width = 70;
                        this.dgvSweepResult.Columns["PAE(%)"].Width = 60;
                        this.dgvSweepResult.Columns["ACP_EULTRA -10MHz(dB)"].Width = 80;
                        this.dgvSweepResult.Columns["ACP_EULTRA +10MHz(dB)"].Width = 80;
                        this.dgvSweepResult.Columns["ACP_ULTRA -0.8MHz(dB)"].Width = 80;
                        this.dgvSweepResult.Columns["ACP_ULTRA +0.8MHz(dB)"].Width = 80;
                        this.dgvSweepResult.Columns["ACP_ULTRA -2.4MHz(dB)"].Width = 80;
                        this.dgvSweepResult.Columns["ACP_ULTRA +2.4MHz(dB)"].Width = 80;
                        this.dgvSweepResult.Columns["EVM(%)"].Width = 70;

                        dgvSweepResult.ScrollBars = ScrollBars.Both;
                        dgvSweepResult.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        dgvSweepResult.AllowUserToOrderColumns = false;

                        foreach (DataGridViewTextBoxColumn dcTmp in dgvSweepResult.Columns)
                        {
                            dcTmp.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                            dcTmp.SortMode = DataGridViewColumnSortMode.NotSortable;
                        }
                        this.dgvSweepResult.AllowUserToAddRows = false;
                        this.dgvSweepResult.RowHeadersVisible = false;
                        this.dgvSweepResult.ReadOnly = true;


                        #endregion Initialize GridView

                        #region *** Waveform ***
                        cbxWaveform.Enabled = true;
                        cbxWaveform.Items.Clear();
                        cbxWaveform.Items.Add(Mod_Waveform_Name.LTETDD);
                        cbxWaveform.Items.Add(Mod_Waveform_Name.LTETDD_FULL);
                        cbxWaveform.SelectedIndex = 0;
                        #endregion *** Waveform ***
                        break;
                    }
                case TestSetting.MODE_LTEFDD_LB:
                    {
                        #region *** Assign LTE FDD Parameter Setting ***
                        bool firstfreq = true;
                        TestSetting.LEVEL_VCC = TestSetting.SETTING_LTEFDD_LB[TestSetting.NAME_VCC];
                        TestSetting.LEVEL_TXEN = TestSetting.SETTING_LTEFDD_LB[TestSetting.NAME_VEN];
                        TestSetting.LEVEL_GPCTRL = TestSetting.SETTING_LTEFDD_LB[TestSetting.NAME_MODE];
                        TestSetting.LEVEL_START = TestSetting.SETTING_LTEFDD_LB[TestSetting.NAME_START];
                        TestSetting.LEVEL_STOP = TestSetting.SETTING_LTEFDD_LB[TestSetting.NAME_STOP];
                        TestSetting.LEVEL_STEP = TestSetting.SETTING_LTEFDD_LB[TestSetting.NAME_STEP];
                        TestSetting.LEVEL_PIN_VRAMP = TestSetting.SETTING_LTEFDD_LB[TestSetting.NAME_PIN_VRAMP];

                        tbxVCC.Text = TestSetting.LEVEL_VCC.ToString();
                        tbxTXEN.Text = TestSetting.LEVEL_TXEN.ToString();
                        tbxPin_Vramp.Text = TestSetting.LEVEL_PIN_VRAMP.ToString();
                        tbxGPCTRL.Text = TestSetting.LEVEL_GPCTRL.ToString();
                        tbxStart.Text = TestSetting.LEVEL_START.ToString();
                        tbxStop.Text = TestSetting.LEVEL_STOP.ToString();
                        tbxStep.Text = TestSetting.LEVEL_STEP.ToString();
                        tbxVBAT.Text = TestSetting.NA;

                        //lblTXEN.Text = "VEN Voltage (V)";
                        //lblGPCTRL.Text = "MODE Voltage (V)";
                        lblPin_Vramp.Text = "Vramp Voltage (V)";
                        lblStep.Text = "LTE FDD Target Power (dBm)";
                        tbxPin_Vramp.Enabled = true;
                        tbxTXEN.Enabled = true;
                        tbxVBAT.Enabled = false;

                        TestSetting.FREQLIST = new List<double>(TestSetting.FREQ_LTEFDD_LB.Keys);

                        foreach (var freq in TestSetting.FREQLIST)
                        {
                            if (firstfreq)
                            {
                                tbxFreqList.Text = freq.ToString();
                                firstfreq = false;
                            }
                            else
                            {
                                tbxFreqList.Text += ",";
                                tbxFreqList.Text += freq.ToString();
                            }
                        }
                        #endregion

                        #region *** Initialize GridView ***
                        this.dgvSweepResult.DataSource = null;
                        this.dgvSweepResult.DataSource = this.dtLTEFDD_B1;

                        this.dgvSweepResult.Columns["#"].Width = 25;
                        this.dgvSweepResult.Columns["Frequency (MHz)"].Width = 70;
                        this.dgvSweepResult.Columns["Target Pout(dBm)"].Width = 70;
                        this.dgvSweepResult.Columns["Pout(dBm)"].Width = 70;
                        this.dgvSweepResult.Columns["Pin(dBm)"].Width = 70;
                        this.dgvSweepResult.Columns["Gain(dB)"].Width = 70; ;
                        this.dgvSweepResult.Columns["Icc(mA)"].Width = 70;
                        this.dgvSweepResult.Columns["PAE(%)"].Width = 60;
                        this.dgvSweepResult.Columns["ACP_EULTRA -10MHz(dB)"].Width = 80;
                        this.dgvSweepResult.Columns["ACP_EULTRA +10MHz(dB)"].Width = 80;
                        this.dgvSweepResult.Columns["ACP_ULTRA -2.5MHz(dB)"].Width = 80;
                        this.dgvSweepResult.Columns["ACP_ULTRA +2.5MHz(dB)"].Width = 80;
                        this.dgvSweepResult.Columns["ACP_ULTRA -7.5MHz(dB)"].Width = 80;
                        this.dgvSweepResult.Columns["ACP_ULTRA +7.5MHz(dB)"].Width = 80;
                        this.dgvSweepResult.Columns["EVM(%)"].Width = 70;

                        dgvSweepResult.ScrollBars = ScrollBars.Both;
                        dgvSweepResult.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        dgvSweepResult.AllowUserToOrderColumns = false;

                        foreach (DataGridViewTextBoxColumn dcTmp in dgvSweepResult.Columns)
                        {
                            dcTmp.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                            dcTmp.SortMode = DataGridViewColumnSortMode.NotSortable;
                        }
                        this.dgvSweepResult.AllowUserToAddRows = false;
                        this.dgvSweepResult.RowHeadersVisible = false;
                        this.dgvSweepResult.ReadOnly = true;


                        #endregion Initialize GridView

                        #region *** Waveform ***
                        cbxWaveform.Enabled = true;
                        cbxWaveform.Items.Clear();
                        cbxWaveform.Items.Add(Mod_Waveform_Name.LTEFDD);
                        cbxWaveform.Items.Add(Mod_Waveform_Name.LTEFDD_FULL);
                        cbxWaveform.SelectedIndex = 0;
                        #endregion *** Waveform ***
                        break;
                    }
                case TestSetting.MODE_LTEFDD_HB:
                    {
                        #region *** Assign LTE FDD Parameter Setting ***
                        bool firstfreq = true;
                        TestSetting.LEVEL_VCC = TestSetting.SETTING_LTEFDD_HB[TestSetting.NAME_VCC];
                        TestSetting.LEVEL_TXEN = TestSetting.SETTING_LTEFDD_HB[TestSetting.NAME_VEN];
                        TestSetting.LEVEL_GPCTRL = TestSetting.SETTING_LTEFDD_HB[TestSetting.NAME_MODE];
                        TestSetting.LEVEL_START = TestSetting.SETTING_LTEFDD_HB[TestSetting.NAME_START];
                        TestSetting.LEVEL_STOP = TestSetting.SETTING_LTEFDD_HB[TestSetting.NAME_STOP];
                        TestSetting.LEVEL_STEP = TestSetting.SETTING_LTEFDD_HB[TestSetting.NAME_STEP];
                        TestSetting.LEVEL_PIN_VRAMP = TestSetting.SETTING_LTEFDD_HB[TestSetting.NAME_PIN_VRAMP];

                        tbxVCC.Text = TestSetting.LEVEL_VCC.ToString();
                        tbxTXEN.Text = TestSetting.LEVEL_TXEN.ToString();
                        tbxPin_Vramp.Text = TestSetting.LEVEL_PIN_VRAMP.ToString();
                        tbxGPCTRL.Text = TestSetting.LEVEL_GPCTRL.ToString();
                        tbxStart.Text = TestSetting.LEVEL_START.ToString();
                        tbxStop.Text = TestSetting.LEVEL_STOP.ToString();
                        tbxStep.Text = TestSetting.LEVEL_STEP.ToString();
                        tbxVBAT.Text = TestSetting.NA;

                        //lblTXEN.Text = "VEN Voltage (V)";
                        //lblGPCTRL.Text = "MODE Voltage (V)";
                        lblPin_Vramp.Text = "Vramp Voltage (V)";
                        lblStep.Text = "LTE FDD Target Power (dBm)";
                        tbxPin_Vramp.Enabled = true;
                        tbxTXEN.Enabled = true;
                        tbxVBAT.Enabled = false;

                        TestSetting.FREQLIST = new List<double>(TestSetting.FREQ_LTEFDD_HB.Keys);

                        foreach (var freq in TestSetting.FREQLIST)
                        {
                            if (firstfreq)
                            {
                                tbxFreqList.Text = freq.ToString();
                                firstfreq = false;
                            }
                            else
                            {
                                tbxFreqList.Text += ",";
                                tbxFreqList.Text += freq.ToString();
                            }
                        }
                        #endregion

                        #region *** Initialize GridView ***
                        this.dgvSweepResult.DataSource = null;
                        this.dgvSweepResult.DataSource = this.dtLTEFDD_B2;

                        this.dgvSweepResult.Columns["#"].Width = 25;
                        this.dgvSweepResult.Columns["Frequency (MHz)"].Width = 70;
                        this.dgvSweepResult.Columns["Target Pout(dBm)"].Width = 70;
                        this.dgvSweepResult.Columns["Pout(dBm)"].Width = 70;
                        this.dgvSweepResult.Columns["Pin(dBm)"].Width = 70;
                        this.dgvSweepResult.Columns["Gain(dB)"].Width = 70; ;
                        this.dgvSweepResult.Columns["Icc(mA)"].Width = 70;
                        this.dgvSweepResult.Columns["PAE(%)"].Width = 60;
                        this.dgvSweepResult.Columns["ACP_EULTRA -10MHz(dB)"].Width = 80;
                        this.dgvSweepResult.Columns["ACP_EULTRA +10MHz(dB)"].Width = 80;
                        this.dgvSweepResult.Columns["ACP_ULTRA -2.5MHz(dB)"].Width = 80;
                        this.dgvSweepResult.Columns["ACP_ULTRA +2.5MHz(dB)"].Width = 80;
                        this.dgvSweepResult.Columns["ACP_ULTRA -7.5MHz(dB)"].Width = 80;
                        this.dgvSweepResult.Columns["ACP_ULTRA +7.5MHz(dB)"].Width = 80;
                        this.dgvSweepResult.Columns["EVM(%)"].Width = 70;

                        dgvSweepResult.ScrollBars = ScrollBars.Both;
                        dgvSweepResult.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        dgvSweepResult.AllowUserToOrderColumns = false;

                        foreach (DataGridViewTextBoxColumn dcTmp in dgvSweepResult.Columns)
                        {
                            dcTmp.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                            dcTmp.SortMode = DataGridViewColumnSortMode.NotSortable;
                        }
                        this.dgvSweepResult.AllowUserToAddRows = false;
                        this.dgvSweepResult.RowHeadersVisible = false;
                        this.dgvSweepResult.ReadOnly = true;


                        #endregion Initialize GridView

                        #region *** Waveform ***
                        cbxWaveform.Enabled = true;
                        cbxWaveform.Items.Clear();
                        cbxWaveform.Items.Add(Mod_Waveform_Name.LTEFDD);
                        cbxWaveform.Items.Add(Mod_Waveform_Name.LTEFDD_FULL);
                        cbxWaveform.SelectedIndex = 0;
                        #endregion *** Waveform ***
                        break;
                    }
                case TestSetting.MODE_CDMA:
                    {
                        #region *** Assign CDMA Parameter Setting ***
                        bool firstfreq = true;
                        TestSetting.LEVEL_VCC = TestSetting.SETTING_CDMA[TestSetting.NAME_VCC];
                        TestSetting.LEVEL_VBAT = TestSetting.SETTING_CDMA[TestSetting.NAME_VBAT];
                        //TestSetting.LEVEL_TXEN = TestSetting.SETTING_CDMA[TestSetting.NAME_TXEN];
                        TestSetting.LEVEL_GPCTRL = TestSetting.SETTING_CDMA[TestSetting.NAME_GPCTRL];
                        TestSetting.LEVEL_START = TestSetting.SETTING_CDMA[TestSetting.NAME_START];
                        TestSetting.LEVEL_STOP = TestSetting.SETTING_CDMA[TestSetting.NAME_STOP];
                        TestSetting.LEVEL_STEP = TestSetting.SETTING_CDMA[TestSetting.NAME_STEP];
                        TestSetting.LEVEL_PIN_VRAMP = TestSetting.SETTING_CDMA[TestSetting.NAME_PIN_VRAMP];


                        lblPin_Vramp.Text = "RF Pin(dBm)";
                        lblPin_Vramp.Text = "Vramp Voltage (V)";
                        lblStep.Text = "CDMA Target Power (dBm)";

                        tbxVCC.Text = TestSetting.LEVEL_VCC.ToString();
                        tbxVBAT.Text = TestSetting.LEVEL_VBAT.ToString();
                        tbxTXEN.Text = TestSetting.NA;
                        tbxPin_Vramp.Text = TestSetting.LEVEL_PIN_VRAMP.ToString(); ;
                        tbxGPCTRL.Text = TestSetting.LEVEL_GPCTRL.ToString();
                        tbxStart.Text = TestSetting.LEVEL_START.ToString();
                        tbxStop.Text = TestSetting.LEVEL_STOP.ToString();
                        tbxStep.Text = TestSetting.LEVEL_STEP.ToString();

                        tbxVBAT.Enabled = true;
                        tbxTXEN.Enabled = false;
                        tbxPin_Vramp.Enabled = true;

                        TestSetting.FREQLIST = new List<double>(TestSetting.FREQ_CDMA.Keys);

                        foreach (var freq in TestSetting.FREQLIST)
                        {
                            if (firstfreq)
                            {
                                tbxFreqList.Text = freq.ToString();
                                firstfreq = false;
                            }
                            else
                            {
                                tbxFreqList.Text += ",";
                                tbxFreqList.Text += freq.ToString();
                            }
                        }
                        #endregion

                        #region *** Initialize GridView ***
                        this.dgvSweepResult.DataSource = null;
                        this.dgvSweepResult.DataSource = this.dtCDMA;

                        this.dgvSweepResult.Columns["#"].Width = 25;
                        this.dgvSweepResult.Columns["Frequency (MHz)"].Width = 70;
                        this.dgvSweepResult.Columns["Target Pout(dBm)"].Width = 70;
                        this.dgvSweepResult.Columns["Pout(dBm)"].Width = 70;
                        this.dgvSweepResult.Columns["Pin(dBm)"].Width = 70;
                        this.dgvSweepResult.Columns["Gain(dB)"].Width = 70; ;
                        this.dgvSweepResult.Columns["Icc(mA)"].Width = 70;
                        this.dgvSweepResult.Columns["PAE(%)"].Width = 60;
                        this.dgvSweepResult.Columns["ACP -5MHz(dB)"].Width = 80;
                        this.dgvSweepResult.Columns["ACP +5MHz(dB)"].Width = 80;
                        this.dgvSweepResult.Columns["ACP -10MHz(dB)"].Width = 80;
                        this.dgvSweepResult.Columns["ACP +10MHz(dB)"].Width = 80;
                        this.dgvSweepResult.Columns["EVM(%)"].Width = 70;

                        dgvSweepResult.ScrollBars = ScrollBars.Both;
                        dgvSweepResult.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        dgvSweepResult.AllowUserToOrderColumns = false;

                        //for (int i = 0; i < dgvSweepResult.Columns.Count; i++)
                        //{
                        //    dgvSweepResult.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                        //}

                        foreach (DataGridViewTextBoxColumn dcTmp in dgvSweepResult.Columns)
                        {
                            dcTmp.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                            dcTmp.SortMode = DataGridViewColumnSortMode.NotSortable;
                        }
                        this.dgvSweepResult.AllowUserToAddRows = false;
                        this.dgvSweepResult.RowHeadersVisible = false;
                        this.dgvSweepResult.ReadOnly = true;


                        #endregion Initialize GridView

                        #region *** Waveform ***
                        cbxWaveform.Enabled = false;
                        cbxWaveform.Items.Clear();
                        cbxWaveform.Items.Add(Mod_Waveform_Name.CDMA_ACP);
                        cbxWaveform.Items.Add(Mod_Waveform_Name.CDMA_EVM);
                        cbxWaveform.SelectedIndex = 0;
                        #endregion *** Waveform ***
                        break;
                    }
                case TestSetting.MODE_EVDO:
                    {
                        #region *** Assign EVDO Parameter Setting ***
                        bool firstfreq = true;
                        TestSetting.LEVEL_VCC = TestSetting.SETTING_EVDO[TestSetting.NAME_VCC];
                        TestSetting.LEVEL_VBAT = TestSetting.SETTING_EVDO[TestSetting.NAME_VBAT];
                        //TestSetting.LEVEL_TXEN = TestSetting.SETTING_EVDO[TestSetting.NAME_TXEN];
                        TestSetting.LEVEL_GPCTRL = TestSetting.SETTING_EVDO[TestSetting.NAME_GPCTRL];
                        TestSetting.LEVEL_START = TestSetting.SETTING_EVDO[TestSetting.NAME_START];
                        TestSetting.LEVEL_STOP = TestSetting.SETTING_EVDO[TestSetting.NAME_STOP];
                        TestSetting.LEVEL_STEP = TestSetting.SETTING_EVDO[TestSetting.NAME_STEP];
                        TestSetting.LEVEL_PIN_VRAMP = TestSetting.SETTING_EVDO[TestSetting.NAME_PIN_VRAMP];


                        lblPin_Vramp.Text = "RF Pin(dBm)";
                        lblPin_Vramp.Text = "Vramp Voltage (V)";
                        lblStep.Text = "EVDO Target Power (dBm)";

                        tbxVCC.Text = TestSetting.LEVEL_VCC.ToString();
                        tbxVBAT.Text = TestSetting.LEVEL_VBAT.ToString();
                        tbxTXEN.Text = TestSetting.NA;
                        tbxPin_Vramp.Text = TestSetting.LEVEL_PIN_VRAMP.ToString(); ;
                        tbxGPCTRL.Text = TestSetting.LEVEL_GPCTRL.ToString();
                        tbxStart.Text = TestSetting.LEVEL_START.ToString();
                        tbxStop.Text = TestSetting.LEVEL_STOP.ToString();
                        tbxStep.Text = TestSetting.LEVEL_STEP.ToString();

                        tbxVBAT.Enabled = true;
                        tbxTXEN.Enabled = false;
                        tbxPin_Vramp.Enabled = true;

                        TestSetting.FREQLIST = new List<double>(TestSetting.FREQ_EVDO.Keys);

                        foreach (var freq in TestSetting.FREQLIST)
                        {
                            if (firstfreq)
                            {
                                tbxFreqList.Text = freq.ToString();
                                firstfreq = false;
                            }
                            else
                            {
                                tbxFreqList.Text += ",";
                                tbxFreqList.Text += freq.ToString();
                            }
                        }
                        #endregion

                        #region *** Initialize GridView ***
                        this.dgvSweepResult.DataSource = null;
                        this.dgvSweepResult.DataSource = this.dtEVDO;

                        this.dgvSweepResult.Columns["#"].Width = 25;
                        this.dgvSweepResult.Columns["Frequency (MHz)"].Width = 70;
                        this.dgvSweepResult.Columns["Target Pout(dBm)"].Width = 70;
                        this.dgvSweepResult.Columns["Pout(dBm)"].Width = 70;
                        this.dgvSweepResult.Columns["Pin(dBm)"].Width = 70;
                        this.dgvSweepResult.Columns["Gain(dB)"].Width = 70; ;
                        this.dgvSweepResult.Columns["Icc(mA)"].Width = 70;
                        this.dgvSweepResult.Columns["PAE(%)"].Width = 60;
                        this.dgvSweepResult.Columns["ACP -5MHz(dB)"].Width = 80;
                        this.dgvSweepResult.Columns["ACP +5MHz(dB)"].Width = 80;
                        this.dgvSweepResult.Columns["ACP -10MHz(dB)"].Width = 80;
                        this.dgvSweepResult.Columns["ACP +10MHz(dB)"].Width = 80;
                        this.dgvSweepResult.Columns["EVM(%)"].Width = 70;

                        dgvSweepResult.ScrollBars = ScrollBars.Both;
                        dgvSweepResult.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        dgvSweepResult.AllowUserToOrderColumns = false;

                        //for (int i = 0; i < dgvSweepResult.Columns.Count; i++)
                        //{
                        //    dgvSweepResult.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                        //}

                        foreach (DataGridViewTextBoxColumn dcTmp in dgvSweepResult.Columns)
                        {
                            dcTmp.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                            dcTmp.SortMode = DataGridViewColumnSortMode.NotSortable;
                        }
                        this.dgvSweepResult.AllowUserToAddRows = false;
                        this.dgvSweepResult.RowHeadersVisible = false;
                        this.dgvSweepResult.ReadOnly = true;


                        #endregion Initialize GridView

                        #region *** Waveform ***
                        cbxWaveform.Enabled = false;
                        cbxWaveform.Items.Clear();
                        cbxWaveform.Items.Add(Mod_Waveform_Name.EVDO_ACP);
                        cbxWaveform.Items.Add(Mod_Waveform_Name.EVDO_EVM);
                        cbxWaveform.SelectedIndex = 0;
                        #endregion *** Waveform ***
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }
        private void cbxEnableSetting_CheckedChanged(object sender, EventArgs e)
        {
            if (cbxEnableSetting.Checked)
            {
                gpParameter.Enabled = true;
                tbxVCC.Focus();
                lblError.Text = "Setting changed";
                lblError.ForeColor = Color.Black;
            }
            else
            {
                gpParameter.Enabled = false;
                lblError.Text = "";
                lblError.ForeColor = Color.Red;
            }
        }
        private void rbnDisplayON_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (rbnDisplayON.Checked)
                {
                    _MXA_N9020A.Display_Enable(true);
                    _PM_N1913A.Display_Enable(true);
                    intDelay_N1913A_Count = 10;
                }
                else
                {
                    _MXA_N9020A.Display_Enable(false);
                    _PM_N1913A.Display_Enable(false);
                    intDelay_N1913A_Count = 5;
                }
            }
            catch
            { }
        }
        private void cbxexthar_CheckedChanged(object sender, EventArgs e)
        {
            if (cbxexthar.Checked)
                ext_har = true;
            else
                ext_har = false;
        }

        private void SetVCC(double dblValue_in_Volts)
        {
            try
            {
                if (Program.Location == LocationList.SH_2 || Program.Location == LocationList.SH_3)
                {
                    _PS_66319B.SetCurrentRange(PS_66319B_Channel.Channel_1, dblVbat_Current_Limit);
                    _PS_66319B.SetVoltage(PS_66319B_Channel.Channel_1, dblValue_in_Volts);
                    _PS_66319B.SetOutput(PS_66319B_Channel.Channel_1, Output.ON);
                }
                else
                {
                    _PS_66332A.SetCurrentRange(dblVbat_Current_Limit);
                    _PS_66332A.SetVoltage(dblValue_in_Volts);
                    _PS_66332A.SetOutput(Output.ON);
                }
            }
            catch (Exception ex)
            {
                lblError.Text = ex.Message.ToString();
            }
        }
        private void SetVramp(double dblVrampValue_in_Volts)
        {
            //double dblDCOffset = dblTxEnableValue_in_Volts / 2;
            double dblDCOffset = 0;
            try
            {
                #region BJ_1
                if (Program.Location == LocationList.BJ_1)
                {
                    _Arb_33522A_USB.SetArbOut(Arb_Waveform_Type.Pulse, Arb_Channel.Channel_2, dblPulse_Freq_Gmsk, dblVrampValue_in_Volts, dblDCOffset, 25);
                    util.Wait(10);
                    _Arb_33522A_USB.SetHighVoltage(dblVrampValue_in_Volts, Arb_Channel.Channel_2);
                    util.Wait(20);
                }
                #endregion BJ_1

                #region SH_1
                else if (Program.Location == LocationList.SH_1)
                {
                    _Arb_33522A_USB.SetArbOut(Arb_Waveform_Type.Pulse, Arb_Channel.Channel_2, dblPulse_Freq_Gmsk, dblVrampValue_in_Volts, dblDCOffset, 25);
                    util.Wait(10);
                    _Arb_33522A_USB.SetHighVoltage(dblVrampValue_in_Volts, Arb_Channel.Channel_2);
                    util.Wait(20);
                }
                #endregion SH1

                #region SH_2 & SH3 & SH4
                else if (Program.Location == LocationList.SH_2 ||
                         Program.Location == LocationList.SH_3 ||
                         Program.Location == LocationList.SH_4)
                {
                    //_Arb_33522A.Initialize(200);
                    //util.Wait(20);
                    _Arb_33522A.SetArbOut(Arb_Waveform_Type.Pulse, Arb_Channel.Channel_2, dblPulse_Freq_Gmsk, dblVrampValue_in_Volts, dblDCOffset, 25);
                    util.Wait(10);
                    _Arb_33522A.SetHighVoltage(dblVrampValue_in_Volts, Arb_Channel.Channel_2);
                    util.Wait(20);
                    _Arb_33522A.SetBurstModeOFF();
                }
                #endregion SH_2 & SH3
                else
                {
                    throw new Exception("Bad Location");
                }
            }
            catch (Exception ex)
            {
                lblError.Text = ex.Message.ToString();
            }
        }
        private void SetGPCTRL(double dblVoltageValue_in_Volts)
        {
            try
            {
                if (Program.Location != LocationList.SH_2) _Arb_33220A.SetDCLevel(dblVoltageValue_in_Volts);
            }
            catch (Exception ex)
            {
                //lblError.Text = ex.Message.ToString();
            }
            finally
            {
 
            }
        }
        private void SetTXEnable(double dblTxEnableValue_in_Volts)
        {
            //double dblDCOffset = dblTxEnableValue_in_Volts / 2;
            double dblDCOffset = 0;
            try
            {
                #region BJ_1
                if (Program.Location == LocationList.BJ_1)
                {
                    _Arb_33522A_USB.SetArbOut(Arb_Waveform_Type.Pulse, Arb_Channel.Channel_1, dblPulse_Freq_Gmsk, dblTxEnableValue_in_Volts, dblDCOffset, 26);
                }
                #endregion BJ_1

                #region SH_1
                else if (Program.Location == LocationList.SH_1)
                {
                    _Arb_33522A_USB.SetArbOut(Arb_Waveform_Type.Pulse, Arb_Channel.Channel_1, dblPulse_Freq_Gmsk, dblTxEnableValue_in_Volts, dblDCOffset, 26);
                }
                #endregion SH1

                #region SH2 & SH3 & SH4
                else if (Program.Location == LocationList.SH_2 ||
                         Program.Location == LocationList.SH_3 ||
                         Program.Location == LocationList.SH_4)
                {
                    _Arb_33522A.SetArbOut(Arb_Waveform_Type.Pulse, Arb_Channel.Channel_1, dblPulse_Freq_Gmsk, dblTxEnableValue_in_Volts, dblDCOffset, 26);
                    _Arb_33522A.SetBurstModeOFF();
                }
                #endregion SH_2 & SH3
                else
                {
                    throw new Exception("Bad Location");
                }
            }
            catch (Exception ex)
            {
                lblError.Text = ex.Message.ToString();
            }

        }
        private void SetVEN(Arb_Channel Channel, double dblValue_in_Volts, bool isDC)
        {
            //double dblDCOffset = dblTxEnableValue_in_Volts / 2;
            double dblDCOffset = 0;
            try
            {
                #region BJ_1 & SH_1
                if (Program.Location == LocationList.BJ_1 ||
                    Program.Location == LocationList.SH_1)
                {
                    if (isDC)
                    {
                        util.Wait(10);
                        _Arb_33522A_USB.SetDC(Channel, dblValue_in_Volts);
                        util.Wait(10);
                    }
                    else
                    {
                        _Arb_33522A_USB.SetArbOut(Arb_Waveform_Type.Pulse, Channel, dblPulse_Freq_LTETDD, dblValue_in_Volts, dblDCOffset, 25);
                    }
                }
                #endregion BJ_1

                #region SH2 & SH3 & SH4
                else if (Program.Location == LocationList.SH_2 ||
                         Program.Location == LocationList.SH_3 ||
                         Program.Location == LocationList.SH_4)
                {
                    if (isDC)
                    {
                        util.Wait(10);
                        _Arb_33522A.SetDC(Channel, dblValue_in_Volts);
                        util.Wait(10);
                    }
                    else
                    {
                        //_Arb_33522A.Initialize(200);
                        _Arb_33522A.SetArbOut(Arb_Waveform_Type.Square, Channel, dblPulse_Freq_LTETDD, dblValue_in_Volts, dblDCOffset, 42, false);
                        util.Wait(10);
                        _Arb_33522A.SYNC_OUT(Channel, Output.ON);
                        util.Wait(10);
                        _Arb_33522A.SetBurstModeOFF();
                        //_Arb_33522A.SetBurstTrig(Channel, 1.9, 999);
                        //util.Wait(10);
                    }
                }
                #endregion SH_2 & SH3
                else
                {
                    throw new Exception("Bad Location");
                }
            }
            catch (Exception ex)
            {
                lblError.Text = ex.Message.ToString();
            }

        }

        private void SaveData()
        {
            if (isCWLBTested || isCWHBTested || isEDGELBTested || isEDGEHBTested || isTDSCDMATested || isWCDMATested ||
                isLTETDD_B38_Tested || isLTETDD_B40_Tested || isLTEFDD_B1_Tested || isLTEFDD_B2_Tested || isCDMA_Tested || isEVDO_Tested)
            {
                #region --- File diaglog ---
                // Displays a SaveFileDialog so the user can save the file
                // Get full path for save data file
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.Filter = "CSV File|*.csv|Text|*.txt";
                saveFileDialog1.Title = "Save as";

                if (saveFileDialog1.ShowDialog() != DialogResult.OK) return;

                string strSaveDataPath = saveFileDialog1.FileName;
                StreamWriter swData = new StreamWriter(strSaveDataPath);
                StringBuilder sbTitle = new StringBuilder();
                //Build test data title
                sbTitle.AppendLine(strProduct + "," + DateTime.Now.ToString());
                sbTitle.AppendLine();
                sbTitle.AppendLine();
                swData.WriteLine(sbTitle.ToString());

                #endregion --- File diaglog ---

                #region --- GMSK LB ---
                //Build and save CW LB test data title
                if (isCWLBTested)
                {
                    sbTitle = new StringBuilder();
                    for (int i = 0; i < dtCWLB.Columns.Count; i++)
                    {
                        if (i != 0)
                            sbTitle.Append(',');
                        sbTitle.Append(dtCWLB.Columns[i].ColumnName);
                    }
                    swData.WriteLine(sbTitle.ToString());

                    //Build and save LB test data 
                    foreach (DataRow drTemp in dtCWLB.Rows)
                    {
                        StringBuilder sbData = new StringBuilder();
                        for (int i = 0; i < dtCWLB.Columns.Count; i++)
                        {
                            if (i != 0)
                                sbData.Append(',');
                            sbData.Append(drTemp[i].ToString());
                        }
                        swData.WriteLine(sbData.ToString());
                    }
                }
                #endregion --- GMSK LB ---

                #region --- GMSK HB ---
                if (isCWHBTested)
                {
                    sbTitle = new StringBuilder();
                    sbTitle.AppendLine();
                    //Build and save CW HB test data title
                    for (int i = 0; i < dtCWHB.Columns.Count; i++)
                    {
                        if (i != 0)
                            sbTitle.Append(',');
                        sbTitle.Append(dtCWHB.Columns[i].ColumnName);
                    }
                    swData.WriteLine(sbTitle.ToString());

                    //Build and save HB test data 
                    foreach (DataRow drTemp in dtCWHB.Rows)
                    {
                        StringBuilder sbData = new StringBuilder();
                        for (int i = 0; i < dtCWHB.Columns.Count; i++)
                        {
                            if (i != 0)
                                sbData.Append(',');
                            sbData.Append(drTemp[i].ToString());
                        }
                        swData.WriteLine(sbData.ToString());
                    }
                }
                #endregion --- GMSK HB ---

                #region --- EDGE LB ---
                if (isEDGELBTested)
                {
                    sbTitle = new StringBuilder();
                    sbTitle.AppendLine();
                    //Build and save dtEDGEHB test data title
                    for (int i = 0; i < dtEDGELB.Columns.Count; i++)
                    {
                        if (i != 0)
                            sbTitle.Append(',');
                        sbTitle.Append(dtEDGELB.Columns[i].ColumnName);
                    }
                    swData.WriteLine(sbTitle.ToString());

                    //Build and save dtEDGEHB test data 
                    foreach (DataRow drTemp in dtEDGELB.Rows)
                    {
                        StringBuilder sbData = new StringBuilder();
                        for (int i = 0; i < dtEDGELB.Columns.Count; i++)
                        {
                            if (i != 0)
                                sbData.Append(',');
                            sbData.Append(drTemp[i].ToString());
                        }
                        swData.WriteLine(sbData.ToString());
                    }
                }
                #endregion --- dtEDGELB ---

                #region --- EDGE HB ---
                if (isEDGEHBTested)
                {
                    sbTitle = new StringBuilder();
                    sbTitle.AppendLine();
                    //Build and save dtEDGEHB test data title
                    for (int i = 0; i < dtEDGEHB.Columns.Count; i++)
                    {
                        if (i != 0)
                            sbTitle.Append(',');
                        sbTitle.Append(dtEDGEHB.Columns[i].ColumnName);
                    }
                    swData.WriteLine(sbTitle.ToString());

                    //Build and save dtEDGEHB test data 
                    foreach (DataRow drTemp in dtEDGEHB.Rows)
                    {
                        StringBuilder sbData = new StringBuilder();
                        for (int i = 0; i < dtEDGEHB.Columns.Count; i++)
                        {
                            if (i != 0)
                                sbData.Append(',');
                            sbData.Append(drTemp[i].ToString());
                        }
                        swData.WriteLine(sbData.ToString());
                    }
                }
                #endregion --- WCDMA ---

                #region --- TDSCDMA ---
                if (isTDSCDMATested)
                {
                    sbTitle = new StringBuilder();
                    sbTitle.AppendLine();
                    //Build and save TDSCDMA test data title
                    for (int i = 0; i < dtTDSCDMA.Columns.Count; i++)
                    {
                        if (i != 0)
                            sbTitle.Append(',');
                        sbTitle.Append(dtTDSCDMA.Columns[i].ColumnName);
                    }
                    swData.WriteLine(sbTitle.ToString());

                    //Build and save TDSCDMA test data 
                    foreach (DataRow drTemp in dtTDSCDMA.Rows)
                    {
                        StringBuilder sbData = new StringBuilder();
                        for (int i = 0; i < dtTDSCDMA.Columns.Count; i++)
                        {
                            if (i != 0)
                                sbData.Append(',');
                            sbData.Append(drTemp[i].ToString());
                        }
                        swData.WriteLine(sbData.ToString());
                    }
                }
                #endregion --- WCDMA ---

                #region --- WCDMA ---
                if (isWCDMATested)
                {
                    sbTitle = new StringBuilder();
                    sbTitle.AppendLine();
                    //Build and save WCDMA test data title
                    for (int i = 0; i < dtWCDMA.Columns.Count; i++)
                    {
                        if (i != 0)
                            sbTitle.Append(',');
                        sbTitle.Append(dtWCDMA.Columns[i].ColumnName);
                    }
                    swData.WriteLine(sbTitle.ToString());

                    //Build and save WCDMA test data 
                    foreach (DataRow drTemp in dtWCDMA.Rows)
                    {
                        StringBuilder sbData = new StringBuilder();
                        for (int i = 0; i < dtWCDMA.Columns.Count; i++)
                        {
                            if (i != 0)
                                sbData.Append(',');
                            sbData.Append(drTemp[i].ToString());
                        }
                        swData.WriteLine(sbData.ToString());
                    }
                }
                #endregion --- WCDMA ---

                #region --- LTETDD_B38 ---
                if (isLTETDD_B38_Tested)
                {
                    sbTitle = new StringBuilder();
                    sbTitle.AppendLine();
                    //Build and save LTETDD test data title
                    for (int i = 0; i < dtLTETDD_B38.Columns.Count; i++)
                    {
                        if (i != 0)
                            sbTitle.Append(',');
                        sbTitle.Append(dtLTETDD_B38.Columns[i].ColumnName);
                    }
                    swData.WriteLine(sbTitle.ToString());

                    //Build and save LTETDD test data 
                    foreach (DataRow drTemp in dtLTETDD_B38.Rows)
                    {
                        StringBuilder sbData = new StringBuilder();
                        for (int i = 0; i < dtLTETDD_B38.Columns.Count; i++)
                        {
                            if (i != 0)
                                sbData.Append(',');
                            sbData.Append(drTemp[i].ToString());
                        }
                        swData.WriteLine(sbData.ToString());
                    }
                }
                #endregion --- LTEFDD_B38 ---

                #region --- LTETDD_B40 ---
                if (isLTETDD_B40_Tested)
                {
                    sbTitle = new StringBuilder();
                    sbTitle.AppendLine();
                    //Build and save LTETDD test data title
                    for (int i = 0; i < dtLTETDD_B40.Columns.Count; i++)
                    {
                        if (i != 0)
                            sbTitle.Append(',');
                        sbTitle.Append(dtLTETDD_B40.Columns[i].ColumnName);
                    }
                    swData.WriteLine(sbTitle.ToString());

                    //Build and save LTETDD test data 
                    foreach (DataRow drTemp in dtLTETDD_B40.Rows)
                    {
                        StringBuilder sbData = new StringBuilder();
                        for (int i = 0; i < dtLTETDD_B40.Columns.Count; i++)
                        {
                            if (i != 0)
                                sbData.Append(',');
                            sbData.Append(drTemp[i].ToString());
                        }
                        swData.WriteLine(sbData.ToString());
                    }
                }
                #endregion --- LTEFDD_B40 ---

                #region --- LTEFDD_B1 ---
                if (isLTEFDD_B1_Tested)
                {
                    sbTitle = new StringBuilder();
                    sbTitle.AppendLine();
                    //Build and save LTEFDD test data title
                    for (int i = 0; i < dtLTEFDD_B1.Columns.Count; i++)
                    {
                        if (i != 0)
                            sbTitle.Append(',');
                        sbTitle.Append(dtLTEFDD_B1.Columns[i].ColumnName);
                    }
                    swData.WriteLine(sbTitle.ToString());

                    //Build and save LTEFDD test data 
                    foreach (DataRow drTemp in dtLTEFDD_B1.Rows)
                    {
                        StringBuilder sbData = new StringBuilder();
                        for (int i = 0; i < dtLTEFDD_B1.Columns.Count; i++)
                        {
                            if (i != 0)
                                sbData.Append(',');
                            sbData.Append(drTemp[i].ToString());
                        }
                        swData.WriteLine(sbData.ToString());
                    }
                }
                #endregion --- LTEFDD_B1 ---

                #region --- LTEFDD_B2 ---
                if (isLTEFDD_B2_Tested)
                {
                    sbTitle = new StringBuilder();
                    sbTitle.AppendLine();
                    //Build and save LTEFDD test data title
                    for (int i = 0; i < dtLTEFDD_B2.Columns.Count; i++)
                    {
                        if (i != 0)
                            sbTitle.Append(',');
                        sbTitle.Append(dtLTEFDD_B2.Columns[i].ColumnName);
                    }
                    swData.WriteLine(sbTitle.ToString());

                    //Build and save LTEFDD test data 
                    foreach (DataRow drTemp in dtLTEFDD_B2.Rows)
                    {
                        StringBuilder sbData = new StringBuilder();
                        for (int i = 0; i < dtLTEFDD_B2.Columns.Count; i++)
                        {
                            if (i != 0)
                                sbData.Append(',');
                            sbData.Append(drTemp[i].ToString());
                        }
                        swData.WriteLine(sbData.ToString());
                    }
                }
                #endregion --- LTEFDD_B2 ---

                #region --- CDMA ---
                if (isCDMA_Tested)
                {
                    sbTitle = new StringBuilder();
                    sbTitle.AppendLine();
                    //Build and save CDMA test data title
                    for (int i = 0; i < dtCDMA.Columns.Count; i++)
                    {
                        if (i != 0)
                            sbTitle.Append(',');
                        sbTitle.Append(dtCDMA.Columns[i].ColumnName);
                    }
                    swData.WriteLine(sbTitle.ToString());

                    //Build and save CDMA test data 
                    foreach (DataRow drTemp in dtCDMA.Rows)
                    {
                        StringBuilder sbData = new StringBuilder();
                        for (int i = 0; i < dtCDMA.Columns.Count; i++)
                        {
                            if (i != 0)
                                sbData.Append(',');
                            sbData.Append(drTemp[i].ToString());
                        }
                        swData.WriteLine(sbData.ToString());
                    }
                }
                #endregion --- CDMA ---

                #region --- EVDO ---
                if (isEVDO_Tested)
                {
                    sbTitle = new StringBuilder();
                    sbTitle.AppendLine();
                    //Build and save EVDO test data title
                    for (int i = 0; i < dtEVDO.Columns.Count; i++)
                    {
                        if (i != 0)
                            sbTitle.Append(',');
                        sbTitle.Append(dtEVDO.Columns[i].ColumnName);
                    }
                    swData.WriteLine(sbTitle.ToString());

                    //Build and save EVDO test data 
                    foreach (DataRow drTemp in dtEVDO.Rows)
                    {
                        StringBuilder sbData = new StringBuilder();
                        for (int i = 0; i < dtEVDO.Columns.Count; i++)
                        {
                            if (i != 0)
                                sbData.Append(',');
                            sbData.Append(drTemp[i].ToString());
                        }
                        swData.WriteLine(sbData.ToString());
                    }
                }
                #endregion --- EVDO ---

                swData.Close();
                isDataSaved = true;
                MessageBox.Show("Data have been saved to " + strSaveDataPath);
                this.Refresh();
            }
            else
            {
                MessageBox.Show("No any data need to be saved!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Refresh();
            }
        }
        private void BeforeTest()
        {
            this.Invoke((MethodInvoker)(delegate
            {
                btnTest.Text = "Initializing";
                btnTest.Enabled = false;
                btnTest.BackColor = Color.Yellow;
                Application.DoEvents();

                btnNext.Enabled = false;
                btnSave.Enabled = false;
                //btnStop.Enabled = true;
                gbDisplay.Enabled = false;
                gbMode.Enabled = false;
                //dgvSweepResult.Enabled = false;
                cbxEnableSetting.Checked = false;
                cbxEnableSetting.Enabled = false;
                gpParameter.Enabled = false;
            }));
        }
        private void AfterTest()
        {
            this.Invoke((MethodInvoker)(delegate
            {
                btnTest.Text = "Test";
                btnTest.Enabled = true;
                btnTest.BackColor = Color.Green;

                btnStop.Text = "Stop";
                btnStop.ForeColor = Color.Black;
                btnStop.Enabled = false;

                btnNext.Enabled = true;
                btnSave.Enabled = true;
                gbDisplay.Enabled = true;
                gbMode.Enabled = true;
                //dgvSweepResult.Enabled = true;
                cbxEnableSetting.Checked = false;
                cbxEnableSetting.Enabled = true;

                if (Program.Location == LocationList.BJ_1 ||
                    Program.Location == LocationList.SH_1)
                {
                    _E4438C.SetOutput(Output.OFF);
                    _PS_66332A.SetOutput(Output.OFF);
                    _Arb_33522A_USB.SetOutput(Output.OFF, Arb_Channel.Channel_1);
                    _Arb_33522A_USB.SetOutput(Output.OFF, Arb_Channel.Channel_2);
                }
                else if (Program.Location == LocationList.BJ_2 ||
                            Program.Location == LocationList.SH_2 ||
                            Program.Location == LocationList.SH_3 ||
                            Program.Location == LocationList.SH_4)
                {
                    _E4438C.SetOutput(Output.OFF);
                    _PS_66332A.SetOutput(Output.OFF);
                    _Arb_33522A.SetOutput(Output.OFF, Arb_Channel.Channel_1);
                    _Arb_33522A.SetOutput(Output.OFF, Arb_Channel.Channel_2);
                }
            }));
        }
        private void wait_2_start(DateTime StartTime)
        {
            int wait_time = 5;

            TimeSpan t_time = DateTime.Now - StartTime;
            if (t_time.Seconds < wait_time)
            {
                for (int i = wait_time - t_time.Seconds; i != 0; i--)
                {
                    btnTest.Text = i.ToString() + " seconds to start";
                    Application.DoEvents();

                    util.Wait(1000);
                }
            }
            btnTest.Text = "Testing";
            btnStop.Enabled = true;

        }


        private void btnTest_Click(object sender, EventArgs e)
        {
            StopTest = false;

            #region --- Group Test ---
            if (rbnCWLB.Checked)
            {
                #region --- GMSK LB ---
                //带参数的多线程
                Thread Test = new Thread(GSMK_TEST_NEW);
                Test.Priority = ThreadPriority.AboveNormal;
                Test.Start((object)BandList.B_1);
                isCWLBTested = true;

                #endregion --- GMSK LB ---
            }
            else if (rbnCWHB.Checked)
            {
                #region --- GMSK HB --- 
                //带参数的多线程
                Thread Test = new Thread(GSMK_TEST_NEW);
                Test.Priority = ThreadPriority.AboveNormal;
                Test.Start((object)BandList.B_2);
                isCWHBTested = true;

                #endregion --- GMSK HB ---
            }
            else if (rbnEDGELB.Checked)
            {
                #region --- EDGE LB ---

                //带参数的多线程
                Thread Test = new Thread(EDGE_TEST_NEW);
                Test.Priority = ThreadPriority.AboveNormal;
                Test.Start((object)BandList.B_1);
                isEDGELBTested = true;

                #endregion --- EDGE LB ---
            }
            else if (rbnEDGEHB.Checked)
            {
                #region --- EDGE HB ---

                //带参数的多线程
                Thread Test = new Thread(EDGE_TEST_NEW);
                Test.Priority = ThreadPriority.AboveNormal;
                Test.Start((object)BandList.B_2);
                isEDGEHBTested = true;

                #endregion --- EDGE HB ---
            }
            else if (rbnTDSCDMA.Checked)
            {
                #region --- TDSCDMA ---
                Thread Test = new Thread(new ThreadStart(TDSCDMA_TEST_NEW));
                Test.Priority = ThreadPriority.AboveNormal;
                Test.Start();
                isTDSCDMATested = true;

                #endregion --- TDSCDMA ---
            }
            else if (rbnWCDMA.Checked)
            {
                #region --- WCDMA ---
                Thread Test = new Thread(new ThreadStart(WCDMA_TEST_NEW));
                Test.Priority = ThreadPriority.AboveNormal;
                Test.Start();
                isWCDMATested = true;
                #endregion --- WCDMA ---
            }
            else if (rbnLTETDD_B38.Checked)
            {
                #region --- LTETDD B38 ---
                //带参数的多线程
                Thread Test = new Thread(LTETDD_TEST_NEW);
                Test.Priority = ThreadPriority.AboveNormal;
                Test.Start((object)BandList.B_1);
                isLTETDD_B38_Tested = true;
                #endregion --- LTETDD B38 ---
            }
            else if (rbnLTETDD_B40.Checked)
            {
                #region --- LTETDD B40---
                //带参数的多线程
                Thread Test = new Thread(LTETDD_TEST_NEW);
                Test.Priority = ThreadPriority.AboveNormal;
                Test.Start((object)BandList.B_2);
                isLTETDD_B40_Tested = true;
                #endregion --- LTETDD B40 ---
            }
            else if (rbnLTEFDDLB.Checked)
            {
                #region --- LTEFDD B1---
                //带参数的多线程
                Thread Test = new Thread(LTEFDD_TEST_NEW);
                Test.Priority = ThreadPriority.AboveNormal;
                Test.Start((object)BandList.B_1);
                isLTEFDD_B1_Tested = true;
                #endregion --- LTEFDD B1 ---
            }
            else if (rbnLTEFDDHB.Checked)
            {
                #region --- LTEFDD B2---
                //带参数的多线程
                Thread Test = new Thread(LTEFDD_TEST_NEW);
                Test.Priority = ThreadPriority.AboveNormal;
                Test.Start((object)BandList.B_2);
                isLTEFDD_B2_Tested = true;
                #endregion --- LTEFDD B2 ---
            }
            else if (rbnCDMA.Checked)
            {
                #region --- CDMA---
                //带参数的多线程
                Thread Test = new Thread(CDMA_TEST_NEW);
                Test.Priority = ThreadPriority.AboveNormal;
                Test.Start((object)BandList.B_1);

                isCDMA_Tested = true;
                
                #endregion --- CDMA ---
            }
            else if (rbnEVDo.Checked)
            {
                #region --- EVDO ---
                //带参数的多线程
                Thread Test = new Thread(CDMA_TEST_NEW);
                Test.Priority = ThreadPriority.AboveNormal;
                Test.Start((object)BandList.B_2);

                isEVDO_Tested = true;
                #endregion --- EVDO ---
            }
            #endregion --- Group Test ---

            this.Refresh();
            Application.DoEvents();
        }
        private void btnStop_Click(object sender, EventArgs e)
        {
            StopTest = true;

            btnStop.Text = "Stoping";
            btnStop.ForeColor = Color.Red;
            btnStop.Enabled = false;

            btnTest.Text = "Waiting to Stop";
            btnTest.BackColor = Color.OrangeRed;
        }
        private void btnNext_Click(object sender, EventArgs e)
        {

            //Check if data has been saved
            if (!isDataSaved)
            {
                if ((MessageBox.Show("Data is not been saved yet, do you want to save the data?"
                    , "Data is not saved", MessageBoxButtons.OKCancel, MessageBoxIcon.Question)) == DialogResult.OK)
                {
                    this.SaveData();
                }
            }

            if ((MessageBox.Show("Are you sure want to start a new test?", "New Test",
                                 MessageBoxButtons.YesNo, MessageBoxIcon.Question)) == DialogResult.Yes)
            {
                dtCWLB.Clear();
                dtCWHB.Clear();
                dtEDGELB.Clear();
                dtEDGEHB.Clear();
                dtTDSCDMA.Clear();
                dtWCDMA.Clear();
                dtLTETDD_B38.Clear();
                dtLTETDD_B40.Clear();
                dtLTEFDD_B1.Clear();
                dtLTEFDD_B2.Clear();


                isCWLBTested = false;
                isCWHBTested = false;
                isEDGELBTested = false;
                isEDGEHBTested = false;
                isTDSCDMATested = false;
                isWCDMATested = false;
                isLTETDD_B38_Tested = false;
                isLTETDD_B40_Tested = false;
                isLTEFDD_B1_Tested = false;
                isLTEFDD_B2_Tested = false;

                isDataSaved = false;

                rbnCWLB.Checked = true;
            }
            this.Refresh();
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            this.SaveData();
        }
        private void btnSettingSave_Click(object sender, EventArgs e)
        {
            #region --- Transfer change ---

            #region --- CW LB ---
            if (rbnCWLB.Checked)
            {
                // Setting
                TestSetting.SETTING_GMSK_LB[TestSetting.NAME_GPCTRL] = TestSetting.LEVEL_GPCTRL;
                TestSetting.SETTING_GMSK_LB[TestSetting.NAME_PIN_VRAMP] = TestSetting.LEVEL_PIN_VRAMP;
                TestSetting.SETTING_GMSK_LB[TestSetting.NAME_START] = TestSetting.LEVEL_START;
                TestSetting.SETTING_GMSK_LB[TestSetting.NAME_STEP] = TestSetting.LEVEL_STEP;
                TestSetting.SETTING_GMSK_LB[TestSetting.NAME_STOP] = TestSetting.LEVEL_STOP;
                TestSetting.SETTING_GMSK_LB[TestSetting.NAME_TXEN] = TestSetting.LEVEL_TXEN;
                //TestSetting.SETTING_GMSK[TestSetting.NAME_VBAT] = TestSetting.LEVEL_VBAT;
                TestSetting.SETTING_GMSK_LB[TestSetting.NAME_VCC] = TestSetting.LEVEL_VCC;

                //Frequency List
                TestSetting.FREQ_CW_LB.Clear();
                foreach (double tmp in TestSetting.FREQLIST)
                {
                    if (!TestSetting.FREQ_CW_LB.ContainsKey(tmp)) TestSetting.FREQ_CW_LB.Add(tmp, tmp);

                }
            }
            #endregion --- CW LB ---

            #region --- CW HB ---
            else if (rbnCWHB.Checked)
            {
                // Setting
                TestSetting.SETTING_GMSK_HB[TestSetting.NAME_GPCTRL] = TestSetting.LEVEL_GPCTRL;
                TestSetting.SETTING_GMSK_HB[TestSetting.NAME_PIN_VRAMP] = TestSetting.LEVEL_PIN_VRAMP;
                TestSetting.SETTING_GMSK_HB[TestSetting.NAME_START] = TestSetting.LEVEL_START;
                TestSetting.SETTING_GMSK_HB[TestSetting.NAME_STEP] = TestSetting.LEVEL_STEP;
                TestSetting.SETTING_GMSK_HB[TestSetting.NAME_STOP] = TestSetting.LEVEL_STOP;
                TestSetting.SETTING_GMSK_HB[TestSetting.NAME_TXEN] = TestSetting.LEVEL_TXEN;
                //TestSetting.SETTING_GMSK_HB[TestSetting.NAME_VBAT] = TestSetting.LEVEL_VBAT;
                TestSetting.SETTING_GMSK_HB[TestSetting.NAME_VCC] = TestSetting.LEVEL_VCC;

                //Frequency List
                TestSetting.FREQ_CW_HB.Clear();
                foreach (double tmp in TestSetting.FREQLIST)
                {
                    if (!TestSetting.FREQ_CW_HB.ContainsKey(tmp)) TestSetting.FREQ_CW_HB.Add(tmp, tmp);
                }
            }
            #endregion --- CW HB ---

            #region --- EDGE LB ---
            else if (rbnEDGELB.Checked)
            {
                // Setting
                TestSetting.SETTING_EDGE_LB[TestSetting.NAME_GPCTRL] = TestSetting.LEVEL_GPCTRL;
                TestSetting.SETTING_EDGE_LB[TestSetting.NAME_PIN_VRAMP] = TestSetting.LEVEL_PIN_VRAMP;
                TestSetting.SETTING_EDGE_LB[TestSetting.NAME_START] = TestSetting.LEVEL_START;
                TestSetting.SETTING_EDGE_LB[TestSetting.NAME_STEP] = TestSetting.LEVEL_STEP;
                TestSetting.SETTING_EDGE_LB[TestSetting.NAME_STOP] = TestSetting.LEVEL_STOP;
                TestSetting.SETTING_EDGE_LB[TestSetting.NAME_TXEN] = TestSetting.LEVEL_TXEN;
                //TestSetting.SETTING_EDGE_LB[TestSetting.NAME_VBAT] = TestSetting.LEVEL_VBAT;
                TestSetting.SETTING_EDGE_LB[TestSetting.NAME_VCC] = TestSetting.LEVEL_VCC;

                //Frequency List
                TestSetting.FREQ_EDGE_LB.Clear();
                foreach (double tmp in TestSetting.FREQLIST)
                {
                    if (!TestSetting.FREQ_EDGE_LB.ContainsKey(tmp)) TestSetting.FREQ_EDGE_LB.Add(tmp, tmp);
                }
            }
            #endregion --- EDGE LB ---

            #region --- EDGE HB ---
            else if (rbnEDGEHB.Checked)
            {
                // Setting
                TestSetting.SETTING_EDGE_HB[TestSetting.NAME_GPCTRL] = TestSetting.LEVEL_GPCTRL;
                TestSetting.SETTING_EDGE_HB[TestSetting.NAME_PIN_VRAMP] = TestSetting.LEVEL_PIN_VRAMP;
                TestSetting.SETTING_EDGE_HB[TestSetting.NAME_START] = TestSetting.LEVEL_START;
                TestSetting.SETTING_EDGE_HB[TestSetting.NAME_STEP] = TestSetting.LEVEL_STEP;
                TestSetting.SETTING_EDGE_HB[TestSetting.NAME_STOP] = TestSetting.LEVEL_STOP;
                TestSetting.SETTING_EDGE_HB[TestSetting.NAME_TXEN] = TestSetting.LEVEL_TXEN;
                TestSetting.SETTING_EDGE_HB[TestSetting.NAME_VBAT] = TestSetting.LEVEL_VBAT;
                TestSetting.SETTING_EDGE_HB[TestSetting.NAME_VCC] = TestSetting.LEVEL_VCC;

                //Frequency List
                TestSetting.FREQ_EDGE_HB.Clear();
                foreach (double tmp in TestSetting.FREQLIST)
                {
                    if (!TestSetting.FREQ_EDGE_HB.ContainsKey(tmp)) TestSetting.FREQ_EDGE_HB.Add(tmp, tmp);
                }
            }
            #endregion --- EDGE HB ---

            #region --- TDSCDMA ---
            else if (rbnTDSCDMA.Checked)
            {
                // Setting
                TestSetting.SETTING_TDSCDMA[TestSetting.NAME_GPCTRL] = TestSetting.LEVEL_GPCTRL;
                TestSetting.SETTING_TDSCDMA[TestSetting.NAME_PIN_VRAMP] = TestSetting.LEVEL_PIN_VRAMP;
                TestSetting.SETTING_TDSCDMA[TestSetting.NAME_START] = TestSetting.LEVEL_START;
                TestSetting.SETTING_TDSCDMA[TestSetting.NAME_STEP] = TestSetting.LEVEL_STEP;
                TestSetting.SETTING_TDSCDMA[TestSetting.NAME_STOP] = TestSetting.LEVEL_STOP;
                TestSetting.SETTING_TDSCDMA[TestSetting.NAME_TXEN] = TestSetting.LEVEL_TXEN;
                //TestSetting.SETTING_TDSCDMA[TestSetting.NAME_VBAT] = TestSetting.LEVEL_VBAT;
                TestSetting.SETTING_TDSCDMA[TestSetting.NAME_VCC] = TestSetting.LEVEL_VCC;

                //Frequency List
                TestSetting.FREQ_TDSCDMA.Clear();
                foreach (double tmp in TestSetting.FREQLIST)
                {
                    if (!TestSetting.FREQ_TDSCDMA.ContainsKey(tmp)) TestSetting.FREQ_TDSCDMA.Add(tmp, tmp);
                }
            }
            #endregion --- TDSCDMA ---

            #region --- WCDMA ---
            else if (rbnWCDMA.Checked)
            {
                // Setting
                TestSetting.SETTING_WCDMA[TestSetting.NAME_GPCTRL] = TestSetting.LEVEL_GPCTRL;
                TestSetting.SETTING_WCDMA[TestSetting.NAME_PIN_VRAMP] = TestSetting.LEVEL_PIN_VRAMP;
                TestSetting.SETTING_WCDMA[TestSetting.NAME_START] = TestSetting.LEVEL_START;
                TestSetting.SETTING_WCDMA[TestSetting.NAME_STEP] = TestSetting.LEVEL_STEP;
                TestSetting.SETTING_WCDMA[TestSetting.NAME_STOP] = TestSetting.LEVEL_STOP;
                //TestSetting.SETTING_WCDMA[TestSetting.NAME_TXEN] = TestSetting.LEVEL_TXEN;
                TestSetting.SETTING_WCDMA[TestSetting.NAME_VBAT] = TestSetting.LEVEL_VBAT;
                TestSetting.SETTING_WCDMA[TestSetting.NAME_VCC] = TestSetting.LEVEL_VCC;

                //Frequency List
                TestSetting.FREQ_WCDMA.Clear();
                foreach (double tmp in TestSetting.FREQLIST)
                {
                    if (!TestSetting.FREQ_WCDMA.ContainsKey(tmp)) TestSetting.FREQ_WCDMA.Add(tmp, tmp);
                }
            }
            #endregion --- WCDMA ---

            #region --- LTETDD_B38 ---
            else if (rbnLTETDD_B38.Checked)
            {
                // Setting
                TestSetting.SETTING_LTETDD_B38[TestSetting.NAME_MODE] = TestSetting.LEVEL_GPCTRL;
                TestSetting.SETTING_LTETDD_B38[TestSetting.NAME_PIN_VRAMP] = TestSetting.LEVEL_PIN_VRAMP;
                TestSetting.SETTING_LTETDD_B38[TestSetting.NAME_START] = TestSetting.LEVEL_START;
                TestSetting.SETTING_LTETDD_B38[TestSetting.NAME_STEP] = TestSetting.LEVEL_STEP;
                TestSetting.SETTING_LTETDD_B38[TestSetting.NAME_STOP] = TestSetting.LEVEL_STOP;
                TestSetting.SETTING_LTETDD_B38[TestSetting.NAME_VEN] = TestSetting.LEVEL_TXEN;
                TestSetting.SETTING_LTETDD_B38[TestSetting.NAME_VCC] = TestSetting.LEVEL_VCC;

                //Frequency List
                TestSetting.FREQ_LTETDD_B38.Clear();
                foreach (double tmp in TestSetting.FREQLIST)
                {
                    if (!TestSetting.FREQ_LTETDD_B38.ContainsKey(tmp)) TestSetting.FREQ_LTETDD_B38.Add(tmp, tmp);
                }
            }
            #endregion --- LTETDD_B38 ---

            #region --- LTETDD_B40 ---
            else if (rbnLTETDD_B40.Checked)
            {
                // Setting
                TestSetting.SETTING_LTETDD_B40[TestSetting.NAME_MODE] = TestSetting.LEVEL_GPCTRL;
                TestSetting.SETTING_LTETDD_B40[TestSetting.NAME_PIN_VRAMP] = TestSetting.LEVEL_PIN_VRAMP;
                TestSetting.SETTING_LTETDD_B40[TestSetting.NAME_START] = TestSetting.LEVEL_START;
                TestSetting.SETTING_LTETDD_B40[TestSetting.NAME_STEP] = TestSetting.LEVEL_STEP;
                TestSetting.SETTING_LTETDD_B40[TestSetting.NAME_STOP] = TestSetting.LEVEL_STOP;
                TestSetting.SETTING_LTETDD_B40[TestSetting.NAME_VEN] = TestSetting.LEVEL_TXEN;
                TestSetting.SETTING_LTETDD_B40[TestSetting.NAME_VCC] = TestSetting.LEVEL_VCC;

                //Frequency List
                TestSetting.FREQ_LTETDD_B40.Clear();
                foreach (double tmp in TestSetting.FREQLIST)
                {
                    if (!TestSetting.FREQ_LTETDD_B40.ContainsKey(tmp)) TestSetting.FREQ_LTETDD_B40.Add(tmp, tmp);
                }
            }
            #endregion --- LTETDD_B40 ---

            #region --- LTEFDD_LB ---
            else if (rbnLTEFDDLB.Checked)
            {
                // Setting
                TestSetting.SETTING_LTEFDD_LB[TestSetting.NAME_MODE] = TestSetting.LEVEL_GPCTRL;
                TestSetting.SETTING_LTEFDD_LB[TestSetting.NAME_PIN_VRAMP] = TestSetting.LEVEL_PIN_VRAMP;
                TestSetting.SETTING_LTEFDD_LB[TestSetting.NAME_START] = TestSetting.LEVEL_START;
                TestSetting.SETTING_LTEFDD_LB[TestSetting.NAME_STEP] = TestSetting.LEVEL_STEP;
                TestSetting.SETTING_LTEFDD_LB[TestSetting.NAME_STOP] = TestSetting.LEVEL_STOP;
                TestSetting.SETTING_LTEFDD_LB[TestSetting.NAME_VEN] = TestSetting.LEVEL_TXEN;
                TestSetting.SETTING_LTEFDD_LB[TestSetting.NAME_VCC] = TestSetting.LEVEL_VCC;

                //Frequency List
                TestSetting.FREQ_LTEFDD_LB.Clear();
                foreach (double tmp in TestSetting.FREQLIST)
                {
                    if (!TestSetting.FREQ_LTEFDD_LB.ContainsKey(tmp)) TestSetting.FREQ_LTEFDD_LB.Add(tmp, tmp);
                }
            }
            #endregion --- LTEFDD_LB ---

            #region --- LTEFDD_HB ---
            else if (rbnLTEFDDHB.Checked)
            {
                // Setting
                TestSetting.SETTING_LTEFDD_HB[TestSetting.NAME_MODE] = TestSetting.LEVEL_GPCTRL;
                TestSetting.SETTING_LTEFDD_HB[TestSetting.NAME_PIN_VRAMP] = TestSetting.LEVEL_PIN_VRAMP;
                TestSetting.SETTING_LTEFDD_HB[TestSetting.NAME_START] = TestSetting.LEVEL_START;
                TestSetting.SETTING_LTEFDD_HB[TestSetting.NAME_STEP] = TestSetting.LEVEL_STEP;
                TestSetting.SETTING_LTEFDD_HB[TestSetting.NAME_STOP] = TestSetting.LEVEL_STOP;
                TestSetting.SETTING_LTEFDD_HB[TestSetting.NAME_VEN] = TestSetting.LEVEL_TXEN;
                TestSetting.SETTING_LTEFDD_HB[TestSetting.NAME_VCC] = TestSetting.LEVEL_VCC;

                //Frequency List
                TestSetting.FREQ_LTEFDD_HB.Clear();
                foreach (double tmp in TestSetting.FREQLIST)
                {
                    if (!TestSetting.FREQ_LTEFDD_HB.ContainsKey(tmp)) TestSetting.FREQ_LTEFDD_HB.Add(tmp, tmp);
                }
            }
            #endregion --- LTEFDD_HB ---

            #region --- CDMA ---
            else if (rbnCDMA.Checked)
            {
                // Setting
                TestSetting.SETTING_CDMA[TestSetting.NAME_GPCTRL] = TestSetting.LEVEL_GPCTRL;
                TestSetting.SETTING_CDMA[TestSetting.NAME_PIN_VRAMP] = TestSetting.LEVEL_PIN_VRAMP;
                TestSetting.SETTING_CDMA[TestSetting.NAME_START] = TestSetting.LEVEL_START;
                TestSetting.SETTING_CDMA[TestSetting.NAME_STEP] = TestSetting.LEVEL_STEP;
                TestSetting.SETTING_CDMA[TestSetting.NAME_STOP] = TestSetting.LEVEL_STOP;
                //TestSetting.SETTING_CDMA[TestSetting.NAME_TXEN] = TestSetting.LEVEL_TXEN;
                TestSetting.SETTING_CDMA[TestSetting.NAME_VBAT] = TestSetting.LEVEL_VBAT;
                TestSetting.SETTING_CDMA[TestSetting.NAME_VCC] = TestSetting.LEVEL_VCC;

                //Frequency List
                TestSetting.FREQ_CDMA.Clear();
                foreach (double tmp in TestSetting.FREQLIST)
                {
                    if (!TestSetting.FREQ_CDMA.ContainsKey(tmp)) TestSetting.FREQ_CDMA.Add(tmp, tmp);
                }
            }
            #endregion --- CDMA ---

            #region --- EVDO ---
            else if (rbnEVDo.Checked)
            {
                // Setting
                TestSetting.SETTING_EVDO[TestSetting.NAME_GPCTRL] = TestSetting.LEVEL_GPCTRL;
                TestSetting.SETTING_EVDO[TestSetting.NAME_PIN_VRAMP] = TestSetting.LEVEL_PIN_VRAMP;
                TestSetting.SETTING_EVDO[TestSetting.NAME_START] = TestSetting.LEVEL_START;
                TestSetting.SETTING_EVDO[TestSetting.NAME_STEP] = TestSetting.LEVEL_STEP;
                TestSetting.SETTING_EVDO[TestSetting.NAME_STOP] = TestSetting.LEVEL_STOP;
                //TestSetting.SETTING_EVDO[TestSetting.NAME_TXEN] = TestSetting.LEVEL_TXEN;
                TestSetting.SETTING_EVDO[TestSetting.NAME_VBAT] = TestSetting.LEVEL_VBAT;
                TestSetting.SETTING_EVDO[TestSetting.NAME_VCC] = TestSetting.LEVEL_VCC;

                //Frequency List
                TestSetting.FREQ_EVDO.Clear();
                foreach (double tmp in TestSetting.FREQLIST)
                {
                    if (!TestSetting.FREQ_EVDO.ContainsKey(tmp)) TestSetting.FREQ_EVDO.Add(tmp, tmp);
                }
            }
            #endregion --- EVDO ---

            #endregion --- Transfer change ---

            #region --- Save file ---
            StreamWriter swcfg = new StreamWriter(Program.strSweepParameter_FileName);
            StringBuilder sbcfg = new StringBuilder();

            sbcfg.AppendLine("for sweep test use, please do not make any change if you know what it is means.......Ace");
            sbcfg.AppendLine("--- Setting ---");

            // Setting GMSK_LB
            sbcfg.AppendLine("--- GMSK_LB ---");
            foreach (KeyValuePair<string, double> tmp in TestSetting.SETTING_GMSK_LB)
            {
                sbcfg.AppendLine(tmp.Key + "," + tmp.Value);
            }
            // Setting GMSK_HB
            sbcfg.AppendLine("--- GMSK_HB ---");
            foreach (KeyValuePair<string, double> tmp in TestSetting.SETTING_GMSK_HB)
            {
                sbcfg.AppendLine(tmp.Key + "," + tmp.Value);
            }
            // Setting EDGE_LB
            sbcfg.AppendLine("--- EDGE_LB ---");
            foreach (KeyValuePair<string, double> tmp in TestSetting.SETTING_EDGE_LB)
            {
                sbcfg.AppendLine(tmp.Key + "," + tmp.Value);
            }
            // Setting EDGE
            sbcfg.AppendLine("--- EDGE_HB ---");
            foreach (KeyValuePair<string, double> tmp in TestSetting.SETTING_EDGE_HB)
            {
                sbcfg.AppendLine(tmp.Key + "," + tmp.Value);
            }
            // Setting TDSCDMA
            sbcfg.AppendLine("--- TDSCDMA ---");
            foreach (KeyValuePair<string, double> tmp in TestSetting.SETTING_TDSCDMA)
            {
                sbcfg.AppendLine(tmp.Key + "," + tmp.Value);
            }
            // Setting WCDMA
            sbcfg.AppendLine("--- WCDMA ---");
            foreach (KeyValuePair<string, double> tmp in TestSetting.SETTING_WCDMA)
            {
                sbcfg.AppendLine(tmp.Key + "," + tmp.Value);
            }
            // Setting LTETDD_B38
            sbcfg.AppendLine("--- LTETDD_B38 ---");
            foreach (KeyValuePair<string, double> tmp in TestSetting.SETTING_LTETDD_B38)
            {
                sbcfg.AppendLine(tmp.Key + "," + tmp.Value);
            }
            // Setting LTETDD_B40
            sbcfg.AppendLine("--- LTETDD_B40 ---");
            foreach (KeyValuePair<string, double> tmp in TestSetting.SETTING_LTETDD_B40)
            {
                sbcfg.AppendLine(tmp.Key + "," + tmp.Value);
            }
            // Setting LTEFDD_LB
            sbcfg.AppendLine("--- LTEFDD_LB ---");
            foreach (KeyValuePair<string, double> tmp in TestSetting.SETTING_LTEFDD_LB)
            {
                sbcfg.AppendLine(tmp.Key + "," + tmp.Value);
            }
            // Setting LTEFDD_HB
            sbcfg.AppendLine("--- LTEFDD_HB ---");
            foreach (KeyValuePair<string, double> tmp in TestSetting.SETTING_LTEFDD_HB)
            {
                sbcfg.AppendLine(tmp.Key + "," + tmp.Value);
            }
            // Setting CDMA
            sbcfg.AppendLine("--- CDMA ---");
            foreach (KeyValuePair<string, double> tmp in TestSetting.SETTING_CDMA)
            {
                sbcfg.AppendLine(tmp.Key + "," + tmp.Value);
            }
            // Setting EVDO
            sbcfg.AppendLine("--- EVDO ---");
            foreach (KeyValuePair<string, double> tmp in TestSetting.SETTING_EVDO)
            {
                sbcfg.AppendLine(tmp.Key + "," + tmp.Value);
            }



            // Frequency List
            sbcfg.AppendLine("--- Frequency List ---");

            // Frequency GMSK LB
            sbcfg.AppendLine("--- CW LB ---");
            foreach (KeyValuePair<double, double> tmp in TestSetting.FREQ_CW_LB)
            {
                sbcfg.AppendLine(tmp.Key.ToString());
            }
            // Frequency GMSK HB
            sbcfg.AppendLine("--- CW HB ---");
            foreach (KeyValuePair<double, double> tmp in TestSetting.FREQ_CW_HB)
            {
                sbcfg.AppendLine(tmp.Key.ToString());
            }
            // Frequency EDGE LB
            sbcfg.AppendLine("--- EDGE LB ---");
            foreach (KeyValuePair<double, double> tmp in TestSetting.FREQ_EDGE_LB)
            {
                sbcfg.AppendLine(tmp.Key.ToString());
            }
            // Frequency EDGE HB
            sbcfg.AppendLine("--- EDGE HB ---");
            foreach (KeyValuePair<double, double> tmp in TestSetting.FREQ_EDGE_HB)
            {
                sbcfg.AppendLine(tmp.Key.ToString());
            }
            // Frequency TDSCDMA
            sbcfg.AppendLine("--- TDSCDMA ---");
            foreach (KeyValuePair<double, double> tmp in TestSetting.FREQ_TDSCDMA)
            {
                sbcfg.AppendLine(tmp.Key.ToString());
            }
            // Frequency WCDMA
            sbcfg.AppendLine("--- WCDMA ---");
            foreach (KeyValuePair<double, double> tmp in TestSetting.FREQ_WCDMA)
            {
                sbcfg.AppendLine(tmp.Key.ToString());
            }
            // Frequency LTETDD_B38
            sbcfg.AppendLine("--- LTETDD_B38 ---");
            foreach (KeyValuePair<double, double> tmp in TestSetting.FREQ_LTETDD_B38)
            {
                sbcfg.AppendLine(tmp.Key.ToString());
            }
            // Frequency LTETDD_B40
            sbcfg.AppendLine("--- LTETDD_B40 ---");
            foreach (KeyValuePair<double, double> tmp in TestSetting.FREQ_LTETDD_B40)
            {
                sbcfg.AppendLine(tmp.Key.ToString());
            }
            // Frequency LTEFDD_LB
            sbcfg.AppendLine("--- LTEFDD_LB ---");
            foreach (KeyValuePair<double, double> tmp in TestSetting.FREQ_LTEFDD_LB)
            {
                sbcfg.AppendLine(tmp.Key.ToString());
            }
            // Frequency LTEFDD_HB
            sbcfg.AppendLine("--- LTEFDD_HB ---");
            foreach (KeyValuePair<double, double> tmp in TestSetting.FREQ_LTEFDD_HB)
            {
                sbcfg.AppendLine(tmp.Key.ToString());
            }
            // Frequency CDMA
            sbcfg.AppendLine("--- CDMA ---");
            foreach (KeyValuePair<double, double> tmp in TestSetting.FREQ_CDMA)
            {
                sbcfg.AppendLine(tmp.Key.ToString());
            }
            // Frequency EVDO
            sbcfg.AppendLine("--- EVDO ---");
            foreach (KeyValuePair<double, double> tmp in TestSetting.FREQ_EVDO)
            {
                sbcfg.AppendLine(tmp.Key.ToString());
            }



            sbcfg.AppendLine("--- The End ---");

            swcfg.Write(sbcfg.ToString());
            swcfg.Close();
            #endregion --- Save file ---

            if (false)
            {
                DialogResult = MessageBox.Show("Please restart current program to perform the loss comp, if new frequency has been added.\r\n  - Yes to restart now\r\n  - No to restart later", "Setting Saved", MessageBoxButtons.YesNo);

                if (DialogResult == System.Windows.Forms.DialogResult.Yes)
                {
                    Application.Exit();
                    System.Diagnostics.Process.Start(@Application.ExecutablePath);
                }
            }
        }



        #region --- BJ_1 Test ---

        void CWLB_Test_BJ_1()
        {
            #region --- Variable Define ---

            dicVramp.Clear();
            int intTestID = 1;
            double[] dblResult = new double[10];

            DataTable dtCWLBTMP = new DataTable();
            dtCWLBTMP = dtCWLB.Clone();

            if (!cbxKeepPrevious.Checked)
            {
                this.Invoke((MethodInvoker)(delegate
                {
                    dtCWLB.Clear();
                    dgvSweepResult.Refresh();
                }));
            }

            #endregion --- Variable Define ---

            #region --- Initialize ---
            BeforeTest();
            SetVCC(TestSetting.LEVEL_VCC);
            SetTXEnable(TestSetting.LEVEL_TXEN);
            SetGPCTRL(TestSetting.LEVEL_GPCTRL);

            _E4438C.Initialize();
            //_PM_N1913A.Initialize(rbnDisplayON.Checked);
            _MXA_N9020A.Initialize(rbnDisplayON.Checked);
            _E4438C.SetOutput(Output.ON);

            string Content = "Make sure everything is setup for TX LB testing \r\n" +
                            "  1) Connect rf source cable to TX LB \r\n" +
                            "  2) Set Control box to TX LB mode \r\n" +
                            "  3) Connect / Change LB highpass filter";

            this.Invoke((MethodInvoker)(delegate
            {
                MessageBox.Show(this, Content, "GMSK LB Testing", MessageBoxButtons.OK);
                Application.DoEvents();
            }));

            #endregion --- Initialize ---

            #region --- Vramp Sweep ---
            for (double dblVramp = TestSetting.LEVEL_START; dblVramp < TestSetting.LEVEL_STOP + 0.01; dblVramp += TestSetting.LEVEL_STEP)
            {
                if (StopTest) break;

                SetVramp(dblVramp);

                foreach (double dblFreq in TestSetting.FREQLIST)
                {
                    if (StopTest) break;
                    _E4438C.SetFrequency(dblFreq);
                    _E4438C.SetPower(TestSetting.LEVEL_PIN_VRAMP + TestSetting.LOSS_SRC[dblFreq]);

                    // Frequency
                    dblResult[0] = dblFreq;

                    // Vramp
                    dblResult[1] = dblVramp;

                    // Pout
                    //_PM_N1913A.Configure__Average_Pulse_Power_with_Duty_Cycle(dblFreq, 25, 10);
                    _PM_U2001A.Configure__Time_Gated_Burst_Power(dblFreq, U2001_Trigger.Internal, -35, 0.6, 0.36);
                    util.Wait(intDelay_PowerMeter);
                    dblResult[2] = _PM_U2001A.GetPowerResult() + TestSetting.LOSS_MSR_POUT[dblFreq];

                    // ICC
                    dblResult[3] = _PS_66332A.High_Current();
                    dblResult[3] = dblResult[3] * 1000;

                    // PAE
                    dblResult[4] = (Math.Pow(10, ((dblResult[2] - 30) / 10))) / TestSetting.LEVEL_VCC / dblResult[3] * 1000;
                    dblResult[4] = dblResult[4] * 100;

                    // harmonic 
                    for (int i = 2; i <= 6; i++)
                    {
                        _MXA_N9020A.SetFrequency(i * dblFreq);

                        util.Wait(intDelay_MXA);

                        dblResult[3 + i] = _MXA_N9020A.Get_CW_PowerResult() + TestSetting.LOSS_MSR_FILTER_LB[i * dblFreq];
                    }
                    // Update_Grid
                    DataRow drNew = dtCWLB.NewRow();
                    DataRow drNewTmp = dtCWLBTMP.NewRow();
                    drNewTmp[0] = drNew[0] = intTestID++;
                    for (int i = 0; i < dblResult.Count(); i++)
                    {
                        drNewTmp[i + 1] = drNew[i + 1] = Math.Round(dblResult[i], 2);
                    }
                    dtCWLBTMP.Rows.Add(drNewTmp);
                    this.Invoke((MethodInvoker)(delegate
                    {
                        dtCWLB.Rows.Add(drNew);
                        dgvSweepResult.Refresh();
                        Application.DoEvents();
                    }));
                } // Frequency loop
            }   // Vramp loop
            #endregion Vramp Sweep

            #region --- Power Servo ---
            foreach (double dblFreq in TestSetting.FREQLIST)
            {
                if (StopTest) break;

                int Count = 0;
                _E4438C.SetFrequency(dblFreq);
                _E4438C.SetPower(TestSetting.LEVEL_PIN_VRAMP + TestSetting.LOSS_SRC[dblFreq]);

                #region Search power
                double LoopResult_Low;
                double LoopResult_High;
                double LoopResult;
                double Slope_mV;

                this.SetVramp(1.2);
                //_PM_N1913A.Configure__Average_Pulse_Power_with_Duty_Cycle(dblFreq, 25, 10);
                _PM_U2001A.Configure__Time_Gated_Burst_Power(dblFreq, U2001_Trigger.Internal, -35, 0.6, 0.36);
                util.Wait(intDelay_PowerMeter);
                LoopResult_Low = _PM_U2001A.GetPowerResult();

                this.SetVramp(1.6);
                util.Wait(intDelay_PowerMeter * intDelay_N1913A_Count);
                LoopResult_High = _PM_U2001A.GetPowerResult();

                Slope_mV = (LoopResult_High - LoopResult_Low) / 200;

                LoopResult = LoopResult_Low + TestSetting.LOSS_MSR_POUT[dblFreq];
                dblRatedVramp = 1.2 + (TestSetting.TARGET_POUT_CWLB - LoopResult) / Slope_mV / 1000;

                if (dblRatedVramp < 0.8 || dblRatedVramp > 1.8) dblRatedVramp = 1.8;

                while (Math.Abs(LoopResult - TestSetting.TARGET_POUT_CWLB) > 0.05 && dblRatedVramp < 1.8 && dblRatedVramp > 0.8)
                {
                    this.SetVramp(dblRatedVramp);
                    util.Wait(intDelay_PowerMeter);
                    LoopResult = _PM_U2001A.GetPowerResult() + TestSetting.LOSS_MSR_POUT[dblFreq];
                    dblRatedVramp = dblRatedVramp + (TestSetting.TARGET_POUT_CWLB - LoopResult) / Slope_mV / 1000;
                    Count++;
                    if (Count > 20) break;
                }
                #endregion Search power

                // Frequency
                dblResult[0] = dblFreq;
                // Rated Vramp
                dblResult[1] = dblRatedVramp;
                if (!dicVramp.ContainsKey(dblFreq)) dicVramp.Add(dblFreq, Math.Round(dblRatedVramp, 2));
                //Rated Pout
                dblResult[2] = LoopResult;

                // Rated ICC
                dblResult[3] = _PS_66332A.High_Current();
                dblResult[3] = dblResult[3] * 1000;

                // Rated PAE
                dblResult[4] = (Math.Pow(10, ((dblResult[2] - 30) / 10))) / TestSetting.LEVEL_VCC / dblResult[3] * 1000;
                dblResult[4] = dblResult[4] * 100;

                // Rated harmonic 
                for (int i = 2; i <= 6; i++)
                {
                    _MXA_N9020A.SetFrequency(i * dblFreq);
                    util.Wait(intDelay_MXA);
                    dblResult[3 + i] = _MXA_N9020A.Get_CW_PowerResult() + TestSetting.LOSS_MSR_FILTER_LB[i * dblFreq];
                }
                // Update_Grid
                DataRow drNew = dtCWLB.NewRow();
                DataRow drNewTmp = dtCWLBTMP.NewRow();
                drNewTmp[0] = drNew[0] = intTestID++;
                for (int i = 0; i < dblResult.Count(); i++)
                {
                    drNewTmp[i + 1] = drNew[i + 1] = Math.Round(dblResult[i], 2);
                }
                dtCWLBTMP.Rows.Add(drNewTmp);
                this.Invoke((MethodInvoker)(delegate
                {
                    dtCWLB.Rows.Add(drNew);
                    dgvSweepResult.Refresh();
                    Application.DoEvents();
                }));

            } // Frequency loop for power servo
            #endregion Power Servo

            #region --- Worst harmonic report ---

            foreach (double dblFreq in TestSetting.FREQLIST)
            {
                if (StopTest) break;

                DataRow drmax = dtCWLB.NewRow();
                drmax[0] = intTestID++;
                drmax[1] = dblFreq;

                var _2fo = from x in dtCWLBTMP.AsEnumerable()
                           where x.Field<double>("Frequency(MHz)") == dblFreq && double.Parse(x.Field<string>("Vramp(V)")) <= dicVramp[dblFreq]
                           select x.Field<double>("2fo(dBm)");

                var _3fo = from x in dtCWLBTMP.AsEnumerable()
                           where x.Field<double>("Frequency(MHz)") == dblFreq && double.Parse(x.Field<string>("Vramp(V)")) <= dicVramp[dblFreq]
                           select x.Field<double>("3fo(dBm)");

                var _4fo = from x in dtCWLBTMP.AsEnumerable()
                           where x.Field<double>("Frequency(MHz)") == dblFreq && double.Parse(x.Field<string>("Vramp(V)")) <= dicVramp[dblFreq]
                           select x.Field<double>("4fo(dBm)");

                var _5fo = from x in dtCWLBTMP.AsEnumerable()
                           where x.Field<double>("Frequency(MHz)") == dblFreq && double.Parse(x.Field<string>("Vramp(V)")) <= dicVramp[dblFreq]
                           select x.Field<double>("5fo(dBm)");

                var _6fo = from x in dtCWLBTMP.AsEnumerable()
                           where x.Field<double>("Frequency(MHz)") == dblFreq && double.Parse(x.Field<string>("Vramp(V)")) <= dicVramp[dblFreq]
                           select x.Field<double>("6fo(dBm)");

                try
                {
                    drmax[6] = _2fo.ToList<double>().Max();
                    drmax[7] = _3fo.ToList<double>().Max();
                    drmax[8] = _4fo.ToList<double>().Max();
                    drmax[9] = _5fo.ToList<double>().Max();
                    drmax[10] = _6fo.ToList<double>().Max();
                }
                catch
                {
                    drmax[6] = 0;
                    drmax[7] = 0;
                    drmax[8] = 0;
                    drmax[9] = 0;
                    drmax[10] = 0;
                }

                this.Invoke((MethodInvoker)(delegate
                {
                    dtCWLB.Rows.Add(drmax);
                    dgvSweepResult.Refresh();
                    Application.DoEvents();
                }));

            }
            #endregion *** Worst harmonic report ***

            #region --- After Test ---
            // After Test
            AfterTest();
            _E4438C.SetOutput(Output.OFF);
            _PS_66332A.SetOutput(Output.OFF);
            _Arb_33522A_USB.SetOutput(Output.OFF, Arb_Channel.Channel_1);
            _Arb_33522A_USB.SetOutput(Output.OFF, Arb_Channel.Channel_2);

            #endregion --- After Test ---


        }

        void CWHB_Test_BJ_1()
        {
            #region --- Variable Define ---
            dicVramp.Clear();
            int intTestID = 1;
            double[] dblResult = new double[7];

            DataTable dtCWHBTMP = new DataTable();
            dtCWHBTMP = dtCWHB.Clone();

            if (!cbxKeepPrevious.Checked)
            {
                this.Invoke((MethodInvoker)(delegate
                {
                    dtCWHB.Clear();
                    dgvSweepResult.Refresh();
                }));
            }
            #endregion --- Variable Define ---

            #region --- Initialize ---
            BeforeTest();
            SetVCC(TestSetting.LEVEL_VCC);
            SetTXEnable(TestSetting.LEVEL_TXEN);
            SetGPCTRL(TestSetting.LEVEL_GPCTRL);
            _E4438C.Initialize();
            //_PM_N1913A.Initialize(rbnDisplayON.Checked);
            _MXA_N9020A.Initialize(rbnDisplayON.Checked);
            _E4438C.SetOutput(Output.ON);

            string Content = "Make sure everything is setup for TX HB testing \r\n" +
                            "  1) Connect rf source cable to TX HB \r\n" +
                            "  2) Set Control box to TX HB mode \r\n" +
                            "  3) Connect / Change HB highpass filter";

            this.Invoke((MethodInvoker)(delegate
            {
                MessageBox.Show(this, Content, "GMSK HB Testing", MessageBoxButtons.OK);
                Application.DoEvents();
            }));

            #endregion --- Initialize ---

            #region --- Vramp Sweep ---
            for (double dblVramp = TestSetting.LEVEL_START; dblVramp < TestSetting.LEVEL_STOP + 0.01; dblVramp += TestSetting.LEVEL_STEP)
            {
                if (StopTest) break;
                SetVramp(dblVramp);

                foreach (double dblFreq in TestSetting.FREQLIST)
                {
                    if (StopTest) break;
                    _E4438C.SetFrequency(dblFreq);
                    _E4438C.SetPower(TestSetting.LEVEL_PIN_VRAMP + TestSetting.LOSS_SRC[dblFreq]);

                    // Frequency
                    dblResult[0] = dblFreq;

                    // Vramp
                    dblResult[1] = dblVramp;

                    // Pout
                    //_PM_N1913A.Configure__Average_Pulse_Power_with_Duty_Cycle(dblFreq, 25, 10);
                    _PM_U2001A.Configure__Time_Gated_Burst_Power(dblFreq, U2001_Trigger.Internal, -35, 0.6, 0.36);

                    util.Wait(intDelay_PowerMeter * intDelay_N1913A_Count);
                    dblResult[2] = _PM_U2001A.GetPowerResult() + TestSetting.LOSS_MSR_POUT[dblFreq];

                    // ICC
                    dblResult[3] = _PS_66332A.High_Current();
                    dblResult[3] = dblResult[3] * 1000;

                    // PAE
                    dblResult[4] = (Math.Pow(10, ((dblResult[2] - 30) / 10))) / TestSetting.LEVEL_VCC / dblResult[3] * 1000;
                    dblResult[4] = dblResult[4] * 100;

                    // harmonic 
                    for (int i = 2; i <= 3; i++)
                    {
                        _MXA_N9020A.SetFrequency(i * dblFreq);

                        util.Wait(intDelay_MXA);

                        dblResult[3 + i] = _MXA_N9020A.Get_CW_PowerResult() + TestSetting.LOSS_MSR_FILTER_HB[i * dblFreq];
                    }
                    // Update_Grid
                    DataRow drNew = dtCWHB.NewRow();
                    DataRow drNewTmp = dtCWHBTMP.NewRow();
                    drNewTmp[0] = drNew[0] = intTestID++;
                    for (int i = 0; i < dblResult.Count(); i++)
                    {
                        drNewTmp[i + 1] = drNew[i + 1] = Math.Round(dblResult[i], 2);
                    }
                    dtCWHBTMP.Rows.Add(drNewTmp);
                    this.Invoke((MethodInvoker)(delegate
                    {
                        dtCWHB.Rows.Add(drNew);
                        dgvSweepResult.Refresh();
                        Application.DoEvents();
                    }));

                }   // Frequency loop
            }  // Vramp loop
            #endregion *** Vramp Sweep ***

            #region --- Power Servo ---
            foreach (double dblFreq in TestSetting.FREQLIST)
            {
                if (StopTest) break;
                int Count = 0;
                _E4438C.SetFrequency(dblFreq);
                _E4438C.SetPower(TestSetting.LEVEL_PIN_VRAMP + TestSetting.LOSS_SRC[dblFreq]);

                #region Search power
                double LoopResult_Low;
                double LoopResult_High;
                double LoopResult;
                double Slope_mV;

                this.SetVramp(1.2);
                //_PM_N1913A.Configure__Average_Pulse_Power_with_Duty_Cycle(dblFreq, 25, 10);
                _PM_U2001A.Configure__Time_Gated_Burst_Power(dblFreq, U2001_Trigger.Internal, -35, 0.6, 0.36);
                util.Wait(intDelay_PowerMeter * intDelay_N1913A_Count);
                LoopResult_Low = _PM_U2001A.GetPowerResult();

                this.SetVramp(1.6);
                util.Wait(intDelay_PowerMeter * intDelay_N1913A_Count);
                LoopResult_High = _PM_U2001A.GetPowerResult();

                Slope_mV = (LoopResult_High - LoopResult_Low) / 200;

                LoopResult = LoopResult_Low + TestSetting.LOSS_MSR_POUT[dblFreq];
                dblRatedVramp = 1.2 + (TestSetting.TARGET_POUT_CWHB - LoopResult) / Slope_mV / 1000;

                if (dblRatedVramp < 0.8 || dblRatedVramp > 1.8) dblRatedVramp = 1.8;

                while (Math.Abs(LoopResult - TestSetting.TARGET_POUT_CWHB) > 0.05 && dblRatedVramp < 1.8 && dblRatedVramp > 0.8)
                {
                    this.SetVramp(dblRatedVramp);
                    util.Wait(intDelay_PowerMeter * intDelay_N1913A_Count);
                    LoopResult = _PM_U2001A.GetPowerResult() + TestSetting.LOSS_MSR_POUT[dblFreq];
                    dblRatedVramp = dblRatedVramp + (TestSetting.TARGET_POUT_CWHB - LoopResult) / Slope_mV / 1000;
                    Count++;
                    if (Count > 20) break;
                }
                #endregion Search power

                // Frequency
                dblResult[0] = dblFreq;
                // Rated Vramp
                dblResult[1] = dblRatedVramp;
                if (!dicVramp.ContainsKey(dblFreq)) dicVramp.Add(dblFreq, Math.Round(dblRatedVramp, 2));
                //Rated Pout
                dblResult[2] = LoopResult;

                // Rated ICC
                dblResult[3] = _PS_66332A.High_Current();
                dblResult[3] = dblResult[3] * 1000;

                // Rated PAE
                dblResult[4] = (Math.Pow(10, ((dblResult[2] - 30) / 10))) / TestSetting.LEVEL_VCC / dblResult[3] * 1000;
                dblResult[4] = dblResult[4] * 100;

                // Rated harmonic 
                for (int i = 2; i <= 3; i++)
                {
                    _MXA_N9020A.SetFrequency(i * dblFreq);
                    util.Wait(intDelay_MXA);
                    dblResult[3 + i] = _MXA_N9020A.Get_CW_PowerResult() + TestSetting.LOSS_MSR_FILTER_HB[i * dblFreq];
                }
                // Update_Grid
                DataRow drNew = dtCWHB.NewRow();
                DataRow drNewTmp = dtCWHBTMP.NewRow();
                drNewTmp[0] = drNew[0] = intTestID++;
                for (int i = 0; i < dblResult.Count(); i++)
                {
                    drNewTmp[i + 1] = drNew[i + 1] = Math.Round(dblResult[i], 2);
                }
                dtCWHBTMP.Rows.Add(drNewTmp);
                this.Invoke((MethodInvoker)(delegate
                {
                    dtCWHB.Rows.Add(drNew);
                    dgvSweepResult.Refresh();
                    Application.DoEvents();
                }));

            } // Frequency loop for power servo
            #endregion Power Servo

            #region --- Worst harmonic report ---

            foreach (double dblFreq in TestSetting.FREQLIST)
            {
                if (StopTest) break;
                DataRow drmax = dtCWHB.NewRow();
                drmax[0] = intTestID++;
                drmax[1] = dblFreq;

                var _2fo = from x in dtCWHBTMP.AsEnumerable()
                           where x.Field<double>("Frequency(MHz)") == dblFreq && double.Parse(x.Field<string>("Vramp(V)")) <= dicVramp[dblFreq]
                           select x.Field<double>("2fo(dBm)");

                var _3fo = from x in dtCWHBTMP.AsEnumerable()
                           where x.Field<double>("Frequency(MHz)") == dblFreq && double.Parse(x.Field<string>("Vramp(V)")) <= dicVramp[dblFreq]
                           select x.Field<double>("3fo(dBm)");
                try
                {
                    drmax[6] = _2fo.ToList<double>().Max();
                    drmax[7] = _3fo.ToList<double>().Max();
                }
                catch
                {
                    drmax[6] = 0;
                    drmax[7] = 0;
                }

                this.Invoke((MethodInvoker)(delegate
                {
                    dtCWHB.Rows.Add(drmax);
                    dgvSweepResult.Refresh();
                    Application.DoEvents();
                }));
            }
            #endregion *** Worst harmonic report ***

            #region --- After Test ---
            // After Test
            AfterTest();
            _E4438C.SetOutput(Output.OFF);
            _PS_66332A.SetOutput(Output.OFF);
            _Arb_33522A_USB.SetOutput(Output.OFF, Arb_Channel.Channel_1);
            _Arb_33522A_USB.SetOutput(Output.OFF, Arb_Channel.Channel_2);

            #endregion --- After Test ---

        }

        void EDGELB_TEST_BJ_1()
        {
            #region --- Variable Define ---

            Dictionary<double, double> lastEDGELBPin = new Dictionary<double, double>();
            int intDelay;
            int intTestID = 1;
            double Pin_Start = -10;
            double[] dblResult = new double[10];

            DataTable dtEDGELBTMP = new DataTable();
            dtEDGELBTMP = dtEDGELB.Clone();

            if (!cbxKeepPrevious.Checked)
            {
                this.Invoke((MethodInvoker)(delegate
                {
                    dtEDGELB.Clear();
                    dgvSweepResult.Refresh();
                }));
            }

            #endregion --- Variable Define ---

            #region --- Initialize ---

            BeforeTest();

            SetVCC(TestSetting.LEVEL_VCC);
            SetTXEnable(TestSetting.LEVEL_TXEN);
            SetVramp(TestSetting.LEVEL_PIN_VRAMP);
            SetGPCTRL(TestSetting.LEVEL_GPCTRL);

            _E4438C.Mode_Initialize(Modulation.EDGE);
            _MXA_N9020A.Mod_Initialize(Modulation.EDGE);

            _E4438C.SetModOutput(Output.ON);
            _E4438C.SetOutput(Output.ON);

            string Content = "Make sure everything is setup for EDGELB testing \r\n" +
                              "  1) Connect rf source cable to EDGELB \r\n" +
                              "  2) Set Control box to EDGELB mode \r\n" +
                              "  3) Connect MXA to coupler out directly";

            this.Invoke((MethodInvoker)(delegate
            {
                MessageBox.Show(this, Content, "EDGELB Testing", MessageBoxButtons.OK);
                Application.DoEvents();
            }));
            util.Wait(3000);

            #endregion --- Initialize ---

            #region --- EDGELB Test ---

            for (double dblPout_Target = TestSetting.LEVEL_START; dblPout_Target <= TestSetting.LEVEL_STOP; dblPout_Target += TestSetting.LEVEL_STEP)
            {
                if (StopTest) break;

                double PoutLL = dblPout_Target - 0.05;
                double PoutUL = dblPout_Target + 0.05;

                foreach (double dblFreq in TestSetting.FREQLIST)
                {
                    if (StopTest) break;

                    int Count = 0;
                    double EDGELB_Pout = 0;
                    double EDGELB_Pin = 0;
                    double EDGELB_Icc = 0;
                    double dblPin = Pin_Start;
                    double[] dblACPResult = new double[10];

                    if (lastEDGELBPin.ContainsKey(dblFreq)) EDGELB_Pin = dblPin = Pin_Start = lastEDGELBPin[dblFreq] + TestSetting.LEVEL_STEP;
                    _E4438C.SetFrequency(dblFreq);
                    _E4438C.SetPower(Pin_Start + TestSetting.LOSS_SRC[dblFreq]);

                    _E4438C.SetModOutput(Output.OFF);
                    _MXA_N9020A.Config__EDGE_Burst_Power(dblFreq);
                    intDelay = Math.Max(intDelay_SigGen, intDelay_MXA) * 5;
                    util.Wait(intDelay);

                    EDGELB_Pout = _MXA_N9020A.Get_EDGE_Burst_Power_Result();
                    EDGELB_Pout += TestSetting.LOSS_MSR_THROUGH[dblFreq];

                    // Freqquency
                    dblResult[0] = dblFreq;

                    // Target Power
                    dblResult[1] = dblPout_Target;

                    //Pout
                    while (EDGELB_Pout <= PoutLL || EDGELB_Pout >= PoutUL)
                    {
                        dblPin = dblPin + dblPout_Target - EDGELB_Pout + 0.02;
                        if (dblPin > 5)
                            break;

                        _E4438C.SetPower(dblPin + TestSetting.LOSS_SRC[dblFreq]);
                        util.Wait(intDelay);
                        EDGELB_Pout = _MXA_N9020A.Get_EDGE_Burst_Power_Result();
                        EDGELB_Pout += TestSetting.LOSS_MSR_THROUGH[dblFreq];
                        EDGELB_Pin = dblPin;
                        Count++;
                        //if (Count > 20) break;
                    }
                    dblResult[2] = EDGELB_Pout;

                    // Pin
                    dblResult[3] = EDGELB_Pin;
                    if (lastEDGELBPin.ContainsKey(dblFreq))
                        lastEDGELBPin[dblFreq] = EDGELB_Pin;
                    else
                        lastEDGELBPin.Add(dblFreq, EDGELB_Pin);

                    // Gain
                    dblResult[4] = EDGELB_Pout - EDGELB_Pin;

                    // Icc
                    EDGELB_Icc = _PS_66332A.High_Current();
                    dblResult[5] = EDGELB_Icc * 1000;
                    // PAE
                    dblResult[6] = 100 * (Math.Pow(10, ((EDGELB_Pout - 30) / 10))) / TestSetting.LEVEL_VCC / EDGELB_Icc;

                    //ACP
                    _E4438C.SetModOutput(Output.ON);
                    util.Wait(1000);
                    _MXA_N9020A.Config__EDGE_ACP(dblFreq);
                    intDelay = Math.Max(intDelay_SigGen, intDelay_MXA);
                    util.Wait(intDelay * 3);

                    dblACPResult = _MXA_N9020A.Get_EDGE_ACP_Result();

                    dblResult[7] = dblACPResult[6];     // -400kHz
                    dblResult[8] = dblACPResult[7];     // +400kHz

                    // EVM
                    if (!cbxBypassEVM.Checked)
                    {
                        _MXA_N9020A.Config__EDGE_EVM(dblFreq);
                        intDelay = Math.Max(intDelay_SigGen, intDelay_MXA);
                        util.Wait(intDelay * 3);

                        dblResult[9] = _MXA_N9020A.Get_EDGE_EVM_Result();
                    }
                    else
                    {
                        dblResult[9] = 0;
                    }
                    // Update_Grid
                    DataRow drNew = dtEDGELB.NewRow();
                    DataRow drNewtmp = dtEDGELBTMP.NewRow();
                    drNewtmp[0] = drNew[0] = intTestID++;
                    for (int i = 0; i < dblResult.Count(); i++)
                    {
                        drNew[i + 1] = drNewtmp[i + 1] = Math.Round(dblResult[i], 2);
                    }
                    dtEDGELBTMP.Rows.Add(drNewtmp);

                    this.Invoke((MethodInvoker)(delegate
                    {
                        dtEDGELB.Rows.Add(drNew);
                        dgvSweepResult.Refresh();
                        Application.DoEvents();
                    }));
                }
            }

            #endregion --- EDGELB Test ---

            #region --- Worst acp report ---

            foreach (double dblFreq in TestSetting.FREQLIST)
            {
                DataRow drmax = dtEDGELB.NewRow();
                drmax[0] = intTestID++;
                drmax[1] = dblFreq;

                var acp_n400 = from x in dtEDGELBTMP.AsEnumerable()
                               where x.Field<double>("Frequency (MHz)") == dblFreq
                               select x.Field<double>("ACP -400kHz(dB)");

                var acp_p400 = from x in dtEDGELBTMP.AsEnumerable()
                               where x.Field<double>("Frequency (MHz)") == dblFreq
                               select x.Field<double>("ACP +400kHz(dB)");

                var evm = from x in dtEDGELBTMP.AsEnumerable()
                          where x.Field<double>("Frequency (MHz)") == dblFreq
                          select x.Field<double>("EVM(%)");

                try
                {
                    drmax[8] = acp_n400.ToList<double>().Max();
                    drmax[9] = acp_p400.ToList<double>().Max();
                    drmax[10] = evm.ToList<double>().Max();
                }
                catch
                {
                    drmax[8] = 0;
                    drmax[9] = 0;
                    drmax[10] = 0;
                }

                this.Invoke((MethodInvoker)(delegate
                {
                    dtEDGELB.Rows.Add(drmax);
                    dgvSweepResult.Refresh();
                    Application.DoEvents();
                }));

            }
            #endregion *** Worst acp report ***

            #region --- After Test ---

            AfterTest();
            _E4438C.SetOutput(Output.OFF);
            _PS_66332A.SetOutput(Output.OFF);
            //_PS_E3631A.SetOutput(Output.OFF);
            _Arb_33522A_USB.SetOutput(Output.OFF, Arb_Channel.Channel_1);
            _Arb_33522A_USB.SetOutput(Output.OFF, Arb_Channel.Channel_2);

            #endregion --- After Test ---
        }

        void EDGEHB_TEST_BJ_1()
        {
            #region --- Variable Define ---

            Dictionary<double, double> lastEDGEHBPin = new Dictionary<double, double>();
            int intDelay;
            int intTestID = 1;
            double Pin_Start = -10;
            double[] dblResult = new double[10];

            DataTable dtEDGEHBTMP = new DataTable();
            dtEDGEHBTMP = dtEDGEHB.Clone();

            if (!cbxKeepPrevious.Checked)
            {
                this.Invoke((MethodInvoker)(delegate
                {
                    dtEDGEHB.Clear();
                    dgvSweepResult.Refresh();
                }));
            }

            #endregion --- Variable Define ---

            #region --- Initialize ---

            BeforeTest();

            SetVCC(TestSetting.LEVEL_VCC);
            SetTXEnable(TestSetting.LEVEL_TXEN);
            SetVramp(TestSetting.LEVEL_PIN_VRAMP);
            SetGPCTRL(TestSetting.LEVEL_GPCTRL);

            _E4438C.Mode_Initialize(Modulation.EDGE);
            _MXA_N9020A.Mod_Initialize(Modulation.EDGE);

            _E4438C.SetModOutput(Output.ON);
            _E4438C.SetOutput(Output.ON);
            //util.Wait(5000);

            string Content = "Make sure everything is setup for EDGEHB testing \r\n" +
                              "  1) Connect rf source cable to EDGEHB \r\n" +
                              "  2) Set Control box to EDGEHB mode \r\n" +
                              "  3) Connect MXA to coupler out directly";

            this.Invoke((MethodInvoker)(delegate
            {
                MessageBox.Show(this, Content, "EDGEHB Testing", MessageBoxButtons.OK);
                Application.DoEvents();
            }));
            util.Wait(3000);

            #endregion --- Initialize ---

            #region --- EDGEHB Test ---

            for (double dblPout_Target = TestSetting.LEVEL_START; dblPout_Target <= TestSetting.LEVEL_STOP; dblPout_Target += TestSetting.LEVEL_STEP)
            {
                if (StopTest) break;

                double PoutLL = dblPout_Target - 0.05;
                double PoutUL = dblPout_Target + 0.05;

                foreach (double dblFreq in TestSetting.FREQLIST)
                {
                    if (StopTest) break;

                    int Count = 0;
                    double EDGEHB_Pout = 0;
                    double EDGEHB_Pin = 0;
                    double EDGEHB_Icc = 0;
                    double dblPin = Pin_Start;
                    double[] dblACPResult = new double[10];

                    if (lastEDGEHBPin.ContainsKey(dblFreq)) EDGEHB_Pin = dblPin = Pin_Start = lastEDGEHBPin[dblFreq] + TestSetting.LEVEL_STEP;
                    _E4438C.SetFrequency(dblFreq);
                    _E4438C.SetPower(Pin_Start + TestSetting.LOSS_SRC[dblFreq]);

                    _E4438C.SetModOutput(Output.OFF);
                    _MXA_N9020A.Config__EDGE_Burst_Power(dblFreq);
                    intDelay = Math.Max(intDelay_SigGen, intDelay_MXA) * 5;
                    util.Wait(intDelay);

                    EDGEHB_Pout = _MXA_N9020A.Get_EDGE_Burst_Power_CW_Result();
                    EDGEHB_Pout += TestSetting.LOSS_MSR_THROUGH[dblFreq];

                    // Freqquency
                    dblResult[0] = dblFreq;

                    // Target Power
                    dblResult[1] = dblPout_Target;

                    //Pout
                    while (EDGEHB_Pout <= PoutLL || EDGEHB_Pout >= PoutUL)
                    {
                        dblPin = dblPin + dblPout_Target - EDGEHB_Pout + 0.02;
                        if (dblPin > 5)
                            break;

                        _E4438C.SetPower(dblPin + TestSetting.LOSS_SRC[dblFreq]);
                        util.Wait(intDelay);
                        EDGEHB_Pout = _MXA_N9020A.Get_EDGE_Burst_Power_CW_Result();
                        EDGEHB_Pout += TestSetting.LOSS_MSR_THROUGH[dblFreq];
                        EDGEHB_Pin = dblPin;
                        Count++;
                        if (Count > 20) break;
                    }
                    dblResult[2] = EDGEHB_Pout;

                    // Pin
                    dblResult[3] = EDGEHB_Pin;
                    if (lastEDGEHBPin.ContainsKey(dblFreq))
                        lastEDGEHBPin[dblFreq] = EDGEHB_Pin;
                    else
                        lastEDGEHBPin.Add(dblFreq, EDGEHB_Pin);

                    // Gain
                    dblResult[4] = EDGEHB_Pout - EDGEHB_Pin;

                    // Icc
                    EDGEHB_Icc = _PS_66332A.High_Current();
                    dblResult[5] = EDGEHB_Icc * 1000;
                    // PAE
                    dblResult[6] = 100 * (Math.Pow(10, ((EDGEHB_Pout - 30) / 10))) / TestSetting.LEVEL_VCC / EDGEHB_Icc;

                    //ACP
                    _E4438C.SetModOutput(Output.ON);
                    util.Wait(1000);
                    _MXA_N9020A.Config__EDGE_ACP(dblFreq);
                    intDelay = Math.Max(intDelay_SigGen, intDelay_MXA);
                    util.Wait(intDelay * 3);

                    dblACPResult = _MXA_N9020A.Get_EDGE_ACP_Result();

                    dblResult[7] = dblACPResult[6];     // -400kHz
                    dblResult[8] = dblACPResult[7];     // +400kHz

                    // EVM
                    if (!cbxBypassEVM.Checked)
                    {
                        _MXA_N9020A.Config__EDGE_EVM(dblFreq);
                        intDelay = Math.Max(intDelay_SigGen, intDelay_MXA);
                        util.Wait(intDelay * 3);

                        dblResult[9] = _MXA_N9020A.Get_EDGE_EVM_Result();
                    }
                    else
                    {
                        dblResult[9] = 0;
                    }
                    // Update_Grid
                    DataRow drNew = dtEDGEHB.NewRow();
                    DataRow drNewtmp = dtEDGEHBTMP.NewRow();
                    drNewtmp[0] = drNew[0] = intTestID++;
                    for (int i = 0; i < dblResult.Count(); i++)
                    {
                        drNew[i + 1] = drNewtmp[i + 1] = Math.Round(dblResult[i], 2);
                    }
                    dtEDGEHBTMP.Rows.Add(drNewtmp);

                    this.Invoke((MethodInvoker)(delegate
                    {
                        dtEDGEHB.Rows.Add(drNew);
                        dgvSweepResult.Refresh();
                        Application.DoEvents();
                    }));
                }
            }

            #endregion --- EDGEHB Test ---

            #region --- Worst acp report ---

            foreach (double dblFreq in TestSetting.FREQLIST)
            {
                DataRow drmax = dtEDGEHB.NewRow();
                drmax[0] = intTestID++;
                drmax[1] = dblFreq;

                var acp_n400 = from x in dtEDGEHBTMP.AsEnumerable()
                               where x.Field<double>("Frequency (MHz)") == dblFreq
                               select x.Field<double>("ACP -400kHz(dB)");

                var acp_p400 = from x in dtEDGEHBTMP.AsEnumerable()
                               where x.Field<double>("Frequency (MHz)") == dblFreq
                               select x.Field<double>("ACP +400kHz(dB)");

                var evm = from x in dtEDGEHBTMP.AsEnumerable()
                          where x.Field<double>("Frequency (MHz)") == dblFreq
                          select x.Field<double>("EVM(%)");

                try
                {
                    drmax[8] = acp_n400.ToList<double>().Max();
                    drmax[9] = acp_p400.ToList<double>().Max();
                    drmax[10] = evm.ToList<double>().Max();
                }
                catch
                {
                    drmax[8] = 0;
                    drmax[9] = 0;
                    drmax[10] = 0;
                }

                this.Invoke((MethodInvoker)(delegate
                {
                    dtEDGEHB.Rows.Add(drmax);
                    dgvSweepResult.Refresh();
                    Application.DoEvents();
                }));

            }
            #endregion *** Worst acp report ***

            #region --- After Test ---

            AfterTest();
            _E4438C.SetOutput(Output.OFF);
            _PS_66332A.SetOutput(Output.OFF);
            //_PS_E3631A.SetOutput(Output.OFF);
            _Arb_33522A_USB.SetOutput(Output.OFF, Arb_Channel.Channel_1);
            _Arb_33522A_USB.SetOutput(Output.OFF, Arb_Channel.Channel_2);

            #endregion --- After Test ---
        }

        void TDSCDMA_TEST_BJ_1()
        {
            #region --- Variable Define ---

            Dictionary<double, double> lastTDSCDMAPin = new Dictionary<double, double>();
            int intDelay;
            int intTestID = 1;
            double Pin_Start = -10;
            double[] dblResult = new double[12];

            DataTable dtTDSCDMATMP = new DataTable();
            dtTDSCDMATMP = dtTDSCDMA.Clone();

            if (!cbxKeepPrevious.Checked)
            {
                this.Invoke((MethodInvoker)(delegate
                {
                    dtTDSCDMA.Clear();
                    dgvSweepResult.Refresh();
                }));
            }

            #endregion --- Variable Define ---

            #region --- Initialize ---

            BeforeTest();
            //_PS_E3631A = new PS_E3631A(Instruments_address._06);
            //_PS_E3631A.Initialize();

            SetVCC(TestSetting.LEVEL_VCC);
            //SetVbat(TestSetting.LEVEL_VBAT, 1);
            SetTXEnable(TestSetting.LEVEL_TXEN);
            SetVramp(TestSetting.LEVEL_PIN_VRAMP);
            SetGPCTRL(TestSetting.LEVEL_GPCTRL);

            if (td_hsdpa)
            {
                _E4438C.Mode_Initialize(Modulation.TDSCDMA, true);
                _MXA_N9020A.Mod_Initialize(Modulation.TDSCDMA, true);
            }
            else
            {
                _E4438C.Mode_Initialize(Modulation.TDSCDMA);
                _MXA_N9020A.Mod_Initialize(Modulation.TDSCDMA);
            }

            _E4438C.SetModOutput(Output.ON);
            _E4438C.SetOutput(Output.ON);

            string Content = "Make sure everything is setup for TDSCDMA testing \r\n" +
                              "  1) Connect rf source cable to TDSCDMA \r\n" +
                              "  2) Set Control box to TDSCDMA mode \r\n" +
                              "  3) Connect MXA to coupler out directly";

            this.Invoke((MethodInvoker)(delegate
            {
                MessageBox.Show(this, Content, "TDSCDMA Testing", MessageBoxButtons.OK);
                Application.DoEvents();
            }));

            #endregion --- Initialize ---

            #region --- TDSCDMA Test ---

            for (double dblPout_Target = TestSetting.LEVEL_START; dblPout_Target <= TestSetting.LEVEL_STOP; dblPout_Target += TestSetting.LEVEL_STEP)
            {
                if (StopTest) break;

                double PoutLL = dblPout_Target - 0.05;
                double PoutUL = dblPout_Target + 0.05;

                foreach (double dblFreq in TestSetting.FREQLIST)
                {
                    if (StopTest) break;

                    double TDSCDMA_Pout = 0;
                    double TDSCDMA_Pin = 0;
                    double TDSCDMA_Icc = 0;
                    double dblPin = Pin_Start;
                    double[] dblACPResult = new double[5];

                    if (lastTDSCDMAPin.ContainsKey(dblFreq)) TDSCDMA_Pin = dblPin = Pin_Start = lastTDSCDMAPin[dblFreq] + TestSetting.LEVEL_STEP;
                    _E4438C.SetFrequency(dblFreq);
                    _E4438C.SetPower(Pin_Start + TestSetting.LOSS_SRC[dblFreq]);

                    _MXA_N9020A.Config__TDSCDMA_CHP(dblFreq);
                    intDelay = Math.Max(intDelay_SigGen, intDelay_MXA) * 10;
                    util.Wait(intDelay);

                    TDSCDMA_Pout = _MXA_N9020A.Get_TDSCDMA_CHP_Result();
                    TDSCDMA_Pout += TestSetting.LOSS_MSR_THROUGH[dblFreq];

                    // Freqquency
                    dblResult[0] = dblFreq;

                    // Target Power
                    dblResult[1] = dblPout_Target;

                    //Pout
                    while (TDSCDMA_Pout <= PoutLL || TDSCDMA_Pout >= PoutUL)
                    {
                        dblPin = dblPin + dblPout_Target - TDSCDMA_Pout;
                        if (dblPin > 5)
                            break;

                        _E4438C.SetPower(dblPin + TestSetting.LOSS_SRC[dblFreq]);
                        util.Wait(intDelay);
                        TDSCDMA_Pout = _MXA_N9020A.Get_TDSCDMA_CHP_Result();
                        TDSCDMA_Pout += TestSetting.LOSS_MSR_THROUGH[dblFreq];
                        TDSCDMA_Pin = dblPin;
                    }
                    dblResult[2] = TDSCDMA_Pout;

                    // Pin
                    dblResult[3] = TDSCDMA_Pin;
                    if (lastTDSCDMAPin.ContainsKey(dblFreq))
                        lastTDSCDMAPin[dblFreq] = TDSCDMA_Pin;
                    else
                        lastTDSCDMAPin.Add(dblFreq, TDSCDMA_Pin);

                    // Gain
                    dblResult[4] = TDSCDMA_Pout - TDSCDMA_Pin;

                    // Icc
                    TDSCDMA_Icc = _PS_66332A.Max_Current();
                    dblResult[5] = TDSCDMA_Icc * 1000;
                    // PAE
                    dblResult[6] = 100 * (Math.Pow(10, ((TDSCDMA_Pout - 30) / 10))) / TestSetting.LEVEL_VCC / TDSCDMA_Icc;

                    //ACP
                    _MXA_N9020A.Config__TDSCDMA_ACP(dblFreq, 4.616);
                    intDelay = Math.Max(intDelay_SigGen, intDelay_MXA);
                    util.Wait(intDelay * 3);

                    dblACPResult = _MXA_N9020A.Get_TDSCDMA_ACP_Result();

                    dblResult[7] = dblACPResult[1];     // -5MHz
                    dblResult[8] = dblACPResult[2];     // +5MHz
                    dblResult[9] = dblACPResult[3];     // -10MHz
                    dblResult[10] = dblACPResult[4];     // +10MHz

                    // EVM
                    if (!cbxBypassEVM.Checked)
                    {
                        _MXA_N9020A.Config__TDSCDMA_EVM(dblFreq);
                        intDelay = Math.Max(intDelay_SigGen, intDelay_MXA);
                        util.Wait(intDelay * 3);

                        dblResult[11] = _MXA_N9020A.Get_TDSCDMA_EVM_Result();
                    }
                    else
                    {
                        dblResult[11] = 0;
                    }
                    // Update_Grid
                    DataRow drNew = dtTDSCDMA.NewRow();
                    DataRow drNewtmp = dtTDSCDMATMP.NewRow();
                    drNewtmp[0] = drNew[0] = intTestID++;
                    for (int i = 0; i < dblResult.Count(); i++)
                    {
                        drNew[i + 1] = drNewtmp[i + 1] = Math.Round(dblResult[i], 2);
                    }
                    dtTDSCDMATMP.Rows.Add(drNewtmp);

                    this.Invoke((MethodInvoker)(delegate
                    {
                        dtTDSCDMA.Rows.Add(drNew);
                        dgvSweepResult.Refresh();
                        Application.DoEvents();
                    }));
                }
            }

            #endregion --- TDSCDMA Test ---

            #region --- Worst acp report ---

            foreach (double dblFreq in TestSetting.FREQLIST)
            {
                DataRow drmax = dtTDSCDMA.NewRow();
                drmax[0] = intTestID++;
                drmax[1] = dblFreq;

                var acp_n5 = from x in dtTDSCDMATMP.AsEnumerable()
                             where x.Field<double>("Frequency (MHz)") == dblFreq
                             select x.Field<double>("ACP -1.6MHz(dB)");

                var acp_p5 = from x in dtTDSCDMATMP.AsEnumerable()
                             where x.Field<double>("Frequency (MHz)") == dblFreq
                             select x.Field<double>("ACP +1.6MHz(dB)");

                var acp_n10 = from x in dtTDSCDMATMP.AsEnumerable()
                              where x.Field<double>("Frequency (MHz)") == dblFreq
                              select x.Field<double>("ACP -3.2MHz(dB)");

                var acp_p10 = from x in dtTDSCDMATMP.AsEnumerable()
                              where x.Field<double>("Frequency (MHz)") == dblFreq
                              select x.Field<double>("ACP +3.2MHz(dB)");

                var evm = from x in dtTDSCDMATMP.AsEnumerable()
                          where x.Field<double>("Frequency (MHz)") == dblFreq
                          select x.Field<double>("EVM(%)");

                try
                {
                    drmax[8] = acp_n5.ToList<double>().Max();
                    drmax[9] = acp_p5.ToList<double>().Max();
                    drmax[10] = acp_n10.ToList<double>().Max();
                    drmax[11] = acp_p10.ToList<double>().Max();
                    drmax[12] = evm.ToList<double>().Max();
                }
                catch
                {
                    drmax[8] = 0;
                    drmax[9] = 0;
                    drmax[10] = 0;
                    drmax[11] = 0;
                    drmax[12] = 0;
                }

                this.Invoke((MethodInvoker)(delegate
                {
                    dtTDSCDMA.Rows.Add(drmax);
                    dgvSweepResult.Refresh();
                    Application.DoEvents();
                }));

            }
            #endregion *** Worst acp report ***

            #region --- After Test ---

            AfterTest();
            _E4438C.SetOutput(Output.OFF);
            _PS_66332A.SetOutput(Output.OFF);
            //_PS_E3631A.SetOutput(Output.OFF);
            _Arb_33522A_USB.SetOutput(Output.OFF, Arb_Channel.Channel_1);
            _Arb_33522A_USB.SetOutput(Output.OFF, Arb_Channel.Channel_2);

            #endregion --- After Test ---
        }

        void WCDMA_TEST_BJ_1()
        {
            #region --- Variable Define ---

            Dictionary<double, double> lastPin = new Dictionary<double, double>();
            int intDelay;
            int intTestID = 1;
            double Pin_Start = -10;
            double[] dblResult = new double[12];

            DataTable dtWCDMATMP = new DataTable();
            dtWCDMATMP = dtWCDMA.Clone();

            if (!cbxKeepPrevious.Checked)
            {
                this.Invoke((MethodInvoker)(delegate
                {
                    dtWCDMA.Clear();
                    dgvSweepResult.Refresh();
                }));
            }

            #endregion --- Variable Define ---

            #region --- Initialize ---

            BeforeTest();
            //_PS_E3631A = new PS_E3631A(Instruments_address._06);
            //_PS_E3631A.Initialize();

            SetVCC(TestSetting.LEVEL_VCC);
            //SetVbat(TestSetting.LEVEL_VBAT, 1);
            //SetTXEnable(TestSetting.LEVEL_TXEN);
            //SetVramp(TestSetting.LEVEL_PIN_VRAMP);
            SetGPCTRL(TestSetting.LEVEL_GPCTRL);

            _E4438C.Mode_Initialize(Modulation.WCDMA);
            _MXA_N9020A.Mod_Initialize(Modulation.WCDMA);

            _E4438C.SetModOutput(Output.ON);
            _E4438C.SetOutput(Output.ON);

            string Content = "Make sure everything is setup for WCDMA testing \r\n" +
                              "  1) Connect rf source cable to WCDMA \r\n" +
                              "  2) Set Control box to WCDMA mode \r\n" +
                              "  3) Connect MXA to coupler out directly";

            this.Invoke((MethodInvoker)(delegate
            {
                MessageBox.Show(this, Content, "WCDMA Testing", MessageBoxButtons.OK);
                Application.DoEvents();
            }));

            #endregion --- Initialize ---

            #region --- WCDMA Test ---

            for (double dblPout_Target = TestSetting.LEVEL_START; dblPout_Target <= TestSetting.LEVEL_STOP; dblPout_Target += TestSetting.LEVEL_STEP)
            {
                if (StopTest) break;

                double PoutLL = dblPout_Target - 0.05;
                double PoutUL = dblPout_Target + 0.05;

                foreach (double dblFreq in TestSetting.FREQLIST)
                {
                    if (StopTest) break;

                    double WCDMA_Pout = 0;
                    double WCDMA_Pin = 0;
                    double WCDMA_Icc = 0;
                    double dblPin = Pin_Start;
                    double[] dblACPResult = new double[5];

                    if (lastPin.ContainsKey(dblFreq)) WCDMA_Pin = dblPin = Pin_Start = lastPin[dblFreq] + TestSetting.LEVEL_STEP;
                    _E4438C.SetFrequency(dblFreq);
                    _E4438C.SetPower(Pin_Start + TestSetting.LOSS_SRC[dblFreq]);

                    //_MXA_N9020A.Config__WCDMA_CHP(dblFreq);
                    //intDelay = Math.Max(intDelay_SigGen, intDelay_MXA) * 10;
                    _PM_N1913A.Configure__CW_Power(dblFreq, 10);
                    intDelay = Math.Max(intDelay_SigGen, intDelay_PowerMeter) * 2;
                    util.Wait(intDelay);

                    //WCDMA_Pout = _MXA_N9020A.Get_WCDMA_CHP_Result();
                    //WCDMA_Pout += TestSetting.LOSS_MSR_THROUGH[dblFreq];
                    WCDMA_Pout = _PM_N1913A.GetPowerResult();
                    WCDMA_Pout += TestSetting.LOSS_MSR_POUT[dblFreq];

                    // Freqquency
                    dblResult[0] = dblFreq;

                    // Target Power
                    dblResult[1] = dblPout_Target;

                    // Pout
                    while (WCDMA_Pout <= PoutLL || WCDMA_Pout >= PoutUL)
                    {
                        dblPin = dblPin + dblPout_Target - WCDMA_Pout;
                        if (dblPin > 5)
                            break;

                        _E4438C.SetPower(dblPin + TestSetting.LOSS_SRC[dblFreq]);
                        util.Wait(intDelay);
                        //WCDMA_Pout = _MXA_N9020A.Get_WCDMA_CHP_Result();
                        //WCDMA_Pout += TestSetting.LOSS_MSR_THROUGH[dblFreq];
                        WCDMA_Pout = _PM_N1913A.GetPowerResult();
                        WCDMA_Pout += TestSetting.LOSS_MSR_POUT[dblFreq];
                        WCDMA_Pin = dblPin;
                    }
                    dblResult[2] = WCDMA_Pout;

                    // Pin
                    dblResult[3] = WCDMA_Pin;

                    if (lastPin.ContainsKey(dblFreq))
                        lastPin[dblFreq] = WCDMA_Pin;
                    else
                        lastPin.Add(dblFreq, WCDMA_Pin);
                    // Gain
                    dblResult[4] = WCDMA_Pout - WCDMA_Pin;

                    // Icc
                    WCDMA_Icc = _PS_66332A.RMS_Current();
                    dblResult[5] = WCDMA_Icc * 1000;
                    // PAE
                    dblResult[6] = 100 * (Math.Pow(10, ((WCDMA_Pout - 30) / 10))) / TestSetting.LEVEL_VCC / WCDMA_Icc;

                    // ACP
                    _MXA_N9020A.Config__WCDMA_ACP(dblFreq);
                    intDelay = Math.Max(intDelay_SigGen, intDelay_MXA);
                    util.Wait(intDelay * 3);

                    dblACPResult = _MXA_N9020A.Get_WCDMA_ACP_Result();

                    dblResult[7] = dblACPResult[1];     // -5MHz
                    dblResult[8] = dblACPResult[2];     // +5MHz
                    dblResult[9] = dblACPResult[3];     // -10MHz
                    dblResult[10] = dblACPResult[4];     // +10MHz

                    // EVM
                    if (!cbxBypassEVM.Checked)
                    {
                        _MXA_N9020A.Config__WCDMA_EVM(dblFreq);
                        intDelay = Math.Max(intDelay_SigGen, intDelay_MXA);
                        util.Wait(intDelay * 3);

                        dblResult[11] = _MXA_N9020A.Get_WCDMA_EVM_Result();
                    }
                    else
                    {
                        dblResult[11] = 0;
                    }
                    // Update_Grid
                    DataRow drNew = dtWCDMA.NewRow();
                    DataRow drNewtmp = dtWCDMATMP.NewRow();
                    drNew[0] = intTestID++;
                    for (int i = 0; i < dblResult.Count(); i++)
                    {
                        drNew[i + 1] = drNewtmp[i + 1] = Math.Round(dblResult[i], 2);
                    }
                    dtWCDMATMP.Rows.Add(drNewtmp);

                    this.Invoke((MethodInvoker)(delegate
                    {
                        dtWCDMA.Rows.Add(drNew);
                        dgvSweepResult.Refresh();
                        Application.DoEvents();
                    }));
                }
            }

            #endregion --- WCDMA Test ---

            #region --- Worst acp report ---

            foreach (double dblFreq in TestSetting.FREQLIST)
            {
                DataRow drmax = dtWCDMA.NewRow();
                drmax[0] = intTestID++;
                drmax[1] = dblFreq;

                var acp_n5 = from x in dtWCDMATMP.AsEnumerable()
                             where x.Field<double>("Frequency (MHz)") == dblFreq
                             select x.Field<double>("ACP -5MHz(dB)");

                var acp_p5 = from x in dtWCDMATMP.AsEnumerable()
                             where x.Field<double>("Frequency (MHz)") == dblFreq
                             select x.Field<double>("ACP +5MHz(dB)");

                var acp_n10 = from x in dtWCDMATMP.AsEnumerable()
                              where x.Field<double>("Frequency (MHz)") == dblFreq
                              select x.Field<double>("ACP -10MHz(dB)");

                var acp_p10 = from x in dtWCDMATMP.AsEnumerable()
                              where x.Field<double>("Frequency (MHz)") == dblFreq
                              select x.Field<double>("ACP +10MHz(dB)");

                var evm = from x in dtWCDMATMP.AsEnumerable()
                          where x.Field<double>("Frequency (MHz)") == dblFreq
                          select x.Field<double>("EVM(%)");

                try
                {
                    drmax[8] = acp_n5.ToList<double>().Max();
                    drmax[9] = acp_p5.ToList<double>().Max();
                    drmax[10] = acp_n10.ToList<double>().Max();
                    drmax[11] = acp_p10.ToList<double>().Max();
                    drmax[12] = evm.ToList<double>().Max();
                }
                catch
                {
                    drmax[8] = 0;
                    drmax[9] = 0;
                    drmax[10] = 0;
                    drmax[11] = 0;
                    drmax[12] = 0;
                }

                this.Invoke((MethodInvoker)(delegate
                {
                    dtWCDMA.Rows.Add(drmax);
                    dgvSweepResult.Refresh();
                    Application.DoEvents();
                }));

            }
            #endregion *** Worst acp report ***

            #region --- After Test ---

            AfterTest();
            _E4438C.SetOutput(Output.OFF);
            _PS_66332A.SetOutput(Output.OFF);
            //_PS_E3631A.SetOutput(Output.OFF);
            _Arb_33522A_USB.SetOutput(Output.OFF, Arb_Channel.Channel_1);
            _Arb_33522A_USB.SetOutput(Output.OFF, Arb_Channel.Channel_2);

            #endregion --- After Test ---

        }

        #endregion --- BJ_1 Test ---
        #region --- SH_1 Test ---

        void CWLB_Test_SH_1()
        {
            #region --- Variable Define ---

            dicVramp.Clear();
            int intTestID = 1;
            double[] dblResult = new double[18];

            DataTable dtCWLBTMP = new DataTable();
            dtCWLBTMP = dtCWLB.Clone();

            if (!cbxKeepPrevious.Checked)
            {
                this.Invoke((MethodInvoker)(delegate
                {
                    dtCWLB.Clear();
                    dgvSweepResult.Refresh();
                }));
            }

            #endregion --- Variable Define ---

            #region --- Initialize ---
            BeforeTest();

            SetVCC(TestSetting.LEVEL_VCC);
            SetTXEnable(TestSetting.LEVEL_TXEN);
            SetGPCTRL(TestSetting.LEVEL_GPCTRL);

            _E4438C.Initialize();
            _PM_N1913A.Initialize(rbnDisplayON.Checked);
            _MXA_N9020A.Initialize(rbnDisplayON.Checked);
            _E4438C.SetOutput(Output.ON);

            util.Wait(1000);
            Application.DoEvents();

            SetVCC(TestSetting.LEVEL_VCC);
            SetTXEnable(TestSetting.LEVEL_TXEN);
            SetGPCTRL(TestSetting.LEVEL_GPCTRL);

            _E4438C.Initialize();
            _PM_N1913A.Initialize(rbnDisplayON.Checked);
            _MXA_N9020A.Initialize(rbnDisplayON.Checked);
            _E4438C.SetOutput(Output.ON);

            DateTime t_Start = DateTime.Now;

            string Content = "Make sure everything is setup for TX LB testing \r\n" +
                            "  1) Connect rf source cable to TX LB \r\n" +
                            "  2) Set Control box to TX LB mode \r\n" +
                            "  3) Connect / Change LB highpass filter";

            this.Invoke((MethodInvoker)(delegate
            {
                MessageBox.Show(this, Content, "GMSK LB Testing", MessageBoxButtons.OK);
                Application.DoEvents();
                wait_2_start(t_Start);
            }));

            #endregion --- Initialize ---

            #region --- Vramp Sweep ---
            for (double dblVramp = TestSetting.LEVEL_START; dblVramp < TestSetting.LEVEL_STOP + 0.01; dblVramp += TestSetting.LEVEL_STEP)
            {
                if (StopTest) break;

                SetVramp(dblVramp);

                foreach (double dblFreq in TestSetting.FREQLIST)
                {
                    if (StopTest) break;
                    _E4438C.SetFrequency(dblFreq);
                    _E4438C.SetPower(TestSetting.LEVEL_PIN_VRAMP + TestSetting.LOSS_SRC[dblFreq]);

                    // Frequency
                    dblResult[0] = dblFreq;

                    // Vramp
                    dblResult[1] = dblVramp;

                    // Pout
                    _PM_N1913A.Configure__Average_Pulse_Power_with_Duty_Cycle(dblFreq, 25, 256);
                    util.Wait(intDelay_PowerMeter * intDelay_N1913A_Count);
                    dblResult[2] = _PM_N1913A.GetPowerResult() + TestSetting.LOSS_MSR_POUT[dblFreq];

                    // ICC
                    dblResult[3] = _PS_66332A.High_Current();
                    dblResult[3] = dblResult[3] * 1000;

                    // PAE
                    dblResult[4] = (Math.Pow(10, ((dblResult[2] - 30) / 10))) / TestSetting.LEVEL_VCC / dblResult[3] * 1000;
                    dblResult[4] = dblResult[4] * 100;

                    // harmonic 
                    for (int i = 2; i <= 6; i++)
                    {
                        _MXA_N9020A.SetFrequency(i * dblFreq);

                        util.Wait(intDelay_MXA);

                        dblResult[3 + i] = _MXA_N9020A.Get_CW_PowerResult() + TestSetting.LOSS_MSR_FILTER_LB[i * dblFreq];
                    }
                    // Extra harmonic 
                    if (ext_har)
                    {
                        for (int i = 7; i <= 14; i++)
                        {
                            _MXA_N9020A.SetFrequency(i * dblFreq);
                            util.Wait(intDelay_MXA);
                            dblResult[3 + i] = _MXA_N9020A.Get_CW_PowerResult() + TestSetting.LOSS_MSR_FILTER_LB[6000];
                        }
                    }
                    // Update_Grid
                    DataRow drNew = dtCWLB.NewRow();
                    DataRow drNewTmp = dtCWLBTMP.NewRow();
                    drNewTmp[0] = drNew[0] = intTestID++;
                    for (int i = 0; i < dblResult.Count(); i++)
                    {
                        drNewTmp[i + 1] = drNew[i + 1] = Math.Round(dblResult[i], 2);
                    }
                    dtCWLBTMP.Rows.Add(drNewTmp);
                    this.Invoke((MethodInvoker)(delegate
                    {
                        dtCWLB.Rows.Add(drNew);
                        dgvSweepResult.Refresh();
                        Application.DoEvents();
                    }));
                } // Frequency loop
            }   // Vramp loop
            #endregion Vramp Sweep

            #region --- Power Servo ---
            foreach (double dblFreq in TestSetting.FREQLIST)
            {
                if (StopTest) break;

                int Count = 0;
                _E4438C.SetFrequency(dblFreq);
                _E4438C.SetPower(TestSetting.LEVEL_PIN_VRAMP + TestSetting.LOSS_SRC[dblFreq]);

                #region Search power
                double LoopResult_Low;
                double LoopResult_High;
                double LoopResult;
                double Slope_mV;

                this.SetVramp(1.2);
                _PM_N1913A.Configure__Average_Pulse_Power_with_Duty_Cycle(dblFreq, 25, 256);
                util.Wait(intDelay_PowerMeter * intDelay_N1913A_Count);
                LoopResult_Low = _PM_N1913A.GetPowerResult();

                this.SetVramp(1.6);
                _PM_N1913A.Configure__Average_Pulse_Power_with_Duty_Cycle(dblFreq, 25, 256);
                util.Wait(intDelay_PowerMeter * intDelay_N1913A_Count);
                LoopResult_High = _PM_N1913A.GetPowerResult();

                Slope_mV = (LoopResult_High - LoopResult_Low) / 200;

                LoopResult = LoopResult_Low + TestSetting.LOSS_MSR_POUT[dblFreq];
                dblRatedVramp = 1.2 + (TestSetting.TARGET_POUT_CWLB - LoopResult) / Slope_mV / 1000;

                if (dblRatedVramp < 0.8 || dblRatedVramp > 1.8) dblRatedVramp = 1.8;

                while (Math.Abs(LoopResult - TestSetting.TARGET_POUT_CWLB) > 0.05 && dblRatedVramp < 1.8 && dblRatedVramp > 0.8)
                {
                    this.SetVramp(dblRatedVramp);
                    util.Wait(intDelay_PowerMeter * intDelay_N1913A_Count);
                    LoopResult = _PM_N1913A.GetPowerResult() + TestSetting.LOSS_MSR_POUT[dblFreq];
                    dblRatedVramp = dblRatedVramp + (TestSetting.TARGET_POUT_CWLB - LoopResult) / Slope_mV / 1000;
                    Count++;
                    if (Count > 20) break;
                }
                #endregion Search power

                // Frequency
                dblResult[0] = dblFreq;
                // Rated Vramp
                dblResult[1] = dblRatedVramp;
                if (!dicVramp.ContainsKey(dblFreq)) dicVramp.Add(dblFreq, Math.Round(dblRatedVramp, 2));
                //Rated Pout
                dblResult[2] = LoopResult;

                // Rated ICC
                dblResult[3] = _PS_66332A.High_Current();
                dblResult[3] = dblResult[3] * 1000;

                // Rated PAE
                dblResult[4] = (Math.Pow(10, ((dblResult[2] - 30) / 10))) / TestSetting.LEVEL_VCC / dblResult[3] * 1000;
                dblResult[4] = dblResult[4] * 100;

                // Rated harmonic 
                for (int i = 2; i <= 6; i++)
                {
                    _MXA_N9020A.SetFrequency(i * dblFreq);
                    util.Wait(intDelay_MXA);
                    dblResult[3 + i] = _MXA_N9020A.Get_CW_PowerResult() + TestSetting.LOSS_MSR_FILTER_LB[i * dblFreq];
                }
                // Extra harmonic 
                if (ext_har)
                {
                    for (int i = 7; i <= 14; i++)
                    {
                        _MXA_N9020A.SetFrequency(i * dblFreq);
                        util.Wait(intDelay_MXA);
                        dblResult[3 + i] = _MXA_N9020A.Get_CW_PowerResult() + TestSetting.LOSS_MSR_FILTER_LB[6000];
                    }
                }

                // Update_Grid
                DataRow drNew = dtCWLB.NewRow();
                DataRow drNewTmp = dtCWLBTMP.NewRow();
                drNewTmp[0] = drNew[0] = intTestID++;
                for (int i = 0; i < dblResult.Count(); i++)
                {
                    drNewTmp[i + 1] = drNew[i + 1] = Math.Round(dblResult[i], 2);
                }
                dtCWLBTMP.Rows.Add(drNewTmp);
                this.Invoke((MethodInvoker)(delegate
                {
                    dtCWLB.Rows.Add(drNew);
                    dgvSweepResult.Refresh();
                    Application.DoEvents();
                }));

            } // Frequency loop for power servo
            #endregion Power Servo

            #region --- Worst harmonic report ---

            foreach (double dblFreq in TestSetting.FREQLIST)
            {
                if (StopTest) break;

                DataRow drmax = dtCWLB.NewRow();
                drmax[0] = intTestID++;
                drmax[1] = dblFreq;

                var _2fo = from x in dtCWLBTMP.AsEnumerable()
                           where x.Field<double>("Frequency(MHz)") == dblFreq && double.Parse(x.Field<string>("Vramp(V)")) <= dicVramp[dblFreq]
                           select x.Field<double>("2fo(dBm)");

                var _3fo = from x in dtCWLBTMP.AsEnumerable()
                           where x.Field<double>("Frequency(MHz)") == dblFreq && double.Parse(x.Field<string>("Vramp(V)")) <= dicVramp[dblFreq]
                           select x.Field<double>("3fo(dBm)");

                var _4fo = from x in dtCWLBTMP.AsEnumerable()
                           where x.Field<double>("Frequency(MHz)") == dblFreq && double.Parse(x.Field<string>("Vramp(V)")) <= dicVramp[dblFreq]
                           select x.Field<double>("4fo(dBm)");

                var _5fo = from x in dtCWLBTMP.AsEnumerable()
                           where x.Field<double>("Frequency(MHz)") == dblFreq && double.Parse(x.Field<string>("Vramp(V)")) <= dicVramp[dblFreq]
                           select x.Field<double>("5fo(dBm)");

                var _6fo = from x in dtCWLBTMP.AsEnumerable()
                           where x.Field<double>("Frequency(MHz)") == dblFreq && double.Parse(x.Field<string>("Vramp(V)")) <= dicVramp[dblFreq]
                           select x.Field<double>("6fo(dBm)");

                try
                {
                    drmax[6] = _2fo.ToList<double>().Max();
                    drmax[7] = _3fo.ToList<double>().Max();
                    drmax[8] = _4fo.ToList<double>().Max();
                    drmax[9] = _5fo.ToList<double>().Max();
                    drmax[10] = _6fo.ToList<double>().Max();
                }
                catch
                {
                    drmax[6] = 0;
                    drmax[7] = 0;
                    drmax[8] = 0;
                    drmax[9] = 0;
                    drmax[10] = 0;
                }

                this.Invoke((MethodInvoker)(delegate
                {
                    dtCWLB.Rows.Add(drmax);
                    dgvSweepResult.Refresh();
                    Application.DoEvents();
                }));

            }
            #endregion *** Worst harmonic report ***

            #region --- After Test ---
            // After Test
            AfterTest();
            _E4438C.SetOutput(Output.OFF);
            _PS_66332A.SetOutput(Output.OFF);
            _Arb_33522A_USB.SetOutput(Output.OFF, Arb_Channel.Channel_1);
            _Arb_33522A_USB.SetOutput(Output.OFF, Arb_Channel.Channel_2);

            #endregion --- After Test ---


        }

        void CWHB_Test_SH_1()
        {
            #region --- Variable Define ---
            dicVramp.Clear();
            int intTestID = 1;
            double[] dblResult = new double[11];

            DataTable dtCWHBTMP = new DataTable();
            dtCWHBTMP = dtCWHB.Clone();

            if (!cbxKeepPrevious.Checked)
            {
                this.Invoke((MethodInvoker)(delegate
                {
                    dtCWHB.Clear();
                    dgvSweepResult.Refresh();
                }));
            }
            #endregion --- Variable Define ---

            #region --- Initialize ---
            BeforeTest();

            SetVCC(TestSetting.LEVEL_VCC);
            SetTXEnable(TestSetting.LEVEL_TXEN);
            SetGPCTRL(TestSetting.LEVEL_GPCTRL);

            _E4438C.Initialize();
            _PM_N1913A.Initialize(rbnDisplayON.Checked);
            _MXA_N9020A.Initialize(rbnDisplayON.Checked);
            _E4438C.SetOutput(Output.ON);

            util.Wait(1000);
            Application.DoEvents();

            SetVCC(TestSetting.LEVEL_VCC);
            SetTXEnable(TestSetting.LEVEL_TXEN);
            SetGPCTRL(TestSetting.LEVEL_GPCTRL);

            _E4438C.Initialize();
            _PM_N1913A.Initialize(rbnDisplayON.Checked);
            _MXA_N9020A.Initialize(rbnDisplayON.Checked);
            _E4438C.SetOutput(Output.ON);

            DateTime t_Satrt = DateTime.Now;

            string Content = "Make sure everything is setup for TX HB testing \r\n" +
                            "  1) Connect rf source cable to TX HB \r\n" +
                            "  2) Set Control box to TX HB mode \r\n" +
                            "  3) Connect / Change HB highpass filter";

            this.Invoke((MethodInvoker)(delegate
            {
                MessageBox.Show(this, Content, "GMSK HB Testing", MessageBoxButtons.OK);
                Application.DoEvents();
                wait_2_start(t_Satrt);
            }));

            #endregion --- Initialize ---

            #region --- Vramp Sweep ---
            for (double dblVramp = TestSetting.LEVEL_START; dblVramp < TestSetting.LEVEL_STOP + 0.01; dblVramp += TestSetting.LEVEL_STEP)
            {
                if (StopTest) break;
                SetVramp(dblVramp);

                foreach (double dblFreq in TestSetting.FREQLIST)
                {
                    if (StopTest) break;
                    _E4438C.SetFrequency(dblFreq);
                    _E4438C.SetPower(TestSetting.LEVEL_PIN_VRAMP + TestSetting.LOSS_SRC[dblFreq]);

                    // Frequency
                    dblResult[0] = dblFreq;

                    // Vramp
                    dblResult[1] = dblVramp;

                    // Pout
                    _PM_N1913A.Configure__Average_Pulse_Power_with_Duty_Cycle(dblFreq, 25, 256);

                    util.Wait(intDelay_PowerMeter * intDelay_N1913A_Count);
                    dblResult[2] = _PM_N1913A.GetPowerResult() + TestSetting.LOSS_MSR_POUT[dblFreq];

                    // ICC
                    dblResult[3] = _PS_66332A.High_Current();
                    dblResult[3] = dblResult[3] * 1000;

                    // PAE
                    dblResult[4] = (Math.Pow(10, ((dblResult[2] - 30) / 10))) / TestSetting.LEVEL_VCC / dblResult[3] * 1000;
                    dblResult[4] = dblResult[4] * 100;

                    // harmonic 
                    for (int i = 2; i <= 3; i++)
                    {
                        _MXA_N9020A.SetFrequency(i * dblFreq);

                        util.Wait(intDelay_MXA);

                        dblResult[3 + i] = _MXA_N9020A.Get_CW_PowerResult() + TestSetting.LOSS_MSR_FILTER_HB[i * dblFreq];
                    }
                    // Extra harmonic 
                    if (ext_har)
                    {
                        for (int i = 4; i <= 7; i++)
                        {
                            _MXA_N9020A.SetFrequency(i * dblFreq);
                            util.Wait(intDelay_MXA);
                            dblResult[3 + i] = _MXA_N9020A.Get_CW_PowerResult() + TestSetting.LOSS_MSR_FILTER_HB[6000];
                        }
                    }
                    // Update_Grid
                    DataRow drNew = dtCWHB.NewRow();
                    DataRow drNewTmp = dtCWHBTMP.NewRow();
                    drNewTmp[0] = drNew[0] = intTestID++;
                    for (int i = 0; i < dblResult.Count(); i++)
                    {
                        drNewTmp[i + 1] = drNew[i + 1] = Math.Round(dblResult[i], 2);
                    }
                    dtCWHBTMP.Rows.Add(drNewTmp);
                    this.Invoke((MethodInvoker)(delegate
                    {
                        dtCWHB.Rows.Add(drNew);
                        dgvSweepResult.Refresh();
                        Application.DoEvents();
                    }));

                }   // Frequency loop
            }  // Vramp loop
            #endregion *** Vramp Sweep ***

            #region --- Power Servo ---
            foreach (double dblFreq in TestSetting.FREQLIST)
            {
                if (StopTest) break;
                int Count = 0;
                _E4438C.SetFrequency(dblFreq);
                _E4438C.SetPower(TestSetting.LEVEL_PIN_VRAMP + TestSetting.LOSS_SRC[dblFreq]);

                #region Search power
                double LoopResult_Low;
                double LoopResult_High;
                double LoopResult;
                double Slope_mV;

                this.SetVramp(1.2);
                _PM_N1913A.Configure__Average_Pulse_Power_with_Duty_Cycle(dblFreq, 25, 256);
                util.Wait(intDelay_PowerMeter * intDelay_N1913A_Count);
                LoopResult_Low = _PM_N1913A.GetPowerResult();

                this.SetVramp(1.6);
                _PM_N1913A.Configure__Average_Pulse_Power_with_Duty_Cycle(dblFreq, 25, 256);
                util.Wait(intDelay_PowerMeter * intDelay_N1913A_Count);
                LoopResult_High = _PM_N1913A.GetPowerResult();

                Slope_mV = (LoopResult_High - LoopResult_Low) / 200;

                LoopResult = LoopResult_Low + TestSetting.LOSS_MSR_POUT[dblFreq];
                dblRatedVramp = 1.2 + (TestSetting.TARGET_POUT_CWHB - LoopResult) / Slope_mV / 1000;

                if (dblRatedVramp < 0.8 || dblRatedVramp > 1.8) dblRatedVramp = 1.8;

                while (Math.Abs(LoopResult - TestSetting.TARGET_POUT_CWHB) > 0.05 && dblRatedVramp < 1.8 && dblRatedVramp > 0.8)
                {
                    this.SetVramp(dblRatedVramp);
                    util.Wait(intDelay_PowerMeter * intDelay_N1913A_Count);
                    LoopResult = _PM_N1913A.GetPowerResult() + TestSetting.LOSS_MSR_POUT[dblFreq];
                    dblRatedVramp = dblRatedVramp + (TestSetting.TARGET_POUT_CWHB - LoopResult) / Slope_mV / 1000;
                    Count++;
                    if (Count > 20) break;
                }
                #endregion Search power

                // Frequency
                dblResult[0] = dblFreq;
                // Rated Vramp
                dblResult[1] = dblRatedVramp;
                if (!dicVramp.ContainsKey(dblFreq)) dicVramp.Add(dblFreq, Math.Round(dblRatedVramp, 2));
                //Rated Pout
                dblResult[2] = LoopResult;

                // Rated ICC
                dblResult[3] = _PS_66332A.High_Current();
                dblResult[3] = dblResult[3] * 1000;

                // Rated PAE
                dblResult[4] = (Math.Pow(10, ((dblResult[2] - 30) / 10))) / TestSetting.LEVEL_VCC / dblResult[3] * 1000;
                dblResult[4] = dblResult[4] * 100;

                // Rated harmonic 
                for (int i = 2; i <= 3; i++)
                {
                    _MXA_N9020A.SetFrequency(i * dblFreq);
                    util.Wait(intDelay_MXA);
                    dblResult[3 + i] = _MXA_N9020A.Get_CW_PowerResult() + TestSetting.LOSS_MSR_FILTER_HB[i * dblFreq];
                }
                // Extra harmonic 
                if (ext_har)
                {
                    for (int i = 4; i <= 7; i++)
                    {
                        _MXA_N9020A.SetFrequency(i * dblFreq);
                        util.Wait(intDelay_MXA);
                        dblResult[3 + i] = _MXA_N9020A.Get_CW_PowerResult() + TestSetting.LOSS_MSR_FILTER_HB[6000];
                    }
                }
                // Update_Grid
                DataRow drNew = dtCWHB.NewRow();
                DataRow drNewTmp = dtCWHBTMP.NewRow();
                drNewTmp[0] = drNew[0] = intTestID++;
                for (int i = 0; i < dblResult.Count(); i++)
                {
                    drNewTmp[i + 1] = drNew[i + 1] = Math.Round(dblResult[i], 2);
                }
                dtCWHBTMP.Rows.Add(drNewTmp);
                this.Invoke((MethodInvoker)(delegate
                {
                    dtCWHB.Rows.Add(drNew);
                    dgvSweepResult.Refresh();
                    Application.DoEvents();
                }));

            } // Frequency loop for power servo
            #endregion Power Servo

            #region --- Worst harmonic report ---

            foreach (double dblFreq in TestSetting.FREQLIST)
            {
                if (StopTest) break;
                DataRow drmax = dtCWHB.NewRow();
                drmax[0] = intTestID++;
                drmax[1] = dblFreq;

                var _2fo = from x in dtCWHBTMP.AsEnumerable()
                           where x.Field<double>("Frequency(MHz)") == dblFreq && double.Parse(x.Field<string>("Vramp(V)")) <= dicVramp[dblFreq]
                           select x.Field<double>("2fo(dBm)");

                var _3fo = from x in dtCWHBTMP.AsEnumerable()
                           where x.Field<double>("Frequency(MHz)") == dblFreq && double.Parse(x.Field<string>("Vramp(V)")) <= dicVramp[dblFreq]
                           select x.Field<double>("3fo(dBm)");
                try
                {
                    drmax[6] = _2fo.ToList<double>().Max();
                    drmax[7] = _3fo.ToList<double>().Max();
                }
                catch
                {
                    drmax[6] = 0;
                    drmax[7] = 0;
                }

                this.Invoke((MethodInvoker)(delegate
                {
                    dtCWHB.Rows.Add(drmax);
                    dgvSweepResult.Refresh();
                    Application.DoEvents();
                }));
            }
            #endregion *** Worst harmonic report ***

            #region --- After Test ---
            // After Test
            AfterTest();
            _E4438C.SetOutput(Output.OFF);
            _PS_66332A.SetOutput(Output.OFF);
            _Arb_33522A_USB.SetOutput(Output.OFF, Arb_Channel.Channel_1);
            _Arb_33522A_USB.SetOutput(Output.OFF, Arb_Channel.Channel_2);

            #endregion --- After Test ---

        }

        void EDGELB_TEST_SH_1()
        {
            #region --- Variable Define ---

            Dictionary<double, double> lastEDGELBPin = new Dictionary<double, double>();
            int intDelay;
            int intTestID = 1;
            double Pin_Start = -10;
            double[] dblResult = new double[10];

            DataTable dtEDGELBTMP = new DataTable();
            dtEDGELBTMP = dtEDGELB.Clone();

            if (!cbxKeepPrevious.Checked)
            {
                this.Invoke((MethodInvoker)(delegate
                {
                    dtEDGELB.Clear();
                    dgvSweepResult.Refresh();
                }));
            }

            #endregion --- Variable Define ---

            #region --- Initialize ---

            BeforeTest();

            SetVCC(TestSetting.LEVEL_VCC);
            SetTXEnable(TestSetting.LEVEL_TXEN);
            SetVramp(TestSetting.LEVEL_PIN_VRAMP);
            SetGPCTRL(TestSetting.LEVEL_GPCTRL);

            _E4438C.Mode_Initialize(Modulation.EDGE);
            _MXA_N9020A.Mod_Initialize(Modulation.EDGE);

            _E4438C.SetModOutput(Output.ON);
            _E4438C.SetOutput(Output.ON);

            string Content = "Make sure everything is setup for EDGELB testing \r\n" +
                              "  1) Connect rf source cable to EDGELB \r\n" +
                              "  2) Set Control box to EDGELB mode \r\n" +
                              "  3) Connect MXA to coupler out directly";

            this.Invoke((MethodInvoker)(delegate
            {
                MessageBox.Show(this, Content, "EDGELB Testing", MessageBoxButtons.OK);
                Application.DoEvents();
            }));
            util.Wait(3000);

            #endregion --- Initialize ---

            #region --- EDGELB Test ---

            for (double dblPout_Target = TestSetting.LEVEL_START; dblPout_Target <= TestSetting.LEVEL_STOP; dblPout_Target += TestSetting.LEVEL_STEP)
            {
                if (StopTest) break;

                double PoutLL = dblPout_Target - 0.05;
                double PoutUL = dblPout_Target + 0.05;

                foreach (double dblFreq in TestSetting.FREQLIST)
                {
                    if (StopTest) break;

                    int Count = 0;
                    double EDGELB_Pout = 0;
                    double EDGELB_Pin = 0;
                    double EDGELB_Icc = 0;
                    double dblPin = Pin_Start;
                    double[] dblACPResult = new double[10];

                    if (lastEDGELBPin.ContainsKey(dblFreq)) EDGELB_Pin = dblPin = Pin_Start = lastEDGELBPin[dblFreq] + TestSetting.LEVEL_STEP;
                    _E4438C.SetFrequency(dblFreq);
                    _E4438C.SetPower(Pin_Start + TestSetting.LOSS_SRC[dblFreq]);

                    _MXA_N9020A.Config__EDGE_Burst_Power(dblFreq);
                    intDelay = Math.Max(intDelay_SigGen, intDelay_MXA) * 5;
                    util.Wait(intDelay);

                    EDGELB_Pout = _MXA_N9020A.Get_EDGE_Burst_Power_Result();
                    EDGELB_Pout += TestSetting.LOSS_MSR_THROUGH[dblFreq];

                    // Freqquency
                    dblResult[0] = dblFreq;

                    // Target Power
                    dblResult[1] = dblPout_Target;

                    //Pout
                    while (EDGELB_Pout <= PoutLL || EDGELB_Pout >= PoutUL)
                    {
                        dblPin = dblPin + dblPout_Target - EDGELB_Pout + 0.02;
                        if (dblPin > 5)
                            break;

                        _E4438C.SetPower(dblPin + TestSetting.LOSS_SRC[dblFreq]);
                        util.Wait(intDelay);
                        EDGELB_Pout = _MXA_N9020A.Get_EDGE_Burst_Power_Result();
                        EDGELB_Pout += TestSetting.LOSS_MSR_THROUGH[dblFreq];
                        EDGELB_Pin = dblPin;
                        Count++;
                        //if (Count > 20) break;
                    }
                    dblResult[2] = EDGELB_Pout;

                    // Pin
                    dblResult[3] = EDGELB_Pin;
                    if (lastEDGELBPin.ContainsKey(dblFreq))
                        lastEDGELBPin[dblFreq] = EDGELB_Pin;
                    else
                        lastEDGELBPin.Add(dblFreq, EDGELB_Pin);

                    // Gain
                    dblResult[4] = EDGELB_Pout - EDGELB_Pin;

                    // Icc
                    EDGELB_Icc = _PS_66332A.High_Current();
                    dblResult[5] = EDGELB_Icc * 1000;
                    // PAE
                    dblResult[6] = 100 * (Math.Pow(10, ((EDGELB_Pout - 30) / 10))) / TestSetting.LEVEL_VCC / EDGELB_Icc;

                    //ACP
                    _MXA_N9020A.Config__EDGE_ACP(dblFreq);
                    intDelay = Math.Max(intDelay_SigGen, intDelay_MXA);
                    util.Wait(intDelay * 3);

                    dblACPResult = _MXA_N9020A.Get_EDGE_ACP_Result();

                    dblResult[7] = dblACPResult[6];     // -400kHz
                    dblResult[8] = dblACPResult[7];     // +400kHz

                    // EVM
                    if (!cbxBypassEVM.Checked)
                    {
                        _MXA_N9020A.Config__EDGE_EVM(dblFreq);
                        intDelay = Math.Max(intDelay_SigGen, intDelay_MXA);
                        util.Wait(intDelay * 3);

                        dblResult[9] = _MXA_N9020A.Get_EDGE_EVM_Result();
                    }
                    else
                    {
                        dblResult[9] = 0;
                    }
                    // Update_Grid
                    DataRow drNew = dtEDGELB.NewRow();
                    DataRow drNewtmp = dtEDGELBTMP.NewRow();
                    drNewtmp[0] = drNew[0] = intTestID++;
                    for (int i = 0; i < dblResult.Count(); i++)
                    {
                        drNew[i + 1] = drNewtmp[i + 1] = Math.Round(dblResult[i], 2);
                    }
                    dtEDGELBTMP.Rows.Add(drNewtmp);

                    this.Invoke((MethodInvoker)(delegate
                    {
                        dtEDGELB.Rows.Add(drNew);
                        dgvSweepResult.Refresh();
                        Application.DoEvents();
                    }));
                }
            }

            #endregion --- EDGELB Test ---

            #region --- Worst acp report ---

            foreach (double dblFreq in TestSetting.FREQLIST)
            {
                DataRow drmax = dtEDGELB.NewRow();
                drmax[0] = intTestID++;
                drmax[1] = dblFreq;

                var acp_n400 = from x in dtEDGELBTMP.AsEnumerable()
                               where x.Field<double>("Frequency (MHz)") == dblFreq
                               select x.Field<double>("ACP -400kHz(dB)");

                var acp_p400 = from x in dtEDGELBTMP.AsEnumerable()
                               where x.Field<double>("Frequency (MHz)") == dblFreq
                               select x.Field<double>("ACP +400kHz(dB)");

                var evm = from x in dtEDGELBTMP.AsEnumerable()
                          where x.Field<double>("Frequency (MHz)") == dblFreq
                          select x.Field<double>("EVM(%)");

                try
                {
                    drmax[8] = acp_n400.ToList<double>().Max();
                    drmax[9] = acp_p400.ToList<double>().Max();
                    drmax[10] = evm.ToList<double>().Max();
                }
                catch
                {
                    drmax[8] = 0;
                    drmax[9] = 0;
                    drmax[10] = 0;
                }

                this.Invoke((MethodInvoker)(delegate
                {
                    dtEDGELB.Rows.Add(drmax);
                    dgvSweepResult.Refresh();
                    Application.DoEvents();
                }));

            }
            #endregion *** Worst acp report ***

            #region --- After Test ---

            AfterTest();
            _E4438C.SetOutput(Output.OFF);
            _PS_66332A.SetOutput(Output.OFF);
            //_PS_E3631A.SetOutput(Output.OFF);
            _Arb_33522A_USB.SetOutput(Output.OFF, Arb_Channel.Channel_1);
            _Arb_33522A_USB.SetOutput(Output.OFF, Arb_Channel.Channel_2);

            #endregion --- After Test ---
        }

        void EDGEHB_TEST_SH_1()
        {
            #region --- Variable Define ---

            Dictionary<double, double> lastEDGEHBPin = new Dictionary<double, double>();
            int intDelay;
            int intTestID = 1;
            double Pin_Start = -10;
            double[] dblResult = new double[10];

            DataTable dtEDGEHBTMP = new DataTable();
            dtEDGEHBTMP = dtEDGEHB.Clone();

            if (!cbxKeepPrevious.Checked)
            {
                this.Invoke((MethodInvoker)(delegate
                {
                    dtEDGEHB.Clear();
                    dgvSweepResult.Refresh();
                }));
            }

            #endregion --- Variable Define ---

            #region --- Initialize ---

            BeforeTest();

            SetVCC(TestSetting.LEVEL_VCC);
            SetTXEnable(TestSetting.LEVEL_TXEN);
            SetVramp(TestSetting.LEVEL_PIN_VRAMP);
            SetGPCTRL(TestSetting.LEVEL_GPCTRL);

            _E4438C.Mode_Initialize(Modulation.EDGE);
            _MXA_N9020A.Mod_Initialize(Modulation.EDGE);

            _E4438C.SetModOutput(Output.ON);
            _E4438C.SetOutput(Output.ON);
            //util.Wait(5000);

            string Content = "Make sure everything is setup for EDGEHB testing \r\n" +
                              "  1) Connect rf source cable to EDGEHB \r\n" +
                              "  2) Set Control box to EDGEHB mode \r\n" +
                              "  3) Connect MXA to coupler out directly";

            this.Invoke((MethodInvoker)(delegate
            {
                MessageBox.Show(this, Content, "EDGEHB Testing", MessageBoxButtons.OK);
                Application.DoEvents();
            }));
            util.Wait(3000);

            #endregion --- Initialize ---

            #region --- EDGEHB Test ---

            for (double dblPout_Target = TestSetting.LEVEL_START; dblPout_Target <= TestSetting.LEVEL_STOP; dblPout_Target += TestSetting.LEVEL_STEP)
            {
                if (StopTest) break;

                double PoutLL = dblPout_Target - 0.05;
                double PoutUL = dblPout_Target + 0.05;

                foreach (double dblFreq in TestSetting.FREQLIST)
                {
                    if (StopTest) break;

                    int Count = 0;
                    double EDGEHB_Pout = 0;
                    double EDGEHB_Pin = 0;
                    double EDGEHB_Icc = 0;
                    double dblPin = Pin_Start;
                    double[] dblACPResult = new double[10];

                    if (lastEDGEHBPin.ContainsKey(dblFreq)) EDGEHB_Pin = dblPin = Pin_Start = lastEDGEHBPin[dblFreq] + TestSetting.LEVEL_STEP;
                    _E4438C.SetFrequency(dblFreq);
                    _E4438C.SetPower(Pin_Start + TestSetting.LOSS_SRC[dblFreq]);

                    _MXA_N9020A.Config__EDGE_Burst_Power(dblFreq);
                    intDelay = Math.Max(intDelay_SigGen, intDelay_MXA) * 5;
                    util.Wait(intDelay);

                    EDGEHB_Pout = _MXA_N9020A.Get_EDGE_Burst_Power_Result();
                    EDGEHB_Pout += TestSetting.LOSS_MSR_THROUGH[dblFreq];

                    // Freqquency
                    dblResult[0] = dblFreq;

                    // Target Power
                    dblResult[1] = dblPout_Target;

                    //Pout
                    while (EDGEHB_Pout <= PoutLL || EDGEHB_Pout >= PoutUL)
                    {
                        dblPin = dblPin + dblPout_Target - EDGEHB_Pout + 0.02;
                        if (dblPin > 5)
                            break;

                        _E4438C.SetPower(dblPin + TestSetting.LOSS_SRC[dblFreq]);
                        util.Wait(intDelay);
                        EDGEHB_Pout = _MXA_N9020A.Get_EDGE_Burst_Power_Result();
                        EDGEHB_Pout += TestSetting.LOSS_MSR_THROUGH[dblFreq];
                        EDGEHB_Pin = dblPin;
                        Count++;
                        if (Count > 20) break;
                    }
                    dblResult[2] = EDGEHB_Pout;

                    // Pin
                    dblResult[3] = EDGEHB_Pin;
                    if (lastEDGEHBPin.ContainsKey(dblFreq))
                        lastEDGEHBPin[dblFreq] = EDGEHB_Pin;
                    else
                        lastEDGEHBPin.Add(dblFreq, EDGEHB_Pin);

                    // Gain
                    dblResult[4] = EDGEHB_Pout - EDGEHB_Pin;

                    // Icc
                    EDGEHB_Icc = _PS_66332A.High_Current();
                    dblResult[5] = EDGEHB_Icc * 1000;
                    // PAE
                    dblResult[6] = 100 * (Math.Pow(10, ((EDGEHB_Pout - 30) / 10))) / TestSetting.LEVEL_VCC / EDGEHB_Icc;

                    //ACP
                    _MXA_N9020A.Config__EDGE_ACP(dblFreq);
                    intDelay = Math.Max(intDelay_SigGen, intDelay_MXA);
                    util.Wait(intDelay * 3);

                    dblACPResult = _MXA_N9020A.Get_EDGE_ACP_Result();

                    dblResult[7] = dblACPResult[6];     // -400kHz
                    dblResult[8] = dblACPResult[7];     // +400kHz

                    // EVM
                    if (!cbxBypassEVM.Checked)
                    {
                        _MXA_N9020A.Config__EDGE_EVM(dblFreq);
                        intDelay = Math.Max(intDelay_SigGen, intDelay_MXA);
                        util.Wait(intDelay * 3);

                        dblResult[9] = _MXA_N9020A.Get_EDGE_EVM_Result();
                    }
                    else
                    {
                        dblResult[9] = 0;
                    }
                    // Update_Grid
                    DataRow drNew = dtEDGEHB.NewRow();
                    DataRow drNewtmp = dtEDGEHBTMP.NewRow();
                    drNewtmp[0] = drNew[0] = intTestID++;
                    for (int i = 0; i < dblResult.Count(); i++)
                    {
                        drNew[i + 1] = drNewtmp[i + 1] = Math.Round(dblResult[i], 2);
                    }
                    dtEDGEHBTMP.Rows.Add(drNewtmp);

                    this.Invoke((MethodInvoker)(delegate
                    {
                        dtEDGEHB.Rows.Add(drNew);
                        dgvSweepResult.Refresh();
                        Application.DoEvents();
                    }));
                }
            }

            #endregion --- EDGEHB Test ---

            #region --- Worst acp report ---

            foreach (double dblFreq in TestSetting.FREQLIST)
            {
                DataRow drmax = dtEDGEHB.NewRow();
                drmax[0] = intTestID++;
                drmax[1] = dblFreq;

                var acp_n400 = from x in dtEDGEHBTMP.AsEnumerable()
                               where x.Field<double>("Frequency (MHz)") == dblFreq
                               select x.Field<double>("ACP -400kHz(dB)");

                var acp_p400 = from x in dtEDGEHBTMP.AsEnumerable()
                               where x.Field<double>("Frequency (MHz)") == dblFreq
                               select x.Field<double>("ACP +400kHz(dB)");

                var evm = from x in dtEDGEHBTMP.AsEnumerable()
                          where x.Field<double>("Frequency (MHz)") == dblFreq
                          select x.Field<double>("EVM(%)");

                try
                {
                    drmax[8] = acp_n400.ToList<double>().Max();
                    drmax[9] = acp_p400.ToList<double>().Max();
                    drmax[10] = evm.ToList<double>().Max();
                }
                catch
                {
                    drmax[8] = 0;
                    drmax[9] = 0;
                    drmax[10] = 0;
                }

                this.Invoke((MethodInvoker)(delegate
                {
                    dtEDGEHB.Rows.Add(drmax);
                    dgvSweepResult.Refresh();
                    Application.DoEvents();
                }));

            }
            #endregion *** Worst acp report ***

            #region --- After Test ---

            AfterTest();
            _E4438C.SetOutput(Output.OFF);
            _PS_66332A.SetOutput(Output.OFF);
            //_PS_E3631A.SetOutput(Output.OFF);
            _Arb_33522A_USB.SetOutput(Output.OFF, Arb_Channel.Channel_1);
            _Arb_33522A_USB.SetOutput(Output.OFF, Arb_Channel.Channel_2);

            #endregion --- After Test ---
        }

        void TDSCDMA_TEST_SH_1()
        {
            #region --- Variable Define ---

            Dictionary<double, double> lastTDSCDMAPin = new Dictionary<double, double>();
            int intDelay;
            int intTestID = 1;
            double Pin_Start = -10;
            double[] dblResult = new double[12];

            DataTable dtTDSCDMATMP = new DataTable();
            dtTDSCDMATMP = dtTDSCDMA.Clone();

            if (!cbxKeepPrevious.Checked)
            {
                this.Invoke((MethodInvoker)(delegate
                {
                    dtTDSCDMA.Clear();
                    dgvSweepResult.Refresh();
                }));
            }

            #endregion --- Variable Define ---

            #region --- Initialize ---

            BeforeTest();
            //_PS_E3631A = new PS_E3631A(Instruments_address._06);
            //_PS_E3631A.Initialize();

            SetVCC(TestSetting.LEVEL_VCC);
            //SetVbat(TestSetting.LEVEL_VBAT, 1);
            SetTXEnable(TestSetting.LEVEL_TXEN);
            SetVramp(TestSetting.LEVEL_PIN_VRAMP);
            SetGPCTRL(TestSetting.LEVEL_GPCTRL);

            if (td_hsdpa)
            {
                _E4438C.Mode_Initialize(Modulation.TDSCDMA, true);
                _MXA_N9020A.Mod_Initialize(Modulation.TDSCDMA, true);
            }
            else
            {
                _E4438C.Mode_Initialize(Modulation.TDSCDMA);
                _MXA_N9020A.Mod_Initialize(Modulation.TDSCDMA);
            }

            _E4438C.SetModOutput(Output.ON);
            _E4438C.SetOutput(Output.ON);

            string Content = "Make sure everything is setup for TDSCDMA testing \r\n" +
                              "  1) Connect rf source cable to TDSCDMA \r\n" +
                              "  2) Set Control box to TDSCDMA mode \r\n" +
                              "  3) Connect MXA to coupler out directly";

            this.Invoke((MethodInvoker)(delegate
            {
                MessageBox.Show(this, Content, "TDSCDMA Testing", MessageBoxButtons.OK);
                Application.DoEvents();
            }));

            #endregion --- Initialize ---

            #region --- TDSCDMA Test ---

            for (double dblPout_Target = TestSetting.LEVEL_START; dblPout_Target <= TestSetting.LEVEL_STOP; dblPout_Target += TestSetting.LEVEL_STEP)
            {
                if (StopTest) break;

                double PoutLL = dblPout_Target - 0.05;
                double PoutUL = dblPout_Target + 0.05;

                foreach (double dblFreq in TestSetting.FREQLIST)
                {
                    if (StopTest) break;

                    double TDSCDMA_Pout = 0;
                    double TDSCDMA_Pin = 0;
                    double TDSCDMA_Icc = 0;
                    double dblPin = Pin_Start;
                    double[] dblACPResult = new double[5];

                    if (lastTDSCDMAPin.ContainsKey(dblFreq)) TDSCDMA_Pin = dblPin = Pin_Start = lastTDSCDMAPin[dblFreq] + TestSetting.LEVEL_STEP;
                    _E4438C.SetFrequency(dblFreq);
                    _E4438C.SetPower(Pin_Start + TestSetting.LOSS_SRC[dblFreq]);

                    _MXA_N9020A.Config__TDSCDMA_CHP(dblFreq);
                    intDelay = Math.Max(intDelay_SigGen, intDelay_MXA) * 10;
                    util.Wait(intDelay);

                    TDSCDMA_Pout = _MXA_N9020A.Get_TDSCDMA_CHP_Result();
                    TDSCDMA_Pout += TestSetting.LOSS_MSR_THROUGH[dblFreq];

                    // Freqquency
                    dblResult[0] = dblFreq;

                    // Target Power
                    dblResult[1] = dblPout_Target;

                    //Pout
                    while (TDSCDMA_Pout <= PoutLL || TDSCDMA_Pout >= PoutUL)
                    {
                        dblPin = dblPin + dblPout_Target - TDSCDMA_Pout;
                        if (dblPin > 5)
                            break;

                        _E4438C.SetPower(dblPin + TestSetting.LOSS_SRC[dblFreq]);
                        util.Wait(intDelay);
                        TDSCDMA_Pout = _MXA_N9020A.Get_TDSCDMA_CHP_Result();
                        TDSCDMA_Pout += TestSetting.LOSS_MSR_THROUGH[dblFreq];
                        TDSCDMA_Pin = dblPin;
                    }
                    dblResult[2] = TDSCDMA_Pout;

                    // Pin
                    dblResult[3] = TDSCDMA_Pin;
                    if (lastTDSCDMAPin.ContainsKey(dblFreq))
                        lastTDSCDMAPin[dblFreq] = TDSCDMA_Pin;
                    else
                        lastTDSCDMAPin.Add(dblFreq, TDSCDMA_Pin);

                    // Gain
                    dblResult[4] = TDSCDMA_Pout - TDSCDMA_Pin;

                    // Icc
                    TDSCDMA_Icc = _PS_66332A.Max_Current();
                    dblResult[5] = TDSCDMA_Icc * 1000;
                    // PAE
                    dblResult[6] = 100 * (Math.Pow(10, ((TDSCDMA_Pout - 30) / 10))) / TestSetting.LEVEL_VCC / TDSCDMA_Icc;

                    //ACP
                    _MXA_N9020A.Config__TDSCDMA_ACP(dblFreq, 0);
                    intDelay = Math.Max(intDelay_SigGen, intDelay_MXA);
                    util.Wait(intDelay * 3);

                    dblACPResult = _MXA_N9020A.Get_TDSCDMA_ACP_Result();

                    dblResult[7] = dblACPResult[1];     // -5MHz
                    dblResult[8] = dblACPResult[2];     // +5MHz
                    dblResult[9] = dblACPResult[3];     // -10MHz
                    dblResult[10] = dblACPResult[4];     // +10MHz

                    // EVM
                    if (!cbxBypassEVM.Checked)
                    {
                        _MXA_N9020A.Config__TDSCDMA_EVM(dblFreq);
                        intDelay = Math.Max(intDelay_SigGen, intDelay_MXA);
                        util.Wait(intDelay * 3);

                        dblResult[11] = _MXA_N9020A.Get_TDSCDMA_EVM_Result();
                    }
                    else
                    {
                        dblResult[11] = 0;
                    }
                    // Update_Grid
                    DataRow drNew = dtTDSCDMA.NewRow();
                    DataRow drNewtmp = dtTDSCDMATMP.NewRow();
                    drNewtmp[0] = drNew[0] = intTestID++;
                    for (int i = 0; i < dblResult.Count(); i++)
                    {
                        drNew[i + 1] = drNewtmp[i + 1] = Math.Round(dblResult[i], 2);
                    }
                    dtTDSCDMATMP.Rows.Add(drNewtmp);

                    this.Invoke((MethodInvoker)(delegate
                    {
                        dtTDSCDMA.Rows.Add(drNew);
                        dgvSweepResult.Refresh();
                        Application.DoEvents();
                    }));
                }
            }

            #endregion --- TDSCDMA Test ---

            #region --- Worst acp report ---

            foreach (double dblFreq in TestSetting.FREQLIST)
            {
                DataRow drmax = dtTDSCDMA.NewRow();
                drmax[0] = intTestID++;
                drmax[1] = dblFreq;

                var acp_n5 = from x in dtTDSCDMATMP.AsEnumerable()
                             where x.Field<double>("Frequency (MHz)") == dblFreq
                             select x.Field<double>("ACP -1.6MHz(dB)");

                var acp_p5 = from x in dtTDSCDMATMP.AsEnumerable()
                             where x.Field<double>("Frequency (MHz)") == dblFreq
                             select x.Field<double>("ACP +1.6MHz(dB)");

                var acp_n10 = from x in dtTDSCDMATMP.AsEnumerable()
                              where x.Field<double>("Frequency (MHz)") == dblFreq
                              select x.Field<double>("ACP -3.2MHz(dB)");

                var acp_p10 = from x in dtTDSCDMATMP.AsEnumerable()
                              where x.Field<double>("Frequency (MHz)") == dblFreq
                              select x.Field<double>("ACP +3.2MHz(dB)");

                var evm = from x in dtTDSCDMATMP.AsEnumerable()
                          where x.Field<double>("Frequency (MHz)") == dblFreq
                          select x.Field<double>("EVM(%)");

                try
                {
                    drmax[8] = acp_n5.ToList<double>().Max();
                    drmax[9] = acp_p5.ToList<double>().Max();
                    drmax[10] = acp_n10.ToList<double>().Max();
                    drmax[11] = acp_p10.ToList<double>().Max();
                    drmax[12] = evm.ToList<double>().Max();
                }
                catch
                {
                    drmax[8] = 0;
                    drmax[9] = 0;
                    drmax[10] = 0;
                    drmax[11] = 0;
                    drmax[12] = 0;
                }

                this.Invoke((MethodInvoker)(delegate
                {
                    dtTDSCDMA.Rows.Add(drmax);
                    dgvSweepResult.Refresh();
                    Application.DoEvents();
                }));

            }
            #endregion *** Worst acp report ***

            #region --- After Test ---

            AfterTest();
            _E4438C.SetOutput(Output.OFF);
            _PS_66332A.SetOutput(Output.OFF);
            //_PS_E3631A.SetOutput(Output.OFF);
            _Arb_33522A_USB.SetOutput(Output.OFF, Arb_Channel.Channel_1);
            _Arb_33522A_USB.SetOutput(Output.OFF, Arb_Channel.Channel_2);

            #endregion --- After Test ---
        }

        void WCDMA_TEST_SH_1()
        {
            #region --- Variable Define ---

            Dictionary<double, double> lastPin = new Dictionary<double, double>();
            int intDelay;
            int intTestID = 1;
            double Pin_Start = -10;
            double[] dblResult = new double[12];

            DataTable dtWCDMATMP = new DataTable();
            dtWCDMATMP = dtWCDMA.Clone();

            if (!cbxKeepPrevious.Checked)
            {
                this.Invoke((MethodInvoker)(delegate
                {
                    dtWCDMA.Clear();
                    dgvSweepResult.Refresh();
                }));
            }

            #endregion --- Variable Define ---

            #region --- Initialize ---

            BeforeTest();
            //_PS_E3631A = new PS_E3631A(Instruments_address._06);
            //_PS_E3631A.Initialize();

            SetVCC(TestSetting.LEVEL_VCC);
            //SetVbat(TestSetting.LEVEL_VBAT, 1);
            //SetTXEnable(TestSetting.LEVEL_TXEN);
            //SetVramp(TestSetting.LEVEL_PIN_VRAMP);
            SetGPCTRL(TestSetting.LEVEL_GPCTRL);

            _E4438C.Mode_Initialize(Modulation.WCDMA);
            _MXA_N9020A.Mod_Initialize(Modulation.WCDMA);

            _E4438C.SetModOutput(Output.ON);
            _E4438C.SetOutput(Output.ON);

            string Content = "Make sure everything is setup for WCDMA testing \r\n" +
                              "  1) Connect rf source cable to WCDMA \r\n" +
                              "  2) Set Control box to WCDMA mode \r\n" +
                              "  3) Connect MXA to coupler out directly";

            this.Invoke((MethodInvoker)(delegate
            {
                MessageBox.Show(this, Content, "WCDMA Testing", MessageBoxButtons.OK);
                Application.DoEvents();
            }));

            #endregion --- Initialize ---

            #region --- WCDMA Test ---

            for (double dblPout_Target = TestSetting.LEVEL_START; dblPout_Target <= TestSetting.LEVEL_STOP; dblPout_Target += TestSetting.LEVEL_STEP)
            {
                if (StopTest) break;

                double PoutLL = dblPout_Target - 0.05;
                double PoutUL = dblPout_Target + 0.05;

                foreach (double dblFreq in TestSetting.FREQLIST)
                {
                    if (StopTest) break;

                    double WCDMA_Pout = 0;
                    double WCDMA_Pin = 0;
                    double WCDMA_Icc = 0;
                    double dblPin = Pin_Start;
                    double[] dblACPResult = new double[5];

                    if (lastPin.ContainsKey(dblFreq)) WCDMA_Pin = dblPin = Pin_Start = lastPin[dblFreq] + TestSetting.LEVEL_STEP;
                    _E4438C.SetFrequency(dblFreq);
                    _E4438C.SetPower(Pin_Start + TestSetting.LOSS_SRC[dblFreq]);

                    //_MXA_N9020A.Config__WCDMA_CHP(dblFreq);
                    //intDelay = Math.Max(intDelay_SigGen, intDelay_MXA) * 10;
                    _PM_N1913A.Configure__CW_Power(dblFreq, 10);
                    intDelay = Math.Max(intDelay_SigGen, intDelay_PowerMeter) * 2;
                    util.Wait(intDelay);

                    //WCDMA_Pout = _MXA_N9020A.Get_WCDMA_CHP_Result();
                    //WCDMA_Pout += TestSetting.LOSS_MSR_THROUGH[dblFreq];
                    WCDMA_Pout = _PM_N1913A.GetPowerResult();
                    WCDMA_Pout += TestSetting.LOSS_MSR_POUT[dblFreq];

                    // Freqquency
                    dblResult[0] = dblFreq;

                    // Target Power
                    dblResult[1] = dblPout_Target;

                    // Pout
                    while (WCDMA_Pout <= PoutLL || WCDMA_Pout >= PoutUL)
                    {
                        dblPin = dblPin + dblPout_Target - WCDMA_Pout;
                        if (dblPin > 5)
                            break;

                        _E4438C.SetPower(dblPin + TestSetting.LOSS_SRC[dblFreq]);
                        util.Wait(intDelay);
                        //WCDMA_Pout = _MXA_N9020A.Get_WCDMA_CHP_Result();
                        //WCDMA_Pout += TestSetting.LOSS_MSR_THROUGH[dblFreq];
                        WCDMA_Pout = _PM_N1913A.GetPowerResult();
                        WCDMA_Pout += TestSetting.LOSS_MSR_POUT[dblFreq];
                        WCDMA_Pin = dblPin;
                    }
                    dblResult[2] = WCDMA_Pout;

                    // Pin
                    dblResult[3] = WCDMA_Pin;

                    if (lastPin.ContainsKey(dblFreq))
                        lastPin[dblFreq] = WCDMA_Pin;
                    else
                        lastPin.Add(dblFreq, WCDMA_Pin);
                    // Gain
                    dblResult[4] = WCDMA_Pout - WCDMA_Pin;

                    // Icc
                    WCDMA_Icc = _PS_66332A.RMS_Current();
                    dblResult[5] = WCDMA_Icc * 1000;
                    // PAE
                    dblResult[6] = 100 * (Math.Pow(10, ((WCDMA_Pout - 30) / 10))) / TestSetting.LEVEL_VCC / WCDMA_Icc;

                    // ACP
                    _MXA_N9020A.Config__WCDMA_ACP(dblFreq);
                    intDelay = Math.Max(intDelay_SigGen, intDelay_MXA);
                    util.Wait(intDelay * 3);

                    dblACPResult = _MXA_N9020A.Get_WCDMA_ACP_Result();

                    dblResult[7] = dblACPResult[1];     // -5MHz
                    dblResult[8] = dblACPResult[2];     // +5MHz
                    dblResult[9] = dblACPResult[3];     // -10MHz
                    dblResult[10] = dblACPResult[4];     // +10MHz

                    // EVM
                    if (!cbxBypassEVM.Checked)
                    {
                        _MXA_N9020A.Config__WCDMA_EVM(dblFreq);
                        intDelay = Math.Max(intDelay_SigGen, intDelay_MXA);
                        util.Wait(intDelay * 3);

                        dblResult[11] = _MXA_N9020A.Get_WCDMA_EVM_Result();
                    }
                    else
                    {
                        dblResult[11] = 0;
                    }
                    // Update_Grid
                    DataRow drNew = dtWCDMA.NewRow();
                    DataRow drNewtmp = dtWCDMATMP.NewRow();
                    drNew[0] = intTestID++;
                    for (int i = 0; i < dblResult.Count(); i++)
                    {
                        drNew[i + 1] = drNewtmp[i + 1] = Math.Round(dblResult[i], 2);
                    }
                    dtWCDMATMP.Rows.Add(drNewtmp);

                    this.Invoke((MethodInvoker)(delegate
                    {
                        dtWCDMA.Rows.Add(drNew);
                        dgvSweepResult.Refresh();
                        Application.DoEvents();
                    }));
                }
            }

            #endregion --- WCDMA Test ---

            #region --- Worst acp report ---

            foreach (double dblFreq in TestSetting.FREQLIST)
            {
                DataRow drmax = dtWCDMA.NewRow();
                drmax[0] = intTestID++;
                drmax[1] = dblFreq;

                var acp_n5 = from x in dtWCDMATMP.AsEnumerable()
                             where x.Field<double>("Frequency (MHz)") == dblFreq
                             select x.Field<double>("ACP -5MHz(dB)");

                var acp_p5 = from x in dtWCDMATMP.AsEnumerable()
                             where x.Field<double>("Frequency (MHz)") == dblFreq
                             select x.Field<double>("ACP +5MHz(dB)");

                var acp_n10 = from x in dtWCDMATMP.AsEnumerable()
                              where x.Field<double>("Frequency (MHz)") == dblFreq
                              select x.Field<double>("ACP -10MHz(dB)");

                var acp_p10 = from x in dtWCDMATMP.AsEnumerable()
                              where x.Field<double>("Frequency (MHz)") == dblFreq
                              select x.Field<double>("ACP +10MHz(dB)");

                var evm = from x in dtWCDMATMP.AsEnumerable()
                          where x.Field<double>("Frequency (MHz)") == dblFreq
                          select x.Field<double>("EVM(%)");

                try
                {
                    drmax[8] = acp_n5.ToList<double>().Max();
                    drmax[9] = acp_p5.ToList<double>().Max();
                    drmax[10] = acp_n10.ToList<double>().Max();
                    drmax[11] = acp_p10.ToList<double>().Max();
                    drmax[12] = evm.ToList<double>().Max();
                }
                catch
                {
                    drmax[8] = 0;
                    drmax[9] = 0;
                    drmax[10] = 0;
                    drmax[11] = 0;
                    drmax[12] = 0;
                }

                this.Invoke((MethodInvoker)(delegate
                {
                    dtWCDMA.Rows.Add(drmax);
                    dgvSweepResult.Refresh();
                    Application.DoEvents();
                }));

            }
            #endregion *** Worst acp report ***

            #region --- After Test ---

            AfterTest();
            _E4438C.SetOutput(Output.OFF);
            _PS_66332A.SetOutput(Output.OFF);
            //_PS_E3631A.SetOutput(Output.OFF);
            _Arb_33522A_USB.SetOutput(Output.OFF, Arb_Channel.Channel_1);
            _Arb_33522A_USB.SetOutput(Output.OFF, Arb_Channel.Channel_2);

            #endregion --- After Test ---

        }

        #endregion --- SH_1 Test ---
        #region --- SH_2 Test ---

        void CWLB_Test_SH_2()
        {
            #region --- Variable Define ---

            dicVramp.Clear();
            int intTestID = 1;
            double[] dblResult = new double[18];

            DataTable dtCWLBTMP = new DataTable();
            dtCWLBTMP = dtCWLB.Clone();

            if (!cbxKeepPrevious.Checked)
            {
                this.Invoke((MethodInvoker)(delegate
                {
                    dtCWLB.Clear();
                    dgvSweepResult.Refresh();
                }));
            }

            #endregion --- Variable Define ---

            #region --- Initialize ---
            BeforeTest();
            SetVCC(TestSetting.LEVEL_VCC);
            SetTXEnable(TestSetting.LEVEL_TXEN);
            //SetGPCTRL(TestSetting.LEVEL_GPCTRL);

            _E4438C.Initialize();
            //_PM_N1913A.Initialize(rbnDisplayON.Checked);
            _MXA_N9020A.Initialize(rbnDisplayON.Checked);
            _E4438C.SetOutput(Output.ON);

            DateTime t_Start = DateTime.Now;

            string Content = "Make sure everything is setup for TX LB testing \r\n" +
                            "  1) Connect rf source cable to TX LB \r\n" +
                            "  2) Set Control box to TX LB mode \r\n" +
                            "  3) Connect / Change LB highpass filter";

            this.Invoke((MethodInvoker)(delegate
            {
                MessageBox.Show(this, Content, "GMSK LB Testing", MessageBoxButtons.OK);
                Application.DoEvents();
                wait_2_start(t_Start);
            }));

            #endregion --- Initialize ---

            #region --- Vramp Sweep ---
            for (double dblVramp = TestSetting.LEVEL_START; dblVramp < TestSetting.LEVEL_STOP + 0.01; dblVramp += TestSetting.LEVEL_STEP)
            {
                if (StopTest) break;

                SetVramp(dblVramp);

                foreach (double dblFreq in TestSetting.FREQLIST)
                {
                    if (StopTest) break;
                    _E4438C.SetFrequency(dblFreq);
                    _E4438C.SetPower(TestSetting.LEVEL_PIN_VRAMP + TestSetting.LOSS_SRC[dblFreq]);

                    // Frequency
                    dblResult[0] = dblFreq;

                    // Vramp
                    dblResult[1] = dblVramp;

                    // Pout
                    _PM_N1913A.Configure__Average_Pulse_Power_with_Duty_Cycle(dblFreq, 25, 256);
                    util.Wait(intDelay_PowerMeter * intDelay_N1913A_Count);
                    dblResult[2] = _PM_N1913A.GetPowerResult() + TestSetting.LOSS_MSR_POUT[dblFreq];

                    // ICC
                    dblResult[3] = _PS_66332A.High_Current();
                    dblResult[3] = dblResult[3] * 1000;

                    // PAE
                    dblResult[4] = (Math.Pow(10, ((dblResult[2] - 30) / 10))) / TestSetting.LEVEL_VCC / dblResult[3] * 1000;
                    dblResult[4] = dblResult[4] * 100;

                    // harmonic 
                    for (int i = 2; i <= 6; i++)
                    {
                        _MXA_N9020A.SetFrequency(i * dblFreq);

                        util.Wait(intDelay_MXA * 2);

                        dblResult[3 + i] = _MXA_N9020A.Get_CW_PowerResult() + TestSetting.LOSS_MSR_FILTER_LB[i * dblFreq];
                    }
                    // Extra harmonic 
                    if (ext_har)
                    {
                        for (int i = 7; i <= 14; i++)
                        {
                            _MXA_N9020A.SetFrequency(i * dblFreq);
                            util.Wait(intDelay_MXA);
                            dblResult[3 + i] = _MXA_N9020A.Get_CW_PowerResult() + TestSetting.LOSS_MSR_FILTER_LB[6000];
                        }
                    }
                    // Update_Grid
                    DataRow drNew = dtCWLB.NewRow();
                    DataRow drNewTmp = dtCWLBTMP.NewRow();
                    drNewTmp[0] = drNew[0] = intTestID++;
                    for (int i = 0; i < dblResult.Count(); i++)
                    {
                        drNewTmp[i + 1] = drNew[i + 1] = Math.Round(dblResult[i], 2);
                    }
                    dtCWLBTMP.Rows.Add(drNewTmp);
                    this.Invoke((MethodInvoker)(delegate
                    {
                        dtCWLB.Rows.Add(drNew);
                        dgvSweepResult.Refresh();
                        Application.DoEvents();
                    }));
                } // Frequency loop
            }   // Vramp loop
            #endregion Vramp Sweep

            #region --- Power Servo ---
            foreach (double dblFreq in TestSetting.FREQLIST)
            {
                if (StopTest) break;

                int Count = 0;
                _E4438C.SetFrequency(dblFreq);
                _E4438C.SetPower(TestSetting.LEVEL_PIN_VRAMP + TestSetting.LOSS_SRC[dblFreq]);

                #region Search power
                double LoopResult_Low;
                double LoopResult_High;
                double LoopResult;
                double Slope_mV;

                this.SetVramp(1.2);
                _PM_N1913A.Configure__Average_Pulse_Power_with_Duty_Cycle(dblFreq, 25, 256);
                util.Wait(intDelay_PowerMeter * intDelay_N1913A_Count);
                LoopResult_Low = _PM_N1913A.GetPowerResult();

                this.SetVramp(1.6);
                _PM_N1913A.Configure__Average_Pulse_Power_with_Duty_Cycle(dblFreq, 25, 256);
                util.Wait(intDelay_PowerMeter * intDelay_N1913A_Count);
                LoopResult_High = _PM_N1913A.GetPowerResult();

                Slope_mV = (LoopResult_High - LoopResult_Low) / 200;

                LoopResult = LoopResult_Low + TestSetting.LOSS_MSR_POUT[dblFreq];
                dblRatedVramp = 1.2 + (TestSetting.TARGET_POUT_CWLB - LoopResult) / Slope_mV / 1000;

                if (dblRatedVramp < 0.8 || dblRatedVramp > 1.8) dblRatedVramp = 1.8;

                while (Math.Abs(LoopResult - TestSetting.TARGET_POUT_CWLB) > 0.05 && dblRatedVramp < 1.8 && dblRatedVramp > 0.8)
                {
                    this.SetVramp(dblRatedVramp);
                    util.Wait(intDelay_PowerMeter * intDelay_N1913A_Count);
                    LoopResult = _PM_N1913A.GetPowerResult() + TestSetting.LOSS_MSR_POUT[dblFreq];
                    dblRatedVramp = dblRatedVramp + (TestSetting.TARGET_POUT_CWLB - LoopResult) / Slope_mV / 1000;
                    Count++;
                    if (Count > 20) break;
                }
                #endregion Search power

                // Frequency
                dblResult[0] = dblFreq;
                // Rated Vramp
                dblResult[1] = dblRatedVramp;
                if (!dicVramp.ContainsKey(dblFreq)) dicVramp.Add(dblFreq, Math.Round(dblRatedVramp, 2));
                //Rated Pout
                dblResult[2] = LoopResult;

                // Rated ICC
                dblResult[3] = _PS_66332A.High_Current();
                dblResult[3] = dblResult[3] * 1000;

                // Rated PAE
                dblResult[4] = (Math.Pow(10, ((dblResult[2] - 30) / 10))) / TestSetting.LEVEL_VCC / dblResult[3] * 1000;
                dblResult[4] = dblResult[4] * 100;

                // Rated harmonic 
                for (int i = 2; i <= 6; i++)
                {
                    _MXA_N9020A.SetFrequency(i * dblFreq);
                    util.Wait(intDelay_MXA);
                    dblResult[3 + i] = _MXA_N9020A.Get_CW_PowerResult() + TestSetting.LOSS_MSR_FILTER_LB[i * dblFreq];
                }
                // Extra harmonic 
                if (ext_har)
                {
                    for (int i = 7; i <= 14; i++)
                    {
                        _MXA_N9020A.SetFrequency(i * dblFreq);
                        util.Wait(intDelay_MXA);
                        dblResult[3 + i] = _MXA_N9020A.Get_CW_PowerResult() + TestSetting.LOSS_MSR_FILTER_LB[6000];
                    }
                }
                // Update_Grid
                DataRow drNew = dtCWLB.NewRow();
                DataRow drNewTmp = dtCWLBTMP.NewRow();
                drNewTmp[0] = drNew[0] = intTestID++;
                for (int i = 0; i < dblResult.Count(); i++)
                {
                    drNewTmp[i + 1] = drNew[i + 1] = Math.Round(dblResult[i], 2);
                }
                dtCWLBTMP.Rows.Add(drNewTmp);
                this.Invoke((MethodInvoker)(delegate
                {
                    dtCWLB.Rows.Add(drNew);
                    dgvSweepResult.Refresh();
                    Application.DoEvents();
                }));

            } // Frequency loop for power servo
            #endregion Power Servo

            #region --- Worst harmonic report ---

            foreach (double dblFreq in TestSetting.FREQLIST)
            {
                if (StopTest) break;

                DataRow drmax = dtCWLB.NewRow();
                drmax[0] = intTestID++;
                drmax[1] = dblFreq;

                var _2fo = from x in dtCWLBTMP.AsEnumerable()
                           where x.Field<double>("Frequency(MHz)") == dblFreq && double.Parse(x.Field<string>("Vramp(V)")) <= dicVramp[dblFreq]
                           select x.Field<double>("2fo(dBm)");

                var _3fo = from x in dtCWLBTMP.AsEnumerable()
                           where x.Field<double>("Frequency(MHz)") == dblFreq && double.Parse(x.Field<string>("Vramp(V)")) <= dicVramp[dblFreq]
                           select x.Field<double>("3fo(dBm)");

                var _4fo = from x in dtCWLBTMP.AsEnumerable()
                           where x.Field<double>("Frequency(MHz)") == dblFreq && double.Parse(x.Field<string>("Vramp(V)")) <= dicVramp[dblFreq]
                           select x.Field<double>("4fo(dBm)");

                var _5fo = from x in dtCWLBTMP.AsEnumerable()
                           where x.Field<double>("Frequency(MHz)") == dblFreq && double.Parse(x.Field<string>("Vramp(V)")) <= dicVramp[dblFreq]
                           select x.Field<double>("5fo(dBm)");

                var _6fo = from x in dtCWLBTMP.AsEnumerable()
                           where x.Field<double>("Frequency(MHz)") == dblFreq && double.Parse(x.Field<string>("Vramp(V)")) <= dicVramp[dblFreq]
                           select x.Field<double>("6fo(dBm)");

                try
                {
                    drmax[6] = _2fo.ToList<double>().Max();
                    drmax[7] = _3fo.ToList<double>().Max();
                    drmax[8] = _4fo.ToList<double>().Max();
                    drmax[9] = _5fo.ToList<double>().Max();
                    drmax[10] = _6fo.ToList<double>().Max();
                }
                catch
                {
                    drmax[6] = 0;
                    drmax[7] = 0;
                    drmax[8] = 0;
                    drmax[9] = 0;
                    drmax[10] = 0;
                }

                this.Invoke((MethodInvoker)(delegate
                {
                    dtCWLB.Rows.Add(drmax);
                    dgvSweepResult.Refresh();
                    Application.DoEvents();
                }));

            }
            #endregion *** Worst harmonic report ***

            #region --- After Test ---
            AfterTest();
            // After Test
            _E4438C.SetOutput(Output.OFF);
            _PS_66332A.SetOutput(Output.OFF);
            _Arb_33522A.SetOutput(Output.OFF, Arb_Channel.Channel_1);
            _Arb_33522A.SetOutput(Output.OFF, Arb_Channel.Channel_2);


            #endregion --- After Test ---


        } 

        void CWHB_Test_SH_2()
        {
            #region --- Variable Define ---
            dicVramp.Clear();
            int intTestID = 1;
            double[] dblResult = new double[11];

            DataTable dtCWHBTMP = new DataTable();
            dtCWHBTMP = dtCWHB.Clone();

            if (!cbxKeepPrevious.Checked)
            {
                this.Invoke((MethodInvoker)(delegate
                {
                    dtCWHB.Clear();
                    dgvSweepResult.Refresh();
                }));
            }
            #endregion --- Variable Define ---

            #region --- Initialize ---
            BeforeTest();

            SetVCC(TestSetting.LEVEL_VCC);
            SetTXEnable(TestSetting.LEVEL_TXEN);
            //SetGPCTRL(TestSetting.LEVEL_GPCTRL);
            _E4438C.Initialize();
            //_PM_N1913A.Initialize(rbnDisplayON.Checked);
            _MXA_N9020A.Initialize(rbnDisplayON.Checked);
            _E4438C.SetOutput(Output.ON);

            DateTime t_Start = DateTime.Now;
            
            string Content = "Make sure everything is setup for TX HB testing \r\n" +
                            "  1) Connect rf source cable to TX HB \r\n" +
                            "  2) Set Control box to TX HB mode \r\n" +
                            "  3) Connect / Change HB highpass filter";

            this.Invoke((MethodInvoker)(delegate
            {
                MessageBox.Show(this, Content, "GMSK HB Testing", MessageBoxButtons.OK);
                Application.DoEvents();
                wait_2_start(t_Start);
            }));

            #endregion --- Initialize ---

            #region --- Vramp Sweep ---
            for (double dblVramp = TestSetting.LEVEL_START; dblVramp < TestSetting.LEVEL_STOP + 0.01; dblVramp += TestSetting.LEVEL_STEP)
            {
                if (StopTest) break;
                SetVramp(dblVramp);

                foreach (double dblFreq in TestSetting.FREQLIST)
                {
                    if (StopTest) break;
                    _E4438C.SetFrequency(dblFreq);
                    _E4438C.SetPower(TestSetting.LEVEL_PIN_VRAMP + TestSetting.LOSS_SRC[dblFreq]);

                    // Frequency
                    dblResult[0] = dblFreq;

                    // Vramp
                    dblResult[1] = dblVramp;

                    // Pout
                    _PM_N1913A.Configure__Average_Pulse_Power_with_Duty_Cycle(dblFreq, 25, 256);

                    util.Wait(intDelay_PowerMeter * intDelay_N1913A_Count);
                    dblResult[2] = _PM_N1913A.GetPowerResult() + TestSetting.LOSS_MSR_POUT[dblFreq];

                    // ICC
                    dblResult[3] = _PS_66332A.High_Current();
                    dblResult[3] = dblResult[3] * 1000;

                    // PAE
                    dblResult[4] = (Math.Pow(10, ((dblResult[2] - 30) / 10))) / TestSetting.LEVEL_VCC / dblResult[3] * 1000;
                    dblResult[4] = dblResult[4] * 100;

                    // harmonic 
                    for (int i = 2; i <= 3; i++)
                    {
                        _MXA_N9020A.SetFrequency(i * dblFreq);

                        util.Wait(intDelay_MXA);

                        dblResult[3 + i] = _MXA_N9020A.Get_CW_PowerResult() + TestSetting.LOSS_MSR_FILTER_HB[i * dblFreq];
                    }
                    // Extra harmonic 
                    if (ext_har)
                    {
                        for (int i = 4; i <= 7; i++)
                        {
                            _MXA_N9020A.SetFrequency(i * dblFreq);
                            util.Wait(intDelay_MXA);
                            dblResult[3 + i] = _MXA_N9020A.Get_CW_PowerResult() + TestSetting.LOSS_MSR_FILTER_HB[6000];
                        }
                    }
                    // Update_Grid
                    DataRow drNew = dtCWHB.NewRow();
                    DataRow drNewTmp = dtCWHBTMP.NewRow();
                    drNewTmp[0] = drNew[0] = intTestID++;
                    for (int i = 0; i < dblResult.Count(); i++)
                    {
                        drNewTmp[i + 1] = drNew[i + 1] = Math.Round(dblResult[i], 2);
                    }
                    dtCWHBTMP.Rows.Add(drNewTmp);
                    this.Invoke((MethodInvoker)(delegate
                    {
                        dtCWHB.Rows.Add(drNew);
                        dgvSweepResult.Refresh();
                        Application.DoEvents();
                    }));

                }   // Frequency loop
            }  // Vramp loop
            #endregion *** Vramp Sweep ***

            #region --- Power Servo ---
            foreach (double dblFreq in TestSetting.FREQLIST)
            {
                if (StopTest) break;
                int Count = 0;
                _E4438C.SetFrequency(dblFreq);
                _E4438C.SetPower(TestSetting.LEVEL_PIN_VRAMP + TestSetting.LOSS_SRC[dblFreq]);

                #region Search power
                double LoopResult_Low;
                double LoopResult_High;
                double LoopResult;
                double Slope_mV;

                this.SetVramp(1.2);
                _PM_N1913A.Configure__Average_Pulse_Power_with_Duty_Cycle(dblFreq, 25, 256);
                util.Wait(intDelay_PowerMeter * intDelay_N1913A_Count);
                LoopResult_Low = _PM_N1913A.GetPowerResult();

                this.SetVramp(1.6);
                _PM_N1913A.Configure__Average_Pulse_Power_with_Duty_Cycle(dblFreq, 25, 256);
                util.Wait(intDelay_PowerMeter * intDelay_N1913A_Count);
                LoopResult_High = _PM_N1913A.GetPowerResult();

                Slope_mV = (LoopResult_High - LoopResult_Low) / 200;

                LoopResult = LoopResult_Low + TestSetting.LOSS_MSR_POUT[dblFreq];
                dblRatedVramp = 1.2 + (TestSetting.TARGET_POUT_CWHB - LoopResult) / Slope_mV / 1000;

                if (dblRatedVramp < 0.8 || dblRatedVramp > 1.8) dblRatedVramp = 1.8;

                while (Math.Abs(LoopResult - TestSetting.TARGET_POUT_CWHB) > 0.05 && dblRatedVramp < 1.8 && dblRatedVramp > 0.8)
                {
                    this.SetVramp(dblRatedVramp);
                    util.Wait(intDelay_PowerMeter * intDelay_N1913A_Count);
                    LoopResult = _PM_N1913A.GetPowerResult() + TestSetting.LOSS_MSR_POUT[dblFreq];
                    dblRatedVramp = dblRatedVramp + (TestSetting.TARGET_POUT_CWHB - LoopResult) / Slope_mV / 1000;
                    Count++;
                    if (Count > 20) break;
                }
                #endregion Search power

                // Frequency
                dblResult[0] = dblFreq;
                // Rated Vramp
                dblResult[1] = dblRatedVramp;
                if (!dicVramp.ContainsKey(dblFreq)) dicVramp.Add(dblFreq, Math.Round(dblRatedVramp, 2));
                //Rated Pout
                dblResult[2] = LoopResult;

                // Rated ICC
                dblResult[3] = _PS_66332A.High_Current();
                dblResult[3] = dblResult[3] * 1000;

                // Rated PAE
                dblResult[4] = (Math.Pow(10, ((dblResult[2] - 30) / 10))) / TestSetting.LEVEL_VCC / dblResult[3] * 1000;
                dblResult[4] = dblResult[4] * 100;

                // Rated harmonic 
                for (int i = 2; i <= 3; i++)
                {
                    _MXA_N9020A.SetFrequency(i * dblFreq);
                    util.Wait(intDelay_MXA);
                    dblResult[3 + i] = _MXA_N9020A.Get_CW_PowerResult() + TestSetting.LOSS_MSR_FILTER_HB[i * dblFreq];
                }
                // Extra harmonic 
                if (ext_har)
                {
                    for (int i = 4; i <= 7; i++)
                    {
                        _MXA_N9020A.SetFrequency(i * dblFreq);
                        util.Wait(intDelay_MXA);
                        dblResult[3 + i] = _MXA_N9020A.Get_CW_PowerResult() + TestSetting.LOSS_MSR_FILTER_HB[6000];
                    }
                }
                // Update_Grid
                DataRow drNew = dtCWHB.NewRow();
                DataRow drNewTmp = dtCWHBTMP.NewRow();
                drNewTmp[0] = drNew[0] = intTestID++;
                for (int i = 0; i < dblResult.Count(); i++)
                {
                    drNewTmp[i + 1] = drNew[i + 1] = Math.Round(dblResult[i], 2);
                }
                dtCWHBTMP.Rows.Add(drNewTmp);
                this.Invoke((MethodInvoker)(delegate
                {
                    dtCWHB.Rows.Add(drNew);
                    dgvSweepResult.Refresh();
                    Application.DoEvents();
                }));

            } // Frequency loop for power servo
            #endregion Power Servo

            #region --- Worst harmonic report ---

            foreach (double dblFreq in TestSetting.FREQLIST)
            {
                if (StopTest) break;
                DataRow drmax = dtCWHB.NewRow();
                drmax[0] = intTestID++;
                drmax[1] = dblFreq;

                var _2fo = from x in dtCWHBTMP.AsEnumerable()
                           where x.Field<double>("Frequency(MHz)") == dblFreq && double.Parse(x.Field<string>("Vramp(V)")) <= dicVramp[dblFreq]
                           select x.Field<double>("2fo(dBm)");

                var _3fo = from x in dtCWHBTMP.AsEnumerable()
                           where x.Field<double>("Frequency(MHz)") == dblFreq && double.Parse(x.Field<string>("Vramp(V)")) <= dicVramp[dblFreq]
                           select x.Field<double>("3fo(dBm)");
                try
                {
                    drmax[6] = _2fo.ToList<double>().Max();
                    drmax[7] = _3fo.ToList<double>().Max();
                }
                catch
                {
                    drmax[6] = 0;
                    drmax[7] = 0;
                }

                this.Invoke((MethodInvoker)(delegate
                {
                    dtCWHB.Rows.Add(drmax);
                    dgvSweepResult.Refresh();
                    Application.DoEvents();
                }));
            }
            #endregion *** Worst harmonic report ***

            #region --- After Test ---
            // After Test
            AfterTest();
            _E4438C.SetOutput(Output.OFF);
            _PS_66332A.SetOutput(Output.OFF);
            _Arb_33522A.SetOutput(Output.OFF, Arb_Channel.Channel_1);
            _Arb_33522A.SetOutput(Output.OFF, Arb_Channel.Channel_2);

            #endregion --- After Test ---

        } 

        void EDGELB_TEST_SH_2()
        {
            #region --- Variable Define ---

            Dictionary<double, double> lastEDGELBPin = new Dictionary<double, double>();
            int intDelay;
            int intTestID = 1;
            double Pin_Start = -10;
            double[] dblResult = new double[10];

            DataTable dtEDGELBTMP = new DataTable();
            dtEDGELBTMP = dtEDGELB.Clone();

            if (!cbxKeepPrevious.Checked)
            {
                this.Invoke((MethodInvoker)(delegate
                {
                    dtEDGELB.Clear();
                    dgvSweepResult.Refresh();
                }));
            }

            #endregion --- Variable Define ---

            #region --- Initialize ---

            BeforeTest();

            SetVCC(TestSetting.LEVEL_VCC);
            SetTXEnable(TestSetting.LEVEL_TXEN);
            SetVramp(TestSetting.LEVEL_PIN_VRAMP);
            //SetGPCTRL(TestSetting.LEVEL_GPCTRL);

            _E4438C.Mode_Initialize(Modulation.EDGE);
            _MXA_N9020A.Mod_Initialize(Modulation.EDGE);

            _E4438C.SetModOutput(Output.ON);
            _E4438C.SetOutput(Output.ON);
            
            DateTime t_Start = DateTime.Now;

            string Content = "Make sure everything is setup for EDGELB testing \r\n" +
                              "  1) Connect rf source cable to EDGELB \r\n" +
                              "  2) Set Control box to EDGELB mode \r\n" +
                              "  3) Connect MXA to coupler out directly";

            this.Invoke((MethodInvoker)(delegate
            {
                MessageBox.Show(this, Content, "EDGELB Testing", MessageBoxButtons.OK);
                Application.DoEvents();
                wait_2_start(t_Start);
            }));
            util.Wait(3000);

            #endregion --- Initialize ---

            #region --- EDGELB Test ---

            for (double dblPout_Target = TestSetting.LEVEL_START; dblPout_Target <= TestSetting.LEVEL_STOP; dblPout_Target += TestSetting.LEVEL_STEP)
            {
                if (StopTest) break;

                double PoutLL = dblPout_Target - 0.05;
                double PoutUL = dblPout_Target + 0.05;

                foreach (double dblFreq in TestSetting.FREQLIST)
                {
                    if (StopTest) break;

                    int Count = 0;
                    double EDGELB_Pout = 0;
                    double EDGELB_Pin = 0;
                    double EDGELB_Icc = 0;
                    double dblPin = Pin_Start;
                    double[] dblACPResult = new double[10];

                    if (lastEDGELBPin.ContainsKey(dblFreq)) EDGELB_Pin = dblPin = Pin_Start = lastEDGELBPin[dblFreq] + TestSetting.LEVEL_STEP;
                    _E4438C.SetFrequency(dblFreq);
                    _E4438C.SetPower(Pin_Start + TestSetting.LOSS_SRC[dblFreq]);

                    _MXA_N9020A.Config__EDGE_Burst_Power(dblFreq);
                    intDelay = Math.Max(intDelay_SigGen, intDelay_MXA) * 5;
                    util.Wait(intDelay);

                    EDGELB_Pout = _MXA_N9020A.Get_EDGE_Burst_Power_Result();
                    EDGELB_Pout += TestSetting.LOSS_MSR_THROUGH[dblFreq];

                    // Freqquency
                    dblResult[0] = dblFreq;

                    // Target Power
                    dblResult[1] = dblPout_Target;

                    //Pout
                    while (EDGELB_Pout <= PoutLL || EDGELB_Pout >= PoutUL)
                    {
                        dblPin = dblPin + dblPout_Target - EDGELB_Pout + 0.02;
                        if (dblPin > 5)
                            break;

                        _E4438C.SetPower(dblPin + TestSetting.LOSS_SRC[dblFreq]);
                        util.Wait(intDelay);
                        EDGELB_Pout = _MXA_N9020A.Get_EDGE_Burst_Power_Result();
                        EDGELB_Pout += TestSetting.LOSS_MSR_THROUGH[dblFreq];
                        EDGELB_Pin = dblPin;
                        Count++;
                        //if (Count > 20) break;
                    }
                    dblResult[2] = EDGELB_Pout;

                    // Pin
                    dblResult[3] = EDGELB_Pin;
                    if (lastEDGELBPin.ContainsKey(dblFreq))
                        lastEDGELBPin[dblFreq] = EDGELB_Pin;
                    else
                        lastEDGELBPin.Add(dblFreq, EDGELB_Pin);

                    // Gain
                    dblResult[4] = EDGELB_Pout - EDGELB_Pin;

                    // Icc
                    EDGELB_Icc = _PS_66332A.High_Current();
                    dblResult[5] = EDGELB_Icc * 1000;
                    // PAE
                    dblResult[6] = 100 * (Math.Pow(10, ((EDGELB_Pout - 30) / 10))) / TestSetting.LEVEL_VCC / EDGELB_Icc;

                    //ACP
                    _MXA_N9020A.Config__EDGE_ACP(dblFreq);
                    intDelay = Math.Max(intDelay_SigGen, intDelay_MXA);
                    util.Wait(intDelay * 3);

                    dblACPResult = _MXA_N9020A.Get_EDGE_ACP_Result();

                    dblResult[7] = dblACPResult[6];     // -400kHz
                    dblResult[8] = dblACPResult[7];     // +400kHz

                    // EVM
                    if (!cbxBypassEVM.Checked)
                    {
                        _MXA_N9020A.Config__EDGE_EVM(dblFreq);
                        intDelay = Math.Max(intDelay_SigGen, intDelay_MXA);
                        util.Wait(intDelay * 3);

                        dblResult[9] = _MXA_N9020A.Get_EDGE_EVM_Result();
                    }
                    else
                    {
                        dblResult[9] = 0;
                    }
                    // Update_Grid
                    DataRow drNew = dtEDGELB.NewRow();
                    DataRow drNewtmp = dtEDGELBTMP.NewRow();
                    drNewtmp[0] = drNew[0] = intTestID++;
                    for (int i = 0; i < dblResult.Count(); i++)
                    {
                        drNew[i + 1] = drNewtmp[i + 1] = Math.Round(dblResult[i], 2);
                    }
                    dtEDGELBTMP.Rows.Add(drNewtmp);

                    this.Invoke((MethodInvoker)(delegate
                    {
                        dtEDGELB.Rows.Add(drNew);
                        dgvSweepResult.Refresh();
                        Application.DoEvents();
                    }));
                }
            }

            #endregion --- EDGELB Test ---

            #region --- Worst acp report ---

            foreach (double dblFreq in TestSetting.FREQLIST)
            {
                DataRow drmax = dtEDGELB.NewRow();
                drmax[0] = intTestID++;
                drmax[1] = dblFreq;

                var acp_n400 = from x in dtEDGELBTMP.AsEnumerable()
                               where x.Field<double>("Frequency (MHz)") == dblFreq
                               select x.Field<double>("ACP -400kHz(dB)");

                var acp_p400 = from x in dtEDGELBTMP.AsEnumerable()
                               where x.Field<double>("Frequency (MHz)") == dblFreq
                               select x.Field<double>("ACP +400kHz(dB)");

                var evm = from x in dtEDGELBTMP.AsEnumerable()
                          where x.Field<double>("Frequency (MHz)") == dblFreq
                          select x.Field<double>("EVM(%)");

                try
                {
                    drmax[8] = acp_n400.ToList<double>().Max();
                    drmax[9] = acp_p400.ToList<double>().Max();
                    drmax[10] = evm.ToList<double>().Max();
                }
                catch
                {
                    drmax[8] = 0;
                    drmax[9] = 0;
                    drmax[10] = 0;
                }

                this.Invoke((MethodInvoker)(delegate
                {
                    dtEDGELB.Rows.Add(drmax);
                    dgvSweepResult.Refresh();
                    Application.DoEvents();
                }));

            }
            #endregion *** Worst acp report ***

            #region --- After Test ---

            AfterTest();
            _E4438C.SetOutput(Output.OFF);
            _PS_66332A.SetOutput(Output.OFF);
            //_PS_E3631A.SetOutput(Output.OFF);
            _Arb_33522A.SetOutput(Output.OFF, Arb_Channel.Channel_1);
            _Arb_33522A.SetOutput(Output.OFF, Arb_Channel.Channel_2);

            #endregion --- After Test ---
        }

        void EDGEHB_TEST_SH_2()
        {
            #region --- Variable Define ---

            Dictionary<double, double> lastEDGEHBPin = new Dictionary<double, double>();
            int intDelay;
            int intTestID = 1;
            double Pin_Start = -10;
            double[] dblResult = new double[10];

            DataTable dtEDGEHBTMP = new DataTable();
            dtEDGEHBTMP = dtEDGEHB.Clone();

            if (!cbxKeepPrevious.Checked)
            {
                this.Invoke((MethodInvoker)(delegate
                {
                    dtEDGEHB.Clear();
                    dgvSweepResult.Refresh();
                }));
            }

            #endregion --- Variable Define ---

            #region --- Initialize ---

            BeforeTest();

            SetVCC(TestSetting.LEVEL_VCC);
            SetTXEnable(TestSetting.LEVEL_TXEN);
            SetVramp(TestSetting.LEVEL_PIN_VRAMP);
            //SetGPCTRL(TestSetting.LEVEL_GPCTRL);

            _E4438C.Mode_Initialize(Modulation.EDGE);
            _MXA_N9020A.Mod_Initialize(Modulation.EDGE);

            _E4438C.SetModOutput(Output.ON);
            _E4438C.SetOutput(Output.ON);
            //util.Wait(5000);


            DateTime t_Start = DateTime.Now;

            string Content = "Make sure everything is setup for EDGEHB testing \r\n" +
                              "  1) Connect rf source cable to EDGEHB \r\n" +
                              "  2) Set Control box to EDGEHB mode \r\n" +
                              "  3) Connect MXA to coupler out directly";

            this.Invoke((MethodInvoker)(delegate
            {
                MessageBox.Show(this, Content, "EDGEHB Testing", MessageBoxButtons.OK);
                Application.DoEvents();
                wait_2_start(t_Start);
            }));
            //util.Wait(3000);

            #endregion --- Initialize ---

            #region --- EDGEHB Test ---

            for (double dblPout_Target = TestSetting.LEVEL_START; dblPout_Target <= TestSetting.LEVEL_STOP; dblPout_Target += TestSetting.LEVEL_STEP)
            {
                if (StopTest) break;

                double PoutLL = dblPout_Target - 0.05;
                double PoutUL = dblPout_Target + 0.05;

                foreach (double dblFreq in TestSetting.FREQLIST)
                {
                    if (StopTest) break;

                    int Count = 0;
                    double EDGEHB_Pout = 0;
                    double EDGEHB_Pin = 0;
                    double EDGEHB_Icc = 0;
                    double dblPin = Pin_Start;
                    double[] dblACPResult = new double[10];

                    if (lastEDGEHBPin.ContainsKey(dblFreq)) EDGEHB_Pin = dblPin = Pin_Start = lastEDGEHBPin[dblFreq] + TestSetting.LEVEL_STEP;
                    _E4438C.SetFrequency(dblFreq);
                    _E4438C.SetPower(Pin_Start + TestSetting.LOSS_SRC[dblFreq]);

                    _MXA_N9020A.Config__EDGE_Burst_Power(dblFreq);
                    intDelay = Math.Max(intDelay_SigGen, intDelay_MXA) * 5;
                    util.Wait(intDelay);

                    EDGEHB_Pout = _MXA_N9020A.Get_EDGE_Burst_Power_Result();
                    EDGEHB_Pout += TestSetting.LOSS_MSR_THROUGH[dblFreq];

                    // Freqquency
                    dblResult[0] = dblFreq;

                    // Target Power
                    dblResult[1] = dblPout_Target;

                    //Pout
                    while (EDGEHB_Pout <= PoutLL || EDGEHB_Pout >= PoutUL)
                    {
                        dblPin = dblPin + dblPout_Target - EDGEHB_Pout + 0.02;
                        if (dblPin > 5)
                            break;

                        _E4438C.SetPower(dblPin + TestSetting.LOSS_SRC[dblFreq]);
                        util.Wait(intDelay);
                        EDGEHB_Pout = _MXA_N9020A.Get_EDGE_Burst_Power_Result();
                        EDGEHB_Pout += TestSetting.LOSS_MSR_THROUGH[dblFreq];
                        EDGEHB_Pin = dblPin;
                        Count++;
                        if (Count > 20) break;
                    }
                    dblResult[2] = EDGEHB_Pout;

                    // Pin
                    dblResult[3] = EDGEHB_Pin;
                    if (lastEDGEHBPin.ContainsKey(dblFreq))
                        lastEDGEHBPin[dblFreq] = EDGEHB_Pin;
                    else
                        lastEDGEHBPin.Add(dblFreq, EDGEHB_Pin);

                    // Gain
                    dblResult[4] = EDGEHB_Pout - EDGEHB_Pin;

                    // Icc
                    EDGEHB_Icc = _PS_66332A.High_Current();
                    dblResult[5] = EDGEHB_Icc * 1000;
                    // PAE
                    dblResult[6] = 100 * (Math.Pow(10, ((EDGEHB_Pout - 30) / 10))) / TestSetting.LEVEL_VCC / EDGEHB_Icc;

                    //ACP
                    _MXA_N9020A.Config__EDGE_ACP(dblFreq);
                    intDelay = Math.Max(intDelay_SigGen, intDelay_MXA);
                    util.Wait(intDelay * 3);

                    dblACPResult = _MXA_N9020A.Get_EDGE_ACP_Result();

                    dblResult[7] = dblACPResult[6];     // -400kHz
                    dblResult[8] = dblACPResult[7];     // +400kHz

                    // EVM
                    if (!cbxBypassEVM.Checked)
                    {
                        _MXA_N9020A.Config__EDGE_EVM(dblFreq);
                        intDelay = Math.Max(intDelay_SigGen, intDelay_MXA);
                        util.Wait(intDelay * 3);

                        dblResult[9] = _MXA_N9020A.Get_EDGE_EVM_Result();
                    }
                    else
                    {
                        dblResult[9] = 0;
                    }
                    // Update_Grid
                    DataRow drNew = dtEDGEHB.NewRow();
                    DataRow drNewtmp = dtEDGEHBTMP.NewRow();
                    drNewtmp[0] = drNew[0] = intTestID++;
                    for (int i = 0; i < dblResult.Count(); i++)
                    {
                        drNew[i + 1] = drNewtmp[i + 1] = Math.Round(dblResult[i], 2);
                    }
                    dtEDGEHBTMP.Rows.Add(drNewtmp);

                    this.Invoke((MethodInvoker)(delegate
                    {
                        dtEDGEHB.Rows.Add(drNew);
                        dgvSweepResult.Refresh();
                        Application.DoEvents();
                    }));
                }
            }

            #endregion --- EDGEHB Test ---

            #region --- Worst acp report ---

            foreach (double dblFreq in TestSetting.FREQLIST)
            {
                DataRow drmax = dtEDGEHB.NewRow();
                drmax[0] = intTestID++;
                drmax[1] = dblFreq;

                var acp_n400 = from x in dtEDGEHBTMP.AsEnumerable()
                               where x.Field<double>("Frequency (MHz)") == dblFreq
                               select x.Field<double>("ACP -400kHz(dB)");

                var acp_p400 = from x in dtEDGEHBTMP.AsEnumerable()
                               where x.Field<double>("Frequency (MHz)") == dblFreq
                               select x.Field<double>("ACP +400kHz(dB)");

                var evm = from x in dtEDGEHBTMP.AsEnumerable()
                          where x.Field<double>("Frequency (MHz)") == dblFreq
                          select x.Field<double>("EVM(%)");

                try
                {
                    drmax[8] = acp_n400.ToList<double>().Max();
                    drmax[9] = acp_p400.ToList<double>().Max();
                    drmax[10] = evm.ToList<double>().Max();
                }
                catch
                {
                    drmax[8] = 0;
                    drmax[9] = 0;
                    drmax[10] = 0;
                }

                this.Invoke((MethodInvoker)(delegate
                {
                    dtEDGEHB.Rows.Add(drmax);
                    dgvSweepResult.Refresh();
                    Application.DoEvents();
                }));

            }
            #endregion *** Worst acp report ***

            #region --- After Test ---

            AfterTest();
            _E4438C.SetOutput(Output.OFF);
            _PS_66332A.SetOutput(Output.OFF);
            //_PS_E3631A.SetOutput(Output.OFF);
            _Arb_33522A.SetOutput(Output.OFF, Arb_Channel.Channel_1);
            _Arb_33522A.SetOutput(Output.OFF, Arb_Channel.Channel_2);

            #endregion --- After Test ---
        }

        void TDSCDMA_TEST_SH_2()
        {
            #region --- Variable Define ---

            Dictionary<double, double> lastTDSCDMAPin = new Dictionary<double, double>();
            int intDelay;
            int intTestID = 1;
            double Pin_Start = -10;
            double[] dblResult = new double[12];

            DataTable dtTDSCDMATMP = new DataTable();
            dtTDSCDMATMP = dtTDSCDMA.Clone();

            if (!cbxKeepPrevious.Checked)
            {
                this.Invoke((MethodInvoker)(delegate
                {
                    dtTDSCDMA.Clear();
                    dgvSweepResult.Refresh();
                }));
            }

            #endregion --- Variable Define ---

            #region --- Initialize ---

            BeforeTest();
            //_PS_E3631A = new PS_E3631A(Instruments_address._06);
            //_PS_E3631A.Initialize();

            SetVCC(TestSetting.LEVEL_VCC);
            //SetVbat(TestSetting.LEVEL_VBAT, 1);
            SetTXEnable(TestSetting.LEVEL_TXEN);
            SetVramp(TestSetting.LEVEL_PIN_VRAMP);
            //SetGPCTRL(TestSetting.LEVEL_GPCTRL);

            _E4438C.Mode_Initialize(Modulation.TDSCDMA, 4.54);
            _MXA_N9020A.Mod_Initialize(Modulation.TDSCDMA);

            _E4438C.SetModOutput(Output.ON);
            _E4438C.SetOutput(Output.ON);

            string Content = "Make sure everything is setup for TDSCDMA testing \r\n" +
                              "  1) Connect rf source cable to TDSCDMA \r\n" +
                              "  2) Set Control box to TDSCDMA mode \r\n" +
                              "  3) Connect MXA to coupler out directly";

            this.Invoke((MethodInvoker)(delegate
            {
                MessageBox.Show(this, Content, "TDSCDMA Testing", MessageBoxButtons.OK);
                Application.DoEvents();
            }));

            #endregion --- Initialize ---

            #region --- TDSCDMA Test ---

            for (double dblPout_Target = TestSetting.LEVEL_START; dblPout_Target <= TestSetting.LEVEL_STOP; dblPout_Target += TestSetting.LEVEL_STEP)
            {
                if (StopTest) break;

                double PoutLL = dblPout_Target - 0.05;
                double PoutUL = dblPout_Target + 0.05;

                foreach (double dblFreq in TestSetting.FREQLIST)
                {
                    if (StopTest) break;

                    double TDSCDMA_Pout = 0;
                    double TDSCDMA_Pin = 0;
                    double TDSCDMA_Icc = 0;
                    double dblPin = Pin_Start;
                    double[] dblACPResult = new double[5];

                    if (lastTDSCDMAPin.ContainsKey(dblFreq)) TDSCDMA_Pin = dblPin = Pin_Start = lastTDSCDMAPin[dblFreq] + TestSetting.LEVEL_STEP;
                    _E4438C.SetFrequency(dblFreq);
                    _E4438C.SetPower(Pin_Start + TestSetting.LOSS_SRC[dblFreq]);

                    _MXA_N9020A.Config__TDSCDMA_CHP(dblFreq);
                    intDelay = Math.Max(intDelay_SigGen, intDelay_MXA) * 10;
                    util.Wait(intDelay);

                    TDSCDMA_Pout = _MXA_N9020A.Get_TDSCDMA_CHP_Result();
                    TDSCDMA_Pout += TestSetting.LOSS_MSR_THROUGH[dblFreq];

                    // Freqquency
                    dblResult[0] = dblFreq;

                    // Target Power
                    dblResult[1] = dblPout_Target;

                    //Pout
                    while (TDSCDMA_Pout <= PoutLL || TDSCDMA_Pout >= PoutUL)
                    {
                        dblPin = dblPin + dblPout_Target - TDSCDMA_Pout;
                        if (dblPin > 5)
                            break;

                        _E4438C.SetPower(dblPin + TestSetting.LOSS_SRC[dblFreq]);
                        util.Wait(intDelay);
                        TDSCDMA_Pout = _MXA_N9020A.Get_TDSCDMA_CHP_Result();
                        TDSCDMA_Pout += TestSetting.LOSS_MSR_THROUGH[dblFreq];
                        TDSCDMA_Pin = dblPin;
                    }
                    dblResult[2] = TDSCDMA_Pout;

                    // Pin
                    dblResult[3] = TDSCDMA_Pin;
                    if (lastTDSCDMAPin.ContainsKey(dblFreq))
                        lastTDSCDMAPin[dblFreq] = TDSCDMA_Pin;
                    else
                        lastTDSCDMAPin.Add(dblFreq, TDSCDMA_Pin);

                    // Gain
                    dblResult[4] = TDSCDMA_Pout - TDSCDMA_Pin;

                    // Icc
                    TDSCDMA_Icc = _PS_66332A.Max_Current();
                    dblResult[5] = TDSCDMA_Icc * 1000;
                    // PAE
                    dblResult[6] = 100 * (Math.Pow(10, ((TDSCDMA_Pout - 30) / 10))) / TestSetting.LEVEL_VCC / TDSCDMA_Icc;

                    //ACP
                    _MXA_N9020A.Config__TDSCDMA_ACP(dblFreq, 0);
                    intDelay = Math.Max(intDelay_SigGen, intDelay_MXA);
                    util.Wait(intDelay * 3);

                    dblACPResult = _MXA_N9020A.Get_TDSCDMA_ACP_Result();

                    dblResult[7] = dblACPResult[1];     // -5MHz
                    dblResult[8] = dblACPResult[2];     // +5MHz
                    dblResult[9] = dblACPResult[3];     // -10MHz
                    dblResult[10] = dblACPResult[4];     // +10MHz

                    // EVM
                    if (!cbxBypassEVM.Checked)
                    {
                        _MXA_N9020A.Config__TDSCDMA_EVM(dblFreq);
                        intDelay = Math.Max(intDelay_SigGen, intDelay_MXA);
                        util.Wait(intDelay * 3);

                        dblResult[11] = _MXA_N9020A.Get_TDSCDMA_EVM_Result();
                    }
                    else
                    {
                        dblResult[11] = 0;
                    }
                    // Update_Grid
                    DataRow drNew = dtTDSCDMA.NewRow();
                    DataRow drNewtmp = dtTDSCDMATMP.NewRow();
                    drNewtmp[0] = drNew[0] = intTestID++;
                    for (int i = 0; i < dblResult.Count(); i++)
                    {
                        drNew[i + 1] = drNewtmp[i + 1] = Math.Round(dblResult[i], 2);
                    }
                    dtTDSCDMATMP.Rows.Add(drNewtmp);

                    this.Invoke((MethodInvoker)(delegate
                    {
                        dtTDSCDMA.Rows.Add(drNew);
                        dgvSweepResult.Refresh();
                        Application.DoEvents();
                    }));
                }
            }

            #endregion --- TDSCDMA Test ---

            #region --- Worst acp report ---

            foreach (double dblFreq in TestSetting.FREQLIST)
            {
                DataRow drmax = dtTDSCDMA.NewRow();
                drmax[0] = intTestID++;
                drmax[1] = dblFreq;

                var acp_n5 = from x in dtTDSCDMATMP.AsEnumerable()
                             where x.Field<double>("Frequency (MHz)") == dblFreq
                             select x.Field<double>("ACP -1.6MHz(dB)");

                var acp_p5 = from x in dtTDSCDMATMP.AsEnumerable()
                             where x.Field<double>("Frequency (MHz)") == dblFreq
                             select x.Field<double>("ACP +1.6MHz(dB)");

                var acp_n10 = from x in dtTDSCDMATMP.AsEnumerable()
                              where x.Field<double>("Frequency (MHz)") == dblFreq
                              select x.Field<double>("ACP -3.2MHz(dB)");

                var acp_p10 = from x in dtTDSCDMATMP.AsEnumerable()
                              where x.Field<double>("Frequency (MHz)") == dblFreq
                              select x.Field<double>("ACP +3.2MHz(dB)");

                var evm = from x in dtTDSCDMATMP.AsEnumerable()
                          where x.Field<double>("Frequency (MHz)") == dblFreq
                          select x.Field<double>("EVM(%)");

                try
                {
                    drmax[8] = acp_n5.ToList<double>().Max();
                    drmax[9] = acp_p5.ToList<double>().Max();
                    drmax[10] = acp_n10.ToList<double>().Max();
                    drmax[11] = acp_p10.ToList<double>().Max();
                    drmax[12] = evm.ToList<double>().Max();
                }
                catch
                {
                    drmax[8] = 0;
                    drmax[9] = 0;
                    drmax[10] = 0;
                    drmax[11] = 0;
                    drmax[12] = 0;
                }

                this.Invoke((MethodInvoker)(delegate
                {
                    dtTDSCDMA.Rows.Add(drmax);
                    dgvSweepResult.Refresh();
                    Application.DoEvents();
                }));

            }
            #endregion *** Worst acp report ***

            #region --- After Test ---

            AfterTest();
            _E4438C.SetOutput(Output.OFF);
            _PS_66332A.SetOutput(Output.OFF);
            //_PS_E3631A.SetOutput(Output.OFF);
            _Arb_33522A.SetOutput(Output.OFF, Arb_Channel.Channel_1);
            _Arb_33522A.SetOutput(Output.OFF, Arb_Channel.Channel_2);

            #endregion --- After Test ---
        }

        void WCDMA_TEST_SH_2()
        {
            #region --- Variable Define ---

            Dictionary<double, double> lastPin = new Dictionary<double, double>();
            int intDelay;
            int intTestID = 1;
            double Pin_Start = -10;
            double[] dblResult = new double[12];

            DataTable dtWCDMATMP = new DataTable();
            dtWCDMATMP = dtWCDMA.Clone();

            if (!cbxKeepPrevious.Checked)
            {
                this.Invoke((MethodInvoker)(delegate
                {
                    dtWCDMA.Clear();
                    dgvSweepResult.Refresh();
                }));
            }

            #endregion --- Variable Define ---

            #region --- Initialize ---

            BeforeTest();
            //_PS_E3631A = new PS_E3631A(Instruments_address._06);
            //_PS_E3631A.Initialize();

            SetVCC(TestSetting.LEVEL_VCC);
            //SetVbat(TestSetting.LEVEL_VBAT, 1);
            //SetTXEnable(TestSetting.LEVEL_TXEN);
            //SetVramp(TestSetting.LEVEL_PIN_VRAMP);
            //SetGPCTRL(TestSetting.LEVEL_GPCTRL);

            _E4438C.Mode_Initialize(Modulation.WCDMA);
            _MXA_N9020A.Mod_Initialize(Modulation.WCDMA);

            _E4438C.SetModOutput(Output.ON);
            _E4438C.SetOutput(Output.ON);

            DateTime t_Start = DateTime.Now;

            string Content = "Make sure everything is setup for WCDMA testing \r\n" +
                              "  1) Connect rf source cable to WCDMA \r\n" +
                              "  2) Set Control box to WCDMA mode \r\n" +
                              "  3) Connect MXA to coupler out directly";

            this.Invoke((MethodInvoker)(delegate
            {
                MessageBox.Show(this, Content, "WCDMA Testing", MessageBoxButtons.OK);
                btnStop.Enabled = false;
                Application.DoEvents();
                wait_2_start(t_Start);
            }));

            #endregion --- Initialize ---

            #region --- WCDMA Test ---

            for (double dblPout_Target = TestSetting.LEVEL_START; dblPout_Target <= TestSetting.LEVEL_STOP; dblPout_Target += TestSetting.LEVEL_STEP)
            {
                if (StopTest) break;

                double PoutLL = dblPout_Target - 0.05;
                double PoutUL = dblPout_Target + 0.05;

                foreach (double dblFreq in TestSetting.FREQLIST)
                {
                    if (StopTest) break;

                    double WCDMA_Pout = 0;
                    double WCDMA_Pin = 0;
                    double WCDMA_Icc = 0;
                    double dblPin = Pin_Start;
                    double[] dblACPResult = new double[5];

                    if (lastPin.ContainsKey(dblFreq)) WCDMA_Pin = dblPin = Pin_Start = lastPin[dblFreq] + TestSetting.LEVEL_STEP;
                    _E4438C.SetFrequency(dblFreq);
                    _E4438C.SetPower(Pin_Start + TestSetting.LOSS_SRC[dblFreq]);

                    _MXA_N9020A.Config__WCDMA_CHP(dblFreq);
                    intDelay = Math.Max(intDelay_SigGen, intDelay_MXA) * 10;
                    //_PM_N1913A.Configure__CW_Power(dblFreq, 10);
                    //intDelay = Math.Max(intDelay_SigGen, intDelay_PowerMeter) * 2;
                    util.Wait(intDelay);

                    WCDMA_Pout = _MXA_N9020A.Get_WCDMA_CHP_Result();
                    WCDMA_Pout += TestSetting.LOSS_MSR_THROUGH[dblFreq];
                    //WCDMA_Pout = _PM_N1913A.GetPowerResult();
                    //WCDMA_Pout += TestSetting.LOSS_MSR_POUT[dblFreq];

                    // Freqquency
                    dblResult[0] = dblFreq;

                    // Target Power
                    dblResult[1] = dblPout_Target;

                    // Pout
                    while (WCDMA_Pout <= PoutLL || WCDMA_Pout >= PoutUL)
                    {
                        dblPin = dblPin + dblPout_Target - WCDMA_Pout;
                        if (dblPin > 5)
                        {
                            dblPin = 5;
                            _E4438C.SetPower(dblPin + TestSetting.LOSS_SRC[dblFreq]);
                            util.Wait(intDelay);
                            WCDMA_Pout = _MXA_N9020A.Get_WCDMA_CHP_Result();
                            WCDMA_Pout += TestSetting.LOSS_MSR_THROUGH[dblFreq];
                            //WCDMA_Pout = _PM_N1913A.GetPowerResult();
                            //WCDMA_Pout += TestSetting.LOSS_MSR_POUT[dblFreq];
                            WCDMA_Pin = dblPin;

                            break;
                        }
                        _E4438C.SetPower(dblPin + TestSetting.LOSS_SRC[dblFreq]);
                        util.Wait(intDelay);
                        WCDMA_Pout = _MXA_N9020A.Get_WCDMA_CHP_Result();
                        WCDMA_Pout += TestSetting.LOSS_MSR_THROUGH[dblFreq];
                        //WCDMA_Pout = _PM_N1913A.GetPowerResult();
                        //WCDMA_Pout += TestSetting.LOSS_MSR_POUT[dblFreq];
                        WCDMA_Pin = dblPin;
                    }
                    dblResult[2] = WCDMA_Pout;

                    // Pin
                    dblResult[3] = WCDMA_Pin;

                    if (lastPin.ContainsKey(dblFreq))
                        lastPin[dblFreq] = WCDMA_Pin;
                    else
                        lastPin.Add(dblFreq, WCDMA_Pin);
                    // Gain
                    dblResult[4] = WCDMA_Pout - WCDMA_Pin;

                    // Icc
                    WCDMA_Icc = _PS_66332A.RMS_Current();
                    dblResult[5] = WCDMA_Icc * 1000;
                    // PAE
                    dblResult[6] = 100 * (Math.Pow(10, ((WCDMA_Pout - 30) / 10))) / TestSetting.LEVEL_VCC / WCDMA_Icc;

                    // ACP
                    _MXA_N9020A.Config__WCDMA_ACP(dblFreq);
                    intDelay = Math.Max(intDelay_SigGen, intDelay_MXA);
                    util.Wait(intDelay * 3);

                    dblACPResult = _MXA_N9020A.Get_WCDMA_ACP_Result();

                    dblResult[7] = dblACPResult[1];     // -5MHz
                    dblResult[8] = dblACPResult[2];     // +5MHz
                    dblResult[9] = dblACPResult[3];     // -10MHz
                    dblResult[10] = dblACPResult[4];     // +10MHz

                    // EVM
                    if (!cbxBypassEVM.Checked)
                    {
                        _MXA_N9020A.Config__WCDMA_EVM(dblFreq);
                        intDelay = Math.Max(intDelay_SigGen, intDelay_MXA);
                        util.Wait(intDelay * 3);

                        dblResult[11] = _MXA_N9020A.Get_WCDMA_EVM_Result();
                    }
                    else
                    {
                        dblResult[11] = 0;
                    }
                    // Update_Grid
                    DataRow drNew = dtWCDMA.NewRow();
                    DataRow drNewtmp = dtWCDMATMP.NewRow();
                    drNew[0] = intTestID++;
                    for (int i = 0; i < dblResult.Count(); i++)
                    {
                        drNew[i + 1] = drNewtmp[i + 1] = Math.Round(dblResult[i], 2);
                    }
                    dtWCDMATMP.Rows.Add(drNewtmp);

                    this.Invoke((MethodInvoker)(delegate
                    {
                        dtWCDMA.Rows.Add(drNew);
                        dgvSweepResult.Refresh();
                        Application.DoEvents();
                    }));
                }
            }

            #endregion --- WCDMA Test ---

            #region --- Worst acp report ---

            foreach (double dblFreq in TestSetting.FREQLIST)
            {
                DataRow drmax = dtWCDMA.NewRow();
                drmax[0] = intTestID++;
                drmax[1] = dblFreq;

                var acp_n5 = from x in dtWCDMATMP.AsEnumerable()
                             where x.Field<double>("Frequency (MHz)") == dblFreq
                             select x.Field<double>("ACP -5MHz(dB)");

                var acp_p5 = from x in dtWCDMATMP.AsEnumerable()
                             where x.Field<double>("Frequency (MHz)") == dblFreq
                             select x.Field<double>("ACP +5MHz(dB)");

                var acp_n10 = from x in dtWCDMATMP.AsEnumerable()
                              where x.Field<double>("Frequency (MHz)") == dblFreq
                              select x.Field<double>("ACP -10MHz(dB)");

                var acp_p10 = from x in dtWCDMATMP.AsEnumerable()
                              where x.Field<double>("Frequency (MHz)") == dblFreq
                              select x.Field<double>("ACP +10MHz(dB)");

                var evm = from x in dtWCDMATMP.AsEnumerable()
                          where x.Field<double>("Frequency (MHz)") == dblFreq
                          select x.Field<double>("EVM(%)");

                try
                {
                    drmax[8] = acp_n5.ToList<double>().Max();
                    drmax[9] = acp_p5.ToList<double>().Max();
                    drmax[10] = acp_n10.ToList<double>().Max();
                    drmax[11] = acp_p10.ToList<double>().Max();
                    drmax[12] = evm.ToList<double>().Max();
                }
                catch
                {
                    drmax[8] = 0;
                    drmax[9] = 0;
                    drmax[10] = 0;
                    drmax[11] = 0;
                    drmax[12] = 0;
                }

                this.Invoke((MethodInvoker)(delegate
                {
                    dtWCDMA.Rows.Add(drmax);
                    dgvSweepResult.Refresh();
                    Application.DoEvents();
                }));

            }
            #endregion *** Worst acp report ***

            #region --- After Test ---

            AfterTest();
            _E4438C.SetOutput(Output.OFF);
            _PS_66332A.SetOutput(Output.OFF);
            //_PS_E3631A.SetOutput(Output.OFF);
            _Arb_33522A.SetOutput(Output.OFF, Arb_Channel.Channel_1);
            _Arb_33522A.SetOutput(Output.OFF, Arb_Channel.Channel_2);

            #endregion --- After Test ---

        }

        void LTETDD_TEST_SH_2(object Band)
        {
            BandList WhichBand = (BandList)Band;

            #region --- Variable Define ---

            Dictionary<double, double> lastLTETDDPin = new Dictionary<double, double>();
            int intDelay;
            int intTestID = 1;
            double Pin_Start = -10;
            double[] dblResult = new double[14];
            string waveform_name = "";

            this.Invoke((MethodInvoker)(delegate
            {
                waveform_name = cbxWaveform.SelectedItem.ToString();
            }));


            DataTable dtLTETDDTMP = new DataTable();
            dtLTETDDTMP = dtLTETDD_B38.Clone();

            if (!cbxKeepPrevious.Checked)
            {
                this.Invoke((MethodInvoker)(delegate
                {
                    if (WhichBand == BandList.B_1)
                        dtLTETDD_B38.Clear();
                    else if (WhichBand == BandList.B_2)
                        dtLTETDD_B40.Clear();
                    else
                        throw new Exception("No this band");

                    dgvSweepResult.Refresh();
                }));
            }
            #endregion --- Variable Define ---

            #region --- Initialize ---

            BeforeTest();

            SetVCC(TestSetting.LEVEL_VCC);
            SetGPCTRL(TestSetting.LEVEL_GPCTRL);

            if (WhichBand == BandList.B_1)
            {
                SetVEN(Arb_Channel.Channel_1, TestSetting.LEVEL_TXEN, false);
                SetVEN(Arb_Channel.Channel_2, 0.05, true);
            }
            else if (WhichBand == BandList.B_2)
            {
                SetVEN(Arb_Channel.Channel_1, 0.05, true);
                SetVEN(Arb_Channel.Channel_2, TestSetting.LEVEL_TXEN, false);
            }
            else
            {
                throw new Exception("No this band");
            }


            _MXA_N9020A.Mod_Initialize(Modulation.LTETDD, true);
            _MXA_N9020A.SetFrequency(2300);
            _E4438C.SetFrequency(2300);
            _E4438C.Mode_Initialize(Modulation.LTETDD, waveform_name);

            _E4438C.SetModOutput(Output.ON);
            _E4438C.SetOutput(Output.ON);

            DateTime t_Start = DateTime.Now;

            string Content = "Make sure everything is setup for LTETDD testing \r\n" +
                              "  1) Connect rf source cable to RFIN \r\n" +
                              "  2) Connect ANT with 20dB pad to RFOUT \r\n" +
                              "  3) Set Control box to right mode \r\n" +
                              "  4) Connect MXA to coupler out directly";

            this.Invoke((MethodInvoker)(delegate
            {
                MessageBox.Show(this, Content, "LTETDD Testing", MessageBoxButtons.OK);
                btnStop.Enabled = false;
                Application.DoEvents();
                wait_2_start(t_Start);
            }));


            #endregion --- Initialize ---

            #region --- LTETDD Test ---

            for (double dblPout_Target = TestSetting.LEVEL_START; dblPout_Target <= TestSetting.LEVEL_STOP; dblPout_Target += TestSetting.LEVEL_STEP)
            {
                if (StopTest) break;

                double PoutLL = dblPout_Target - 0.05;
                double PoutUL = dblPout_Target + 0.05;

                foreach (double dblFreq in TestSetting.FREQLIST)
                {
                    if (StopTest) break;

                    double LTETDD_Pout = 0;
                    double LTETDD_Pin = 0;
                    double LTETDD_Icc = 0;
                    double dblPin = Pin_Start;
                    double[] dblACPResult = new double[5];

                    if (lastLTETDDPin.ContainsKey(dblFreq)) LTETDD_Pin = dblPin = Pin_Start = lastLTETDDPin[dblFreq] + TestSetting.LEVEL_STEP;
                    _E4438C.SetFrequency(dblFreq);
                    _E4438C.SetPower(Pin_Start + TestSetting.LOSS_SRC[dblFreq]);

                    _MXA_N9020A.Config__LTETDD_CHP(dblFreq);
                    intDelay = Math.Max(intDelay_SigGen, intDelay_MXA) * 10;
                    util.Wait(intDelay);

                    LTETDD_Pout = _MXA_N9020A.Get_LTETDD_CHP_Result();
                    LTETDD_Pout += TestSetting.LOSS_MSR_THROUGH[dblFreq];

                    // Freqquency
                    dblResult[0] = dblFreq;

                    // Target Power
                    dblResult[1] = dblPout_Target;

                    //Pout
                    while (LTETDD_Pout <= PoutLL || LTETDD_Pout >= PoutUL)
                    {
                        dblPin = dblPin + dblPout_Target - LTETDD_Pout;
                        if (dblPin > 5)
                            break;

                        _E4438C.SetPower(dblPin + TestSetting.LOSS_SRC[dblFreq]);
                        util.Wait(intDelay);
                        LTETDD_Pout = _MXA_N9020A.Get_LTETDD_CHP_Result();
                        LTETDD_Pout += TestSetting.LOSS_MSR_THROUGH[dblFreq];
                        LTETDD_Pin = dblPin;
                    }
                    dblResult[2] = LTETDD_Pout;

                    // Pin
                    dblResult[3] = LTETDD_Pin;
                    if (lastLTETDDPin.ContainsKey(dblFreq))
                        lastLTETDDPin[dblFreq] = LTETDD_Pin;
                    else
                        lastLTETDDPin.Add(dblFreq, LTETDD_Pin);

                    // Gain
                    dblResult[4] = LTETDD_Pout - LTETDD_Pin;

                    // Icc
                    LTETDD_Icc = _PS_66332A.Max_Current();
                    dblResult[5] = LTETDD_Icc * 1000;
                    // PAE
                    dblResult[6] = 100 * (Math.Pow(10, ((LTETDD_Pout - 30) / 10))) / TestSetting.LEVEL_VCC / LTETDD_Icc;

                    //ACP E_ULTRA
                    _MXA_N9020A.Config__LTETDD_ACP_EULTRA(dblFreq);
                    intDelay = Math.Max(intDelay_SigGen, intDelay_MXA);
                    util.Wait(intDelay * 3);

                    dblACPResult = _MXA_N9020A.Get_LTETDD_ACP_EULTRA_Result();

                    dblResult[7] = dblACPResult[1];     // -10MHz
                    dblResult[8] = dblACPResult[2];     // +10MHz

                    //ACP ULTRA
                    _MXA_N9020A.Config__LTETDD_ACP_ULTRA(dblFreq);
                    intDelay = Math.Max(intDelay_SigGen, intDelay_MXA);
                    util.Wait(intDelay * 3);

                    dblACPResult = _MXA_N9020A.Get_LTETDD_ACP_ULTRA_Result();

                    dblResult[9] = dblACPResult[1];     // -5.8MHz
                    dblResult[10] = dblACPResult[2];     // -5.8MHz
                    dblResult[11] = dblACPResult[3];     // -7.4MHz
                    dblResult[12] = dblACPResult[4];     // -7.4MHz

                    // EVM
                    if (!cbxBypassEVM.Checked)
                    {
                        _MXA_N9020A.Config__LTETDD_EVM(dblFreq);
                        intDelay = Math.Max(intDelay_SigGen, intDelay_MXA);
                        util.Wait(intDelay * 3);

                        dblResult[13] = _MXA_N9020A.Get_LTETDD_EVM_Result();
                    }
                    else
                    {
                        dblResult[13] = 0;
                    }
                    // Update_Grid
                    DataRow drNew = null;
                    if (WhichBand == BandList.B_1)
                        drNew = dtLTETDD_B38.NewRow();
                    else if (WhichBand == BandList.B_2)
                        drNew = dtLTETDD_B40.NewRow();
                    else
                        throw new Exception("No this band");

                    DataRow drNewtmp = dtLTETDDTMP.NewRow();
                    drNewtmp[0] = drNew[0] = intTestID++;
                    for (int i = 0; i < dblResult.Count(); i++)
                    {
                        drNew[i + 1] = drNewtmp[i + 1] = Math.Round(dblResult[i], 2);
                    }
                    dtLTETDDTMP.Rows.Add(drNewtmp);

                    this.Invoke((MethodInvoker)(delegate
                    {
                        if (WhichBand == BandList.B_1)
                            dtLTETDD_B38.Rows.Add(drNew);
                        else if (WhichBand == BandList.B_2)
                            dtLTETDD_B40.Rows.Add(drNew);
                        else
                            throw new Exception("No this band");

                        dgvSweepResult.Refresh();
                        Application.DoEvents();
                    }));
                }
            }

            #endregion --- LTETDD Test ---

            #region --- Worst acp report ---

            foreach (double dblFreq in TestSetting.FREQLIST)
            {
                DataRow drmax = null;
                if (WhichBand == BandList.B_1)
                    drmax = dtLTETDD_B38.NewRow();
                else if (WhichBand == BandList.B_2)
                    drmax = dtLTETDD_B40.NewRow();
                else
                    throw new Exception("No this band");

                drmax[0] = intTestID++;
                drmax[1] = dblFreq;

                var acp_n10 = from x in dtLTETDDTMP.AsEnumerable()
                              where x.Field<double>("Frequency (MHz)") == dblFreq
                              select x.Field<double>("ACP_EULTRA -10MHz(dB)");

                var acp_p10 = from x in dtLTETDDTMP.AsEnumerable()
                              where x.Field<double>("Frequency (MHz)") == dblFreq
                              select x.Field<double>("ACP_EULTRA +10MHz(dB)");

                var acp_n58 = from x in dtLTETDDTMP.AsEnumerable()
                              where x.Field<double>("Frequency (MHz)") == dblFreq
                              select x.Field<double>("ACP_ULTRA -0.8MHz(dB)");

                var acp_p58 = from x in dtLTETDDTMP.AsEnumerable()
                              where x.Field<double>("Frequency (MHz)") == dblFreq
                              select x.Field<double>("ACP_ULTRA +0.8MHz(dB)");

                var acp_n74 = from x in dtLTETDDTMP.AsEnumerable()
                              where x.Field<double>("Frequency (MHz)") == dblFreq
                              select x.Field<double>("ACP_ULTRA -2.4MHz(dB)");

                var acp_p74 = from x in dtLTETDDTMP.AsEnumerable()
                              where x.Field<double>("Frequency (MHz)") == dblFreq
                              select x.Field<double>("ACP_ULTRA +2.4MHz(dB)");

                var evm = from x in dtLTETDDTMP.AsEnumerable()
                          where x.Field<double>("Frequency (MHz)") == dblFreq
                          select x.Field<double>("EVM(%)");

                try
                {
                    drmax[8] = acp_n10.ToList<double>().Max();
                    drmax[9] = acp_p10.ToList<double>().Max();
                    drmax[10] = acp_n58.ToList<double>().Max();
                    drmax[11] = acp_p58.ToList<double>().Max();
                    drmax[12] = acp_n74.ToList<double>().Max();
                    drmax[13] = acp_p74.ToList<double>().Max();
                    drmax[14] = evm.ToList<double>().Max();
                }
                catch
                {
                    drmax[8] = 0;
                    drmax[9] = 0;
                    drmax[10] = 0;
                    drmax[11] = 0;
                    drmax[12] = 0;
                    drmax[13] = 0;
                    drmax[14] = 0;
                }

                this.Invoke((MethodInvoker)(delegate
                {
                    if (WhichBand == BandList.B_1)
                        dtLTETDD_B38.Rows.Add(drmax);
                    else if (WhichBand == BandList.B_2)
                        dtLTETDD_B40.Rows.Add(drmax);
                    else
                        throw new Exception("No this band");

                    dgvSweepResult.Refresh();
                    Application.DoEvents();
                }));

            }
            #endregion *** Worst acp report ***

            #region --- After Test ---

            AfterTest();
            _E4438C.SetOutput(Output.OFF);
            _PS_66332A.SetOutput(Output.OFF);
            //_PS_E3631A.SetOutput(Output.OFF);
            _Arb_33522A.SetOutput(Output.OFF, Arb_Channel.Channel_1);
            _Arb_33522A.SetOutput(Output.OFF, Arb_Channel.Channel_2);

            #endregion --- After Test ---
        }

        void LTEFDD_TEST_SH_2(object Band)
        {
            BandList WhichBand = (BandList)Band;

            #region --- Variable Define ---

            Dictionary<double, double> lastLTEFDDPin = new Dictionary<double, double>();
            int intDelay;
            int intTestID = 1;
            double Pin_Start = -10;
            double[] dblResult = new double[14];
            string waveform_name = "";

            this.Invoke((MethodInvoker)(delegate
            {
                waveform_name = cbxWaveform.SelectedItem.ToString();
            }));


            DataTable dtLTEFDDTMP = new DataTable();
            dtLTEFDDTMP = dtLTEFDD_B1.Clone();

            if (!cbxKeepPrevious.Checked)
            {
                this.Invoke((MethodInvoker)(delegate
                {
                    if (WhichBand == BandList.B_1)
                        dtLTEFDD_B1.Clear();
                    else if (WhichBand == BandList.B_2)
                        dtLTEFDD_B2.Clear();
                    else
                        throw new Exception("No this band");

                    dgvSweepResult.Refresh();
                }));
            }

            #endregion --- Variable Define ---

            #region --- Initialize ---

            BeforeTest();

            SetVCC(TestSetting.LEVEL_VCC);
            SetGPCTRL(TestSetting.LEVEL_GPCTRL);

            if (WhichBand == BandList.B_1)
            {
                SetVEN(Arb_Channel.Channel_1, TestSetting.LEVEL_TXEN, true);
                SetVEN(Arb_Channel.Channel_2, 0.05, true);
            }
            else if (WhichBand == BandList.B_2)
            {
                SetVEN(Arb_Channel.Channel_1, 0.05, true);
                SetVEN(Arb_Channel.Channel_2, TestSetting.LEVEL_TXEN, true);
            }
            else
            {
                throw new Exception("No this band");
            }



            _MXA_N9020A.Mod_Initialize(Modulation.LTEFDD, true);
            _MXA_N9020A.SetFrequency(2496);
            _E4438C.SetFrequency(2496);
            _E4438C.Mode_Initialize(Modulation.LTEFDD, waveform_name);

            _E4438C.SetModOutput(Output.ON);
            _E4438C.SetOutput(Output.ON);

            DateTime t_Start = DateTime.Now;

            string Content = "Make sure everything is setup for LTEFDD testing \r\n" +
                              "  1) Connect rf source cable to RFIN \r\n" +
                              "  2) Connect ANT with 20dB pad to RFOUT \r\n" +
                              "  3) Set Control box to right mode \r\n" +
                              "  4) Connect MXA to coupler out directly";

            this.Invoke((MethodInvoker)(delegate
            {
                MessageBox.Show(this, Content, "LTEFDD Testing", MessageBoxButtons.OK);
                Application.DoEvents();
                wait_2_start(t_Start);
            }));

            #endregion --- Initialize ---

            #region --- LTEFDD Test ---

            for (double dblPout_Target = TestSetting.LEVEL_START; dblPout_Target <= TestSetting.LEVEL_STOP; dblPout_Target += TestSetting.LEVEL_STEP)
            {
                if (StopTest) break;

                double PoutLL = dblPout_Target - 0.05;
                double PoutUL = dblPout_Target + 0.05;

                foreach (double dblFreq in TestSetting.FREQLIST)
                {
                    if (StopTest) break;

                    double LTEFDD_Pout = 0;
                    double LTEFDD_Pin = 0;
                    double LTEFDD_Icc = 0;
                    double dblPin = Pin_Start;
                    double[] dblACPResult = new double[5];

                    if (lastLTEFDDPin.ContainsKey(dblFreq)) LTEFDD_Pin = dblPin = Pin_Start = lastLTEFDDPin[dblFreq] + TestSetting.LEVEL_STEP;
                    _E4438C.SetFrequency(dblFreq);
                    _E4438C.SetPower(Pin_Start + TestSetting.LOSS_SRC[dblFreq]);

                    _MXA_N9020A.Config__LTEFDD_CHP(dblFreq);
                    intDelay = Math.Max(intDelay_SigGen, intDelay_MXA) * 10;
                    util.Wait(intDelay);

                    LTEFDD_Pout = _MXA_N9020A.Get_LTEFDD_CHP_Result();
                    LTEFDD_Pout += TestSetting.LOSS_MSR_THROUGH[dblFreq];

                    // Freqquency
                    dblResult[0] = dblFreq;

                    // Target Power
                    dblResult[1] = dblPout_Target;

                    //Pout
                    while (LTEFDD_Pout <= PoutLL || LTEFDD_Pout >= PoutUL)
                    {
                        dblPin = dblPin + dblPout_Target - LTEFDD_Pout;
                        if (dblPin > 5)
                            break;

                        _E4438C.SetPower(dblPin + TestSetting.LOSS_SRC[dblFreq]);
                        util.Wait(intDelay);
                        LTEFDD_Pout = _MXA_N9020A.Get_LTEFDD_CHP_Result();
                        LTEFDD_Pout += TestSetting.LOSS_MSR_THROUGH[dblFreq];
                        LTEFDD_Pin = dblPin;
                    }
                    dblResult[2] = LTEFDD_Pout;

                    // Pin
                    dblResult[3] = LTEFDD_Pin;
                    if (lastLTEFDDPin.ContainsKey(dblFreq))
                        lastLTEFDDPin[dblFreq] = LTEFDD_Pin;
                    else
                        lastLTEFDDPin.Add(dblFreq, LTEFDD_Pin);

                    // Gain
                    dblResult[4] = LTEFDD_Pout - LTEFDD_Pin;

                    // Icc
                    LTEFDD_Icc = _PS_66332A.High_Current();
                    dblResult[5] = LTEFDD_Icc * 1000;
                    // PAE
                    dblResult[6] = 100 * (Math.Pow(10, ((LTEFDD_Pout - 30) / 10))) / TestSetting.LEVEL_VCC / LTEFDD_Icc;

                    //ACP E_ULTRA
                    _MXA_N9020A.Config__LTEFDD_ACP_EULTRA(dblFreq);
                    intDelay = Math.Max(intDelay_SigGen, intDelay_MXA);
                    util.Wait(intDelay * 3);

                    dblACPResult = _MXA_N9020A.Get_LTEFDD_ACP_EULTRA_Result();

                    dblResult[7] = dblACPResult[1];     // -10MHz
                    dblResult[8] = dblACPResult[2];     // +10MHz

                    //ACP ULTRA
                    _MXA_N9020A.Config__LTEFDD_ACP_ULTRA(dblFreq);
                    intDelay = Math.Max(intDelay_SigGen, intDelay_MXA);
                    util.Wait(intDelay * 3);

                    dblACPResult = _MXA_N9020A.Get_LTEFDD_ACP_ULTRA_Result();

                    dblResult[9] = dblACPResult[1];     // -5.8MHz
                    dblResult[10] = dblACPResult[2];     // -5.8MHz
                    dblResult[11] = dblACPResult[3];     // -7.4MHz
                    dblResult[12] = dblACPResult[4];     // -7.4MHz

                    // EVM
                    if (!cbxBypassEVM.Checked)
                    {
                        _MXA_N9020A.Config__LTEFDD_EVM(dblFreq);
                        intDelay = Math.Max(intDelay_SigGen, intDelay_MXA);
                        util.Wait(intDelay * 3);

                        dblResult[13] = _MXA_N9020A.Get_LTEFDD_EVM_Result();
                    }
                    else
                    {
                        dblResult[13] = 0;
                    }
                    // Update_Grid
                    DataRow drNew = null;
                    if (WhichBand == BandList.B_1)
                        drNew = dtLTEFDD_B1.NewRow();
                    else if (WhichBand == BandList.B_2)
                        drNew = dtLTEFDD_B2.NewRow();
                    else
                        throw new Exception("No this band");

                    DataRow drNewtmp = dtLTEFDDTMP.NewRow();
                    drNewtmp[0] = drNew[0] = intTestID++;
                    for (int i = 0; i < dblResult.Count(); i++)
                    {
                        drNew[i + 1] = drNewtmp[i + 1] = Math.Round(dblResult[i], 2);
                    }
                    dtLTEFDDTMP.Rows.Add(drNewtmp);

                    this.Invoke((MethodInvoker)(delegate
                    {
                        if (WhichBand == BandList.B_1)
                            dtLTEFDD_B1.Rows.Add(drNew);
                        else if (WhichBand == BandList.B_2)
                            dtLTEFDD_B2.Rows.Add(drNew);
                        else
                            throw new Exception("No this band");

                        dgvSweepResult.Refresh();
                        Application.DoEvents();
                    }));
                }
            }

            #endregion --- LTEFDD Test ---

            #region --- Worst acp report ---

            foreach (double dblFreq in TestSetting.FREQLIST)
            {
                DataRow drmax = null;
                if (WhichBand == BandList.B_1)
                    drmax = dtLTEFDD_B1.NewRow();
                else if (WhichBand == BandList.B_2)
                    drmax = dtLTEFDD_B2.NewRow();
                else
                    throw new Exception("No this band");

                drmax[0] = intTestID++;
                drmax[1] = dblFreq;

                var acp_n10 = from x in dtLTEFDDTMP.AsEnumerable()
                              where x.Field<double>("Frequency (MHz)") == dblFreq
                              select x.Field<double>("ACP_EULTRA -10MHz(dB)");

                var acp_p10 = from x in dtLTEFDDTMP.AsEnumerable()
                              where x.Field<double>("Frequency (MHz)") == dblFreq
                              select x.Field<double>("ACP_EULTRA +10MHz(dB)");

                var acp_n25 = from x in dtLTEFDDTMP.AsEnumerable()
                              where x.Field<double>("Frequency (MHz)") == dblFreq
                              select x.Field<double>("ACP_ULTRA -2.5MHz(dB)");

                var acp_p25 = from x in dtLTEFDDTMP.AsEnumerable()
                              where x.Field<double>("Frequency (MHz)") == dblFreq
                              select x.Field<double>("ACP_ULTRA +2.5MHz(dB)");

                var acp_n50 = from x in dtLTEFDDTMP.AsEnumerable()
                              where x.Field<double>("Frequency (MHz)") == dblFreq
                              select x.Field<double>("ACP_ULTRA -7.5MHz(dB)");

                var acp_p50 = from x in dtLTEFDDTMP.AsEnumerable()
                              where x.Field<double>("Frequency (MHz)") == dblFreq
                              select x.Field<double>("ACP_ULTRA +7.5MHz(dB)");

                var evm = from x in dtLTEFDDTMP.AsEnumerable()
                          where x.Field<double>("Frequency (MHz)") == dblFreq
                          select x.Field<double>("EVM(%)");

                try
                {
                    drmax[8] = acp_n10.ToList<double>().Max();
                    drmax[9] = acp_p10.ToList<double>().Max();
                    drmax[10] = acp_n25.ToList<double>().Max();
                    drmax[11] = acp_p25.ToList<double>().Max();
                    drmax[12] = acp_n50.ToList<double>().Max();
                    drmax[13] = acp_p50.ToList<double>().Max();
                    drmax[14] = evm.ToList<double>().Max();
                }
                catch
                {
                    drmax[8] = 0;
                    drmax[9] = 0;
                    drmax[10] = 0;
                    drmax[11] = 0;
                    drmax[12] = 0;
                    drmax[13] = 0;
                    drmax[14] = 0;
                }

                this.Invoke((MethodInvoker)(delegate
                {
                    if (WhichBand == BandList.B_1)
                        dtLTEFDD_B1.Rows.Add(drmax);
                    else if (WhichBand == BandList.B_2)
                        dtLTEFDD_B2.Rows.Add(drmax);
                    else
                        throw new Exception("No this band");

                    dgvSweepResult.Refresh();
                    Application.DoEvents();
                }));

            }
            #endregion *** Worst acp report ***

            #region --- After Test ---

            AfterTest();
            _E4438C.SetOutput(Output.OFF);
            _PS_66332A.SetOutput(Output.OFF);
            //_PS_E3631A.SetOutput(Output.OFF);
            _Arb_33522A.SetOutput(Output.OFF, Arb_Channel.Channel_1);
            _Arb_33522A.SetOutput(Output.OFF, Arb_Channel.Channel_2);

            #endregion --- After Test ---
        }

        void CDMA_TEST_SH_2(object Band)
        {
            BandList WhichBand = (BandList)Band;

            #region --- Variable Define ---

            Dictionary<double, double> lastPin = new Dictionary<double, double>();
            int intDelay;
            int intTestID = 1;
            double Pin_Start = -10;
            double[] dblResult = new double[12];
            string waveform_name = "";

            this.Invoke((MethodInvoker)(delegate
            {
                waveform_name = cbxWaveform.Items[1].ToString();
            }));

            DataTable dtCDMATMP = new DataTable();

            if (WhichBand == BandList.B_1)
                dtCDMATMP = dtCDMA.Clone();
            else if (WhichBand == BandList.B_2)
                dtCDMATMP = dtEVDO.Clone();
            else
                throw new Exception("No this band");

            if (!cbxKeepPrevious.Checked)
            {
                this.Invoke((MethodInvoker)(delegate
                {
                    if (WhichBand == BandList.B_1)
                        dtCDMA.Clear();
                    else if (WhichBand == BandList.B_2)
                        dtEVDO.Clear();
                    else
                        throw new Exception("No this band");

                    dgvSweepResult.Refresh();
                }));
            }

            #endregion --- Variable Define ---

            #region --- Initialize ---

            BeforeTest();
            //_PS_E3631A = new PS_E3631A(Instruments_address._06);
            //_PS_E3631A.Initialize();

            SetVCC(TestSetting.LEVEL_VCC);
            SetGPCTRL(TestSetting.LEVEL_GPCTRL);

            if (WhichBand == BandList.B_1)  // B_1 is CDMA
            {
                SetVEN(Arb_Channel.Channel_1, 0.05, true);
                SetVEN(Arb_Channel.Channel_2, 0.05, true);

                _E4438C.Mode_Initialize(Modulation.CDMA, waveform_name);
                _MXA_N9020A.Mod_Initialize(Modulation.CDMA, false);
            }
            else if (WhichBand == BandList.B_2)  // B_2 is EVDO
            {
                SetVEN(Arb_Channel.Channel_1, 0.05, true);
                SetVEN(Arb_Channel.Channel_2, 0.05, true);

                _E4438C.Mode_Initialize(Modulation.EVDO, waveform_name);
                _MXA_N9020A.Mod_Initialize(Modulation.EVDO, false);
            }
            else
            {
                throw new Exception("No This band");
            }
            _E4438C.SetModOutput(Output.ON);
            _E4438C.SetOutput(Output.ON);

            DateTime t_start = DateTime.Now;

            string Content = "Make sure everything is setup for CDMA/EVDO testing \r\n" +
                              "  1) Connect rf source cable to CDMA/EVDO \r\n" +
                              "  2) Set Control box to CDMA/EVDO mode \r\n" +
                              "  3) Connect MXA to coupler out directly";

            this.Invoke((MethodInvoker)(delegate
            {
                MessageBox.Show(this, Content, "CDMA/EVDO Testing", MessageBoxButtons.OK);
                Application.DoEvents();
                wait_2_start(t_start);
            }));

            #endregion --- Initialize ---

            #region --- CDMA/EVDO Test ---

            for (double dblPout_Target = TestSetting.LEVEL_START; dblPout_Target <= TestSetting.LEVEL_STOP; dblPout_Target += TestSetting.LEVEL_STEP)
            {
                if (StopTest) break;

                double PoutLL = dblPout_Target - 0.05;
                double PoutUL = dblPout_Target + 0.05;

                foreach (double dblFreq in TestSetting.FREQLIST)
                {
                    if (StopTest) break;

                    double CDMA_Pout = 0;
                    double CDMA_Pin = 0;
                    double CDMA_Icc = 0;
                    double dblPin = Pin_Start;
                    double[] dblACPResult = new double[5];

                    if (lastPin.ContainsKey(dblFreq)) CDMA_Pin = dblPin = Pin_Start = lastPin[dblFreq] + TestSetting.LEVEL_STEP;
                    _E4438C.SetFrequency(dblFreq);
                    _E4438C.SetPower(Pin_Start + TestSetting.LOSS_SRC[dblFreq]);

                    // MXA to measure channel power
                    #region
                    intDelay = Math.Max(intDelay_SigGen, intDelay_MXA) * 10;
                    if (WhichBand == BandList.B_1)
                    {
                        _MXA_N9020A.Config__CDMA_CHP(dblFreq);
                        util.Wait(intDelay);

                        CDMA_Pout = _MXA_N9020A.Get_CDMA_CHP_Result();
                    }
                    else if (WhichBand == BandList.B_2)
                    {
                        _MXA_N9020A.Config__EVDO_CHP(dblFreq);
                        util.Wait(intDelay);

                        CDMA_Pout = _MXA_N9020A.Get_EVDO_CHP_Result();
                    }
                    else
                        throw new Exception("No this band");

                    CDMA_Pout += TestSetting.LOSS_MSR_THROUGH[dblFreq];
                    #endregion

                    //// PowerMeter A to measure channel power
                    //_PM_N1913A.Configure__CW_Power(dblFreq, 10);
                    //intDelay = Math.Max(intDelay_SigGen, intDelay_PowerMeter) * 2;
                    //util.Wait(intDelay);
                    //CDMA_Pout = _PM_N1913A.GetPowerResult();
                    //CDMA_Pout += TestSetting.LOSS_MSR_POUT[dblFreq];

                    // Freqquency
                    dblResult[0] = dblFreq;

                    // Target Power
                    dblResult[1] = dblPout_Target;

                    // Pout
                    while (CDMA_Pout <= PoutLL || CDMA_Pout >= PoutUL)
                    {
                        dblPin = dblPin + dblPout_Target - CDMA_Pout;
                        if (dblPin > 5)
                            break;

                        _E4438C.SetPower(dblPin + TestSetting.LOSS_SRC[dblFreq]);
                        util.Wait(intDelay);

                        // MXA to measure channel power
                        #region
                        if (WhichBand == BandList.B_1)
                            CDMA_Pout = _MXA_N9020A.Get_CDMA_CHP_Result();
                        else if (WhichBand == BandList.B_2)
                            CDMA_Pout = _MXA_N9020A.Get_EVDO_CHP_Result();
                        else
                            throw new Exception("No this band");

                        CDMA_Pout += TestSetting.LOSS_MSR_THROUGH[dblFreq];
                        #endregion

                        //// PowerMeter A to measure channel power
                        //CDMA_Pout = _PM_N1913A.GetPowerResult();
                        //CDMA_Pout += TestSetting.LOSS_MSR_POUT[dblFreq];

                        CDMA_Pin = dblPin;
                    }
                    dblResult[2] = CDMA_Pout;

                    // Pin
                    dblResult[3] = CDMA_Pin;

                    if (lastPin.ContainsKey(dblFreq))
                        lastPin[dblFreq] = CDMA_Pin;
                    else
                        lastPin.Add(dblFreq, CDMA_Pin);
                    // Gain
                    dblResult[4] = CDMA_Pout - CDMA_Pin;

                    // Icc
                    CDMA_Icc = _PS_66332A.RMS_Current();
                    dblResult[5] = CDMA_Icc * 1000;
                    // PAE
                    dblResult[6] = 100 * (Math.Pow(10, ((CDMA_Pout - 30) / 10))) / TestSetting.LEVEL_VCC / CDMA_Icc;

                    // ACP
                    intDelay = Math.Max(intDelay_SigGen, intDelay_MXA);
                    if (WhichBand == BandList.B_1)
                    {
                        _MXA_N9020A.Config__CDMA_ACP(dblFreq);
                        util.Wait(intDelay * 3);

                        dblACPResult = _MXA_N9020A.Get_CDMA_ACP_Result();
                    }
                    else if (WhichBand == BandList.B_2)
                    {
                        _MXA_N9020A.Config__EVDO_ACP(dblFreq);
                        util.Wait(intDelay * 3);

                        dblACPResult = _MXA_N9020A.Get_EVDO_ACP_Result();
                    }
                    else
                        throw new Exception("No this band");


                    dblResult[7] = dblACPResult[1];     // -5MHz
                    dblResult[8] = dblACPResult[2];     // +5MHz
                    dblResult[9] = dblACPResult[3];     // -10MHz
                    dblResult[10] = dblACPResult[4];     // +10MHz

                    // EVM
                    if (!cbxBypassEVM.Checked)
                    {
                        ////waveform_name = cbxWaveform.Items[1].ToString();

                        ////if (WhichBand == BandList.B_1)
                        ////    _E4438C.Mode_Initialize(Modulation.CDMA, waveform_name);
                        ////else if (WhichBand == BandList.B_2)
                        ////    _E4438C.Mode_Initialize(Modulation.EVDO, waveform_name);
                        ////else
                        ////    throw new Exception("No this band");

                        ////_E4438C.SetFrequency(dblFreq);
                        ////_E4438C.SetPower(CDMA_Icc + TestSetting.LOSS_SRC[dblFreq]);
                        ////_E4438C.SetModOutput(Output.ON);
                        ////_E4438C.SetOutput(Output.ON);
                        ////util.Wait(1000);

                        intDelay = Math.Max(intDelay_SigGen, intDelay_MXA);
                        if (WhichBand == BandList.B_1)
                        {
                            _MXA_N9020A.Config__CDMA_EVM(dblFreq);
                            util.Wait(intDelay * 3);

                            dblResult[11] = _MXA_N9020A.Get_CDMA_EVM_Result();
                        }
                        else if (WhichBand == BandList.B_2)
                        {
                            _MXA_N9020A.Config__EVDO_EVM(dblFreq);
                            util.Wait(intDelay * 3);

                            dblResult[11] = _MXA_N9020A.Get_EVDO_EVM_Result();
                        }
                        else
                            throw new Exception("No this band");

                    }
                    else
                    {
                        dblResult[11] = 0;
                    }
                    // Update_Grid
                    DataRow drNew = null;
                    if (WhichBand == BandList.B_1)
                        drNew = dtCDMA.NewRow();
                    else if (WhichBand == BandList.B_2)
                        drNew = dtEVDO.NewRow();
                    else
                        throw new Exception("No this band");

                    DataRow drNewtmp = dtCDMATMP.NewRow();
                    drNewtmp[0] = drNew[0] = intTestID++;
                    for (int i = 0; i < dblResult.Count(); i++)
                    {
                        drNew[i + 1] = drNewtmp[i + 1] = Math.Round(dblResult[i], 2);
                    }
                    dtCDMATMP.Rows.Add(drNewtmp);

                    this.Invoke((MethodInvoker)(delegate
                    {
                        if (WhichBand == BandList.B_1)
                            dtCDMA.Rows.Add(drNew);
                        else if (WhichBand == BandList.B_2)
                            dtEVDO.Rows.Add(drNew);
                        else
                            throw new Exception("No this band");

                        dgvSweepResult.Refresh();
                        Application.DoEvents();
                    }));

                }
            }

            #endregion --- CDMA/EVDO Test ---

            #region --- Worst acp report ---

            foreach (double dblFreq in TestSetting.FREQLIST)
            {
                DataRow drmax = null;
                if (WhichBand == BandList.B_1)
                    drmax = dtCDMA.NewRow();
                else if (WhichBand == BandList.B_2)
                    drmax = dtEVDO.NewRow();
                else
                    throw new Exception("No this band");

                var acp_n5 = from x in dtCDMATMP.AsEnumerable()
                             where x.Field<double>("Frequency (MHz)") == dblFreq
                             select x.Field<double>("ACP -5MHz(dB)");

                var acp_p5 = from x in dtCDMATMP.AsEnumerable()
                             where x.Field<double>("Frequency (MHz)") == dblFreq
                             select x.Field<double>("ACP +5MHz(dB)");

                var acp_n10 = from x in dtCDMATMP.AsEnumerable()
                              where x.Field<double>("Frequency (MHz)") == dblFreq
                              select x.Field<double>("ACP -10MHz(dB)");

                var acp_p10 = from x in dtCDMATMP.AsEnumerable()
                              where x.Field<double>("Frequency (MHz)") == dblFreq
                              select x.Field<double>("ACP +10MHz(dB)");

                var evm = from x in dtCDMATMP.AsEnumerable()
                          where x.Field<double>("Frequency (MHz)") == dblFreq
                          select x.Field<double>("EVM(%)");

                try
                {
                    drmax[8] = acp_n5.ToList<double>().Max();
                    drmax[9] = acp_p5.ToList<double>().Max();
                    drmax[10] = acp_n10.ToList<double>().Max();
                    drmax[11] = acp_p10.ToList<double>().Max();
                    drmax[12] = evm.ToList<double>().Max();
                }
                catch
                {
                    drmax[8] = 0;
                    drmax[9] = 0;
                    drmax[10] = 0;
                    drmax[11] = 0;
                    drmax[12] = 0;
                }

                this.Invoke((MethodInvoker)(delegate
                {
                    if (WhichBand == BandList.B_1)
                        dtCDMA.Rows.Add(drmax);
                    else if (WhichBand == BandList.B_2)
                        dtEVDO.Rows.Add(drmax);
                    else
                        throw new Exception("No this band");

                    dgvSweepResult.Refresh();
                    Application.DoEvents();
                }));

            }
            #endregion *** Worst acp report ***

            #region --- After Test ---

            AfterTest();
            _E4438C.SetOutput(Output.OFF);
            _PS_66332A.SetOutput(Output.OFF);
            //_PS_E3631A.SetOutput(Output.OFF);
            _Arb_33522A.SetOutput(Output.OFF, Arb_Channel.Channel_1);
            _Arb_33522A.SetOutput(Output.OFF, Arb_Channel.Channel_2);

            #endregion --- After Test ---

        }

        #endregion --- SH_2 Test ---
        #region --- SH_3 Test ---

        void CWLB_Test_SH_3()
        {
            #region --- Variable Define ---

            dicVramp.Clear();
            int intTestID = 1;
            double[] dblResult = new double[18];

            DataTable dtCWLBTMP = new DataTable();
            dtCWLBTMP = dtCWLB.Clone();

            if (!cbxKeepPrevious.Checked)
            {
                this.Invoke((MethodInvoker)(delegate
                {
                    dtCWLB.Clear();
                    dgvSweepResult.Refresh();
                }));
            }

            #endregion --- Variable Define ---

            #region --- Initialize ---
            BeforeTest();
            SetVCC(TestSetting.LEVEL_VCC);
            SetTXEnable(TestSetting.LEVEL_TXEN);
            SetGPCTRL(TestSetting.LEVEL_GPCTRL);

            _E4438C.Initialize();
            _PM_N1913A.Initialize(rbnDisplayON.Checked);
            _MXA_N9020A.Initialize(rbnDisplayON.Checked);
            _E4438C.SetOutput(Output.ON);

            DateTime t_Start = DateTime.Now;
    
            string Content = "Make sure everything is setup for TX LB testing \r\n" +
                            "  1) Connect rf source cable to TX LB \r\n" +
                            "  2) Set Control box to TX LB mode \r\n" +
                            "  3) Connect / Change LB highpass filter";

            this.Invoke((MethodInvoker)(delegate
            {
                MessageBox.Show(this, Content, "GMSK LB Testing", MessageBoxButtons.OK);
                Application.DoEvents();
                wait_2_start(t_Start);
            }));

            #endregion --- Initialize ---

            #region --- Vramp Sweep ---
            for (double dblVramp = TestSetting.LEVEL_START; dblVramp < TestSetting.LEVEL_STOP + 0.01; dblVramp += TestSetting.LEVEL_STEP)
            {
                if (StopTest) break;

                SetVramp(dblVramp);

                foreach (double dblFreq in TestSetting.FREQLIST)
                {
                    if (StopTest) break;
                    _E4438C.SetFrequency(dblFreq);
                    _E4438C.SetPower(TestSetting.LEVEL_PIN_VRAMP + TestSetting.LOSS_SRC[dblFreq]);

                    // Frequency
                    dblResult[0] = dblFreq;

                    // Vramp
                    dblResult[1] = dblVramp;

                    // Pout
                    _PM_N1913A.Configure__Average_Pulse_Power_with_Duty_Cycle(dblFreq, 25,256);
                    util.Wait(intDelay_PowerMeter * intDelay_N1913A_Count);
                    dblResult[2] = _PM_N1913A.GetPowerResult() + TestSetting.LOSS_MSR_POUT[dblFreq];

                    // ICC
                    dblResult[3] = _PS_66332A.High_Current();
                    dblResult[3] = dblResult[3] * 1000;

                    // PAE
                    dblResult[4] = (Math.Pow(10, ((dblResult[2] - 30) / 10))) / TestSetting.LEVEL_VCC / dblResult[3] * 1000;
                    dblResult[4] = dblResult[4] * 100;

                    // harmonic 
                    for (int i = 2; i <= 6; i++)
                    {
                        _MXA_N9020A.SetFrequency(i * dblFreq);
                        _MXA_N9020A.SetAttenuattor(0);

                        util.Wait(intDelay_MXA);

                        dblResult[3 + i] = _MXA_N9020A.Get_CW_PowerResult() + TestSetting.LOSS_MSR_FILTER_LB[i * dblFreq];
                    }
                    // Extra harmonic 
                    if (ext_har)
                    {
                        for (int i = 7; i <= 14; i++)
                        {
                            _MXA_N9020A.SetFrequency(i * dblFreq);
                            util.Wait(intDelay_MXA);
                            dblResult[3 + i] = _MXA_N9020A.Get_CW_PowerResult() + TestSetting.LOSS_MSR_FILTER_LB[6000];
                        }
                    }
                    // Update_Grid
                    DataRow drNew = dtCWLB.NewRow();
                    DataRow drNewTmp = dtCWLBTMP.NewRow();
                    drNewTmp[0] = drNew[0] = intTestID++;
                    for (int i = 0; i < dblResult.Count(); i++)
                    {
                        drNewTmp[i + 1] = drNew[i + 1] = Math.Round(dblResult[i], 2);
                    }
                    dtCWLBTMP.Rows.Add(drNewTmp);
                    this.Invoke((MethodInvoker)(delegate
                    {
                        dtCWLB.Rows.Add(drNew);
                        dgvSweepResult.Refresh();
                        Application.DoEvents();
                    }));
                } // Frequency loop
            }   // Vramp loop
            #endregion Vramp Sweep

            #region --- Power Servo ---
            foreach (double dblFreq in TestSetting.FREQLIST)
            {
                if (StopTest) break;

                int Count = 0;
                _E4438C.SetFrequency(dblFreq);
                _E4438C.SetPower(TestSetting.LEVEL_PIN_VRAMP + TestSetting.LOSS_SRC[dblFreq]);

                #region Search power
                double LoopResult_Low;
                double LoopResult_High;
                double LoopResult;
                double Slope_mV;

                this.SetVramp(1.2);
                _PM_N1913A.Configure__Average_Pulse_Power_with_Duty_Cycle(dblFreq, 25, 256);
                util.Wait(intDelay_PowerMeter * intDelay_N1913A_Count);
                LoopResult_Low = _PM_N1913A.GetPowerResult();

                this.SetVramp(1.6);
                _PM_N1913A.Configure__Average_Pulse_Power_with_Duty_Cycle(dblFreq, 25, 256);
                util.Wait(intDelay_PowerMeter * intDelay_N1913A_Count);
                LoopResult_High = _PM_N1913A.GetPowerResult();

                Slope_mV = (LoopResult_High - LoopResult_Low) / 200;

                LoopResult = LoopResult_Low + TestSetting.LOSS_MSR_POUT[dblFreq];
                dblRatedVramp = 1.2 + (TestSetting.TARGET_POUT_CWLB - LoopResult) / Slope_mV / 1000;

                if (dblRatedVramp < 0.8 || dblRatedVramp > 1.8) dblRatedVramp = 1.8;

                while (Math.Abs(LoopResult - TestSetting.TARGET_POUT_CWLB) > 0.05 && dblRatedVramp < 1.8 && dblRatedVramp > 0.8)
                {
                    this.SetVramp(dblRatedVramp);
                    util.Wait(intDelay_PowerMeter * intDelay_N1913A_Count);
                    LoopResult = _PM_N1913A.GetPowerResult() + TestSetting.LOSS_MSR_POUT[dblFreq];
                    dblRatedVramp = dblRatedVramp + (TestSetting.TARGET_POUT_CWLB - LoopResult) / Slope_mV / 1000;
                    Count++;
                    if (Count > 20) break;
                }
                #endregion Search power

                // Frequency
                dblResult[0] = dblFreq;
                // Rated Vramp
                dblResult[1] = dblRatedVramp;
                if (!dicVramp.ContainsKey(dblFreq)) dicVramp.Add(dblFreq, Math.Round(dblRatedVramp, 2));
                //Rated Pout
                dblResult[2] = LoopResult;

                // Rated ICC
                dblResult[3] = _PS_66332A.High_Current();
                dblResult[3] = dblResult[3] * 1000;

                // Rated PAE
                dblResult[4] = (Math.Pow(10, ((dblResult[2] - 30) / 10))) / TestSetting.LEVEL_VCC / dblResult[3] * 1000;
                dblResult[4] = dblResult[4] * 100;

                // Rated harmonic 
                for (int i = 2; i <= 6; i++)
                {
                    _MXA_N9020A.SetFrequency(i * dblFreq);
                    _MXA_N9020A.SetAttenuattor(0);
                    util.Wait(intDelay_MXA);
                    dblResult[3 + i] = _MXA_N9020A.Get_CW_PowerResult() + TestSetting.LOSS_MSR_FILTER_LB[i * dblFreq];
                }
                // Extra harmonic 
                if (ext_har)
                {
                    for (int i = 7; i <= 14; i++)
                    {
                        _MXA_N9020A.SetFrequency(i * dblFreq);
                        util.Wait(intDelay_MXA);
                        dblResult[3 + i] = _MXA_N9020A.Get_CW_PowerResult() + TestSetting.LOSS_MSR_FILTER_LB[6000];
                    }
                }
                // Update_Grid
                DataRow drNew = dtCWLB.NewRow();
                DataRow drNewTmp = dtCWLBTMP.NewRow();
                drNewTmp[0] = drNew[0] = intTestID++;
                for (int i = 0; i < dblResult.Count(); i++)
                {
                    drNewTmp[i + 1] = drNew[i + 1] = Math.Round(dblResult[i], 2);
                }
                dtCWLBTMP.Rows.Add(drNewTmp);
                this.Invoke((MethodInvoker)(delegate
                {
                    dtCWLB.Rows.Add(drNew);
                    dgvSweepResult.Refresh();
                    Application.DoEvents();
                }));

            } // Frequency loop for power servo
            #endregion Power Servo

            #region --- Worst harmonic report ---

            foreach (double dblFreq in TestSetting.FREQLIST)
            {
                if (StopTest) break;

                DataRow drmax = dtCWLB.NewRow();
                drmax[0] = intTestID++;
                drmax[1] = dblFreq;

                var _2fo = from x in dtCWLBTMP.AsEnumerable()
                           where x.Field<double>("Frequency(MHz)") == dblFreq && double.Parse(x.Field<string>("Vramp(V)")) <= dicVramp[dblFreq]
                           select x.Field<double>("2fo(dBm)");

                var _3fo = from x in dtCWLBTMP.AsEnumerable()
                           where x.Field<double>("Frequency(MHz)") == dblFreq && double.Parse(x.Field<string>("Vramp(V)")) <= dicVramp[dblFreq]
                           select x.Field<double>("3fo(dBm)");

                var _4fo = from x in dtCWLBTMP.AsEnumerable()
                           where x.Field<double>("Frequency(MHz)") == dblFreq && double.Parse(x.Field<string>("Vramp(V)")) <= dicVramp[dblFreq]
                           select x.Field<double>("4fo(dBm)");

                var _5fo = from x in dtCWLBTMP.AsEnumerable()
                           where x.Field<double>("Frequency(MHz)") == dblFreq && double.Parse(x.Field<string>("Vramp(V)")) <= dicVramp[dblFreq]
                           select x.Field<double>("5fo(dBm)");

                var _6fo = from x in dtCWLBTMP.AsEnumerable()
                           where x.Field<double>("Frequency(MHz)") == dblFreq && double.Parse(x.Field<string>("Vramp(V)")) <= dicVramp[dblFreq]
                           select x.Field<double>("6fo(dBm)");

                try
                {
                    drmax[6] = _2fo.ToList<double>().Max();
                    drmax[7] = _3fo.ToList<double>().Max();
                    drmax[8] = _4fo.ToList<double>().Max();
                    drmax[9] = _5fo.ToList<double>().Max();
                    drmax[10] = _6fo.ToList<double>().Max();
                }
                catch
                {
                    drmax[6] = 0;
                    drmax[7] = 0;
                    drmax[8] = 0;
                    drmax[9] = 0;
                    drmax[10] = 0;
                }

                this.Invoke((MethodInvoker)(delegate
                {
                    dtCWLB.Rows.Add(drmax);
                    dgvSweepResult.Refresh();
                    Application.DoEvents();
                }));

            }
            #endregion *** Worst harmonic report ***

            #region --- After Test ---
            // After Test
            AfterTest();
            _E4438C.SetOutput(Output.OFF);
            _PS_66332A.SetOutput(Output.OFF);
            _Arb_33522A.SetOutput(Output.OFF, Arb_Channel.Channel_1);
            _Arb_33522A.SetOutput(Output.OFF, Arb_Channel.Channel_2);

            #endregion --- After Test ---


        }

        void CWHB_Test_SH_3()
        {
            #region --- Variable Define ---
            dicVramp.Clear();
            int intTestID = 1;
            double[] dblResult = new double[11];

            DataTable dtCWHBTMP = new DataTable();
            dtCWHBTMP = dtCWHB.Clone();

            if (!cbxKeepPrevious.Checked)
            {
                this.Invoke((MethodInvoker)(delegate
                {
                    dtCWHB.Clear();
                    dgvSweepResult.Refresh();
                }));
            }
            #endregion --- Variable Define ---

            #region --- Initialize ---
            BeforeTest();
            SetVCC(TestSetting.LEVEL_VCC);
            SetTXEnable(TestSetting.LEVEL_TXEN);
            SetGPCTRL(TestSetting.LEVEL_GPCTRL);
            _E4438C.Initialize();
            _PM_N1913A.Initialize(rbnDisplayON.Checked);
            _MXA_N9020A.Initialize(rbnDisplayON.Checked);
            _E4438C.SetOutput(Output.ON);

            DateTime t_Start = DateTime.Now;

            string Content = "Make sure everything is setup for TX HB testing \r\n" +
                            "  1) Connect rf source cable to TX HB \r\n" +
                            "  2) Set Control box to TX HB mode \r\n" +
                            "  3) Connect / Change HB highpass filter";

            this.Invoke((MethodInvoker)(delegate
            {
                MessageBox.Show(this, Content, "GMSK HB Testing", MessageBoxButtons.OK);
                Application.DoEvents();
                wait_2_start(t_Start);
            }));

            #endregion --- Initialize ---

            #region --- Vramp Sweep ---
            for (double dblVramp = TestSetting.LEVEL_START; dblVramp < TestSetting.LEVEL_STOP + 0.01; dblVramp += TestSetting.LEVEL_STEP)
            {
                if (StopTest) break;
                SetVramp(dblVramp);

                foreach (double dblFreq in TestSetting.FREQLIST)
                {
                    if (StopTest) break;
                    _E4438C.SetFrequency(dblFreq);
                    _E4438C.SetPower(TestSetting.LEVEL_PIN_VRAMP + TestSetting.LOSS_SRC[dblFreq]);

                    // Frequency
                    dblResult[0] = dblFreq;

                    // Vramp
                    dblResult[1] = dblVramp;

                    // Pout
                    _PM_N1913A.Configure__Average_Pulse_Power_with_Duty_Cycle(dblFreq, 25, 256);

                    util.Wait(intDelay_PowerMeter * intDelay_N1913A_Count);
                    dblResult[2] = _PM_N1913A.GetPowerResult() + TestSetting.LOSS_MSR_POUT[dblFreq];

                    // ICC
                    dblResult[3] = _PS_66332A.High_Current();
                    dblResult[3] = dblResult[3] * 1000;

                    // PAE
                    dblResult[4] = (Math.Pow(10, ((dblResult[2] - 30) / 10))) / TestSetting.LEVEL_VCC / dblResult[3] * 1000;
                    dblResult[4] = dblResult[4] * 100;

                    // harmonic 
                    for (int i = 2; i <= 3; i++)
                    {
                        _MXA_N9020A.SetFrequency(i * dblFreq);
                        _MXA_N9020A.SetAttenuattor(0);

                        util.Wait(intDelay_MXA);

                        dblResult[3 + i] = _MXA_N9020A.Get_CW_PowerResult() + TestSetting.LOSS_MSR_FILTER_HB[i * dblFreq];
                    }
                    // Extra harmonic 
                    if (ext_har)
                    {
                        for (int i = 4; i <= 7; i++)
                        {
                            _MXA_N9020A.SetFrequency(i * dblFreq);
                            util.Wait(intDelay_MXA);
                            dblResult[3 + i] = _MXA_N9020A.Get_CW_PowerResult() + TestSetting.LOSS_MSR_FILTER_HB[6000];
                        }
                    }
                    // Update_Grid
                    DataRow drNew = dtCWHB.NewRow();
                    DataRow drNewTmp = dtCWHBTMP.NewRow();
                    drNewTmp[0] = drNew[0] = intTestID++;
                    for (int i = 0; i < dblResult.Count(); i++)
                    {
                        drNewTmp[i + 1] = drNew[i + 1] = Math.Round(dblResult[i], 2);
                    }
                    dtCWHBTMP.Rows.Add(drNewTmp);
                    this.Invoke((MethodInvoker)(delegate
                    {
                        dtCWHB.Rows.Add(drNew);
                        dgvSweepResult.Refresh();
                        Application.DoEvents();
                    }));

                }   // Frequency loop
            }  // Vramp loop
            #endregion *** Vramp Sweep ***

            #region --- Power Servo ---
            foreach (double dblFreq in TestSetting.FREQLIST)
            {
                if (StopTest) break;
                int Count = 0;
                _E4438C.SetFrequency(dblFreq);
                _E4438C.SetPower(TestSetting.LEVEL_PIN_VRAMP + TestSetting.LOSS_SRC[dblFreq]);

                #region Search power
                double LoopResult_Low;
                double LoopResult_High;
                double LoopResult;
                double Slope_mV;

                this.SetVramp(1.2);
                _PM_N1913A.Configure__Average_Pulse_Power_with_Duty_Cycle(dblFreq, 25, 256);
                util.Wait(intDelay_PowerMeter * intDelay_N1913A_Count);
                LoopResult_Low = _PM_N1913A.GetPowerResult();

                this.SetVramp(1.6);
                _PM_N1913A.Configure__Average_Pulse_Power_with_Duty_Cycle(dblFreq, 25, 256);
                util.Wait(intDelay_PowerMeter * intDelay_N1913A_Count);
                LoopResult_High = _PM_N1913A.GetPowerResult();

                Slope_mV = (LoopResult_High - LoopResult_Low) / 200;

                LoopResult = LoopResult_Low + TestSetting.LOSS_MSR_POUT[dblFreq];
                dblRatedVramp = 1.2 + (TestSetting.TARGET_POUT_CWHB - LoopResult) / Slope_mV / 1000;

                if (dblRatedVramp < 0.8 || dblRatedVramp > 1.8) dblRatedVramp = 1.8;

                while (Math.Abs(LoopResult - TestSetting.TARGET_POUT_CWHB) > 0.05 && dblRatedVramp < 1.8 && dblRatedVramp > 0.8)
                {
                    this.SetVramp(dblRatedVramp);
                    util.Wait(intDelay_PowerMeter * intDelay_N1913A_Count);
                    LoopResult = _PM_N1913A.GetPowerResult() + TestSetting.LOSS_MSR_POUT[dblFreq];
                    dblRatedVramp = dblRatedVramp + (TestSetting.TARGET_POUT_CWHB - LoopResult) / Slope_mV / 1000;
                    Count++;
                    if (Count > 20) break;
                }
                #endregion Search power

                // Frequency
                dblResult[0] = dblFreq;
                // Rated Vramp
                dblResult[1] = dblRatedVramp;
                if (!dicVramp.ContainsKey(dblFreq)) dicVramp.Add(dblFreq, Math.Round(dblRatedVramp, 2));
                //Rated Pout
                dblResult[2] = LoopResult;

                // Rated ICC
                dblResult[3] = _PS_66332A.High_Current();
                dblResult[3] = dblResult[3] * 1000;

                // Rated PAE
                dblResult[4] = (Math.Pow(10, ((dblResult[2] - 30) / 10))) / TestSetting.LEVEL_VCC / dblResult[3] * 1000;
                dblResult[4] = dblResult[4] * 100;

                // Rated harmonic 
                for (int i = 2; i <= 3; i++)
                {
                    _MXA_N9020A.SetFrequency(i * dblFreq);
                    _MXA_N9020A.SetAttenuattor(0);
                    util.Wait(intDelay_MXA);
                    dblResult[3 + i] = _MXA_N9020A.Get_CW_PowerResult() + TestSetting.LOSS_MSR_FILTER_HB[i * dblFreq];
                }
                // Extra harmonic 
                if (ext_har)
                {
                    for (int i = 4; i <= 7; i++)
                    {
                        _MXA_N9020A.SetFrequency(i * dblFreq);
                        util.Wait(intDelay_MXA);
                        dblResult[3 + i] = _MXA_N9020A.Get_CW_PowerResult() + TestSetting.LOSS_MSR_FILTER_HB[6000];
                    }
                }
                // Update_Grid
                DataRow drNew = dtCWHB.NewRow();
                DataRow drNewTmp = dtCWHBTMP.NewRow();
                drNewTmp[0] = drNew[0] = intTestID++;
                for (int i = 0; i < dblResult.Count(); i++)
                {
                    drNewTmp[i + 1] = drNew[i + 1] = Math.Round(dblResult[i], 2);
                }
                dtCWHBTMP.Rows.Add(drNewTmp);
                this.Invoke((MethodInvoker)(delegate
                {
                    dtCWHB.Rows.Add(drNew);
                    dgvSweepResult.Refresh();
                    Application.DoEvents();
                }));

            } // Frequency loop for power servo
            #endregion Power Servo

            #region --- Worst harmonic report ---

            foreach (double dblFreq in TestSetting.FREQLIST)
            {
                if (StopTest) break;
                DataRow drmax = dtCWHB.NewRow();
                drmax[0] = intTestID++;
                drmax[1] = dblFreq;

                var _2fo = from x in dtCWHBTMP.AsEnumerable()
                           where x.Field<double>("Frequency(MHz)") == dblFreq && double.Parse(x.Field<string>("Vramp(V)")) <= dicVramp[dblFreq]
                           select x.Field<double>("2fo(dBm)");

                var _3fo = from x in dtCWHBTMP.AsEnumerable()
                           where x.Field<double>("Frequency(MHz)") == dblFreq && double.Parse(x.Field<string>("Vramp(V)")) <= dicVramp[dblFreq]
                           select x.Field<double>("3fo(dBm)");
                try
                {
                    drmax[6] = _2fo.ToList<double>().Max();
                    drmax[7] = _3fo.ToList<double>().Max();
                }
                catch
                {
                    drmax[6] = 0;
                    drmax[7] = 0;
                }

                this.Invoke((MethodInvoker)(delegate
                {
                    dtCWHB.Rows.Add(drmax);
                    dgvSweepResult.Refresh();
                    Application.DoEvents();
                }));
            }
            #endregion *** Worst harmonic report ***

            #region --- After Test ---
            // After Test
            AfterTest();
            _E4438C.SetOutput(Output.OFF);
            _PS_66332A.SetOutput(Output.OFF);
            _Arb_33522A.SetOutput(Output.OFF, Arb_Channel.Channel_1);
            _Arb_33522A.SetOutput(Output.OFF, Arb_Channel.Channel_2);

            #endregion --- After Test ---

        }

        void LTETDD_B38_TEST_SH_3()
        {
            #region --- Variable Define ---

            Dictionary<double, double> lastLTETDDPin = new Dictionary<double, double>();
            int intDelay;
            int intTestID = 1;
            double Pin_Start = -10;
            double[] dblResult = new double[14];
            string waveform_name = "";

            this.Invoke((MethodInvoker)(delegate
            {
                waveform_name = cbxWaveform.SelectedItem.ToString();
            }));


            DataTable dtLTETDDTMP_B38 = new DataTable();
            dtLTETDDTMP_B38 = dtLTETDD_B38.Clone();

            if (!cbxKeepPrevious.Checked)
            {
                this.Invoke((MethodInvoker)(delegate
                {
                    dtLTETDD_B38.Clear();
                    dgvSweepResult.Refresh();
                }));
            }

            #endregion --- Variable Define ---

            #region --- Initialize ---

            BeforeTest();

            SetVCC(TestSetting.LEVEL_VCC);
            SetVEN(Arb_Channel.Channel_2, TestSetting.LEVEL_TXEN, false);
            SetVEN(Arb_Channel.Channel_1, 0.05, true);
            SetGPCTRL(TestSetting.LEVEL_GPCTRL);


            _MXA_N9020A.Mod_Initialize(Modulation.LTETDD, true);
            _MXA_N9020A.SetFrequency(2496);
            _E4438C.SetFrequency(2496);
            _E4438C.Mode_Initialize(Modulation.LTETDD, waveform_name);

            _E4438C.SetModOutput(Output.ON);
            _E4438C.SetOutput(Output.ON);

            string Content = "Make sure everything is setup for LTETDD B38 testing \r\n" +
                              "  1) Connect rf source cable to RFIN_B38 \r\n" +
                              "  2) Connect ANT with 20dB pad to RFOUT_B38 \r\n" +
                              "  3) Set Control box to B38 mode \r\n" +
                              "  4) Connect MXA to coupler out directly";

            this.Invoke((MethodInvoker)(delegate
            {
                MessageBox.Show(this, Content, "LTETDD B38 Testing", MessageBoxButtons.OK);
                Application.DoEvents();
            }));

            #endregion --- Initialize ---

            #region --- LTETDD Test ---

            for (double dblPout_Target = TestSetting.LEVEL_START; dblPout_Target <= TestSetting.LEVEL_STOP; dblPout_Target += TestSetting.LEVEL_STEP)
            {
                if (StopTest) break;

                double PoutLL = dblPout_Target - 0.05;
                double PoutUL = dblPout_Target + 0.05;

                foreach (double dblFreq in TestSetting.FREQLIST)
                {
                    if (StopTest) break;

                    double LTETDD_B38_Pout = 0;
                    double LTETDD_B38_Pin = 0;
                    double LTETDD_B38_Icc = 0;
                    double dblPin = Pin_Start;
                    double[] dblACPResult = new double[5];

                    if (lastLTETDDPin.ContainsKey(dblFreq)) LTETDD_B38_Pin = dblPin = Pin_Start = lastLTETDDPin[dblFreq] + TestSetting.LEVEL_STEP;
                    _E4438C.SetFrequency(dblFreq);
                    _E4438C.SetPower(Pin_Start + TestSetting.LOSS_SRC[dblFreq]);

                    _MXA_N9020A.Config__LTETDD_CHP(dblFreq);
                    intDelay = Math.Max(intDelay_SigGen, intDelay_MXA) * 10;
                    util.Wait(intDelay);

                    LTETDD_B38_Pout = _MXA_N9020A.Get_LTETDD_CHP_Result();
                    LTETDD_B38_Pout += TestSetting.LOSS_MSR_THROUGH[dblFreq];

                    // Freqquency
                    dblResult[0] = dblFreq;

                    // Target Power
                    dblResult[1] = dblPout_Target;

                    //Pout
                    while (LTETDD_B38_Pout <= PoutLL || LTETDD_B38_Pout >= PoutUL)
                    {
                        dblPin = dblPin + dblPout_Target - LTETDD_B38_Pout;
                        if (dblPin > 5)
                            break;

                        _E4438C.SetPower(dblPin + TestSetting.LOSS_SRC[dblFreq]);
                        util.Wait(intDelay);
                        LTETDD_B38_Pout = _MXA_N9020A.Get_LTETDD_CHP_Result();
                        LTETDD_B38_Pout += TestSetting.LOSS_MSR_THROUGH[dblFreq];
                        LTETDD_B38_Pin = dblPin;
                    }
                    dblResult[2] = LTETDD_B38_Pout;

                    // Pin
                    dblResult[3] = LTETDD_B38_Pin;
                    if (lastLTETDDPin.ContainsKey(dblFreq))
                        lastLTETDDPin[dblFreq] = LTETDD_B38_Pin;
                    else
                        lastLTETDDPin.Add(dblFreq, LTETDD_B38_Pin);

                    // Gain
                    dblResult[4] = LTETDD_B38_Pout - LTETDD_B38_Pin;

                    // Icc
                    LTETDD_B38_Icc = _PS_66332A.High_Current();
                    dblResult[5] = LTETDD_B38_Icc * 1000;
                    // PAE
                    dblResult[6] = 100 * (Math.Pow(10, ((LTETDD_B38_Pout - 30) / 10))) / TestSetting.LEVEL_VCC / LTETDD_B38_Icc;

                    //ACP E_ULTRA
                    _MXA_N9020A.Config__LTETDD_ACP_EULTRA(dblFreq);
                    intDelay = Math.Max(intDelay_SigGen, intDelay_MXA);
                    util.Wait(intDelay * 3);

                    dblACPResult = _MXA_N9020A.Get_LTETDD_ACP_EULTRA_Result();

                    dblResult[7] = dblACPResult[1];     // -10MHz
                    dblResult[8] = dblACPResult[2];     // +10MHz

                    //ACP ULTRA
                    _MXA_N9020A.Config__LTETDD_ACP_ULTRA(dblFreq);
                    intDelay = Math.Max(intDelay_SigGen, intDelay_MXA);
                    util.Wait(intDelay * 3);

                    dblACPResult = _MXA_N9020A.Get_LTETDD_ACP_ULTRA_Result();

                    dblResult[9] = dblACPResult[1];     // -5.8MHz
                    dblResult[10] = dblACPResult[2];     // -5.8MHz
                    dblResult[11] = dblACPResult[3];     // -7.4MHz
                    dblResult[12] = dblACPResult[4];     // -7.4MHz

                    // EVM
                    if (!cbxBypassEVM.Checked)
                    {
                        _MXA_N9020A.Config__LTETDD_EVM(dblFreq);
                        intDelay = Math.Max(intDelay_SigGen, intDelay_MXA);
                        util.Wait(intDelay * 3);

                        dblResult[13] = _MXA_N9020A.Get_LTETDD_EVM_Result();
                    }
                    else
                    {
                        dblResult[13] = 0;
                    }
                    // Update_Grid
                    DataRow drNew = dtLTETDD_B38.NewRow();
                    DataRow drNewtmp = dtLTETDDTMP_B38.NewRow();
                    drNewtmp[0] = drNew[0] = intTestID++;
                    for (int i = 0; i < dblResult.Count(); i++)
                    {
                        drNew[i + 1] = drNewtmp[i + 1] = Math.Round(dblResult[i], 2);
                    }
                    dtLTETDDTMP_B38.Rows.Add(drNewtmp);

                    this.Invoke((MethodInvoker)(delegate
                    {
                        dtLTETDD_B38.Rows.Add(drNew);
                        dgvSweepResult.Refresh();
                        Application.DoEvents();
                    }));
                }
            }

            #endregion --- LTETDD_B38 Test ---

            #region --- Worst acp report ---

            foreach (double dblFreq in TestSetting.FREQLIST)
            {
                DataRow drmax = dtLTETDD_B38.NewRow();
                drmax[0] = intTestID++;
                drmax[1] = dblFreq;

                var acp_n10 = from x in dtLTETDDTMP_B38.AsEnumerable()
                             where x.Field<double>("Frequency (MHz)") == dblFreq
                             select x.Field<double>("ACP_EULTRA -10MHz(dB)");

                var acp_p10 = from x in dtLTETDDTMP_B38.AsEnumerable()
                             where x.Field<double>("Frequency (MHz)") == dblFreq
                             select x.Field<double>("ACP_EULTRA +10MHz(dB)");

                var acp_n58 = from x in dtLTETDDTMP_B38.AsEnumerable()
                              where x.Field<double>("Frequency (MHz)") == dblFreq
                              select x.Field<double>("ACP_ULTRA -5.8MHz(dB)");

                var acp_p58 = from x in dtLTETDDTMP_B38.AsEnumerable()
                              where x.Field<double>("Frequency (MHz)") == dblFreq
                              select x.Field<double>("ACP_ULTRA +5.8MHz(dB)");

                var acp_n74 = from x in dtLTETDDTMP_B38.AsEnumerable()
                              where x.Field<double>("Frequency (MHz)") == dblFreq
                              select x.Field<double>("ACP_ULTRA -7.4MHz(dB)");

                var acp_p74 = from x in dtLTETDDTMP_B38.AsEnumerable()
                              where x.Field<double>("Frequency (MHz)") == dblFreq
                              select x.Field<double>("ACP_ULTRA +7.4MHz(dB)");

                var evm = from x in dtLTETDDTMP_B38.AsEnumerable()
                          where x.Field<double>("Frequency (MHz)") == dblFreq
                          select x.Field<double>("EVM(%)");

                try
                {
                    drmax[8] = acp_n10.ToList<double>().Max();
                    drmax[9] = acp_p10.ToList<double>().Max();
                    drmax[10] = acp_n58.ToList<double>().Max();
                    drmax[11] = acp_p58.ToList<double>().Max();
                    drmax[12] = acp_n74.ToList<double>().Max();
                    drmax[13] = acp_p74.ToList<double>().Max();
                    drmax[14] = evm.ToList<double>().Max();
                }
                catch
                {
                    drmax[8] = 0;
                    drmax[9] = 0;
                    drmax[10] = 0;
                    drmax[11] = 0;
                    drmax[12] = 0;
                    drmax[13] = 0;
                    drmax[14] = 0;
                }

                this.Invoke((MethodInvoker)(delegate
                {
                    dtLTETDD_B38.Rows.Add(drmax);
                    dgvSweepResult.Refresh();
                    Application.DoEvents();
                }));

            }
            #endregion *** Worst acp report ***

            #region --- After Test ---

            AfterTest();
            _E4438C.SetOutput(Output.OFF);
            _PS_66332A.SetOutput(Output.OFF);
            //_PS_E3631A.SetOutput(Output.OFF);
            _Arb_33522A.SetOutput(Output.OFF, Arb_Channel.Channel_1);
            _Arb_33522A.SetOutput(Output.OFF, Arb_Channel.Channel_2);

            #endregion --- After Test ---
        }

        void LTETDD_B40_TEST_SH_3()
        {
            #region --- Variable Define ---

            Dictionary<double, double> lastLTETDDPin = new Dictionary<double, double>();
            int intDelay;
            int intTestID = 1;
            double Pin_Start = -10;
            double[] dblResult = new double[14];
            string waveform_name = "";

            this.Invoke((MethodInvoker)(delegate
            {
                waveform_name = cbxWaveform.SelectedItem.ToString();
            }));


            DataTable dtLTETDDTMP_B40 = new DataTable();
            dtLTETDDTMP_B40 = dtLTETDD_B40.Clone();

            if (!cbxKeepPrevious.Checked)
            {
                this.Invoke((MethodInvoker)(delegate
                {
                    dtLTETDD_B40.Clear();
                    dgvSweepResult.Refresh();
                }));
            }
            #endregion --- Variable Define ---

            #region --- Initialize ---

            BeforeTest();

            SetVCC(TestSetting.LEVEL_VCC);
            SetVEN(Arb_Channel.Channel_1, TestSetting.LEVEL_TXEN, false);
            SetVEN(Arb_Channel.Channel_2, 0.05, true);            
            SetGPCTRL(TestSetting.LEVEL_GPCTRL);

            _MXA_N9020A.Mod_Initialize(Modulation.LTETDD, true);
            _MXA_N9020A.SetFrequency(2300);
            _E4438C.SetFrequency(2300);
            _E4438C.Mode_Initialize(Modulation.LTETDD, waveform_name);

            _E4438C.SetModOutput(Output.ON);
            _E4438C.SetOutput(Output.ON);

            string Content = "Make sure everything is setup for LTETDD B40 testing \r\n" +
                              "  1) Connect rf source cable to RFIN_B40 \r\n" +
                              "  2) Connect ANT with 20dB pad to RFOUT_B40 \r\n" +
                              "  3) Set Control box to B40 mode \r\n" +
                              "  4) Connect MXA to coupler out directly";

            this.Invoke((MethodInvoker)(delegate
            {
                MessageBox.Show(this, Content, "LTETDD B40 Testing", MessageBoxButtons.OK);
                Application.DoEvents();
            }));

            #endregion --- Initialize ---

            #region --- LTETDD Test ---

            for (double dblPout_Target = TestSetting.LEVEL_START; dblPout_Target <= TestSetting.LEVEL_STOP; dblPout_Target += TestSetting.LEVEL_STEP)
            {
                if (StopTest) break;

                double PoutLL = dblPout_Target - 0.05;
                double PoutUL = dblPout_Target + 0.05;

                foreach (double dblFreq in TestSetting.FREQLIST)
                {
                    if (StopTest) break;

                    double LTETDD_B40_Pout = 0;
                    double LTETDD_B40_Pin = 0;
                    double LTETDD_B40_Icc = 0;
                    double dblPin = Pin_Start;
                    double[] dblACPResult = new double[5];

                    if (lastLTETDDPin.ContainsKey(dblFreq)) LTETDD_B40_Pin = dblPin = Pin_Start = lastLTETDDPin[dblFreq] + TestSetting.LEVEL_STEP;
                    _E4438C.SetFrequency(dblFreq);
                    _E4438C.SetPower(Pin_Start + TestSetting.LOSS_SRC[dblFreq]);

                    _MXA_N9020A.Config__LTETDD_CHP(dblFreq);
                    intDelay = Math.Max(intDelay_SigGen, intDelay_MXA) * 10;
                    util.Wait(intDelay);

                    LTETDD_B40_Pout = _MXA_N9020A.Get_LTETDD_CHP_Result();
                    LTETDD_B40_Pout += TestSetting.LOSS_MSR_THROUGH[dblFreq];

                    // Freqquency
                    dblResult[0] = dblFreq;

                    // Target Power
                    dblResult[1] = dblPout_Target;

                    //Pout
                    while (LTETDD_B40_Pout <= PoutLL || LTETDD_B40_Pout >= PoutUL)
                    {
                        dblPin = dblPin + dblPout_Target - LTETDD_B40_Pout;
                        if (dblPin > 5)
                            break;

                        _E4438C.SetPower(dblPin + TestSetting.LOSS_SRC[dblFreq]);
                        util.Wait(intDelay);
                        LTETDD_B40_Pout = _MXA_N9020A.Get_LTETDD_CHP_Result();
                        LTETDD_B40_Pout += TestSetting.LOSS_MSR_THROUGH[dblFreq];
                        LTETDD_B40_Pin = dblPin;
                    }
                    dblResult[2] = LTETDD_B40_Pout;

                    // Pin
                    dblResult[3] = LTETDD_B40_Pin;
                    if (lastLTETDDPin.ContainsKey(dblFreq))
                        lastLTETDDPin[dblFreq] = LTETDD_B40_Pin;
                    else
                        lastLTETDDPin.Add(dblFreq, LTETDD_B40_Pin);

                    // Gain
                    dblResult[4] = LTETDD_B40_Pout - LTETDD_B40_Pin;

                    // Icc
                    LTETDD_B40_Icc = _PS_66332A.Max_Current();
                    dblResult[5] = LTETDD_B40_Icc * 1000;
                    // PAE
                    dblResult[6] = 100 * (Math.Pow(10, ((LTETDD_B40_Pout - 30) / 10))) / TestSetting.LEVEL_VCC / LTETDD_B40_Icc;
                    
                    //ACP E_ULTRA
                    _MXA_N9020A.Config__LTETDD_ACP_EULTRA(dblFreq);
                    intDelay = Math.Max(intDelay_SigGen, intDelay_MXA);
                    util.Wait(intDelay * 3);

                    dblACPResult = _MXA_N9020A.Get_LTETDD_ACP_EULTRA_Result();

                    dblResult[7] = dblACPResult[1];     // -10MHz
                    dblResult[8] = dblACPResult[2];     // +10MHz

                    //ACP ULTRA
                    _MXA_N9020A.Config__LTETDD_ACP_ULTRA(dblFreq);
                    intDelay = Math.Max(intDelay_SigGen, intDelay_MXA);
                    util.Wait(intDelay * 3);

                    dblACPResult = _MXA_N9020A.Get_LTETDD_ACP_ULTRA_Result();

                    dblResult[9] = dblACPResult[1];     // -5.8MHz
                    dblResult[10] = dblACPResult[2];     // -5.8MHz
                    dblResult[11] = dblACPResult[3];     // -7.4MHz
                    dblResult[12] = dblACPResult[4];     // -7.4MHz

                    // EVM
                    if (!cbxBypassEVM.Checked)
                    {
                        _MXA_N9020A.Config__LTETDD_EVM(dblFreq);
                        intDelay = Math.Max(intDelay_SigGen, intDelay_MXA);
                        util.Wait(intDelay * 3);

                        dblResult[13] = _MXA_N9020A.Get_LTETDD_EVM_Result();
                    }
                    else
                    {
                        dblResult[13] = 0;
                    }
                    // Update_Grid
                    DataRow drNew = dtLTETDD_B40.NewRow();
                    DataRow drNewtmp = dtLTETDDTMP_B40.NewRow();
                    drNewtmp[0] = drNew[0] = intTestID++;
                    for (int i = 0; i < dblResult.Count(); i++)
                    {
                        drNew[i + 1] = drNewtmp[i + 1] = Math.Round(dblResult[i], 2);
                    }
                    dtLTETDDTMP_B40.Rows.Add(drNewtmp);

                    this.Invoke((MethodInvoker)(delegate
                    {
                        dtLTETDD_B40.Rows.Add(drNew);
                        dgvSweepResult.Refresh();
                        Application.DoEvents();
                    }));
                }
            }

            #endregion --- LTETDD_B40 Test ---

            #region --- Worst acp report ---

            foreach (double dblFreq in TestSetting.FREQLIST)
            {
                DataRow drmax = dtLTETDD_B40.NewRow();
                drmax[0] = intTestID++;
                drmax[1] = dblFreq;

                var acp_n10 = from x in dtLTETDDTMP_B40.AsEnumerable()
                              where x.Field<double>("Frequency (MHz)") == dblFreq
                              select x.Field<double>("ACP_EULTRA -10MHz(dB)");

                var acp_p10 = from x in dtLTETDDTMP_B40.AsEnumerable()
                              where x.Field<double>("Frequency (MHz)") == dblFreq
                              select x.Field<double>("ACP_EULTRA +10MHz(dB)");

                var acp_n58 = from x in dtLTETDDTMP_B40.AsEnumerable()
                              where x.Field<double>("Frequency (MHz)") == dblFreq
                              select x.Field<double>("ACP_ULTRA -5.8MHz(dB)");

                var acp_p58 = from x in dtLTETDDTMP_B40.AsEnumerable()
                              where x.Field<double>("Frequency (MHz)") == dblFreq
                              select x.Field<double>("ACP_ULTRA +5.8MHz(dB)");

                var acp_n74 = from x in dtLTETDDTMP_B40.AsEnumerable()
                              where x.Field<double>("Frequency (MHz)") == dblFreq
                              select x.Field<double>("ACP_ULTRA -7.4MHz(dB)");

                var acp_p74 = from x in dtLTETDDTMP_B40.AsEnumerable()
                              where x.Field<double>("Frequency (MHz)") == dblFreq
                              select x.Field<double>("ACP_ULTRA +7.4MHz(dB)");

                var evm = from x in dtLTETDDTMP_B40.AsEnumerable()
                          where x.Field<double>("Frequency (MHz)") == dblFreq
                          select x.Field<double>("EVM(%)");

                try
                {
                    drmax[8] = acp_n10.ToList<double>().Max();
                    drmax[9] = acp_p10.ToList<double>().Max();
                    drmax[10] = acp_n58.ToList<double>().Max();
                    drmax[11] = acp_p58.ToList<double>().Max();
                    drmax[12] = acp_n74.ToList<double>().Max();
                    drmax[13] = acp_p74.ToList<double>().Max();
                    drmax[14] = evm.ToList<double>().Max();
                }
                catch
                {
                    drmax[8] = 0;
                    drmax[9] = 0;
                    drmax[10] = 0;
                    drmax[11] = 0;
                    drmax[12] = 0;
                    drmax[13] = 0;
                    drmax[14] = 0;
                }

                this.Invoke((MethodInvoker)(delegate
                {
                    dtLTETDD_B40.Rows.Add(drmax);
                    dgvSweepResult.Refresh();
                    Application.DoEvents();
                }));

            }
            #endregion *** Worst acp report ***

            #region --- After Test ---

            AfterTest();
            _E4438C.SetOutput(Output.OFF);
            _PS_66332A.SetOutput(Output.OFF);
            //_PS_E3631A.SetOutput(Output.OFF);
            _Arb_33522A.SetOutput(Output.OFF, Arb_Channel.Channel_1);
            _Arb_33522A.SetOutput(Output.OFF, Arb_Channel.Channel_2);

            #endregion --- After Test ---
        }

        void LTETDD_TEST_SH_3(object Band)
        {
            BandList WhichBand = (BandList)Band;
 
            #region --- Variable Define ---

            Dictionary<double, double> lastLTETDDPin = new Dictionary<double, double>();
            int intDelay;
            int intTestID = 1;
            double Pin_Start = -10;
            double[] dblResult = new double[14];
            string waveform_name = "";

            this.Invoke((MethodInvoker)(delegate
            {
                waveform_name = cbxWaveform.SelectedItem.ToString();
            }));


            DataTable dtLTETDDTMP = new DataTable();
            dtLTETDDTMP = dtLTETDD_B38.Clone();

            if (!cbxKeepPrevious.Checked)
            {
                this.Invoke((MethodInvoker)(delegate
                {
                    if (WhichBand == BandList.B_1)
                        dtLTETDD_B38.Clear();
                    else if (WhichBand == BandList.B_2)
                        dtLTETDD_B40.Clear();
                    else
                        throw new Exception("No this band");
             
                    dgvSweepResult.Refresh();
                }));
            }
            #endregion --- Variable Define ---

            #region --- Initialize ---

            BeforeTest();

            SetVCC(TestSetting.LEVEL_VCC);
            SetGPCTRL(TestSetting.LEVEL_GPCTRL);

            if (WhichBand == BandList.B_1)
            {
                SetVEN(Arb_Channel.Channel_1, TestSetting.LEVEL_TXEN, false);
                SetVEN(Arb_Channel.Channel_2, 0.05, true);
            }
            else if (WhichBand == BandList.B_2)
            {
                SetVEN(Arb_Channel.Channel_1, 0.05, true);
                SetVEN(Arb_Channel.Channel_2, TestSetting.LEVEL_TXEN, false);
            }
            else
            {
                throw new Exception("No this band");
            }


            _MXA_N9020A.Mod_Initialize(Modulation.LTETDD, true);
            _MXA_N9020A.SetFrequency(2300);
            _E4438C.SetFrequency(2300);
            _E4438C.Mode_Initialize(Modulation.LTETDD, waveform_name);

            _E4438C.SetModOutput(Output.ON);
            _E4438C.SetOutput(Output.ON);

            DateTime t_Start = DateTime.Now;

            string Content = "Make sure everything is setup for LTETDD testing \r\n" +
                              "  1) Connect rf source cable to RFIN \r\n" +
                              "  2) Connect ANT with 20dB pad to RFOUT \r\n" +
                              "  3) Set Control box to right mode \r\n" +
                              "  4) Connect MXA to coupler out directly";

            this.Invoke((MethodInvoker)(delegate
            {
                MessageBox.Show(this, Content, "LTETDD Testing", MessageBoxButtons.OK);
                btnStop.Enabled = false;
                Application.DoEvents();
                wait_2_start(t_Start);
            }));


            #endregion --- Initialize ---

            #region --- LTETDD Test ---

            for (double dblPout_Target = TestSetting.LEVEL_START; dblPout_Target <= TestSetting.LEVEL_STOP; dblPout_Target += TestSetting.LEVEL_STEP)
            {
                if (StopTest) break;

                double PoutLL = dblPout_Target - 0.05;
                double PoutUL = dblPout_Target + 0.05;

                foreach (double dblFreq in TestSetting.FREQLIST)
                {
                    if (StopTest) break;

                    double LTETDD_Pout = 0;
                    double LTETDD_Pin = 0;
                    double LTETDD_Icc = 0;
                    double dblPin = Pin_Start;
                    double[] dblACPResult = new double[5];

                    if (lastLTETDDPin.ContainsKey(dblFreq)) LTETDD_Pin = dblPin = Pin_Start = lastLTETDDPin[dblFreq] + TestSetting.LEVEL_STEP;
                    _E4438C.SetFrequency(dblFreq);
                    _E4438C.SetPower(Pin_Start + TestSetting.LOSS_SRC[dblFreq]);

                    _MXA_N9020A.Config__LTETDD_CHP(dblFreq);
                    intDelay = Math.Max(intDelay_SigGen, intDelay_MXA) * 10;
                    util.Wait(intDelay);

                    LTETDD_Pout = _MXA_N9020A.Get_LTETDD_CHP_Result();
                    LTETDD_Pout += TestSetting.LOSS_MSR_THROUGH[dblFreq];

                    // Freqquency
                    dblResult[0] = dblFreq;

                    // Target Power
                    dblResult[1] = dblPout_Target;

                    //Pout
                    while (LTETDD_Pout <= PoutLL || LTETDD_Pout >= PoutUL)
                    {
                        dblPin = dblPin + dblPout_Target - LTETDD_Pout;
                        if (dblPin > 5)
                            break;

                        _E4438C.SetPower(dblPin + TestSetting.LOSS_SRC[dblFreq]);
                        util.Wait(intDelay);
                        LTETDD_Pout = _MXA_N9020A.Get_LTETDD_CHP_Result();
                        LTETDD_Pout += TestSetting.LOSS_MSR_THROUGH[dblFreq];
                        LTETDD_Pin = dblPin;
                    }
                    dblResult[2] = LTETDD_Pout;

                    // Pin
                    dblResult[3] = LTETDD_Pin;
                    if (lastLTETDDPin.ContainsKey(dblFreq))
                        lastLTETDDPin[dblFreq] = LTETDD_Pin;
                    else
                        lastLTETDDPin.Add(dblFreq, LTETDD_Pin);

                    // Gain
                    dblResult[4] = LTETDD_Pout - LTETDD_Pin;

                    // Icc
                    LTETDD_Icc = _PS_66332A.Max_Current();
                    dblResult[5] = LTETDD_Icc * 1000;
                    // PAE
                    dblResult[6] = 100 * (Math.Pow(10, ((LTETDD_Pout - 30) / 10))) / TestSetting.LEVEL_VCC / LTETDD_Icc;

                    //ACP E_ULTRA
                    _MXA_N9020A.Config__LTETDD_ACP_EULTRA(dblFreq);
                    intDelay = Math.Max(intDelay_SigGen, intDelay_MXA);
                    util.Wait(intDelay * 3);

                    dblACPResult = _MXA_N9020A.Get_LTETDD_ACP_EULTRA_Result();

                    dblResult[7] = dblACPResult[1];     // -10MHz
                    dblResult[8] = dblACPResult[2];     // +10MHz

                    //ACP ULTRA
                    _MXA_N9020A.Config__LTETDD_ACP_ULTRA(dblFreq);
                    intDelay = Math.Max(intDelay_SigGen, intDelay_MXA);
                    util.Wait(intDelay * 3);

                    dblACPResult = _MXA_N9020A.Get_LTETDD_ACP_ULTRA_Result();

                    dblResult[9] = dblACPResult[1];     // -5.8MHz
                    dblResult[10] = dblACPResult[2];     // -5.8MHz
                    dblResult[11] = dblACPResult[3];     // -7.4MHz
                    dblResult[12] = dblACPResult[4];     // -7.4MHz

                    // EVM
                    if (!cbxBypassEVM.Checked)
                    {
                        _MXA_N9020A.Config__LTETDD_EVM(dblFreq);
                        intDelay = Math.Max(intDelay_SigGen, intDelay_MXA);
                        util.Wait(intDelay * 3);

                        dblResult[13] = _MXA_N9020A.Get_LTETDD_EVM_Result();
                    }
                    else
                    {
                        dblResult[13] = 0;
                    }
                    // Update_Grid
                    DataRow drNew = null;
                    if (WhichBand == BandList.B_1)                        
                        drNew = dtLTETDD_B38.NewRow();
                    else if (WhichBand == BandList.B_2)
                        drNew = dtLTETDD_B40.NewRow();
                    else
                        throw new Exception("No this band");

                    DataRow drNewtmp = dtLTETDDTMP.NewRow();
                    drNewtmp[0] = drNew[0] = intTestID++;
                    for (int i = 0; i < dblResult.Count(); i++)
                    {
                        drNew[i + 1] = drNewtmp[i + 1] = Math.Round(dblResult[i], 2);
                    }
                    dtLTETDDTMP.Rows.Add(drNewtmp);

                    this.Invoke((MethodInvoker)(delegate
                    {
                        if (WhichBand == BandList.B_1)
                            dtLTETDD_B38.Rows.Add(drNew);
                        else if (WhichBand == BandList.B_2)
                            dtLTETDD_B40.Rows.Add(drNew);
                        else
                            throw new Exception("No this band");

                        dgvSweepResult.Refresh();
                        Application.DoEvents();
                    }));
                }
            }

            #endregion --- LTETDD Test ---

            #region --- Worst acp report ---

            foreach (double dblFreq in TestSetting.FREQLIST)
            {
                DataRow drmax = null;
                if (WhichBand == BandList.B_1)
                    drmax = dtLTETDD_B38.NewRow();
                else if (WhichBand == BandList.B_2)
                    drmax = dtLTETDD_B40.NewRow();
                else
                    throw new Exception("No this band");

                drmax[0] = intTestID++;
                drmax[1] = dblFreq;

                var acp_n10 = from x in dtLTETDDTMP.AsEnumerable()
                              where x.Field<double>("Frequency (MHz)") == dblFreq
                              select x.Field<double>("ACP_EULTRA -10MHz(dB)");

                var acp_p10 = from x in dtLTETDDTMP.AsEnumerable()
                              where x.Field<double>("Frequency (MHz)") == dblFreq
                              select x.Field<double>("ACP_EULTRA +10MHz(dB)");

                var acp_n58 = from x in dtLTETDDTMP.AsEnumerable()
                              where x.Field<double>("Frequency (MHz)") == dblFreq
                              select x.Field<double>("ACP_ULTRA -0.8MHz(dB)");

                var acp_p58 = from x in dtLTETDDTMP.AsEnumerable()
                              where x.Field<double>("Frequency (MHz)") == dblFreq
                              select x.Field<double>("ACP_ULTRA +0.8MHz(dB)");

                var acp_n74 = from x in dtLTETDDTMP.AsEnumerable()
                              where x.Field<double>("Frequency (MHz)") == dblFreq
                              select x.Field<double>("ACP_ULTRA -2.4MHz(dB)");

                var acp_p74 = from x in dtLTETDDTMP.AsEnumerable()
                              where x.Field<double>("Frequency (MHz)") == dblFreq
                              select x.Field<double>("ACP_ULTRA +2.4MHz(dB)");

                var evm = from x in dtLTETDDTMP.AsEnumerable()
                          where x.Field<double>("Frequency (MHz)") == dblFreq
                          select x.Field<double>("EVM(%)");

                try
                {
                    drmax[8] = acp_n10.ToList<double>().Max();
                    drmax[9] = acp_p10.ToList<double>().Max();
                    drmax[10] = acp_n58.ToList<double>().Max();
                    drmax[11] = acp_p58.ToList<double>().Max();
                    drmax[12] = acp_n74.ToList<double>().Max();
                    drmax[13] = acp_p74.ToList<double>().Max();
                    drmax[14] = evm.ToList<double>().Max();
                }
                catch
                {
                    drmax[8] = 0;
                    drmax[9] = 0;
                    drmax[10] = 0;
                    drmax[11] = 0;
                    drmax[12] = 0;
                    drmax[13] = 0;
                    drmax[14] = 0;
                }

                this.Invoke((MethodInvoker)(delegate
                {
                    if (WhichBand == BandList.B_1)
                        dtLTETDD_B38.Rows.Add(drmax);
                    else if (WhichBand == BandList.B_2)
                        dtLTETDD_B40.Rows.Add(drmax);
                    else
                        throw new Exception("No this band");

                    dgvSweepResult.Refresh();
                    Application.DoEvents();
                }));

            }
            #endregion *** Worst acp report ***

            #region --- After Test ---

            AfterTest();
            _E4438C.SetOutput(Output.OFF);
            _PS_66332A.SetOutput(Output.OFF);
            //_PS_E3631A.SetOutput(Output.OFF);
            _Arb_33522A.SetOutput(Output.OFF, Arb_Channel.Channel_1);
            _Arb_33522A.SetOutput(Output.OFF, Arb_Channel.Channel_2);

            #endregion --- After Test ---
        }

        void LTEFDD_TEST_SH_3(object Band)
        {
            BandList WhichBand = (BandList)Band;

            #region --- Variable Define ---

            Dictionary<double, double> lastLTEFDDPin = new Dictionary<double, double>();
            int intDelay;
            int intTestID = 1;
            double Pin_Start = -10;
            double[] dblResult = new double[14];
            string waveform_name = "";

            this.Invoke((MethodInvoker)(delegate
            {
                waveform_name = cbxWaveform.SelectedItem.ToString();
            }));


            DataTable dtLTEFDDTMP = new DataTable();
            dtLTEFDDTMP = dtLTEFDD_B1.Clone();

            if (!cbxKeepPrevious.Checked)
            {
                this.Invoke((MethodInvoker)(delegate
                {
                    if (WhichBand == BandList.B_1)
                        dtLTEFDD_B1.Clear();
                    else if (WhichBand == BandList.B_2)
                        dtLTEFDD_B2.Clear();
                    else
                        throw new Exception("No this band");

                    dgvSweepResult.Refresh();
                }));
            }

            #endregion --- Variable Define ---

            #region --- Initialize ---

            BeforeTest();

            SetVCC(TestSetting.LEVEL_VCC);
            SetGPCTRL(TestSetting.LEVEL_GPCTRL);

            if (WhichBand == BandList.B_1)
            {
                SetVEN(Arb_Channel.Channel_1, TestSetting.LEVEL_TXEN, true);
                SetVEN(Arb_Channel.Channel_2, 0.05, true);
            }
            else if (WhichBand == BandList.B_2)
            {
                SetVEN(Arb_Channel.Channel_1, 0.05, true);
                SetVEN(Arb_Channel.Channel_2, TestSetting.LEVEL_TXEN, true);
            }
            else
            {
                throw new Exception("No this band");
            }



            _MXA_N9020A.Mod_Initialize(Modulation.LTEFDD, true);
            _MXA_N9020A.SetFrequency(2496);
            _E4438C.SetFrequency(2496);
            _E4438C.Mode_Initialize(Modulation.LTEFDD, waveform_name);

            _E4438C.SetModOutput(Output.ON);
            _E4438C.SetOutput(Output.ON);

            DateTime t_Start = DateTime.Now;
 
            string Content = "Make sure everything is setup for LTEFDD testing \r\n" +
                              "  1) Connect rf source cable to RFIN \r\n" +
                              "  2) Connect ANT with 20dB pad to RFOUT \r\n" +
                              "  3) Set Control box to right mode \r\n" +
                              "  4) Connect MXA to coupler out directly";

            this.Invoke((MethodInvoker)(delegate
            {
                MessageBox.Show(this, Content, "LTEFDD Testing", MessageBoxButtons.OK);
                Application.DoEvents();
                wait_2_start(t_Start);
            }));

            #endregion --- Initialize ---

            #region --- LTEFDD Test ---

            for (double dblPout_Target = TestSetting.LEVEL_START; dblPout_Target <= TestSetting.LEVEL_STOP; dblPout_Target += TestSetting.LEVEL_STEP)
            {
                if (StopTest) break;

                double PoutLL = dblPout_Target - 0.05;
                double PoutUL = dblPout_Target + 0.05;

                foreach (double dblFreq in TestSetting.FREQLIST)
                {
                    if (StopTest) break;

                    double LTEFDD_Pout = 0;
                    double LTEFDD_Pin = 0;
                    double LTEFDD_Icc = 0;
                    double dblPin = Pin_Start;
                    double[] dblACPResult = new double[5];

                    if (lastLTEFDDPin.ContainsKey(dblFreq)) LTEFDD_Pin = dblPin = Pin_Start = lastLTEFDDPin[dblFreq] + TestSetting.LEVEL_STEP;
                    _E4438C.SetFrequency(dblFreq);
                    _E4438C.SetPower(Pin_Start + TestSetting.LOSS_SRC[dblFreq]);

                    _MXA_N9020A.Config__LTEFDD_CHP(dblFreq);
                    intDelay = Math.Max(intDelay_SigGen, intDelay_MXA) * 10;
                    util.Wait(intDelay);

                    LTEFDD_Pout = _MXA_N9020A.Get_LTEFDD_CHP_Result();
                    LTEFDD_Pout += TestSetting.LOSS_MSR_THROUGH[dblFreq];

                    // Freqquency
                    dblResult[0] = dblFreq;

                    // Target Power
                    dblResult[1] = dblPout_Target;

                    //Pout
                    while (LTEFDD_Pout <= PoutLL || LTEFDD_Pout >= PoutUL)
                    {
                        dblPin = dblPin + dblPout_Target - LTEFDD_Pout;
                        if (dblPin > 5)
                            break;

                        _E4438C.SetPower(dblPin + TestSetting.LOSS_SRC[dblFreq]);
                        util.Wait(intDelay);
                        LTEFDD_Pout = _MXA_N9020A.Get_LTEFDD_CHP_Result();
                        LTEFDD_Pout += TestSetting.LOSS_MSR_THROUGH[dblFreq];
                        LTEFDD_Pin = dblPin;
                    }
                    dblResult[2] = LTEFDD_Pout;

                    // Pin
                    dblResult[3] = LTEFDD_Pin;
                    if (lastLTEFDDPin.ContainsKey(dblFreq))
                        lastLTEFDDPin[dblFreq] = LTEFDD_Pin;
                    else
                        lastLTEFDDPin.Add(dblFreq, LTEFDD_Pin);

                    // Gain
                    dblResult[4] = LTEFDD_Pout - LTEFDD_Pin;

                    // Icc
                    LTEFDD_Icc = _PS_66332A.High_Current();
                    dblResult[5] = LTEFDD_Icc * 1000;
                    // PAE
                    dblResult[6] = 100 * (Math.Pow(10, ((LTEFDD_Pout - 30) / 10))) / TestSetting.LEVEL_VCC / LTEFDD_Icc;

                    //ACP E_ULTRA
                    _MXA_N9020A.Config__LTEFDD_ACP_EULTRA(dblFreq);
                    intDelay = Math.Max(intDelay_SigGen, intDelay_MXA);
                    util.Wait(intDelay * 3);

                    dblACPResult = _MXA_N9020A.Get_LTEFDD_ACP_EULTRA_Result();

                    dblResult[7] = dblACPResult[1];     // -10MHz
                    dblResult[8] = dblACPResult[2];     // +10MHz

                    //ACP ULTRA
                    _MXA_N9020A.Config__LTEFDD_ACP_ULTRA(dblFreq);
                    intDelay = Math.Max(intDelay_SigGen, intDelay_MXA);
                    util.Wait(intDelay * 3);

                    dblACPResult = _MXA_N9020A.Get_LTEFDD_ACP_ULTRA_Result();

                    dblResult[9] = dblACPResult[1];     // -5.8MHz
                    dblResult[10] = dblACPResult[2];     // -5.8MHz
                    dblResult[11] = dblACPResult[3];     // -7.4MHz
                    dblResult[12] = dblACPResult[4];     // -7.4MHz

                    // EVM
                    if (!cbxBypassEVM.Checked)
                    {
                        _MXA_N9020A.Config__LTEFDD_EVM(dblFreq);
                        intDelay = Math.Max(intDelay_SigGen, intDelay_MXA);
                        util.Wait(intDelay * 3);

                        dblResult[13] = _MXA_N9020A.Get_LTEFDD_EVM_Result();
                    }
                    else
                    {
                        dblResult[13] = 0;
                    }
                    // Update_Grid
                    DataRow drNew = null;
                    if (WhichBand == BandList.B_1)                        
                        drNew = dtLTEFDD_B1.NewRow();
                    else if (WhichBand == BandList.B_2)
                        drNew = dtLTEFDD_B2.NewRow();
                    else
                        throw new Exception("No this band");

                    DataRow drNewtmp = dtLTEFDDTMP.NewRow();
                    drNewtmp[0] = drNew[0] = intTestID++;
                    for (int i = 0; i < dblResult.Count(); i++)
                    {
                        drNew[i + 1] = drNewtmp[i + 1] = Math.Round(dblResult[i], 2);
                    }
                    dtLTEFDDTMP.Rows.Add(drNewtmp);

                    this.Invoke((MethodInvoker)(delegate
                    {
                        if (WhichBand == BandList.B_1)
                            dtLTEFDD_B1.Rows.Add(drNew);
                        else if (WhichBand == BandList.B_2)
                            dtLTEFDD_B2.Rows.Add(drNew);
                        else
                            throw new Exception("No this band");

                        dgvSweepResult.Refresh();
                        Application.DoEvents();
                    }));
                }
            }

            #endregion --- LTEFDD Test ---

            #region --- Worst acp report ---

            foreach (double dblFreq in TestSetting.FREQLIST)
            {
                DataRow drmax = null;
                if (WhichBand == BandList.B_1)
                    drmax = dtLTEFDD_B1.NewRow();
                else if (WhichBand == BandList.B_2)
                    drmax = dtLTEFDD_B2.NewRow();
                else
                    throw new Exception("No this band");

                drmax[0] = intTestID++;
                drmax[1] = dblFreq;

                var acp_n10 = from x in dtLTEFDDTMP.AsEnumerable()
                              where x.Field<double>("Frequency (MHz)") == dblFreq
                              select x.Field<double>("ACP_EULTRA -10MHz(dB)");

                var acp_p10 = from x in dtLTEFDDTMP.AsEnumerable()
                              where x.Field<double>("Frequency (MHz)") == dblFreq
                              select x.Field<double>("ACP_EULTRA +10MHz(dB)");

                var acp_n25 = from x in dtLTEFDDTMP.AsEnumerable()
                              where x.Field<double>("Frequency (MHz)") == dblFreq
                              select x.Field<double>("ACP_ULTRA -2.5MHz(dB)");

                var acp_p25 = from x in dtLTEFDDTMP.AsEnumerable()
                              where x.Field<double>("Frequency (MHz)") == dblFreq
                              select x.Field<double>("ACP_ULTRA +2.5MHz(dB)");

                var acp_n50 = from x in dtLTEFDDTMP.AsEnumerable()
                              where x.Field<double>("Frequency (MHz)") == dblFreq
                              select x.Field<double>("ACP_ULTRA -7.5MHz(dB)");

                var acp_p50 = from x in dtLTEFDDTMP.AsEnumerable()
                              where x.Field<double>("Frequency (MHz)") == dblFreq
                              select x.Field<double>("ACP_ULTRA +7.5MHz(dB)");

                var evm = from x in dtLTEFDDTMP.AsEnumerable()
                          where x.Field<double>("Frequency (MHz)") == dblFreq
                          select x.Field<double>("EVM(%)");

                try
                {
                    drmax[8] = acp_n10.ToList<double>().Max();
                    drmax[9] = acp_p10.ToList<double>().Max();
                    drmax[10] = acp_n25.ToList<double>().Max();
                    drmax[11] = acp_p25.ToList<double>().Max();
                    drmax[12] = acp_n50.ToList<double>().Max();
                    drmax[13] = acp_p50.ToList<double>().Max();
                    drmax[14] = evm.ToList<double>().Max();
                }
                catch
                {
                    drmax[8] = 0;
                    drmax[9] = 0;
                    drmax[10] = 0;
                    drmax[11] = 0;
                    drmax[12] = 0;
                    drmax[13] = 0;
                    drmax[14] = 0;
                }

                this.Invoke((MethodInvoker)(delegate
                {
                    if (WhichBand == BandList.B_1)
                        dtLTEFDD_B1.Rows.Add(drmax);
                    else if (WhichBand == BandList.B_2)
                        dtLTEFDD_B2.Rows.Add(drmax);
                    else
                        throw new Exception("No this band");

                    dgvSweepResult.Refresh();
                    Application.DoEvents();
                }));

            }
            #endregion *** Worst acp report ***

            #region --- After Test ---

            AfterTest();
            _E4438C.SetOutput(Output.OFF);
            _PS_66332A.SetOutput(Output.OFF);
            //_PS_E3631A.SetOutput(Output.OFF);
            _Arb_33522A.SetOutput(Output.OFF, Arb_Channel.Channel_1);
            _Arb_33522A.SetOutput(Output.OFF, Arb_Channel.Channel_2);

            #endregion --- After Test ---
        }

        void CDMA_TEST_SH_3(object Band)
        {
            BandList WhichBand = (BandList)Band;

            #region --- Variable Define ---

            Dictionary<double, double> lastPin = new Dictionary<double, double>();
            int intDelay;
            int intTestID = 1;
            double Pin_Start = -10;
            double[] dblResult = new double[12];
            string waveform_name = "";

            this.Invoke((MethodInvoker)(delegate
            {
                waveform_name = cbxWaveform.Items[1].ToString();
            }));

            DataTable dtCDMATMP = new DataTable();
            dtCDMATMP = dtCDMA.Clone();

            if (!cbxKeepPrevious.Checked)
            {
                this.Invoke((MethodInvoker)(delegate
                {
                    dtCDMA.Clear();
                    dgvSweepResult.Refresh();
                }));
            }

            #endregion --- Variable Define ---

            #region --- Initialize ---

            BeforeTest();
            //_PS_E3631A = new PS_E3631A(Instruments_address._06);
            //_PS_E3631A.Initialize();

            SetVCC(TestSetting.LEVEL_VCC);
            SetGPCTRL(TestSetting.LEVEL_GPCTRL);
            
            if (WhichBand == BandList.B_1)  // B_1 is CDMA
            {
                SetVEN(Arb_Channel.Channel_1, 0.05, true);
                SetVEN(Arb_Channel.Channel_2, 0.05, true);

                _E4438C.Mode_Initialize(Modulation.CDMA, waveform_name);
                _MXA_N9020A.Mod_Initialize(Modulation.CDMA, false);
            }
            else if (WhichBand == BandList.B_2)  // B_2 is EVDO
            {
                SetVEN(Arb_Channel.Channel_1, 0.05, true);
                SetVEN(Arb_Channel.Channel_2, 0.05,true);

                _E4438C.Mode_Initialize(Modulation.EVDO, waveform_name);
                _MXA_N9020A.Mod_Initialize(Modulation.EVDO,false);
            }
            else
            {
                throw new Exception("No This band");
            }
            _E4438C.SetModOutput(Output.ON);
            _E4438C.SetOutput(Output.ON);

            DateTime t_start = DateTime.Now;

            string Content = "Make sure everything is setup for CDMA/EVDO testing \r\n" +
                              "  1) Connect rf source cable to CDMA/EVDO \r\n" +
                              "  2) Set Control box to CDMA/EVDO mode \r\n" +
                              "  3) Connect MXA to coupler out directly";

            this.Invoke((MethodInvoker)(delegate
            {
                MessageBox.Show(this, Content, "CDMA/EVDO Testing", MessageBoxButtons.OK);
                Application.DoEvents();
                wait_2_start(t_start);
            }));

            #endregion --- Initialize ---

            #region --- CDMA/EVDO Test ---

            for (double dblPout_Target = TestSetting.LEVEL_START; dblPout_Target <= TestSetting.LEVEL_STOP; dblPout_Target += TestSetting.LEVEL_STEP)
            {
                if (StopTest) break;

                double PoutLL = dblPout_Target - 0.05;
                double PoutUL = dblPout_Target + 0.05;

                foreach (double dblFreq in TestSetting.FREQLIST)
                {
                    if (StopTest) break;

                    double CDMA_Pout = 0;
                    double CDMA_Pin = 0;
                    double CDMA_Icc = 0;
                    double dblPin = Pin_Start;
                    double[] dblACPResult = new double[5];

                    if (lastPin.ContainsKey(dblFreq)) CDMA_Pin = dblPin = Pin_Start = lastPin[dblFreq] + TestSetting.LEVEL_STEP;
                    _E4438C.SetFrequency(dblFreq);
                    _E4438C.SetPower(Pin_Start + TestSetting.LOSS_SRC[dblFreq]);

                    // MXA to measure channel power
                    #region 
                    //intDelay = Math.Max(intDelay_SigGen, intDelay_MXA) * 10;
                    //if (WhichBand == BandList.B_1)
                    //{
                    //    _MXA_N9020A.Config__CDMA_CHP(dblFreq);
                    //    util.Wait(intDelay);

                    //    CDMA_Pout = _MXA_N9020A.Get_CDMA_CHP_Result();
                    //}
                    //else if (WhichBand == BandList.B_2)
                    //{
                    //    _MXA_N9020A.Config__EVDO_CHP(dblFreq);
                    //    util.Wait(intDelay);

                    //    CDMA_Pout = _MXA_N9020A.Get_EVDO_CHP_Result();
                    //}
                    //else
                    //    throw new Exception("No this band");

                    //CDMA_Pout += TestSetting.LOSS_MSR_THROUGH[dblFreq];
                    #endregion

                    // PowerMeter A to measure channel power
                    _PM_N1913A.Configure__CW_Power(dblFreq, 10);
                    intDelay = Math.Max(intDelay_SigGen, intDelay_PowerMeter) * 2;
                    util.Wait(intDelay);
                    CDMA_Pout = _PM_N1913A.GetPowerResult();
                    CDMA_Pout += TestSetting.LOSS_MSR_POUT[dblFreq];

                    // Freqquency
                    dblResult[0] = dblFreq;

                    // Target Power
                    dblResult[1] = dblPout_Target;

                    // Pout
                    while (CDMA_Pout <= PoutLL || CDMA_Pout >= PoutUL)
                    {
                        dblPin = dblPin + dblPout_Target - CDMA_Pout;
                        if (dblPin > 5)
                            break;

                        _E4438C.SetPower(dblPin + TestSetting.LOSS_SRC[dblFreq]);
                        util.Wait(intDelay);

                        // MXA to measure channel power
                        #region
                        //if (WhichBand == BandList.B_1)
                        //    CDMA_Pout = _MXA_N9020A.Get_CDMA_CHP_Result();
                        //else if (WhichBand == BandList.B_2)
                        //    CDMA_Pout = _MXA_N9020A.Get_EVDO_CHP_Result();
                        //else
                        //    throw new Exception("No this band");

                        //CDMA_Pout += TestSetting.LOSS_MSR_THROUGH[dblFreq];
                        #endregion

                        // PowerMeter A to measure channel power
                        CDMA_Pout = _PM_N1913A.GetPowerResult();
                        CDMA_Pout += TestSetting.LOSS_MSR_POUT[dblFreq];

                        CDMA_Pin = dblPin;
                    }
                    dblResult[2] = CDMA_Pout;

                    // Pin
                    dblResult[3] = CDMA_Pin;

                    if (lastPin.ContainsKey(dblFreq))
                        lastPin[dblFreq] = CDMA_Pin;
                    else
                        lastPin.Add(dblFreq, CDMA_Pin);
                    // Gain
                    dblResult[4] = CDMA_Pout - CDMA_Pin;

                    // Icc
                    CDMA_Icc = _PS_66332A.RMS_Current();
                    dblResult[5] = CDMA_Icc * 1000;
                    // PAE
                    dblResult[6] = 100 * (Math.Pow(10, ((CDMA_Pout - 30) / 10))) / TestSetting.LEVEL_VCC / CDMA_Icc;

                    // ACP
                    intDelay = Math.Max(intDelay_SigGen, intDelay_MXA);
                    if (WhichBand == BandList.B_1)
                    {
                        _MXA_N9020A.Config__CDMA_ACP(dblFreq);
                        util.Wait(intDelay * 3);

                        dblACPResult = _MXA_N9020A.Get_CDMA_ACP_Result();
                    }
                    else if (WhichBand == BandList.B_2)
                    {
                        _MXA_N9020A.Config__EVDO_ACP(dblFreq);
                        util.Wait(intDelay * 3);

                        dblACPResult = _MXA_N9020A.Get_EVDO_ACP_Result();
                    }
                    else
                        throw new Exception("No this band");


                    dblResult[7] = dblACPResult[1];     // -5MHz
                    dblResult[8] = dblACPResult[2];     // +5MHz
                    dblResult[9] = dblACPResult[3];     // -10MHz
                    dblResult[10] = dblACPResult[4];     // +10MHz

                    // EVM
                    if (!cbxBypassEVM.Checked)
                    {
                        ////waveform_name = cbxWaveform.Items[1].ToString();

                        ////if (WhichBand == BandList.B_1)
                        ////    _E4438C.Mode_Initialize(Modulation.CDMA, waveform_name);
                        ////else if (WhichBand == BandList.B_2)
                        ////    _E4438C.Mode_Initialize(Modulation.EVDO, waveform_name);
                        ////else
                        ////    throw new Exception("No this band");

                        ////_E4438C.SetFrequency(dblFreq);
                        ////_E4438C.SetPower(CDMA_Icc + TestSetting.LOSS_SRC[dblFreq]);
                        ////_E4438C.SetModOutput(Output.ON);
                        ////_E4438C.SetOutput(Output.ON);
                        ////util.Wait(1000);

                        intDelay = Math.Max(intDelay_SigGen, intDelay_MXA);
                        if (WhichBand == BandList.B_1)
                        {
                            _MXA_N9020A.Config__CDMA_EVM(dblFreq);
                            util.Wait(intDelay * 3);

                            dblResult[11] = _MXA_N9020A.Get_CDMA_EVM_Result();
                        }
                        else if (WhichBand == BandList.B_2)
                        {
                            _MXA_N9020A.Config__EVDO_EVM(dblFreq);
                            util.Wait(intDelay * 3);

                            dblResult[11] = _MXA_N9020A.Get_EVDO_EVM_Result();
                        }
                        else
                            throw new Exception("No this band");

                    }
                    else
                    {
                        dblResult[11] = 0;
                    }
                    // Update_Grid
                    DataRow drNew = null;
                    if (WhichBand == BandList.B_1)
                        drNew = dtCDMA.NewRow();
                    else if (WhichBand == BandList.B_2)
                        drNew = dtEVDO.NewRow();
                    else
                        throw new Exception("No this band");

                    DataRow drNewtmp = dtCDMATMP.NewRow();
                    drNewtmp[0] = drNew[0] = intTestID++;
                    for (int i = 0; i < dblResult.Count(); i++)
                    {
                        drNew[i + 1] = drNewtmp[i + 1] = Math.Round(dblResult[i], 2);
                    }
                    dtCDMATMP.Rows.Add(drNewtmp);

                    this.Invoke((MethodInvoker)(delegate
                    {
                        if (WhichBand == BandList.B_1)
                            dtCDMA.Rows.Add(drNew);
                        else if (WhichBand == BandList.B_2)
                            dtEVDO.Rows.Add(drNew);
                        else
                            throw new Exception("No this band");

                        dgvSweepResult.Refresh();
                        Application.DoEvents();
                    }));

                }
            }

            #endregion --- CDMA/EVDO Test ---

            #region --- Worst acp report ---

            foreach (double dblFreq in TestSetting.FREQLIST)
            {
                DataRow drmax = null;
                if (WhichBand == BandList.B_1)
                    drmax = dtCDMA.NewRow();
                else if (WhichBand == BandList.B_2)
                    drmax = dtEVDO.NewRow();
                else
                    throw new Exception("No this band");
                
                var acp_n5 = from x in dtCDMATMP.AsEnumerable()
                             where x.Field<double>("Frequency (MHz)") == dblFreq
                             select x.Field<double>("ACP -5MHz(dB)");

                var acp_p5 = from x in dtCDMATMP.AsEnumerable()
                             where x.Field<double>("Frequency (MHz)") == dblFreq
                             select x.Field<double>("ACP +5MHz(dB)");

                var acp_n10 = from x in dtCDMATMP.AsEnumerable()
                              where x.Field<double>("Frequency (MHz)") == dblFreq
                              select x.Field<double>("ACP -10MHz(dB)");

                var acp_p10 = from x in dtCDMATMP.AsEnumerable()
                              where x.Field<double>("Frequency (MHz)") == dblFreq
                              select x.Field<double>("ACP +10MHz(dB)");

                var evm = from x in dtCDMATMP.AsEnumerable()
                          where x.Field<double>("Frequency (MHz)") == dblFreq
                          select x.Field<double>("EVM(%)");

                try
                {
                    drmax[8] = acp_n5.ToList<double>().Max();
                    drmax[9] = acp_p5.ToList<double>().Max();
                    drmax[10] = acp_n10.ToList<double>().Max();
                    drmax[11] = acp_p10.ToList<double>().Max();
                    drmax[12] = evm.ToList<double>().Max();
                }
                catch
                {
                    drmax[8] = 0;
                    drmax[9] = 0;
                    drmax[10] = 0;
                    drmax[11] = 0;
                    drmax[12] = 0;
                }

                this.Invoke((MethodInvoker)(delegate
                {
                    if (WhichBand == BandList.B_1)
                        dtCDMA.Rows.Add(drmax);
                    else if (WhichBand == BandList.B_2)
                        dtEVDO.Rows.Add(drmax);
                    else
                        throw new Exception("No this band");
                   
                    dgvSweepResult.Refresh();
                    Application.DoEvents();
                }));

            }
            #endregion *** Worst acp report ***

            #region --- After Test ---

            AfterTest();
            _E4438C.SetOutput(Output.OFF);
            _PS_66332A.SetOutput(Output.OFF);
            //_PS_E3631A.SetOutput(Output.OFF);
            _Arb_33522A.SetOutput(Output.OFF, Arb_Channel.Channel_1);
            _Arb_33522A.SetOutput(Output.OFF, Arb_Channel.Channel_2);

            #endregion --- After Test ---

        }

        void WCDMA_TEST_SH_3()
        {
            #region --- Variable Define ---

            Dictionary<double, double> lastPin = new Dictionary<double, double>();
            int intDelay;
            int intTestID = 1;
            double Pin_Start = -10;
            double[] dblResult = new double[12];

            DataTable dtWCDMATMP = new DataTable();
            dtWCDMATMP = dtWCDMA.Clone();

            if (!cbxKeepPrevious.Checked)
            {
                this.Invoke((MethodInvoker)(delegate
                {
                    dtWCDMA.Clear();
                    dgvSweepResult.Refresh();
                }));
            }

            #endregion --- Variable Define ---

            #region --- Initialize ---

            BeforeTest();
            //_PS_E3631A = new PS_E3631A(Instruments_address._06);
            //_PS_E3631A.Initialize();

            SetVCC(TestSetting.LEVEL_VCC);
            //SetVbat(TestSetting.LEVEL_VBAT, 1);
            //SetTXEnable(TestSetting.LEVEL_TXEN);
            //SetVramp(TestSetting.LEVEL_PIN_VRAMP);
            //SetGPCTRL(TestSetting.LEVEL_GPCTRL);

            _E4438C.Mode_Initialize(Modulation.WCDMA);
            _MXA_N9020A.Mod_Initialize(Modulation.WCDMA);

            _E4438C.SetModOutput(Output.ON);
            _E4438C.SetOutput(Output.ON);

            DateTime t_Start = DateTime.Now;

            string Content = "Make sure everything is setup for WCDMA testing \r\n" +
                              "  1) Connect rf source cable to WCDMA \r\n" +
                              "  2) Set Control box to WCDMA mode \r\n" +
                              "  3) Connect MXA to coupler out directly";

            this.Invoke((MethodInvoker)(delegate
            {
                MessageBox.Show(this, Content, "WCDMA Testing", MessageBoxButtons.OK);
                btnStop.Enabled = false;
                Application.DoEvents();
                wait_2_start(t_Start);
            }));

            #endregion --- Initialize ---

            #region --- WCDMA Test ---

            for (double dblPout_Target = TestSetting.LEVEL_START; dblPout_Target <= TestSetting.LEVEL_STOP; dblPout_Target += TestSetting.LEVEL_STEP)
            {
                if (StopTest) break;

                double PoutLL = dblPout_Target - 0.05;
                double PoutUL = dblPout_Target + 0.05;

                foreach (double dblFreq in TestSetting.FREQLIST)
                {
                    if (StopTest) break;

                    double WCDMA_Pout = 0;
                    double WCDMA_Pin = 0;
                    double WCDMA_Icc = 0;
                    double dblPin = Pin_Start;
                    double[] dblACPResult = new double[5];

                    if (lastPin.ContainsKey(dblFreq)) WCDMA_Pin = dblPin = Pin_Start = lastPin[dblFreq] + TestSetting.LEVEL_STEP;
                    _E4438C.SetFrequency(dblFreq);
                    _E4438C.SetPower(Pin_Start + TestSetting.LOSS_SRC[dblFreq]);

                    _MXA_N9020A.Config__WCDMA_CHP(dblFreq);
                    intDelay = Math.Max(intDelay_SigGen, intDelay_MXA) * 10;
                    //_PM_N1913A.Configure__CW_Power(dblFreq, 10);
                    //intDelay = Math.Max(intDelay_SigGen, intDelay_PowerMeter) * 2;
                    util.Wait(intDelay);

                    WCDMA_Pout = _MXA_N9020A.Get_WCDMA_CHP_Result();
                    WCDMA_Pout += TestSetting.LOSS_MSR_THROUGH[dblFreq];
                    //WCDMA_Pout = _PM_N1913A.GetPowerResult();
                    //WCDMA_Pout += TestSetting.LOSS_MSR_POUT[dblFreq];

                    // Freqquency
                    dblResult[0] = dblFreq;

                    // Target Power
                    dblResult[1] = dblPout_Target;

                    // Pout
                    while (WCDMA_Pout <= PoutLL || WCDMA_Pout >= PoutUL)
                    {
                        dblPin = dblPin + dblPout_Target - WCDMA_Pout;
                        if (dblPin > 5)
                        {
                            dblPin = 5;
                            _E4438C.SetPower(dblPin + TestSetting.LOSS_SRC[dblFreq]);
                            util.Wait(intDelay);
                            WCDMA_Pout = _MXA_N9020A.Get_WCDMA_CHP_Result();
                            WCDMA_Pout += TestSetting.LOSS_MSR_THROUGH[dblFreq];
                            //WCDMA_Pout = _PM_N1913A.GetPowerResult();
                            //WCDMA_Pout += TestSetting.LOSS_MSR_POUT[dblFreq];
                            WCDMA_Pin = dblPin;

                            break;
                        }

                        _E4438C.SetPower(dblPin + TestSetting.LOSS_SRC[dblFreq]);
                        util.Wait(intDelay);
                        WCDMA_Pout = _MXA_N9020A.Get_WCDMA_CHP_Result();
                        WCDMA_Pout += TestSetting.LOSS_MSR_THROUGH[dblFreq];
                        //WCDMA_Pout = _PM_N1913A.GetPowerResult();
                        //WCDMA_Pout += TestSetting.LOSS_MSR_POUT[dblFreq];
                        WCDMA_Pin = dblPin;
                    }
                    dblResult[2] = WCDMA_Pout;

                    // Pin
                    dblResult[3] = WCDMA_Pin;

                    if (lastPin.ContainsKey(dblFreq))
                        lastPin[dblFreq] = WCDMA_Pin;
                    else
                        lastPin.Add(dblFreq, WCDMA_Pin);
                    // Gain
                    dblResult[4] = WCDMA_Pout - WCDMA_Pin;

                    // Icc
                    WCDMA_Icc = _PS_66332A.RMS_Current();
                    dblResult[5] = WCDMA_Icc * 1000;
                    // PAE
                    dblResult[6] = 100 * (Math.Pow(10, ((WCDMA_Pout - 30) / 10))) / TestSetting.LEVEL_VCC / WCDMA_Icc;

                    // ACP
                    _MXA_N9020A.Config__WCDMA_ACP(dblFreq);
                    intDelay = Math.Max(intDelay_SigGen, intDelay_MXA);
                    util.Wait(intDelay * 3);

                    dblACPResult = _MXA_N9020A.Get_WCDMA_ACP_Result();

                    dblResult[7] = dblACPResult[1];     // -5MHz
                    dblResult[8] = dblACPResult[2];     // +5MHz
                    dblResult[9] = dblACPResult[3];     // -10MHz
                    dblResult[10] = dblACPResult[4];     // +10MHz

                    // EVM
                    if (!cbxBypassEVM.Checked)
                    {
                        _MXA_N9020A.Config__WCDMA_EVM(dblFreq);
                        intDelay = Math.Max(intDelay_SigGen, intDelay_MXA);
                        util.Wait(intDelay * 3);

                        dblResult[11] = _MXA_N9020A.Get_WCDMA_EVM_Result();
                    }
                    else
                    {
                        dblResult[11] = 0;
                    }
                    // Update_Grid
                    DataRow drNew = dtWCDMA.NewRow();
                    DataRow drNewtmp = dtWCDMATMP.NewRow();
                    drNew[0] = intTestID++;
                    for (int i = 0; i < dblResult.Count(); i++)
                    {
                        drNew[i + 1] = drNewtmp[i + 1] = Math.Round(dblResult[i], 2);
                    }
                    dtWCDMATMP.Rows.Add(drNewtmp);

                    this.Invoke((MethodInvoker)(delegate
                    {
                        dtWCDMA.Rows.Add(drNew);
                        dgvSweepResult.Refresh();
                        Application.DoEvents();
                    }));
                }
            }

            #endregion --- WCDMA Test ---

            #region --- Worst acp report ---

            foreach (double dblFreq in TestSetting.FREQLIST)
            {
                DataRow drmax = dtWCDMA.NewRow();
                drmax[0] = intTestID++;
                drmax[1] = dblFreq;

                var acp_n5 = from x in dtWCDMATMP.AsEnumerable()
                             where x.Field<double>("Frequency (MHz)") == dblFreq
                             select x.Field<double>("ACP -5MHz(dB)");

                var acp_p5 = from x in dtWCDMATMP.AsEnumerable()
                             where x.Field<double>("Frequency (MHz)") == dblFreq
                             select x.Field<double>("ACP +5MHz(dB)");

                var acp_n10 = from x in dtWCDMATMP.AsEnumerable()
                              where x.Field<double>("Frequency (MHz)") == dblFreq
                              select x.Field<double>("ACP -10MHz(dB)");

                var acp_p10 = from x in dtWCDMATMP.AsEnumerable()
                              where x.Field<double>("Frequency (MHz)") == dblFreq
                              select x.Field<double>("ACP +10MHz(dB)");

                var evm = from x in dtWCDMATMP.AsEnumerable()
                          where x.Field<double>("Frequency (MHz)") == dblFreq
                          select x.Field<double>("EVM(%)");

                try
                {
                    drmax[8] = acp_n5.ToList<double>().Max();
                    drmax[9] = acp_p5.ToList<double>().Max();
                    drmax[10] = acp_n10.ToList<double>().Max();
                    drmax[11] = acp_p10.ToList<double>().Max();
                    drmax[12] = evm.ToList<double>().Max();
                }
                catch
                {
                    drmax[8] = 0;
                    drmax[9] = 0;
                    drmax[10] = 0;
                    drmax[11] = 0;
                    drmax[12] = 0;
                }

                this.Invoke((MethodInvoker)(delegate
                {
                    dtWCDMA.Rows.Add(drmax);
                    dgvSweepResult.Refresh();
                    Application.DoEvents();
                }));

            }
            #endregion *** Worst acp report ***

            #region --- After Test ---

            AfterTest();
            _E4438C.SetOutput(Output.OFF);
            _PS_66332A.SetOutput(Output.OFF);
            //_PS_E3631A.SetOutput(Output.OFF);
            _Arb_33522A.SetOutput(Output.OFF, Arb_Channel.Channel_1);
            _Arb_33522A.SetOutput(Output.OFF, Arb_Channel.Channel_2);

            #endregion --- After Test ---

        }

        #endregion --- SH_3 Test ---
        void LTETDD_TEST()
        {
            #region --- Variable Define ---

            Dictionary<double, double> lastLTETDDPin = new Dictionary<double, double>();
            int intDelay;
            int intTestID = 1;
            double Pin_Start = -10;
            double[] dblResult = new double[14];
            string waveform_name = "";

            this.Invoke((MethodInvoker)(delegate
            {
                waveform_name = cbxWaveform.SelectedItem.ToString();
            }));


            DataTable dtLTETDDTMP_B38 = new DataTable();
            dtLTETDDTMP_B38 = dtLTETDD_B38.Clone();

            if (!cbxKeepPrevious.Checked)
            {
                this.Invoke((MethodInvoker)(delegate
                {
                    dtLTETDD_B38.Clear();
                    dgvSweepResult.Refresh();
                }));
            }

            #endregion --- Variable Define ---

            #region --- Initialize ---

            BeforeTest();

            _E4438C.SetFrequency(2496);
            _E4438C.Mode_Initialize(Modulation.LTETDD, waveform_name);

            _MXA_N9020A.Mod_Initialize(Modulation.LTETDD, true);
            _MXA_N9020A.SetFrequency(2496);

            _E4438C.SetModOutput(Output.ON);
            _E4438C.SetOutput(Output.ON);

            SetVCC_New(TestSetting.LEVEL_VCC, 1000);
            SetVbat_New(TestSetting.LEVEL_VBAT, 100);

            if (cbxMipi.Checked)
            {
                string Content = "Make sure everything is setup for LTETDD testing \r\n" +
                                    "  1) Connect rf source cable to RFIN \r\n" +
                                    "  2) Connect MXA to ANT output with 20dB Pad directly \r\n" +
                                    "  3) Set mipi control to the right mode in next windows";

                this.Invoke((MethodInvoker)(delegate
                {
                    MessageBox.Show(this, Content, "LTETDD Testing", MessageBoxButtons.OK);
                    Application.DoEvents();
                    frmMipi.ShowDialog(); 
                    util.Wait(1000);
                }));
            }
            else
            {
                string Content = "Make sure everything is setup for LTETDD testing \r\n" +
                                    "  1) Connect rf source cable to RFIN8 \r\n" +
                                    "  2) Connect MXA to ANT output with 20dB Pad directly \r\n" +
                                    "  3) Set Control box to right mode";

                this.Invoke((MethodInvoker)(delegate
                {
                    MessageBox.Show(this, Content, "LTETDD Testing", MessageBoxButtons.OK);
                    Application.DoEvents();
                }));

                SetGPCTRL(TestSetting.LEVEL_GPCTRL);
            }

            #endregion --- Initialize ---

            #region --- LTETDD Test ---

            for (double dblPout_Target = TestSetting.LEVEL_START; dblPout_Target <= TestSetting.LEVEL_STOP; dblPout_Target += TestSetting.LEVEL_STEP)
            {
                if (StopTest) break;

                double PoutLL = dblPout_Target - 0.05;
                double PoutUL = dblPout_Target + 0.05;

                foreach (double dblFreq in TestSetting.FREQLIST)
                {
                    if (StopTest) break;

                    double LTETDD_B38_Pout = 0;
                    double LTETDD_B38_Pin = 0;
                    double LTETDD_B38_Icc = 0;
                    double dblPin = Pin_Start;
                    double[] dblACPResult = new double[5];

                    if (lastLTETDDPin.ContainsKey(dblFreq)) LTETDD_B38_Pin = dblPin = Pin_Start = lastLTETDDPin[dblFreq] + TestSetting.LEVEL_STEP;
                    _E4438C.SetFrequency(dblFreq);
                    _E4438C.SetPower(Pin_Start + TestSetting.LOSS_SRC[dblFreq]);

                    _MXA_N9020A.Config__LTETDD_CHP(dblFreq);
                    intDelay = Math.Max(intDelay_SigGen, intDelay_MXA) * 10;
                    util.Wait(intDelay);

                    LTETDD_B38_Pout = _MXA_N9020A.Get_LTETDD_CHP_Result();
                    LTETDD_B38_Pout += TestSetting.LOSS_MSR_THROUGH[dblFreq];

                    // Freqquency
                    dblResult[0] = dblFreq;

                    // Target Power
                    dblResult[1] = dblPout_Target;

                    //Pout
                    while (LTETDD_B38_Pout <= PoutLL || LTETDD_B38_Pout >= PoutUL)
                    {
                        dblPin = dblPin + dblPout_Target - LTETDD_B38_Pout;
                        if (dblPin > 5)
                            break;

                        _E4438C.SetPower(dblPin + TestSetting.LOSS_SRC[dblFreq]);
                        util.Wait(intDelay);
                        LTETDD_B38_Pout = _MXA_N9020A.Get_LTETDD_CHP_Result();
                        LTETDD_B38_Pout += TestSetting.LOSS_MSR_THROUGH[dblFreq];
                        LTETDD_B38_Pin = dblPin;
                    }
                    dblResult[2] = LTETDD_B38_Pout;

                    // Pin
                    dblResult[3] = LTETDD_B38_Pin;
                    if (lastLTETDDPin.ContainsKey(dblFreq))
                        lastLTETDDPin[dblFreq] = LTETDD_B38_Pin;
                    else
                        lastLTETDDPin.Add(dblFreq, LTETDD_B38_Pin);

                    // Gain
                    dblResult[4] = LTETDD_B38_Pout - LTETDD_B38_Pin;

                    // Icc
                    LTETDD_B38_Icc = _PS_66332A.High_Current();
                    dblResult[5] = LTETDD_B38_Icc * 1000;
                    // PAE
                    dblResult[6] = 100 * (Math.Pow(10, ((LTETDD_B38_Pout - 30) / 10))) / TestSetting.LEVEL_VCC / LTETDD_B38_Icc;

                    //ACP E_ULTRA
                    _MXA_N9020A.Config__LTETDD_ACP_EULTRA(dblFreq);
                    intDelay = Math.Max(intDelay_SigGen, intDelay_MXA);
                    util.Wait(intDelay * 3);

                    dblACPResult = _MXA_N9020A.Get_LTETDD_ACP_EULTRA_Result();

                    dblResult[7] = dblACPResult[1];     // -10MHz
                    dblResult[8] = dblACPResult[2];     // +10MHz

                    //ACP ULTRA
                    _MXA_N9020A.Config__LTETDD_ACP_ULTRA(dblFreq);
                    intDelay = Math.Max(intDelay_SigGen, intDelay_MXA);
                    util.Wait(intDelay * 3);

                    dblACPResult = _MXA_N9020A.Get_LTETDD_ACP_ULTRA_Result();

                    dblResult[9] = dblACPResult[1];     // -5.8MHz
                    dblResult[10] = dblACPResult[2];     // -5.8MHz
                    dblResult[11] = dblACPResult[3];     // -7.4MHz
                    dblResult[12] = dblACPResult[4];     // -7.4MHz

                    // EVM
                    if (!cbxBypassEVM.Checked)
                    {
                        _MXA_N9020A.Config__LTETDD_EVM(dblFreq);
                        intDelay = Math.Max(intDelay_SigGen, intDelay_MXA);
                        util.Wait(intDelay * 3);

                        dblResult[13] = _MXA_N9020A.Get_LTETDD_EVM_Result();
                    }
                    else
                    {
                        dblResult[13] = 0;
                    }

                    this.Invoke((MethodInvoker)(delegate
                    {
                        // Update_Grid
                        //DataRow drNew = dtLTETDD_B38.NewRow();
                        DataRow drNewtmp = dtLTETDDTMP_B38.NewRow();
                        //drNewtmp[0] = drNew[0] = intTestID++;
                        drNewtmp[0] = intTestID++;
                        for (int i = 0; i < dblResult.Count(); i++)
                        {
                            //drNew[i + 1] = drNewtmp[i + 1] = Math.Round(dblResult[i], 2);
                            drNewtmp[i + 1] = Math.Round(dblResult[i], 2);
                        }
                        dtLTETDDTMP_B38.Rows.Add(drNewtmp);
                        //dtLTETDD_B38.Rows.Add(drNew);

                        UpdateResult(ref dtLTETDD_B38, drNewtmp);
                       
                        //dtLTETDD_B38.Rows.Add(drNew);
                        //dgvSweepResult.Refresh();
                        //Application.DoEvents();
                    }));
                }
            }

            #endregion --- LTETDD_B38 Test ---

            #region --- Worst acp report ---

            foreach (double dblFreq in TestSetting.FREQLIST)
            {
                DataRow drmax = dtLTETDDTMP_B38.NewRow();
                drmax[0] = intTestID++;
                drmax[1] = dblFreq;

                var acp_n10 = from x in dtLTETDDTMP_B38.AsEnumerable()
                              where x.Field<double>("Frequency (MHz)") == dblFreq
                              select x.Field<double>("ACP_EULTRA -10MHz(dB)");

                var acp_p10 = from x in dtLTETDDTMP_B38.AsEnumerable()
                              where x.Field<double>("Frequency (MHz)") == dblFreq
                              select x.Field<double>("ACP_EULTRA +10MHz(dB)");

                var acp_n58 = from x in dtLTETDDTMP_B38.AsEnumerable()
                              where x.Field<double>("Frequency (MHz)") == dblFreq
                              select x.Field<double>("ACP_ULTRA -5.8MHz(dB)");

                var acp_p58 = from x in dtLTETDDTMP_B38.AsEnumerable()
                              where x.Field<double>("Frequency (MHz)") == dblFreq
                              select x.Field<double>("ACP_ULTRA +5.8MHz(dB)");

                var acp_n74 = from x in dtLTETDDTMP_B38.AsEnumerable()
                              where x.Field<double>("Frequency (MHz)") == dblFreq
                              select x.Field<double>("ACP_ULTRA -7.4MHz(dB)");

                var acp_p74 = from x in dtLTETDDTMP_B38.AsEnumerable()
                              where x.Field<double>("Frequency (MHz)") == dblFreq
                              select x.Field<double>("ACP_ULTRA +7.4MHz(dB)");

                var evm = from x in dtLTETDDTMP_B38.AsEnumerable()
                          where x.Field<double>("Frequency (MHz)") == dblFreq
                          select x.Field<double>("EVM(%)");

                try
                {
                    drmax[8] = acp_n10.ToList<double>().Max();
                    drmax[9] = acp_p10.ToList<double>().Max();
                    drmax[10] = acp_n58.ToList<double>().Max();
                    drmax[11] = acp_p58.ToList<double>().Max();
                    drmax[12] = acp_n74.ToList<double>().Max();
                    drmax[13] = acp_p74.ToList<double>().Max();
                    drmax[14] = evm.ToList<double>().Max();
                }
                catch
                {
                    drmax[8] = 0;
                    drmax[9] = 0;
                    drmax[10] = 0;
                    drmax[11] = 0;
                    drmax[12] = 0;
                    drmax[13] = 0;
                    drmax[14] = 0;
                }

                this.Invoke((MethodInvoker)(delegate
                {
                    UpdateResult(ref dtLTETDD_B38, drmax);
                    //dtLTETDD_B38.Rows.Add(drmax);
                    //dgvSweepResult.Refresh();
                    //Application.DoEvents();
                }));

            }
            #endregion *** Worst acp report ***

            #region --- After Test ---

            AfterTest();
            _E4438C.SetOutput(Output.OFF);
            _PS_66332A.SetOutput(Output.OFF);
            //_PS_E3631A.SetOutput(Output.OFF);
            _Arb_33522A.SetOutput(Output.OFF, Arb_Channel.Channel_1);
            _Arb_33522A.SetOutput(Output.OFF, Arb_Channel.Channel_2);

            #endregion --- After Test ---
        }





        void LTETDD_TEST_NEW(object Band)
        {
            BandList WhichBand = (BandList)Band;

            #region --- Variable Define ---

            Dictionary<double, double> lastLTETDDPin = new Dictionary<double, double>();
            int intDelay;
            int intTestID = 1;
            double Pin_Start = -10;
            double[] dblResult = new double[14];
            string waveform_name = "";

            this.Invoke((MethodInvoker)(delegate
            {
                waveform_name = cbxWaveform.SelectedItem.ToString();
            }));


            DataTable dtLTETDDTMP = new DataTable();
            dtLTETDDTMP = dtLTETDD_B38.Clone();

            if (!cbxKeepPrevious.Checked)
            {
                this.Invoke((MethodInvoker)(delegate
                {
                    if (WhichBand == BandList.B_1)
                        dtLTETDD_B38.Clear();
                    else if (WhichBand == BandList.B_2)
                        dtLTETDD_B40.Clear();
                    else
                        throw new Exception("No this band");

                    dgvSweepResult.Refresh();
                }));
            }
            #endregion --- Variable Define ---

            #region --- Initialize ---

            BeforeTest();
            
            _MXA_N9020A.Mod_Initialize(Modulation.LTETDD, true);
            _MXA_N9020A.SetFrequency(2300);

            _E4438C.SetFrequency(2300);
            _E4438C.Mode_Initialize(Modulation.LTETDD, waveform_name);
            _E4438C.SetArbTrig(Triger_Type.Continous_Reset, Triger_Source.Bus, 0); // Set Trigger to Reset Run
            _E4438C.TrigerBus();

            _E4438C.SetModOutput(Output.ON);
            _E4438C.SetOutput(Output.ON);

            SetVCC_New(TestSetting.LEVEL_VCC, 1000);
            SetVbat_New(TestSetting.LEVEL_VBAT, 100);
            SetVEN(Arb_Channel.Channel_1, TestSetting.LEVEL_TXEN, true);
            SetVEN(Arb_Channel.Channel_2, TestSetting.LEVEL_PIN_VRAMP, true);

            DateTime t_Start = DateTime.Now;

            if (cbxMipi.Checked)
            {
                string Content = "Make sure everything is setup for LTETDD testing \r\n" +
                                    "  1) Connect rf source cable to RFIN \r\n" +
                                    "  2) Connect MXA to ANT output with 30dB Pad directly \r\n" +
                                    "  3) Set mipi control to the right mode in next windows";

                this.Invoke((MethodInvoker)(delegate
                {
                    MessageBox.Show(this, Content, "LTETDD Testing", MessageBoxButtons.OK);
                    btnStop.Enabled = false;
                    Application.DoEvents();
                    frmMipi.ShowDialog();
                    wait_2_start(t_Start);
                }));
            }
            else
            {
                string Content = "Make sure everything is setup for LTETDD testing \r\n" +
                                    "  1) Connect rf source cable to RFIN \r\n" +
                                    "  2) Connect MXA to ANT output with 30dB Pad directly \r\n" +
                                    "  3) Set Control box to right mode";

                this.Invoke((MethodInvoker)(delegate
                {
                    MessageBox.Show(this, Content, "LTETDD Testing", MessageBoxButtons.OK);
                    btnStop.Enabled = false;
                    Application.DoEvents();
                    wait_2_start(t_Start);
                }));
           
            }


            #endregion --- Initialize ---

            #region --- LTETDD Test ---

            for (double dblPout_Target = TestSetting.LEVEL_START; dblPout_Target <= TestSetting.LEVEL_STOP; dblPout_Target += TestSetting.LEVEL_STEP)
            {
                if (StopTest) break;

                double PoutLL = dblPout_Target - 0.05;
                double PoutUL = dblPout_Target + 0.05;

                int intMXA_Atten = 20;
                if (dblPout_Target > 20)
                    intMXA_Atten = 20;
                else if (dblPout_Target > 10)
                    intMXA_Atten = 10;
                else
                    intMXA_Atten = 0;

                foreach (double dblFreq in TestSetting.FREQLIST)
                {
                    if (StopTest) break;

                    double LTETDD_Pout = 0;
                    double LTETDD_Pin = 0;
                    double LTETDD_Icc = 0;
                    double dblPin = Pin_Start;
                    double[] dblACPResult = new double[5];

                    if (lastLTETDDPin.ContainsKey(dblFreq)) LTETDD_Pin = dblPin = Pin_Start = lastLTETDDPin[dblFreq] + TestSetting.LEVEL_STEP;
                    _E4438C.SetFrequency(dblFreq);
                    _E4438C.SetPower(Pin_Start + TestSetting.LOSS_SRC[dblFreq]);

                    _MXA_N9020A.Config__LTETDD_CHP(dblFreq);
                    _MXA_N9020A.SetAttenuattor(20);

                    intDelay = Math.Max(intDelay_SigGen, intDelay_MXA) * 10;
                    util.Wait(intDelay);

                    LTETDD_Pout = _MXA_N9020A.Get_LTETDD_CHP_Result();
                    LTETDD_Pout += TestSetting.LOSS_MSR_THROUGH[dblFreq];

                    // Freqquency
                    dblResult[0] = dblFreq;

                    // Target Power
                    dblResult[1] = dblPout_Target;

                    //Pout
                    int loop = 0;
                    while (LTETDD_Pout <= PoutLL || LTETDD_Pout >= PoutUL)
                    {
                        if (loop++ > 15) break;
                        if ((dblPin = dblPin + dblPout_Target - LTETDD_Pout) > 8)
                        {
                            dblPin = 8;
                            _E4438C.SetPower(dblPin + TestSetting.LOSS_SRC[dblFreq]);
                            _MXA_N9020A.SetAttenuattor(intMXA_Atten);
                            util.Wait(intDelay);
                            LTETDD_Pout = _MXA_N9020A.Get_LTETDD_CHP_Result();
                            LTETDD_Pout += TestSetting.LOSS_MSR_THROUGH[dblFreq];
                            LTETDD_Pin = dblPin;

                            break;
                        }

                        _E4438C.SetPower(dblPin + TestSetting.LOSS_SRC[dblFreq]);
                        _MXA_N9020A.SetAttenuattor(intMXA_Atten);
                        util.Wait(intDelay);
                        LTETDD_Pout = _MXA_N9020A.Get_LTETDD_CHP_Result();
                        LTETDD_Pout += TestSetting.LOSS_MSR_THROUGH[dblFreq];
                        LTETDD_Pin = dblPin;
                    }
                    dblResult[2] = LTETDD_Pout;

                    // Pin
                    dblResult[3] = LTETDD_Pin;
                    if (lastLTETDDPin.ContainsKey(dblFreq))
                        lastLTETDDPin[dblFreq] = LTETDD_Pin;
                    else
                        lastLTETDDPin.Add(dblFreq, LTETDD_Pin);

                    // Gain
                    dblResult[4] = LTETDD_Pout - LTETDD_Pin;

                    // Icc
                    LTETDD_Icc = _PS_66319B.High_Current(PS_66319B_Channel.Channel_1);
                    dblResult[5] = LTETDD_Icc * 1000;
                    // PAE
                    dblResult[6] = 100 * (Math.Pow(10, ((LTETDD_Pout - 30) / 10))) / TestSetting.LEVEL_VCC / LTETDD_Icc;

                    //ACP E_ULTRA
                    _MXA_N9020A.Config__LTETDD_ACP_EULTRA(dblFreq);
                    _MXA_N9020A.SetAttenuattor(intMXA_Atten);

                    intDelay = Math.Max(intDelay_SigGen, intDelay_MXA);
                    util.Wait(intDelay * 3);

                    dblACPResult = _MXA_N9020A.Get_LTETDD_ACP_EULTRA_Result();

                    dblResult[7] = dblACPResult[1];     // -10MHz
                    dblResult[8] = dblACPResult[2];     // +10MHz

                    //ACP ULTRA
                    _MXA_N9020A.Config__LTETDD_ACP_ULTRA(dblFreq);
                    _MXA_N9020A.SetAttenuattor(intMXA_Atten);

                    intDelay = Math.Max(intDelay_SigGen, intDelay_MXA);
                    util.Wait(intDelay * 3);

                    dblACPResult = _MXA_N9020A.Get_LTETDD_ACP_ULTRA_Result();

                    dblResult[9] = dblACPResult[1];     // -5.8MHz
                    dblResult[10] = dblACPResult[2];     // -5.8MHz
                    dblResult[11] = dblACPResult[3];     // -7.4MHz
                    dblResult[12] = dblACPResult[4];     // -7.4MHz

                    // EVM
                    if (!cbxBypassEVM.Checked)
                    {
                        _MXA_N9020A.Config__LTETDD_EVM(dblFreq);
                        _MXA_N9020A.SetAttenuattor(intMXA_Atten);

                        intDelay = Math.Max(intDelay_SigGen, intDelay_MXA);
                        util.Wait(intDelay * 3);

                        dblResult[13] = _MXA_N9020A.Get_LTETDD_EVM_Result();
                    }
                    else
                    {
                        dblResult[13] = 0;
                    }
                    // Update_Grid
                    DataRow drNew = null;
                    if (WhichBand == BandList.B_1)
                        drNew = dtLTETDD_B38.NewRow();
                    else if (WhichBand == BandList.B_2)
                        drNew = dtLTETDD_B40.NewRow();
                    else
                        throw new Exception("No this band");

                    DataRow drNewtmp = dtLTETDDTMP.NewRow();
                    drNewtmp[0] = drNew[0] = intTestID++;
                    for (int i = 0; i < dblResult.Count(); i++)
                    {
                        drNew[i + 1] = drNewtmp[i + 1] = Math.Round(dblResult[i], 2);
                    }
                    dtLTETDDTMP.Rows.Add(drNewtmp);

                    this.Invoke((MethodInvoker)(delegate
                    {
                        if (WhichBand == BandList.B_1)
                            dtLTETDD_B38.Rows.Add(drNew);
                        else if (WhichBand == BandList.B_2)
                            dtLTETDD_B40.Rows.Add(drNew);
                        else
                            throw new Exception("No this band");

                        dgvSweepResult.Refresh();
                        Application.DoEvents();
                    }));
                }
            }

            #endregion --- LTETDD Test ---

            #region --- Worst acp report ---

            foreach (double dblFreq in TestSetting.FREQLIST)
            {
                DataRow drmax = null;
                if (WhichBand == BandList.B_1)
                    drmax = dtLTETDD_B38.NewRow();
                else if (WhichBand == BandList.B_2)
                    drmax = dtLTETDD_B40.NewRow();
                else
                    throw new Exception("No this band");

                drmax[0] = intTestID++;
                drmax[1] = dblFreq;

                var acp_n10 = from x in dtLTETDDTMP.AsEnumerable()
                              where x.Field<double>("Frequency (MHz)") == dblFreq
                              select x.Field<double>("ACP_EULTRA -10MHz(dB)");

                var acp_p10 = from x in dtLTETDDTMP.AsEnumerable()
                              where x.Field<double>("Frequency (MHz)") == dblFreq
                              select x.Field<double>("ACP_EULTRA +10MHz(dB)");

                var acp_n58 = from x in dtLTETDDTMP.AsEnumerable()
                              where x.Field<double>("Frequency (MHz)") == dblFreq
                              select x.Field<double>("ACP_ULTRA -0.8MHz(dB)");

                var acp_p58 = from x in dtLTETDDTMP.AsEnumerable()
                              where x.Field<double>("Frequency (MHz)") == dblFreq
                              select x.Field<double>("ACP_ULTRA +0.8MHz(dB)");

                var acp_n74 = from x in dtLTETDDTMP.AsEnumerable()
                              where x.Field<double>("Frequency (MHz)") == dblFreq
                              select x.Field<double>("ACP_ULTRA -2.4MHz(dB)");

                var acp_p74 = from x in dtLTETDDTMP.AsEnumerable()
                              where x.Field<double>("Frequency (MHz)") == dblFreq
                              select x.Field<double>("ACP_ULTRA +2.4MHz(dB)");

                var evm = from x in dtLTETDDTMP.AsEnumerable()
                          where x.Field<double>("Frequency (MHz)") == dblFreq
                          select x.Field<double>("EVM(%)");

                try
                {
                    drmax[8] = acp_n10.ToList<double>().Max();
                    drmax[9] = acp_p10.ToList<double>().Max();
                    drmax[10] = acp_n58.ToList<double>().Max();
                    drmax[11] = acp_p58.ToList<double>().Max();
                    drmax[12] = acp_n74.ToList<double>().Max();
                    drmax[13] = acp_p74.ToList<double>().Max();
                    drmax[14] = evm.ToList<double>().Max();
                }
                catch
                {
                    drmax[8] = 0;
                    drmax[9] = 0;
                    drmax[10] = 0;
                    drmax[11] = 0;
                    drmax[12] = 0;
                    drmax[13] = 0;
                    drmax[14] = 0;
                }

                this.Invoke((MethodInvoker)(delegate
                {
                    if (WhichBand == BandList.B_1)
                        dtLTETDD_B38.Rows.Add(drmax);
                    else if (WhichBand == BandList.B_2)
                        dtLTETDD_B40.Rows.Add(drmax);
                    else
                        throw new Exception("No this band");

                    dgvSweepResult.Refresh();
                    Application.DoEvents();
                }));

            }
            #endregion *** Worst acp report ***

            AfterTest();
        }
        void LTEFDD_TEST_NEW(object Band)
        {
            BandList WhichBand = (BandList)Band;

            #region --- Variable Define ---

            Dictionary<double, double> lastLTEFDDPin = new Dictionary<double, double>();
            int intDelay;
            int intTestID = 1;
            double Pin_Start = -10;
            double[] dblResult = new double[14];
            string waveform_name = "";

            this.Invoke((MethodInvoker)(delegate
            {
                waveform_name = cbxWaveform.SelectedItem.ToString();
            }));


            DataTable dtLTEFDDTMP = new DataTable();
            dtLTEFDDTMP = dtLTEFDD_B1.Clone();

            if (!cbxKeepPrevious.Checked)
            {
                this.Invoke((MethodInvoker)(delegate
                {
                    if (WhichBand == BandList.B_1)
                        dtLTEFDD_B1.Clear();
                    else if (WhichBand == BandList.B_2)
                        dtLTEFDD_B2.Clear();
                    else
                        throw new Exception("No this band");

                    dgvSweepResult.Refresh();
                }));
            }

            #endregion --- Variable Define ---

            #region --- Initialize ---

            BeforeTest();

            _MXA_N9020A.Mod_Initialize(Modulation.LTEFDD, true);
            _MXA_N9020A.SetFrequency(2496);

            _E4438C.SetFrequency(2496);
            _E4438C.Mode_Initialize(Modulation.LTEFDD, waveform_name);
            _E4438C.SetArbTrig(Triger_Type.Continous_Reset, Triger_Source.Bus, 0); // Set Trigger to Reset Run
            _E4438C.TrigerBus();

            _E4438C.SetModOutput(Output.ON);
            _E4438C.SetOutput(Output.ON);

            SetVCC_New(TestSetting.LEVEL_VCC, 1000);
            SetVbat_New(TestSetting.LEVEL_VBAT, 100);
            SetVEN(Arb_Channel.Channel_1, TestSetting.LEVEL_TXEN, true);
            SetVEN(Arb_Channel.Channel_2, TestSetting.LEVEL_PIN_VRAMP, true);

            DateTime t_Start = DateTime.Now;

            if (cbxMipi.Checked)
            {
                string Content = "Make sure everything is setup for LTEFDD testing \r\n" +
                                    "  1) Connect rf source cable to RFIN \r\n" +
                                    "  2) Connect MXA to ANT output with 30dB Pad directly \r\n" +
                                    "  3) Set mipi control to the right mode in next windows";

                this.Invoke((MethodInvoker)(delegate
                {
                    MessageBox.Show(this, Content, "LTEFDD Testing", MessageBoxButtons.OK);
                    btnStop.Enabled = false;
                    Application.DoEvents();
                    frmMipi.ShowDialog();
                    wait_2_start(t_Start);
                }));
            }
            else
            {
                string Content = "Make sure everything is setup for LTEFDD testing \r\n" +
                                    "  1) Connect rf source cable to RFIN \r\n" +
                                    "  2) Connect MXA to ANT output with 30dB Pad directly \r\n" +
                                    "  3) Set Control box to right mode";

                this.Invoke((MethodInvoker)(delegate
                {
                    MessageBox.Show(this, Content, "LTEFDD Testing", MessageBoxButtons.OK);
                    btnStop.Enabled = false;
                    Application.DoEvents();
                    wait_2_start(t_Start);
                }));

            }


            #endregion --- Initialize ---

            #region --- LTEFDD Test ---

            for (double dblPout_Target = TestSetting.LEVEL_START; dblPout_Target <= TestSetting.LEVEL_STOP; dblPout_Target += TestSetting.LEVEL_STEP)
            {
                if (StopTest) break;

                double PoutLL = dblPout_Target - 0.05;
                double PoutUL = dblPout_Target + 0.05;

                int intMXA_Atten = 20;
                if (dblPout_Target > 20)
                    intMXA_Atten = 20;
                else if (dblPout_Target > 10)
                    intMXA_Atten = 10;
                else
                    intMXA_Atten = 0;

                foreach (double dblFreq in TestSetting.FREQLIST)
                {
                    if (StopTest) break;

                    double LTEFDD_Pout = 0;
                    double LTEFDD_Pin = 0;
                    double LTEFDD_Icc = 0;
                    double dblPin = Pin_Start;
                    double[] dblACPResult = new double[5];

                    if (lastLTEFDDPin.ContainsKey(dblFreq)) LTEFDD_Pin = dblPin = Pin_Start = lastLTEFDDPin[dblFreq] + TestSetting.LEVEL_STEP;
                    _E4438C.SetFrequency(dblFreq);
                    _E4438C.SetPower(Pin_Start + TestSetting.LOSS_SRC[dblFreq]);

                    _MXA_N9020A.Config__LTEFDD_CHP(dblFreq);
                    _MXA_N9020A.SetAttenuattor(20);

                    intDelay = Math.Max(intDelay_SigGen, intDelay_MXA) * 10;
                    util.Wait(intDelay);

                    LTEFDD_Pout = _MXA_N9020A.Get_LTEFDD_CHP_Result();
                    LTEFDD_Pout += TestSetting.LOSS_MSR_THROUGH[dblFreq];

                    // Freqquency
                    dblResult[0] = dblFreq;

                    // Target Power
                    dblResult[1] = dblPout_Target;

                    //Pout
                    int loop = 0;
                    while (LTEFDD_Pout <= PoutLL || LTEFDD_Pout >= PoutUL)
                    {
                        if (loop++ > 15) break;
                        if ((dblPin = dblPin + dblPout_Target - LTEFDD_Pout) > 8)
                        {
                            dblPin = 8;
                            _E4438C.SetPower(dblPin + TestSetting.LOSS_SRC[dblFreq]);
                            _MXA_N9020A.SetAttenuattor(intMXA_Atten);
                            util.Wait(intDelay);
                            LTEFDD_Pout = _MXA_N9020A.Get_LTEFDD_CHP_Result();
                            LTEFDD_Pout += TestSetting.LOSS_MSR_THROUGH[dblFreq];
                            LTEFDD_Pin = dblPin;

                            break;
                        }

                        _E4438C.SetPower(dblPin + TestSetting.LOSS_SRC[dblFreq]);
                        _MXA_N9020A.SetAttenuattor(intMXA_Atten);
                        util.Wait(intDelay);
                        LTEFDD_Pout = _MXA_N9020A.Get_LTEFDD_CHP_Result();
                        LTEFDD_Pout += TestSetting.LOSS_MSR_THROUGH[dblFreq];
                        LTEFDD_Pin = dblPin;

                    }
                    dblResult[2] = LTEFDD_Pout;

                    // Pin
                    dblResult[3] = LTEFDD_Pin;
                    if (lastLTEFDDPin.ContainsKey(dblFreq))
                        lastLTEFDDPin[dblFreq] = LTEFDD_Pin;
                    else
                        lastLTEFDDPin.Add(dblFreq, LTEFDD_Pin);

                    // Gain
                    dblResult[4] = LTEFDD_Pout - LTEFDD_Pin;

                    // Icc
                    LTEFDD_Icc = _PS_66332A.High_Current();
                    dblResult[5] = LTEFDD_Icc * 1000;
                    // PAE
                    dblResult[6] = 100 * (Math.Pow(10, ((LTEFDD_Pout - 30) / 10))) / TestSetting.LEVEL_VCC / LTEFDD_Icc;

                    //ACP E_ULTRA
                    _MXA_N9020A.Config__LTEFDD_ACP_EULTRA(dblFreq);
                    _MXA_N9020A.SetAttenuattor(intMXA_Atten);
                    
                    intDelay = Math.Max(intDelay_SigGen, intDelay_MXA);
                    util.Wait(intDelay * 3);

                    dblACPResult = _MXA_N9020A.Get_LTEFDD_ACP_EULTRA_Result();

                    dblResult[7] = dblACPResult[1];     // -10MHz
                    dblResult[8] = dblACPResult[2];     // +10MHz

                    //ACP ULTRA
                    _MXA_N9020A.Config__LTEFDD_ACP_ULTRA(dblFreq);
                    _MXA_N9020A.SetAttenuattor(intMXA_Atten);
                    
                    intDelay = Math.Max(intDelay_SigGen, intDelay_MXA);
                    util.Wait(intDelay * 3);

                    dblACPResult = _MXA_N9020A.Get_LTEFDD_ACP_ULTRA_Result();

                    dblResult[9] = dblACPResult[1];     // -5.8MHz
                    dblResult[10] = dblACPResult[2];     // -5.8MHz
                    dblResult[11] = dblACPResult[3];     // -7.4MHz
                    dblResult[12] = dblACPResult[4];     // -7.4MHz

                    // EVM
                    if (!cbxBypassEVM.Checked)
                    {
                        _MXA_N9020A.Config__LTEFDD_EVM(dblFreq);
                        _MXA_N9020A.SetAttenuattor(intMXA_Atten);
                    
                        intDelay = Math.Max(intDelay_SigGen, intDelay_MXA);
                        util.Wait(intDelay * 3);

                        dblResult[13] = _MXA_N9020A.Get_LTEFDD_EVM_Result();
                    }
                    else
                    {
                        dblResult[13] = 0;
                    }
                    // Update_Grid
                    DataRow drNew = null;
                    if (WhichBand == BandList.B_1)
                        drNew = dtLTEFDD_B1.NewRow();
                    else if (WhichBand == BandList.B_2)
                        drNew = dtLTEFDD_B2.NewRow();
                    else
                        throw new Exception("No this band");

                    DataRow drNewtmp = dtLTEFDDTMP.NewRow();
                    drNewtmp[0] = drNew[0] = intTestID++;
                    for (int i = 0; i < dblResult.Count(); i++)
                    {
                        drNew[i + 1] = drNewtmp[i + 1] = Math.Round(dblResult[i], 2);
                    }
                    dtLTEFDDTMP.Rows.Add(drNewtmp);

                    this.Invoke((MethodInvoker)(delegate
                    {
                        if (WhichBand == BandList.B_1)
                            dtLTEFDD_B1.Rows.Add(drNew);
                        else if (WhichBand == BandList.B_2)
                            dtLTEFDD_B2.Rows.Add(drNew);
                        else
                            throw new Exception("No this band");

                        dgvSweepResult.Refresh();
                        Application.DoEvents();
                    }));
                }
            }

            #endregion --- LTEFDD Test ---

            #region --- Worst acp report ---

            foreach (double dblFreq in TestSetting.FREQLIST)
            {
                DataRow drmax = null;
                if (WhichBand == BandList.B_1)
                    drmax = dtLTEFDD_B1.NewRow();
                else if (WhichBand == BandList.B_2)
                    drmax = dtLTEFDD_B2.NewRow();
                else
                    throw new Exception("No this band");

                drmax[0] = intTestID++;
                drmax[1] = dblFreq;

                var acp_n10 = from x in dtLTEFDDTMP.AsEnumerable()
                              where x.Field<double>("Frequency (MHz)") == dblFreq
                              select x.Field<double>("ACP_EULTRA -10MHz(dB)");

                var acp_p10 = from x in dtLTEFDDTMP.AsEnumerable()
                              where x.Field<double>("Frequency (MHz)") == dblFreq
                              select x.Field<double>("ACP_EULTRA +10MHz(dB)");

                var acp_n25 = from x in dtLTEFDDTMP.AsEnumerable()
                              where x.Field<double>("Frequency (MHz)") == dblFreq
                              select x.Field<double>("ACP_ULTRA -2.5MHz(dB)");

                var acp_p25 = from x in dtLTEFDDTMP.AsEnumerable()
                              where x.Field<double>("Frequency (MHz)") == dblFreq
                              select x.Field<double>("ACP_ULTRA +2.5MHz(dB)");

                var acp_n50 = from x in dtLTEFDDTMP.AsEnumerable()
                              where x.Field<double>("Frequency (MHz)") == dblFreq
                              select x.Field<double>("ACP_ULTRA -7.5MHz(dB)");

                var acp_p50 = from x in dtLTEFDDTMP.AsEnumerable()
                              where x.Field<double>("Frequency (MHz)") == dblFreq
                              select x.Field<double>("ACP_ULTRA +7.5MHz(dB)");

                var evm = from x in dtLTEFDDTMP.AsEnumerable()
                          where x.Field<double>("Frequency (MHz)") == dblFreq
                          select x.Field<double>("EVM(%)");

                try
                {
                    drmax[8] = acp_n10.ToList<double>().Max();
                    drmax[9] = acp_p10.ToList<double>().Max();
                    drmax[10] = acp_n25.ToList<double>().Max();
                    drmax[11] = acp_p25.ToList<double>().Max();
                    drmax[12] = acp_n50.ToList<double>().Max();
                    drmax[13] = acp_p50.ToList<double>().Max();
                    drmax[14] = evm.ToList<double>().Max();
                }
                catch
                {
                    drmax[8] = 0;
                    drmax[9] = 0;
                    drmax[10] = 0;
                    drmax[11] = 0;
                    drmax[12] = 0;
                    drmax[13] = 0;
                    drmax[14] = 0;
                }

                this.Invoke((MethodInvoker)(delegate
                {
                    if (WhichBand == BandList.B_1)
                        dtLTEFDD_B1.Rows.Add(drmax);
                    else if (WhichBand == BandList.B_2)
                        dtLTEFDD_B2.Rows.Add(drmax);
                    else
                        throw new Exception("No this band");

                    dgvSweepResult.Refresh();
                    Application.DoEvents();
                }));

            }
            #endregion *** Worst acp report ***

            AfterTest();
        }
        void WCDMA_TEST_NEW()
        {
            #region --- Variable Define ---

            Dictionary<double, double> lastPin = new Dictionary<double, double>();
            int intDelay;
            int intTestID = 1;
            double Pin_Start = -10;
            double[] dblResult = new double[12];

            DataTable dtWCDMATMP = new DataTable();
            dtWCDMATMP = dtWCDMA.Clone();

            if (!cbxKeepPrevious.Checked)
            {
                this.Invoke((MethodInvoker)(delegate
                {
                    dtWCDMA.Clear();
                    dgvSweepResult.Refresh();
                }));
            }

            #endregion --- Variable Define ---

            #region --- Initialize ---

            BeforeTest();

            _E4438C.Mode_Initialize(Modulation.WCDMA);
            _E4438C.SetArbTrig(Triger_Type.Continous_Reset, Triger_Source.Bus, 0); // Set Trigger to Reset Run
            _E4438C.TrigerBus();

            _MXA_N9020A.Mod_Initialize(Modulation.WCDMA);

            _E4438C.SetModOutput(Output.ON);
            _E4438C.SetOutput(Output.OFF);

            SetVCC_New(TestSetting.LEVEL_VCC, 1000);
            SetVbat_New(TestSetting.LEVEL_VBAT, 100);
            SetVEN(Arb_Channel.Channel_2, TestSetting.LEVEL_PIN_VRAMP, true);

            DateTime t_Start = DateTime.Now;

            if (cbxMipi.Checked)
            {
                string Content = "Make sure everything is setup for WCDMA testing \r\n" +
                                    "  1) Connect rf source cable to RFIN \r\n" +
                                    "  2) Connect MXA to ANT output with 30dB Pad directly \r\n" +
                                    "  3) Set mipi control to the right mode in next windows";

                this.Invoke((MethodInvoker)(delegate
                {
                    MessageBox.Show(this, Content, "WCDMA Testing", MessageBoxButtons.OK);
                    btnStop.Enabled = false;
                    Application.DoEvents();
                    frmMipi.ShowDialog();
                    _E4438C.SetOutput(Output.ON);
                    wait_2_start(t_Start);
                }));
            }
            else
            {
                string Content = "Make sure everything is setup for WCDMA testing \r\n" +
                                    "  1) Connect rf source cable to RFIN \r\n" +
                                    "  2) Connect MXA to ANT output with 30dB Pad directly \r\n" +
                                    "  3) Set Control box to right mode";

                this.Invoke((MethodInvoker)(delegate
                {
                    MessageBox.Show(this, Content, "WCDMA Testing", MessageBoxButtons.OK);
                    btnStop.Enabled = false;
                    Application.DoEvents();
                    _E4438C.SetOutput(Output.ON);
                    wait_2_start(t_Start);
                }));

            }

            #endregion --- Initialize ---

            #region --- WCDMA Test ---

            for (double dblPout_Target = TestSetting.LEVEL_START; dblPout_Target <= TestSetting.LEVEL_STOP; dblPout_Target += TestSetting.LEVEL_STEP)
            {
                if (StopTest) break;

                double PoutLL = dblPout_Target - 0.05;
                double PoutUL = dblPout_Target + 0.05;

                int intMXA_Atten = 20;
                if (dblPout_Target > 20)
                    intMXA_Atten = 20;
                else if (dblPout_Target > 10)
                    intMXA_Atten = 10;
                else
                    intMXA_Atten = 0;

                foreach (double dblFreq in TestSetting.FREQLIST)
                {
                    if (StopTest) break;

                    double WCDMA_Pout = 0;
                    double WCDMA_Pin = 0;
                    double WCDMA_Icc = 0;
                    double dblPin = Pin_Start;
                    double[] dblACPResult = new double[5];

                    if (lastPin.ContainsKey(dblFreq)) WCDMA_Pin = dblPin = Pin_Start = lastPin[dblFreq] + TestSetting.LEVEL_STEP;
                    _E4438C.SetFrequency(dblFreq);
                    _E4438C.SetPower(Pin_Start + TestSetting.LOSS_SRC[dblFreq]);

                    _MXA_N9020A.Config__WCDMA_CHP(dblFreq);
                    _MXA_N9020A.SetAttenuattor(20);

                    intDelay = Math.Max(intDelay_SigGen, intDelay_MXA) * 10;
                    //_PM_N1913A.Configure__CW_Power(dblFreq, 10);
                    //intDelay = Math.Max(intDelay_SigGen, intDelay_PowerMeter) * 2;
                    util.Wait(intDelay);

                    WCDMA_Pout = _MXA_N9020A.Get_WCDMA_CHP_Result();
                    WCDMA_Pout += TestSetting.LOSS_MSR_THROUGH[dblFreq];
                    //WCDMA_Pout = _PM_N1913A.GetPowerResult();
                    //WCDMA_Pout += TestSetting.LOSS_MSR_POUT[dblFreq];

                    // Freqquency
                    dblResult[0] = dblFreq;

                    // Target Power
                    dblResult[1] = dblPout_Target;

                    // Pout
                    int loop = 0;
                    while (WCDMA_Pout <= PoutLL || WCDMA_Pout >= PoutUL)
                    {
                        if (loop++ > 15) break;
                        if ((dblPin = dblPin + dblPout_Target - WCDMA_Pout) > 8)
                        {
                            dblPin = 8;
                            _MXA_N9020A.SetAttenuattor(intMXA_Atten);
                            _E4438C.SetPower(dblPin + TestSetting.LOSS_SRC[dblFreq]);
                            util.Wait(intDelay);
                            WCDMA_Pout = _MXA_N9020A.Get_WCDMA_CHP_Result();
                            WCDMA_Pout += TestSetting.LOSS_MSR_THROUGH[dblFreq];
                            //WCDMA_Pout = _PM_N1913A.GetPowerResult();
                            //WCDMA_Pout += TestSetting.LOSS_MSR_POUT[dblFreq];
                            WCDMA_Pin = dblPin;

                            break;
                        }

                        _E4438C.SetPower(dblPin + TestSetting.LOSS_SRC[dblFreq]);
                        util.Wait(intDelay);
                        WCDMA_Pout = _MXA_N9020A.Get_WCDMA_CHP_Result();
                        WCDMA_Pout += TestSetting.LOSS_MSR_THROUGH[dblFreq];
                        //WCDMA_Pout = _PM_N1913A.GetPowerResult();
                        //WCDMA_Pout += TestSetting.LOSS_MSR_POUT[dblFreq];
                        WCDMA_Pin = dblPin;
                    }
                    dblResult[2] = WCDMA_Pout;

                    // Pin
                    dblResult[3] = WCDMA_Pin;

                    if (lastPin.ContainsKey(dblFreq))
                        lastPin[dblFreq] = WCDMA_Pin;
                    else
                        lastPin.Add(dblFreq, WCDMA_Pin);
                    // Gain
                    dblResult[4] = WCDMA_Pout - WCDMA_Pin;

                    // Icc
                    WCDMA_Icc = _PS_66332A.RMS_Current();
                    dblResult[5] = WCDMA_Icc * 1000;
                    // PAE
                    dblResult[6] = 100 * (Math.Pow(10, ((WCDMA_Pout - 30) / 10))) / TestSetting.LEVEL_VCC / WCDMA_Icc;

                    // ACP
                    _MXA_N9020A.Config__WCDMA_ACP(dblFreq);
                    _MXA_N9020A.SetAttenuattor(intMXA_Atten);

                    intDelay = Math.Max(intDelay_SigGen, intDelay_MXA);
                    util.Wait(intDelay * 3);

                    dblACPResult = _MXA_N9020A.Get_WCDMA_ACP_Result();

                    dblResult[7] = dblACPResult[1];     // -5MHz
                    dblResult[8] = dblACPResult[2];     // +5MHz
                    dblResult[9] = dblACPResult[3];     // -10MHz
                    dblResult[10] = dblACPResult[4];     // +10MHz

                    // EVM
                    if (!cbxBypassEVM.Checked)
                    {
                        _MXA_N9020A.Config__WCDMA_EVM(dblFreq);
                        _MXA_N9020A.SetAttenuattor(intMXA_Atten);

                        intDelay = Math.Max(intDelay_SigGen, intDelay_MXA);
                        util.Wait(intDelay * 3);

                        dblResult[11] = _MXA_N9020A.Get_WCDMA_EVM_Result();
                    }
                    else
                    {
                        dblResult[11] = 0;
                    }
                    // Update_Grid
                    DataRow drNew = dtWCDMA.NewRow();
                    DataRow drNewtmp = dtWCDMATMP.NewRow();
                    drNew[0] = intTestID++;
                    for (int i = 0; i < dblResult.Count(); i++)
                    {
                        drNew[i + 1] = drNewtmp[i + 1] = Math.Round(dblResult[i], 2);
                    }
                    dtWCDMATMP.Rows.Add(drNewtmp);

                    this.Invoke((MethodInvoker)(delegate
                    {
                        dtWCDMA.Rows.Add(drNew);
                        dgvSweepResult.Refresh();
                        Application.DoEvents();
                    }));
                }
            }

            #endregion --- WCDMA Test ---

            #region --- Worst acp report ---

            foreach (double dblFreq in TestSetting.FREQLIST)
            {
                DataRow drmax = dtWCDMA.NewRow();
                drmax[0] = intTestID++;
                drmax[1] = dblFreq;

                var acp_n5 = from x in dtWCDMATMP.AsEnumerable()
                             where x.Field<double>("Frequency (MHz)") == dblFreq
                             select x.Field<double>("ACP -5MHz(dB)");

                var acp_p5 = from x in dtWCDMATMP.AsEnumerable()
                             where x.Field<double>("Frequency (MHz)") == dblFreq
                             select x.Field<double>("ACP +5MHz(dB)");

                var acp_n10 = from x in dtWCDMATMP.AsEnumerable()
                              where x.Field<double>("Frequency (MHz)") == dblFreq
                              select x.Field<double>("ACP -10MHz(dB)");

                var acp_p10 = from x in dtWCDMATMP.AsEnumerable()
                              where x.Field<double>("Frequency (MHz)") == dblFreq
                              select x.Field<double>("ACP +10MHz(dB)");

                var evm = from x in dtWCDMATMP.AsEnumerable()
                          where x.Field<double>("Frequency (MHz)") == dblFreq
                          select x.Field<double>("EVM(%)");

                try
                {
                    drmax[8] = acp_n5.ToList<double>().Max();
                    drmax[9] = acp_p5.ToList<double>().Max();
                    drmax[10] = acp_n10.ToList<double>().Max();
                    drmax[11] = acp_p10.ToList<double>().Max();
                    drmax[12] = evm.ToList<double>().Max();
                }
                catch
                {
                    drmax[8] = 0;
                    drmax[9] = 0;
                    drmax[10] = 0;
                    drmax[11] = 0;
                    drmax[12] = 0;
                }

                this.Invoke((MethodInvoker)(delegate
                {
                    dtWCDMA.Rows.Add(drmax);
                    dgvSweepResult.Refresh();
                    Application.DoEvents();
                }));

            }
            #endregion *** Worst acp report ***

            AfterTest();

        }
        void TDSCDMA_TEST_NEW()
        {
            #region --- Variable Define ---

            Dictionary<double, double> lastTDSCDMAPin = new Dictionary<double, double>();
            int intDelay;
            int intTestID = 1;
            double Pin_Start = -10;
            double[] dblResult = new double[12];
            string waveform_name = "";

            DataTable dtTDSCDMATMP = new DataTable();
            dtTDSCDMATMP = dtTDSCDMA.Clone();

            this.Invoke((MethodInvoker)(delegate
            {
                waveform_name = cbxWaveform.SelectedItem.ToString();

                if (!cbxKeepPrevious.Checked)
                {
                    dtTDSCDMA.Clear();
                    dgvSweepResult.Refresh();
                }
            }));

            #endregion --- Variable Define ---

            #region --- Initialize ---

            BeforeTest();

            _E4438C.Mode_Initialize(Modulation.TDSCDMA, waveform_name);
            _E4438C.SetArbTrig(Triger_Type.Continous_Reset, Triger_Source.Bus, 0);
            _E4438C.TrigerBus();

            _MXA_N9020A.Mod_Initialize(Modulation.TDSCDMA);

            _E4438C.SetModOutput(Output.ON);
            _E4438C.SetOutput(Output.ON);

            SetVCC_New(TestSetting.LEVEL_VCC, 1000);
            SetVbat_New(TestSetting.LEVEL_VBAT, 100);


            DateTime t_Start = DateTime.Now;

            if (cbxMipi.Checked)
            {
                SetVEN(Arb_Channel.Channel_2, TestSetting.LEVEL_PIN_VRAMP, true);

                string Content = "Make sure everything is setup for TDSCDMA testing \r\n" +
                                    "  1) Connect rf source cable to RFIN \r\n" +
                                    "  2) Connect MXA to ANT output with 30dB Pad directly \r\n" +
                                    "  3) Set mipi control to the right mode in next windows";

                this.Invoke((MethodInvoker)(delegate
                {
                    MessageBox.Show(this, Content, "TDSCDMA Testing", MessageBoxButtons.OK);
                    btnStop.Enabled = false;
                    Application.DoEvents();
                    frmMipi.ShowDialog();
                    wait_2_start(t_Start);
                }));
            }
            else
            {
                Arb_Init();
                SetVEN_TDS(TestSetting.LEVEL_TXEN);
                SetVEN(Arb_Channel.Channel_2, TestSetting.LEVEL_PIN_VRAMP, true);

                string Content = "Make sure everything is setup for TDSCDMA testing \r\n" +
                                    "  1) Connect rf source cable to RFIN \r\n" +
                                    "  2) Connect MXA to ANT output with 30dB Pad directly \r\n" +
                                    "  3) Set Control box to right mode";

                this.Invoke((MethodInvoker)(delegate
                {
                    MessageBox.Show(this, Content, "TDSCDMA Testing", MessageBoxButtons.OK);
                    btnStop.Enabled = false;
                    Application.DoEvents();
                    wait_2_start(t_Start);
                }));

            }

            #endregion --- Initialize ---

            #region --- TDSCDMA Test ---

            for (double dblPout_Target = TestSetting.LEVEL_START; dblPout_Target <= TestSetting.LEVEL_STOP; dblPout_Target += TestSetting.LEVEL_STEP)
            {
                if (StopTest) break;

                double PoutLL = dblPout_Target - 0.05;
                double PoutUL = dblPout_Target + 0.05;

                int intMXA_Atten = 20;
                if (dblPout_Target > 20)
                    intMXA_Atten = 20;
                else if (dblPout_Target > 10)
                    intMXA_Atten = 10;
                else
                    intMXA_Atten = 0;

                foreach (double dblFreq in TestSetting.FREQLIST)
                {
                    if (StopTest) break;

                    double TDSCDMA_Pout = 0;
                    double TDSCDMA_Pin = 0;
                    double TDSCDMA_Icc = 0;
                    double dblPin = Pin_Start;
                    double[] dblACPResult = new double[5];

                    if (lastTDSCDMAPin.ContainsKey(dblFreq)) TDSCDMA_Pin = dblPin = Pin_Start = lastTDSCDMAPin[dblFreq] + TestSetting.LEVEL_STEP;
                    _E4438C.SetFrequency(dblFreq);
                    _E4438C.SetPower(Pin_Start + TestSetting.LOSS_SRC[dblFreq]);

                    _MXA_N9020A.Config__TDSCDMA_CHP(dblFreq);
                    _MXA_N9020A.SetAttenuattor(20);

                    intDelay = Math.Max(intDelay_SigGen, intDelay_MXA) * 10;
                    util.Wait(intDelay);

                    TDSCDMA_Pout = _MXA_N9020A.Get_TDSCDMA_CHP_Result();
                    TDSCDMA_Pout += TestSetting.LOSS_MSR_THROUGH[dblFreq];

                    // Freqquency
                    dblResult[0] = dblFreq;

                    // Target Power
                    dblResult[1] = dblPout_Target;

                    //Pout
                    int loop = 0;
                    while (TDSCDMA_Pout <= PoutLL || TDSCDMA_Pout >= PoutUL)
                    {
                        if (loop++ > 15) break;
                        if ((dblPin = dblPin + dblPout_Target - TDSCDMA_Pout) > 8)
                        {
                            dblPin = 8;
                            _E4438C.SetPower(dblPin + TestSetting.LOSS_SRC[dblFreq]);
                            _MXA_N9020A.SetAttenuattor(intMXA_Atten);
                            util.Wait(intDelay);
                            TDSCDMA_Pout = _MXA_N9020A.Get_TDSCDMA_CHP_Result();
                            TDSCDMA_Pout += TestSetting.LOSS_MSR_THROUGH[dblFreq];
                            TDSCDMA_Pin = dblPin;

                            break;
                        }

                        _E4438C.SetPower(dblPin + TestSetting.LOSS_SRC[dblFreq]);
                        _MXA_N9020A.SetAttenuattor(intMXA_Atten);
                        util.Wait(intDelay);
                        TDSCDMA_Pout = _MXA_N9020A.Get_TDSCDMA_CHP_Result();
                        TDSCDMA_Pout += TestSetting.LOSS_MSR_THROUGH[dblFreq];
                        TDSCDMA_Pin = dblPin;

                    }
                    dblResult[2] = TDSCDMA_Pout;

                    // Pin
                    dblResult[3] = TDSCDMA_Pin;
                    if (lastTDSCDMAPin.ContainsKey(dblFreq))
                        lastTDSCDMAPin[dblFreq] = TDSCDMA_Pin;
                    else
                        lastTDSCDMAPin.Add(dblFreq, TDSCDMA_Pin);

                    // Gain
                    dblResult[4] = TDSCDMA_Pout - TDSCDMA_Pin;

                    // Icc
                    TDSCDMA_Icc = _PS_66332A.High_Current();
                    dblResult[5] = TDSCDMA_Icc * 1000;
                    // PAE
                    dblResult[6] = 100 * (Math.Pow(10, ((TDSCDMA_Pout - 30) / 10))) / TestSetting.LEVEL_VCC / TDSCDMA_Icc;

                    //ACP
                    _MXA_N9020A.Config__TDSCDMA_ACP(dblFreq, 5);
                    _MXA_N9020A.SetAttenuattor(intMXA_Atten);

                    intDelay = Math.Max(intDelay_SigGen, intDelay_MXA);
                    util.Wait(intDelay * 3);

                    dblACPResult = _MXA_N9020A.Get_TDSCDMA_ACP_Result();

                    dblResult[7] = dblACPResult[1];     // -5MHz
                    dblResult[8] = dblACPResult[2];     // +5MHz
                    dblResult[9] = dblACPResult[3];     // -10MHz
                    dblResult[10] = dblACPResult[4];     // +10MHz

                    // EVM
                    if (!cbxBypassEVM.Checked)
                    {
                        _MXA_N9020A.Config__TDSCDMA_EVM(dblFreq);
                        _MXA_N9020A.SetAttenuattor(intMXA_Atten);

                        intDelay = Math.Max(intDelay_SigGen, intDelay_MXA);
                        util.Wait(intDelay * 3);

                        dblResult[11] = _MXA_N9020A.Get_TDSCDMA_EVM_Result();
                    }
                    else
                    {
                        dblResult[11] = 0;
                    }
                    // Update_Grid
                    DataRow drNew = dtTDSCDMA.NewRow();
                    DataRow drNewtmp = dtTDSCDMATMP.NewRow();
                    drNewtmp[0] = drNew[0] = intTestID++;
                    for (int i = 0; i < dblResult.Count(); i++)
                    {
                        drNew[i + 1] = drNewtmp[i + 1] = Math.Round(dblResult[i], 2);
                    }
                    dtTDSCDMATMP.Rows.Add(drNewtmp);

                    this.Invoke((MethodInvoker)(delegate
                    {
                        dtTDSCDMA.Rows.Add(drNew);
                        dgvSweepResult.Refresh();
                        Application.DoEvents();
                    }));
                }
            }

            #endregion --- TDSCDMA Test ---

            #region --- Worst acp report ---

            foreach (double dblFreq in TestSetting.FREQLIST)
            {
                DataRow drmax = dtTDSCDMA.NewRow();
                drmax[0] = intTestID++;
                drmax[1] = dblFreq;

                var acp_n5 = from x in dtTDSCDMATMP.AsEnumerable()
                             where x.Field<double>("Frequency (MHz)") == dblFreq
                             select x.Field<double>("ACP -1.6MHz(dB)");

                var acp_p5 = from x in dtTDSCDMATMP.AsEnumerable()
                             where x.Field<double>("Frequency (MHz)") == dblFreq
                             select x.Field<double>("ACP +1.6MHz(dB)");

                var acp_n10 = from x in dtTDSCDMATMP.AsEnumerable()
                              where x.Field<double>("Frequency (MHz)") == dblFreq
                              select x.Field<double>("ACP -3.2MHz(dB)");

                var acp_p10 = from x in dtTDSCDMATMP.AsEnumerable()
                              where x.Field<double>("Frequency (MHz)") == dblFreq
                              select x.Field<double>("ACP +3.2MHz(dB)");

                var evm = from x in dtTDSCDMATMP.AsEnumerable()
                          where x.Field<double>("Frequency (MHz)") == dblFreq
                          select x.Field<double>("EVM(%)");

                try
                {
                    drmax[8] = acp_n5.ToList<double>().Max();
                    drmax[9] = acp_p5.ToList<double>().Max();
                    drmax[10] = acp_n10.ToList<double>().Max();
                    drmax[11] = acp_p10.ToList<double>().Max();
                    drmax[12] = evm.ToList<double>().Max();
                }
                catch
                {
                    drmax[8] = 0;
                    drmax[9] = 0;
                    drmax[10] = 0;
                    drmax[11] = 0;
                    drmax[12] = 0;
                }

                this.Invoke((MethodInvoker)(delegate
                {
                    dtTDSCDMA.Rows.Add(drmax);
                    dgvSweepResult.Refresh();
                    Application.DoEvents();
                }));

            }
            #endregion *** Worst acp report ***

            AfterTest();
        }
        void EDGE_TEST_NEW(object Band)
        {
            BandList WhichBand = (BandList)Band;

            #region --- Variable Define ---

            Dictionary<double, double> lastEDGELBPin = new Dictionary<double, double>();
            int intDelay;
            int intTestID = 1;
            double Pin_Start = -10;
            double[] dblResult = new double[10];

            DataTable dtEDGETMP = new DataTable();
            dtEDGETMP = dtEDGELB.Clone();

            if (!cbxKeepPrevious.Checked)
            {
                this.Invoke((MethodInvoker)(delegate
                {
                    if (WhichBand == BandList.B_1)
                        dtEDGELB.Clear();
                    else if (WhichBand == BandList.B_2)
                        dtEDGEHB.Clear();
                    else
                        throw new Exception("No this band");

                    dgvSweepResult.Refresh();
                }));
            }

            #endregion --- Variable Define ---

            #region --- Initialize ---

            BeforeTest();

            SetVCC_New(TestSetting.LEVEL_VCC,1500);
            SetGPCTRL(TestSetting.LEVEL_GPCTRL);

            _E4438C.Mode_Initialize(Modulation.EDGE, Mod_Waveform_Name.EDGE);
            _E4438C.SetArbTrig(Triger_Type.Continous_Reset, Triger_Source.Bus, 0);
            _E4438C.TrigerBus();
            _E4438C.SetModOutput(Output.ON);
            _E4438C.SetOutput(Output.ON);

            _MXA_N9020A.Mod_Initialize(Modulation.EDGE);

            DateTime t_Start = DateTime.Now;

            if (cbxMipi.Checked)
            {
                SetVEN(Arb_Channel.Channel_2, TestSetting.LEVEL_PIN_VRAMP, true);

                string Content = "Make sure everything is setup for EDGE testing \r\n" +
                                  "  1) Connect rf source cable to EDGE \r\n" +
                                  "  2) Set mipi control to the right mode in next windows \r\n" +
                                  "  3) Connect MXA to ANT output with 30dB Pad directly";

                this.Invoke((MethodInvoker)(delegate
                {
                    MessageBox.Show(this, Content, "EDGE Testing", MessageBoxButtons.OK);
                    Application.DoEvents();
                    frmMipi.ShowDialog();
                    wait_2_start(t_Start);
                }));
            }
            else
            {
                Arb_Init();
                SetVEN_EDGE(TestSetting.LEVEL_TXEN);
                SetVEN(Arb_Channel.Channel_2, TestSetting.LEVEL_PIN_VRAMP, true);

                string Content = "Make sure everything is setup for EDGE testing \r\n" +
                                  "  1) Connect rf source cable to EDGE \r\n" +
                                  "  2) Set Control box to EDGE mode \r\n" +
                                  "  3) Connect MXA to ANT output with 30dB Pad directly";

                this.Invoke((MethodInvoker)(delegate
                {
                    MessageBox.Show(this, Content, "EDGE Testing", MessageBoxButtons.OK);
                    Application.DoEvents();
                    wait_2_start(t_Start);
                }));
            }


            #endregion --- Initialize ---

            #region --- EDGE Test ---

            for (double dblPout_Target = TestSetting.LEVEL_START; dblPout_Target <= TestSetting.LEVEL_STOP; dblPout_Target += TestSetting.LEVEL_STEP)
            {
                if (StopTest) break;

                double PoutLL = dblPout_Target - 0.05;
                double PoutUL = dblPout_Target + 0.05;

                int intMXA_Atten = 20;
                if (dblPout_Target > 20)
                    intMXA_Atten = 20;
                else if (dblPout_Target > 10)
                    intMXA_Atten = 10;
                else
                    intMXA_Atten = 0;

                foreach (double dblFreq in TestSetting.FREQLIST)
                {
                    if (StopTest) break;

                    int Count = 0;
                    double EDGE_Pout = 0;
                    double EDGE_Pin = 0;
                    double EDGE_Icc = 0;
                    double dblPin = Pin_Start;
                    double[] dblACPResult = new double[10];

                    if (lastEDGELBPin.ContainsKey(dblFreq)) EDGE_Pin = dblPin = Pin_Start = lastEDGELBPin[dblFreq] + TestSetting.LEVEL_STEP;
                    _E4438C.SetFrequency(dblFreq);
                    _E4438C.SetPower(Pin_Start + TestSetting.LOSS_SRC[dblFreq]);

                    _MXA_N9020A.Config__EDGE_Burst_Power(dblFreq);
                    _MXA_N9020A.SetAttenuattor(20);

                    intDelay = Math.Max(intDelay_SigGen, intDelay_MXA) * 5;
                    util.Wait(intDelay);

                    EDGE_Pout = _MXA_N9020A.Get_EDGE_Burst_Power_Result();
                    EDGE_Pout += TestSetting.LOSS_MSR_THROUGH[dblFreq];

                    // Freqquency
                    dblResult[0] = dblFreq;

                    // Target Power
                    dblResult[1] = dblPout_Target;

                    //Pout
                    int loop = 0;
                    while (EDGE_Pout <= PoutLL || EDGE_Pout >= PoutUL)
                    {
                        if (loop++ > 15) break;
                        if ((dblPin = dblPin + dblPout_Target - EDGE_Pout) > 8)
                        {
                            dblPin = 8;
                            _E4438C.SetPower(dblPin + TestSetting.LOSS_SRC[dblFreq]);
                            _MXA_N9020A.SetAttenuattor(intMXA_Atten);
                            util.Wait(intDelay);
                            EDGE_Pout = _MXA_N9020A.Get_EDGE_Burst_Power_Result();
                            EDGE_Pout += TestSetting.LOSS_MSR_THROUGH[dblFreq];
                            EDGE_Pin = dblPin;

                            break;
                        }

                        _E4438C.SetPower(dblPin + TestSetting.LOSS_SRC[dblFreq]);
                        _MXA_N9020A.SetAttenuattor(intMXA_Atten);
                        util.Wait(intDelay);
                        EDGE_Pout = _MXA_N9020A.Get_EDGE_Burst_Power_Result();
                        EDGE_Pout += TestSetting.LOSS_MSR_THROUGH[dblFreq];
                        EDGE_Pin = dblPin;

                    }
                    dblResult[2] = EDGE_Pout;

                    // Pin
                    dblResult[3] = EDGE_Pin;
                    if (lastEDGELBPin.ContainsKey(dblFreq))
                        lastEDGELBPin[dblFreq] = EDGE_Pin;
                    else
                        lastEDGELBPin.Add(dblFreq, EDGE_Pin);

                    // Gain
                    dblResult[4] = EDGE_Pout - EDGE_Pin;

                    // Icc
                    _PS_66332A.High_Current_Set();

                    _E4438C.SetArbTrig(Triger_Type.Continous_Reset, Triger_Source.Bus, 0);
                    _E4438C.TrigerBus();

                    EDGE_Icc = _PS_66332A.High_Current_Read();

                    _PS_66332A.High_Current_Set();
                    _E4438C.Write(":SOUR:RAD:ARB OFF");
                    _E4438C.Write(":SOUR:RAD:ARB ON");
                    _E4438C.TrigerBus();

                    EDGE_Icc = _PS_66332A.High_Current_Read();

                    dblResult[5] = EDGE_Icc * 1000;
                    // PAE
                    dblResult[6] = 100 * (Math.Pow(10, ((EDGE_Pout - 30) / 10))) / TestSetting.LEVEL_VCC / EDGE_Icc;

                    //ACP
                    //_MXA_N9020A.Config__EDGE_ACP(dblFreq);
                    _MXA_N9020A.Config__EDGE_ORFS(dblFreq);
                    _MXA_N9020A.SetAttenuattor(intMXA_Atten);

                    intDelay = Math.Max(intDelay_SigGen, intDelay_MXA);
                    util.Wait(intDelay * 3);

                    //dblACPResult = _MXA_N9020A.Get_EDGE_ACP_Result();
                    dblACPResult = _MXA_N9020A.Get_EDGE_ORFS_Result();

                    dblResult[7] = dblACPResult[6];     // -400kHz
                    dblResult[8] = dblACPResult[7];     // +400kHz

                    // EVM
                    if (!cbxBypassEVM.Checked)
                    {
                        _MXA_N9020A.Config__EDGE_EVM(dblFreq);
                        _MXA_N9020A.SetAttenuattor(intMXA_Atten);

                        intDelay = Math.Max(intDelay_SigGen, intDelay_MXA);
                        util.Wait(intDelay * 3);

                        dblResult[9] = _MXA_N9020A.Get_EDGE_EVM_Result();
                    }
                    else
                    {
                        dblResult[9] = 0;
                    }
                    // Update_Grid
                    DataRow drNew = null;
                    if (WhichBand == BandList.B_1)
                        drNew = dtEDGELB.NewRow();
                    else if (WhichBand == BandList.B_2)
                        drNew = dtEDGEHB.NewRow();
                    else
                        throw new Exception("No this band");

                    DataRow drNewTmp = dtEDGETMP.NewRow();
                    drNewTmp[0] = drNew[0] = intTestID++;
                    for (int i = 0; i < dblResult.Count(); i++)
                    {
                        drNewTmp[i + 1] = drNew[i + 1] = Math.Round(dblResult[i], 2);
                    }
                    dtEDGETMP.Rows.Add(drNewTmp);
                    this.Invoke((MethodInvoker)(delegate
                    {
                        if (WhichBand == BandList.B_1)
                            dtEDGELB.Rows.Add(drNew);
                        else if (WhichBand == BandList.B_2)
                            dtEDGEHB.Rows.Add(drNew);
                        else
                            throw new Exception("No this band");

                        dgvSweepResult.Refresh();
                        Application.DoEvents();
                    }));

                }
            }

            #endregion --- EDGE Test ---

            #region --- Worst acp report ---

            foreach (double dblFreq in TestSetting.FREQLIST)
            {
                DataRow drmax = null;

                if (WhichBand == BandList.B_1)
                    drmax = dtEDGELB.NewRow();
                else if (WhichBand == BandList.B_2)
                    drmax = dtEDGEHB.NewRow();
                else
                    throw new Exception("No this band");

                
                drmax[0] = intTestID++;
                drmax[1] = dblFreq;

                var acp_n400 = from x in dtEDGETMP.AsEnumerable()
                               where x.Field<double>("Frequency (MHz)") == dblFreq
                               select x.Field<double>("ACP -400kHz(dB)");

                var acp_p400 = from x in dtEDGETMP.AsEnumerable()
                               where x.Field<double>("Frequency (MHz)") == dblFreq
                               select x.Field<double>("ACP +400kHz(dB)");

                var evm = from x in dtEDGETMP.AsEnumerable()
                          where x.Field<double>("Frequency (MHz)") == dblFreq
                          select x.Field<double>("EVM(%)");

                try
                {
                    drmax[8] = acp_n400.ToList<double>().Max();
                    drmax[9] = acp_p400.ToList<double>().Max();
                    drmax[10] = evm.ToList<double>().Max();
                }
                catch
                {
                    drmax[8] = 0;
                    drmax[9] = 0;
                    drmax[10] = 0;
                }

                this.Invoke((MethodInvoker)(delegate
                {
                    if (WhichBand == BandList.B_1)
                        dtEDGELB.Rows.Add(drmax);
                    else if (WhichBand == BandList.B_2)
                        dtEDGEHB.Rows.Add(drmax);
                    else
                        throw new Exception("No this band");

                    dgvSweepResult.Refresh();
                    Application.DoEvents();
                }));

            }
            #endregion *** Worst acp report ***

            AfterTest();
        }
        void CDMA_TEST_NEW(object Band)
        {
            BandList WhichBand = (BandList)Band;

            #region --- Variable Define ---

            Dictionary<double, double> lastPin = new Dictionary<double, double>();
            int intDelay;
            int intTestID = 1;
            double Pin_Start = -10;
            double[] dblResult = new double[12];
            string waveform_name = "";

            this.Invoke((MethodInvoker)(delegate
            {
                waveform_name = cbxWaveform.Items[1].ToString();
            }));

            DataTable dtCDMATMP = new DataTable();

            if (WhichBand == BandList.B_1)
                dtCDMATMP = dtCDMA.Clone();
            else if (WhichBand == BandList.B_2)
                dtCDMATMP = dtEVDO.Clone();
            else
                throw new Exception("No this band");

            if (!cbxKeepPrevious.Checked)
            {
                this.Invoke((MethodInvoker)(delegate
                {
                    if (WhichBand == BandList.B_1)
                        dtCDMA.Clear();
                    else if (WhichBand == BandList.B_2)
                        dtEVDO.Clear();
                    else
                        throw new Exception("No this band");

                    dgvSweepResult.Refresh();
                }));
            }

            #endregion --- Variable Define ---

            #region --- Initialize ---

            BeforeTest();

            SetVCC_New(TestSetting.LEVEL_VCC,1000);
            SetGPCTRL(TestSetting.LEVEL_GPCTRL); 
            SetVEN(Arb_Channel.Channel_2, TestSetting.LEVEL_PIN_VRAMP, true);

            if (WhichBand == BandList.B_1)  // B_1 is CDMA
            {
                _E4438C.Mode_Initialize(Modulation.CDMA, waveform_name);
                _MXA_N9020A.Mod_Initialize(Modulation.CDMA, false);
            }
            else if (WhichBand == BandList.B_2)  // B_2 is EVDO
            {
                _E4438C.Mode_Initialize(Modulation.EVDO, waveform_name);
                _MXA_N9020A.Mod_Initialize(Modulation.EVDO, false);
            }
            else
            {
                throw new Exception("No This band");
            }
            _E4438C.SetArbTrig(Triger_Type.Continous_Reset, Triger_Source.Bus, 0);
            _E4438C.TrigerBus();
            _E4438C.SetModOutput(Output.ON);
            _E4438C.SetOutput(Output.ON);

            DateTime t_start = DateTime.Now;
            if (cbxMipi.Checked)
            {
                string Content = "Make sure everything is setup for CDMA/EVDO testing \r\n" +
                                  "  1) Connect rf source cable to CDMA/EVDO \r\n" +
                                  "  2) Set mipi control to the right mode in next windows \r\n" +
                                  "  3) Connect MXA to ANT output with 30dB Pad directly";

                this.Invoke((MethodInvoker)(delegate
                {
                    MessageBox.Show(this, Content, "CDMA/EVDO Testing", MessageBoxButtons.OK);
                    Application.DoEvents();
                    frmMipi.ShowDialog();
                    wait_2_start(t_start);
                }));
            }
            else
            {
                string Content = "Make sure everything is setup for CDMA/EVDO testing \r\n" +
                                  "  1) Connect rf source cable to CDMA/EVDO \r\n" +
                                  "  2) Set Control box to CDMA/EVDO mode \r\n" +
                                  "  3) Connect MXA to ANT output with 30dB Pad directly";

                this.Invoke((MethodInvoker)(delegate
                {
                    MessageBox.Show(this, Content, "CDMA/EVDO Testing", MessageBoxButtons.OK);
                    Application.DoEvents();
                    wait_2_start(t_start);
                }));
            }

            #endregion --- Initialize ---

            #region --- CDMA/EVDO Test ---

            for (double dblPout_Target = TestSetting.LEVEL_START; dblPout_Target <= TestSetting.LEVEL_STOP; dblPout_Target += TestSetting.LEVEL_STEP)
            {
                if (StopTest) break;

                double PoutLL = dblPout_Target - 0.05;
                double PoutUL = dblPout_Target + 0.05;

                int intMXA_Atten = 20;
                if (dblPout_Target > 20)
                    intMXA_Atten = 20;
                else if (dblPout_Target > 10)
                    intMXA_Atten = 10;
                else
                    intMXA_Atten = 0;

                foreach (double dblFreq in TestSetting.FREQLIST)
                {
                    if (StopTest) break;

                    double CDMA_Pout = 0;
                    double CDMA_Pin = 0;
                    double CDMA_Icc = 0;
                    double dblPin = Pin_Start;
                    double[] dblACPResult = new double[5];

                    if (lastPin.ContainsKey(dblFreq)) CDMA_Pin = dblPin = Pin_Start = lastPin[dblFreq] + TestSetting.LEVEL_STEP;
                    _E4438C.SetFrequency(dblFreq);
                    _E4438C.SetPower(Pin_Start + TestSetting.LOSS_SRC[dblFreq]);

                    // MXA to measure channel power
                    #region
                    intDelay = Math.Max(intDelay_SigGen, intDelay_MXA) * 10;
                    if (WhichBand == BandList.B_1)
                    {
                        _MXA_N9020A.Config__CDMA_CHP(dblFreq);
                        _MXA_N9020A.SetAttenuattor(20);
                        util.Wait(intDelay);

                        CDMA_Pout = _MXA_N9020A.Get_CDMA_CHP_Result();
                    }
                    else if (WhichBand == BandList.B_2)
                    {
                        _MXA_N9020A.Config__EVDO_CHP(dblFreq);
                        _MXA_N9020A.SetAttenuattor(20);
                       
                        util.Wait(intDelay);

                        CDMA_Pout = _MXA_N9020A.Get_EVDO_CHP_Result();
                    }
                    else
                        throw new Exception("No this band");

                    CDMA_Pout += TestSetting.LOSS_MSR_THROUGH[dblFreq];
                    #endregion

                    //// PowerMeter A to measure channel power
                    //_PM_N1913A.Configure__CW_Power(dblFreq, 10);
                    //intDelay = Math.Max(intDelay_SigGen, intDelay_PowerMeter) * 2;
                    //util.Wait(intDelay);
                    //CDMA_Pout = _PM_N1913A.GetPowerResult();
                    //CDMA_Pout += TestSetting.LOSS_MSR_POUT[dblFreq];

                    // Freqquency
                    dblResult[0] = dblFreq;

                    // Target Power
                    dblResult[1] = dblPout_Target;

                    // Pout
                    int loop = 0;
                    while (CDMA_Pout <= PoutLL || CDMA_Pout >= PoutUL)
                    {
                        if (loop++ > 15) break;
                        if ((dblPin = dblPin + dblPout_Target - CDMA_Pout) > 8)
                        {
                            dblPin = 8;
                            _E4438C.SetPower(dblPin + TestSetting.LOSS_SRC[dblFreq]);
                            _MXA_N9020A.SetAttenuattor(intMXA_Atten);
                            util.Wait(intDelay);

                            if (WhichBand == BandList.B_1)
                                CDMA_Pout = _MXA_N9020A.Get_CDMA_CHP_Result();
                            else if (WhichBand == BandList.B_2)
                                CDMA_Pout = _MXA_N9020A.Get_EVDO_CHP_Result();
                            else
                                throw new Exception("No this band");

                            CDMA_Pout += TestSetting.LOSS_MSR_THROUGH[dblFreq];
                            CDMA_Pin = dblPin;

                            break;
                        }

                        _E4438C.SetPower(dblPin + TestSetting.LOSS_SRC[dblFreq]);
                        _MXA_N9020A.SetAttenuattor(intMXA_Atten);
                        util.Wait(intDelay);

                        if (WhichBand == BandList.B_1)
                            CDMA_Pout = _MXA_N9020A.Get_CDMA_CHP_Result();
                        else if (WhichBand == BandList.B_2)
                            CDMA_Pout = _MXA_N9020A.Get_EVDO_CHP_Result();
                        else
                            throw new Exception("No this band");

                        CDMA_Pout += TestSetting.LOSS_MSR_THROUGH[dblFreq];
                        CDMA_Pin = dblPin;

                    }
                    dblResult[2] = CDMA_Pout;

                    // Pin
                    dblResult[3] = CDMA_Pin;

                    if (lastPin.ContainsKey(dblFreq))
                        lastPin[dblFreq] = CDMA_Pin;
                    else
                        lastPin.Add(dblFreq, CDMA_Pin);
                    // Gain
                    dblResult[4] = CDMA_Pout - CDMA_Pin;

                    // Icc
                    CDMA_Icc = _PS_66332A.RMS_Current();
                    dblResult[5] = CDMA_Icc * 1000;
                    // PAE
                    dblResult[6] = 100 * (Math.Pow(10, ((CDMA_Pout - 30) / 10))) / TestSetting.LEVEL_VCC / CDMA_Icc;

                    // ACP
                    intDelay = Math.Max(intDelay_SigGen, intDelay_MXA);
                    if (WhichBand == BandList.B_1)
                    {
                        _MXA_N9020A.Config__CDMA_ACP(dblFreq);
                        _MXA_N9020A.SetAttenuattor(intMXA_Atten);

                        util.Wait(intDelay * 3);

                        dblACPResult = _MXA_N9020A.Get_CDMA_ACP_Result();
                    }
                    else if (WhichBand == BandList.B_2)
                    {
                        _MXA_N9020A.Config__EVDO_ACP(dblFreq);
                        _MXA_N9020A.SetAttenuattor(intMXA_Atten);

                        util.Wait(intDelay * 3);

                        dblACPResult = _MXA_N9020A.Get_EVDO_ACP_Result();
                    }
                    else
                        throw new Exception("No this band");


                    dblResult[7] = dblACPResult[1];     // -5MHz
                    dblResult[8] = dblACPResult[2];     // +5MHz
                    dblResult[9] = dblACPResult[3];     // -10MHz
                    dblResult[10] = dblACPResult[4];     // +10MHz

                    // EVM
                    if (!cbxBypassEVM.Checked)
                    {
                        ////waveform_name = cbxWaveform.Items[1].ToString();

                        ////if (WhichBand == BandList.B_1)
                        ////    _E4438C.Mode_Initialize(Modulation.CDMA, waveform_name);
                        ////else if (WhichBand == BandList.B_2)
                        ////    _E4438C.Mode_Initialize(Modulation.EVDO, waveform_name);
                        ////else
                        ////    throw new Exception("No this band");

                        ////_E4438C.SetFrequency(dblFreq);
                        ////_E4438C.SetPower(CDMA_Icc + TestSetting.LOSS_SRC[dblFreq]);
                        ////_E4438C.SetModOutput(Output.ON);
                        ////_E4438C.SetOutput(Output.ON);
                        ////util.Wait(1000);

                        intDelay = Math.Max(intDelay_SigGen, intDelay_MXA);
                        if (WhichBand == BandList.B_1)
                        {
                            _MXA_N9020A.Config__CDMA_EVM(dblFreq);
                            _MXA_N9020A.SetAttenuattor(intMXA_Atten);
                            util.Wait(intDelay * 3);

                            dblResult[11] = _MXA_N9020A.Get_CDMA_EVM_Result();
                        }
                        else if (WhichBand == BandList.B_2)
                        {
                            _MXA_N9020A.Config__EVDO_EVM(dblFreq);
                            _MXA_N9020A.SetAttenuattor(intMXA_Atten);
                            util.Wait(intDelay * 3);

                            dblResult[11] = _MXA_N9020A.Get_EVDO_EVM_Result();
                        }
                        else
                            throw new Exception("No this band");

                    }
                    else
                    {
                        dblResult[11] = 0;
                    }
                    // Update_Grid
                    DataRow drNew = null;
                    if (WhichBand == BandList.B_1)
                        drNew = dtCDMA.NewRow();
                    else if (WhichBand == BandList.B_2)
                        drNew = dtEVDO.NewRow();
                    else
                        throw new Exception("No this band");

                    DataRow drNewtmp = dtCDMATMP.NewRow();
                    drNewtmp[0] = drNew[0] = intTestID++;
                    for (int i = 0; i < dblResult.Count(); i++)
                    {
                        drNew[i + 1] = drNewtmp[i + 1] = Math.Round(dblResult[i], 2);
                    }
                    dtCDMATMP.Rows.Add(drNewtmp);

                    this.Invoke((MethodInvoker)(delegate
                    {
                        if (WhichBand == BandList.B_1)
                            dtCDMA.Rows.Add(drNew);
                        else if (WhichBand == BandList.B_2)
                            dtEVDO.Rows.Add(drNew);
                        else
                            throw new Exception("No this band");

                        dgvSweepResult.Refresh();
                        Application.DoEvents();
                    }));

                }
            }

            #endregion --- CDMA/EVDO Test ---

            #region --- Worst acp report ---

            foreach (double dblFreq in TestSetting.FREQLIST)
            {
                DataRow drmax = null;
                if (WhichBand == BandList.B_1)
                    drmax = dtCDMA.NewRow();
                else if (WhichBand == BandList.B_2)
                    drmax = dtEVDO.NewRow();
                else
                    throw new Exception("No this band");

                var acp_n5 = from x in dtCDMATMP.AsEnumerable()
                             where x.Field<double>("Frequency (MHz)") == dblFreq
                             select x.Field<double>("ACP -5MHz(dB)");

                var acp_p5 = from x in dtCDMATMP.AsEnumerable()
                             where x.Field<double>("Frequency (MHz)") == dblFreq
                             select x.Field<double>("ACP +5MHz(dB)");

                var acp_n10 = from x in dtCDMATMP.AsEnumerable()
                              where x.Field<double>("Frequency (MHz)") == dblFreq
                              select x.Field<double>("ACP -10MHz(dB)");

                var acp_p10 = from x in dtCDMATMP.AsEnumerable()
                              where x.Field<double>("Frequency (MHz)") == dblFreq
                              select x.Field<double>("ACP +10MHz(dB)");

                var evm = from x in dtCDMATMP.AsEnumerable()
                          where x.Field<double>("Frequency (MHz)") == dblFreq
                          select x.Field<double>("EVM(%)");

                try
                {
                    drmax[8] = acp_n5.ToList<double>().Max();
                    drmax[9] = acp_p5.ToList<double>().Max();
                    drmax[10] = acp_n10.ToList<double>().Max();
                    drmax[11] = acp_p10.ToList<double>().Max();
                    drmax[12] = evm.ToList<double>().Max();
                }
                catch
                {
                    drmax[8] = 0;
                    drmax[9] = 0;
                    drmax[10] = 0;
                    drmax[11] = 0;
                    drmax[12] = 0;
                }

                this.Invoke((MethodInvoker)(delegate
                {
                    if (WhichBand == BandList.B_1)
                        dtCDMA.Rows.Add(drmax);
                    else if (WhichBand == BandList.B_2)
                        dtEVDO.Rows.Add(drmax);
                    else
                        throw new Exception("No this band");

                    dgvSweepResult.Refresh();
                    Application.DoEvents();
                }));

            }
            #endregion *** Worst acp report ***

            AfterTest();

        }
        void GSMK_TEST_NEW(object Band)
        {
            BandList WhichBand = (BandList)Band;

            #region --- Variable Define ---

            dicVramp.Clear();
            int intTestID = 1;
            double[] dblResult = null;
            double target_power = 0;
            DataTable dtCWTMP = new DataTable();

            if (WhichBand == BandList.B_1)
            {
                dblResult = new double[18];
                target_power = TestSetting.TARGET_POUT_CWLB;
                dtCWTMP = dtCWLB.Clone();
            }
            else if (WhichBand == BandList.B_2)
            {
                dblResult = new double[11];
                target_power = TestSetting.TARGET_POUT_CWHB;
                dtCWTMP = dtCWHB.Clone();
            }
            else
                throw new Exception("No this band");
        
            if (!cbxKeepPrevious.Checked)
            {
                this.Invoke((MethodInvoker)(delegate
                {
                    if (WhichBand == BandList.B_1)
                        dtCWLB.Clear();
                    else if (WhichBand == BandList.B_2)
                        dtCWHB.Clear();
                    else
                        throw new Exception("No this band");

                    dgvSweepResult.Refresh();
                }));
            }

            #endregion --- Variable Define ---

            #region --- Initialize ---
            BeforeTest();
            SetVCC_New(TestSetting.LEVEL_VCC, 3000);
            SetTXEnable(TestSetting.LEVEL_TXEN);
            SetGPCTRL(TestSetting.LEVEL_GPCTRL);
            SetVramp(1.8);

            _E4438C.Initialize();
            _PM_N1913A.Initialize(rbnDisplayON.Checked);
            _MXA_N9020A.Initialize(rbnDisplayON.Checked);
            _E4438C.SetOutput(Output.ON);

            DateTime t_Start = DateTime.Now;

            if (cbxMipi.Checked)
            {
                string Content = "Make sure everything is setup for GMSK testing \r\n" +
                                "  1) Connect rf source cable to GMSK \r\n" +
                                "  2) Set mipi control to the right mode in next windows \r\n" +
                                "  3) Connect / Change highpass filter";

                this.Invoke((MethodInvoker)(delegate
                {
                    MessageBox.Show(this, Content, "GMSK Testing", MessageBoxButtons.OK);
                    Application.DoEvents();
                    frmMipi.ShowDialog();
                    wait_2_start(t_Start);
                }));
            }
            else
            {
                string Content = "Make sure everything is setup for GMSK testing \r\n" +
                                "  1) Connect rf source cable to GMSK \r\n" +
                                "  2) Set Control box to GMSK mode \r\n" +
                                "  3) Connect / Change highpass filter";

                this.Invoke((MethodInvoker)(delegate
                {
                    MessageBox.Show(this, Content, "GMSK Testing", MessageBoxButtons.OK);
                    Application.DoEvents();
                    wait_2_start(t_Start);
                }));

            }


            #endregion --- Initialize ---

            #region --- Vramp Sweep ---
            for (double dblVramp = TestSetting.LEVEL_START; dblVramp < TestSetting.LEVEL_STOP + 0.01; dblVramp += TestSetting.LEVEL_STEP)
            {
                if (StopTest) break;

                SetVramp(dblVramp);

                foreach (double dblFreq in TestSetting.FREQLIST)
                {
                    if (StopTest) break;
                    _E4438C.SetFrequency(dblFreq);
                    _E4438C.SetPower(TestSetting.LEVEL_PIN_VRAMP + TestSetting.LOSS_SRC[dblFreq]);

                    // Frequency
                    dblResult[0] = dblFreq;

                    // Vramp
                    dblResult[1] = dblVramp;

                    // Pout
                    _PM_N1913A.Configure__Average_Pulse_Power_with_Duty_Cycle(dblFreq, 25, 256);
                    util.Wait(intDelay_PowerMeter * intDelay_N1913A_Count);
                    dblResult[2] = _PM_N1913A.GetPowerResult() + TestSetting.LOSS_MSR_POUT[dblFreq];

                    // ICC
                    dblResult[3] = _PS_66332A.High_Current();
                    dblResult[3] = dblResult[3] * 1000;

                    // PAE
                    dblResult[4] = (Math.Pow(10, ((dblResult[2] - 30) / 10))) / TestSetting.LEVEL_VCC / dblResult[3] * 1000;
                    dblResult[4] = dblResult[4] * 100;

                    // harmonic 
                    int har_stop_point;
                    int ext_har_stop_point;
                    if (WhichBand == BandList.B_1)
                    {
                        har_stop_point = 6;
                        ext_har_stop_point = 14;
                    }
                    else if (WhichBand == BandList.B_2)
                    {
                        har_stop_point = 3;
                        ext_har_stop_point = 7;
                    }
                    else
                        throw new Exception("No this band");

                    for (int i = 2; i <= har_stop_point; i++)
                    {
                        _MXA_N9020A.SetFrequency(i * dblFreq);
                        _MXA_N9020A.SetAttenuattor(0);

                        util.Wait(intDelay_MXA);

                        if (WhichBand == BandList.B_1)
                            dblResult[3 + i] = _MXA_N9020A.Get_CW_PowerResult() + TestSetting.LOSS_MSR_FILTER_LB[i * dblFreq];
                        else if (WhichBand == BandList.B_2)
                            dblResult[3 + i] = _MXA_N9020A.Get_CW_PowerResult() + TestSetting.LOSS_MSR_FILTER_HB[i * dblFreq];
                        else
                            throw new Exception("No this band");

                    }
                    // Extra harmonic 
                    if (ext_har)
                    {
                        for (int i = har_stop_point + 1; i <= ext_har_stop_point; i++)
                        {
                            _MXA_N9020A.SetFrequency(i * dblFreq);
                            util.Wait(intDelay_MXA);

                            if (WhichBand == BandList.B_1)
                                dblResult[3 + i] = _MXA_N9020A.Get_CW_PowerResult() + TestSetting.LOSS_MSR_FILTER_LB[6000];
                            else if (WhichBand == BandList.B_2)
                                dblResult[3 + i] = _MXA_N9020A.Get_CW_PowerResult() + TestSetting.LOSS_MSR_FILTER_HB[6000];
                            else
                                throw new Exception("No this band");

                        }
                    }
                    // Update_Grid
                    DataRow drNew = null;
                    if (WhichBand == BandList.B_1)
                        drNew = dtCWLB.NewRow();
                    else if (WhichBand == BandList.B_2)
                        drNew = dtCWHB.NewRow();
                    else
                        throw new Exception("No this band");

                    DataRow drNewTmp = dtCWTMP.NewRow();
                    drNewTmp[0] = drNew[0] = intTestID++;
                    for (int i = 0; i < dblResult.Count(); i++)
                    {
                        drNewTmp[i + 1] = drNew[i + 1] = Math.Round(dblResult[i], 2);
                    }
                    dtCWTMP.Rows.Add(drNewTmp);
                    this.Invoke((MethodInvoker)(delegate
                    {
                        if (WhichBand == BandList.B_1)
                            dtCWLB.Rows.Add(drNew);
                        else if (WhichBand == BandList.B_2)
                            dtCWHB.Rows.Add(drNew);
                        else
                            throw new Exception("No this band");

                        dgvSweepResult.Refresh();
                        Application.DoEvents();
                    }));
                } // Frequency loop
            }   // Vramp loop
            #endregion Vramp Sweep

            #region --- Power Servo ---
            foreach (double dblFreq in TestSetting.FREQLIST)
            {
                if (StopTest) break;

                int Count = 0;
                _E4438C.SetFrequency(dblFreq);
                _E4438C.SetPower(TestSetting.LEVEL_PIN_VRAMP + TestSetting.LOSS_SRC[dblFreq]);

                #region Search power
                double LoopResult_Low;
                double LoopResult_High;
                double LoopResult;
                double Slope_mV;
                double Vramp_low = 1.1;
                double Vramp_high = 1.5;


                this.SetVramp(Vramp_low);
                _PM_N1913A.Configure__Average_Pulse_Power_with_Duty_Cycle(dblFreq, 25, 256);
                util.Wait(intDelay_PowerMeter * intDelay_N1913A_Count);
                LoopResult_Low = _PM_N1913A.GetPowerResult() + TestSetting.LOSS_MSR_POUT[dblFreq];

                this.SetVramp(Vramp_high);
                _PM_N1913A.Configure__Average_Pulse_Power_with_Duty_Cycle(dblFreq, 25, 256);
                util.Wait(intDelay_PowerMeter * intDelay_N1913A_Count);
                LoopResult_High = _PM_N1913A.GetPowerResult() + TestSetting.LOSS_MSR_POUT[dblFreq];

                Slope_mV = (LoopResult_High - LoopResult_Low) / ((Vramp_high - Vramp_low) * 1000);

                LoopResult = LoopResult_Low;
                dblRatedVramp = Vramp_low + (target_power - LoopResult) / Slope_mV / 1000;

                //while (Math.Abs(LoopResult - target_power) > 0.05 && dblRatedVramp < 1.8 && dblRatedVramp > 0.8)
                while (Math.Abs(LoopResult - target_power) > 0.05)
                {
                    //if (Count++ > 20) break;
                    if (Count++ > 20 || dblRatedVramp < 0.8 || dblRatedVramp > 1.8)
                    {
                        dblRatedVramp = 1.85;
                        this.SetVramp(dblRatedVramp);
                        util.Wait(intDelay_PowerMeter * intDelay_N1913A_Count);
                        LoopResult = _PM_N1913A.GetPowerResult() + TestSetting.LOSS_MSR_POUT[dblFreq];

                        break;
                    }

                    this.SetVramp(dblRatedVramp);
                    util.Wait(intDelay_PowerMeter * intDelay_N1913A_Count);
                    LoopResult = _PM_N1913A.GetPowerResult() + TestSetting.LOSS_MSR_POUT[dblFreq];
                    //optimize the servo speed by accurice the Slope_mV
                    if (dblRatedVramp < (Vramp_low + Vramp_high))
                        Slope_mV = (LoopResult - LoopResult_Low) / ((dblRatedVramp - Vramp_low) * 1000);
                    else
                        Slope_mV = (LoopResult_High - LoopResult) / ((Vramp_high - dblRatedVramp) * 1000);

                    if (Math.Abs(LoopResult - target_power) > 0.05)
                        dblRatedVramp = dblRatedVramp + (target_power - LoopResult) / Slope_mV / 1000;

                }
                #endregion Search power
                lblError.Text = Count.ToString();
                lblError.Refresh();

                // Frequency
                dblResult[0] = dblFreq;
                // Rated Vramp
                dblResult[1] = dblRatedVramp;
                if (!dicVramp.ContainsKey(dblFreq)) dicVramp.Add(dblFreq, Math.Round(dblRatedVramp, 2));
                //Rated Pout
                dblResult[2] = LoopResult;

                // Rated ICC
                dblResult[3] = _PS_66332A.High_Current();
                dblResult[3] = dblResult[3] * 1000;

                // Rated PAE
                dblResult[4] = (Math.Pow(10, ((dblResult[2] - 30) / 10))) / TestSetting.LEVEL_VCC / dblResult[3] * 1000;
                dblResult[4] = dblResult[4] * 100;

                // Rated harmonic 
                int har_stop_point;
                int ext_har_stop_point;
                if (WhichBand == BandList.B_1)
                {
                    har_stop_point = 6;
                    ext_har_stop_point = 14;
                }
                else if (WhichBand == BandList.B_2)
                {
                    har_stop_point = 3;
                    ext_har_stop_point = 7;
                }
                else
                    throw new Exception("No this band");

                for (int i = 2; i <= har_stop_point; i++)
                {
                    _MXA_N9020A.SetFrequency(i * dblFreq);
                    _MXA_N9020A.SetAttenuattor(0);

                    util.Wait(intDelay_MXA);

                    if (WhichBand == BandList.B_1)
                        dblResult[3 + i] = _MXA_N9020A.Get_CW_PowerResult() + TestSetting.LOSS_MSR_FILTER_LB[i * dblFreq];
                    else if (WhichBand == BandList.B_2)
                        dblResult[3 + i] = _MXA_N9020A.Get_CW_PowerResult() + TestSetting.LOSS_MSR_FILTER_HB[i * dblFreq];
                    else
                        throw new Exception("No this band");

                }
                // Extra harmonic 
                if (ext_har)
                {
                    for (int i = har_stop_point + 1; i <= ext_har_stop_point; i++)
                    {
                        _MXA_N9020A.SetFrequency(i * dblFreq);
                        util.Wait(intDelay_MXA);

                        if (WhichBand == BandList.B_1)
                            dblResult[3 + i] = _MXA_N9020A.Get_CW_PowerResult() + TestSetting.LOSS_MSR_FILTER_LB[6000];
                        else if (WhichBand == BandList.B_2)
                            dblResult[3 + i] = _MXA_N9020A.Get_CW_PowerResult() + TestSetting.LOSS_MSR_FILTER_HB[6000];
                        else
                            throw new Exception("No this band");

                    }
                }
                // Update_Grid
                DataRow drNew = null;
                if (WhichBand == BandList.B_1)
                    drNew = dtCWLB.NewRow();
                else if (WhichBand == BandList.B_2)
                    drNew = dtCWHB.NewRow();
                else
                    throw new Exception("No this band");

                DataRow drNewTmp = dtCWTMP.NewRow();
                drNewTmp[0] = drNew[0] = intTestID++;
                for (int i = 0; i < dblResult.Count(); i++)
                {
                    drNewTmp[i + 1] = drNew[i + 1] = Math.Round(dblResult[i], 2);
                }
                dtCWTMP.Rows.Add(drNewTmp);
                this.Invoke((MethodInvoker)(delegate
                {
                    if (WhichBand == BandList.B_1)
                        dtCWLB.Rows.Add(drNew);
                    else if (WhichBand == BandList.B_2)
                        dtCWHB.Rows.Add(drNew);
                    else
                        throw new Exception("No this band");

                    dgvSweepResult.Refresh();
                    Application.DoEvents();
                }));

            } // Frequency loop for power servo
            #endregion Power Servo

            #region --- Worst harmonic report ---

            foreach (double dblFreq in TestSetting.FREQLIST)
            {
                if (StopTest) break;

                DataRow drmax = null;
                if (WhichBand == BandList.B_1)
                    drmax = dtCWLB.NewRow();
                else if (WhichBand == BandList.B_2)
                    drmax = dtCWHB.NewRow();
                else
                    throw new Exception("No this band");

                drmax[0] = intTestID++;
                drmax[1] = dblFreq;

                var _2fo = from x in dtCWTMP.AsEnumerable()
                           where x.Field<double>("Frequency(MHz)") == dblFreq && double.Parse(x.Field<string>("Vramp(V)")) <= dicVramp[dblFreq]
                           select x.Field<double>("2fo(dBm)");

                var _3fo = from x in dtCWTMP.AsEnumerable()
                           where x.Field<double>("Frequency(MHz)") == dblFreq && double.Parse(x.Field<string>("Vramp(V)")) <= dicVramp[dblFreq]
                           select x.Field<double>("3fo(dBm)");

                var _4fo = from x in dtCWTMP.AsEnumerable()
                           where x.Field<double>("Frequency(MHz)") == dblFreq && double.Parse(x.Field<string>("Vramp(V)")) <= dicVramp[dblFreq]
                           select x.Field<double>("4fo(dBm)");

                var _5fo = from x in dtCWTMP.AsEnumerable()
                           where x.Field<double>("Frequency(MHz)") == dblFreq && double.Parse(x.Field<string>("Vramp(V)")) <= dicVramp[dblFreq]
                           select x.Field<double>("5fo(dBm)");

                var _6fo = from x in dtCWTMP.AsEnumerable()
                           where x.Field<double>("Frequency(MHz)") == dblFreq && double.Parse(x.Field<string>("Vramp(V)")) <= dicVramp[dblFreq]
                           select x.Field<double>("6fo(dBm)");

                try
                {
                    drmax[6] = _2fo.ToList<double>().Max();
                    drmax[7] = _3fo.ToList<double>().Max();
                    drmax[8] = _4fo.ToList<double>().Max();
                    drmax[9] = _5fo.ToList<double>().Max();
                    drmax[10] = _6fo.ToList<double>().Max();
                }
                catch
                {
                    drmax[6] = 0;
                    drmax[7] = 0;
                    drmax[8] = 0;
                    drmax[9] = 0;
                    drmax[10] = 0;
                }

                this.Invoke((MethodInvoker)(delegate
                {

                    if (WhichBand == BandList.B_1)
                        dtCWLB.Rows.Add(drmax);
                    else if (WhichBand == BandList.B_2)
                        dtCWHB.Rows.Add(drmax);
                    else
                        throw new Exception("No this band");

                    dgvSweepResult.Refresh();
                    Application.DoEvents();
                }));

            }
            #endregion *** Worst harmonic report ***

            AfterTest();


        }

        

        private void SetVCC_New(double dblValue_in_Volts, double dblValue_in_MiniAmps)
        {
            try
            {
                if (Program.Location == LocationList.SH_2 || 
                    Program.Location == LocationList.SH_3 ||
                         Program.Location == LocationList.SH_4)
                {
                    _PS_66319B.SetCurrentRange(PS_66319B_Channel.Channel_1, dblValue_in_MiniAmps * 1000);
                    _PS_66319B.SetVoltage(PS_66319B_Channel.Channel_1, dblValue_in_Volts);
                    _PS_66319B.SetOutput(PS_66319B_Channel.Channel_1, Output.ON);
                }
                else
                {
                    _PS_66332A.SetCurrentRange(dblValue_in_MiniAmps * 1000);
                    _PS_66332A.SetVoltage(dblValue_in_Volts);
                    _PS_66332A.SetOutput(Output.ON);
                }
            }
            catch (Exception ex)
            {
                lblError.Text = ex.Message.ToString();
            }
        }
        private void SetVbat_New(double dblValue_in_Volts, double dblValue_in_MiniAmps)
        {
            try
            {
                if (Program.Location == LocationList.SH_2 || 
                    Program.Location == LocationList.SH_3 ||
                         Program.Location == LocationList.SH_4)
                {
                    _PS_66319B.SetCurrentRange(PS_66319B_Channel.Channel_2, dblValue_in_MiniAmps * 1000);
                    _PS_66319B.SetVoltage(PS_66319B_Channel.Channel_2, dblValue_in_Volts);
                    _PS_66319B.SetOutput(PS_66319B_Channel.Channel_2, Output.ON);
                }
                else
                {
                    throw new Exception("Cannot set Vbat on this bench!");
                }
            }
            catch (Exception ex)
            {
                lblError.Text = ex.Message.ToString();
            }
        }
        private void SetVEN_TDS(double dblValue_in_Volts)
        {
            //double dblDCOffset = dblTxEnableValue_in_Volts / 2;
            double dblDCOffset = 0;
            try
            {
                #region BJ_1 & SH_1
                if (Program.Location == LocationList.BJ_1 || Program.Location == LocationList.SH_1)
                {
                    _Arb_33522A_USB.SetArbOut(Arb_Waveform_Type.Pulse, Arb_Channel.Channel_1, 200, dblValue_in_Volts, dblDCOffset, 25);
                    util.Wait(10);
                    _Arb_33522A_USB.SetHighVoltage(dblValue_in_Volts, Arb_Channel.Channel_1);
                    _Arb_33522A_USB.SetBurstTrig(Arb_Channel.Channel_1, 0, 0);
                    util.Wait(20);
                }
                #endregion BJ_1


                #region SH_2 & SH3 & SH4
                else if (Program.Location == LocationList.SH_2 ||
                         Program.Location == LocationList.SH_3 ||
                         Program.Location == LocationList.SH_4)
                {
                    //_Arb_33522A.Initialize(200);
                    //util.Wait(20);
                    _Arb_33522A.SetArbOut(Arb_Waveform_Type.Pulse, Arb_Channel.Channel_1, 200, dblValue_in_Volts, dblDCOffset, 25);
                    util.Wait(10);
                    _Arb_33522A.SetHighVoltage(dblValue_in_Volts, Arb_Channel.Channel_1);
                    _Arb_33522A.SetBurstTrig(Arb_Channel.Channel_1, 0, 0);
                    util.Wait(20);
                }
                #endregion SH_2 & SH3
                else
                {
                    throw new Exception("Bad Location");
                }
            }
            catch (Exception ex)
            {
                lblError.Text = ex.Message.ToString();
            }
        }
        private void SetVEN_EDGE(double dblValue_in_Volts)
        {
            //double dblDCOffset = dblTxEnableValue_in_Volts / 2;
            double dblDCOffset = 0;
            double dblfreq = 216.638;
            try
            {
                #region BJ_1 & SH_1
                if (Program.Location == LocationList.BJ_1 || Program.Location == LocationList.SH_1)
                {
                    _Arb_33522A_USB.SetArbOut(Arb_Waveform_Type.Pulse, Arb_Channel.Channel_1, dblfreq, dblValue_in_Volts, dblDCOffset, 25);
                    util.Wait(10);
                    _Arb_33522A_USB.SetHighVoltage(dblValue_in_Volts, Arb_Channel.Channel_1);
                    _Arb_33522A_USB.SetBurstTrig(Arb_Channel.Channel_1, 0, 12);
                    util.Wait(20);
                }
                #endregion BJ_1


                #region SH_2 & SH3 & SH4
                else if (Program.Location == LocationList.SH_2 ||
                         Program.Location == LocationList.SH_3 ||
                         Program.Location == LocationList.SH_4)
                {
                    //_Arb_33522A.Initialize(200);
                    //util.Wait(20);
                    _Arb_33522A.SetArbOut(Arb_Waveform_Type.Pulse, Arb_Channel.Channel_1, dblfreq, dblValue_in_Volts, dblDCOffset, 25);
                    util.Wait(10);
                    _Arb_33522A.SetHighVoltage(dblValue_in_Volts, Arb_Channel.Channel_1);
                    _Arb_33522A.SetBurstTrig(Arb_Channel.Channel_1, 0, 12);
                    util.Wait(20);
                }
                #endregion SH_2 & SH3
                else
                {
                    throw new Exception("Bad Location");
                }
            }
            catch (Exception ex)
            {
                lblError.Text = ex.Message.ToString();
            }
        }
        private void Arb_Init()
        {
            try
            {
                #region BJ_1 & SH_1
                if (Program.Location == LocationList.BJ_1 ||
                    Program.Location == LocationList.SH_1)
                {
                    _Arb_33522A_USB.Initialize(200);
                }
                #endregion BJ_1

                #region SH2 & SH3 & SH4
                else if (Program.Location == LocationList.SH_2 ||
                         Program.Location == LocationList.SH_3 ||
                         Program.Location == LocationList.SH_4)
                {
                    _Arb_33522A.Initialize(200);
                }
                #endregion SH_2 & SH3
                else
                {
                    throw new Exception("Bad Location");
                }
            }
            catch (Exception ex)
            {
                lblError.Text = ex.Message.ToString();
            }

        }

        




        private void UpdateResult(ref DataTable dtTable, DataRow drRow)
        {
            //DataRow drNew = dtTable.NewRow();
            //dtTable.Rows.Add(drNew);
            dtTable.ImportRow(drRow);
            dgvSweepResult.Refresh();
            Application.DoEvents();
        }


    }
}
