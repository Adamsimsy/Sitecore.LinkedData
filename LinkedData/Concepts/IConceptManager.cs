using VDS.RDF;

namespace LinkedData.Concepts
{
    public interface IConceptManager
    {
        IUriNode GetPredicate(IUriNode sub, IUriNode obj);
        IUriNode GetPredicate(IUriNode sub, ILiteralNode obj);
    }
}
