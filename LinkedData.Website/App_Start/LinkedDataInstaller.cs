using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Web;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using LinkedData.Concepts;
using LinkedData.DataManagers;
using LinkedData.Formatters;
using LinkedData.Installers;
using LinkedData.Website.App_Start;
using Sitecore.ApplicationCenter.Applications;
using VDS.RDF.Storage;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(LinkedDataInstaller), "Start")]

namespace LinkedData.Website.App_Start
{
    public static class LinkedDataInstaller
    {
        public static void Start()
        {
            var container = new WindsorContainer();

            //container.Register(Component.For<IQueryableStorage>().ImplementedBy<SesameHttpProtocolVersion6Connector>().LifestyleSingleton()
            //    .DependsOn(Dependency.OnValue("baseUri", "http://localhost:8080/openrdf-sesame/"), Dependency.OnValue("storeID", "in-mem-sesame")));

            container.Register(Component.For<IQueryableStorage>().ImplementedBy<InMemoryManager>().LifestyleSingleton());

            var concepts = new List<BaseConcept>();

            concepts.Add(new SitecoreTemplateConcept() { SubjectTemplateName = "league", ConceptUri = new Uri("http://football.com/league-to-team"), ObjectTemplateName = "team" });
            concepts.Add(new SitecoreTemplateConcept() { SubjectTemplateName = "team", ConceptUri = new Uri("http://football.com/team-to-player"), ObjectTemplateName = "player" });
   
            container.Register(Component.For<IConceptProvider>().Instance(new SitecoreConceptProvider(concepts)).LifestyleSingleton());

            container.Install(new LinkedDataInstallerSitecore());
        }
    }
}