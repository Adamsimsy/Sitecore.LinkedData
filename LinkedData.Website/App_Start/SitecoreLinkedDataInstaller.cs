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

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(SitecoreLinkedDataInstaller), "Start")]

namespace LinkedData.Website.App_Start
{
    public static class SitecoreLinkedDataInstaller
    {
        public static void Start()
        {
            var container = new WindsorContainer();

            //container.Register(Component.For<IQueryableStorage>().ImplementedBy<SesameHttpProtocolVersion6Connector>().LifestyleSingleton()
            //.DependsOn(Dependency.OnValue("baseUri", "http://server_name:8080/openrdf-sesame"), Dependency.OnValue("storeID", "repository_name")));

            container.Register(Component.For<IQueryableStorage>().ImplementedBy<InMemoryManager>().LifestyleSingleton());

            var concepts = new List<BaseConcept>();

            concepts.Add(new SitecoreTemplateConcept() { SubjectTemplateName = "league", ConceptUri = new Uri("http://football.com/league-to-team"), ObjectTemplateName = "team" });
            concepts.Add(new SitecoreTemplateConcept() { SubjectTemplateName = "team", ConceptUri = new Uri("http://football.com/team-to-player"), ObjectTemplateName = "player" });
            concepts.Add(new SitecoreTemplateConcept() { SubjectTemplateName = "newsstory", ConceptUri = new Uri("http://football.com/news-to-item"), ObjectTemplateName = "*" });
   
            container.Register(Component.For<IConceptProvider>().Instance(new SitecoreConceptProvider(concepts)).LifestyleSingleton());

            container.Install(new LinkedDataInstallerSitecore());
        }
    }
}