using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Reflection;

namespace FileHatchery
{
    interface IKeyHandler
    {
        bool HandleKey(Keys keyData);
        bool NeedsFocus { get; }
        void setFocus();
    }

    public partial class Form1 : Form
    {
        DirectoryBrowser browser;
        TextBox console;

        public Form1()
        {
            InitializeComponent();
        }

        // 모든 키입력에 대해 먼저 처리하는 곳이라능.
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (console.Visible)
            {
                // do nothing
                console.Focus();
                if (keyData == Keys.Tab) return true;
            }
            else
            {
                //            const int WM_KEYDOWN = 0x100;
                if ((keyData & Keys.Alt) == Keys.Alt || (keyData & Keys.Control) == Keys.Control)
                {
                    Program.engine.RunShortcut(keyData);
                }

                if (keyData == Keys.F5)
                {
                    Program.engine.RunCommand("refresh");
                    return true;
                }

                if (keyData == Keys.Delete)
                {
                    Program.engine.RunCommand("delete");
                    return true;
                }

                if (((IKeyHandler)browser).HandleKey(keyData)) return true;

            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            console = Config.Selector.Console;
            console.Hide();

            Font font = Config.Selector.Font;
            if (font != null)
                Program.engine.Font = font;

            demoFlowPanel1.Controls.Add(console);

            //            consoleTextBox1.engine = Program.engine;

            browser = Program.engine.Browser;
            browser.onChangeDirectory += new changeDelegate(browser_onChangeDirectory);
            browserPanel.PageService = browser;
            browser.onChangeCursor += new changeDelegate(browserPanel.onCursorChanged);

            browser.CurrentDir = new DirectoryInfo(Directory.GetCurrentDirectory());

            Timer timer = new Timer();
            timer.Interval = 1000;
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();
        }

        void timer_Tick(object sender, EventArgs e)
        {
//            browser.Refresh();
        }

        public class TmpLabel : Control
        {
            public IBrowserItem m_item;

            public TmpLabel(IBrowserItem item) 
            { 
                m_item = item;
                m_item.onChanged += delegate()
                {
                    Refresh();
                };
                InitializeComponent();
            }

            private void TmpLabel_Paint(object sender, PaintEventArgs e)
            {
                Brush background = Brushes.White, foreground = Brushes.Black;
                if ((m_item.State & BrowserItemState.Selected) == BrowserItemState.Selected)
                {
                    background = Brushes.Black;
                    foreground = Brushes.White;
                }

                if ((m_item.State & BrowserItemState.Marked) == BrowserItemState.Marked)
                {
                    foreground = Brushes.Green;
                }

                Graphics g = e.Graphics;
                g.FillRectangle(background, ClientRectangle);
                if(m_item.Icon != null)
                    g.DrawIcon(m_item.Icon, 0, 0);
                g.DrawString(this.Text, Font, foreground, 32, 0);
            }

            private void InitializeComponent() {
                this.SuspendLayout();
                this.Paint += new System.Windows.Forms.PaintEventHandler(this.TmpLabel_Paint);
                this.Font = Program.engine.Font;
                this.ResumeLayout(false);
            }
        }

        void browser_onChangeDirectory()
        {
            browserPanel.Controls.Clear();
            try
            {
                List<IBrowserItem> items = browser.Items;

                foreach (IBrowserItem item in items)
                {
                    Control tmp = new TmpLabel(item);
                    tmp.Size = new Size(400, 20);
                    tmp.Text = item.showName;
                    tmp.MouseClick += new MouseEventHandler(tmp_MouseClick);
                    tmp.MouseDoubleClick += new MouseEventHandler(tmp_MouseDoubleClick);
                    browserPanel.Controls.Add(tmp);
                }
            }
            catch (Exception EE)
            {
                MessageBox.Show(EE.Message);
            }
        }

        void tmp_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            TmpLabel tmp = (TmpLabel)sender;
            tmp.m_item.execute();
        }

        void tmp_MouseClick(object sender, MouseEventArgs e)
        {
            TmpLabel tmp = sender as TmpLabel;
            browser.SelectItem(tmp.m_item);
            if ((e.Button & MouseButtons.Right) == MouseButtons.Right)
            {
                Program.engine.ShowContextMenu(Cursor.Position);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Button btn = new Button();
            btn.Text = "KKKK";
            demoFlowPanel1.Controls.Add(btn);
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.OemSemicolon:
                    if (e.Shift)
                    {
                        if (console.Visible == false)
                        {
                            console.Text = "";
                            console.Show();
                            console.Focus();
                        }
                    }
                    break;
            }
        }

        private void 붙여넣기ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (console.Visible)
            {
                console.Paste();
                return;
            }
            Program.engine.RunCommand("paste");
            Program.engine.RunCommand("refresh");
        }

        private void 잘라내기ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Program.engine.RunCommand("cut selected");
        }

        private void 복사ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Program.engine.RunCommand("copy selected");
        }
    }
}
