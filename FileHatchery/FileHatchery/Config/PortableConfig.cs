using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace FileHatchery.Config
{
    public class PortableConfig : IConfig, ICollection
    {
        Dictionary<string, string> m_Config;

        private string ConfigPath
        {
            get
            {
                return Path.Combine(Application.LocalUserAppDataPath, "config.ini");
            }
        }

        public PortableConfig()
        {
            FileStream stream = File.Open(ConfigPath, FileMode.OpenOrCreate);
            XmlSerializer serializer = new XmlSerializer(typeof(Dictionary<string,string>));
            m_Config = (Dictionary<string,string>)serializer.Deserialize(stream);
            stream.Close();
        }

        ~PortableConfig()
        {
            FileStream stream = File.Open(ConfigPath, FileMode.OpenOrCreate);
            XmlSerializer serializer = new XmlSerializer(typeof(Dictionary<string, string>));
            serializer.Serialize(stream, m_Config);
            stream.Close();
        }

        string IConfig.getConfig(string name)
        {
            return m_Config[name];
        }

        void IConfig.setConfig(string name, string value)
        {
            m_Config[name] = value;
        }

        void ICollection.CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        int ICollection.Count
        {
            get { throw new NotImplementedException(); }
        }

        bool ICollection.IsSynchronized
        {
            get { throw new NotImplementedException(); }
        }

        object ICollection.SyncRoot
        {
            get { throw new NotImplementedException(); }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
