namespace LinkedData.Concepts
{
    public class SitecoreTemplateConcept : BaseConcept
    {
        public string SubjectTemplateName { get; set; }
        public string ObjectTemplateName { get; set; }

        public override bool IsMatch(string subjectCompareValue, string objectCompareValue)
        {
            return subjectCompareValue.ToLower().Equals(SubjectTemplateName.ToLower())
                && (objectCompareValue.ToLower().Equals(ObjectTemplateName.ToLower()) || ObjectTemplateName.Equals("*"));
        }
    }
}
