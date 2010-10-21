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
            string htmldoc = "";
            {
                var stream = new System.IO.StreamReader("asdf.xls", Encoding.Default);
                string tmpx;
                do
                {
                    tmpx = stream.ReadLine();
                    if (tmpx.Contains("메모"))
                    {
                        htmldoc = "<table>" + stream.ReadToEnd();
                        break;
                    }
                } while (stream.EndOfStream == false);                                
                stream.Close();
            }

            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(htmldoc);
            HtmlAgilityPack.HtmlNode node = doc.DocumentNode;
            var par = node.SelectNodes("//table")[0];
            foreach (var iter in par.SelectNodes("//tr"))
            {
                //거래일자	적요	  기재내용	찾으신금액	맡기신금액	거래후잔액	취급점	메모
                List<string> list = new List<string>();
                foreach (var jter in iter.Elements("td"))
                {
                    list.Add(jter.InnerText);
                }

                DateTime date = DateTime.Parse(list[0]);

                string cost = list[3];
                int result;
                result = Int32.Parse(cost, System.Globalization.NumberStyles.Number);
                System.Diagnostics.Debug.WriteLine(result);
            }
            System.Diagnostics.Debug.WriteLine("I am using dot net debugging");
        }
    }
}
