using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace FileHatchery.Config
{
    public interface IConfig
    {
        string getConfig(string name);
        void setConfig(string name, string value);
    }
}
