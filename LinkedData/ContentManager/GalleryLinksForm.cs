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
        /// Gets the references.
        /// 
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>
        /// The references.
        /// 
        /// </returns>
        protected virtual ItemLink[] GetReferences(Item item)
        {
            Assert.ArgumentNotNull((object) item, "item");
            LinkDatabase linkDatabase = Globals.LinkDatabase;
            Assert.IsNotNull((object) linkDatabase, "Link database cannot be null");
            return linkDatabase.GetItemReferences(item, true);
        }

        /// <summary>
        /// Gets the refererers.
        /// 
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>
        /// The refererers.
        /// 
        /// </returns>
        protected virtual ItemLink[] GetRefererers(Item item)
        {
            Assert.ArgumentNotNull((object) item, "item");
            LinkDatabase linkDatabase = Globals.LinkDatabase;
            Assert.IsNotNull((object) linkDatabase, "Link database cannot be null");
            return linkDatabase.GetItemReferrers(item, true);
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
                ItemLink[] refererers = this.GetRefererers(itemFromQueryString);
                List<Pair<Item, ItemLink>> referrers = new List<Pair<Item, ItemLink>>();
                foreach (ItemLink part2 in refererers)
                {
                    Database database = Factory.GetDatabase(part2.SourceDatabaseName, false);
                    if (database != null)
                    {
                        Item part1 = database.Items[part2.SourceItemID];
                        if (part1 == null || !this.IsHidden(part1) || UserOptions.View.ShowHiddenItems)
                            referrers.Add(new Pair<Item, ItemLink>(part1, part2));
                    }
                }
                if (referrers.Count > 0)
                    this.RenderReferrers(result, referrers);
                ItemLink[] references1 = this.GetReferences(itemFromQueryString);
                List<Pair<Item, ItemLink>> references2 = new List<Pair<Item, ItemLink>>();
                foreach (ItemLink part2 in references1)
                {
                    Database database = Factory.GetDatabase(part2.TargetDatabaseName, false);
                    if (database != null)
                    {
                        Item part1 = database.Items[part2.TargetItemID];
                        if (part1 == null || !this.IsHidden(part1) || UserOptions.View.ShowHiddenItems)
                            references2.Add(new Pair<Item, ItemLink>(part1, part2));
                    }
                }
                if (references2.Count > 0)
                    this.RenderReferences(result, references2);
            }
            if (result.Length == 0)
                result.Append(Translate.Text("This item has no references."));
            this.Links.Controls.Add((System.Web.UI.Control) new LiteralControl(((object) result).ToString()));
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

        /// <summary>
        /// Renders the referrers.
        /// 
        /// </summary>
        /// <param name="result">The result.</param><param name="referrers">The referrers.</param>
        private void RenderReferrers(StringBuilder result, List<Pair<Item, ItemLink>> referrers)
        {
            result.Append("<div style=\"font-weight:bold;padding:2px 0px 4px 0px\">" + Translate.Text("Referrers:") +
                          "</div>");
            foreach (Pair<Item, ItemLink> pair in referrers)
            {
                Item part1 = pair.Part1;
                ItemLink part2 = pair.Part2;
                Item obj = (Item) null;
                if (part2 != null)
                    obj = part2.GetSourceItem();
                if (part1 == null)
                {
                    result.Append(string.Format("<div class=\"scLink\">{0} {1}: {2}, {3}</div>",
                        (object) Images.GetImage("Applications/16x16/error.png", 16, 16, "absmiddle", "0px 4px 0px 0px"),
                        (object) Translate.Text("Not found"), (object) part2.SourceDatabaseName,
                        (object) part2.SourceItemID));
                }
                else
                {
                    string str1 = part1.Language.ToString();
                    string str2 = part1.Version.ToString();
                    if (obj != null)
                    {
                        str1 = obj.Language.ToString();
                        str2 = obj.Version.ToString();
                    }
                    result.Append(
                        "<a href=\"#\" class=\"scLink\" onclick='javascript:return scForm.invoke(\"item:load(id=" +
                        (object) part1.ID + ",language=" + str1 + ",version=" + str2 + ")\")'>" +
                        Images.GetImage(part1.Appearance.Icon, 16, 16, "absmiddle", "0px 4px 0px 0px") +
                        part1.DisplayName);
                    if (part2 != null && !part2.SourceFieldID.IsNull)
                    {
                        Field field1 = part1.Fields[part2.SourceFieldID];
                        if (!string.IsNullOrEmpty(field1.DisplayName))
                        {
                            result.Append(" - ");
                            result.Append(field1.DisplayName);
                            if (obj != null)
                            {
                                Field field2 = obj.Fields[part2.SourceFieldID];
                                if (field2 != null && !field2.HasValue)
                                {
                                    result.Append(" <span style=\"color:#999999\">");
                                    result.Append(Translate.Text("[inherited]"));
                                    result.Append("</span>");
                                }
                            }
                        }
                    }
                    result.Append(" - [" + part1.Paths.Path + "]</a>");
                }
            }
        }

        /// <summary>
        /// Renders the references.
        /// 
        /// </summary>
        /// <param name="result">The result.</param><param name="references">The references.</param>
        private void RenderReferences(StringBuilder result, List<Pair<Item, ItemLink>> references)
        {
            result.Append("<div style=\"font-weight:bold;padding:2px 0px 4px 0px\">" + Translate.Text("References:") +
                          "</div>");
            foreach (Pair<Item, ItemLink> pair in references)
            {
                Item part1 = pair.Part1;
                ItemLink part2 = pair.Part2;
                if (part1 == null)
                    result.Append(string.Format("<div class=\"scLink\">{0} {1}: {2}, {3}</div>",
                        (object) Images.GetImage("Applications/16x16/error.png", 16, 16, "absmiddle", "0px 4px 0px 0px"),
                        (object) Translate.Text("Not found"), (object) part2.TargetDatabaseName,
                        (object) part2.TargetItemID));
                else
                    result.Append(
                        "<a href=\"#\" class=\"scLink\" onclick='javascript:return scForm.invoke(\"item:load(id=" +
                        (object)part1.ID + ",language=" + part1.Language + ",version=" +
                        part1.Version + ")\")'>" +
                        Images.GetImage(part1.Appearance.Icon, 16, 16, "absmiddle", "0px 4px 0px 0px") +
                        part1.DisplayName + " - [" + part1.Paths.Path + "]</a>");
            }
        }
    }
}
