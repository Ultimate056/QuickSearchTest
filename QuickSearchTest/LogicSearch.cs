using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickSearchTest
{
    /// <summary>
    /// Класс, определяющий что имел ввиду пользователь, когда вбил слово в input
    /// </summary>
    public static class LogicSearch
    {
       
        private static readonly char[] RULetters = { 'а', 'я', 'у', 'ю', 'о', 'е', 'ё', 'э', 'и', 'ы', 'ь', 'ъ' };
        private static readonly char[] Numbers = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
        private static readonly char[] Special = { '/', '.', '-', ' ', '\\', '|', '&', '^', '*', ';', ':', '>', '<', ',', '+', '"', '?', '=', '(', ')', ' ' };

        public static List<string> NormalizeString(string str, bool optional = false)
        {
            // Разбиваем U-SKU по пробелам
            List<string> result = new List<string>();
            string mainStr = str.ToLower();

            string[] subStr = mainStr.Trim().Split(' ');
            if(subStr.Length > 0)
            {
                for (int i = 0; i < subStr.Length; i++)
                {
                    string subValue = DelSymbols(subStr[i], Special);
                    subValue = DelSymbols(subValue, RULetters);

                    result.Add(subValue);
                }
            }
            else
            {
                string subValue = DelSymbols(str, Special);
                subValue = DelSymbols(subValue, RULetters);
                result.Add(subValue);
            }



            return result;
        }

        public static string DelSymbols(string s, char[] symbols, params char[] parOptional)
        {
            foreach (var symbol in symbols)
                s = s.Replace(symbol.ToString(), "");


            if (parOptional.Length > 0)
            {
                foreach (var p in parOptional)
                    s = s.Replace(p.ToString(), "");
            }


            return s;
        }


        /// <summary>
        /// Разбивает слово на токены
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public static Dictionary<string, bool> TokenizeWord(string word, bool optional = false)
        {
            var listResult = new Dictionary<string, bool>();

            if (word.ToArray().Length <= Program.lengthToken)
            {
                listResult.Add(word, true);
            }
            else
            {

                int le = Program.lengthToken;
                int wle = word.Length;
                if (optional)
                {
                    for (int i = 0; i < wle; i++)
                    {
                        if (le <= wle - i)
                        {
                            string str1 = word.Substring(i, le);
                            listResult.Add(str1, false);
                        }
                        if (le - 1 <= wle - i)
                        {
                            string str2 = word.Substring(i, le - 1);
                            listResult.Add(str2, false);
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < wle; i++)
                    {
                        if (le <= wle - i)
                        {
                            string str1 = word.Substring(i, le);
                            listResult.Add(str1, false);
                        }
                    }
                }

            }

            return listResult;
        }

    }
}
