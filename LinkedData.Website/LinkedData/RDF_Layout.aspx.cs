using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using LinkedData.Repository;
using Sitecore.Shell.Framework.Commands.Masters;
using VDS.RDF;
using VDS.RDF.Writing;

namespace LinkedData.Website.LinkedData
{
    public partial class RDF_Layout : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var graph = LinkedDataManager.ReadGraph();

            var itemTriples = LinkedDataManager.GetItemTriples(Sitecore.Context.Item);

            var rdfxmlwriter = new RdfXmlWriter();
            var sw = new System.IO.StringWriter();

            var outG = new Graph();

            outG.Assert(itemTriples);

            rdfxmlwriter.Save(outG, sw);

            var data = sw.ToString();

            Response.Write(data);
        }
    }
}