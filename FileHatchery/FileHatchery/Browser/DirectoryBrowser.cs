using System;
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
    /// <summary>
    /// FileHatchery 기본 Browser의 인터페이스 정의입니다.
    /// </summary>
    public interface IBrowser : IPagedLayoutInterface
    {
        /// <summary>
        /// 현재 Cursor 위치를 설정하거나 리턴합니다.
        /// </summary>
        IBrowserItem Cursor { get; set; }

        /// <summary>
        /// 현재 위치를 설정하거나 리턴합니다.
        /// </summary>
        DirectoryInfo CurrentDir { get; set; }

        /// <summary>
        /// 현재 위치의 모든 객체들을 리턴한다.
        /// </summary>
        List<IBrowserItem> Items { get; }

        /// <summary>
        /// 특정 IBrowserItem 객체를 커서로 지정한다.
        /// </summary>
        /// <param name="item">커서로 지정하고자 하는 객체</param>
        void SelectItem(IBrowserItem item);

        /// <summary>
        /// 탐색하고 있는 디렉토리가 변경되었을 때 발생하는 이벤트
        /// </summary>
        event EventHandler onChangeDirectory;
        /// <summary>
        /// 커서 위치가 변경되었을 때 발생하는 이벤트
        /// </summary>
        event EventHandler onChangeCursor;

        /// <summary>
        /// 특정 객체의 Mark 상태를 변경합니다.
        /// </summary>
        /// <param name="item">상태를 변경하고자 하는 객체 Item</param>
        void MarkItem(IBrowserItem item);
        /// <summary>
        /// 현재 선택되어 있는 아이템들의 나열자를 반환합니다.
        /// </summary>
        IEnumerable<IBrowserItem> CurSelItems { get; }

        /// <summary>
        /// 현재 '선택'을 반환합니다.
        /// </summary>
        Selection Selection { get; }

        /// <summary>
        /// 디렉토리 전체의 내용을 다시 읽는다.
        /// </summary>
        void Refresh();
    };

    public partial class DirectoryBrowser : IBrowser, IKeyHandler
    {
        DirectoryInfo m_CurrentDir;
        List<IBrowserItem> m_ItemList;
        Selection m_curSelection;
        int m_CursorIndex = -1;
        int m_RowSize;
        Algorithm.StupidSearcher m_Searcher;
        bool m_ShowHiddenFiles = false;
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

        /// <summary>
        /// 탐색하고 있는 디렉토리가 변경되었을 때 발생하는 이벤트
        /// </summary>
        public event EventHandler onChangeDirectory;
        /// <summary>
        /// 커서 위치가 변경되었을 때 발생하는 이벤트
        /// </summary>
        public event EventHandler onChangeCursor;

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
                onChangeDirectory += delegate(object obj, EventArgs e)
                {
                    m_curSelection.clear();
                };
                onChangeDirectory += delegate(object obj, EventArgs e)
                {
                    List<string> list = new List<string>();
                    foreach(IBrowserItem item in m_ItemList)
                    {
                        list.Add(item.showName);
                    }
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
            string fullPath = Cursor.FullPath;
            ReadDirectoryContents();
            onChangeDirectory(this, EventArgs.Empty);
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
                IBrowserItem prevItem = null, newItem = null;

                if (m_CursorIndex >= 0 && m_CursorIndex < m_ItemList.Count)
                    prevItem = m_ItemList[m_CursorIndex];
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
                onChangeCursor(this, EventArgs.Empty);
            }
        }

        private void SetNewDirectory(DirectoryInfo dir)
        {
            DirectoryInfo prev = m_CurrentDir;
            m_CurrentDir = (DirectoryInfo)dir;
            try
            {
                ReadDirectoryContents();
                onChangeDirectory(this, EventArgs.Empty);
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
    };
}
