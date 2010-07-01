using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FileHatchery.Engine.Notification
{
    /// <summary>
    /// Engine에서 알리고 싶은 메시지가 있는 경우 사용하는 메시지 인터페이스.
    /// </summary>
    public interface INotification
    {
        /// <summary>
        /// 알리고 싶은 메시지
        /// </summary>
        string Message { get; }
    }

    /// <summary>
    /// INotification의 간단한 구현.
    /// </summary>
    public class Notification : INotification
    {
        string m_message;

        /// <summary>
        /// 기본 생성자. 알리고 싶은 메시지를 argument로 넣는다.
        /// </summary>
        /// <param name="msg">알리고 싶은 메시지</param>
        public Notification(string msg)
        {
            m_message = msg;
        }

        string INotification.Message
        {
            get
            {
                return m_message;
            }
        }
    }
}
