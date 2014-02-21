using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using VDS.RDF;
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

            foreach (Triple t in g.Triples)
            {
                Console.WriteLine(t.ToString());
            }

            NTriplesWriter ntwriter = new NTriplesWriter();
            //ntwriter.Save(g, "HelloWorld.nt");

            RdfXmlWriter rdfxmlwriter = new RdfXmlWriter();
            rdfxmlwriter.Save(g, Server.MapPath("~/HelloWorld.rdf"));

            System.IO.StringWriter sw = new System.IO.StringWriter();

            //Call the Save() method to write to the StringWriter
            rdfxmlwriter.Save(g, sw);

            litRdf.Text = HttpUtility.HtmlEncode(sw.ToString());
        }
    }
}