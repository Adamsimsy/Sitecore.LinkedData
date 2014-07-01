using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using LinkedData.DataManagers;

namespace LinkedData.Website.LinkedData
{
    public partial class PlayersInLeague : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var manager = DependencyResolver.Instance.Resolve<SitecoreLinkedDataManager>("web");

            var triples = manager.GetItemTriplesBySubject(Sitecore.Context.Item);

            foreach (var triple in triples)
            {
                litRdf.Text += "Subject: <a href=\"" + triple.Subject.ToString() + "\">" + triple.Subject.ToString() + "</a>"
                               + " Predicate: <a href=\"" + triple.Predicate.ToString() + "\">" + triple.Predicate.ToString() + "</a>" +
                               " Object: <a href=\"" + triple.Object.ToString() + "\">" + triple.Object.ToString() + "</a><br/>";
            }
        }
    }
}