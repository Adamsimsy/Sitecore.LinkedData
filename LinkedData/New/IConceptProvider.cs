using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinkedData.Concepts;

namespace LinkedData.New
{
    public interface IConceptProvider
    {
        List<BaseConcept> GetConcepts();
    }
}
