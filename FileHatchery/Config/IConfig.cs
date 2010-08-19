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
        private FileStream stream;
        private string filepath;

        public PortableConfig(string filename = "filehatchery.ini")
        {
            filepath = Util.getExecutablePath() + "\\" + filename;
            XmlSerializer ser = new XmlSerializer(typeof(SerializableDictionary<string, string>));
            if (File.Exists(filepath))
            {
                try
                {
                    stream = File.Open(filepath, FileMode.Open);
                    ser.Deserialize(stream);
                }
                catch
                {
                }
            }
            else
            {
                stream = File.Open(filepath, FileMode.CreateNew);
            }

            ser = null;
            stream.Flush();
            stream.Close();
            stream.Dispose();
            stream = null;
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
            if (stream == null)
            {
                stream = File.Open(filepath, FileMode.Create);
            }

            stream.Seek(0, SeekOrigin.Begin);
            XmlSerializer ser = new XmlSerializer(typeof(SerializableDictionary<string, string>));
            ser.Serialize(stream, Dict);
        }
    }
}
