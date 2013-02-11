using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using ShellApi;
using System.IO;
using System.Drawing;
using FileHatchery;

namespace Testing
{
    class Program
    {
        static void Test1()
        {
            DirectoryInfo info = new DirectoryInfo(@"D:\AV"); // or 뭔가 이상한 일본어+중국어 짬뽕 이름의 디렉토리가 필요함
            if (info == null) return;
            foreach(DirectoryInfo dir in info.GetDirectories())
            {
                try
                {
                    Icon icon = ShellApi.Win32.getIcon(dir.FullName);
                }
                catch (Exception E)
                {
                    Console.WriteLine("Exception From Test1");
                    Console.WriteLine("Exception while handling " + dir.FullName);
                    Console.WriteLine(E.Message);
                    throw;
                }
            }
        }

        static void Test2()
        {
            Form ff = new Form();
            FileHatchery.PagedLayoutPanel pp = new PagedLayoutPanel();
            pp.Size = new Size(0, 0);
            pp.PageService = new SimplePagedLayoutInterface();
            ff.Controls.Add(pp);
            ff.Load += new EventHandler(delegate(object obj, EventArgs e)
                {
                    pp.PerformLayout();
                    ff.Close();
                });
            try
            {
                ff.ShowDialog();
            }
            catch (System.DivideByZeroException)
            {
                throw;
            }
        }

        static void Test4()
        {
            FileInfo tmpfile = new FileInfo(System.IO.Path.GetTempFileName());
            FileHatchery.Engine.TestEngineQuery eng = new FileHatchery.Engine.TestEngineQuery(new DirectoryBrowser(), IntPtr.Zero);
            eng.RunCommand("open " + tmpfile.Directory);
            eng.RunCommand("select " + tmpfile.Name);
            eng.RunCommand("delete silent");
        }

        static void Test6()
        {
            string dirpath = FileHatchery.Engine.Components.Config.Util.getExecutablePath();
            Console.WriteLine(dirpath);
            string filepath = dirpath + "\\test.dat";
            Console.WriteLine(filepath);

            XmlSerializer seri = new XmlSerializer(typeof(FileHatchery.Engine.Components.Config.SerializableDictionary<string, string>));
            FileHatchery.Engine.Components.Config.SerializableDictionary<string, string> vv;

            FileStream str = File.Open(filepath, FileMode.OpenOrCreate);

            try
            {
                object des = seri.Deserialize(str);
                vv = des as FileHatchery.Engine.Components.Config.SerializableDictionary<string, string>;
            }
            catch (Exception)
            {
                vv = new FileHatchery.Engine.Components.Config.SerializableDictionary<string, string>();
            }

            vv["asdf"] = "news";

            str.Seek(0, SeekOrigin.Begin);
            seri.Serialize(str, vv);
        }

        static void Test3()
        {
            FileHatchery.Engine.Components.Config.IConfig pp = new FileHatchery.Engine.Components.Config.PortableConfig();
            pp["news"] = "asdf";
            pp.Save();
        }

        static void Main(string[] args)
        {
            try
            {
                /*
                Test1();
                Test2();
                Test4();
                 */
                Test6();
                Test3();
                SerializationTest.Test.SerializationTest();
                Console.WriteLine("Test Successful");
            }
            catch(Exception e)
            {
                Console.WriteLine("Test Failed");
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);

                Console.WriteLine(e.InnerException);
            }
        }
    }
}
