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
        public static string Brand = null;
        public static int CheckWord(string norm)
        {
            int result_case = 0;
            bool isVT = false;
            bool isBrand = false;

            int counterHitsVT = 0;
            // Берем массив чаров
            for (int i = 0; i < norm.Length; i++)
            {
                // Предполагаем, что если введены русские буквы, значит он вписал вид товара
                if (norm[i] >= 'а' && norm[i] <= 'я')
                {
                    isVT = true;
                    counterHitsVT++;
                }
            }

            Brand = ExtractBrand(norm);
            if (Brand != null)
            {
                isBrand = true;
            }
            isVT = counterHitsVT >= 3;

            if (isVT && isBrand)
                result_case = 2;
            else if (isVT && !isBrand)
                result_case = 1;
            else if (!isVT && isBrand)
                result_case = 3;
            else
                result_case = 0;

            return result_case;
        }

        /// <summary>
        /// Извлекаем брэнд если есть
        /// </summary>
        /// <param name="norm"></param>
        /// <returns></returns>
        public static string ExtractBrand(string norm)
        {
            string res = null;
            int id_start_brand = 0;
            int id_end_brand = 0;
            bool f_start_brand = false;
            bool f_end_brand = false;
            int countSuitSymbols = 0;
            for (int i = 0; i < norm.Length; i++)
            {
                // Если бренда нет
                if (!f_start_brand)
                {
                    // Проверяем есть ли латинские буквы, если есть предполагаем что начался бренд
                    if (norm[i] >= 'a' && norm[i] <= 'z')
                    {
                        id_start_brand = i;
                        f_start_brand = true;
                    }
                }
                // Иначе если бренд начался
                else
                {
                    // Если последний символ в слове
                    if (i == norm.Length - 1)
                    {
                        // Проверяем латинский символ или нет
                        if ((norm[i] >= 'a' && norm[i] <= 'z'))
                        {
                            id_end_brand = i;
                            // Если количество лат.символов подряд > 2 , то это бренд
                            countSuitSymbols = id_end_brand - id_start_brand + 1;
                            f_end_brand = countSuitSymbols >= 2;
                        }
                    }
                    // Если не последний символ в слове
                    else
                    {
                        // Проверяем: нелатинский символ
                        if (!(norm[i] >= 'a' && norm[i] <= 'z'))
                        {
                            id_end_brand = i - 1;
                            countSuitSymbols = id_end_brand - id_start_brand + 1;
                            f_end_brand = countSuitSymbols >= 2;
                            break;
                        }
                    }
                }
            }
            if(f_start_brand && f_end_brand)
            {
                res = norm.Substring(id_start_brand, countSuitSymbols);
            }

            return res;
        }


    }
}
