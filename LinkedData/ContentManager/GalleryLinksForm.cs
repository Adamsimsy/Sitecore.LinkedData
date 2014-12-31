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
                //if (part1 == null || !this.IsHidden(part1) || UserOptions.View.ShowHiddenItems)

                var _factory = DependencyResolver.Instance.Resolve<SitecoreManagerFactory>();

                var referrers = _factory.GetContextLinkDatabaseDataManager(itemFromQueryString).GetItemTriplesByObject(itemFromQueryString);
                var references = _factory.GetContextLinkDatabaseDataManager(itemFromQueryString).GetItemTriplesBySubject(itemFromQueryString);

                if (referrers.Count() > 0)
                {
                    RenderReferrersTriples(result, referrers);
                }

                if (references.Count() > 0)
                {
                    RenderReferencesTriples(result, references);
                }                                        
            }

            if (result.Length == 0)
            {
                result.Append(Translate.Text("This item has no references."));
            }
                
            this.Links.Controls.Add((System.Web.UI.Control) new LiteralControl(((object) result).ToString()));
        }

        private void RenderReferencesTriples(StringBuilder result, IEnumerable<VDS.RDF.Triple> references)
        {
            result.Append("<div style=\"font-weight:bold;padding:2px 0px 4px 0px\">" + Translate.Text("References:") + "</div>");

            foreach (var triple in references.ToSitecoreTriples())
            {
                if (triple.ObjectItem == null)
                {
                    result.Append(string.Format("<div class=\"scLink\">{0} {1}: {2}, {3}</div>",
                        (object)Images.GetImage("Applications/16x16/error.png", 16, 16, "absmiddle", "0px 4px 0px 0px"),
                        (object)Translate.Text("Not found"), "TODO: Target database for link",
                        triple.ObjectNode.ToString()));
                }
                else
                {
                    result.Append(
                        "<a href=\"#\" class=\"scLink\" onclick='javascript:return scForm.invoke(\"item:load(id=" +
                        (object)triple.ObjectItem.ID + ",language=" + triple.ObjectItem.Language + ",version=" +
                        triple.ObjectItem.Version + ")\")'>" +
                        Images.GetImage(triple.ObjectItem.Appearance.Icon, 16, 16, "absmiddle", "0px 4px 0px 0px") +
                        triple.ObjectItem.DisplayName + " - [" + triple.ObjectItem.Paths.Path + "]</a>");
                }
            }
        }

        private void RenderReferrersTriples(StringBuilder result, IEnumerable<VDS.RDF.Triple> referrers)
        {
            result.Append("<div style=\"font-weight:bold;padding:2px 0px 4px 0px\">" + Translate.Text("Referrers:") + "</div>");

            foreach (var triple in referrers.ToSitecoreTriples())
            {
                if (triple.SubjectItem == null)
                {
                    result.Append(string.Format("<div class=\"scLink\">{0} {1}: {2}, {3}</div>",
                        (object)Images.GetImage("Applications/16x16/error.png", 16, 16, "absmiddle", "0px 4px 0px 0px"),
                        (object)Translate.Text("Not found"), "TODO: Target database for link",
                        triple.SubjectItem.ToString()));
                }
                else
                {
                    result.Append(
                        "<a href=\"#\" class=\"scLink\" onclick='javascript:return scForm.invoke(\"item:load(id=" +
                        (object)triple.SubjectItem.ID + ",language=" + triple.SubjectItem.Language.ToString() + ",version=" + triple.SubjectItem.Version.ToString() + ")\")'>" +
                        Images.GetImage(triple.SubjectItem.Appearance.Icon, 16, 16, "absmiddle", "0px 4px 0px 0px") +
                        triple.SubjectItem.DisplayName);
                    if (triple.SubjectItem != null && !SitecoreTripleHelper.GetFieldIdFromPredicate(triple.PredicateNode).IsNull)
                    {
                        Field field1 = triple.SubjectItem.Fields[SitecoreTripleHelper.GetFieldIdFromPredicate(triple.PredicateNode)];
                        if (!string.IsNullOrEmpty(field1.DisplayName))
                        {
                            result.Append(" - ");
                            result.Append(field1.DisplayName);
                            if (triple.SubjectItem != null)
                            {
                                Field field2 = triple.SubjectItem.Fields[SitecoreTripleHelper.GetFieldIdFromPredicate(triple.PredicateNode)];
                                if (field2 != null && !field2.HasValue)
                                {
                                    result.Append(" <span style=\"color:#999999\">");
                                    result.Append(Translate.Text("[inherited]"));
                                    result.Append("</span>");
                                }
                            }
                        }
                    }
                    result.Append(" - [" + triple.SubjectItem.Paths.Path + "]</a>");
                }
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
