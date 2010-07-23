using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FileHatchery
{
    public interface IComponentContainer
    {
        object getComponent(Type type);
        void addComponent(Type type, object obj);
        void delComponent(Type type);
    }

    public abstract class ComponentContainer : IComponentContainer
    {
        private Dictionary<Type, object> m_dict = new Dictionary<Type,object>();

        public object getComponent(Type type)
        {
            object ret = null;
            if (m_dict.TryGetValue(type, out ret)) 
                return ret;
            return null;
        }

        public void addComponent(Type type, object obj)
        {
            m_dict[type] = obj;
        }

        public void delComponent(Type type)
        {
            m_dict.Remove(type);
        }
    }

    public class Core
    {
        public static object getComponent(IComponentContainer container, Type type)
        {
            return container.getComponent(type);
        }
    }
}
