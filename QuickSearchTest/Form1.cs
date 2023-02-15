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

            public PairToken(int idTov, string tok)
            {
                _idTov = idTov;
                _token = tok;
            }
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
        List<PairToken> _tokensList;
        int _lengthOfToken=3;
        Stopwatch _timer;
        

        public Form1()
        {
            InitializeComponent();

            //Заполняем табличку на форме видами товара
            _connectionString = @"Server=DBSRV\DBSRV;Database=test;Integrated Security=SSPI;Connect Timeout=600";
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string Query = @"select  st.id_tov, st.n_tov 
                                        from spr_tov_level4 l4(nolock)
										inner join spr_tov st(nolock) on st.id_tov4 =l4.tov_id
                                        where tov_id_top_level!=0
                                        order by tov_id_top_level
                                        ";

                DataSet ds = new DataSet();
                SqlDataAdapter da = new SqlDataAdapter(Query, connection);
                da.SelectCommand.CommandTimeout = 0;
                da.Fill(ds);
 
                dataGridView1.DataSource=  ds.Tables[0];

                connection.Close();

                _timer = new Stopwatch();
            }
        }

        //Токенезируем все наименования в исходных данных
        private void button1_Click(object sender, EventArgs e)
        {
            _tokensList = new List<PairToken>();
            DataTable rawDataTable = (DataTable)dataGridView1.DataSource;

            foreach(DataRow row in rawDataTable.Rows)
            {
                int idTov = Convert.ToInt32(row.ItemArray.First());
                string normalizedNameTov = NormalizeString(row.ItemArray.Last());
                List<string> tokenByWord = TokenizeWord(normalizedNameTov);

                foreach(var token in tokenByWord)
                {
                    try
                    {
                        var pair = new PairToken(idTov, token);
                        _tokensList.Add(pair);
                    }
                    catch(Exception ex)
                    {
                    }
                }
            }

            dataGridView2.DataSource = _tokensList.ToArray();

            label3.Text = $"Общее количество токенов: {_tokensList.Count}";
        }

        private string NormalizeString(object str)
        {        
            string res= str.ToString().Trim().ToLower();
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
                    //char[] charArray = str.ToCharArray();
                    //Array.Reverse(charArray);
                    //listOfResult.Add(new string(charArray));
                }
            }

            return listOfResult;
        }

        //Выполнить поиск по слову из ввода
        private void button2_Click(object sender, EventArgs e)
        {
            if (_tokensList != null)
            {
                _timer.Start();

                //Нормализуем ввод и разбиваем его на токены
                var word = NormalizeString(textBox1.Text);
                var listOfTokens = TokenizeWord(word);

                //Ищем по существующим токенам пары с токенами из запроса
                List<PairToken> resultPair = new List<PairToken>();
                resultPair = (from t in _tokensList
                              join l in listOfTokens on t._token equals l
                              orderby t._idTov
                              select t).ToList();

                //Выделяем все Id из обнаруженных пар
                IEnumerable<int> listOfId = (from id in resultPair
                                             select id._idTov);

                //Подсчитываем количество совпадений токенов по id
                var resultMatch = listOfId.GroupBy(x => x).ToDictionary(x => x.Key, x => x.Count());


                //Выбираем id по максимальному количеству совпадений
                var idMaxMatch = (from m in resultMatch
                                  orderby m.Value descending
                                  select m).Take(10);

                //Ищем в датасорсе наименование по id
                DataTable rawDataTable = (DataTable)dataGridView1.DataSource;
                string resultName = "";

                List<string> listOfResult = (from id in rawDataTable.AsEnumerable()
                                             join match in idMaxMatch on id.ItemArray.FirstOrDefault().ToString() equals match.Key.ToString()
                                             select id
                                                      .ItemArray
                                                      .LastOrDefault()
                                                      .ToString())
                                                           .ToList();

                resultName = listOfResult.FirstOrDefault();

                //Выводим id на форму
                label1.Text = resultName;
                listBox1.DataSource = listOfResult;



                _timer.Stop();

                TimeSpan timeTaken = _timer.Elapsed;
                label2.Text = "Time taken: " + timeTaken.ToString(@"m\:ss\.fff");
                _timer.Reset();
            }
            else
                MessageBox.Show("Сначала надо токенизировать слово");
            
        }

        private void listBox1_Enter(object sender, EventArgs e)
        {
            var item= listBox1.SelectedItem.ToString();
            textBox1.Text = item;
        }
    }
}
