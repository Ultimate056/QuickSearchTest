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
using QuickSearchTest.Models;
using QuickSearchTest.Database;
using System.Threading;

namespace QuickSearchTest
{
    public partial class Form1 : Form
    {
        
        private Stopwatch timer = new Stopwatch();
        private string sql;

        public Form1()
        {
            InitializeComponent();

            Cursor.Current = Cursors.WaitCursor;
            fillgcUsku();
            generateTokens();

            Cursor.Current = Cursors.Default;

        }

        private SqlParameter sp(string key, object value) => new SqlParameter(key, value);

        private int parse(object value) => Convert.ToInt32(value);



        private void fillgcUsku()
        {
            sql = @"select distinct
                     rUSKU.idusku as idusku, 
                    (select nameusku from dbo.f_getuskuname(rUSKU.idusku)) nameusku
                     from rUSKU(nolock)";
            gcUsku.DataSource = DBExecute.SelectTable(sql);
        }


        List<PairToken> tokens = new List<PairToken>();
        List<string> SearchValueTokens;
        List<DataRow> rowsUSKU => (gcUsku.DataSource as DataTable).AsEnumerable()
                .ToList();

        private void generateTokens()
        {
            DataTable genTokens = new DataTable();
            genTokens.Columns.Add("idToken");
            genTokens.Columns.Add("valueToken");
            genTokens.Columns.Add("idUSKU");
            genTokens.Columns.Add("isPriority");
            genTokens.Columns["idToken"].AutoIncrement = true;
            genTokens.Columns["idToken"].Caption = "ID токена";
            genTokens.Columns["valueToken"].Caption = "Токен";

            try
            {
                int counter = 0;
                foreach (DataRow dr in rowsUSKU)
                {
                    List<string> words = LogicSearch.NormalizeString(dr["nameusku"].ToString());
                    int idusku = parse(dr["idusku"]);
                    foreach (string word in words)
                    {
                        Dictionary<string, bool> valueTokens = LogicSearch.TokenizeWord(word);
                        foreach (var token in valueTokens)
                        {
                            genTokens.Rows.Add(++counter, token.Key, idusku, token.Value);
                        }
                    }
                }


                List<DataRow> rowsToken = genTokens.AsEnumerable().ToList();
                foreach (DataRow dr in rowsToken)
                {
                    tokens.Add(new PairToken
                    {
                        idToken = parse(dr["idToken"]),
                        idUSKU = parse(dr["idUSKU"]),
                        IsPriority = Convert.ToBoolean(dr["isPriority"]),
                        Value = dr["valueToken"].ToString()
                    });
                }


                gcToken.DataSource = genTokens;
            }
            catch(Exception ex)
            {
                MessageBox.Show("Ошибка. " + ex.Message);
            }
     
        }



        private void SearchButton_Click(object sender, EventArgs e)
        {
            timer.Start();
            listBoxControl1.DataSource = null;
            gcResults.DataSource = null;
            #region Формируем токены введенных слов
            SearchValueTokens = new List<string>();
            string searchWord = SearchWordEdit.EditValue.ToString();
            List<string> words = LogicSearch.NormalizeString(searchWord);
            foreach(var word in words)
            { 
                Dictionary<string, bool> valueTokens = LogicSearch.TokenizeWord(word, true);
                foreach(var token in valueTokens)
                {
                    SearchValueTokens.Add(token.Key);
                }
            }
            #endregion

            int searchCountTokens = SearchValueTokens.Count;

            // Выбираем обнаруженные токены
            List<PairToken> findTokens = tokens.Join(SearchValueTokens, x => x.Value, j => j,
                (x, j) => new PairToken { idToken = x.idToken, idUSKU = x.idUSKU, IsPriority = x.IsPriority, Value = x.Value})
                .Distinct().ToList() ;

            // Выбираем кол-во найденных USKU из обнаруженных
            IEnumerable<int> listOfUsku = findTokens
                            .Select(x => x.idUSKU);


            var matches = listOfUsku.GroupBy(x => x).ToDictionary(x => x.Key, x => x.Count())
                .Select(x=> new PairMatch { idUSKU = x.Key, count = x.Value }).OrderByDescending(x=> x.count).ToList();

            List<PairMatch> top = matches.Take(5).ToList();

            if(top == null)
            {
                MessageBox.Show("Ничего не найдено");
                timer.Stop(); timer.Reset();
                return;
            }

            List<string> res = top.Join(rowsUSKU, x => x.idUSKU,
                j => parse(j["idusku"]), (x, j) => j["nameusku"].ToString()).ToList();


            listBoxControl1.DataSource = res;

            timer.Stop();

            timeSearchLabel.Text = "Время поиска " + timer.Elapsed.ToString(@"m\:ss\.fff");
            timer.Reset();
        }

        private void listBoxControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxControl1.SelectedValue == null)
                return;

            DataRow tmp = rowsUSKU.Where(x => x["nameusku"].Equals(listBoxControl1.SelectedValue.ToString())).FirstOrDefault();
            if (tmp == null)
                return;

            int idSelUsku = parse(tmp["idusku"]);


            sql = @"SELECT rel.idtov as idtov, spr_tov_level4.tov_name as vidtov,
                     spr_tm.tm_name as brand, spr_tov.id_tov_oem as articul, spr_tov.n_tov as skuName FROM rLinkSKUwithUSKU as rel (nolock)
                     INNER JOIN spr_tov (nolock) ON spr_tov.id_tov = rel.idtov
                     INNER JOIN spr_tm (nolock) ON spr_tm.tm_id = spr_tov.id_tm
                     INNER JOIN spr_tov_level4 ON spr_tov_level4.tov_id = spr_tov.id_tov4
                     WHERE idUSKU = @idusku";
            var p_usku = sp("idusku", idSelUsku);
            gcResults.DataSource = DBExecute.SelectTable(sql, p_usku);
        }

        private void SearchWordEdit_EditValueChanged(object sender, EventArgs e)
        {

            timer.Start();
            listBoxControl1.DataSource = null;
            gcResults.DataSource = null;
            #region Формируем токены введенных слов
            SearchValueTokens = new List<string>();
            string searchWord = SearchWordEdit.EditValue.ToString();
            List<string> words = LogicSearch.NormalizeString(searchWord);
            foreach (var word in words)
            {
                Dictionary<string, bool> valueTokens = LogicSearch.TokenizeWord(word, true);
                foreach (var token in valueTokens)
                {
                    SearchValueTokens.Add(token.Key);
                }
            }
            #endregion

            int searchCountTokens = SearchValueTokens.Count;

            // Выбираем обнаруженные токены
            List<PairToken> findTokens = tokens.Join(SearchValueTokens, x => x.Value, j => j,
                (x, j) => new PairToken { idToken = x.idToken, idUSKU = x.idUSKU, IsPriority = x.IsPriority, Value = x.Value })
                .Distinct().ToList();

            // Выбираем кол-во найденных USKU из обнаруженных
            IEnumerable<int> listOfUsku = findTokens
                            .Select(x => x.idUSKU);


            var matches = listOfUsku.GroupBy(x => x).ToDictionary(x => x.Key, x => x.Count())
                .Select(x => new PairMatch { idUSKU = x.Key, count = x.Value }).OrderByDescending(x => x.count).ToList();

            List<PairMatch> top = matches.Take(5).ToList();

            if (top == null)
            {
                MessageBox.Show("Ничего не найдено");
                timer.Stop(); timer.Reset();
                return;
            }

            List<string> res = top.Join(rowsUSKU, x => x.idUSKU,
                j => parse(j["idusku"]), (x, j) => j["nameusku"].ToString()).ToList();


            listBoxControl1.DataSource = res;

            timer.Stop();

            timeSearchLabel.Text = "Время поиска " + timer.Elapsed.ToString(@"m\:ss\.fff");
            timer.Reset();
        }

        private void SearchWordEdit_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        {
            Thread.Sleep(10);
        }
    }
}
