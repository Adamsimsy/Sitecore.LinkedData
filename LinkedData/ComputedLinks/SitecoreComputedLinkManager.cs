using Sitecore.Data.Items;
using Sitecore.Links;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkedData.ComputedLinks
{
    public class SitecoreComputedLinkManager : IComputedLinkManager
    {
        IEnumerable<IComputedLinkItem> _computedLinkItems;

        public SitecoreComputedLinkManager(IEnumerable<IComputedLinkItem> computedLinkItems)
        {
            _computedLinkItems = computedLinkItems;
        }

        public List<ItemLink> GetComputedLinkItems(Item item)
        {
            var items = new List<ItemLink>();

            if (_computedLinkItems != null)
            {
                _computedLinkItems.ToList().ForEach(x => items.AddRange(x.ComputeLinkItem(item)));
            }

            return items;
        }
    }
}
