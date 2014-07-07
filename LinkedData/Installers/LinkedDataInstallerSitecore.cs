using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using LinkedData.Concepts;
using LinkedData.DatabaseContext;
using LinkedData.DataManagers;
using LinkedData.Filters;
using LinkedData.Formatters;
using VDS.RDF.Storage;

namespace LinkedData.Installers
{
    public class LinkedDataInstallerSitecore : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            DependencyResolver.Instance = container;

            container.Register(Component.For<IConceptManager>().ImplementedBy<SitecoreConceptManager>().LifestyleSingleton());

            var coreDatabaseConfigurations = new List<GraphConfiguration>()
                {
                    new GraphConfiguration()
                    {
                        
                        GraphUri = new Uri("http://sitecore.net/graph-core-links"),
                        GraphType = GraphType.Links,
                        InFormatters = null,
                        OutFormatters = null
                    }
                };

            var masterDatabaseConfigurations = new List<GraphConfiguration>()
                {
                    new GraphConfiguration()
                    {                     
                        GraphUri = new Uri("http://sitecore.net/graph-master-links"),
                        GraphType = GraphType.Links,
                        InFormatters = null,
                        OutFormatters = null
                    },
                    new GraphConfiguration()
                    {
                        GraphUri = new Uri("http://sitecore.net/graph-master-website"),
                        GraphType = GraphType.Website,
                        InFormatters = new List<ITripleFormatter>() { new StripLinkFieldFormatter()},
                        OutFormatters = null,
                        InFilters = new List<IFilter>() { new FilterSitecoreSystemFolders() }
                    },
                    new GraphConfiguration()
                    {
                        GraphUri = new Uri("http://sitecore.net/graph-master-public"),
                        GraphType = GraphType.Other,
                        InFormatters = new List<ITripleFormatter>() { new UriToDynamicUrlFormatter()},
                        OutFormatters = null,
                        InFilters = new List<IFilter>() { new FilterSitecoreSystemFolders() }
                    }
                };

            var webDatabaseConfigurations = new List<GraphConfiguration>()
                {
                    new GraphConfiguration()
                    {                  
                        GraphUri = new Uri("http://sitecore.net/graph-web-links"),
                        GraphType = GraphType.Links,
                        InFormatters = null,
                        OutFormatters = null
                    },
                    new GraphConfiguration()
                    {
                        GraphUri = new Uri("http://sitecore.net/graph-web-website"),
                        GraphType = GraphType.Website,
                        InFormatters = new List<ITripleFormatter>() { new StripLinkFieldFormatter()},
                        OutFormatters = null
                    },
                    new GraphConfiguration()
                    {
                        GraphUri = new Uri("http://sitecore.net/graph-web-public"),
                        GraphType = GraphType.Other,
                        InFormatters = new List<ITripleFormatter>() { new UriToDynamicUrlFormatter()},
                        OutFormatters = null
                    }
                };

            var contexts = new List<SitecoreLinkedDataContext>();

            contexts.Add(new SitecoreLinkedDataContext(coreDatabaseConfigurations, "core"));
            contexts.Add(new SitecoreLinkedDataContext(masterDatabaseConfigurations, "master"));
            contexts.Add(new SitecoreLinkedDataContext(webDatabaseConfigurations, "web"));

            var factory = new SitecoreManagerFactory(contexts);

            container.Register(Component.For<SitecoreManagerFactory>().Instance(factory).LifestyleSingleton());
        }
    }
}
