using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using LinkedData.Repository;
using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Writing;

namespace LinkedData.Website.LinkedData
{
    public partial class LinkedDataControl : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //Fill in the code shown on this page here to build your hello world application
            Graph g = new Graph();

            IUriNode dotNetRDF = g.CreateUriNode(UriFactory.Create("http://www.dotnetrdf.org"));
            IUriNode says = g.CreateUriNode(UriFactory.Create("http://example.org/says"));
            ILiteralNode helloWorld = g.CreateLiteralNode("Hello World");
            ILiteralNode bonjourMonde = g.CreateLiteralNode("Bonjour tout le Monde", "fr");

            g.Assert(new Triple(dotNetRDF, says, helloWorld));
            g.Assert(new Triple(dotNetRDF, says, bonjourMonde));

            IUriNode dotNetRDF2 = g.CreateUriNode(UriFactory.Create("http://www.dotnetrdf2.org"));
            ILiteralNode helloWorld2 = g.CreateLiteralNode("Hello World2");
            g.Assert(new Triple(dotNetRDF2, says, helloWorld2));

            foreach (Triple t in g.Triples)
            {
                Console.WriteLine(t.ToString());
            }

            //NTriplesWriter ntwriter = new NTriplesWriter();
            //ntwriter.Save(g, "HelloWorld.nt");

            //LinkedDataManager.WriteGraph(g);
            //LinkedDataManager

            RdfXmlWriter rdfxmlwriter = new RdfXmlWriter();
            rdfxmlwriter.Save(g, Server.MapPath("~/HelloWorld.rdf"));

            IGraph g2 = new Graph();
            FileLoader.Load(g2, Server.MapPath("~/HelloWorld.rdf"));

            //Call the Save() method to write to the StringWriter
            System.IO.StringWriter sw = new System.IO.StringWriter();
            rdfxmlwriter.Save(g2, sw);
            litRdf.Text = HttpUtility.HtmlEncode(sw.ToString());
        }
    }
}