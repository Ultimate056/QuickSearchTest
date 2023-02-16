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
        DataTable rawDataTable;

        public Form1()
        {
            InitializeComponent();

            //Заполняем табличку на форме видами товара
            _connectionString = @"Server=DBSRV\DBSRV;Database=test;Integrated Security=SSPI;Connect Timeout=600";
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

        //Токенезируем все наименования в исходных данных
        private void button1_Click(object sender, EventArgs e)
        {
            _tokensList = new List<PairToken>();
            foreach(DataRow row in rawDataTable.Rows)
            {
                int idTov = Convert.ToInt32(row.ItemArray[3]);
                string normalizedNameTov = NormalizeString(row.ItemArray[1].ToString());
                string normalizedBrand = NormalizeString(row.ItemArray[2].ToString());
                string ConcatName = normalizedNameTov + normalizedBrand;
                List<string> tokenByConcatName= TokenizeWord(ConcatName);

                foreach (var token in tokenByConcatName)
                {
                    var pair = new PairToken(idTov, token);
                    _tokensList.Add(pair);
                }


            }

            dataGridView2.DataSource = _tokensList.ToArray();

            label3.Text = $"Общее количество токенов: {_tokensList.Count}";
        }

        private string NormalizeString(string str)
        {
            string res = str.ToLower().DeleteSpaces();
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
                float procent = 0.6f;
                float countTargetTokens = listOfTokens.Count;
                //Ищем по существующим токенам пары с токенами из запроса
                List<PairToken> resultPair = new List<PairToken>();
                resultPair = _tokensList.Join(listOfTokens, t => t._token, x => x, (t, x) =>
                                new PairToken(t._idTov, t._token)).Distinct().OrderBy(x=> x._idTov).ToList();

                //Выделяем все Id из обнаруженных пар
                IEnumerable<int> listOfId = (from id in resultPair
                                             select id._idTov);

                //Подсчитываем количество совпадений токенов по id
                var resultMatch = listOfId.GroupBy(x => x).ToDictionary(x => x.Key, x => x.Count());


                //Выбираем id по максимальному количеству совпадений

                var idMaxMatch = resultMatch.OrderByDescending(x => x.Value)
                                            .Where(x => x.Value / countTargetTokens >= procent);


                //Ищем в датасорсе наименование по id

                List<string> listOfResult = rawDataTable.AsEnumerable()
                                    .Join(idMaxMatch, dt => Int32.Parse(dt.ItemArray[3].ToString()), id => id.Key,
                                    
                                    
                                    (dt, id) => dt[1].ToString() + " " + dt[2].ToString()).ToList();

                string resultName = listOfResult.FirstOrDefault();

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
