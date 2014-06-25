using System.Collections.Generic;

namespace LinkedData.Concepts
{
    public interface IConceptProvider
    {
        List<BaseConcept> GetConcepts();
    }
}
