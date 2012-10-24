﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using ShellApi;
using FileHatchery.Engine;

namespace FileHatchery
{
    /// <summary>
    /// 관리자 권한으로 Item을 실행하는 Visitor 패턴 구현
    /// </summary>
    public class AdminExecutor : IBrowserItemVisitor
    {
        ComponentContainer m_engine;

        public AdminExecutor(ComponentContainer engine)
        {
            m_engine = engine;
        }

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
            catch (Exception e)
            {
                m_engine.getComponent<IExceptionHandler>().handleException(e);
            }
        }

        /// <summary>
        /// Directory를 이동한다.
        /// </summary>
        /// <param name="directory">이동하고자 하는 디렉토리</param>
        public void visit(DirectoryItem directory)
        {
            m_engine.getComponent<IBrowser>().CurrentDir = directory.DirInfo;
        }

        #endregion
    }

    /// <summary>
    /// Item을 실행하는 Visitor
    /// </summary>
    public class NormalExecutor : IBrowserItemVisitor
    {
        ComponentContainer m_engine;

        public NormalExecutor(ComponentContainer engine)
        {
            m_engine = engine;
        }

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
            catch (Exception e)
            {
                m_engine.getComponent<IExceptionHandler>().handleException(e);
            }
        }

        /// <summary>
        /// Directory로 이동한다.
        /// </summary>
        /// <param name="directory">이동하고자 하는 Directory</param>
        public void visit(DirectoryItem directory)
        {
            m_engine.getComponent<IBrowser>().CurrentDir = directory.DirInfo;
        }

        #endregion
    }

    public class RenameVisitor : IBrowserItemVisitor
    {
        public void visit(FileItem file)
        {
            throw new NotImplementedException();
        }

        public void visit(DirectoryItem directory)
        {
            throw new NotImplementedException();
        }
    }
}