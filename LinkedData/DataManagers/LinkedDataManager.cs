using System;
using System.Collections.Generic;
using System.Linq;
using LinkedData.Concepts;
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
        private readonly IQueryableStorage _store;
        protected readonly IConceptManager ConceptManager;
        private readonly string _graphUri;

        public LinkedDataManager(List<ITripleFormatter> inFormatters, List<ITripleFormatter> outFormatters, IQueryableStorage store, IConceptManager conceptManager)
        {
            _inFormatters = inFormatters;
            _outFormatters = outFormatters;
            _store = store;
            ConceptManager = conceptManager;
            //_graphUri = "http://examplegraph.com"; //could use this as different graphs per database
            _graphUri = string.Empty;
        }

        public IEnumerable<Triple> GetTriples(string query)
        {
            Object results = _store.Query(query);

            if (results is IGraph)
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
