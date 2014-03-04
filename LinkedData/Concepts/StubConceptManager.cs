using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    }
}
