﻿using FileHatchery;
using ShellApi;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace FileHatchery.Engine
{
    /// <summary>
    /// 기본 엔진.
    /// </summary>
    public class TestEngineQuery : ComponentContainer, IDisposable, IExceptionHandler
    {
        public static TestEngineQuery s_inst;
        public event EventHandler<Notification.NotifyArgs> UINotify;
        public System.Drawing.Font Font { get; set; }
        IntPtr m_windowhandle = IntPtr.Zero;

        /// <summary>
        /// 엔진 생성자.
        /// </summary>
        /// <param name="browser">UI에 해당하는 Browser</param>
        /// <param name="windowHandle">부모 윈도우 Form의 Handle. 없으면 IntPtr.Zero를 넘겨달라</param>
        public TestEngineQuery(IBrowser browser, IntPtr windowHandle)
        {
            s_inst = this;
            m_windowhandle = windowHandle;

            setComponent(typeof(IIconProducer), new NullProducer());
            setComponent(typeof(Components.Config.IConfig), new Components.Config.PortableConfig());
            setComponent(typeof(IBrowser), browser);
            setComponent(typeof(IExceptionHandler), this);
            setComponent(typeof(Components.IAutoCompletion), new Components.BasicAutoCompletion());
        }

        public void Dispose()
        {
        }

        public bool getConfiguration(string name)
        {
            if (name.Equals("useConsoleTextBox")) return false;
            return false;
        }

        private void _RunCommand(string cmd)
        {
            string vimpath = @"C:\Program Files (x86)\Vim\vim73\gvim.exe";
            string explorerpath = @"C:\Windows\explorer.exe";
            string daemonpath = @"C:\Program Files (x86)\DAEMON Tools Lite\DTLite.exe";

            switch (cmd)
            {
                case "edit this":
                    {
                        FileInfo file = new FileInfo(vimpath);
                        IBrowserItem item = getComponent<IBrowser>().Cursor;
                        Win32.SHExecute(file, "\"" + item.FullPath + "\"", false);
                        return;
                    }
                case "sudo edit this":
                    {
                        FileInfo file = new FileInfo(vimpath);
                        IBrowserItem item = getComponent<IBrowser>().Cursor;
                        Win32.SHExecute(file, "\"" + item.FullPath + "\"", true);
                        return;
                    }
                case "explore here":
                    {
                        FileInfo file = new FileInfo(explorerpath);
                        DirectoryInfo dir = getComponent<IBrowser>().CurrentDir;

                        Win32.SHExecute(file, dir.FullName, false);
                        return;
                    }
                case "select this":
                    {
                        IBrowser browser = getComponent<IBrowser>();
                        browser.MarkItem(browser.Cursor);
                        return;
                    }
                case "select all":
                    {
                        IBrowser browser = getComponent<IBrowser>();
                        List<IBrowserItem> items = browser.Items;
                        Selection sel = browser.Selection;
                        foreach (IBrowserItem item in items)
                        {
                            sel.addItem(item);
                        }
                        return;
                    }
                case "alt-u":
                    {
                        IBrowser browser = getComponent<IBrowser>();
                        if (browser.Selection.Count > 0)
                        {
                            browser.Selection.clear();
                        }
                        else
                        {
                            RunCommand("select all");
                        }
                        return;
                    }
                case "refresh":
                    {
                        getComponent<IBrowser>().Refresh();
                        return;
                    }
                case "delete silent":
                    {
                        DeleteFiles(true);
                        return;
                    }
                case "delete":
                    {
                        DeleteFiles(false);
                        return;
                    }
                case "mount this":
                    {
                        FileInfo file = new FileInfo(daemonpath);
                        IBrowserItem item = getComponent<IBrowser>().Cursor;
                        Win32.SHExecute(file, "-mount 0," + item.FullPath, false);
                        return;
                    }
                case "!cmd":
                case "!":
                case "cmd":
                    {
                        Win32.SHExecute("cmd", "", false);
                        return;
                    }
                case "paste":
                    {
                        Paste();
                        return;
                    }
                case "cut selected":
                    {
                        SetDropFileList(true);
                        return;
                    }
                case "copy selected":
                    {
                        SetDropFileList(false);
                        return;
                    }
                case "goroot":
                    {
                        getComponent<IBrowser>().CurrentDir = getComponent<IBrowser>().CurrentDir.Root;
                        return;
                    }
                case "test":
                    {
                        var temp = UINotify;
                        if (temp != null)
                            temp(this, new Notification.NotifyArgs("Asdfnews"));
                        return;
                    }
                case "gc":
                    {
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                        GC.Collect();
                        return;
                    }
                case "set": // FIXME: temporary
                    {
                        getComponent<Components.Config.IConfig>()["Font"] = "FixedSys, 12pt";
                        return;
                    }
                default:
                    {
                        if (cmd.StartsWith("notify ", true, null))
                        {
                            string msg = cmd.Substring(7);
                            throw new Exception(msg);
                        }
                        if (cmd.StartsWith("mkdir ", true, null))
                        {
                            string dirName = cmd.Substring(6);
                            Directory.CreateDirectory(getComponent<IBrowser>().CurrentDir.FullName + "\\" + dirName);
                            getComponent<IBrowser>().Refresh();
                            return;
                        }
                        if (cmd.StartsWith("cd ", true, null))
                        {
                            getComponent<IBrowser>().CurrentDir = new DirectoryInfo(cmd.Substring(3));
                            return;
                        }
                        if (cmd.StartsWith("open ", true, null))
                        {
                            getComponent<IBrowser>().CurrentDir = new DirectoryInfo(cmd.Substring(5));
                            return;
                        }
                        if (cmd.StartsWith("select ", true, null))
                        {
                            IBrowser browser = getComponent<IBrowser>();
                            string tmp = cmd.Substring(7);
                            IBrowserItem selItem;
                            selItem = browser.Items.Find(delegate(IBrowserItem item)
                            {
                                return (item.showName == tmp);
                            });
                            if (selItem == null)
                            {
                                throw new FileNotFoundException();
                            }
                            browser.Selection.addItem(selItem);
                            return;
                        }
                        if (cmd.StartsWith("rename ", true, null))
                        {
                            string newFilename = cmd.Substring(7);
                            IBrowser browser = getComponent<IBrowser>();
                            browser.Cursor.accept(new RenameVisitor(this, newFilename));
                            browser.Refresh();
                            return;
                        }
                        if (cmd.StartsWith("save"))
                        {
                            string vv = cmd.Substring(4);
                            getComponent<Components.Config.IConfig>()["Bookmark" + vv] = getComponent<IBrowser>().CurrentDir.FullName;
                            return;
                        }
                        if (cmd.StartsWith("load"))
                        {
                            string vv = cmd.Substring(4);
                            getComponent<IBrowser>().CurrentDir = new DirectoryInfo(getComponent<Components.Config.IConfig>()["Bookmark" + vv]);
                            return;
                        }
                        if (cmd.StartsWith("new "))
                        {
                            string vv = cmd.Substring(4);
                            File.Create(getComponent<IBrowser>().CurrentDir.FullName + "\\" + vv).Close();
                            getComponent<IBrowser>().Refresh();
                            return;
                        }
                        throw new NotImplementedException("Operation " + cmd + " is not implemented");
                    }
            }
        }

        /// <summary>
        /// Run Console Command
        /// </summary>
        /// <param name="cmd">Command wanna be run</param>
        /// <remarks>Command should be valid</remarks>
        /// <exception cref="NotImplementedException">Command is not recognized nor implemented</exception>
        public void RunCommand(string cmd)
        {
            try
            {
                _RunCommand(cmd);
            }
            catch (Exception E)
            {
                handleException(E);
            }
        }

        public void handleException(Exception E)
        {
            var tmp = UINotify;
            if (tmp != null)
                tmp(this, new Notification.NotifyArgs(E.Message));
            else
                throw E;
        }

        private void DeleteFiles(bool silent)
        {
            System.Collections.Specialized.StringCollection files = new System.Collections.Specialized.StringCollection();
            IEnumerable<IBrowserItem> itemset = getComponent<IBrowser>().CurSelItems;

            foreach (IBrowserItem item in itemset)
            {
                files.Add(item.FullPath);
            }

            ShellLib.ShellFileOperation fo = new ShellLib.ShellFileOperation();

            string[] filearray = new string[files.Count];
            files.CopyTo(filearray, 0);
            fo.Operation = ShellLib.ShellFileOperation.FileOperations.FO_DELETE;
            fo.OwnerWindow = m_windowhandle;
            fo.SourceFiles = filearray;

            fo.OperationFlags = ShellLib.ShellFileOperation.ShellFileOperationFlags.FOF_NO_CONNECTED_ELEMENTS
                | ShellLib.ShellFileOperation.ShellFileOperationFlags.FOF_WANTNUKEWARNING;

            if (silent)
                fo.OperationFlags = fo.OperationFlags | ShellLib.ShellFileOperation.ShellFileOperationFlags.FOF_NOCONFIRMATION;

            bool retVal = fo.DoOperation();

            getComponent<IBrowser>().Refresh();

            if (retVal == false)
            {
                throw new Exception("Shell File Operation Failed");
            }
        }

        private void SetDropFileList(bool cut)
        {
            try
            {
                System.Collections.Specialized.StringCollection files = new System.Collections.Specialized.StringCollection();

                IEnumerable<IBrowserItem> itemset = getComponent<IBrowser>().CurSelItems;

                foreach (IBrowserItem item in itemset)
                {
                    files.Add(item.FullPath);
                }

                DataObject data = new DataObject();

                data.SetFileDropList(files);

                MemoryStream stream = new MemoryStream();
                BinaryWriter writer = new BinaryWriter(stream);
                if (cut)
                {
                    writer.Write((int)DragDropEffects.Move);
                }
                else
                {
                    writer.Write((int)DragDropEffects.Copy);
                }
                data.SetData("Preferred DropEffect", stream);

                Clipboard.Clear();
                Clipboard.SetDataObject(data);
            }
            catch (Exception EE)
            {
                MessageBox.Show(EE.Message);
            }
        }

        public void ShowContextMenu(Point cursorPoint)
        {
            AndreasJohansson.Win32.Shell.ShellContextMenu scm = new AndreasJohansson.Win32.Shell.ShellContextMenu();
            List<FileInfo> files = new List<FileInfo>();

            foreach (IBrowserItem item in getComponent<IBrowser>().CurSelItems)
            {
                FileInfo file = new FileInfo(item.FullPath);
                files.Add(file);
            }

            scm.ShowContextMenu(m_windowhandle, files.ToArray(), cursorPoint);
        }

        void Paste()
        {
            IDataObject obj = Clipboard.GetDataObject();

            int dropeffect = 0;
            MemoryStream stream = (System.IO.MemoryStream)obj.GetData("Preferred DropEffect");
            if (stream != null)
            {
                BinaryReader reader = new BinaryReader(stream);
                dropeffect = reader.ReadInt32();
            }

            System.Collections.Specialized.StringCollection files = Clipboard.GetFileDropList();
            ShellLib.ShellFileOperation fo = new ShellLib.ShellFileOperation();

            string[] filearray = new string[files.Count];
            files.CopyTo(filearray, 0);

            if ((DragDropEffects.Move & (DragDropEffects)dropeffect) == DragDropEffects.Move)
            {
                fo.Operation = ShellLib.ShellFileOperation.FileOperations.FO_MOVE;
            }
            else
            {
                fo.Operation = ShellLib.ShellFileOperation.FileOperations.FO_COPY;
            }

            fo.OwnerWindow = m_windowhandle;
            fo.SourceFiles = filearray;
            /*            if (filearray.Length == 0)
                        {
                            string[] destarray = new string[files.Count];
                            for (int i = 0; i < files.Count; i++)
                            {
                                string filename = files[i].Substring(files[i].LastIndexOf('\\'));
                                destarray[i] = Browser.CurrentDir.FullName + filename;
                            }
                            fo.DestFiles = destarray;
                        }
                        else*/
            {
                fo.OperationFlags = ShellLib.ShellFileOperation.ShellFileOperationFlags.FOF_ALLOWUNDO
                    | ShellLib.ShellFileOperation.ShellFileOperationFlags.FOF_RENAMEONCOLLISION
                    | ShellLib.ShellFileOperation.ShellFileOperationFlags.FOF_NO_CONNECTED_ELEMENTS
                    | ShellLib.ShellFileOperation.ShellFileOperationFlags.FOF_WANTNUKEWARNING;

                fo.DestFiles = new string[] { getComponent<IBrowser>().CurrentDir.FullName };
            }

            try
            {
                bool retVal = fo.DoOperation();
                if (retVal == false)
                {
                    MessageBox.Show("Error Occurs");
                }
            }
            catch (Exception EE)
            {
                MessageBox.Show(EE.Message);
            }
        }

        public bool RunShortcut(Keys e)
        {
            if (e == (Keys.Control | Keys.D1))
            {
                RunCommand("save 1 here");
                return true;
            }
            if (e == (Keys.Alt | Keys.D1))
            {
                RunCommand("load 1");
                return true;
            }

            return true;
        }
    }
}
