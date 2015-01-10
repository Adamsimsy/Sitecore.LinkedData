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
using VDS.RDF.Query;
using Sitecore.Links;

namespace LinkedData.Website.Layouts
{
    public partial class LinkedData_SparqlQuery_Sublayout : System.Web.UI.UserControl
    {
        public List<SitecoreTriple> SitecoreTriples { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            var sublayout = Parent as Sitecore.Web.UI.WebControls.Sublayout;
            var dataSourceItem = Sitecore.Context.Database.GetItem(sublayout.DataSource);

            var factory = DependencyResolver.Instance.Resolve<SitecoreManagerFactory>();
            var manager = factory.GetContextWebDatabaseDataManager();

            //string rawParameters = Attributes["sc_parameters"]; 
            //var parameters = Sitecore.Web.WebUtil.ParseUrlParameters(rawParameters); 
            var title = dataSourceItem.Fields["Title"].Value;
            var sparqlQuery = dataSourceItem.Fields["SparqlQuery"].Value;

            litTitle.Text = title;

            SitecoreTriples = new List<SitecoreTriple>();

            if (!string.IsNullOrEmpty(sparqlQuery))
            {
                //Format the query
                var formattedQuery = String.Format(sparqlQuery,
                    SitecoreTripleHelper.ItemToUri(Sitecore.Context.Item));

                var sqp = new SparqlQueryParser();
                var query = sqp.ParseFromString(formattedQuery);
                
                if (query.QueryType == SparqlQueryType.Construct)
                {
                    var triples = manager.TripleQuery(query);

                    SitecoreTriples = triples.ToSitecoreTriples();
                }
                else if (query.QueryType == SparqlQueryType.Select || query.QueryType == SparqlQueryType.SelectAll)
                {
                    var resultSet = manager.ResultSetQuery(query);

                    foreach (SparqlResult result in resultSet)
                    {
                        foreach (var variable in result.ToList())
                        {
                            var sitecoreNode = variable.Value.ToSitecoreNode();

                            if (sitecoreNode != null)
                            {
                                litSparqlQueryResult.Text += string.Format("Key: {0} Value: <a href=\"{1}\">{2}</a><br/>",
                                    variable.Key, LinkManager.GetItemUrl(sitecoreNode.Item), sitecoreNode.Item.Name);
                            }
                            else
                            {
                                litSparqlQueryResult.Text += string.Format("Key: {0} Value: {1}<br/>",
                                    variable.Key, variable.Value.ToString());
                            }
                        }                        
                    }                    
                }
            }
        }
    }
}