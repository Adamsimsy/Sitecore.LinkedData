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
