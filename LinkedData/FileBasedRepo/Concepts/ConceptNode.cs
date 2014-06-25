namespace LinkedData.FileBasedRepo.Concepts
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
