﻿using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Links;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkedData.ComputedLinks
{
    public class AncestorComputedLinkItem : IComputedLinkItem
    {
        public string AncestorTemplateName { get; set; }
        public string SourceTemplateName { get; set; }

        public AncestorComputedLinkItem(string ancestorTemplateName)
        {
            AncestorTemplateName = ancestorTemplateName;
            SourceTemplateName = null;
        }

        public AncestorComputedLinkItem(string ancestorTemplateName, string sourceTemplateName)
        {
            AncestorTemplateName = ancestorTemplateName;
            SourceTemplateName = sourceTemplateName;
        }

        public IEnumerable<ItemLink> ComputeLinkItem(Item item)
        {
            var links = new List<ItemLink>();

            if (item != null && item.Axes != null && item.Axes.GetAncestors() != null)
            {
                if (SourceTemplateName == null || item.TemplateName.ToLower().Equals(SourceTemplateName.ToLower()))
                {
                    var matches = item.Axes.GetAncestors().ToList().Where(x => x.TemplateName.ToLower().Equals(AncestorTemplateName.ToLower()));

                    if (matches != null && matches.Any())
                    {
                        var matchingAncestor = matches.Last();

                        if (matchingAncestor != null)
                        {
                            links = new List<ItemLink>();

                            links.Add(new ItemLink(item, ID.Undefined, matchingAncestor, matchingAncestor.Paths.FullPath));

                            return links;
                        }
                    }
                }
            }

            return links;
        }
    }
}
