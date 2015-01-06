using System;
using System.Collections.Generic;
using System.Linq;
using LinkedData.Concepts;
using LinkedData.Filters;
using LinkedData.Formatters;
using LinkedData.Helpers;
using Sitecore.Data.Items;
using Sitecore.Links;
using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Query;
using VDS.RDF.Storage;

namespace LinkedData.DataManagers
{
    public class LinkedDataManager
    {
        private readonly List<ITripleFormatter> _inFormatters;
        private readonly List<ITripleFormatter> _outFormatters;
        private readonly List<IFilter> _inFilters;
        private readonly IQueryableStorage _store;
        protected readonly IConceptManager ConceptManager;
        private readonly Uri _graphUri;
        private List<Triple> _tripleWriteBuffer = new List<Triple>();

        public LinkedDataManager(IQueryableStorage store, IConceptManager conceptManager, Uri graphUri)
        {
            _inFormatters = null;
            _outFormatters = null;
            _store = store;
            ConceptManager = conceptManager;
            _graphUri = graphUri;
        }

        public LinkedDataManager(List<ITripleFormatter> inFormatters, List<ITripleFormatter> outFormatters, List<IFilter> inFilters, IQueryableStorage store, IConceptManager conceptManager, Uri graphUri)
        {
            _inFormatters = inFormatters;
            _outFormatters = outFormatters;
            _inFilters = inFilters;
            _store = store;
            ConceptManager = conceptManager;
            _graphUri = graphUri;
        }

        public IEnumerable<Triple> GetTriples(string query)
        {
            var sqp = new SparqlQueryParser();
            var sparqlQuery = sqp.ParseFromString(query);
            sparqlQuery.AddDefaultGraph(_graphUri);

            Object results = _store.Query(sparqlQuery.ToString());

            if (results is IGraph)
            {
                var g = (IGraph)results;

                var triples = g.Triples;

                return ApplyFormatters(_outFormatters, triples);
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
            triples = ApplyFilters(_inFilters, triples);

            triples = ApplyFormatters(_inFormatters, triples);

            //Not need atm.
            //triples = AddGraphUriToTriples(_graphUri, triples);

            _tripleWriteBuffer.AddRange(triples);
        }

        public void Flush()
        {
            if (_store.UpdateSupported && _tripleWriteBuffer != null && _tripleWriteBuffer.Any())
            {
                _store.UpdateGraph(_graphUri, _tripleWriteBuffer, null);
            }

            _tripleWriteBuffer = new List<Triple>();
        }

        private IEnumerable<Triple> AddGraphUriToTriples(Uri _graphUri, IEnumerable<Triple> triples)
        {
            var newTriples = new List<Triple>();

            foreach (var triple in triples)
            {
                newTriples.Add(new Triple(triple.Subject, triple.Predicate, triple.Object, _graphUri));
            }

            return newTriples;
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

        private IEnumerable<Triple> ApplyFormatters(IEnumerable<ITripleFormatter> formatters, IEnumerable<Triple> triples)
        {
            if (formatters != null && triples != null)
            {
                var formattedTriples = new List<Triple>();

                foreach (var formatter in formatters)
                {
                    var innerFormattedTriples = new List<Triple>();

                    foreach (var triple in triples.ToList())
                    {
                        innerFormattedTriples.Add(formatter.FormatTriple(triple));
                    }

                    formattedTriples = innerFormattedTriples;
                }

                return formattedTriples;
            }
            return triples;
        }

        private IEnumerable<Triple> ApplyFilters(IEnumerable<IFilter> filters, IEnumerable<Triple> triples)
        {
            if (filters != null && triples != null)
            {
                var FilteredTriples = new List<Triple>();

                foreach (var filter in filters)
                {
                    var innerFilteredTriples = new List<Triple>();

                    foreach (var triple in triples.ToList())
                    {
                        if (!filter.ShouldFilter(triple))
                        {
                            innerFilteredTriples.Add(triple);
                        }
                    }

                    FilteredTriples = innerFilteredTriples;
                }

                return FilteredTriples;
            }
            return triples;
        }
    }
}
