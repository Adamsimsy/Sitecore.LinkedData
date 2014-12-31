using LinkedData.DataManagers;
using LinkedData.Helpers;
using Sitecore;
using Sitecore.Collections;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Links;
using Sitecore.Resources;
using Sitecore.Shell;
using Sitecore.Shell.Applications.ContentManager.Galleries;
using Sitecore.Web.UI.HtmlControls;
using Sitecore.Web.UI.Sheer;
using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Linq;

namespace LinkedData.ContentManager
{
    public class GalleryLinksForm : GalleryForm
    {
        /// <summary/>
        protected Border Links;

        /// <summary>
        /// Handles the message.
        /// 
        /// </summary>
        /// <param name="message">The message.</param>
        public override void HandleMessage(Message message)
        {
            Assert.ArgumentNotNull((object) message, "message");
            this.Invoke(message, true);
            message.CancelBubble = true;
            message.CancelDispatch = true;
        }

        /// <summary>
        /// Raises the load event.
        /// 
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            Assert.ArgumentNotNull((object) e, "e");
            base.OnLoad(e);

            if (Context.ClientPage.IsEvent)
                return;

            StringBuilder result = new StringBuilder();

            Item itemFromQueryString = UIUtil.GetItemFromQueryString(Context.ContentDatabase);

            if (itemFromQueryString != null)
            {
                //

                var _factory = DependencyResolver.Instance.Resolve<SitecoreManagerFactory>();

                var referrers = _factory.GetContextLinkDatabaseDataManager(itemFromQueryString).GetItemTriplesByObject(itemFromQueryString);
                var references = _factory.GetContextLinkDatabaseDataManager(itemFromQueryString).GetItemTriplesBySubject(itemFromQueryString);

                if (referrers.Count() > 0)
                {
                    RenderTriples(result, referrers, false);
                }

                if (references.Count() > 0)
                {
                    RenderTriples(result, references, true);
                }                                        
            }

            if (result.Length == 0)
            {
                result.Append(Translate.Text("This item has no references."));
            }
                
            this.Links.Controls.Add((System.Web.UI.Control) new LiteralControl(((object) result).ToString()));
        }

        private void RenderTriples(StringBuilder result, IEnumerable<VDS.RDF.Triple> referrers, bool refferences)
        {
            string heading = string.Empty;

            if (refferences)
            {
                heading = "References with predicate: ";
            }
            else
            {
                heading = "Referrers with predicate: ";
            }

            foreach (var predicateGroup in referrers.ToSitecoreTriples().GroupBy(x => SitecoreTripleHelper.RemoveLinkFieldFromPredicate(x.PredicateNode)))
            {
                var triples = predicateGroup.AsEnumerable();

                result.Append("<div style=\"font-size:14px; padding:10px 15px 10px;background-color:#474747; color:white\"><span style=\"font-weight:bold; \">" + Translate.Text(heading) + "</span>" + predicateGroup.Key.ToString() + "</div>");

                foreach (var triple in triples)
                {
                    RenderLink(result, triple, refferences);
                }                
            }
        }

        private void RenderLink(StringBuilder result, SitecoreTriple triple, bool refferences)
        {
            Item item;

            if (refferences)
            {
                item = triple.ObjectItem;
            }
            else
            {
                item = triple.SubjectItem;
            }

            if (IsHidden(item) && !UserOptions.View.ShowHiddenItems)
            {
                return;
            }

            if (item == null)
            {
                result.Append(string.Format("<div class=\"scLink\">{0} {1}: {2}, {3}</div>",
                    (object)Images.GetImage("Applications/16x16/error.png", 16, 16, "absmiddle", "0px 4px 0px 0px"),
                    (object)Translate.Text("Not found"), "TODO: Target database for link",
                    item.ToString()));
            }
            else
            {
                result.Append("<a href=\"#\" class=\"scLink\" onclick='javascript:return scForm.invoke(\"item:load(id=" +
                            (object)item.ID + ",language=" + item.Language.ToString() + ",version=" + item.Version.ToString() + ")\")'>" +
                            Images.GetImage(item.Appearance.Icon, 16, 16, "absmiddle", "0px 4px 0px 0px") +
                            item.DisplayName);


                if (!SitecoreTripleHelper.GetFieldIdFromPredicate(triple.PredicateNode).IsNull)
                {
                    var linkField = triple.SubjectItem.Fields[SitecoreTripleHelper.GetFieldIdFromPredicate(triple.PredicateNode)];

                    if (linkField != null)
                    {
                        if (!string.IsNullOrEmpty(linkField.DisplayName))
                        {
                            result.Append(" - ");
                            result.Append(linkField.DisplayName);

                            if (!linkField.HasValue)
                            {
                                result.Append(" <span style=\"color:#999999\">");
                                result.Append(Translate.Text("[inherited]"));
                                result.Append("</span>");
                            }
                        }
                    }
                }

                result.Append(" - [" + item.Paths.Path + "]</a>");
            }
        }

        /// <summary>
        /// Determines whether the specified reference is hidden.
        /// 
        /// </summary>
        /// <param name="item">The reference.</param>
        /// <returns>
        /// <c>true</c> if the specified reference is hidden; otherwise, <c>false</c>.
        /// 
        /// </returns>
        private bool IsHidden(Item item)
        {
            for (; item != null; item = item.Parent)
            {
                if (item.Appearance.Hidden)
                    return true;
            }
            return false;
        }
    }
}
