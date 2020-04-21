using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmITExchange
{
    public class DataFile
    {
        public DateTime Date { get; set; }
        public int Count { get; set; }
        public List<PageData> Pages { get; set; }
    }
}
