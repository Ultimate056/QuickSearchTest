using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickSearchTest.Models
{
    public class CompositeKeyPair
    {
        public int IdPair { get; set; }

        public int Filter { get; set; }

        public CompositeKeyPair()
        {

        }

        public CompositeKeyPair(int idUsku, int filter)
        {
            IdPair = idUsku;
            Filter = filter;
        }
    }
}
