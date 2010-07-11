using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using FileHatchery.Engine;

namespace FileHatchery
{
    static class Program
    {
        /// <summary>
        /// 해당 응용 프로그램의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                form = new Form1();
                engine = new TestEngineQuery(new DirectoryBrowser(), form.Handle);
                Application.Run(form);
                engine.Dispose();
            }
            catch(Exception E)
            {
                MessageBox.Show(E.Message);
                throw;
            }
        }

        public static TestEngineQuery engine;
        public static Form1 form;
    }
}
