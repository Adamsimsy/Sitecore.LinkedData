using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinkedData.Concepts;
using VDS.RDF;

namespace LinkedData.New
{
    public class SitecoreConceptManager : IConceptManager
    {
        private readonly List<BaseConcept> _concepts;

        public SitecoreConceptManager(IConceptProvider provider)
        {
            _concepts = provider.GetConcepts();
        }

        public IUriNode GetPredicate(IUriNode sub, IUriNode obj)
        {
            return GetPredicate(sub.ToString(), obj.ToString());
        }


        public IUriNode GetPredicate(IUriNode sub, ILiteralNode obj)
        {
            return GetPredicate(sub.ToString(), obj.ToString());
        }

        private IUriNode GetPredicate(string subUri, string objUri)
        {
            var g = new Graph();

            var predicate = g.CreateUriNode(UriFactory.Create("http://example.org/linkedto"));

            foreach (var baseConcept in _concepts)
            {
                var sourceItem = SitecoreTripleHelper.UriToItem(subUri);
                var targetItem = SitecoreTripleHelper.UriToItem(objUri);

                var sourceTemplateName = sourceItem.TemplateName;
                var targetTemplateName = targetItem.TemplateName;

                if (baseConcept.IsMatch(sourceTemplateName, targetTemplateName))
                {
                    predicate = g.CreateUriNode(baseConcept.ConceptUri);
                }
            }

            return predicate;
        }
    }
}
