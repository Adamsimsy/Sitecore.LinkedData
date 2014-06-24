using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkedData.New
{
    public class SitecoreTemplateConcept : BaseConcept
    {
        public string ObjectTemplateName { get; set; }
        public string SubjectTemplateName { get; set; }

        public override bool IsMatch(string objectCompareValue, string subjectCompareValue)
        {
            return objectCompareValue.ToLower().Equals(ObjectTemplateName.ToLower()) && subjectCompareValue.ToLower().Equals(SubjectTemplateName.ToLower());
        }
    }
}
