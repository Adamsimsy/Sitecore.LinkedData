using System;
using System.Collections.Generic;

namespace LinkedData.Concepts
{
    public class SitecoreConceptProvider : IConceptProvider
    {
        private readonly List<BaseConcept> _concepts;
        

        public SitecoreConceptProvider()
        {
            _concepts = new List<BaseConcept>();

            _concepts.Add(new SitecoreTemplateConcept() { SubjectTemplateName = "home", ConceptUri = new Uri("http://example.org/home-to-sampleitem"), ObjectTemplateName = "sample item" });
            _concepts.Add(new SitecoreTemplateConcept() { SubjectTemplateName = "sample item", ConceptUri = new Uri("http://example.org/sampleitem-to-home"), ObjectTemplateName = "home" });
        }

        public SitecoreConceptProvider(List<BaseConcept> concepts)
        {
            _concepts = concepts;
        }

        public List<BaseConcept> GetConcepts()
        {        
            return _concepts;
        }
    }
}
