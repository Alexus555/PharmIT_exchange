using System;
using System.Collections.Generic;

namespace PharmITExchange
{
    public class DataFile
    {
        public DateTime Date { get; set; }
        public DateTime Month { get; set; }
        public int Count { get; set; }
        public List<PageData> Pages { get; set; }
    }
}
