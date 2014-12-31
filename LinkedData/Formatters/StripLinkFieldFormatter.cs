using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinkedData.Helpers;
using Sitecore.Links;
using VDS.RDF;

namespace LinkedData.Formatters
{
    public class StripLinkFieldFormatter : ITripleFormatter
    {
        public Triple FormatTriple(Triple triple)
        {
            var g = new Graph();

            IUriNode sub = g.CreateUriNode(new Uri(triple.Subject.ToString()));
            IUriNode pred = g.CreateUriNode(new Uri(SitecoreTripleHelper.RemoveLinkFieldFromPredicate(triple.Predicate)));
            IUriNode obj = g.CreateUriNode(new Uri(triple.Object.ToString()));

            triple = new Triple(sub, pred, obj);

            return triple;
        }
    }
}
