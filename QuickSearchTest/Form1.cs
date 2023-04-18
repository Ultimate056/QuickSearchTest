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



        // Cловарь
        private void fillgcUsku()
        {
            sql = @" select tov_id as idusku, 
                 tov_name as nameusku, 
                 1 as filter 
                 from spr_tov_level4 (nolock) WHERE spr_tov_level4.tov_id_top_level <> 0
                 UNION
                 select tm_id as idusku, tm_name as nameusku, 2 as filter from spr_tm (nolock)
                 UNION
                 select distinct
                rUSKU.idusku as idusku, 
                (select nameusku from dbo.f_getuskuname(rUSKU.idusku)) as nameusku,
                3 as filter
                from rUSKU(nolock)
                UNION
                SELECT rel.idUSKU as idusku, 
                rel.id_tov_oem as nameusku, 
                4 as filter
                FROM rAUSKU (nolock) as rel
						                INNER JOIN spr_tov (nolock) ON spr_tov.id_tov_oem = rel.id_tov_oem and spr_tov.id_tm = rel.id_brand
						                INNER JOIN spr_tov_level4 stl4 (nolock) ON stl4.tov_id = spr_tov.id_tov4
	                                    INNER JOIN spr_tm (nolock) ON spr_tm.tm_id = spr_tov.id_tm
                ORDER BY filter";
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
            genTokens.Columns.Add("filter");

            genTokens.Columns["idToken"].Caption = "ID токена";
            genTokens.Columns["valueToken"].Caption = "Токен";
            
            try
            {
                int counter = 0;
                foreach (DataRow dr in rowsUSKU)
                {
                    List<string> words = LogicSearch.NormalizeString(dr["nameusku"].ToString());
                    int idusku = parse(dr["idusku"]);
                    int filter = parse(dr["filter"]);
                    foreach (string word in words)
                    {
                        List<string> valueTokens = LogicSearch.TokenizeWord(word);
                        foreach (var token in valueTokens)
                        {
                            genTokens.Rows.Add(++counter, token, idusku, filter);
                        }
                    }
                }


                List<DataRow> rowsToken = genTokens.AsEnumerable().ToList();
                foreach (DataRow dr in rowsToken)
                {
                    tokens.Add(new PairToken
                    {
                        idToken = parse(dr["idToken"]),
                        Value = dr["valueToken"].ToString(),
                        Id = new CompositeKeyPair(parse(dr["idUSKU"]), parse(dr["filter"]))
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
            gcMatches.DataSource = null;
            gcResults.DataSource = null;
            #region Формируем токены введенных слов
            SearchValueTokens = new List<string>();
            string searchWord = SearchWordEdit.EditValue.ToString();
            List<string> words = LogicSearch.NormalizeString(searchWord);
            foreach (var word in words)
            {
                List<string> valueTokens = LogicSearch.TokenizeWord(word);
                foreach (var token in valueTokens)
                {
                    SearchValueTokens.Add(token);
                }
            }
            #endregion

            int searchCountTokens = SearchValueTokens.Count;

            // Выборка всех обнаруженных токенов
            List<PairToken> findTokens = tokens.Join(SearchValueTokens, x => x.Value, j => j,
                (x, j) => new PairToken { idToken = x.idToken, Id = x.Id, Value = x.Value })
                .ToList();

            // Выбираем id usku обнаруженных токенов
            var listOfUsku = findTokens.Select(x => new { x.Id.IdPair, x.Id.Filter });

            var Dict = listOfUsku.GroupBy(x => x).ToDictionary(x => x.Key, x => x.Count());
            // По id создаем словарь : id - кол-во и отсортировываем по убыванию

            List<PairMatch> matches = Dict.Select(x => new PairMatch
            {
                Key = new CompositeKeyPair { IdPair = x.Key.IdPair, Filter = x.Key.Filter},
                count = x.Value
            }).Distinct(new MatchEqualityComparer()).ToList();


            // Берем первые совпадения по кол-ву найденных токенов
            List<PairMatch> top = matches.OrderByDescending(x=> x.count).Take(15).ToList();


            DataTable dtRes = new DataTable();
            dtRes.Columns.Add("idUSKU");
            dtRes.Columns.Add("filter");
            dtRes.Columns.Add("nameMatch");

            foreach (PairMatch match in top)
            {
                string nameMatch = rowsUSKU.Where(x => parse(x["idusku"]).Equals(match.Key.IdPair) && parse(x["filter"]).Equals(match.Key.Filter))
                    .Select(x => x["nameusku"].ToString()).FirstOrDefault();

                dtRes.Rows.Add(match.Key.IdPair, match.Key.Filter, nameMatch);
            }

            // Заполняем найденные совпадения
            gcMatches.DataSource = dtRes;

            timer.Stop();

            timeSearchLabel.Text = "Время поиска " + timer.Elapsed.ToString(@"m\:ss\.fff");
            timer.Reset();
        }


        private void SearchWordEdit_EditValueChanged(object sender, EventArgs e)
        {

            //timer.Start();
            //listBoxControl1.DataSource = null;
            //gcResults.DataSource = null;
            //#region Формируем токены введенных слов
            //SearchValueTokens = new List<string>();
            //string searchWord = SearchWordEdit.EditValue.ToString();
            //List<string> words = LogicSearch.NormalizeString(searchWord);
            //foreach (var word in words)
            //{
            //    Dictionary<string, bool> valueTokens = LogicSearch.TokenizeWord(word, true);
            //    foreach (var token in valueTokens)
            //    {
            //        SearchValueTokens.Add(token.Key);
            //    }
            //}
            //#endregion

            //int searchCountTokens = SearchValueTokens.Count;

            //// Выбираем обнаруженные токены
            //List<PairToken> findTokens = tokens.Join(SearchValueTokens, x => x.Value, j => j,
            //    (x, j) => new PairToken { idToken = x.idToken, idUSKU = x.idUSKU, IsPriority = x.IsPriority, Value = x.Value })
            //    .Distinct().ToList();

            //// Выбираем кол-во найденных USKU из обнаруженных
            //IEnumerable<int> listOfUsku = findTokens
            //                .Select(x => x.idUSKU);


            //var matches = listOfUsku.GroupBy(x => x).ToDictionary(x => x.Key, x => x.Count())
            //    .Select(x => new PairMatch { idUSKU = x.Key, count = x.Value }).OrderByDescending(x => x.count).ToList();

            //List<PairMatch> top = matches.Take(5).ToList();

            //if (top == null)
            //{
            //    timer.Stop(); timer.Reset();
            //    return;
            //}

            //List<string> res = top.Join(rowsUSKU, x => x.idUSKU,
            //    j => parse(j["idusku"]), (x, j) => j["nameusku"].ToString()).ToList();


            //listBoxControl1.DataSource = res;

            //timer.Stop();

            //timeSearchLabel.Text = "Время поиска " + timer.Elapsed.ToString(@"m\:ss\.fff");
            //timer.Reset();
        }

        private void SearchWordEdit_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        {
            //Thread.Sleep(10);
        }

        private void gvMatches_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {
            if (gvMatches.FocusedRowHandle < 0)
                return;

            try
            {
                gcResults.DataSource = null;
                DataRow rowSel = gvMatches.GetFocusedDataRow();

                int filter = parse(rowSel["filter"]);
                int idSelUsku = parse(rowSel["idUSKU"]);

                switch (filter)
                {
                    case 1:
                        sql = @"SELECT spr_tm.tm_id as id_brand, 
                                spr_tov.id_tov as idTov,
                                spr_tov_level4.tov_name as vidtov,
                                spr_tm.tm_name as brandName,
                                spr_tov.id_tov_oem as artikul,
                                spr_tov.n_tov as nameTov
                                FROM spr_tov (nolock)
                                INNER JOIN spr_tm (nolock) ON spr_tov.id_tm = spr_tm.tm_id
                                INNER JOIN spr_tov_level4 (nolock) ON spr_tov_level4.tov_id = spr_tov.id_tov4 
                                WHERE id_tov4 = @idusku";
                        break;
                    case 2:
                        sql = @"SELECT spr_tm.tm_id as id_brand, 
                            spr_tov.id_tov as idTov,
                            spr_tov_level4.tov_name as vidtov,
                            spr_tm.tm_name as brandName,
                            spr_tov.id_tov_oem as artikul,
                            spr_tov.n_tov as nameTov
                            FROM spr_tov (nolock)
                            INNER JOIN spr_tm (nolock) ON spr_tov.id_tm = spr_tm.tm_id
                            INNER JOIN spr_tov_level4 (nolock) ON spr_tov_level4.tov_id = spr_tov.id_tov4 
                            WHERE id_tm = @idusku";
                        break;
                    case 3:
                        sql = @"SELECT rel.idtov as idtov, spr_tov_level4.tov_name as vidtov,
                     spr_tm.tm_name as brand, spr_tov.id_tov_oem as articul, spr_tov.n_tov as skuName FROM rLinkSKUwithUSKU as rel (nolock)
                     INNER JOIN spr_tov (nolock) ON spr_tov.id_tov = rel.idtov
                     INNER JOIN spr_tm (nolock) ON spr_tm.tm_id = spr_tov.id_tm
                     INNER JOIN spr_tov_level4 ON spr_tov_level4.tov_id = spr_tov.id_tov4
                     WHERE idUSKU = @idusku";
                        break;
                    case 4:
                        sql = @"
                            SELECT 
                            rel.idtov as idTov,
                            spr_tov_level4.tov_name as vidtov,
                            spr_tm.tm_name as brand,
                            usku.id_tov_oem as artikul,
                            spr_tov.n_tov as nameTov
                            FROM rLinkSKUwithUSKUofArticul  as rel (nolock)
                            INNER JOIN rAUsku as usku ON usku.idUSKU = rel.idUSKU
                            INNER JOIN spr_tov ON spr_tov.id_tov = rel.idtov
                            INNER JOIN spr_tov_level4 ON spr_tov_level4.tov_id = spr_tov.id_tov4
                            INNER JOIN spr_tm ON spr_tm.tm_id = usku.id_brand
                            WHERE rel.idUSKU = @idusku";
                        break;


                }
                
                var p_usku = sp("idusku", idSelUsku);
                gcResults.DataSource = DBExecute.SelectTable(sql, p_usku);
                gvResults.PopulateColumns();
            }
            catch(Exception ex)
            {
                MessageBox.Show("RowClick: " + ex.Message);
            }
        }
    }
}
