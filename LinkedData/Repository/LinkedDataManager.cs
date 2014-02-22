using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.Converters;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Links;
using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Writing;

namespace LinkedData.Repository
{
    public static class LinkedDataManager
    {
        private static readonly string RdfFilePath = System.Web.HttpContext.Current.Server.MapPath("~/HelloWorld.rdf");

        public static void WriteGraph(IGraph graph)
        {
            var rdfxmlwriter = new RdfXmlWriter();
            rdfxmlwriter.Save(graph, RdfFilePath);
        }

        public static IGraph ReadGraph()
        {
            var graph = new Graph();
            FileLoader.Load(graph, RdfFilePath);
            return graph;
        }

        public static string ItemToUri(Item item)
        {
            //var itemUriConverter = new IndexFieldItemUriValueConverter();

            //ItemUri itemUri = (ItemUri) ((SitecoreItemUniqueId) item);

            //var str = itemUriConverter.ConvertFrom(item);
            return item.Uri != null ? item.Uri.ToString() : string.Empty;
        }

        public static Item UriToItem(string uri)
        {
            var itemUri = new ItemUri(uri);

            var database = Sitecore.Configuration.Factory.GetDatabase(itemUri.DatabaseName);

            return database.GetItem(itemUri.ItemID, itemUri.Language, itemUri.Version);
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
            IUriNode says = g.CreateUriNode(UriFactory.Create("http://example.org/says"));
            ILiteralNode obj = g.CreateLiteralNode(ItemToUri(itemLink.GetTargetItem()));

            return new Triple(sub, says, obj);
        }
    }
}
