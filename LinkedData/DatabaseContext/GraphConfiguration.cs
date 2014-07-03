using System.Collections.Generic;
using LinkedData.Formatters;

namespace LinkedData.DatabaseContext
{
    public class GraphConfiguration
    {
        public string GraphUri { get; set; }
        public IEnumerable<ITripleFormatter> InFormatters { get; set; }
        public IEnumerable<ITripleFormatter> OutFormatters { get; set; } 
    }
}
