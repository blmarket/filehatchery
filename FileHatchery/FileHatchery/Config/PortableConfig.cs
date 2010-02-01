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
        const string configfilename = "config.ini";
        Dictionary<string, string> m_Config;

        public PortableConfig(EngineQuery engine)
        {
            string LocalPath = Application.LocalUserAppDataPath;
            string path = Path.Combine(LocalPath, configfilename);
            FileStream stream;
            stream = File.Open(path, FileMode.OpenOrCreate);
            XmlSerializer serializer = new XmlSerializer(typeof(Dictionary<string,string>));
            m_Config = (Dictionary<string,string>)serializer.Deserialize(stream);
        }

        #region IConfig 멤버

        public void execute(object[] args)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
