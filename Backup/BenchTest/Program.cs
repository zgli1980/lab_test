using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Vanchip.Common;

namespace Bench_Test   
{
    static class Program
    {
        #region *** Variable Define ***
        public static string tp;
        public static string lastCalDate;
        public static string csvHeader;
        public static bool evb;
        public static bool debug_mipi;
        //public static Product ProductTest;
        public static ProductTest[] ProductTest;


        public static string strRootPath = System.Environment.CurrentDirectory;
        public static string strFilePath = strRootPath + @"./cfg/";
        public static string strLocationr_FileName = strRootPath + @"./cfg/location.ini";
        public static string strLocationr_FileName_sys = System.Environment.SystemDirectory.ToString() + "\\location.ini";
        public static string strFilePath_Product = strFilePath + "product.ini";
        public static string strFilePath_Testcfg;

        public static string strSweepParameter_FileName = strRootPath + @"./cfg/sweep.ini";
        public static string strSweepCableLoss_FileName = strRootPath + @"./cfg/sweep_lc.ini";
        public static string strMipi_FilePath = strRootPath + @"./cfg/mipi/";

        public const string SweepTest = "Sweep";
        public const string Mipi = "Mipi ControlPanel";

        public static LocationList Location;

        #endregion *** Variable Define ***

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Start frmStart = new Start();
            frmStart.ShowDialog();
            if (frmStart.DialogResult != DialogResult.OK) return;

            #region *** Localtion ***
            FileInfo fileLocation_sys = new FileInfo(Program.strLocationr_FileName_sys); 
            
            //string[] arrDLL = Directory.GetFiles(@"C:\Windows\System32\", "*.ini", SearchOption.TopDirectoryOnly);


            StreamReader srLocation;
            string temp;

            if (fileLocation_sys.Exists)
            {
                srLocation = new StreamReader(strLocationr_FileName_sys);
                temp = srLocation.ReadLine();
            }
            else
            {
                FileInfo fileLocation = new FileInfo(Program.strLocationr_FileName);
                if (fileLocation.Exists)
                {
                    srLocation = new StreamReader(strLocationr_FileName);
                    temp = srLocation.ReadLine();
                }
                else
                {
                    MessageBox.Show("Location Setting  file is not exist in " + Program.strLocationr_FileName
                                        + " \r\nPlease contact the developer",
                                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }


            if (temp != null)
            {
                if (temp == "1") Location = LocationList.SH_1;
                if (temp == "2") Location = LocationList.SH_2;
                if (temp == "3") Location = LocationList.SH_3;
                if (temp == "4") Location = LocationList.SH_4;
                if (temp == "4") Location = LocationList.SH_4;

                if (temp == "11") Location = LocationList.BJ_1;
                if (temp == "12") Location = LocationList.BJ_2;
                if (temp == "99") Location = LocationList.Simulation;
            }
            #endregion Localtion

            // check which program to start
            if (frmStart.DialogResult == DialogResult.OK && tp == Mipi)
            {
                debug_mipi = true;
                Application.Run(new Mipi());
            }
            else if (frmStart.DialogResult == DialogResult.OK && tp == SweepTest)
            {
                debug_mipi = false;

                #region *** Variable Define
                string line;
                double tmpFreq;
                string[] content;
                /////////////////////////////////////////////////////////////
                int ParameterIndentify = 17 + 5 + 4;

                #endregion *** Variable Define ***

                #region *** Read Parameter Setting & Build Frequence List ***
                FileInfo fileParameter = new FileInfo(Program.strSweepParameter_FileName);

                //for LB filter verify
                TestSetting.LOSS_SRC_ROLL.Add(997.6, 0.0);
                TestSetting.LOSS_MSR_FILTER_LB.Add(997.6, 0.0);
                //for HB filter verify
                TestSetting.LOSS_SRC_ROLL.Add(1806.7, 0.0);
                TestSetting.LOSS_MSR_FILTER_HB.Add(1806.7, 0.0);
                //for harmonic which up 6GHz
                TestSetting.LOSS_SRC_ROLL.Add(6000, 0.0);
                TestSetting.LOSS_MSR_FILTER_LB.Add(6000, 0.0);
                TestSetting.LOSS_MSR_FILTER_HB.Add(6000, 0.0);

                if (!fileParameter.Exists)
                {
                    MessageBox.Show("Parameter Setting  file is not exist in " + Program.strSweepParameter_FileName
                                        + " \r\nPlease contact the developer",
                                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                StreamReader srParameter = new StreamReader(strSweepParameter_FileName);
                line = srParameter.ReadLine();

                while (line != null)
                {
                    if (line.Contains("--- Setting ---"))
                    {
                        line = srParameter.ReadLine();
                        ParameterIndentify--;
                    }

                    #region  GMSK_LB Setting
                    if (line.Contains("--- GMSK_LB ---"))
                    {
                        while (!(line = srParameter.ReadLine()).Contains("---"))
                        {
                            content = line.Split(',');
                            TestSetting.SETTING_GMSK_LB.Add(content[0], double.Parse(content[1]));
                        }
                        ParameterIndentify--;
                    }
                    #endregion  GMSK Setting

                    #region  GMSK_HB Setting
                    if (line.Contains("--- GMSK_HB ---"))
                    {
                        while (!(line = srParameter.ReadLine()).Contains("---"))
                        {
                            content = line.Split(',');
                            TestSetting.SETTING_GMSK_HB.Add(content[0], double.Parse(content[1]));
                        }
                        ParameterIndentify--;
                    }
                    #endregion  GMSK Setting

                    #region  EDGE_LB Setting
                    if (line.Contains("--- EDGE_LB ---"))
                    {
                        while (!(line = srParameter.ReadLine()).Contains("---"))
                        {
                            content = line.Split(',');
                            TestSetting.SETTING_EDGE_LB.Add(content[0], double.Parse(content[1]));
                        }
                        ParameterIndentify--;
                    }
                    #endregion  EDGE_LB Setting

                    #region  EDGE_HB Setting
                    if (line.Contains("--- EDGE_HB ---"))
                    {
                        while (!(line = srParameter.ReadLine()).Contains("---"))
                        {
                            content = line.Split(',');
                            TestSetting.SETTING_EDGE_HB.Add(content[0], double.Parse(content[1]));
                        }
                        ParameterIndentify--;
                    }
                    #endregion  EDGE_HB Setting

                    #region  TDSCDMA Setting
                    if (line.Contains("--- TDSCDMA ---"))
                    {
                        while (!(line = srParameter.ReadLine()).Contains("---"))
                        {
                            content = line.Split(',');
                            TestSetting.SETTING_TDSCDMA.Add(content[0], double.Parse(content[1]));
                        }
                        ParameterIndentify--;
                    }
                    #endregion  TDSCDMA Setting

                    #region  WCDMA Setting
                    if (line.Contains("--- WCDMA ---"))
                    {
                        while (!(line = srParameter.ReadLine()).Contains("---"))
                        {
                            content = line.Split(',');
                            TestSetting.SETTING_WCDMA.Add(content[0], double.Parse(content[1]));
                        }
                        ParameterIndentify--;
                    }
                    #endregion  WCDMA Setting

                    #region  LTETDD_B38 Setting
                    if (line.Contains("--- LTETDD_B38 ---"))
                    {
                        while (!(line = srParameter.ReadLine()).Contains("---"))
                        {
                            content = line.Split(',');
                            TestSetting.SETTING_LTETDD_B38.Add(content[0], double.Parse(content[1]));
                        }
                        ParameterIndentify--;
                    }
                    #endregion  LTETDD_B38 Setting

                    #region  LTETDD_B40 Setting
                    if (line.Contains("--- LTETDD_B40 ---"))
                    {
                        while (!(line = srParameter.ReadLine()).Contains("---"))
                        {
                            content = line.Split(',');
                            TestSetting.SETTING_LTETDD_B40.Add(content[0], double.Parse(content[1]));
                        }
                        ParameterIndentify--;
                    }
                    #endregion  LTETDD_B40 Setting

                    #region  LTEFDD_LB Setting
                    if (line.Contains("--- LTEFDD_LB ---"))
                    {
                        while (!(line = srParameter.ReadLine()).Contains("---"))
                        {
                            content = line.Split(',');
                            TestSetting.SETTING_LTEFDD_LB.Add(content[0], double.Parse(content[1]));
                        }
                        ParameterIndentify--;
                    }
                    #endregion  LTEFDD_LB Setting

                    #region  LTEFDD_HB Setting
                    if (line.Contains("--- LTEFDD_HB ---"))
                    {
                        while (!(line = srParameter.ReadLine()).Contains("---"))
                        {
                            content = line.Split(',');
                            TestSetting.SETTING_LTEFDD_HB.Add(content[0], double.Parse(content[1]));
                        }
                        ParameterIndentify--;
                    }
                    #endregion  LTEFDD_HB Setting

                    #region  CDMA Setting
                    if (line.Contains("--- CDMA ---"))
                    {
                        while (!(line = srParameter.ReadLine()).Contains("---"))
                        {
                            content = line.Split(',');
                            TestSetting.SETTING_CDMA.Add(content[0], double.Parse(content[1]));
                        }
                        ParameterIndentify--;
                    }
                    #endregion  CDMA Setting

                    #region  EVDO Setting
                    if (line.Contains("--- EVDO ---"))
                    {
                        while (!(line = srParameter.ReadLine()).Contains("---"))
                        {
                            content = line.Split(',');
                            TestSetting.SETTING_EVDO.Add(content[0], double.Parse(content[1]));
                        }
                        ParameterIndentify--;
                    }
                    #endregion  EVDO Setting


                    if (line.Contains("--- Frequency List ---"))
                    {
                        line = srParameter.ReadLine();
                        ParameterIndentify--;
                    }

                    #region CW LB Frequency list
                    if (line.Contains("--- CW LB ---"))
                    {
                        while (!(line = srParameter.ReadLine()).Contains("---"))
                        {
                            tmpFreq = double.Parse(line);
                            if (!TestSetting.FREQ_CW_LB.ContainsKey(tmpFreq)) TestSetting.FREQ_CW_LB.Add(tmpFreq, 0.0);
                            if (!TestSetting.LOSS_SRC.ContainsKey(tmpFreq)) TestSetting.LOSS_SRC.Add(tmpFreq, 0.0);
                            if (!TestSetting.LOSS_SRC_ROLL.ContainsKey(tmpFreq)) TestSetting.LOSS_SRC_ROLL.Add(tmpFreq, 0.0);
                            if (!TestSetting.LOSS_MSR_POUT.ContainsKey(tmpFreq)) TestSetting.LOSS_MSR_POUT.Add(tmpFreq, 0.0);
                            if (!TestSetting.LOSS_MSR_THROUGH.ContainsKey(tmpFreq)) TestSetting.LOSS_MSR_THROUGH.Add(tmpFreq, 0.0);
                            //Harmonic
                            for (int i = 2; i <= 6; i++)
                            {
                                if (!TestSetting.LOSS_SRC_ROLL.ContainsKey(tmpFreq * i)) TestSetting.LOSS_SRC_ROLL.Add(tmpFreq * i, 0.0);
                                if (!TestSetting.LOSS_MSR_FILTER_LB.ContainsKey(tmpFreq * i)) TestSetting.LOSS_MSR_FILTER_LB.Add(tmpFreq * i, 0.0);
                            }
                        }
                        ParameterIndentify--;
                    }
                    #endregion

                    #region CW HB Frequency list
                    if (line.Contains("--- CW HB ---"))
                    {
                        while (!(line = srParameter.ReadLine()).Contains("---"))
                        {
                            tmpFreq = double.Parse(line);
                            if (!TestSetting.FREQ_CW_HB.ContainsKey(tmpFreq)) TestSetting.FREQ_CW_HB.Add(tmpFreq, 0.0);
                            if (!TestSetting.LOSS_SRC.ContainsKey(tmpFreq)) TestSetting.LOSS_SRC.Add(tmpFreq, 0.0);
                            if (!TestSetting.LOSS_SRC_ROLL.ContainsKey(tmpFreq)) TestSetting.LOSS_SRC_ROLL.Add(tmpFreq, 0.0);
                            if (!TestSetting.LOSS_MSR_POUT.ContainsKey(tmpFreq)) TestSetting.LOSS_MSR_POUT.Add(tmpFreq, 0.0);
                            if (!TestSetting.LOSS_MSR_THROUGH.ContainsKey(tmpFreq)) TestSetting.LOSS_MSR_THROUGH.Add(tmpFreq, 0.0);
                            //Harmonic
                            for (int i = 2; i <= 3; i++)
                            {
                                if (!TestSetting.LOSS_SRC_ROLL.ContainsKey(tmpFreq * i)) TestSetting.LOSS_SRC_ROLL.Add(tmpFreq * i, 0.0);
                                if (!TestSetting.LOSS_MSR_FILTER_HB.ContainsKey(tmpFreq * i)) TestSetting.LOSS_MSR_FILTER_HB.Add(tmpFreq * i, 0.0);
                            }
                        }
                        ParameterIndentify--;
                    }
                    #endregion

                    #region EDGE LB Frequency list
                    if (line.Contains("--- EDGE LB ---"))
                    {
                        while (!(line = srParameter.ReadLine()).Contains("---"))
                        {
                            tmpFreq = double.Parse(line);
                            if (!TestSetting.FREQ_EDGE_LB.ContainsKey(tmpFreq)) TestSetting.FREQ_EDGE_LB.Add(tmpFreq, 0.0);
                            if (!TestSetting.LOSS_SRC.ContainsKey(tmpFreq)) TestSetting.LOSS_SRC.Add(tmpFreq, 0.0);
                            if (!TestSetting.LOSS_SRC_ROLL.ContainsKey(tmpFreq)) TestSetting.LOSS_SRC_ROLL.Add(tmpFreq, 0.0);
                            if (!TestSetting.LOSS_MSR_POUT.ContainsKey(tmpFreq)) TestSetting.LOSS_MSR_POUT.Add(tmpFreq, 0.0);
                            if (!TestSetting.LOSS_MSR_THROUGH.ContainsKey(tmpFreq)) TestSetting.LOSS_MSR_THROUGH.Add(tmpFreq, 0.0);
                        }
                        ParameterIndentify--;
                    }
                    #endregion

                    #region EDGE HB Frequency list
                    if (line.Contains("--- EDGE HB ---"))
                    {
                        while (!(line = srParameter.ReadLine()).Contains("---"))
                        {
                            tmpFreq = double.Parse(line);
                            if (!TestSetting.FREQ_EDGE_HB.ContainsKey(tmpFreq)) TestSetting.FREQ_EDGE_HB.Add(tmpFreq, 0.0);
                            if (!TestSetting.LOSS_SRC.ContainsKey(tmpFreq)) TestSetting.LOSS_SRC.Add(tmpFreq, 0.0);
                            if (!TestSetting.LOSS_SRC_ROLL.ContainsKey(tmpFreq)) TestSetting.LOSS_SRC_ROLL.Add(tmpFreq, 0.0);
                            if (!TestSetting.LOSS_MSR_POUT.ContainsKey(tmpFreq)) TestSetting.LOSS_MSR_POUT.Add(tmpFreq, 0.0);
                            if (!TestSetting.LOSS_MSR_THROUGH.ContainsKey(tmpFreq)) TestSetting.LOSS_MSR_THROUGH.Add(tmpFreq, 0.0);
                        }
                        ParameterIndentify--;
                    }
                    #endregion

                    #region TDSCDMA Frequency list
                    if (line.Contains("--- TDSCDMA ---"))
                    {
                        while (!(line = srParameter.ReadLine()).Contains("---"))
                        {
                            tmpFreq = double.Parse(line);
                            if (!TestSetting.FREQ_TDSCDMA.ContainsKey(tmpFreq)) TestSetting.FREQ_TDSCDMA.Add(tmpFreq, 0.0);
                            if (!TestSetting.LOSS_SRC.ContainsKey(tmpFreq)) TestSetting.LOSS_SRC.Add(tmpFreq, 0.0);
                            if (!TestSetting.LOSS_SRC_ROLL.ContainsKey(tmpFreq)) TestSetting.LOSS_SRC_ROLL.Add(tmpFreq, 0.0);
                            if (!TestSetting.LOSS_MSR_POUT.ContainsKey(tmpFreq)) TestSetting.LOSS_MSR_POUT.Add(tmpFreq, 0.0);
                            if (!TestSetting.LOSS_MSR_THROUGH.ContainsKey(tmpFreq)) TestSetting.LOSS_MSR_THROUGH.Add(tmpFreq, 0.0);
                        }
                        ParameterIndentify--;
                    }
                    #endregion

                    #region WCDMA Frequency list
                    if (line.Contains("--- WCDMA ---"))
                    {
                        while (!(line = srParameter.ReadLine()).Contains("---"))
                        {
                            tmpFreq = double.Parse(line);
                            if (!TestSetting.FREQ_WCDMA.ContainsKey(tmpFreq)) TestSetting.FREQ_WCDMA.Add(tmpFreq, 0.0);
                            if (!TestSetting.LOSS_SRC.ContainsKey(tmpFreq)) TestSetting.LOSS_SRC.Add(tmpFreq, 0.0);
                            if (!TestSetting.LOSS_SRC_ROLL.ContainsKey(tmpFreq)) TestSetting.LOSS_SRC_ROLL.Add(tmpFreq, 0.0);
                            if (!TestSetting.LOSS_MSR_POUT.ContainsKey(tmpFreq)) TestSetting.LOSS_MSR_POUT.Add(tmpFreq, 0.0);
                            if (!TestSetting.LOSS_MSR_THROUGH.ContainsKey(tmpFreq)) TestSetting.LOSS_MSR_THROUGH.Add(tmpFreq, 0.0);
                        }
                        ParameterIndentify--;
                    }
                    #endregion

                    #region LTE TDD_B38 Frequency list
                    if (line.Contains("--- LTETDD_B38 ---"))
                    {
                        while (!(line = srParameter.ReadLine()).Contains("---"))
                        {
                            tmpFreq = double.Parse(line);
                            if (!TestSetting.FREQ_LTETDD_B38.ContainsKey(tmpFreq)) TestSetting.FREQ_LTETDD_B38.Add(tmpFreq, 0.0);
                            if (!TestSetting.LOSS_SRC.ContainsKey(tmpFreq)) TestSetting.LOSS_SRC.Add(tmpFreq, 0.0);
                            if (!TestSetting.LOSS_SRC_ROLL.ContainsKey(tmpFreq)) TestSetting.LOSS_SRC_ROLL.Add(tmpFreq, 0.0);
                            if (!TestSetting.LOSS_MSR_POUT.ContainsKey(tmpFreq)) TestSetting.LOSS_MSR_POUT.Add(tmpFreq, 0.0);
                            if (!TestSetting.LOSS_MSR_THROUGH.ContainsKey(tmpFreq)) TestSetting.LOSS_MSR_THROUGH.Add(tmpFreq, 0.0);
                        }
                        ParameterIndentify--;
                    }
                    #endregion

                    #region LTE TDD_B40 Frequency list
                    if (line.Contains("--- LTETDD_B40 ---"))
                    {
                        while (!(line = srParameter.ReadLine()).Contains("---"))
                        {
                            tmpFreq = double.Parse(line);
                            if (!TestSetting.FREQ_LTETDD_B40.ContainsKey(tmpFreq)) TestSetting.FREQ_LTETDD_B40.Add(tmpFreq, 0.0);
                            if (!TestSetting.LOSS_SRC.ContainsKey(tmpFreq)) TestSetting.LOSS_SRC.Add(tmpFreq, 0.0);
                            if (!TestSetting.LOSS_SRC_ROLL.ContainsKey(tmpFreq)) TestSetting.LOSS_SRC_ROLL.Add(tmpFreq, 0.0);
                            if (!TestSetting.LOSS_MSR_POUT.ContainsKey(tmpFreq)) TestSetting.LOSS_MSR_POUT.Add(tmpFreq, 0.0);
                            if (!TestSetting.LOSS_MSR_THROUGH.ContainsKey(tmpFreq)) TestSetting.LOSS_MSR_THROUGH.Add(tmpFreq, 0.0);
                        }
                        ParameterIndentify--;
                    }
                    #endregion

                    #region LTE FDD LB Frequency list
                    if (line.Contains("--- LTEFDD_LB ---"))
                    {
                        while (!(line = srParameter.ReadLine()).Contains("---"))
                        {
                            tmpFreq = double.Parse(line);
                            if (!TestSetting.FREQ_LTEFDD_LB.ContainsKey(tmpFreq)) TestSetting.FREQ_LTEFDD_LB.Add(tmpFreq, 0.0);
                            if (!TestSetting.LOSS_SRC.ContainsKey(tmpFreq)) TestSetting.LOSS_SRC.Add(tmpFreq, 0.0);
                            if (!TestSetting.LOSS_SRC_ROLL.ContainsKey(tmpFreq)) TestSetting.LOSS_SRC_ROLL.Add(tmpFreq, 0.0);
                            if (!TestSetting.LOSS_MSR_POUT.ContainsKey(tmpFreq)) TestSetting.LOSS_MSR_POUT.Add(tmpFreq, 0.0);
                            if (!TestSetting.LOSS_MSR_THROUGH.ContainsKey(tmpFreq)) TestSetting.LOSS_MSR_THROUGH.Add(tmpFreq, 0.0);
                        }
                        ParameterIndentify--;
                    }
                    #endregion

                    #region LTE FDD HB Frequency list
                    if (line.Contains("--- LTEFDD_HB ---"))
                    {
                        while (!(line = srParameter.ReadLine()).Contains("---"))
                        {
                            tmpFreq = double.Parse(line);
                            if (!TestSetting.FREQ_LTEFDD_HB.ContainsKey(tmpFreq)) TestSetting.FREQ_LTEFDD_HB.Add(tmpFreq, 0.0);
                            if (!TestSetting.LOSS_SRC.ContainsKey(tmpFreq)) TestSetting.LOSS_SRC.Add(tmpFreq, 0.0);
                            if (!TestSetting.LOSS_SRC_ROLL.ContainsKey(tmpFreq)) TestSetting.LOSS_SRC_ROLL.Add(tmpFreq, 0.0);
                            if (!TestSetting.LOSS_MSR_POUT.ContainsKey(tmpFreq)) TestSetting.LOSS_MSR_POUT.Add(tmpFreq, 0.0);
                            if (!TestSetting.LOSS_MSR_THROUGH.ContainsKey(tmpFreq)) TestSetting.LOSS_MSR_THROUGH.Add(tmpFreq, 0.0);
                        }
                        ParameterIndentify--;
                    }
                    #endregion

                    #region CDMA Frequency list
                    if (line.Contains("--- CDMA ---"))
                    {
                        while (!(line = srParameter.ReadLine()).Contains("---"))
                        {
                            tmpFreq = double.Parse(line);
                            if (!TestSetting.FREQ_CDMA.ContainsKey(tmpFreq)) TestSetting.FREQ_CDMA.Add(tmpFreq, 0.0);
                            if (!TestSetting.LOSS_SRC.ContainsKey(tmpFreq)) TestSetting.LOSS_SRC.Add(tmpFreq, 0.0);
                            if (!TestSetting.LOSS_SRC_ROLL.ContainsKey(tmpFreq)) TestSetting.LOSS_SRC_ROLL.Add(tmpFreq, 0.0);
                            if (!TestSetting.LOSS_MSR_POUT.ContainsKey(tmpFreq)) TestSetting.LOSS_MSR_POUT.Add(tmpFreq, 0.0);
                            if (!TestSetting.LOSS_MSR_THROUGH.ContainsKey(tmpFreq)) TestSetting.LOSS_MSR_THROUGH.Add(tmpFreq, 0.0);
                        }
                        ParameterIndentify--;
                    }
                    #endregion

                    #region EVDO Frequency list
                    if (line.Contains("--- EVDO ---"))
                    {
                        while (!(line = srParameter.ReadLine()).Contains("---"))
                        {
                            tmpFreq = double.Parse(line);
                            if (!TestSetting.FREQ_EVDO.ContainsKey(tmpFreq)) TestSetting.FREQ_EVDO.Add(tmpFreq, 0.0);
                            if (!TestSetting.LOSS_SRC.ContainsKey(tmpFreq)) TestSetting.LOSS_SRC.Add(tmpFreq, 0.0);
                            if (!TestSetting.LOSS_SRC_ROLL.ContainsKey(tmpFreq)) TestSetting.LOSS_SRC_ROLL.Add(tmpFreq, 0.0);
                            if (!TestSetting.LOSS_MSR_POUT.ContainsKey(tmpFreq)) TestSetting.LOSS_MSR_POUT.Add(tmpFreq, 0.0);
                            if (!TestSetting.LOSS_MSR_THROUGH.ContainsKey(tmpFreq)) TestSetting.LOSS_MSR_THROUGH.Add(tmpFreq, 0.0);
                        }
                        ParameterIndentify--;
                    }
                    #endregion

                    if (!line.Contains("---"))
                        line = srParameter.ReadLine();
                    else if (line.Contains("--- The End ---"))
                        break;

                }

                srParameter.Close();

                if (ParameterIndentify != 0)
                {
                    MessageBox.Show("Parameter Setting  file is interrupt in " + Program.strSweepParameter_FileName
                                        + " \r\nPlease contact the developer",
                                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                #endregion *** Read Parameter Setting & Build Frequence List ***

                #region *** Start Test Program ***
                // everything is ready, start the test program
                if (MessageBox.Show("Perform Loss Comp?", "Loss Comp", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    NewLossComp frmNewLossComp = new NewLossComp();
                    frmNewLossComp.ShowDialog();
                    if (frmNewLossComp.DialogResult == DialogResult.OK)        //if Losscomp form closed, start Benchtest from
                        Application.Run(new SweepTest());
                }
                else
                    Application.Run(new SweepTest());

                #endregion *** Start Test Program ***
            }
            // if read configuration file fail, abort load the test program.
            else if (frmStart.DialogResult == DialogResult.OK)
            {
                debug_mipi = false;

                #region *** Check tp Configuration file ***
                strFilePath_Testcfg = strFilePath + tp + ".csv";
                if (!File.Exists(strFilePath_Testcfg))
                {
                    MessageBox.Show("Read " + tp + " configuration file fail.\r\nThere is no configuration file '"
                                        + tp + ".csv' in cfg folder", "oops!!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                #endregion *** Check tp Configuration file ***

                #region *** Get program configuration information ***
                int i = 1;
                string line;
                string[] content;
                ProductTest = new Vanchip.Common.ProductTest[TestSetting.MaxTestItem];

                StreamReader srProduct = new StreamReader(strFilePath_Testcfg);
                //Last Cal Date
                line = srProduct.ReadLine();
                content = line.Split(',');
                lastCalDate = content[0];
                //csv Header
                csvHeader = srProduct.ReadLine();
                while ((line = srProduct.ReadLine()) != null)
                {
                    if (line == "") break;
                    content = line.Split(',');
                    ProductTest[i].Item = Convert.ToInt32(content[0]);
                    ProductTest[i].TestItem = content[1];
                    ProductTest[i].Description = content[2];
                    ProductTest[i].Units = content[3];
                    ProductTest[i].LowLimit = Convert.ToDouble(content[4]);
                    ProductTest[i].HighLimit = Convert.ToDouble(content[5]);
                    ProductTest[i].VCC = Convert.ToDouble(content[6]);

                    if (content[7] == "0")
                        ProductTest[i].VBAT = 0.01;
                    else
                        ProductTest[i].VBAT = Convert.ToDouble(content[7]);

                    if (content[8] == "0")
                        ProductTest[i].Vramp = 0.01;
                    else
                        ProductTest[i].Vramp = Convert.ToDouble(content[8]);

                    if (content[9] == "0")
                        ProductTest[i].Txen_Ven = 0.01;
                    else
                        ProductTest[i].Txen_Ven = Convert.ToDouble(content[9]);

                    if (content[10] == "0")
                        ProductTest[i].Gpctrl0_Vmode0 = 0.01;
                    else
                        ProductTest[i].Gpctrl0_Vmode0 = Convert.ToDouble(content[10]);

                    if (content[11] == "0")
                        ProductTest[i].Gpctrl1_Vmode1 = 0.01;
                    else
                        ProductTest[i].Gpctrl1_Vmode1 = Convert.ToDouble(content[11]);

                    if (content[12] == "0")
                        ProductTest[i].Gpctrl2_Vmode2 = 0.01;
                    else
                        ProductTest[i].Gpctrl2_Vmode2 = Convert.ToDouble(content[12]);

                    ProductTest[i].Pin = Convert.ToDouble(content[13]);
                    ProductTest[i].Pout = Convert.ToDouble(content[14]);
                    ProductTest[i].FreqIn = Convert.ToDouble(content[15]);
                    ProductTest[i].FreqOut = Convert.ToDouble(content[16]);
                    ProductTest[i].LossIn = Convert.ToDouble(content[17]);
                    ProductTest[i].LossOut = Convert.ToDouble(content[18]);
                    ProductTest[i].SocketOffset = Convert.ToDouble(content[19]);

                    i++;
                }
                srProduct.Close();
                #endregion Get program configuration information

                #region *** Start Test Program ***
                // everything is ready, start the test program
                if (MessageBox.Show("Perform Loss Comp?", "Loss Comp", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    LossComp frmLoss = new LossComp();
                    frmLoss.ShowDialog();
                    if (frmLoss.DialogResult == DialogResult.OK)        //if Losscomp form closed, start Benchtest from
                        Application.Run(new BenchTest());
                }
                else
                    Application.Run(new BenchTest());

                #endregion *** Start Test Program ***
            }

        }
    }

    static class TestSetting
    {
        public const string NA = "N/A";

        public const double TARGET_POUT_CWLB = 33;
        public const double TARGET_POUT_CWHB = 30;

        public const string NAME_VCC = "VCC";
        public const string NAME_VBAT = "VBAT";
        public const string NAME_GPCTRL = "GPCTRL";
        public const string NAME_TXEN = "TXEN";
        public const string NAME_VEN = "VEN";
        public const string NAME_MODE = "MODE";
        public const string NAME_START = "START";
        public const string NAME_STOP = "STOP";
        public const string NAME_STEP = "STEP";
        public const string NAME_PIN_VRAMP = "PIN_VRAMP";

        public const string MODE_CW_LB = "CW_LB";
        public const string MODE_CW_HB = "CW_HB";
        public const string MODE_EDGE_LB = "EDGE_LB";
        public const string MODE_EDGE_HB = "EDGE_HB";
        public const string MODE_TDSCDMA = "TDSCDMA";
        public const string MODE_WCDMA = "WCDMA";
        public const string MODE_LTETDD_B38 = "LTETDD(B38)";
        public const string MODE_LTETDD_B40 = "LTETDD(B40)";
        public const string MODE_LTEFDD_LB = "LTEFDD(LB)";
        public const string MODE_LTEFDD_HB = "LTEFDD(HB)";
        public const string MODE_CDMA = "CDMA";
        public const string MODE_EVDO = "EVDO";

        public const int MaxTestItem = 500;

        public const int DELAY_ARB_in_ms = 60;
        public const int DELAY_SIGGEN_in_ms = 60;
        public const int DELAY_SIGGEN_OLD_in_ms = 300;
        public const int DELAY_POWER_METER_in_ms = 200;
        public const int DELAY_POWER_METER_OLD_in_ms = 1000;
        public const int DELAY_VSA_in_ms = 60;
        public const int DELAY_POWER_SUPPLLY_in_ms = 60;

        public const int POWERMETER_AVERAGE_CW = 128;

        public const double ARB_PULSE_FREQ_GMSK_in_khz = 216.638; //216.6377816291161;  //208.5;
        public const double ARB_PULSE_FREQ_LTETDD_in_khz = 200;


        public static Dictionary<string, double> SETTING_GMSK_LB = new Dictionary<string, double>();
        public static Dictionary<string, double> SETTING_GMSK_HB = new Dictionary<string, double>();
        public static Dictionary<string, double> SETTING_EDGE_LB = new Dictionary<string, double>();
        public static Dictionary<string, double> SETTING_EDGE_HB = new Dictionary<string, double>();
        public static Dictionary<string, double> SETTING_TDSCDMA = new Dictionary<string, double>();
        public static Dictionary<string, double> SETTING_WCDMA = new Dictionary<string, double>();
        public static Dictionary<string, double> SETTING_LTETDD_B38 = new Dictionary<string, double>();
        public static Dictionary<string, double> SETTING_LTETDD_B40 = new Dictionary<string, double>();
        public static Dictionary<string, double> SETTING_LTEFDD_LB = new Dictionary<string, double>();
        public static Dictionary<string, double> SETTING_LTEFDD_HB = new Dictionary<string, double>();
        public static Dictionary<string, double> SETTING_CDMA = new Dictionary<string, double>();
        public static Dictionary<string, double> SETTING_EVDO = new Dictionary<string, double>();

        public static Dictionary<double, double> FREQ_CW_LB = new Dictionary<double, double>();
        public static Dictionary<double, double> FREQ_CW_HB = new Dictionary<double, double>();
        public static Dictionary<double, double> FREQ_EDGE_LB = new Dictionary<double, double>();
        public static Dictionary<double, double> FREQ_EDGE_HB = new Dictionary<double, double>();
        public static Dictionary<double, double> FREQ_TDSCDMA = new Dictionary<double, double>();
        public static Dictionary<double, double> FREQ_WCDMA = new Dictionary<double, double>();
        public static Dictionary<double, double> FREQ_LTETDD_B38 = new Dictionary<double, double>();
        public static Dictionary<double, double> FREQ_LTETDD_B40 = new Dictionary<double, double>();
        public static Dictionary<double, double> FREQ_LTEFDD_LB = new Dictionary<double, double>();
        public static Dictionary<double, double> FREQ_LTEFDD_HB = new Dictionary<double, double>();
        public static Dictionary<double, double> FREQ_CDMA = new Dictionary<double, double>();
        public static Dictionary<double, double> FREQ_EVDO = new Dictionary<double, double>();

        public static Dictionary<double, double> LOSS_SRC = new Dictionary<double, double>();
        public static Dictionary<double, double> LOSS_SRC_ROLL = new Dictionary<double, double>();
        public static Dictionary<double, double> LOSS_MSR_POUT = new Dictionary<double, double>();
        public static Dictionary<double, double> LOSS_MSR_THROUGH = new Dictionary<double, double>();
        public static Dictionary<double, double> LOSS_MSR_FILTER_LB = new Dictionary<double, double>();
        public static Dictionary<double, double> LOSS_MSR_FILTER_HB = new Dictionary<double, double>();

        public static List<double> FREQLIST;

        public static double LEVEL_VCC;
        public static double LEVEL_VBAT;
        public static double LEVEL_TXEN;
        public static double LEVEL_GPCTRL;
        public static double LEVEL_PIN_VRAMP;
        public static double LEVEL_START;
        public static double LEVEL_STOP;
        public static double LEVEL_STEP;


    }
}
