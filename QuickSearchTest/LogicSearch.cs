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
                    if (subStr[i].Length <= 1 || subStr[i].Trim() == "")
                        continue;
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
        /// optional - 
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public static List<string> TokenizeWord(string word, bool optional = false)
        {
            var listResult = new List<string>();
            int maxLenToken = Program.lengthToken;
            int lenWord = word.Length;



            if (word.ToArray().Length <= maxLenToken)
            {
                listResult.Add(word);
            }
            else
            {
                if (optional)
                {
                    for (int i = 0; i < lenWord; i++)
                    {
                        if (maxLenToken <= lenWord - i)
                        {
                            string str1 = word.Substring(i, maxLenToken);
                            listResult.Add(str1);
                        }
                        if (maxLenToken - 1 <= lenWord - i)
                        {
                            string str2 = word.Substring(i, maxLenToken - 1);
                            listResult.Add(str2);
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < lenWord; i++)
                    {
                        if (maxLenToken <= lenWord - i)
                        {
                            string str1 = word.Substring(i, maxLenToken);
                            listResult.Add(str1);
                        }
                    }
                }

            }

            return listResult;
        }

    }
}
