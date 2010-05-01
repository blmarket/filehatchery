using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Config
{
    public class FileHatcheryConfig
        : ConfigurationSection
    {
        [ConfigurationProperty("bookmark", DefaultValue = "")]
        [StringValidator()]
        public string Bookmark
        {
            get
            {
                return (string)this["bookmark"];
            }
            set
            {
                this["bookmark"] = value;
            }
        }
    }

    public class FileHatcheryConfigManager
    {
        const string ConfigCategoryName = "FileHatcheryConfig";
        static Configuration mainConfig = null;

        public static Configuration Config
        {
            get
            {
                if (mainConfig == null)
                    mainConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                return mainConfig;
            }
        }

        public static FileHatcheryConfig FHConfig
        {
            get
            {
                try
                {
                    FileHatcheryConfig myConfig;

                    // Get the current configuration file.
                    Configuration config = Config;

                    // Create the section entry  
                    // in <configSections> and the 
                    // related target section in <configuration>.
                    if (config.Sections[ConfigCategoryName] == null)
                    {
                        myConfig = new FileHatcheryConfig();
                        config.Sections.Add(ConfigCategoryName, myConfig);
                        myConfig.SectionInformation.ForceSave = true;
                        config.Save(ConfigurationSaveMode.Full);
                    }

                    return (FileHatcheryConfig)config.Sections[ConfigCategoryName];
                }
                catch
                {
                    throw;
                }
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            FileHatcheryConfig conf = FileHatcheryConfigManager.FHConfig;
            FileHatcheryConfigManager.Config.Save();
        }
    }
}
