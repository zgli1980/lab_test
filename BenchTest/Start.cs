using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Bench_Test
{
    public partial class Start : Form
    {
        public Start()
        {
            InitializeComponent();
        }

        private void Start_Load(object sender, EventArgs e)
        {
            lbxProduct.Items.Clear();
            lbxProduct.Items.Add(Program.SweepTest);
            lbxProduct.Items.Add(Program.Mipi);

            System.IO.StreamReader srProduct = new System.IO.StreamReader(Program.strFilePath_Product);
            string line;

            while ((line = srProduct.ReadLine()) != null)
            {
                lbxProduct.Items.Add(line);
            }
            lbxProduct.SetSelected(0, true);
            cbxDUT.Checked = false;

        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            Program.tp = lbxProduct.SelectedItem.ToString();

            if (cbxDUT.Checked)
                Program.evb = false;
            else
                Program.evb = true;

            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }


    }
}
