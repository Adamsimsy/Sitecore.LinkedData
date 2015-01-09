using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using LinkedData.Concepts;
using LinkedData.DataManagers;
using LinkedData.Filters;
using LinkedData.Formatters;
using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Storage;
using VDS.RDF.Writing;
using LinkedData.Helpers;

namespace LinkedData.Website.Layouts
{
    public partial class LinkedData_SparqlQuery_Sublayout : System.Web.UI.UserControl
    {
        public List<SitecoreTriple> SitecoreTriples { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            var factory = DependencyResolver.Instance.Resolve<SitecoreManagerFactory>();
            var manager = factory.GetContextWebDatabaseDataManager();

            string rawParameters = Attributes["sc_parameters"]; 
            var parameters = Sitecore.Web.WebUtil.ParseUrlParameters(rawParameters); 
            var title = parameters["Title"];
            var sparqlQuery = parameters["SparqlQuery"];

            litTitle.Text = title;

            SitecoreTriples = new List<SitecoreTriple>();

            if (!string.IsNullOrEmpty(sparqlQuery))
            {
                //Format the query
                var formattedQuery = String.Format(sparqlQuery,
                    SitecoreTripleHelper.ItemToUri(Sitecore.Context.Item));

                var triples = manager.TripleQuery(formattedQuery);

                SitecoreTriples = triples.ToSitecoreTriples();
            }
        }
    }
}