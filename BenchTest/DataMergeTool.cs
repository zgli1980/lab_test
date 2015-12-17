using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using Excel;

namespace Bench_Test
{
    public partial class DataMergeTool : Form
    {
        public DataMergeTool()
        {
            InitializeComponent();
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog opencsv = new OpenFileDialog();

            opencsv.Filter = "CSV File|*.csv";
            opencsv.Title = "Open File";
            opencsv.Multiselect = true;
            lbxFile.Items.Clear();

            if (opencsv.ShowDialog() == DialogResult.OK)
            {
                List<string> strFileName =  opencsv.SafeFileNames.ToList();

                //strFileName.Sort();

                foreach (string filename in strFileName)
                {
                    lbxFile.Items.Add(filename);
                }
                lbxFile.Tag = opencsv.FileNames[0].Substring(0, opencsv.FileName.Length - opencsv.SafeFileNames[0].Length);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            bool is_limitset = false;
            SaveFileDialog savefile = new SaveFileDialog();
            savefile.Filter = "Excel File(.xls)|*.xls";
            savefile.FileName = ".xls";

            if (savefile.ShowDialog() == DialogResult.OK)
            {
                btnSave.Enabled = false;

                System.Windows.Forms.Application.DoEvents();
                string filename = savefile.FileName;
                int DeviceCount = 0;

                Excel.ApplicationClass excel = new ApplicationClass();
                excel.Visible = false;

                Workbook wBook = excel.Workbooks.Add(true);
                //Worksheet wSheet = (Excel._Worksheet)wBook.ActiveSheet;
                Worksheet wSheet = (Excel.Worksheet)wBook.ActiveSheet;
                wSheet.Name = "Data";

                foreach (string stmp in lbxFile.Items)
                {
                    bool DeviceFail = false;
                    bool FirstLine = true;
                    int intParameterCount = 1;
                    string DeviceID = stmp.Substring(0, stmp.Length - 4);

                    FileStream fi = new FileStream(lbxFile.Tag + stmp, FileMode.Open, FileAccess.Read);
                    StreamReader sr = new StreamReader(fi);
                    sr.BaseStream.Seek(0,SeekOrigin.Begin);

                    string line = sr.ReadLine();
                    line = sr.ReadLine();
                    line = sr.ReadLine();
                    line = sr.ReadLine();

                    #region --- Header ---
                    if (!is_limitset)
                    {
                        while (line != null && line != "")
                        {
                            string[] tmp = line.Split(',');
                            excel.Cells[intParameterCount, 1] = tmp[0];     // #
                            excel.Cells[intParameterCount, 2] = tmp[1];     // Test Item
                            excel.Cells[intParameterCount, 3] = tmp[4];     // Low Limit
                            excel.Cells[intParameterCount, 4] = tmp[5];     // High Limit
                            excel.Cells[intParameterCount, 5] = tmp[2];     // Units
                            
                            if (FirstLine)
                            {
                                excel.Cells[intParameterCount, 6] = DeviceID;    // Result
                                FirstLine = false;
                            }
                            else
                                excel.Cells[intParameterCount, 6] = tmp[3];     // Result

                            if (tmp[6].ToUpper() == "FAIL")
                            {
                                wSheet.get_Range(excel.Cells[intParameterCount, 6], excel.Cells[intParameterCount, 6]).Interior.Color = System.Drawing.ColorTranslator.ToWin32(Color.Red);
                                DeviceFail = true;
                            }
                            line = sr.ReadLine();
                            intParameterCount++;
                        }
                        excel.Cells[intParameterCount, 1] = intParameterCount - 1;
                        excel.Cells[intParameterCount, 2] = "Status"; 

                        if (DeviceFail)
                            excel.Cells[intParameterCount, 6] = "Fail";     // PASS FAIL
                        else
                            excel.Cells[intParameterCount, 6] = "Pass";     // PASS FAIL

                        DeviceCount++;
                        is_limitset = true;
                    }
                    #endregion --- Header ---

                    #region --- Device Data ---
                    else
                    {
                        while (line != null && line != "")
                        {
                            string[] tmp = line.Split(',');
                            if (FirstLine)
                            {
                                excel.Cells[intParameterCount, 6 + DeviceCount] = DeviceID;    // Result
                                FirstLine = false;
                            }
                            else
                                excel.Cells[intParameterCount, 6 + DeviceCount] = tmp[3];     // Result

                            if (tmp[6].ToUpper() == "FAIL")
                            {
                                wSheet.get_Range(excel.Cells[intParameterCount, 6 + DeviceCount], excel.Cells[intParameterCount, 6 + DeviceCount]).Interior.Color = System.Drawing.ColorTranslator.ToWin32(Color.Red);
                                DeviceFail = true;
                            }
                            line = sr.ReadLine();
                            intParameterCount++;
                        }
                        if (DeviceFail)
                            excel.Cells[intParameterCount, 6 + DeviceCount] = "Fail";     // PASS FAIL
                        else
                            excel.Cells[intParameterCount, 6 + DeviceCount] = "Pass";     // PASS FAIL

                        DeviceCount++;
                    }
                    #endregion --- Device Data ---
                    sr.Close();
                    fi.Close();
                }


                //设置禁止弹出保存和覆盖的询问提示框
                excel.DisplayAlerts = false;
                excel.AlertBeforeOverwriting = true;
                //保存工作薄
                //  wBook.Save();
                //每次保存激活的表，这样才能多次操作保存不同的Excel表，默认保存位置是在”我的文档"

                //excel.Cells.Font.Size = 12;
                //excel.Cells.Font.Bold = false;
                //  Excel.Range m_objRange = m_objRange.get_Range(1, 3);
                //wSheet.get_Range(excel.Cells[1, 3], excel.Cells[1, 3]).Font.Size = 24;
                //wSheet.get_Range(excel.Cells[1, 3], excel.Cells[1, 3]).Font.Bold = true;
                //wSheet.get_Range(excel.Cells[3, 1], excel.Cells[5, 1]).Interior.Color = System.Drawing.ColorTranslator.ToWin32(Color.Red); 
                //wSheet.get_Range(excel.Cells[3, 1], excel.Cells[5, 1]).Font.ColorIndex = 3;//此处设为红色，不能用Font.Color来设置颜色
                //  m_objRange.Cells.Font.Size = 24;
                //  m_objRange.Cells.Font.Bold = true;

                wSheet.get_Range("A1", "HE200").Cells.Font.Size = 9;
                wSheet.get_Range("A1", "HE200").VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                wSheet.get_Range("A1", "HE200").HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                wSheet.get_Range("B2", "B200").HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                wSheet.get_Range("B1", "B200").ColumnWidth = 21;

                excel.ActiveWorkbook.SaveCopyAs(filename);
                excel.Quit();
                //this.Close();
                MessageBox.Show("File has been save to " + filename);
                btnSave.Enabled = true;

            }

        }
    }
}
