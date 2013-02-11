using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FileHatchery
{
    public interface IComponentContainer
    {
        T getComponent<T>();
        void setComponent(Type type, object obj);
        void delComponent(Type type);
    }

    public abstract class ComponentContainer : IComponentContainer
    {
        private Dictionary<Type, object> m_dict = new Dictionary<Type,object>();

        public T getComponent<T>()
        {
            object ret = null;
            if (m_dict.TryGetValue(typeof(T), out ret)) 
                return (T)ret;
            return default(T);
        }

        public void setComponent(Type type, object obj)
        {
            m_dict[type] = obj;
        }

        public void delComponent(Type type)
        {
            m_dict.Remove(type);
        }

        public static T getComponent<T>(IComponentContainer container)
        {
            return container.getComponent<T>();
        }
    }
}
