using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using LinkedData.Concepts;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Links;
using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Query;

namespace LinkedData.New
{
    public static class SitecoreTripleHelper
    {
        public static Triple ToTriple(IConceptManager conceptManager, Item item, ItemLink itemLink)
        {
            var g = new Graph();
            g.NamespaceMap.AddNamespace("sitecore", new Uri("sitecore:"));

            IUriNode sub = g.CreateUriNode(ItemToUri(item));
            ILiteralNode obj = g.CreateLiteralNode(ItemToUri(itemLink.GetTargetItem()));

            IUriNode predicate = conceptManager.GetPredicate(sub, obj);

            return new Triple(sub, Tools.CopyNode(predicate, g), obj);
        }

        public static string ItemToUri(Item item)
        {
            if (item != null && item.Uri != null)
            {
                return item.Uri.ToString();
            }
            return string.Empty;
        }

        public static Item UriToItem(string uri)
        {
            if (uri != null && !string.IsNullOrEmpty(uri) && uri.StartsWith("sitecore:"))
            {
                var itemUri = new ItemUri(uri.Replace("%7B", "{").Replace("%7D", "}"));

                var database = Sitecore.Configuration.Factory.GetDatabase(itemUri.DatabaseName);

                return database.GetItem(itemUri.ItemID, itemUri.Language, itemUri.Version);
            }
            return null;
        }

        public static SparqlQuery StringToSparqlQuery(string str)
        {
            var parser = new SparqlQueryParser();

            return parser.ParseFromString("PREFIX sitecore: <sitecore:>" + str);
        }
    }
}
