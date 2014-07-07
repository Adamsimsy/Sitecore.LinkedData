using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinkedData.Concepts;
using LinkedData.DatabaseContext;
using LinkedData.Filters;
using LinkedData.Formatters;
using VDS.RDF.Storage;

namespace LinkedData.DataManagers
{
    public class SitecoreLinkedDataContext
    {
        private readonly List<GraphConfiguration> _graphConfigurations;

        public string DatabaseName { get; set; }

        public SitecoreLinkedDataContext(List<GraphConfiguration> graphConfigurations, string databaseName)
        {
            _graphConfigurations = graphConfigurations;
            DatabaseName = databaseName;

            foreach (var graphConfiguration in _graphConfigurations)
            {
                var manager = new SitecoreLinkedDataManager(
                    graphConfiguration.InFormatters,
                    graphConfiguration.OutFormatters,
                    graphConfiguration.InFilters,
                    DependencyResolver.Instance.Resolve<IQueryableStorage>(),
                    DependencyResolver.Instance.Resolve<IConceptManager>(),
                    graphConfiguration.GraphUri);

                graphConfiguration.Manager = manager;
            }
        }

        public List<GraphConfiguration> GraphConfigurations
        {
            get
            {
                return _graphConfigurations;
            }
        }

        public SitecoreLinkedDataManager GetManager(bool linkDatabase)
        {
            if (linkDatabase)
            {
                return _graphConfigurations.Single(x => x.GraphUri.ToString().ToLower().Contains("link")).Manager;
            }
            else
            {
                return _graphConfigurations.Single(x => !x.GraphUri.ToString().ToLower().Contains("link")).Manager;
            }
        }
    }
}
