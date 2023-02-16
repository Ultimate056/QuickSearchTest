using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickSearchTest
{
    public static class DelSpaces
    {
        public static string DeleteSpaces(this string str)
        {
            string res = str.Trim();
            for(int i = 0; i < res.Length; i++)
            {
                if (res[i] == ' ')
                {
                    res = res.Remove(i, 1);
                    i--;
                }
            }
            return res;
        }
    }
}
