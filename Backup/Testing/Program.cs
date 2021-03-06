﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
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

        static void Main(string[] args)
        {
            try
            {
                Test1();
                Test2();
                Console.WriteLine("Test Successful");
            }
            catch(Exception e)
            {
                Console.WriteLine("Test Failed");
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }
    }
}
