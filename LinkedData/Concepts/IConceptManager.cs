using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Data.Items;

namespace LinkedData.Concepts
{
    public interface IConceptManager
    {
        List<Concept> GetConcepts();
        List<Concept> GetMatchingConcepts(Item sourceItem, Item targetItem);
    }
}
