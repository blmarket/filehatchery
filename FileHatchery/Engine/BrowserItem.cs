﻿using ShellApi;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;

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
        EventHandler onChanged { get; set; }

        /// <summary>
        /// Visitor 패턴 구현. 임의의 Visitor별 구현을 실행합니다.
        /// </summary>
        /// <param name="visitor">사용하고 싶은 Visitor</param>
        void accept(IBrowserItemVisitor visitor);
    };

    /// <summary>
    /// 파일 정보를 읽어들여 Icon을 생성한 다음에 넘겨주는 작업을 하는 Class
    /// </summary>
    public interface IIconProducer : IDisposable
    {
        /// <summary>
        /// 작업 목록을 날린다. 디렉토리 변경 따위를 할 때 유용함.
        /// </summary>
        void ClearQueue();

        /// <summary>
        /// 새로운 작업을 추가한다.
        /// </summary>
        /// <param name="item">원하는 파일/디렉토리</param>
        void EnqueueTask(IBrowserItem item);
    }

    class NullProducer : IIconProducer
    {
        public void EnqueueTask(IBrowserItem item)
        {
            // do nothing
        }

        public void Dispose()
        {
            // do nothing
        }

        public void ClearQueue()
        {
            // do nothing
        }
    }

    class SimpleProducer : IIconProducer
    {
        public void EnqueueTask(IBrowserItem item)
        {
            Icon icon = Win32.getIcon(item.FullPath);
            item.Icon = icon;
        }

        public void Dispose()
        {
        }

        public void ClearQueue()
        {
            // do nothing
        }
    }

    /// <summary>
    /// Thread 관련 문제가 있어서 일단 안쓰도록 해 놨다.
    /// </summary>
    class ProducerConsumerQueue : IIconProducer
    {
        EventWaitHandle wh = new AutoResetEvent(false);
        Thread worker;
        Queue<IBrowserItem> tasks = new Queue<IBrowserItem>();

        public ProducerConsumerQueue()
        {
            worker = new Thread(Work);
            worker.Priority = ThreadPriority.BelowNormal;
            worker.Start();
        }

        public void EnqueueTask(IBrowserItem task)
        {
            lock (this)
            {
                tasks.Enqueue(task);
                wh.Set();
            }
        }

        public void ClearQueue()
        {
            lock (this)
            {
                tasks.Clear();
            }
        }

        public void Dispose()
        {
            ClearQueue();
            EnqueueTask(null);     // Signal the consumer to exit.
            worker.Join();          // Wait for the consumer's thread to finish.
            wh.Close();             // Release any OS resources.
        }

        void Work()
        {
            while (true)
            {
                IBrowserItem task = null;
                lock (this)
                    if (tasks.Count > 0)
                    {
                        task = tasks.Dequeue();
                        if (task == null) return;
                    }
                if (task != null)
                {
                    Thread.Sleep(15);
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
        public FileInfo File { get; set; }
        IBrowser m_browser;
        Icon m_Icon;
        BrowserItemState m_state;

        public FileItem(FileInfo file, IBrowser browser)
        {
            File = file;
            m_browser = browser;

            IIconProducer temp = ComponentContainer.getComponent<IIconProducer>(Engine.TestEngineQuery.s_inst);
            if(temp != null)
                temp.EnqueueTask(this);
        }

        #region IBrowserItem 멤버

        public string showName
        {
            get { return File.Name; }
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

        public EventHandler onChanged { get; set; }

        public string FullPath
        {
            get { return File.FullName; }
        }

        public void accept(IBrowserItemVisitor visitor)
        {
            visitor.visit(this);
        }

        #endregion
    }

    public class DirectoryItem : IBrowserItem
    {
        public DirectoryInfo DirInfo { get; set; }
        IBrowser m_browser;
        string m_name;
        Icon m_icon;
        BrowserItemState m_state;

        public DirectoryItem(DirectoryInfo info, string name, IBrowser browser)
        {
            if (info == null) throw new NullReferenceException("DirectoryItem 클래스의 생성자가 잘못되었습니다");
            DirInfo = info;
            m_name = name;
            m_browser = browser;

            var temp = Engine.TestEngineQuery.s_inst.getComponent<IIconProducer>();
            if (temp != null)
                temp.EnqueueTask(this);
        }

        public string showName
        {
            get
            {
                return m_name;
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

        public EventHandler onChanged { get; set; }

        #region IBrowserItem 멤버

        public string FullPath
        {
            get { return DirInfo.FullName; }
        }

        public void accept(IBrowserItemVisitor visitor)
        {
            visitor.visit(this);
        }

        #endregion
    }
}