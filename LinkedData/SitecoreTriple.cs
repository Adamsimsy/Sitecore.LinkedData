using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinkedData.Helpers;
using Sitecore.Data.Items;
using VDS.RDF;

namespace LinkedData
{
    public class SitecoreTriple
    {
        private readonly Triple _triple;
        private Item _subjectItem;
        private Item _objectItem;

        public SitecoreTriple(Triple triple)
        {
            _triple = triple;
        }

        public INode SubjectNode
        {
            get
            {
                return _triple.Subject;
            }        
        }

        public INode PredicateNode
        {
            get
            {
                return _triple.Predicate;
            }
        }

        public INode ObjectNode
        {
            get
            {
                return _triple.Object;
            }
        }

        public Item SubjectItem
        {
            get
            {
                if (_subjectItem == null)
                {
                    _subjectItem = SitecoreTripleHelper.UriToItem(_triple.Subject.ToString());
                }
                return _subjectItem;
            }
        }

        public Item ObjectItem
        {
            get
            {
                if (_objectItem == null)
                {
                    _objectItem = SitecoreTripleHelper.UriToItem(_triple.Object.ToString());
                }
                return _objectItem;
            }
        }
    }
}
