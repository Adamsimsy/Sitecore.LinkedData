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

namespace LinkedData.Website.LinkedData
{
    public partial class LinkedDataControl : System.Web.UI.UserControl
    {
        public List<SitecoreTriple> SitecoreTriples { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            var factory = DependencyResolver.Instance.Resolve<SitecoreManagerFactory>();
            var manager = factory.GetContextWebDatabaseDataManager();

            SitecoreTriples = manager.GetItemTriplesBySubject(Sitecore.Context.Item).ToSitecoreTriples();

            //var pred = "http://example.org/sampleitem-to-home";
            var pred2 = "http://example.org/home-to-sampleitem";

            var triples2 = manager.GetItemTriplesBySubjectPredicate(Sitecore.Context.Item, pred2);

            foreach (var triple in triples2)
            {
                //litRdf.Text += triple.ToString();
                litRdf2.Text += "Subject: <a href=\"" + triple.Subject.ToString() + "\">" + triple.Subject.ToString() + "</a>"
                               + " Predicate: <a href=\"" + triple.Predicate.ToString() + "\">" + triple.Predicate.ToString() + "</a>" +
                               " Object: <a href=\"" + triple.Object.ToString() + "\">" + triple.Object.ToString() + "</a><br/>";
            }
        }
    }
}