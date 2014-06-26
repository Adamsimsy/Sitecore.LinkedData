using System.Diagnostics;
using System.IO;
using System.Web;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using LinkedData.Concepts;
using LinkedData.DataManagers;
using LinkedData.Formatters;
using LinkedData.Website.App_Start;
using Sitecore.ApplicationCenter.Applications;
using VDS.RDF.Storage;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(SitecoreLinkedDataInstaller), "Start")]

namespace LinkedData.Website.App_Start
{
    public static class SitecoreLinkedDataInstaller
    {
        public static void Start()
        {
            var container = new WindsorContainer();

            //container.Register(Component.For<IQueryableStorage>().ImplementedBy<SesameHttpProtocolVersion6Connector>().LifestyleSingleton()
            //    .DependsOn(Dependency.OnValue("baseUri", "http://localhost:8080/openrdf-sesame/"), Dependency.OnValue("storeID", "in-mem-sesame")));

            container.Register(Component.For<IQueryableStorage>().ImplementedBy<InMemoryManager>().LifestyleSingleton());
            container.Register(Component.For<IConceptManager>().ImplementedBy<SitecoreConceptManager>().LifestyleSingleton());
            container.Register(Component.For<IConceptProvider>().ImplementedBy<SitecoreConceptProvider>().LifestyleSingleton());
            container.Register(Component.For<SitecoreLinkedDataManager>().ImplementedBy<SitecoreLinkedDataManager>().LifestyleSingleton());

            DependencyResolver.Instance = container;
        }
    }
}