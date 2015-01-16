using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Links;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkedData.ComputedLinks
{
    public class DescendantComputedLinkItem : IComputedLinkItem
    {
        public string DescendantTemplateName { get; set; }
        public string SourceTemplateName { get; set; }

        public DescendantComputedLinkItem(string descendantTemplateName)
        {
            DescendantTemplateName = descendantTemplateName;
            SourceTemplateName = null;
        }

        public DescendantComputedLinkItem(string descendantTemplateName, string sourceTemplateName)
        {
            DescendantTemplateName = descendantTemplateName;
            SourceTemplateName = sourceTemplateName;
        }

        public IEnumerable<ItemLink> ComputeLinkItem(Item item)
        {
            var links = new List<ItemLink>();

            if (item != null && item.Axes != null && item.Axes.GetDescendants() != null)
            {
                if (SourceTemplateName == null || item.TemplateName.ToLower().Equals(SourceTemplateName.ToLower()))
                {
                    var matchingDescendants = item.Axes.GetDescendants().Where(x => x.TemplateName.ToLower().Equals(DescendantTemplateName.ToLower()));

                    matchingDescendants.ToList().ForEach(matchingDescendant =>
                        links.Add(new ItemLink(item, ID.Undefined, matchingDescendant, matchingDescendant.Paths.FullPath)));

                    return links;
                }
            }

            return links;
        }
    }
}
