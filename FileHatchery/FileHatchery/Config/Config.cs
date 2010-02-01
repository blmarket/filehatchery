using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace FileHatchery.Config
{
    public interface IConfig
    {
        void execute(object[] args);
    }
}
