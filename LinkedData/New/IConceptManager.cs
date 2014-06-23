using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDS.RDF;

namespace LinkedData.New
{
    public interface IConceptManager
    {
        IUriNode GetPredicate(IUriNode sub, IUriNode obj);
        IUriNode GetPredicate(IUriNode sub, ILiteralNode obj);
    }
}
