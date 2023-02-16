using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickSearchTest
{
    public static class LogicSearch
    {
        public static int CheckWord(string norm)
        {
            int custom_case = 0;

            int id_start_brand = 0;
            int id_end_brand = 0;
            bool f_start_brand = false;
            bool f_end_brand = false;
            bool isVT = false;
            bool isBrand = false;
            for (int i = 0; i < norm.Length; i++)
            {
                if (norm[i] >= 'а' && norm[i] <= 'я')
                    isVT = true;
                if (!f_start_brand)
                {
                    if (norm[i] >= 'a' && norm[i] <= 'z')
                    {
                        id_start_brand = i;
                        f_start_brand = true;
                    }
                }
                else
                {
                    if ((norm[i] < 'a' && norm[i] > 'z') || i == norm.Length - 1)
                    {
                        id_end_brand = i;
                        f_end_brand = true;
                        break;
                    }
                }
            }
            isBrand = f_start_brand && f_end_brand;

            if (isVT && isBrand)
                custom_case = 2;
            else if (isVT && !isBrand)
                custom_case = 1;
            else if (!isVT && isBrand)
                custom_case = 3;
            else
                custom_case = 0;

            return custom_case;
        }



    }
}
