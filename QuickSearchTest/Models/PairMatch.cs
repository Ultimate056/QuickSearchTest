using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickSearchTest.Models
{
    /// <summary>
    /// Связь товара с количеством найденных вхождений по  токенам
    /// /// </summary>
    public class PairMatch
    {
        public CompositeKeyPair Key { get; set; }

        public int count { get; set; }

        public PairMatch()
        {

        }

    }
}
