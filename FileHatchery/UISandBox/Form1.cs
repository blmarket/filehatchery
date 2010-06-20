using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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
            tt.Tick += new EventHandler(tt_Tick);
            tt.Enabled = true;
        }

        void tt_Tick(object sender, EventArgs e)
        {
            SelfLabel tmp = new SelfLabel();
            tmp.Text = "test1";
            tmp.Visible = true;
            tmp.Parent = flowLayoutPanel1;
            flowLayoutPanel1.Controls.Add(tmp);
            tt.Enabled = false;
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

    public class SelfLabel : Label
    {
        private Timer hideTimer;
        private double opacity;

        public SelfLabel()
        {
            hideTimer = new Timer();
            opacity = 0;
            this.Click += new EventHandler(SelfLabel_Click);
        }

        ~SelfLabel()
        {
            int a = 1;
            a = a + 1;
        }        

        void SelfLabel_Click(object sender, EventArgs e)
        {
            hideTimer.Enabled = true;
            hideTimer.Tick += new EventHandler(hideTimer_Tick);
        }

        void hideTimer_Tick(object sender, EventArgs e)
        {
            Dispose();

            if(Parent != null)
                Parent.Controls.Remove(this);
            GC.Collect();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.ResumeLayout(false);
        }
    }
}
