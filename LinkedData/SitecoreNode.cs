using LinkedData.Helpers;
using Sitecore.Data.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDS.RDF;

namespace LinkedData
{
    public class SitecoreNode
    {
        private readonly INode _node;
        private Item _sitecoreItem;

        public SitecoreNode(INode node)
        {
            _node = node;
        }

        public Item Item
        {
            get
            {
                if (_sitecoreItem == null)
                {
                    _sitecoreItem = SitecoreTripleHelper.UriToItem(_node.ToString());
                }
                return _sitecoreItem;
            }
        }
    }
}
