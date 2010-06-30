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
    /// <summary>
    /// 관리자 권한으로 Item을 실행하는 Visitor 패턴 구현
    /// </summary>
    class AdminExecutor : IBrowserItemVisitor
    {
        #region IBrowserItemVisitor 멤버

        /// <summary>
        /// 관리자 권한으로 file을 실행한다.
        /// </summary>
        /// <param name="file">관리자 권한으로 실행하고자 하는 File</param>
        public void visit(FileItem file)
        {
            Win32.SHExecute(file.FullPath, "", true);
        }

        /// <summary>
        /// Directory를 이동한다.
        /// </summary>
        /// <param name="directory">이동하고자 하는 디렉토리</param>
        public void visit(DirectoryItem directory)
        {
            Program.engine.Browser.CurrentDir = directory.DirInfo;
        }

        #endregion
    }

    /// <summary>
    /// Item을 실행하는 Visitor
    /// </summary>
    class NormalExecutor : IBrowserItemVisitor
    {
        #region IBrowserItemVisitor 멤버

        /// <summary>
        /// File을 실행한다.
        /// </summary>
        /// <param name="file">실행하고자 하는 File</param>
        public void visit(FileItem file)
        {
            Win32.SHExecute(file.FullPath, "", false);
        }

        /// <summary>
        /// Directory로 이동한다.
        /// </summary>
        /// <param name="directory">이동하고자 하는 Directory</param>
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
                        AdminExecutor adminexec = new AdminExecutor();
                        if (Cursor is FileItem)
                            adminexec.visit(Cursor as FileItem);
                        else
                            adminexec.visit(Cursor as DirectoryItem);
                    }
                    return true;
                case Keys.Enter:                    
                    if (Cursor != null)
                    {
                        NormalExecutor adminexec = new NormalExecutor();
                        if (Cursor is FileItem)
                            adminexec.visit(Cursor as FileItem);
                        else
                            adminexec.visit(Cursor as DirectoryItem);
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