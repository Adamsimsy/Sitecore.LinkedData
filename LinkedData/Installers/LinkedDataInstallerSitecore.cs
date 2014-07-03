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

            var coreDatabaseContext = new DatabaseContext.DatabaseContext()
            {
                DatabaseName = "core", GraphConfigurations = 
                new List<GraphConfiguration>()
                {
                    new GraphConfiguration()
                    {
                        GraphUri = "http://sitecore.net/core-link-graph",
                        InFormatters = null,
                        OutFormatters = null
                    }
                }
            };

            var masterDatabaseContext = new DatabaseContext.DatabaseContext()
            {
                DatabaseName = "master",
                GraphConfigurations =
                    new List<GraphConfiguration>()
                {
                    new GraphConfiguration()
                    {
                        GraphUri = "http://sitecore.net/master-link-graph",
                        InFormatters = null,
                        OutFormatters = null
                    },
                    new GraphConfiguration()
                    {
                        GraphUri = "http://sitecore.net/master-graph",
                        InFormatters = null,
                        OutFormatters = null
                    }
                }
            };

            var webDatabaseContext = new DatabaseContext.DatabaseContext()
            {
                DatabaseName = "web",
                GraphConfigurations =
                    new List<GraphConfiguration>()
                {
                    new GraphConfiguration()
                    {
                        GraphUri = "http://sitecore.net/web-link-graph",
                        InFormatters = null,
                        OutFormatters = null
                    },
                    new GraphConfiguration()
                    {
                        GraphUri = "http://sitecore.net/web-graph",
                        InFormatters = null,
                        OutFormatters = null
                    }
                }
            };

            container.Register(Component.For<DatabaseContext.DatabaseContext>().Instance(coreDatabaseContext).LifestyleSingleton().Named("core"));
            container.Register(Component.For<DatabaseContext.DatabaseContext>().Instance(masterDatabaseContext).LifestyleSingleton().Named("master"));
            container.Register(Component.For<DatabaseContext.DatabaseContext>().Instance(webDatabaseContext).LifestyleSingleton().Named("web2"));

            container.Register(Component.For<IConceptManager>().ImplementedBy<SitecoreConceptManager>().LifestyleSingleton());
            
            //register for cms context
            var managerCms = new SitecoreLinkedDataManager(null,
                null,
                null,
                DependencyResolver.Instance.Resolve<IQueryableStorage>(),
                DependencyResolver.Instance.Resolve<IConceptManager>());

            container.Register(Component.For<SitecoreLinkedDataManager>().Instance(managerCms).LifestyleSingleton().Named("cms"));

            //register for web context
            var managerWeb = new SitecoreLinkedDataManager(null,
                //new List<ITripleFormatter>() { new UriToDynamicUrlFormatter() },
                null,
                new List<IFilter>() { new FilterSitecoreSystemFolders() },
                DependencyResolver.Instance.Resolve<IQueryableStorage>(),
                DependencyResolver.Instance.Resolve<IConceptManager>());

            container.Register(Component.For<SitecoreLinkedDataManager>().Instance(managerWeb).LifestyleSingleton().Named("web"));

            //container.Register(Component.For<IConceptProvider>().ImplementedBy<SitecoreConceptProvider>().LifestyleSingleton());
            //container.Register(Component.For<SitecoreLinkedDataManager>().ImplementedBy<SitecoreLinkedDataManager>().LifestyleSingleton());
        }
    }
}
