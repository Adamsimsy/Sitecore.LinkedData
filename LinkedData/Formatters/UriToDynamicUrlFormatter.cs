﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinkedData.Helpers;
using Sitecore.Links;
using VDS.RDF;

namespace LinkedData.Formatters
{
    public class UriToDynamicUrlFormatter : ITripleFormatter
    {
        public Triple FormatTriple(Triple triple)
        {
            var subjectItem = SitecoreTripleHelper.UriToItem(triple.Subject.ToString());
            var objectItem = SitecoreTripleHelper.UriToItem(triple.Object.ToString());

            if (subjectItem == null || objectItem == null)
            {
                return triple;
            }

            //TODO: Better solution for removing /sitecore/shell from links
            var subjectUrl = LinkManager.GetItemUrl(subjectItem).Replace("/sitecore/shell","");
            var objectUrl = LinkManager.GetItemUrl(objectItem).Replace("/sitecore/shell", "");

            var g = new Graph();

            IUriNode sub = g.CreateUriNode(new Uri(subjectUrl));
            IUriNode pred = g.CreateUriNode(new Uri(triple.Predicate.ToString().Split('#')[0]));
            IUriNode obj = g.CreateUriNode(new Uri(objectUrl));

            triple = new Triple(sub, pred, obj);

            return triple;
        }
    }
}
