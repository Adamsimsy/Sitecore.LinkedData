using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinkedData.Concepts;
using LinkedData.Filters;
using LinkedData.Formatters;
using VDS.RDF.Storage;

namespace LinkedData.DataManagers
{
    public class SitecoreLinkedDataContext : SitecoreLinkedDataManager
    {
        public SitecoreLinkedDataContext(IQueryableStorage store, IConceptManager conceptManager) : base(store, conceptManager)
        {
        }

        public SitecoreLinkedDataContext(List<ITripleFormatter> inFormatters, List<ITripleFormatter> outFormatters, List<IFilter> outFilters, IQueryableStorage store, IConceptManager conceptManager) : base(inFormatters, outFormatters, outFilters, store, conceptManager)
        {
        }
    }
}
