using System;

namespace LinkedData.Concepts
{
    public abstract class BaseConcept
    {
        public Uri ConceptUri { get; set; }
        abstract public bool IsMatch(string objectCompareValue, string subjectCompareValue);
    }
}
