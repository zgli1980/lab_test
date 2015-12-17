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
    public partial class Datalog : Form
    {
        public Datalog()
        {
            InitializeComponent();
        }

        private void rtxtDatalog_TextChanged(object sender, EventArgs e)
        {
            rtxtDatalog.AppendText("asdasd asdasdasd asdas\r\n ");

            rtxtDatalog.AppendText("asdasd asdasdasd asdas\r\n ");

            rtxtDatalog.AppendText("asdasd asdasdasd asdas\r\n ");

            rtxtDatalog.AppendText("asdasd asdasdasd asdas\r\n ");

            rtxtDatalog.AppendText("asdasd asdasdasd asdas\r\n ");

            rtxtDatalog.AppendText("asdasd asdasdasd asdas\r\n ");

            rtxtDatalog.AppendText("asdasd asdasdasd asdas\r\n ");

            rtxtDatalog.AppendText("asdasd asdasdasd asdas\r\n ");

        }

        private void Datalog_Load(object sender, EventArgs e)
        {

        }

        private void hideToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Hide();
        }
    }
}
