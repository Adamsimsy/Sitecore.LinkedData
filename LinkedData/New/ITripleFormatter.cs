using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDS.RDF;

namespace LinkedData.New
{
    public interface ITripleFormatter
    {
        Triple FormatTriple(Triple triple);
    }
}
