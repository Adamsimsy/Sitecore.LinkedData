using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinkedData.DatabaseContext;

namespace LinkedData.DataManagers
{
    public class SitecoreManagerFactory
    {
        private readonly List<DatabaseGraphContext> _contexts;

        public SitecoreManagerFactory(List<DatabaseGraphContext> contexts)
        {
            _contexts = contexts;
        }

        public List<SitecoreLinkedDataManager> GetContextSitecoreLinkedDataManagers(string database)
        {
            return new List<SitecoreLinkedDataManager>();
        }
    }
}
