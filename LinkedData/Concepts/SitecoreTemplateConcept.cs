namespace LinkedData.Concepts
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
