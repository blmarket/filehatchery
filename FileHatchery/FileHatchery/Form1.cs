﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using FileHatchery.Engine;
using FileHatchery.Engine.Components;

namespace FileHatchery
{
    interface IKeyHandler
    {
        bool handleSpecialKey(Keys keyData);
        void handleChar(char key);
        bool NeedsFocus { get; }
        void setFocus();
    }

    partial class Form1 : Form
    {
        IBrowser browser;
        TextBox console;
        System.Windows.Forms.Timer m_demoFlowPanelTimer;
        Queue<KeyValuePair<int, Control>> m_ControlQueue;

        public Form1()
        {
            InitializeComponent();
            m_ControlQueue = new Queue<KeyValuePair<int, Control>>();
        }

        void m_demoFlowPanelTimer_Tick(object sender, EventArgs e)
        {
            while(m_ControlQueue.Count > 0)
            {
                var item = m_ControlQueue.Peek();
                if (item.Key < Environment.TickCount)
                {
                    demoFlowPanel1.Controls.Remove(item.Value);
                    item.Value.Dispose();
                    m_ControlQueue.Dequeue();
                }
                else
                {
                    break;
                }
            }
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
                //const int WM_KEYDOWN = 0x100;
                if ((keyData & Keys.Alt) == Keys.Alt || (keyData & Keys.Control) == Keys.Control)
                {
                    Program.engine.RunShortcut(keyData);
                }

                if (keyData == Keys.F2)
                {
                    string newName = Microsoft.VisualBasic.Interaction.InputBox("새 파일명을 입력해주세요");
                    Program.engine.RunCommand("rename " + newName);
                    return true;
                }

                if (keyData == Keys.F5)
                {
                    Program.engine.RunCommand("refresh");
                    return true;
                }

                if (keyData == Keys.F6)
                {
                    Program.engine.RunCommand("open c:\\windows\\system32");
                    return true;
                }

                if (keyData == Keys.Delete)
                {
                    Program.engine.RunCommand("delete");
                    return true;
                }

                if (keyData == Keys.OemPipe)
                {
                    Program.engine.RunCommand("goroot");
                    return true;
                }

                if (((IKeyHandler)browser).handleSpecialKey(keyData)) return true;

            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Program.engine.setComponent(typeof(IIconProducer), new IconQueue());
            IconGetter.RunWorkerAsync();

            console = ConfigSelector.createConsole();
            console.Hide();

            Font font = ConfigSelector.Font;
            if (font != null)
                Program.engine.Font = font;

            demoFlowPanel1.Controls.Add(console);

            //            consoleTextBox1.engine = Program.engine;

            browser = Program.engine.getComponent<IBrowser>();
            browser.DirectoryChanged += new EventHandler(browser_onChangeDirectory);
            browserPanel.PageService = browser;
            browser.CursorChanged += new EventHandler(browserPanel.onCursorChanged);

            browser.CurrentDir = new DirectoryInfo(Directory.GetCurrentDirectory());

            Program.engine.UINotify += new EventHandler<Engine.Notification.NotifyArgs>(onEngineNotification);

            m_demoFlowPanelTimer = new Timer();
            m_demoFlowPanelTimer.Interval = 100;
            m_demoFlowPanelTimer.Tick += new EventHandler(m_demoFlowPanelTimer_Tick);
            m_demoFlowPanelTimer.Start();
        }

        void onEngineNotification(object obj, Engine.Notification.NotifyArgs msg)
        {
            UITest(msg.Message);
        }

        void browser_onChangeDirectory(object senderobj, EventArgs ee)
        {
            browserPanel.Controls.Clear(); // FIXME: remove all eventhandlers.
            try
            {
                List<IBrowserItem> items = browser.Items;
                var labels = new MyLabel[items.Count];
                int cnt = 0;

                foreach (IBrowserItem item in items)
                {
                    var tmp = new MyLabel(item);
                    tmp.Size = new Size(400, 20);
                    tmp.Text = item.showName;
                    tmp.MouseDown += new MouseEventHandler(tmp_MouseDown);
                    tmp.MouseDoubleClick += new MouseEventHandler(tmp_MouseDoubleClick);
                    labels[cnt] = tmp;
                    cnt = cnt + 1;
                }

                browserPanel.Controls.AddRange(labels);
            }
            catch (Exception EE)
            {
                MessageBox.Show(EE.Message);
            }
        }

        void tmp_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            MyLabel tmp = sender as MyLabel;
            if (tmp == null)
                throw new InvalidDataException("object can't be casted into MyLabel");
            tmp.Item.accept(new NormalExecutor(Program.engine));
        }

        void tmp_MouseDown(object sender, MouseEventArgs e)
        {
            MyLabel tmp = sender as MyLabel;
            if (tmp == null)
                throw new InvalidDataException("object can't be casted into MyLabel");
            browser.SelectItem(tmp.Item);
            if ((e.Button & MouseButtons.Right) == MouseButtons.Right)
            {
                Program.engine.ShowContextMenu(Cursor.Position);
            }
            /*
            DataObject data = new DataObject();
            data.SetText("Hello World");
            DoDragDrop(data, DragDropEffects.All);
             * */
        }

        Random testRandom = new Random();
        public void UITest(string msg)
        {
            Color[] colors = new Color[] { Color.AliceBlue, Color.Aqua, Color.Azure, Color.Red, Color.RoyalBlue };
            Label tmp = new Label();
            tmp.Text = msg;
            Padding pad = tmp.Margin;
            pad.All = 0;
            tmp.Margin = pad;
            tmp.Visible = true;
            tmp.BackColor = colors[testRandom.Next(colors.Length)];

            demoFlowPanel1.Controls.Add(tmp);

            m_ControlQueue.Enqueue(new KeyValuePair<int, Control>(Environment.TickCount + 3000, tmp));
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.OemSemicolon:
                    if (console.Visible == false)
                    {
                        console.Text = "";
                        console.Show();
                        console.Focus();
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

        private void 모두선택ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Program.engine.RunCommand("alt-u");
        }

        private void 숨겨진파일보기ToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            Program.engine.RunCommand("show hidden files");
            //Program.engine.Browser.ShowHiddenFiles = this.숨겨진파일보기ToolStripMenuItem.Checked;
        }

        private void IconGetter_DoWork(object sender, DoWorkEventArgs e)
        {
            var obj = Program.engine.getComponent<IIconProducer>();
            if (obj == null) throw new Exception("IIconProducer expected to exists");
            IconQueue queue = obj as IconQueue;
            if (queue == null) return;
            queue.Work(IconGetter);              
        }

        private void IconGetter_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            IBrowserItem item = e.UserState as IBrowserItem;
            if (item != null)
            {
                var temp = item.onChanged;
                if (temp != null)
                    temp(item, EventArgs.Empty);
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            // FIXME: 일관되지 않은 느낌이다...
            Engine.Components.Config.IConfig cfg = Program.engine.getComponent<Engine.Components.Config.IConfig>();
            cfg.Save();
        }

        private void 디렉토리생성ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string dirName = Microsoft.VisualBasic.Interaction.InputBox("새 디렉토리 이름을 입력해주세요");
            Program.engine.RunCommand("mkdir " + dirName);
        }

        private void 홈디렉토리로이동하기ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            Program.engine.RunCommand("cd "+ path);
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (console.Visible)
            {
                return;
            }
            ((IKeyHandler)browser).handleChar(e.KeyChar);
        }
    }

    class IconQueue : IIconProducer
    {
        System.Threading.EventWaitHandle wh = new System.Threading.AutoResetEvent(false);
        Queue<IBrowserItem> tasks = new Queue<IBrowserItem>();

        public void EnqueueTask(IBrowserItem task)
        {
            lock (tasks)
            {
                tasks.Enqueue(task);
                wh.Set();
            }
        }

        public void ClearQueue()
        {
            lock (tasks)
            {
                tasks.Clear();
            }
        }

        public void Dispose()
        {
            ClearQueue();
        }

        public void Work(BackgroundWorker report)
        {
            while (true)
            {
                IBrowserItem task = null;
                lock (tasks)
                    if (tasks.Count > 0)
                    {
                        task = tasks.Dequeue();
                        if (task == null) return;
                    }
                if (task != null)
                {
                    System.Threading.Thread.Sleep(15);
                    Icon icon = ShellApi.Win32.getIcon(task.FullPath);
                    task.Icon = icon;
                    report.ReportProgress(0, task);
                }
                else
                    wh.WaitOne();         // No more tasks - wait for a signal
            }
        }
    }

}
