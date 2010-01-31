using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

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
            engine = new EngineQuery();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            form = new Form1();
            Application.Run(form);
        }

        public static EngineQuery engine;
        public static Form1 form;
    }
}
