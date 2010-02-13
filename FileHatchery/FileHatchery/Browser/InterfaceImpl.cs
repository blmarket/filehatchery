using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using ShellApi;

namespace FileHatchery
{
    class AdminExecutor : IBrowserItemVisitor
    {
        #region IBrowserItemVisitor 멤버

        public void visit(FileItem file)
        {
            Win32.SHExecute(file.FullPath, "", true);
        }

        public void visit(DirectoryItem directory)
        {
            Program.engine.Browser.CurrentDir = directory.DirInfo;
        }

        #endregion
    }

    class NormalExecutor : IBrowserItemVisitor
    {
        #region IBrowserItemVisitor 멤버

        public void visit(FileItem file)
        {
            Win32.SHExecute(file.FullPath, "", false);
        }

        public void visit(DirectoryItem directory)
        {
            Program.engine.Browser.CurrentDir = directory.DirInfo;
        }

        #endregion
    }

    public partial class DirectoryBrowser // 상속 정보는 DirectoryBrowser.cs 참조 : IBrowser, IKeyHandler, IPagedLayoutInterface
    {
        #region IKeyHandler 멤버

        bool IKeyHandler.HandleKey(Keys kdata)
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
                        Cursor.accept(new AdminExecutor());
                    }
                    return true;
                case Keys.Enter:                    
                    if (Cursor != null)
                    {
                        Cursor.accept(new NormalExecutor());
                    }
                    return true;
                case Keys.Tab:
                    return true;
                case Keys.Escape:
                    m_Searcher.Clear();
                    return true;
                default:
                    if ((kdata & Keys.Alt) == Keys.Alt || (kdata & Keys.Control) == Keys.Control || (kdata & Keys.Shift) == Keys.Shift)
                        break;
                    m_Searcher.AddChar(this, (char)kdata);
                    return true;
            }
            return false;
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