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
using LinkedData.ComputedLinks;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(SitecoreLinkedDataInstaller), "Start")]

namespace LinkedData.Website.App_Start
{
    public static class SitecoreLinkedDataInstaller
    {
        public static void Start()
        {
            var container = new WindsorContainer();

            SetupTripleStore(container);

            SetupConcepts(container);

            SetupComputedLinkItems(container);

            container.Install(new BaseSitecoreLinkedDataInstaller());
        }

        /// <summary>
        /// Setup the triple store. See http://github.com/Adamsimsy/Sitecore.LinkedData/wiki/Triple-store-configuration for configuration details.
        /// </summary>
        /// <param name="container"></param>
        private static void SetupTripleStore(IWindsorContainer container)
        {
            //InMemory Triple Store for testing.
            container.Register(Component.For<IQueryableStorage>().ImplementedBy<InMemoryManager>().LifestyleSingleton());

            //Sesame Triple Store example.
            //container.Register(Component.For<IQueryableStorage>().ImplementedBy<SesameHttpProtocolVersion6Connector>().LifestyleSingleton()
            //.DependsOn(Dependency.OnValue("baseUri", "http://server_name:8080/openrdf-sesame"), Dependency.OnValue("storeID", "repository_name")));
        }

        /// <summary>
        /// Setup link concepts. See http://github.com/Adamsimsy/Sitecore.LinkedData/wiki/Concept-configuration for configuration details.
        /// </summary>
        /// <param name="container"></param>
        private static void SetupConcepts(IWindsorContainer container)
        {
            var concepts = new List<BaseConcept>();

            //Example football concepts for use with http://github.com/Adamsimsy/Sitecore.LinkedData/wiki/Example-content.
            concepts.Add(new SitecoreTemplateConcept() { SubjectTemplateName = "league", ConceptUri = new Uri("http://football.com/league-to-team"), ObjectTemplateName = "team" });
            concepts.Add(new SitecoreTemplateConcept() { SubjectTemplateName = "team", ConceptUri = new Uri("http://football.com/team-to-player"), ObjectTemplateName = "player" });
            concepts.Add(new SitecoreTemplateConcept() { SubjectTemplateName = "newsstory", ConceptUri = new Uri("http://football.com/news-to-item"), ObjectTemplateName = "*" });
            concepts.Add(new SitecoreTemplateConcept() { SubjectTemplateName = "ground", ConceptUri = new Uri("http://football.com/home-of-team"), ObjectTemplateName = "team" });
            concepts.Add(new SitecoreTemplateConcept() { SubjectTemplateName = "team", ConceptUri = new Uri("http://football.com/team-staff"), ObjectTemplateName = "staff" });

            container.Register(Component.For<IConceptProvider>().Instance(new SitecoreConceptProvider(concepts)).LifestyleSingleton());
        }

        /// <summary>
        /// Setup of computed links. See http://github.com/Adamsimsy/Sitecore.LinkedData/wiki/Computed-link-configuration for configuration details.
        /// </summary>
        /// <param name="container"></param>
        private static void SetupComputedLinkItems(IWindsorContainer container)
        {
            var computedLinkItems = new List<IComputedLinkItem>();

            //Example football computed links for use with http://github.com/Adamsimsy/Sitecore.LinkedData/wiki/Example-content.
            computedLinkItems.Add(new AncestorComputedLinkItem("team", "ground"));
            computedLinkItems.Add(new DescendantComputedLinkItem("staff", "team"));

            container.Register(Component.For<IComputedLinkManager>().Instance(new SitecoreComputedLinkManager(computedLinkItems)).LifestyleSingleton());
        }
    }
}