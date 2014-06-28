using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using LinkedData.Concepts;
using LinkedData.DataManagers;
using LinkedData.Formatters;
using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Storage;
using VDS.RDF.Writing;
using LinkedDataManager = LinkedData.FileBasedRepo.LinkedDataManager;

namespace LinkedData.Website.LinkedData
{
    public partial class LinkedDataControl : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //var manager = DependencyResolver.Instance.Resolve<SitecoreLinkedDataManager>();
            var manager = new SitecoreLinkedDataManager(null,
                new List<ITripleFormatter>() {new UriToDynamicUrlFormatter()},
                DependencyResolver.Instance.Resolve<IQueryableStorage>(),
                DependencyResolver.Instance.Resolve<IConceptManager>());

            var triples = manager.GetItemTriplesBySubject(Sitecore.Context.Item);

            foreach (var triple in triples)
            {
                //litRdf.Text += triple.ToString();
                litRdf.Text += "Subject: <a href=\"" + triple.Subject.ToString() + "\">" + triple.Subject.ToString() + "</a>"
                               + " Predicate: <a href=\"" + triple.Predicate.ToString() + "\">" + triple.Predicate.ToString() + "</a>" +
                               " Object: <a href=\"" + triple.Object.ToString() + "\">" + triple.Object.ToString() + "</a><br/>";
            }
        }
    }
}