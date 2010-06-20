using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace UISandBox
{
    public partial class Form1 : Form
    {
        Timer tt = new Timer();

        public Form1()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {
            tt.Interval = 1000;
            tt.Enabled = true;
            tt.Tick += new EventHandler(tt_Tick);
        }

        void tt_Tick(object sender, EventArgs e)
        {
            label1.Hide();
        }

        private void label2_Click(object sender, EventArgs e)
        {
            label2.Hide();
        }

        private void label3_Click(object sender, EventArgs e)
        {
            label3.Hide();
        }
    }
}
