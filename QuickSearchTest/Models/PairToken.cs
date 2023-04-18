using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickSearchTest.Models
{
    /// <summary>
    /// Связь токена с id SKU
    /// </summary>
    public class PairToken
    {
        //public int _idTov { get; private set; }

        public CompositeKeyPair Id { get; set; }

        public int idToken { get; set; }

        public string Value { get; set; }


        public PairToken() { }


        public bool IsPriority { get; set; } = false;
    }
}
