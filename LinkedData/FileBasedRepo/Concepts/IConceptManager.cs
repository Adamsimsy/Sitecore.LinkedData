using System.Collections.Generic;
using Sitecore.Data.Items;

namespace LinkedData.FileBasedRepo.Concepts
{
    public interface IConceptManager
    {
        List<Concept> GetConcepts();
        List<Concept> GetMatchingConcepts(Item sourceItem, Item targetItem);
    }
}
