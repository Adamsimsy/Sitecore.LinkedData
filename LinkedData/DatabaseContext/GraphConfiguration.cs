using System;
using System.Collections.Generic;
using LinkedData.DataManagers;
using LinkedData.Filters;
using LinkedData.Formatters;

namespace LinkedData.DatabaseContext
{
    public class GraphConfiguration
    {
        public Uri GraphUri { get; set; }
        public List<ITripleFormatter> InFormatters { get; set; }
        public List<ITripleFormatter> OutFormatters { get; set; } 
        public List<IFilter> OutFilters { get; set; } 
        public SitecoreLinkedDataManager Manager { get; set; }
    }
}
