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
            try
            {
                Win32.SHExecute(file.FullPath, "", true);
            }
            catch (Exception E)
            {
                Program.engine.HandleException(E);
            }
        }

        /// <summary>
        /// Directory를 이동한다.
        /// </summary>
        /// <param name="directory">이동하고자 하는 디렉토리</param>
        public void visit(DirectoryItem directory)
        {
            Program.engine.getComponent<IBrowser>().CurrentDir = directory.DirInfo;
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
            try
            {
                Win32.SHExecute(file.FullPath, "", false);
            }
            catch (Exception E)
            {
                Program.engine.HandleException(E);
            }
        }

        /// <summary>
        /// Directory로 이동한다.
        /// </summary>
        /// <param name="directory">이동하고자 하는 Directory</param>
        public void visit(DirectoryItem directory)
        {
            Program.engine.getComponent<IBrowser>().CurrentDir = directory.DirInfo;
        }

        #endregion
    }
}