using System.Collections.Generic;
using LinkedData.Helpers;
using VDS.RDF;

namespace LinkedData.Concepts
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

            var predicate = g.CreateUriNode(UriFactory.Create("http://sitecore.net/linkedto"));

            foreach (var baseConcept in _concepts)
            {
                var sourceItem = SitecoreTripleHelper.UriToItem(subUri);
                var targetItem = SitecoreTripleHelper.UriToItem(objUri);

                
                if (sourceItem!= null &&
                    targetItem != null)
                {
                    var sourceTemplateName = sourceItem.TemplateName;
                    var targetTemplateName = targetItem.TemplateName;

                    if (baseConcept.IsMatch(sourceTemplateName, targetTemplateName))
                    {
                        predicate = g.CreateUriNode(baseConcept.ConceptUri);
                    }
                }
            }

            return predicate;
        }
    }
}
