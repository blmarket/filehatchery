using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace FileHatchery
{
    using Engine.Components;
    class ConfigSelector
    {
        public static Font Font
        {
            get
            {
                try
                {
                    FileHatchery.Engine.Components.Config.IConfig cfg = Program.engine.getComponent<FileHatchery.Engine.Components.Config.IConfig>();

                    FontConverter fc = new FontConverter();
                    Font tmpFont = (Font)fc.ConvertFromString(cfg["Font"]);
                    return tmpFont;
                }
                catch (Exception EE)
                {
                    MessageBox.Show(EE.Message);
                }
                return null;
            }
        }

        public static TextBox Console
        {
            get
            {
                if (Program.engine.getConfiguration("useConsoleTextBox"))
                {
                    ConsoleTextBox tmp = new ConsoleTextBox();
                    tmp.engine = Program.engine.getComponent<Engine.Components.IAutoCompletion>();
                    return tmp;
                }
                else
                {
                    AutoCompleteStringCollection collection = new AutoCompleteStringCollection();
                    foreach (string str in Program.engine.getComponent<IAutoCompletion>().Commands) collection.Add(str);

                    TextBox console = new TextBox();
                    console.AutoCompleteMode = AutoCompleteMode.Suggest;
                    console.AutoCompleteSource = AutoCompleteSource.CustomSource;
                    console.AutoCompleteCustomSource = collection;
                    console.Width = 200;
                    console.AcceptsTab = true;
                    console.KeyDown += new KeyEventHandler(console_KeyDown);
                    return console;
                }
            }
        }

        static void console_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Escape)
            {
                TextBox console = sender as TextBox;
                console.Hide();
                e.Handled = true;
            }
            else if (e.KeyData == Keys.Enter)
            {
                TextBox console = (TextBox)sender;
                try
                {
                    Program.engine.RunCommand(console.Text);
                }
                catch (Exception EE)
                {
                    MessageBox.Show(console.Text + " : " + EE.Message);
                }
                console.Hide();
                e.Handled = true;
            }
        }
    }
}
