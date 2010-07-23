using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

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

    /// <summary>
    /// UI에서 필요한 정보를 넘겨주는 Interface
    /// </summary>
    public interface IPagedLayoutInterface
    {
        /// <summary>
        /// 현재 Cursor의 위치
        /// </summary>
        int CursorPos { get; }

        /// <summary>
        /// 한 열에 몇개의 row가 들어가는 지 설정함.
        /// </summary>
        /// <param name="rowSize">들어가야 할 row의 갯수</param>
        void setRowSize(int rowSize);
    }

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
        /// 탐색하고 있는 디렉토리가 변경되려고 할 때 발생하는 이벤트
        /// </summary>
        event EventHandler DirectoryChanging;

        /// <summary>
        /// 탐색하고 있는 디렉토리가 변경되었을 때 발생하는 이벤트
        /// </summary>
        event EventHandler DirectoryChanged;

        /// <summary>
        /// 커서 위치가 변경되었을 때 발생하는 이벤트
        /// </summary>
        event EventHandler CursorChanged;

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
}
