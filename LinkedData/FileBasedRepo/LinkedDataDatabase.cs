using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Sitecore;
using Sitecore.Collections;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Diagnostics.PerformanceCounters;
using Sitecore.Globalization;
using Sitecore.Jobs;
using Sitecore.Links;
using Sitecore.SecurityModel;

namespace LinkedData.FileBasedRepo
{
    public class LinkedDataDatabase : LinkDatabase
    {
        private readonly LockSet locks = new LockSet();

        public LinkedDataDatabase(string connectionString)
        {

        }

        public override void Compact(Database database)
        {
            Assert.ArgumentNotNull((object)database, "database");
        }

        public override ItemLink[] GetBrokenLinks(Database database)
        {
            Assert.ArgumentNotNull((object)database, "database");

            List<ItemLink> links = new List<ItemLink>();
            return links.ToArray();
        }

        public override int GetReferenceCount(Item item)
        {
            Assert.ArgumentNotNull((object)item, "item");

            var g = LinkedDataManager.ReadGraph();

            var items = g.GetTriplesWithSubject(g.CreateUriNode(LinkedDataManager.ItemToUri(item)));

            if (items != null)
            {
                return items.Count();
            }
            return 0;
        }

        public override ItemLink[] GetReferences(Item item)
        {
            Assert.ArgumentNotNull((object)item, "item");

            var list = new List<ItemLink>();
            lock (this.locks.GetLock((object)item.ID))
            {
                var g = LinkedDataManager.ReadGraph();

                var items = g.GetTriplesWithSubject(g.CreateUriNode(LinkedDataManager.ItemToUri(item)));

                foreach (var triple in items)
                {
                    var sourceItem = LinkedDataManager.UriToItem(triple.Subject.ToString());
                    var targetItem = LinkedDataManager.UriToItem(triple.Object.ToString());
                    //TODO: Need to hold somewhere in the triple the fieldId
                    if (sourceItem != null && targetItem != null)
                    {
                        list.Add(new ItemLink(sourceItem.Database.Name, sourceItem.ID,
                            new ID("{A60ACD61-A6DB-4182-8329-C957982CEC74}"), targetItem.Database.Name, targetItem.ID,
                            targetItem.Paths.FullPath));
                    }
                }
            }
            return list.ToArray();
        }

        public override int GetReferrerCount(Item item)
        {
            Assert.ArgumentNotNull((object)item, "item");

            var g = LinkedDataManager.ReadGraph();

            var items = g.GetTriplesWithObject(g.CreateUriNode(LinkedDataManager.ItemToUri(item)));

            if (items != null)
            {
                return items.Count();
            }
            return 0;
        }

        public override ItemLink[] GetReferrers(Item item)
        {
            Assert.ArgumentNotNull((object)item, "item");

            var list = new List<ItemLink>();
            lock (this.locks.GetLock((object)item.ID))
            {
                var g = LinkedDataManager.ReadGraph();

                var literalNode = g.GetLiteralNode(LinkedDataManager.ItemToUri(item));

                if (literalNode != null)
                {
                    var items = g.GetTriplesWithObject(literalNode);

                    foreach (var triple in items)
                    {
                        var sourceItem = LinkedDataManager.UriToItem(triple.Subject.ToString());
                        var targetItem = LinkedDataManager.UriToItem(triple.Object.ToString());
                        //TODO: Need to hold somewhere in the triple the fieldId
                        list.Add(new ItemLink(sourceItem.Database.Name, sourceItem.ID, new ID("{A60ACD61-A6DB-4182-8329-C957982CEC74}"), targetItem.Database.Name, targetItem.ID, targetItem.Paths.FullPath));
                    }
                }
            }
            return list.ToArray();
        }

        public override ItemLink[] GetReferrers(Item item, ID sourceFieldId)
        {
            Assert.ArgumentNotNull((object)item, "item");
            Assert.ArgumentNotNull((object)sourceFieldId, "sourceFieldId");

            var list = new List<ItemLink>();
            lock (this.locks.GetLock((object)item.ID))
            {
                var g = LinkedDataManager.ReadGraph();

                var items = g.GetTriplesWithObject(g.CreateUriNode(LinkedDataManager.ItemToUri(item)));

                foreach (var triple in items)
                {
                    var sourceItem = LinkedDataManager.UriToItem(triple.Subject.ToString());
                    var targetItem = LinkedDataManager.UriToItem(triple.Object.ToString());
                    //TODO: Need to hold somewhere in the triple the fieldId
                    list.Add(new ItemLink(sourceItem.Database.Name, sourceItem.ID, new ID("{A60ACD61-A6DB-4182-8329-C957982CEC74}"), targetItem.Database.Name, targetItem.ID, targetItem.Paths.FullPath));
                }
            }
            return list.ToArray();
        }

        public override ItemLink[] GetItemVersionReferrers(Item version)
        {
            Assert.ArgumentNotNull((object)version, "version");
            var list = new List<ItemLink>();
            lock (this.locks.GetLock((object)version.ID))
            {
                var g = LinkedDataManager.ReadGraph();

                var items = g.GetTriplesWithObject(g.CreateUriNode(LinkedDataManager.ItemToUri(version)));

                foreach (var triple in items)
                {
                    var sourceItem = LinkedDataManager.UriToItem(triple.Subject.ToString());
                    var targetItem = LinkedDataManager.UriToItem(triple.Object.ToString());
                    //TODO: Need to hold somewhere in the triple the fieldId
                    list.Add(new ItemLink(sourceItem.Database.Name, sourceItem.ID, new ID("{A60ACD61-A6DB-4182-8329-C957982CEC74}"), targetItem.Database.Name, targetItem.ID, targetItem.Paths.FullPath));
                }
            }
            return list.ToArray();
        }

        [Obsolete("Deprecated - Use GetReferers(Item) instead.")]
        public override ItemLink[] GetReferrers(Item item, bool deep)
        {
            Assert.ArgumentNotNull((object)item, "item");
            if (!deep)
                return this.GetReferrers(item);
            else
                return this.GetHTMLReferersDeep(item);
        }

        public override void RemoveReferences(Item item)
        {
            Assert.ArgumentNotNull((object)item, "item");

            var g = LinkedDataManager.ReadGraph();

            var items = g.GetTriplesWithObject(g.CreateUriNode(LinkedDataManager.ItemToUri(item)));

            foreach (var triple in items)
            {
                g.Retract(triple);
            }
            LinkedDataManager.WriteGraph(g);

            LinkCounters.DataUpdated.Increment();
        }

        protected virtual void AddLink(Item item, ItemLink link)
        {
            Assert.ArgumentNotNull((object)item, "item");
            Assert.ArgumentNotNull((object)link, "link");

            lock (this.locks.GetLock((object)"rdflock"))
            {
                LinkedDataManager.AddLink(item, link);
            }
        }

        protected virtual string GetString(string value, int maxLength)
        {
            Assert.ArgumentNotNull((object)value, "value");
            if (value.Length <= maxLength)
                return value;
            string str = value.Substring(0, maxLength);
            Log.Warn("A string had to be truncated before being written to the link database.", (object)this);
            Log.Warn("Original value: '" + value + "'.", (object)this);
            Log.Warn("Truncated value: '" + str + "'.", (object)this);
            return value.Substring(0, maxLength);
        }

        protected virtual void RemoveLinks(Item item)
        {
            Assert.ArgumentNotNull((object)item, "item");

            var g = LinkedDataManager.ReadGraph();

            var items = g.GetTriplesWithSubject(g.CreateUriNode(LinkedDataManager.ItemToUri(item)));

            foreach (var triple in items)
            {
                g.Retract(triple);
            }
            LinkedDataManager.WriteGraph(g);
        }

        protected virtual void RemoveItemVersionLinks(Item item)
        {
            Assert.ArgumentNotNull((object)item, "item");

            var g = LinkedDataManager.ReadGraph();

            var items = g.GetTriplesWithSubject(g.CreateUriNode(LinkedDataManager.ItemToUri(item)));

            foreach (var triple in items)
            {
                g.Retract(triple);
            }
            LinkedDataManager.WriteGraph(g);
        }

        protected override void UpdateLinks(Item item, ItemLink[] links)
        {
            Assert.ArgumentNotNull((object)item, "item");
            Assert.ArgumentNotNull((object)links, "links");

            foreach (var itemLink in links)
            {
                LinkedDataManager.AddLink(item, itemLink);
            }
        }

        protected override void UpdateItemVersionLinks(Item item, ItemLink[] links)
        {
            Assert.ArgumentNotNull((object)item, "item");
            Assert.ArgumentNotNull((object)links, "links");

            foreach (var itemLink in links)
            {
                LinkedDataManager.AddLink(item, itemLink);
            }

            //Now remove removed links
            var oldLinks = GetReferences(item);
            var removeLinks = new List<ItemLink>();

            foreach (var link in oldLinks)
            {
                if (!links.ToList().Where(x => x.TargetItemID.Guid == link.TargetItemID.Guid).Any())
                {
                    removeLinks.Add(link);
                }
            }

            foreach (var removeLink in removeLinks)
            {
                LinkedDataManager.RemoveLinksForItem(item, removeLink);
            }
        }

        private void AddBrokenLinks(IDataReader reader, List<ItemLink> links, Database database)
        {
            Assert.ArgumentNotNull((object)reader, "reader");
            Assert.ArgumentNotNull((object)links, "links");
            Assert.ArgumentNotNull((object)database, "database");
            using (new SecurityDisabler())
            {
                string name = database.Name;
                while (reader.Read())
                {
                    ID sourceItemID = ID.Parse(reader.GetGuid(0));
                    Language sourceItemLanguage = Language.Parse(reader.GetString(1));
                    Sitecore.Data.Version sourceItemVersion = Sitecore.Data.Version.Parse(reader.GetInt32(2));
                    ID sourceFieldID = ID.Parse(reader.GetGuid(3));
                    string string1 = reader.GetString(4);
                    ID id = ID.Parse(reader.GetGuid(5));
                    Language language = Language.Parse(reader.GetString(6));
                    Sitecore.Data.Version version = Sitecore.Data.Version.Parse(reader.GetInt32(7));
                    string string2 = reader.GetString(8);
                    Database database1 = Factory.GetDatabase(string1);
                    if (!this.ItemExists(id, string2, language, version, database1))
                        links.Add(new ItemLink(name, sourceItemID, sourceItemLanguage, sourceItemVersion, sourceFieldID, name, id, language, version, string2));
                    Job job = Context.Job;
                    if (job != null && job.Category == "GetBrokenLinks")
                        ++job.Status.Processed;
                    LinkCounters.DataRead.Increment();
                    DataCounters.PhysicalReads.Increment();
                }
            }
        }

        [Obsolete("Deprecated - Use GetReferers(item) instead.")]
        private ItemLink[] GetHTMLReferersDeep(Item item)
        {
            Assert.ArgumentNotNull((object)item, "item");

            return GetReferrers(item);
        }
    }
}