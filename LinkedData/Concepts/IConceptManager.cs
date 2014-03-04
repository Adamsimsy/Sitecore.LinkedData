using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkedData.Concepts
{
    public interface IConceptManager
    {
        List<Concept> GetConcepts();
    }
}
