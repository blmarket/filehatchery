using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Layout;
using FileHatchery;

public class ConsoleTextBox : TextBox
{
    public ConsoleTextBox()
    {
    }

    IAutoCompletion m_engine;

    public IAutoCompletion engine
    {
        set
        {
            m_engine = value;
            List<string> cmds = m_engine.Commands;
            AutoCompleteStringCollection src = new AutoCompleteStringCollection();
            foreach (string str in cmds) src.Add(str);
            AutoCompleteCustomSource = src;
        }
    }

    private string auto_complete(string current)
    {
        List<string> cmds = m_engine.Commands;

        string ret = null;

        foreach(string str in cmds)
        {
            if (str.StartsWith(current) )
            {
                if (ret == null) ret = str;
                else
                {
                    if (str.Length < ret.Length)
                        ret.Remove(str.Length);
                    while ( str.StartsWith(ret) == false && ret.Length > 0)
                    {
                        ret = ret.Remove(ret.Length - 1);
                    }
                }
            }
        }

        if (ret == null) return current;
        return ret;        
    }

    protected override void OnPreviewKeyDown(PreviewKeyDownEventArgs e)
    {
        if (e.KeyCode == Keys.Tab)
        {
            e.IsInputKey = true;

            string cur = auto_complete(Text);
            if (cur.Equals(Text) == false)
            {
                Text = cur;
                this.Select(Text.Length, 0);
            }
            return;
        }
        base.OnPreviewKeyDown(e);
    }
};