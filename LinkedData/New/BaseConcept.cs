using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkedData.New
{
    public abstract class BaseConcept
    {
        public Uri ConceptUri { get; set; }
        abstract public bool IsMatch(string objectCompareValue, string subjectCompareValue);
    }
}
