using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using ShellApi;

namespace FileHatchery
{
    public interface IBrowserItemVisitor
    {
        void visit(FileItem file);
        void visit(DirectoryItem directory);
    }

    /// <summary>
    /// IBrowser 클래스에서 Browse할 수 있는 Item의 interface를 나타냅니다.
    /// </summary>
    public interface IBrowserItem
    {
        /// <summary>
        /// 이 Item의 표현명을 리턴합니다.
        /// </summary>
        string showName { get; }

        /// <summary>
        /// 이 파일/디렉토리의 경로를 리턴합니다.
        /// </summary>
        string FullPath { get; }

        /// <summary>
        /// 이 파일/디렉토리의 아이콘을 리턴합니다.
        /// </summary>
        System.Drawing.Icon Icon { get; set; }

        /// <summary>
        /// 이 Item의 현재 상태를 설정하거나 반환합니다.
        /// </summary>
        BrowserItemState State { get; set; }

        /// <summary>
        /// Item의 State가 변경되었을 때 발생하는 event입니다.
        /// </summary>
        event EventHandler onChanged;

        /// <summary>
        /// Visitor 패턴 구현. 임의의 Visitor별 구현을 실행합니다.
        /// </summary>
        /// <param name="visitor">사용하고 싶은 Visitor</param>
        void accept(IBrowserItemVisitor visitor);
    };

    public interface IIconProducer
    {
        void EnqueueTask(IBrowserItem item);
    }

    public class IconProducer
    {
        // FIXME: 이래놓으면 해제를 안하잖아 -_-; 나중에 고치자
        public static IIconProducer s_inst = new SimpleProducer(); // new ProducerConsumerQueue();
    }

    class SimpleProducer : IIconProducer
    {
        public void EnqueueTask(IBrowserItem item)
        {
            Icon icon = Win32.getIcon(item.FullPath);
            item.Icon = icon;
        }
    }

    /// <summary>
    /// Thread 관련 문제가 있어서 일단 안쓰도록 해 놨다.
    /// </summary>
    class ProducerConsumerQueue : IDisposable, IIconProducer
    {
        EventWaitHandle wh = new AutoResetEvent(false);
        Thread worker;
        object locker = new object();
        Queue<IBrowserItem> tasks = new Queue<IBrowserItem>();

        public ProducerConsumerQueue()
        {
            worker = new Thread(Work);
            worker.Start();
        }

        public void EnqueueTask(IBrowserItem task)
        {
            lock (locker) tasks.Enqueue(task);
            wh.Set();
        }

        public void Dispose()
        {
            EnqueueTask(null);     // Signal the consumer to exit.
            worker.Join();          // Wait for the consumer's thread to finish.
            wh.Close();             // Release any OS resources.
        }

        void Work()
        {
            while (true)
            {
                IBrowserItem task = null;
                lock (locker)
                    if (tasks.Count > 0)
                    {
                        task = tasks.Dequeue();
                        if (task == null) return;
                    }
                if (task != null)
                {
                    Thread.Sleep(1000);
                    Icon icon = Win32.getIcon(task.FullPath);
                    task.Icon = icon;
                }
                else
                    wh.WaitOne();         // No more tasks - wait for a signal
            }
        }
    }
    /// <summary>
    /// 파일 형식에 대한 IBrowserItem 구현입니다.
    /// </summary>
    public class FileItem : IBrowserItem
    {
        FileInfo m_file;
        IBrowser m_browser;
        Icon m_Icon;
        BrowserItemState m_state;

        public FileItem(FileInfo file, IBrowser browser)
        {
            m_file = file;
            m_browser = browser;
            IconProducer.s_inst.EnqueueTask(this);
        }

        #region IBrowserItem 멤버

        public string showName
        {
            get { return m_file.Name; }
        }

        public Icon Icon
        {
            get 
            { 
                return m_Icon; 
            }
            set 
            {
                m_Icon = value;
                var temp = onChanged;
                if (temp != null) temp(this, EventArgs.Empty);
            }
        }

        public BrowserItemState State
        {
            get
            {
                return m_state;
            }
            set
            {
                m_state = value;
                EventHandler temp = onChanged;
                if (temp != null)
                    temp(this, EventArgs.Empty);
            }
        }

        public event EventHandler onChanged;

        public string FullPath
        {
            get { return m_file.FullName; }
        }

        public void accept(IBrowserItemVisitor visitor)
        {
            visitor.visit(this);
        }

        #endregion
    }

    public class DirectoryItem : IBrowserItem
    {
        DirectoryInfo m_info;
        IBrowser m_browser;
        string m_name;
        Icon m_icon;
        BrowserItemState m_state;

        public DirectoryItem(DirectoryInfo info, string name, IBrowser browser)
        {
            if (info == null) throw new NullReferenceException("DirectoryItem 클래스의 생성자가 잘못되었습니다");
            m_info = info;
            m_name = name;
            m_browser = browser;
            IconProducer.s_inst.EnqueueTask(this);
        }

        public string showName
        {
            get
            {
                return m_name;
            }
        }

        public DirectoryInfo DirInfo
        {
            get
            {
                return m_info;
            }
        }

        public Icon Icon
        {
            get
            {
                return m_icon;
            }
            set 
            { 
                m_icon = value;
                var temp = onChanged;
                if (temp != null) temp(this, EventArgs.Empty);
            }
        }

        public BrowserItemState State
        {
            get
            {
                return m_state;
            }
            set
            {
                m_state = value;
                EventHandler temp = onChanged;
                if (temp != null)
                    temp(this, EventArgs.Empty);
            }
        }

        public event EventHandler onChanged;

        #region IBrowserItem 멤버

        public string FullPath
        {
            get { return m_info.FullName; }
        }

        public void accept(IBrowserItemVisitor visitor)
        {
            visitor.visit(this);
        }

        #endregion
    }
}