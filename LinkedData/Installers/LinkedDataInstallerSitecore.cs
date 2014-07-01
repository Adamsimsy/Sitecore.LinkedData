using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using LinkedData.Concepts;
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
            
            //register for cms context
            var managerCms = new SitecoreLinkedDataManager(null,
                null,
                null,
                DependencyResolver.Instance.Resolve<IQueryableStorage>(),
                DependencyResolver.Instance.Resolve<IConceptManager>());

            container.Register(Component.For<SitecoreLinkedDataManager>().Instance(managerCms).LifestyleSingleton().Named("cms"));

            //register for web context
            var managerWeb = new SitecoreLinkedDataManager(null,
                new List<ITripleFormatter>() { new UriToDynamicUrlFormatter() },
                new List<IFilter>() { new FilterSitecoreSystemFolders() },
                DependencyResolver.Instance.Resolve<IQueryableStorage>(),
                DependencyResolver.Instance.Resolve<IConceptManager>());

            container.Register(Component.For<SitecoreLinkedDataManager>().Instance(managerWeb).LifestyleSingleton().Named("web"));

            //container.Register(Component.For<IConceptProvider>().ImplementedBy<SitecoreConceptProvider>().LifestyleSingleton());
            //container.Register(Component.For<SitecoreLinkedDataManager>().ImplementedBy<SitecoreLinkedDataManager>().LifestyleSingleton());
        }
    }
}
