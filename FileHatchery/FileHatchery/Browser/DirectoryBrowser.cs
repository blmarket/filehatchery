﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;

namespace FileHatchery
{
    public class DirectoryBrowser : IBrowser, IKeyHandler
    {
        DirectoryInfo m_CurrentDir;
        List<IBrowserItem> m_ItemList;
        Selection m_curSelection;
        int m_CursorIndex = -1;
        int m_RowSize;
        Algorithm.StupidSearcher m_Searcher;
        bool m_ShowHiddenFiles = false;
        public event EventHandler DirectoryChanged;
        public event EventHandler CursorChanged;
        public event EventHandler DirectoryChanging;

        public bool ShowHiddenFiles 
        {
            get
            {
                return m_ShowHiddenFiles;
            }
            set
            {
                if (m_ShowHiddenFiles != value)
                {
                    m_ShowHiddenFiles = value;
                    Refresh();
                }
            }
        }

        public void MarkItem(IBrowserItem item)
        {
            m_curSelection.MarkItem(item);
        }

        public Selection Selection
        {
            get
            {
                return m_curSelection;
            }
        }

        public IEnumerable<IBrowserItem> CurSelItems
        {
            get
            {
                if (m_curSelection.Count == 0)
                {
                    List<IBrowserItem> list = new List<IBrowserItem>();
                    list.Add(Cursor);
                    return list;
                }
                else
                {
                    return m_curSelection.Context;
                }
            }
        }

        public DirectoryBrowser()
        {
            try
            {
                m_CurrentDir = null;
                m_curSelection = new Selection();
                m_Searcher = new FileHatchery.Algorithm.StupidSearcher();
                DirectoryChanged += delegate(object obj, EventArgs e)
                {
                    m_curSelection.clear();
                };
            }
            catch (Exception EE)
            {
                MessageBox.Show(EE.Message);
            }
        }

        public IBrowserItem Cursor
        {
            get
            {
                if(m_CursorIndex < 0 || m_ItemList.Count <= m_CursorIndex) return null;
                return m_ItemList[CursorIndex];
            }
            set
            {
                if (value == null)
                {
                    CursorIndex = -1;
                    return;
                }
                for (int i = 0; i < m_ItemList.Count; i++)
                {
                    if (m_ItemList[i] == value)
                    {
                        CursorIndex = i;
                        return;
                    }
                }
                CursorIndex = -1;
            }
        }

        /// <summary>
        /// 디렉토리 전체의 내용을 다시 읽는다.
        /// </summary>
        public void Refresh()
        {
            {
                var temp = DirectoryChanging;
                if (temp != null)
                    temp(this, EventArgs.Empty);
            }
            string fullPath = Cursor.FullPath;
            ReadDirectoryContents();
            {
                EventHandler temp = DirectoryChanged;
                if (temp != null)
                    temp(this, EventArgs.Empty);
            }
            SelectItem(fullPath);
        }

        /// <summary>
        /// 특정 파일을 커서로 지정한다.
        /// </summary>
        /// <param name="fullpath">특정 파일의 경로</param>
        public void SelectItem(string fullpath)
        {
            if (Cursor.FullPath == fullpath) return;

            int curpos = m_ItemList.FindIndex(
                delegate(IBrowserItem it)
                {
                    return it.FullPath == fullpath;
                });
            CursorIndex = curpos;
        }

        /// <summary>
        /// 특정 IBrowserItem 객체를 커서로 지정한다.
        /// </summary>
        /// <param name="item">커서로 지정하고자 하는 객체</param>
        public void SelectItem(IBrowserItem item)
        {
            if (Cursor == item) return;
            try
            {
                int curpos = m_ItemList.FindIndex(delegate(IBrowserItem it)
                {
                    return it == item;
                });
                CursorIndex = curpos;
            }                
            catch (System.ArgumentNullException EE)
            {
                MessageBox.Show(EE.Message);
                // ignore
            }
        }

        public int CursorIndex
        {
            get
            {
                return m_CursorIndex;
            }
            set
            {
                IBrowserItem prevItem = Cursor, newItem = null;

                m_CursorIndex = value;
                if (m_CursorIndex < 0) m_CursorIndex = 0;
                if (m_CursorIndex >= m_ItemList.Count) m_CursorIndex = m_ItemList.Count - 1;
                if (m_CursorIndex >= 0 && m_CursorIndex < m_ItemList.Count)
                    newItem = m_ItemList[m_CursorIndex];

                if (prevItem == newItem && (prevItem.State & BrowserItemState.Selected) == BrowserItemState.Selected)
                {
                    return;
                }

                if(prevItem != null)
                    prevItem.State = prevItem.State & (~BrowserItemState.Selected);

                if(newItem != null)
                    newItem.State = newItem.State | BrowserItemState.Selected;

                EventHandler temp = CursorChanged;
                if (temp != null)
                    temp(this, EventArgs.Empty);
            }
        }

        private void SetNewDirectory(DirectoryInfo dir)
        {
            DirectoryInfo prev = m_CurrentDir;
            m_CurrentDir = dir;
            try
            {
                var tmp1 = DirectoryChanging;
                if(tmp1 != null)
                    tmp1(this, EventArgs.Empty);
                ReadDirectoryContents();
                var tmp2 = DirectoryChanged;
                if (tmp2 != null)
                    tmp2(this, EventArgs.Empty);
                Directory.SetCurrentDirectory(dir.FullName);
            }
            catch (Exception EE)
            {
                MessageBox.Show(EE.Message);
                m_CurrentDir = prev;
                ReadDirectoryContents();
            }
        }

        public DirectoryInfo CurrentDir 
        {
            get
            {
                return m_CurrentDir;
            }
            set 
            {
                // FIXME: check really changed
                if (m_CurrentDir != null && value.FullName == m_CurrentDir.FullName) return;
                SetNewDirectory(value);
            } 
        }

        public List<IBrowserItem> Items
        {
            get
            {
                return m_ItemList;
            }
        }

        private void ReadDirectoryContents()
        {
            if (m_CurrentDir == null) return;
            m_ItemList = new List<IBrowserItem>();
            if (m_CurrentDir.Parent != null)
            {
                IBrowserItem item = new DirectoryItem(m_CurrentDir.Parent, "..", this);
                item.State = item.State | BrowserItemState.UnMarkable;
                m_ItemList.Add(item);
            }
            foreach (DirectoryInfo dir in m_CurrentDir.GetDirectories())
            {
                if (ShowHiddenFiles == false && (dir.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden) continue;
                m_ItemList.Add(new DirectoryItem(dir, dir.Name, this));
            }
            foreach (FileInfo file in m_CurrentDir.GetFiles())
            {
                if (ShowHiddenFiles == false && (file.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden) continue;
                m_ItemList.Add(new FileItem(file, this));
            }

            //FIXME: Directory must have 1 or more contents.
            if (m_ItemList.Count == 0)
            {
                IBrowserItem item = new DirectoryItem(m_CurrentDir, ".", this);
                item.State = item.State | BrowserItemState.UnMarkable;
                m_ItemList.Add(item);
            }
            CursorIndex = 0;
        }

        public void Open(string npath)
        {
            try
            {
                CurrentDir = new DirectoryInfo(npath);
            }
            catch (Exception EE)
            {
                MessageBox.Show(EE.Message);
            }
        }

        #region IKeyHandler 멤버
        bool IKeyHandler.handleSpecialKey(Keys kdata)
        {
            switch (kdata)
            {
                case Keys.Space:
                    Program.engine.RunCommand("select this");
                    CursorIndex = CursorIndex + 1;
                    return true;
                case Keys.Home:
                    CursorIndex = 0;
                    return true;
                case Keys.End:
                    CursorIndex = m_ItemList.Count - 1;
                    return true;
                case Keys.Down:
                    CursorIndex = CursorIndex + 1;
                    return true;
                case Keys.Right:
                    if (CursorIndex + m_RowSize >= m_ItemList.Count)
                        CursorIndex = CursorIndex + 1;
                    else
                        CursorIndex = CursorIndex + m_RowSize;
                    return true;
                case Keys.Up:
                    CursorIndex = CursorIndex - 1;
                    return true;
                case Keys.Left:
                    if (CursorIndex - m_RowSize < 0)
                        CursorIndex = CursorIndex - 1;
                    else
                        CursorIndex = CursorIndex - m_RowSize;
                    return true;
                case Keys.Shift | Keys.Enter:
                    {
                        Cursor.accept(new AdminExecutor(Program.engine));
                    }
                    return true;
                case Keys.Enter:
                    if (Cursor != null)
                    {
                        Cursor.accept(new NormalExecutor(Program.engine));
                    }
                    return true;
                case Keys.Tab:
                    return true;
                case Keys.Escape:
                    m_Searcher.Clear();
                    return true;
            }
            return false;
        }

        void IKeyHandler.handleChar(char key)
        {
            m_Searcher.AddChar(this, key);
        }

        bool IKeyHandler.NeedsFocus
        {
            get { return true; }
        }

        void IKeyHandler.setFocus()
        {
            // 어차피 HandleKey에서 처리하니 따로 Focus를 줄 필요가 없다.
        }

        #endregion

        #region IPagedLayoutInterface Members

        int IPagedLayoutInterface.CursorPos
        {
            get { return CursorIndex; }
        }

        void IPagedLayoutInterface.setRowSize(int rowSize)
        {
            m_RowSize = rowSize;
        }

        #endregion


    }
}
