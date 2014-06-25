using System;

namespace LinkedData.FileBasedRepo.Concepts
{
    public class Concept
    {
        public ConceptNode Subject { get; set; }
        public Uri Predicate { get; set; }
        public ConceptNode Object { get; set; }
    }
}
