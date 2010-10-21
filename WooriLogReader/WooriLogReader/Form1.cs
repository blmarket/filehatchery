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

            DateTime minDate = DateTime.MaxValue, maxDate = DateTime.MinValue;

            System.Collections.Generic.Dictionary<DateTime, List<Tuple<string, string, long, long, long, string, string>>> dict = new Dictionary<DateTime,List<Tuple<string,string,long,long,long,string,string>>>();

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

                if (minDate == null || minDate > date) minDate = date;
                if (maxDate == null || maxDate < date) maxDate = date;

                long expense = Int64.Parse(list[3], System.Globalization.NumberStyles.Number);
                long income = Int64.Parse(list[4], System.Globalization.NumberStyles.Number);
                long cash = Int64.Parse(list[5], System.Globalization.NumberStyles.Number);

                if (dict.ContainsKey(date) == false)
                    dict.Add(date, new List<Tuple<string, string, long, long, long, string, string>>());

                dict[date].Add(new Tuple<string,string,long,long,long,string,string>(list[1], list[2], expense, income, cash, list[6], list[7]));
            }
            System.Diagnostics.Debug.WriteLine(minDate + " " + maxDate);
            minDate = minDate.Add(new TimeSpan(1, 0, 0, 0));
//            maxDate = maxDate.Subtract(new TimeSpan(1, 0, 0, 0));
            System.Diagnostics.Debug.WriteLine(minDate + " " + maxDate);

            for (DateTime date = minDate; date != maxDate; date = date.Add(new TimeSpan(1, 0, 0, 0)))
            {
                System.Diagnostics.Debug.WriteLine(date);
            }
        }
    }
}
