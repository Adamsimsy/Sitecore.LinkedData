using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using LinkedData.DataManagers;
using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Writing;
using LinkedDataManager = LinkedData.FileBasedRepo.LinkedDataManager;

namespace LinkedData.Website.LinkedData
{
    public partial class LinkedDataControl : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var manager = DependencyResolver.Instance.Resolve<SitecoreLinkedDataManager>();

            var triples = manager.GetItemTriplesBySubject(Sitecore.Context.Item);

            foreach (var triple in triples)
            {
                litRdf.Text += triple.ToString();
            }
        }
    }
}