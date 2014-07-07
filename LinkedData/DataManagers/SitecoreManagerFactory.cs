using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinkedData.DatabaseContext;
using Sitecore.Data.Items;

namespace LinkedData.DataManagers
{
    public class SitecoreManagerFactory
    {
        private readonly List<SitecoreLinkedDataContext> _contexts;

        public SitecoreManagerFactory(List<SitecoreLinkedDataContext> contexts)
        {
            _contexts = contexts;
        }

        public List<SitecoreLinkedDataManager> GetContextSitecoreLinkedDataManagers(Item contextItem)
        {
            return GetContextSitecoreLinkedDataManagers(contextItem.Database.Name);
        }

        public List<SitecoreLinkedDataManager> GetContextSitecoreLinkedDataManagers(string database)
        {
            var graphConfigurations = _contexts.Single(x => x.DatabaseName.ToLower() == database.ToLower()).GraphConfigurations;

            return graphConfigurations.Select(x => x.Manager).ToList();
        }

        public SitecoreLinkedDataManager GetContextLinkDatabaseDataManager(Item item)
        {
            var graphConfigurations = _contexts.Single(x => x.DatabaseName.ToLower() == item.Database.Name.ToLower()).GraphConfigurations;

            var configuration = graphConfigurations.Single(x => x.GraphType.Equals(GraphType.Links));

            return configuration.Manager;
        }

        public SitecoreLinkedDataManager GetContextWebDatabaseDataManager(Item item)
        {
            var graphConfigurations = _contexts.Single(x => x.DatabaseName.ToLower() == item.Database.Name.ToLower()).GraphConfigurations;

            var configuration = graphConfigurations.Single(x => x.GraphType.Equals(GraphType.Website));

            return configuration.Manager;
        }

        public SitecoreLinkedDataManager GetContextWebDatabaseDataManager()
        {
            return GetContextWebDatabaseDataManager(Sitecore.Context.Item);
        }
    }
}
