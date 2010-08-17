using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Config
{
    interface IConfig
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
}
