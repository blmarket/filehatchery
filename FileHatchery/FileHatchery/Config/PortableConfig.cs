using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace FileHatchery.Config
{
    class PortableConfig : IConfig
    {
        Dictionary<string, string> m_Config;

        private string ConfigPath
        {
            get
            {
                return Path.Combine(Application.LocalUserAppDataPath, "config.ini");
            }
        }

        public PortableConfig(TestEngineQuery engine)
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

        #region IConfig 멤버

        public void execute(object[] args)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
