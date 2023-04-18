using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickSearchTest.Models
{
    public class MatchEqualityComparer : IEqualityComparer<PairMatch>
    {
        public bool Equals(PairMatch x, PairMatch y)
        {
            return x.Key.IdPair.Equals(y.Key.IdPair) && x.Key.Filter.Equals(y.Key.Filter);
        }

        public int GetHashCode(PairMatch obj)
        {
            return obj.Key.IdPair.GetHashCode();
        }
    }
}
