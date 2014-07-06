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

            //var triples = manager.GetItemTriplesBySubject(Sitecore.Context.Item);
            var playersInLeaguesFormat = @"CONSTRUCT {{ <{0}> ?p2 ?o2 }} WHERE {{ <{0}> ?p ?o . ?o ?p2 ?o2 .}}";

            var triples = manager.GetTriples(String.Format(playersInLeaguesFormat, SitecoreTripleHelper.ItemToUri(Sitecore.Context.Item)));

            SitecoreTriples = triples.ToSitecoreTriples();

            //foreach (var triple in triples)
            //{
            //    var sitecoreTriple = triple.ToSitecoreTriple();

            //    litRdf.Text += "Subject: <a href=\"" + triple.Subject.ToString() + "\">" + sitecoreTriple.SubjectItem.Name + "</a>"
            //                   + " Predicate: <a href=\"" + triple.Predicate.ToString() + "\">" + sitecoreTriple.PredicateNode.ToString() + "</a>" +
            //                   " Object: <a href=\"" + triple.Object.ToString() + "\">" + sitecoreTriple.ObjectItem.Name + "</a><br/>";
            //}

            //foreach (var triple in triples)
            //{
            //    litRdf.Text += "Subject: <a href=\"" + triple.Subject.ToString() + "\">" + triple.Subject.ToString() + "</a>"
            //                   + " Predicate: <a href=\"" + triple.Predicate.ToString() + "\">" + triple.Predicate.ToString() + "</a>" +
            //                   " Object: <a href=\"" + triple.Object.ToString() + "\">" + triple.Object.ToString() + "</a><br/>";
            //}
        }
    }
}