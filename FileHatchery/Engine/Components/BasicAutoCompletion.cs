using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FileHatchery.Engine.Components
{
    class BasicAutoCompletion : IAutoCompletion
    {
        Dictionary<string, int> internal_commands { get; set; }

        public BasicAutoCompletion()
        {
            internal_commands = new Dictionary<string, int>();
            // for testing
            internal_commands.Add("asdfnews", 1);
            internal_commands.Add("google", 1);
            internal_commands.Add("gosick", 1);
            internal_commands.Add("gooooogle", 1);
            internal_commands.Add("microsoft", 1);
        }

        #region IAutoCompletion 멤버

        public List<string> Commands
        {
            get
            {
                List<string> ret = new List<string>();
                foreach (KeyValuePair<string, int> kv in internal_commands)
                {
                    ret.Add(kv.Key);
                }
                return ret;
            }
        }

        #endregion
    }
}
