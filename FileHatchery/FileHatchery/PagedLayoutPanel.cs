using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Layout;

namespace FileHatchery
{
    public interface IPagedLayoutInterface
    {
        int CursorPos { get; }
        void setRowSize(int rowSize);
    }

    public class PagedLayoutPanel : Panel
    {
        private PagedLayoutEngine layoutEngine;

        public PagedLayoutPanel()
        {
        }

        private PagedLayoutEngine PagedLayout
        {
            get
            {
                if (layoutEngine == null)
                {
                    layoutEngine = new PagedLayoutEngine();
                }

                return layoutEngine;
            }
        }

        public override LayoutEngine LayoutEngine
        {
            get
            {
                return PagedLayout;
            }
        }

        protected override Size DefaultSize
        {
            get
            {
                return Size.Empty;
            }
        }

        public IPagedLayoutInterface PageService
        {
            get
            {
                return PagedLayout.PageService;
            }
            set
            {
                if (value != null)
                {
                    PagedLayout.PageService = value;
                }
            }
        }

        public void onCursorChanged()
        {
            PerformLayout();
        }
    }

    public class PagedLayoutEngine : LayoutEngine
    {
        public IPagedLayoutInterface PageService { get; set; }

        const int colSize = 2;
        const int itemHeight = 20;

        public int RowSize(Control c) { return c.Height / itemHeight; }

        public override bool Layout(object container, LayoutEventArgs layoutEventArgs)
        {
            Control parent = container as Control;

            Size curSize = parent.Size;
            int rowSize = RowSize(parent);
            if (rowSize == 0) rowSize = 1; // fixes Issue 1 : http://code.google.com/p/filehatchery/issues/detail?id=1

            Size itemSize = new Size(curSize.Width / colSize, itemHeight);
            int numItems = rowSize * colSize;
            int curPage = 0;
            if (PageService != null)
            {
                PageService.setRowSize(rowSize);
                curPage = PageService.CursorPos / numItems;
            }

            Rectangle parentDisplayRectangle = parent.DisplayRectangle;
            Point nextControlLocation = parentDisplayRectangle.Location;

            int spos = curPage * numItems, epos = spos + numItems;
            for (int i = 0; i < parent.Controls.Count; i++)
            {
                Control c = parent.Controls[i];
                if (i < spos || i >= epos)
                {
                    c.Hide();
                    continue;
                }
                if (c.Visible == false)
                    c.Show();
                int pos = i - spos;
                c.Location = Point.Add(nextControlLocation, new Size(c.Margin.Left, c.Margin.Top));
                c.Size = itemSize;
                nextControlLocation.Y += itemHeight;
                if ((pos + 1) % rowSize == 0)
                {
                    nextControlLocation.X += itemSize.Width;
                    nextControlLocation.Y = parentDisplayRectangle.Location.Y;
                }
            }

            return parent.AutoSize;
        }
    }

    public class SimplePagedLayoutInterface : IPagedLayoutInterface
    {
        public int CursorPos { get { return 0; } }
        public void setRowSize(int rowSize) { }
    }
}