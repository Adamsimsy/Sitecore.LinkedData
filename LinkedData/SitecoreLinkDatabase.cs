using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using LinkedData.Concepts;
using LinkedData.DataManagers;
using LinkedData.Helpers;
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
using VDS.RDF.Storage;

namespace LinkedData
{
    public class SitecoreLinkDatabase : LinkDatabase
    {
        private readonly LockSet locks = new LockSet();
        private readonly SitecoreManagerFactory _factory;

        public SitecoreLinkDatabase(string connectionString)
        {
            _factory = DependencyResolver.Instance.Resolve<SitecoreManagerFactory>();
        }

        public override void Compact(Database database)
        {
            Assert.ArgumentNotNull((object)database, "database");
        }

        public override ItemLink[] GetBrokenLinks(Database database)
        {
            Assert.ArgumentNotNull((object)database, "database");

            var manager = _factory.GetContextLinkDatabaseDataManager(database);

            var query = @"CONSTRUCT {{ ?s ?p <{0}> }} WHERE {{ ?s ?p <{0}> }}";

            query = String.Format(query, SitecoreTripleHelper.BrokenLinkUri.ToString());

            var triples = manager.GetTriples(query);

            var links = new List<ItemLink>();

            foreach (var triple in triples)
            {
                var sourceItem = SitecoreTripleHelper.UriToItem(triple.Subject.ToString());

                if (sourceItem != null)
                {
                    links.Add(new ItemLink(sourceItem.Database.Name, sourceItem.ID, sourceItem.Language, sourceItem.Version, new ID(SitecoreTripleHelper.GetFieldIdFromPredicate(triple.Predicate.ToString())), sourceItem.Database.Name, new ID(Guid.Empty), sourceItem.Language, sourceItem.Version, string.Empty));
                }

                Job job = Context.Job;
                if (job != null && job.Category == "GetBrokenLinks")
                    ++job.Status.Processed;
                LinkCounters.DataRead.Increment();
                DataCounters.PhysicalReads.Increment();
            }

            
            return links.ToArray();
        }

        public override int GetReferenceCount(Item item)
        {
            Assert.ArgumentNotNull((object)item, "item");

            var items = _factory.GetContextLinkDatabaseDataManager(item).GetItemTriplesBySubject(item);

            if (items != null)
            {
                return items.Count();
            }
            return 0;
        }

        public override ItemLink[] GetReferences(Item item)
        {
            Assert.ArgumentNotNull((object)item, "item");
            List<ItemLink> list;

            lock (this.locks.GetLock((object)item.ID))
            {
                var items = _factory.GetContextLinkDatabaseDataManager(item).GetItemTriplesBySubject(item);

                list = SitecoreTripleHelper.TriplesToItemLinks(items);
            }
            return list.ToArray();
        }

        public override int GetReferrerCount(Item item)
        {
            Assert.ArgumentNotNull((object)item, "item");

            var items = _factory.GetContextLinkDatabaseDataManager(item).GetItemTriplesByObject(item);

            if (items != null)
            {
                return items.Count();
            }
            return 0;
        }

        public override ItemLink[] GetReferrers(Item item)
        {
            Assert.ArgumentNotNull((object)item, "item");
            List<ItemLink> list;
            lock (this.locks.GetLock((object)item.ID))
            {
                var items = _factory.GetContextLinkDatabaseDataManager(item).GetItemTriplesByObject(item);

                list = SitecoreTripleHelper.TriplesToItemLinks(items);
            }
            return list.ToArray();
        }

        public override ItemLink[] GetReferrers(Item item, ID sourceFieldId)
        {
            Assert.ArgumentNotNull((object)item, "item");
            Assert.ArgumentNotNull((object)sourceFieldId, "sourceFieldId");
            List<ItemLink> list;
            lock (this.locks.GetLock((object)item.ID))
            {
                var items = _factory.GetContextLinkDatabaseDataManager(item).GetItemTriplesByObject(item);

                list = SitecoreTripleHelper.TriplesToItemLinks(items);
            }
            return list.ToArray();
        }

        public override ItemLink[] GetItemVersionReferrers(Item version)
        {
            Assert.ArgumentNotNull((object)version, "version");
            List<ItemLink> list;
            lock (this.locks.GetLock((object)version.ID))
            {
                var items = _factory.GetContextLinkDatabaseDataManager(version).GetItemTriplesByObject(version);

                list = SitecoreTripleHelper.TriplesToItemLinks(items);
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

            foreach (var manager in _factory.GetContextSitecoreLinkedDataManagers(item))
            {
                var items = manager.GetItemTriplesByObject(item);

                manager.DeleteTriples(items);
            }

            LinkCounters.DataUpdated.Increment();
        }

        protected virtual void AddLink(Item item, ItemLink link)
        {
            Assert.ArgumentNotNull((object)item, "item");
            Assert.ArgumentNotNull((object)link, "link");

            lock (this.locks.GetLock((object)"rdflock"))
            {
                foreach (var manager in _factory.GetContextSitecoreLinkedDataManagers(item))
                {
                    manager.AddLink(item, link);
                }
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

            foreach (var manager in _factory.GetContextSitecoreLinkedDataManagers(item))
            {
                var items = manager.GetItemTriplesBySubject(item);

                manager.DeleteTriples(items);
            }
        }

        protected virtual void RemoveItemVersionLinks(Item item)
        {
            Assert.ArgumentNotNull((object)item, "item");

            foreach (var manager in _factory.GetContextSitecoreLinkedDataManagers(item))
            {
                var items = manager.GetItemTriplesBySubject(item);

                manager.DeleteTriples(items);
            }
        }

        protected override void UpdateLinks(Item item, ItemLink[] links)
        {
            Assert.ArgumentNotNull((object)item, "item");
            Assert.ArgumentNotNull((object)links, "links");

            foreach (var manager in _factory.GetContextSitecoreLinkedDataManagers(item))
            {
                foreach (var itemLink in links)
                {
                    manager.AddLink(item, itemLink);
                }
            }
        }

        protected override void UpdateItemVersionLinks(Item item, ItemLink[] links)
        {
            Assert.ArgumentNotNull((object)item, "item");
            Assert.ArgumentNotNull((object)links, "links");

            foreach (var manager in _factory.GetContextSitecoreLinkedDataManagers(item))
            {
                foreach (var itemLink in links)
                {
                    manager.AddLink(item, itemLink);
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
                    manager.RemoveLinksForItem(item, removeLink);
                }
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