using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace QuickSearchTest
{
    public partial class Form1 : Form
    {
        public class PairToken
        {
            public int _idTov { get; private set; }

            public string _token { get; private set; }

            public PairToken(int idTov, string tok, bool priority = false)
            {
                _idTov = idTov;
                _token = tok;
                IsPriority = priority;
            }
            public bool IsPriority { get; set; } = false;
        }

        public class PairMatch
        {
            public int _idTov { get; private set; }

            public int _count { get; private set; }

            public PairMatch(int idTov, int count)
            {
                _idTov = idTov;
                _count = count;
            }
        }

        readonly string _connectionString;
        List<PairToken> _tokensVTBrandList;
        List<PairToken> _tokensBrandList;
        int _lengthOfToken=3;
        Stopwatch _timer;
        DataTable rawDataTable;

        public Form1()
        {
            InitializeComponent();

            //Заполняем табличку на форме видами товара
            _connectionString = @"Server=DBSRV\DBSRV;Database=clon;Integrated Security=SSPI;Connect Timeout=600";
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string Query = @"select distinct stl.tov_id idTov, stl.tov_name tovName, brandName FROM spr_tov_level4 stl (nolock)
                                INNER JOIN (SELECT DISTINCT tov.id_tov4 idTov, brand.tm_name brandName FROM spr_tov tov (nolock) 
			                                INNER JOIN spr_tm brand (nolock) ON tov.id_tm = brand.tm_id) monstr
                                ON monstr.idTov = stl.tov_id
                                where tov_id_top_level <> 0
                                ORDER BY idTov, brandName
                                        ";

                DataSet ds = new DataSet();
                SqlDataAdapter da = new SqlDataAdapter(Query, connection);
                da.SelectCommand.CommandTimeout = 0;
                da.Fill(ds);

                var dt = ds.Tables[0];

                // Добавляем в таблицу икусственные айдишники для работы с токенами
                dt.Columns.Add(new DataColumn("id", typeof(int)));
                int counter = 1;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    dt.Rows[i].SetField(dt.Columns["id"], counter++);
                }
                dataGridView1.DataSource=  ds.Tables[0];

                connection.Close();
                rawDataTable = (DataTable)dataGridView1.DataSource;
                _timer = new Stopwatch();
            }
        }

        /// <summary>
        /// По нажатию кнопки получаем списки токенов
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            _tokensBrandList = new List<PairToken>();
            _tokensVTBrandList = new List<PairToken>();
            foreach (DataRow row in rawDataTable.Rows)
            {
                int idTov = Convert.ToInt32(row.ItemArray[3]);
                string NameTov = row.ItemArray[1].ToString();
                string Brand = row.ItemArray[2].ToString();

                string normalizedNameTov = NormalizeString(NameTov);
                string normalizedBrand = NormalizeString(Brand);
                string ConcatName = normalizedNameTov + normalizedBrand;
                List<string> tokenByConcatName= TokenizeWord(ConcatName);

                foreach (var token in tokenByConcatName)
                {
                    var pair = new PairToken(idTov, token);
                    _tokensVTBrandList.Add(pair);
                }
                // Выделяем полностью бренд как токен
                _tokensVTBrandList.Add(new PairToken(idTov, normalizedBrand, true));

                List<string> tokenByBrand = TokenizeWord(normalizedBrand);

                foreach (var token in tokenByBrand)
                {
                    var pair = new PairToken(idTov, token);
                    _tokensBrandList.Add(pair);
                }
                // Добавляем сам бренд в список токенов и делаем его приоритетным
                _tokensBrandList.Add(new PairToken(idTov, normalizedBrand, true));

            }

            dataGridView2.DataSource = _tokensVTBrandList.ToArray();

            label3.Text = $"Общее количество токенов: {_tokensVTBrandList.Count}";
        }


        /// <summary>
        /// Нормализует входную строку
        /// Optional - false - надо удалять ру буквы, true - не надо
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>

        private string NormalizeString(string str, bool optional = false)
        {
            string res = str.ToLower().DeleteSpaces();
            if(!optional)
                res = DelSymbols(res, 'а', 'я', 'у', 'ю', 'о', 'е', 'ё', 'э', 'и', 'ы','ь','ъ');
            res = DelSymbols(res, '0', '1', '2', '3', '4', '5', '6', '7', '8', '9');
            return DelSymbols(res, '/', '.', '-', ' ', '\\', '|', '&', '^', '*', ';', ':', '>', '<', ',', '+', '"', '?', '=', '(', ')',' ');
        }

        private string DelSymbols(string s, params char[] par)
        {
            foreach (var p in par)
                s = s.Replace(p.ToString(), "");

            return s;
        }

       
        /// <summary>
        /// Разбивает слово на токены
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        private List<string> TokenizeWord(string word)
        {
            var listOfResult = new List<string>();

            if (word.ToArray().Length <= _lengthOfToken)
            {
                listOfResult.Add(word);
            }
            else
            {
                for(int i=0; i< word.ToArray().Length-_lengthOfToken+1;i++)
                {
                    string str = word.Substring(i, _lengthOfToken);
                    listOfResult.Add(str);

                    // Перевернутые токены (старая версия, зачем-непонятно)

                    //char[] charArray = str.ToCharArray();
                    //Array.Reverse(charArray);
                    //listOfResult.Add(new string(charArray));
                }
            }

            return listOfResult;
        }


        /// <summary>
        /// Процедура поиска элемента по вводу пользователем слова
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            // Настройка коэффициента !
            float kfConcate = (float)koeffConcate.Value;
            float kfBrand = (float)koeffBrandOnly.Value;

            if (_tokensVTBrandList != null)
            {
                _timer.Start();

                // Нормализация вводной строки
                var word = NormalizeString(textBox1.Text);
                // Алгоритм, определяющий что имел ввиду пользователь
                switch(LogicSearch.CheckWord(word))
                {
                    // ВТ + Брэнд
                    case 1:
                        Calculate(_tokensVTBrandList, word, kfConcate);
                        break;
                    
                    // Только брэнд
                    case 2:
                        Calculate(_tokensBrandList, word, kfBrand);
                        break;
                    case 0:
                        break;
                }

                _timer.Stop();

                TimeSpan timeTaken = _timer.Elapsed;
                label2.Text = "Time taken: " + timeTaken.ToString(@"m\:ss\.fff");
                _timer.Reset();
            }
            else
                MessageBox.Show("Сначала надо токенизировать словарь");
            
        }

        private void Calculate(List<PairToken> _tokensList, string word, float procent)
        {
            //Нормализуем ввод и разбиваем его на токены

            var listOfTokens = TokenizeWord(word);
            // Добавляем бренд
            if (LogicSearch.Brand != null)
                listOfTokens.Add(LogicSearch.Brand);
            float countTargetTokens = listOfTokens.Count;
            //Ищем по существующим токенам пары с токенами из запроса
            List<PairToken> resultPair = new List<PairToken>();
            resultPair = _tokensList.Join(listOfTokens, t => t._token, x => x, (t, x) =>
                            new PairToken(t._idTov, t._token, t.IsPriority))
                            .Distinct().OrderBy(x => x._idTov).ToList();


            //Выделяем все Id из обнаруженных пар
            IEnumerable<int> listOfId = resultPair
                            .Select(x => x._idTov);

            bool isHavePriority = false;
            IEnumerable<int> listOfPriorityId = new List<int>();
            Dictionary<int, int> resultPriorityMatch = null;
            IEnumerable<int> idPriorityMaxMatch = null;



            listOfPriorityId = resultPair
                .Where(x => x.IsPriority == true).Select(x => x._idTov);
            if (listOfPriorityId.Count() > 0)
                isHavePriority = true;

            //Подсчитываем количество совпадений токенов по id
            Dictionary<int,int> resultMatch = listOfId.GroupBy(x => x).
                ToDictionary(x => x.Key, x => x.Count());

            // Подсчитываем количество совпадений по приоритетным полям
            if (isHavePriority)
            {
                resultPriorityMatch = listOfPriorityId.GroupBy(x => x).
                ToDictionary(x => x.Key, x => x.Count());
            }

            //Выбираем id по максимальному количеству совпадений

            IEnumerable<int> idMaxMatch = resultMatch
                .Where(x => x.Value / countTargetTokens >= procent)
                .OrderByDescending(x => x.Value)
                .Select(x => x.Key)
                .Take(10);

            // Выбираем id по количеству совпадений приоритетов
            if(isHavePriority)
            {
                idPriorityMaxMatch = resultPriorityMatch
                .OrderByDescending(x => x.Value)
                .Select(x=> x.Key) ;
            }

            IEnumerable<int> finalMaxMatch = null;
            if (isHavePriority)
            {
                finalMaxMatch = idMaxMatch.Intersect(idPriorityMaxMatch);
            }
            else
            {
                finalMaxMatch = idMaxMatch;
            }


            //Ищем в датасорсе наименование по id

            List<string> res = new List<string>(); 

            foreach(var item in finalMaxMatch)
            {
                DataRow dr = rawDataTable.AsEnumerable()
                    .Where(x => x.ItemArray[3].ToString() == item.ToString())
                    .FirstOrDefault();
                if (dr != null)
                {
                    res.Add($"{dr.ItemArray[1]}  {dr.ItemArray[2]}");
                }
            }

            string resultName = res.FirstOrDefault();

            //Выводим id на форму
            label1.Text = resultName;
            listBox1.DataSource = res;
        }

        private void listBox1_Enter(object sender, EventArgs e)
        {
            var item= listBox1.SelectedItem.ToString();
            textBox1.Text = item;
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
    }
}
