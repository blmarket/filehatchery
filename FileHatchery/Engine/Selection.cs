using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FileHatchery
{
    /// <summary>
    /// 현재 선택중인 파일들을 정의하는 클래스
    /// </summary>
    public class Selection
    {
        private HashSet<IBrowserItem> m_Dict;

        /// <summary>
        /// 생성자
        /// </summary>
        public Selection()
        {
            m_Dict = new HashSet<IBrowserItem>();
        }

        /// <summary>
        /// 선택된 item들을 clear한다.
        /// </summary>
        public void clear() 
        {
            foreach (var item in m_Dict)
            {
                item.State = item.State ^ BrowserItemState.Marked;
            }
            m_Dict.Clear();
        }

        /// <summary>
        /// item이 선택되었는지 여부를 알려준다.
        /// </summary>
        /// <param name="item">확인하고 싶은 item</param>
        /// <returns>선택 여부, true면 선택된 상태, false인 경우 선택되지 않은 상태</returns>
        public bool isSelected(IBrowserItem item)
        {
            return m_Dict.Contains(item);
        }

        /// <summary>
        /// item이 선택되어 있으면 해제하고, 그렇지 않은 경우 선택한다.
        /// </summary>
        /// <param name="item">선택/해제하고 싶은 item</param>
        public void MarkItem(IBrowserItem item)
        {
            // Mark할 수 없는 Item인 경우 Mark하지 않는다.
            if ((item.State & BrowserItemState.UnMarkable) == BrowserItemState.UnMarkable)
                return;

            // 이미 Mark된 경우 해제하고, 안그런 경우 선택한다.
            if ((item.State & BrowserItemState.Marked) == BrowserItemState.Marked)
            {
                m_Dict.Remove(item);
            }
            else
            {
                m_Dict.Add(item);
            }

            // Mark flag값을 XOR해준다.
            item.State = item.State ^ BrowserItemState.Marked;
        }

        /// <summary>
        /// item을 선택한다.
        /// </summary>
        /// <param name="item">선택하고 싶은 item</param>
        public void addItem(IBrowserItem item)
        {
            // Mark할 수 없는 Item인 경우 Mark하지 않는다.
            if ((item.State & BrowserItemState.UnMarkable) == BrowserItemState.UnMarkable)
                return;

            m_Dict.Add(item);
            item.State = item.State | BrowserItemState.Marked;
        }

        /// <summary>
        /// item의 선택을 해제한다.
        /// </summary>
        /// <param name="item">선택 해제하고 싶은 item</param>
        public void removeitem(IBrowserItem item)
        {
            m_Dict.Remove(item);
            item.State = item.State & (~BrowserItemState.Marked);
        }

        public HashSet<IBrowserItem> Context
        {
            get
            {
                return m_Dict;
            }
        }

        /// <summary>
        /// 열거자를 반환한다.
        /// </summary>
        /// <returns>선택 개체들의 열거자</returns>
        public HashSet<IBrowserItem>.Enumerator GetEnumerator()
        {
            return m_Dict.GetEnumerator();
        }

        /// <summary>
        /// 현재 선택된 item들의 갯수
        /// </summary>
        public int Count
        {
            get
            {
                return m_Dict.Count;
            }
        }
    }
}
