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
    /// BrowserItem의 상태 값들을 bitmask 형태로 표현합니다.
    /// </summary>
    [Flags]
    public enum BrowserItemState
    {
        /// <summary>
        /// 현재 커서가 Item 위일 때 체크됩니다.
        /// </summary>
        Selected = 1,

        /// <summary>
        /// 현재 Item이 선택된 상태일 때 체크됩니다.
        /// </summary>
        Marked = 2,

        /// <summary>
        /// 현재 Item이 선택 불가능한 경우 체크합니다.
        /// </summary>
        UnMarkable = 4,
    };

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
        System.Drawing.Icon Icon { get; }

        /// <summary>
        /// 이 Item의 현재 상태를 설정하거나 반환합니다.
        /// </summary>
        BrowserItemState State { get; set; }

        /// <summary>
        /// Visitor Pattern Implementation
        /// </summary>
        /// <param name="visitor">Specific Visitor</param>
        void accept(IBrowserItemVisitor visitor);

        /// <summary>
        /// Item의 State가 변경되었을 때 발생하는 event입니다.
        /// </summary>
        event EventHandler onChanged;
    };

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
            m_Icon = Win32.getIcon(file.FullName);
        }

        #region IBrowserItem 멤버

        public string showName
        {
            get { return m_file.Name; }
        }

        public Icon Icon
        {
            get { return m_Icon; }
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
                if (onChanged != null) onChanged(this, EventArgs.Empty);
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
            m_icon = Win32.getIcon(m_info.FullName);
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
                if (onChanged != null)
                    onChanged(this, EventArgs.Empty);
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