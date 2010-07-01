using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FileHatchery.Engine.Notification
{
    public interface INotification
    {
        string Message { get; }
    }

    public class Notification : INotification
    {
        string m_message;

        public Notification(string msg)
        {
            m_message = msg;
        }

        public string Message
        {
            get
            {
                return m_message;
            }
        }
    }
}
