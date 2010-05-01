using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FileHatchery.Algorithm
{
    /// <summary>
    /// 간단한 이름 검색 알고리즘이다.
    /// </summary>
    /// <example>
    /// NameSearcher ns = new NameSearcher();
    /// ns.SetData(some Sorted Enumerable);
    /// newidx = ns.PutKey(some Keys);
    /// </example>
    class NameSearcher
    {
        private string curStr;
        private string[] sortedArray;

        /// <summary>
        /// 검색해야 할 대상들을 입력한다.
        /// </summary>
        /// <param name="data">검색대상</param>
        public void SetData(IEnumerable<string> data)
        {
            List<string> list = new List<string>(data);
            list.Sort();
            sortedArray = list.ToArray();
        }

        /// <summary>
        /// 새로운 키 입력이 들어왔음을 알린다.
        /// </summary>
        /// <param name="kdata">새로운 키값</param>
        /// <returns>검색된 index 값</returns>
        public int PutKey(Keys kdata)
        {
            char val = (char)kdata;
            curStr += val;
            int tmp = Array.BinarySearch(sortedArray, curStr);

            return tmp;
        }
    }
}
