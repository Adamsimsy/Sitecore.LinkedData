using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Writing;

namespace LinkedData.Repository
{
    public static class LinkedDataManager
    {
        private static readonly string RdfFilePath = System.Web.HttpContext.Current.Server.MapPath("~/HelloWorld.rdf");

        public static void WriteGraph(IGraph graph)
        {
            var rdfxmlwriter = new RdfXmlWriter();
            rdfxmlwriter.Save(graph, RdfFilePath);
        }

        public static IGraph ReadGraph()
        {
            var graph = new Graph();
            FileLoader.Load(graph, RdfFilePath);
            return graph;
        }
    }
}
