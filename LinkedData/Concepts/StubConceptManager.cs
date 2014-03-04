using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Data.Items;
using VDS.RDF.Storage.Virtualisation;

namespace LinkedData.Concepts
{
    public class StubConceptManager : IConceptManager
    {
        public List<Concept> GetConcepts()
        {
            var concepts = new List<Concept>();

            concepts.Add(new Concept() { Subject = new ConceptNode("home"), Predicate = new Uri("http://example.org/home-to-sampleitem"), Object = new ConceptNode("sample item") });
            concepts.Add(new Concept() { Subject = new ConceptNode("sample item"), Predicate = new Uri("http://example.org/sampleitem-to-home"), Object = new ConceptNode("home") });

            return concepts;
        }

        public List<Concept> GetMatchingConcepts(Item sourceItem, Item targetItem)
        {
            if (sourceItem != null && targetItem != null)
            {
                
            
            return GetConcepts().Where(x =>
                x.Subject.TemplateName.ToLower().Equals(sourceItem.TemplateName.ToLower())
                && x.Object.TemplateName.ToLower().Equals(targetItem.TemplateName.ToLower())).ToList();
            }
            else
            {
                return null;
            }
        }
    }
}
