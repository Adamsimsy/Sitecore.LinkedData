using Sitecore.Data.Items;
using Sitecore.Links;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkedData.ComputedLinks
{
    public interface IComputedLinkItem
    {
        IEnumerable<ItemLink> ComputeLinkItem(Item item);
    }
}
