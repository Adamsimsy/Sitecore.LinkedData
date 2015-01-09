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
        private readonly SitecoreNode _subjectNode;
        private readonly SitecoreNode _objectNode;

        public SitecoreTriple(Triple triple)
        {
            _triple = triple;
            _subjectNode = new SitecoreNode(_triple.Subject);
            _objectNode = new SitecoreNode(_triple.Object);
        }

        public SitecoreNode SubjectNode
        {
            get
            {
                return _subjectNode;
            }        
        }

        public INode PredicateNode
        {
            get
            {
                return _triple.Predicate;
            }
        }

        public SitecoreNode ObjectNode
        {
            get
            {
                return _objectNode;
            }
        }
    }
}
