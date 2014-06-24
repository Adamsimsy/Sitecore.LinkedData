using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Sitecore.Common;
using Sitecore.Data.Items;
using Sitecore.Links;
using Sitecore.Web.UI.HtmlControls;
using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Query;
using VDS.RDF.Query.Datasets;
using VDS.RDF.Storage;

namespace LinkedData.New
{
    public class LinkedDataManager
    {
        private readonly List<ITripleFormatter> _inFormatters;
        private readonly List<ITripleFormatter> _outFormatters;
        private readonly IQueryableStorage _store;
        private readonly IConceptManager _conceptManager;
        private readonly string _graphUri;

        private readonly string _triplesByObjectFormat = "CONSTRUCT {{ ?s ?p <{0}> }} WHERE {{ ?s ?p <{0}> }}";
        private readonly string _triplesBySubjectFormat = "CONSTRUCT {{ <{0}> ?p ?o }} WHERE { <{0}> ?p ?o }}";
        private readonly string _triplesBySubjectObjectFormat = "CONSTRUCT {{ <{0}> ?p <{1}> }} WHERE {{ <{0}> ?p <{1}> }}";

        public LinkedDataManager(List<ITripleFormatter> inFormatters, List<ITripleFormatter> outFormatters, IQueryableStorage store, IConceptManager conceptManager)
        {
            _inFormatters = inFormatters;
            _outFormatters = outFormatters;
            _store = store;
            _conceptManager = conceptManager;
            _graphUri = "http://examplegraph.com";
        }

        public IEnumerable<Triple> GetItemTriplesByObject(Item item)
        {
            var itemUri = SitecoreTripleHelper.ItemToUri(item);

            var query = SitecoreTripleHelper.StringToSparqlQuery(String.Format(_triplesByObjectFormat, itemUri));

            return GetTriples(query);
        }

        public IEnumerable<Triple> GetItemTriplesBySubject(Item item)
        {
            var itemUri = SitecoreTripleHelper.ItemToUri(item);

            var query = SitecoreTripleHelper.StringToSparqlQuery(String.Format(_triplesBySubjectFormat, itemUri));

            return GetTriples(query);
        }

        public void AddLink(Item item, ItemLink link)
        {
            var triple = SitecoreTripleHelper.ToTriple(_conceptManager, item, link);

            WriteTriple(triple);
        }

        public void RemoveLinksForItem(Item item, ItemLink link)
        {
            var parser = new SparqlQueryParser();

            var subjectUri = SitecoreTripleHelper.ItemToUri(item);
            var objectUri = SitecoreTripleHelper.ItemToUri(link.GetTargetItem());

            var query = SitecoreTripleHelper.StringToSparqlQuery(String.Format(_triplesBySubjectObjectFormat, subjectUri, objectUri));

            var triplesToDelete = GetTriples(query);

            DeleteTriples(triplesToDelete);
        }

        public IEnumerable<Triple> GetTriples(string query)
        {

            //var r1 = (IGraph)_store.Query("CONSTRUCT { ?s ?p ?o } WHERE { ?s ?p ?o }");

            //ISparqlQueryProcessor processor = new ExplainQueryProcessor(new InMemoryDataset(r1), ExplanationLevel.Full);

            //Object results = processor.ProcessQuery(new SparqlQueryParser().ParseFromString(query));
            Object results = _store.Query(query);

            if (results is SparqlResultSet)
            {
                //Print the results
                var resultSet = (SparqlResultSet)results;
                foreach (var result in resultSet)
                {
                    Console.WriteLine(result.ToString());
                }
            }
            else if (results is IGraph)
            {
                var g = (IGraph)results;

                return g.Triples;
            }
            else
            {
                throw new Exception("Did not get a SPARQL Result Set as expected");
            }

            return new List<Triple>();
        }

        public IEnumerable<Triple> GetTriples(SparqlQuery query)
        {
            return GetTriples(query.ToString());
        }

        public void WriteTriple(Triple triple)
        {
            WriteTriples(new List<Triple>() { triple });
        }

        public void WriteTriples(IEnumerable<Triple> triples)
        {
            ApplyFormatters(_inFormatters, triples);

            if (_store.UpdateSupported)
            {
                _store.UpdateGraph(_graphUri, triples, null);
            }
        }

        public void DeleteTriple(Triple triple)
        {
            DeleteTriples(new List<Triple>() { triple });
        }

        public void DeleteTriples(IEnumerable<Triple> triples)
        {
            ApplyFormatters(_inFormatters, triples);

            if (_store.DeleteSupported)
            {
                _store.UpdateGraph(_graphUri, null, triples);
            }
        }

        private void ApplyFormatters(IEnumerable<ITripleFormatter> formatters, IEnumerable<Triple> triples)
        {
            if (formatters != null && triples != null)
            {
                foreach (var formatter in formatters)
                {
                    foreach (var triple in triples.ToList())
                    {
                        formatter.FormatTriple(triple);
                    }
                }
            }
        }
    }
}
