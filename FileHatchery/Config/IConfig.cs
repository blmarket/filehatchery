using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;

namespace Config
{
    public interface IConfig
    {
        string this[string key] { get; set; }
        void Save();
    }

    public class Util
    {
        public static string getExecutablePath()
        {
            System.Reflection.Assembly ass = System.Reflection.Assembly.GetExecutingAssembly();
            return Path.GetDirectoryName(ass.Location);
        }
    }

    public class PortableConfig : IConfig
    {
        private SerializableDictionary<string, string> Dict = new SerializableDictionary<string, string>();
        private string filepath;

        public PortableConfig(string filename = "filehatchery.ini")
        {
            filepath = Util.getExecutablePath() + "\\" + filename;
            XmlSerializer ser = new XmlSerializer(typeof(SerializableDictionary<string, string>));
            if (File.Exists(filepath))
            {
                try
                {
                    FileStream str = File.OpenRead(filepath);
                    ser.Deserialize(str);
                    str.Close();
                    str.Dispose();
                }
                catch
                {
                }
            }
        }

        string IConfig.this[string key]
        {
            get
            {
                return Dict[key];
            }
            set
            {
                Dict[key] = value;
            }
        }

        void IConfig.Save()
        {
            XmlSerializer ser = new XmlSerializer(typeof(SerializableDictionary<string, string>));
            ser.Serialize(File.Open(filepath, FileMode.Create), Dict);
        }
    }
}
