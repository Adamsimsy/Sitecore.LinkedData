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
        public List<SitecoreTriple> SitecoreReferredTriples { get; set; }
        public List<SitecoreTriple> SitecoreReferringTriples { get; set; }
        public List<SitecoreTriple> SitecorePredicateTriples { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            var factory = DependencyResolver.Instance.Resolve<SitecoreManagerFactory>();
            var manager = factory.GetContextWebDatabaseDataManager();

            SitecoreReferredTriples = manager.GetItemTriplesBySubject(Sitecore.Context.Item).ToSitecoreTriples();

            SitecoreReferringTriples = manager.GetItemTriplesByObject(Sitecore.Context.Item).ToSitecoreTriples();

            //var pred = "http://example.org/sampleitem-to-home";
            var pred = "http://example.org/home-to-sampleitem";

            SitecorePredicateTriples = manager.GetItemTriplesBySubjectPredicate(Sitecore.Context.Item, pred).ToSitecoreTriples(); ;
        }
    }
}