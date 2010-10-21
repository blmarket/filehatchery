using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WooriLogReader
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            {
                var stream = new System.IO.StreamReader("asdf.xls", Encoding.Default);
                string tmpx;
                do
                {
                    tmpx = stream.ReadLine();
                    if (tmpx.Contains("메모"))
                    {
                    }
                    System.Diagnostics.Debug.WriteLine(tmpx);
                } while (stream.EndOfStream == false);                
                
                stream.Close();
            }

            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.Load("asdf.xls", true);
            HtmlAgilityPack.HtmlNode node = doc.DocumentNode;
            var tmp = node.SelectNodes("//tr[@class='output_title']");
            var par = tmp[0].ParentNode;
            foreach (var iter in par.ChildNodes)
            {
                System.Diagnostics.Debug.WriteLine(iter);
            }
            System.Diagnostics.Debug.WriteLine("I am using dot net debugging");
        }
    }
}
