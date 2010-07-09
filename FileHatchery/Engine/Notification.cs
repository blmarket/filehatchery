using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FileHatchery.Engine.Notification
{
    /// <summary>
    /// Engine에서 알리고 싶은 메시지가 있는 경우 사용하는 메시지 인터페이스.
    /// </summary>
    public class NotifyArgs : EventArgs
    {
        /// <summary>
        /// 기본 생성자
        /// </summary>
        /// <param name="msg">알리고 싶은 메시지. 귀찮아서 지금은 string 하나</param>
        public NotifyArgs(string msg)
        {
            Message = msg;
        }

        /// <summary>
        /// 알리고 싶은 메시지
        /// </summary>
        public string Message;
    }
}
