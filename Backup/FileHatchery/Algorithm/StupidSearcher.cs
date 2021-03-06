﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FileHatchery.Algorithm
{
    class StupidSearcher
    {
        private string curString;

        public void AddChar(IBrowser browser, char chr)
        {
            char newChar = Char.ToLower(chr);
            curString += newChar;
            IBrowserItem cur = browser.Cursor;
            if (cur.showName.ToLower().StartsWith(curString)) return;
            List<IBrowserItem> items = browser.Items;
            foreach (IBrowserItem item in items)
            {
                if (item.showName.ToLower().StartsWith(curString))
                {
                    browser.SelectItem(item);
                    return;
                }
            }
            curString = new string(newChar,1);
            foreach (IBrowserItem item in items)
            {
                if (item.showName.ToLower().StartsWith(curString))
                {
                    browser.SelectItem(item);
                    return;
                }
            }
            curString = "";
        }

        public void Clear()
        {
            curString = "";
        }
    }
}