using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkedData.New
{
    class SitecoreConceptProvider : IConceptProvider
    {
        public List<BaseConcept> GetConcepts()
        {
            var concepts = new List<BaseConcept>();

            concepts.Add(new SitecoreTemplateConcept() { SubjectTemplateName = "home", ConceptUri = new Uri("http://example.org/home-to-sampleitem"), ObjectTemplateName = "sample item" });
            concepts.Add(new SitecoreTemplateConcept() { SubjectTemplateName = "sample item", ConceptUri = new Uri("http://example.org/sampleitem-to-home"), ObjectTemplateName = "home" });

            return concepts;
        }
    }
}
