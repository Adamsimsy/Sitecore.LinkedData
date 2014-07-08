using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using LinkedData.DataManagers;
using LinkedData.Helpers;

namespace LinkedData.Website.LinkedData
{
    public partial class PlayersInLeague : System.Web.UI.UserControl
    {
        public List<SitecoreTriple> SitecoreTriples { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            var factory = DependencyResolver.Instance.Resolve<SitecoreManagerFactory>();
            var manager = factory.GetContextWebDatabaseDataManager();

            var playersInLeaguesQuery = @"CONSTRUCT {{ <{0}> ?p2 ?o2 }} WHERE {{ <{0}> ?p ?o . ?o ?p2 ?o2 .}}";
            
            //Format the query
            var formattedQuery = String.Format(playersInLeaguesQuery,
                SitecoreTripleHelper.ItemToUri(Sitecore.Context.Item));

            var triples = manager.GetTriples(formattedQuery);

            SitecoreTriples = triples.ToSitecoreTriples();
        }
    }
}