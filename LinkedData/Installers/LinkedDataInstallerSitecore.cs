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

namespace LinkedData.Installers
{
    public class LinkedDataInstallerSitecore : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<IConceptManager>().ImplementedBy<SitecoreConceptManager>().LifestyleSingleton());
            //container.Register(Component.For<IConceptProvider>().ImplementedBy<SitecoreConceptProvider>().LifestyleSingleton());
            container.Register(Component.For<SitecoreLinkedDataManager>().ImplementedBy<SitecoreLinkedDataManager>().LifestyleSingleton());
            DependencyResolver.Instance = container;
        }
    }
}
