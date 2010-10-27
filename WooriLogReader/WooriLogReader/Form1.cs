using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

using System.Data.SqlServerCe;

namespace WooriLogReader
{
    public partial class Form1 : Form
    {
        private string connString = "Data Source='Test.sdf'; LCID=1033; Password=asdf; Encrypt = TRUE;";
        private SqlCeDataReader current_reader;
        private SqlCeConnection conn;

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

            try
            {
                if (conn == null)
                {
                    conn = new SqlCeConnection(connString);
                    conn.Open();
                }

                for (DateTime date = minDate; date != maxDate; date = date.Add(new TimeSpan(1, 0, 0, 0)))
                {
                    System.Diagnostics.Debug.WriteLine(date);

                    if (dict.ContainsKey(date) == false)
                        dict.Add(date, new List<Tuple<string, string, long, long, long, string, string>>());

                    foreach(var item in dict[date])
                    {
                        SqlCeCommand cmd = conn.CreateCommand();
                        cmd.CommandText = "INSERT INTO banklogs (date, category, name, expense, income, bank, memo) VALUES (@date, @category, @name, @expense, @income, @bank, @memo);";
                        cmd.Parameters.AddWithValue("@date", date);
                        cmd.Parameters.AddWithValue("@category", item.Item1);
                        cmd.Parameters.AddWithValue("@name", item.Item2);
                        cmd.Parameters.AddWithValue("@expense", item.Item3);
                        cmd.Parameters.AddWithValue("@income", item.Item4);
                        cmd.Parameters.AddWithValue("@bank", item.Item6);
                        cmd.Parameters.AddWithValue("@memo", item.Item7);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception E)
            {
                MessageBox.Show(E.Message);
                conn.Close();
                conn = null;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            File.Delete("Test.sdf");
            SqlCeEngine engine = new SqlCeEngine(connString);
            engine.CreateDatabase();
            engine.Dispose();

            try
            {
                if (conn == null)
                {
                    conn = new SqlCeConnection(connString);
                    conn.Open();
                }

                SqlCeCommand cmd = conn.CreateCommand();
                cmd.CommandText = @"CREATE TABLE banklogs ( 
idx INT IDENTITY NOT NULL PRIMARY KEY ,
date DATETIME NOT NULL ,
category NVARCHAR( 20 ) NOT NULL ,
name NVARCHAR( 20 ) NOT NULL ,
expense INT NOT NULL ,
income INT NOT NULL ,
bank NVARCHAR( 20 ) NOT NULL ,
memo NVARCHAR( 100 ) NOT NULL ,
cat INT
);";
                cmd.ExecuteNonQuery();
            }
            catch (Exception E)
            {
                MessageBox.Show(E.Message);
                conn.Close();
                conn.Dispose();
                conn = null;
            }
        }

        private string ToSQLDateTime(DateTime date)
        {
            return date.ToShortDateString();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                if (conn == null)
                {
                    conn = new SqlCeConnection(connString);
                    conn.Open();
                }

                SqlCeCommand cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT * FROM banklogs";
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    System.Diagnostics.Debug.WriteLine(String.Format("{0} {1} {2} {3} {4} {5} {6} {7}",
                        reader[0],reader[1],reader[2],reader[3],reader[4],reader[5],reader[6],reader[7]));
                }
            }
            catch (Exception E)
            {
                MessageBox.Show(E.Message);
                conn.Close();
                conn = null;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "") return;
            string txt = textBox1.Text;

            Button btn = new Button();
//            btn.Width = flowLayoutPanel1.DisplayRectangle.Width - 20;
//            btn.Width = flowLayoutPanel1.Width - 20;
            btn.Height = btn.Height + 10;
            btn.Text = txt;

            Padding pad = btn.Margin;
            pad.All = 0;
            btn.Margin = pad;
            btn.Visible = true;

            flowLayoutPanel1.Controls.Add(btn);
        }

        private void ReadNextItem()
        {
            try
            {
                if (conn == null)
                {
                    conn = new SqlCeConnection(connString);
                    conn.Open();
                }

                SqlCeCommand cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT * FROM banklogs WHERE cat IS NULL AND idx>@index;";
                cmd.Parameters.AddWithValue("@index", current_reader != null ? (int)current_reader[0] : 0);
                SqlCeDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    current_reader = reader;
                    dateTimePicker1.Value = (DateTime)current_reader[1];
                    textBox2.Text = current_reader[2].ToString();
                    textBox3.Text = current_reader[3].ToString();
                    textBox4.Text = current_reader[6].ToString();
                    textBox5.Text = current_reader[4].ToString();
                    textBox6.Text = current_reader[5].ToString();
                    textBox7.Text = current_reader[7].ToString();
                }
            }
            catch (Exception E)
            {
                MessageBox.Show(E.Message);

                if (current_reader != null)
                {
                    current_reader.Dispose();
                    current_reader = null;
                }
                conn.Close();
                conn = null;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            ReadNextItem();
        }

        private bool SetCatCurrent(int val)
        {
            if (current_reader == null) return false;
            try
            {
                int index = (int)current_reader[0];

                SqlCeCommand cmd = conn.CreateCommand();
                cmd.CommandText = "UPDATE banklogs SET cat=@cat WHERE idx=@index";
                cmd.Parameters.AddWithValue("@index", index);
                cmd.Parameters.AddWithValue("@cat", val);
                cmd.ExecuteNonQuery();
            }
            catch (Exception E)
            {
                MessageBox.Show(E.Message);
                conn.Close();
                conn.Dispose();
                conn = null;
                current_reader = null;
                return false;
            }

            current_reader = null;
            return true;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (current_reader == null) return;
            if (SetCatCurrent(0))
                ReadNextItem();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (current_reader == null) return;
            if (SetCatCurrent(1))
                ReadNextItem();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (current_reader == null) return;
            if (SetCatCurrent(2))
                ReadNextItem();
        }
    }
}
