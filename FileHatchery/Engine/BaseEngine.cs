using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Collections;
using ShellApi;

namespace FileHatchery.Engine
{
    /// <summary>
    /// 자동완성 기능 구현 인터페이스
    /// </summary>
    public interface IAutoCompletion
    {
        List<string> Commands { get; }
    }

    /// <summary>
    /// 기본 엔진.
    /// </summary>
    public class TestEngineQuery : ComponentContainer, IAutoCompletion, IDisposable
    {
        public static TestEngineQuery s_inst;
        public event EventHandler<Notification.NotifyArgs> UINotify;
        Dictionary<string, int> internal_commands { get; set; }
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
            internal_commands = new Dictionary<string, int>();
            // for testing
            internal_commands.Add("asdfnews", 1);
            internal_commands.Add("google", 1);
            internal_commands.Add("gosick", 1);
            internal_commands.Add("gooooogle", 1);
            internal_commands.Add("microsoft", 1);

            setComponent(typeof(IIconProducer), new NullProducer());
            setComponent(typeof(Config.IConfig), new Config.PortableConfig());
            setComponent(typeof(IBrowser), browser);
        }

        public void Dispose()
        {
        }

        #region IAutoCompletion 멤버

        public List<string> Commands
        {
            get
            {
                List<string> ret = new List<string>();
                foreach (KeyValuePair<string, int> kv in internal_commands)
                {
                    ret.Add(kv.Key);
                }
                return ret;
            }
        }

        #endregion

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
            if (cmd == "edit this")
            {
                FileInfo file = new FileInfo(vimpath);
                IBrowserItem item = getComponent<IBrowser>().Cursor;
                Win32.SHExecute(file, "\"" + item.FullPath + "\"", false);
                return;
            }
            if (cmd == "sudo edit this")
            {
                FileInfo file = new FileInfo(vimpath);
                IBrowserItem item = getComponent<IBrowser>().Cursor;
                Win32.SHExecute(file, "\"" + item.FullPath + "\"", true);
                return;
            }
            if (cmd == "explore here")
            {
                FileInfo file = new FileInfo(explorerpath);
                DirectoryInfo dir = getComponent<IBrowser>().CurrentDir;

                Win32.SHExecute(file, dir.FullName, false);
                return;
            }
            /*            if (cmd == "save 1 here")
                        {
                            object[] args = new object[3];
                            args[0] = "save";
                            args[1] = 1;
                            args[2] = Browser.CurrentDir;
                            DynamicConfig.execute(args);
                            return;
                        }
                        if (cmd == "load 1")
                        {
                            object[] args = new object[2];
                            args[0] = "load";
                            args[1] = 1;
                            DynamicConfig.execute(args);
                            return;
                        }*/
            if (cmd == "select this")
            {
                IBrowser browser = getComponent<IBrowser>();
                browser.MarkItem(browser.Cursor);
                return;
            }
            if (cmd == "select all")
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
            if (cmd == "alt-u")
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
            if (cmd == "refresh")
            {
                getComponent<IBrowser>().Refresh();
                return;
            }
            if (cmd == "delete silent")
            {
                DeleteFiles(true);
                return;
            }
            if (cmd == "delete")
            {
                DeleteFiles(false);
                return;
            }
            if (cmd == "mount this")
            {
                FileInfo file = new FileInfo(daemonpath);
                IBrowserItem item = getComponent<IBrowser>().Cursor;
                Win32.SHExecute(file, "-mount 0," + item.FullPath, false);
                return;
            }
            if (cmd == "!cmd" || cmd == "!" || cmd == "cmd")
            {
                Win32.SHExecute("cmd", "", false);
                return;
            }
            if (cmd == "paste")
            {
                Paste();
                return;
            }
            if (cmd == "cut selected")
            {
                SetDropFileList(true);
                return;
            }
            if (cmd == "copy selected")
            {
                SetDropFileList(false);
                return;
            }
            if (cmd == "goroot")
            {
                getComponent<IBrowser>().CurrentDir = getComponent<IBrowser>().CurrentDir.Root;
                return;
            }
            if (cmd == "test")
            {
                var temp = UINotify;
                if (temp != null)
                    temp(this, new Notification.NotifyArgs("Asdfnews"));
                return;
            }
            if (cmd == "gc")
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                return;
            }
            if (cmd == "set") // FIXME: temporary
            {
                getComponent<Config.IConfig>()["Font"] = "FixedSys, 12pt";
                return;
            }
            if (cmd.StartsWith("save"))
            {
                string vv = cmd.Substring(4);
                getComponent<Config.IConfig>()["Bookmark" + vv] = getComponent<IBrowser>().CurrentDir.FullName;
                return;
            }
            if (cmd.StartsWith("load"))
            {
                string vv = cmd.Substring(4);
                getComponent<IBrowser>().CurrentDir = new DirectoryInfo(getComponent<Config.IConfig>()["Bookmark" + vv]);
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
                HandleException(E);
            }
        }

        public void HandleException(Exception E)
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

            if(silent)
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
