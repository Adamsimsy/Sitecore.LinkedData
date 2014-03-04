using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using LinkedData.Concepts;
using Sitecore.Collections;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Links;
using Sitecore.Shell.Applications.ContentEditor;
using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Writing;

namespace LinkedData.Repository
{
    public static class LinkedDataManager
    {
        private static readonly string RdfFilePath = System.Web.HttpContext.Current.Server.MapPath("~/HelloWorld.rdf");
        private static readonly LockSet Locks = new LockSet();

        public static IEnumerable<Triple> GetItemTriples(Item item)
        {
            var g = LinkedDataManager.ReadGraph();

            var items = g.GetTriplesWithSubject(g.CreateUriNode(LinkedDataManager.ItemToUri(item)));

            return items;
        }

        public static void AddLink(Item item, ItemLink link)
        {
            var g = LinkedDataManager.ReadGraph();

            LinkedDataManager.WriteTriple(g, LinkedDataManager.ToTriple(g, item, link));
        }

        public static void RemoveLinksForItem(Item item, ItemLink link)
        {
            var g = LinkedDataManager.ReadGraph();

            var items = g.GetTriplesWithSubjectObject(
                g.CreateUriNode(LinkedDataManager.ItemToUri(item)),
                g.CreateLiteralNode(LinkedDataManager.ItemToUri(link.GetTargetItem())));

            if (items != null && items.Any())
            {
                g.Retract(items.First());
                WriteGraph(g);
            }
        }

        public static void WriteGraph(IGraph graph)
        {
            lock (Locks.GetLock((object) "rdflock"))
            {
                var rdfxmlwriter = new RdfXmlWriter();
                rdfxmlwriter.Save(graph, RdfFilePath);
            }
        }

        public static IGraph ReadGraph()
        {
            lock (Locks.GetLock((object) "rdflock"))
            {
                var graph = new Graph();
                FileLoader.Load(graph, RdfFilePath);
                return graph;
            }
        }

        public static string ItemToUri(Item item)
        {
            //var itemUriConverter = new IndexFieldItemUriValueConverter();

            //ItemUri itemUri = (ItemUri) ((SitecoreItemUniqueId) item);

            //var str = itemUriConverter.ConvertFrom(item);

            if (item != null && item.Uri != null)
            {
                return item.Uri.ToString();
            }
            return string.Empty;
        }

        public static Item UriToItem(string uri)
        {
            if (uri != null)
            {
                var itemUri = new ItemUri(uri);

                var database = Sitecore.Configuration.Factory.GetDatabase(itemUri.DatabaseName);

                return database.GetItem(itemUri.ItemID, itemUri.Language, itemUri.Version);
            }
            return null;
        }

        public static void WriteTriple(IGraph g, Triple triple)
        {
            g.Retract(triple);
            g.Assert(triple);
            WriteGraph(g);
        }

        public static Triple ToTriple(IGraph g, Item item, ItemLink itemLink)
        {
            IUriNode sub = g.CreateUriNode(ItemToUri(item));

            IConceptManager conceptManager = new StubConceptManager();

            var matchingConcepts = conceptManager.GetMatchingConcepts(item, itemLink.GetTargetItem());

            IUriNode predicate;

            if (matchingConcepts != null && matchingConcepts.Any())
            {
                predicate = g.CreateUriNode(matchingConcepts.First().Predicate);
            }
            else
            {
                predicate = g.CreateUriNode(UriFactory.Create("http://example.org/says"));
            }

            ILiteralNode obj = g.CreateLiteralNode(ItemToUri(itemLink.GetTargetItem()));

            return new Triple(sub, predicate, obj);
        }
    }
}
