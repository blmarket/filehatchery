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
            tt.Tick += new EventHandler(tt_Tick);
        }

        private void label1_Click(object sender, EventArgs e)
        {
            tt.Interval = 1000;
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
        int elaspedTime;

        public SelfLabel()
        {
            hideTimer = new Timer();
            opacity = 0;
            elaspedTime = 0;
            this.Click += new EventHandler(SelfLabel_Click);
        }

        void SelfLabel_Click(object sender, EventArgs e)
        {
            SetStyle(ControlStyles.Opaque, false);
            this.BackColor = Color.Red;
            hideTimer.Enabled = true;
            hideTimer.Interval = 10;
            hideTimer.Tick += new EventHandler(hideTimer_Tick);
        }

        void hideTimer_Tick(object sender, EventArgs e)
        {
            elaspedTime += 10;            

            if (elaspedTime >= 500)
            {
                hideTimer.Dispose();
                Dispose();

                if (Parent != null)
                    Parent.Controls.Remove(this);
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.ResumeLayout(false);
        }
    }
}
