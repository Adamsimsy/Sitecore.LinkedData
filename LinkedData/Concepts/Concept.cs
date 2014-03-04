using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDS.RDF;

namespace LinkedData.Concepts
{
    public class Concept
    {
        public ConceptNode Subject { get; set; }
        public Uri Predicate { get; set; }
        public ConceptNode Object { get; set; }
    }
}
