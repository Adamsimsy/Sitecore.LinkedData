using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkedData.Concepts
{
    public class ConceptNode
    {
        public ConceptNode(string templateName)
        {
            TemplateName = templateName;
        }

        public string TemplateName { get; set; }
    }
}
