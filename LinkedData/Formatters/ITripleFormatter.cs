using VDS.RDF;

namespace LinkedData.Formatters
{
    public interface ITripleFormatter
    {
        Triple FormatTriple(Triple triple);
    }
}
