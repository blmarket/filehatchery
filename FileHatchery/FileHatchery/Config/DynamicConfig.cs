using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace FileHatchery.Config
{
    class DynamicConfig : IModule
    {
        private Dictionary<int, string> Config;
        private EngineQuery engine;

        public DynamicConfig(EngineQuery engine)
        {
            Config = new Dictionary<int, string>();
            this.engine = engine;
        }

        #region IModule 멤버

        public void execute(object[] args)
        {
            switch ((string)args[0])
            {
                case "save":
                    {
                        int num = (int)args[1];
                        DirectoryInfo info = args[2] as DirectoryInfo;
                        Config[num] = info.FullName;
                    }
                    break;
                case "load":
                    {
                        int num = (int)args[1];
                        engine.Browser.CurrentDir = new DirectoryInfo(Config[num]);
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        #endregion
    }
}
