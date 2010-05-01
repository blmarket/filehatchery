using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Configuration
{
    // Define a custom section.
    public sealed class CustomSection :
        ConfigurationSection
    {

        public enum Permissions
        {
            FullControl = 0,
            Modify = 1,
            ReadExecute = 2,
            Read = 3,
            Write = 4,
            SpecialPermissions = 5
        }

        public CustomSection()
        {
        }

        [ConfigurationProperty("fileName",
            DefaultValue = "default.txt")]
        [StringValidator(InvalidCharacters = " ~!@#$%^&*()[]{}/;'\"|\\",
                   MinLength = 1, MaxLength = 60)]
        public String FileName
        {
            get
            {
                return (String)this["fileName"];
            }
            set
            {
                this["fileName"] = value;
            }
        }

        [ConfigurationProperty("maxIdleTime", DefaultValue = "1:30:30")]
        public TimeSpan MaxIdleTime
        {
            get
            {
                return (TimeSpan)this["maxIdleTime"];
            }
            set
            {
                this["maxIdleTime"] = value;
            }
        }


        [ConfigurationProperty("permission",
            DefaultValue = Permissions.Read)]
        public Permissions Permission
        {
            get
            {
                return (Permissions)this["permission"];
            }

            set
            {
                this["permission"] = value;
            }

        }

    }


    class Program
    {
        static void Main(string[] args)
        {
            CustomSection customSection;

            // Get the current configuration file.
            System.Configuration.Configuration config =
                    ConfigurationManager.OpenExeConfiguration(
                    ConfigurationUserLevel.None);

            // Create the section entry  
            // in <configSections> and the 
            // related target section in <configuration>.
            if (config.Sections["CustomSection"] == null)
            {
                customSection = new CustomSection();
                config.Sections.Add("CustomSection", customSection);
                customSection.SectionInformation.ForceSave = true;
                config.Save(ConfigurationSaveMode.Full);

                Console.WriteLine("Section name: {0} created",
                    customSection.SectionInformation.Name);

            }
        }
    }
}
