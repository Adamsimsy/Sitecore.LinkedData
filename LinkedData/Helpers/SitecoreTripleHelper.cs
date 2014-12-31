using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using Castle.Core.Internal;
using LinkedData.Concepts;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Links;
using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Query;

namespace LinkedData.Helpers
{
    public static class SitecoreTripleHelper
    {
        public static Uri BrokenLinkUri = new Uri("sitecore://broken-link");

        public static List<SitecoreTriple> ToSitecoreTriples(this IEnumerable<Triple> triples)
        {
            var sitecoreTriples = new List<SitecoreTriple>();

            triples.ForEach(triple => sitecoreTriples.Add(new SitecoreTriple(triple)));

            return sitecoreTriples;
        }

        public static SitecoreTriple ToSitecoreTriple(this Triple triple)
        {
            return new SitecoreTriple(triple);
        }

        public static List<ItemLink> TriplesToItemLinks(IEnumerable<Triple> triples)
        {
            var list = new List<ItemLink>();

            foreach (var triple in triples)
            {
                var sourceItem = SitecoreTripleHelper.UriToItem(triple.Subject.ToString());
                var targetItem = SitecoreTripleHelper.UriToItem(triple.Object.ToString());

                if (sourceItem != null && targetItem != null)
                {
                    list.Add(new ItemLink(sourceItem.Database.Name, sourceItem.ID,
                        SitecoreTripleHelper.GetFieldIdFromPredicate(triple.Predicate),
                        targetItem.Database.Name, targetItem.ID,
                        targetItem.Paths.FullPath));
                }
            }

            return list;
        }

        public static Triple ToTriple(IConceptManager conceptManager, Item item, ItemLink itemLink)
        {
            var g = new Graph();

            g.NamespaceMap.AddNamespace("sitecore", new Uri("sitecore:"));
            //g.NamespaceMap.AddNamespace("http", new Uri("http:"));

            IUriNode sub = g.CreateUriNode(ItemToUri(item));

            IUriNode obj;

            if (itemLink.GetTargetItem() == null)
            {
                //Broken link creation
                obj = g.CreateUriNode(BrokenLinkUri);
            }
            else
            {
                obj = g.CreateUriNode(ItemToUri(itemLink.GetTargetItem()));
            }

            IUriNode predicate = conceptManager.GetPredicate(sub, obj);

            //Add Source Field Id to predicate
            IUriNode predicateWithFieldId = g.CreateUriNode(new Uri(predicate.Uri + "#" + itemLink.SourceFieldID));

            return new Triple(sub, Tools.CopyNode(predicateWithFieldId, g), obj);
        }

        public static ID GetFieldIdFromPredicate(INode predicateNode)
        {
            return new ID(SitecoreTripleHelper.GetFieldIdFromPredicate(predicateNode.ToString()));
        }

        public static string GetFieldIdFromPredicate(string uri)
        {
            var uriArray = uri.Split('#');

            if (uriArray.Count() > 1)
            {
                return uriArray[1].Replace("%7B", "{").Replace("%7D", "}");
            }

            return string.Empty;
        }

        public static string ItemToUri(Item item)
        {
            if (item != null && item.Uri != null)
            {
                return item.Uri.ToString().Replace("{", "%7B").Replace("}", "%7D");
            }
            return string.Empty;
        }

        public static Item UriToItem(string uri)
        {
            if (uri != null && !string.IsNullOrEmpty(uri) 
                && uri.StartsWith("sitecore:")
                && !new Uri(uri).Equals(BrokenLinkUri))
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
