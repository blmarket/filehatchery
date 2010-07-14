using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FileHatchery
{
    public partial class MyLabel : UserControl
    {
        private IBrowserItem m_item;

        public IBrowserItem Item { get { return m_item; } }

        public void onItemChanged(object obj, EventArgs e)
        {
            if (IsDisposed) return;
            if (InvokeRequired)
            {
                try
                {
                    Invoke(new EventHandler(onItemChanged));
                }
                catch (InvalidOperationException)
                {
                    // do nothing... but is it safe?
                }
            }
            else
            {
                Refresh();
            }
        }

        public MyLabel(IBrowserItem label)
        {
            m_item = label;

            m_item.onChanged += new EventHandler(onItemChanged);

            InitializeComponent();
            Font = Program.engine.Font;
        }

        private void MyLabel_Paint(object sender, PaintEventArgs e)
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
            if (m_item.Icon != null)
                g.DrawIcon(m_item.Icon, 0, 0);
            g.DrawString(m_item.showName, Font, foreground, 32, 0);
        }
    }
}
