﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinkedData.Concepts;
using LinkedData.FileBasedRepo;
using LinkedData.Filters;
using LinkedData.Formatters;
using LinkedData.Helpers;
using Sitecore.Data.Items;
using Sitecore.Links;
using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Storage;

namespace LinkedData.DataManagers
{
    public class SitecoreLinkedDataManager : LinkedDataManager
    {
        private const string TriplesByObjectFormat = @"CONSTRUCT {{ ?s ?p <{0}> }} WHERE {{ ?s ?p <{0}> }}";
        private const string TriplesBySubjectFormat = @"CONSTRUCT {{ <{0}> ?p ?o }} WHERE {{ <{0}> ?p ?o }}";
        private const string TriplesBySubjectObjectFormat = @"CONSTRUCT {{ <{0}> ?p <{1}> }} WHERE {{ <{0}> ?p <{1}> }}";
        private const string TriplesBySubjectPredicateFormat = @"CONSTRUCT {{ <{0}> ?p ?o }} WHERE {{ <{0}> ?p ?o FILTER(STRSTARTS(STR(?p), ""{1}"")) }}";

        public SitecoreLinkedDataManager(IQueryableStorage store, IConceptManager conceptManager, Uri graphUri)
            : base(store, conceptManager, graphUri)
        {
        }

        public SitecoreLinkedDataManager(List<ITripleFormatter> inFormatters, List<ITripleFormatter> outFormatters, List<IFilter> inFilters, IQueryableStorage store, IConceptManager conceptManager, Uri graphUri)
            : base(inFormatters, outFormatters, inFilters, store, conceptManager, graphUri)
        {
        }

        public IEnumerable<Triple> GetItemTriplesByObject(Item item)
        {
            var itemUri = SitecoreTripleHelper.ItemToUri(item);

            var query = SitecoreTripleHelper.StringToSparqlQuery(String.Format(TriplesByObjectFormat, itemUri));

            return TripleQuery(query);
        }

        public IEnumerable<Triple> GetItemTriplesBySubject(Item item)
        {
            var itemUri = SitecoreTripleHelper.ItemToUri(item);

            var query = SitecoreTripleHelper.StringToSparqlQuery(String.Format(TriplesBySubjectFormat, itemUri));

            return TripleQuery(query);
        }

        public IEnumerable<Triple> GetItemTriplesBySubjectPredicate(Item item, string predicate)
        {
            var itemUri = SitecoreTripleHelper.ItemToUri(item);

            var query = SitecoreTripleHelper.StringToSparqlQuery(String.Format(TriplesBySubjectPredicateFormat, itemUri, predicate));

            return TripleQuery(query);
        }

        public void AddLink(Item item, ItemLink link)
        {
            var triple = SitecoreTripleHelper.ToTriple(ConceptManager, item, link);

            WriteTriple(triple);
        }

        public void RemoveLinksForItem(Item item, ItemLink link)
        {
            var parser = new SparqlQueryParser();

            var subjectUri = SitecoreTripleHelper.ItemToUri(item);
            var objectUri = SitecoreTripleHelper.ItemToUri(link.GetTargetItem());

            var query = SitecoreTripleHelper.StringToSparqlQuery(String.Format(TriplesBySubjectObjectFormat, subjectUri, objectUri));

            var triplesToDelete = TripleQuery(query);

            DeleteTriples(triplesToDelete);
        }
    }
}
