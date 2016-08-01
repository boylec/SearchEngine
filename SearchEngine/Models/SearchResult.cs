using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SearchEngine.Models
{
    public class SearchResult
    {
        public int Count { get; set; }
        public IEnumerable<string> Urls { get; set; }
    }
}