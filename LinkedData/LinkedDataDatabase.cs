using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore;
using Sitecore.Collections;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.DataProviders.Sql;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Diagnostics.PerformanceCounters;
using Sitecore.Globalization;
using Sitecore.Jobs;
using Sitecore.Links;
using Sitecore.SecurityModel;

namespace LinkedData
{
    class LinkedDataDatabase : LinkDatabase
  {
    private readonly LockSet locks = new LockSet();
    [Obsolete("Deprecated - Use protected DataApi property instead. Field is going to be made private.")]
    protected readonly SqlDataApi _dataApi;

    protected SqlDataApi DataApi
    {
      get
      {
        return this._dataApi;
      }
    }

    public LinkedDataDatabase(SqlDataApi dataApi)
    {
      Assert.ArgumentNotNull((object) dataApi, "dataApi");
      this._dataApi = dataApi;
    }

    public override void Compact(Database database)
    {
      Assert.ArgumentNotNull((object) database, "database");
      string sql1 = " SELECT {0}ID{1}, {0}SourceItemID{1}, {0}SourceLanguage{1}, {0}SourceVersion{1}\r\n                      FROM {0}Links{1}\r\n                      WHERE {0}SourceDatabase{1} = {2}database{3}";
      string sql2 = " DELETE FROM {0}Links{1} \r\n                            WHERE {0}ID{1} = {2}id{3}";
      using (DataProviderReader reader = this.DataApi.CreateReader(sql1, (object) "database", (object) database.Name))
      {
        while (reader.Read())
        {
          LinkCounters.DataRead.Increment();
          Guid guid = reader.InnerReader.GetGuid(0);
          if (!this.ItemExists(ID.Parse(reader.InnerReader.GetGuid(1)), (string) null, Language.Parse(reader.InnerReader.GetString(2)), Sitecore.Data.Version.Parse(reader.InnerReader.GetInt32(3)), database))
            this.DataApi.Execute(sql2, (object) "id", (object) guid);
        }
      }
    }

    public override ItemLink[] GetBrokenLinks(Database database)
    {
      Assert.ArgumentNotNull((object) database, "database");
      using (DataProviderReader reader = this.DataApi.CreateReader(" SELECT {0}SourceItemID{1}, {0}SourceLanguage{1}, {0}SourceVersion{1}, {0}SourceFieldID{1}, {0}TargetDatabase{1}, {0}TargetItemID{1}, {0}TargetLanguage{1}, {0}TargetVersion{1}, {0}TargetPath{1}\r\n                      FROM {0}Links{1}\r\n                      WHERE {0}SourceDatabase{1} = {2}database{3}\r\n                      ORDER BY {0}SourceItemID{1}, {0}SourceFieldID{1}", (object) "database", (object) database.Name))
      {
        LinkCounters.DataRead.Increment();
        List<ItemLink> links = new List<ItemLink>();
        this.AddBrokenLinks(reader.InnerReader, links, database);
        return links.ToArray();
      }
    }

    public override int GetReferenceCount(Item item)
    {
      Assert.ArgumentNotNull((object) item, "item");
      using (DataProviderReader reader = this.DataApi.CreateReader(" SELECT COUNT(*)\r\n                      FROM {0}Links{1}\r\n                      WHERE {0}SourceItemID{1} = {2}itemID{3}\r\n                      AND {0}SourceDatabase{1} = {2}database{3}", (object) "database", (object) item.Database.Name, (object) "itemID", (object) item.ID.ToGuid()))
      {
        LinkCounters.DataRead.Increment();
        if (reader.Read())
          return reader.InnerReader.GetInt32(0);
      }
      return 0;
    }

    public override ItemLink[] GetReferences(Item item)
    {
      Assert.ArgumentNotNull((object) item, "item");
      return this.GetLinks(" SELECT {0}SourceDatabase{1}, {0}SourceItemID{1}, {0}SourceLanguage{1}, {0}SourceVersion{1}, {0}SourceFieldID{1}, {0}TargetDatabase{1}, {0}TargetItemID{1}, {0}TargetLanguage{1}, {0}TargetVersion{1}, {0}TargetPath{1} FROM {0}Links{1} WHERE {0}SourceItemID{1} = {2}itemID{3} AND {0}SourceDatabase{1} = {2}database{3}", item);
    }

    public override int GetReferrerCount(Item item)
    {
      Assert.ArgumentNotNull((object) item, "item");
      using (DataProviderReader reader = this.DataApi.CreateReader(" SELECT COUNT(*)\r\n                      FROM {0}Links{1}\r\n                      WHERE {0}TargetItemID{1} = {2}itemID{3}\r\n                      AND {0}TargetDatabase{1} = {2}database{3}", (object) "database", (object) item.Database.Name, (object) "itemID", (object) item.ID.ToGuid()))
      {
        LinkCounters.DataRead.Increment();
        if (reader.Read())
          return reader.InnerReader.GetInt32(0);
      }
      return 0;
    }

    public override ItemLink[] GetReferrers(Item item)
    {
      Assert.ArgumentNotNull((object) item, "item");
      return this.GetLinks(" SELECT {0}SourceDatabase{1}, {0}SourceItemID{1}, {0}SourceLanguage{1}, {0}SourceVersion{1}, {0}SourceFieldID{1}, {0}TargetDatabase{1}, {0}TargetItemID{1}, {0}TargetLanguage{1}, {0}TargetVersion{1}, {0}TargetPath{1} FROM {0}Links{1} WHERE {0}TargetItemID{1} = {2}itemID{3} AND {0}TargetDatabase{1} = {2}database{3}", item);
    }

    public override ItemLink[] GetReferrers(Item item, ID sourceFieldId)
    {
      Assert.ArgumentNotNull((object) item, "item");
      Assert.ArgumentNotNull((object) sourceFieldId, "sourceFieldId");
      return this.GetLinks(" SELECT {0}SourceDatabase{1}, {0}SourceItemID{1}, {0}SourceLanguage{1}, {0}SourceVersion{1}, {0}SourceFieldID{1}, {0}TargetDatabase{1}, {0}TargetItemID{1}, {0}TargetLanguage{1}, {0}TargetVersion{1}, {0}TargetPath{1} FROM {0}Links{1} WHERE {0}TargetItemID{1} = {2}itemID{3} AND {0}TargetDatabase{1} = {2}database{3} AND {0}SourceFieldID{1} = {2}sourceFieldID{3}", item, sourceFieldId);
    }

    public override ItemLink[] GetItemVersionReferrers(Item version)
    {
      Assert.ArgumentNotNull((object) version, "version");
      return this.GetLinks(" SELECT {0}SourceDatabase{1}, {0}SourceItemID{1}, {0}SourceLanguage{1}, {0}SourceVersion{1}, {0}SourceFieldID{1}, {0}TargetDatabase{1}, {0}TargetItemID{1}, {0}TargetLanguage{1}, {0}TargetVersion{1}, {0}TargetPath{1}\r\n           FROM {0}Links{1} \r\n           WHERE {0}TargetItemID{1} = {2}itemID{3} AND {0}TargetDatabase{1} = {2}database{3} AND {0}TargetLanguage{1}={2}targetLanguage{3} AND {0}TargetVersion{1}={2}targetVersion{3}", version, new object[8]
      {
        (object) "database",
        (object) version.Database.Name,
        (object) "itemID",
        (object) version.ID.ToGuid(),
        (object) "targetLanguage",
        (object) version.Language.ToString(),
        (object) "targetVersion",
        (object) version.Version.Number
      });
    }

    [Obsolete("Deprecated - Use GetReferers(Item) instead.")]
    public override ItemLink[] GetReferrers(Item item, bool deep)
    {
      Assert.ArgumentNotNull((object) item, "item");
      if (!deep)
        return this.GetReferrers(item);
      else
        return this.GetHTMLReferersDeep(item);
    }

    public override void RemoveReferences(Item item)
    {
      Assert.ArgumentNotNull((object) item, "item");
      this.DataApi.Execute(" DELETE FROM {0}Links{1} WHERE {0}SourceItemID{1} = {2}itemID{3} AND {0}SourceDatabase{1} = {2}database{3}", (object) "itemID", (object) item.ID.ToGuid(), (object) "database", (object) item.Database.Name);
      LinkCounters.DataUpdated.Increment();
    }

    protected virtual void AddLink(Item item, ItemLink link)
    {
      Assert.ArgumentNotNull((object) item, "item");
      Assert.ArgumentNotNull((object) link, "link");
      this.DataApi.Execute(" INSERT INTO {0}Links{1}(   {0}SourceDatabase{1}, {0}SourceItemID{1}, {0}SourceLanguage{1}, {0}SourceVersion{1}, {0}SourceFieldID{1}, {0}TargetDatabase{1}, {0}TargetItemID{1}, {0}TargetLanguage{1}, {0}TargetVersion{1}, {0}TargetPath{1} ) VALUES(  {2}database{3}, {2}itemID{3}, {2}sourceLanguage{3}, {2}sourceVersion{3}, {2}fieldID{3}, {2}targetDatabase{3}, {2}targetID{3}, {2}targetLanguage{3}, {2}targetVersion{3}, {2}targetPath{3} )", (object) "itemID", (object) item.ID.ToGuid(), (object) "database", (object) this.GetString(item.Database.Name, 50), (object) "fieldID", (object) link.SourceFieldID.ToGuid(), (object) "sourceLanguage", (object) this.GetString(link.SourceItemLanguage.ToString(), 50), (object) "sourceVersion", (object) link.SourceItemVersion.Number, (object) "targetDatabase", (object) this.GetString(link.TargetDatabaseName, 50), (object) "targetID", (object) link.TargetItemID.ToGuid(), (object) "targetLanguage", (object) this.GetString(link.TargetItemLanguage.ToString(), 50), (object) "targetVersion", (object) link.TargetItemVersion.Number, (object) "targetPath", (object) link.TargetPath);
    }

    protected virtual string GetString(string value, int maxLength)
    {
      Assert.ArgumentNotNull((object) value, "value");
      if (value.Length <= maxLength)
        return value;
      string str = value.Substring(0, maxLength);
      Log.Warn("A string had to be truncated before being written to the link database.", (object) this);
      Log.Warn("Original value: '" + value + "'.", (object) this);
      Log.Warn("Truncated value: '" + str + "'.", (object) this);
      return value.Substring(0, maxLength);
    }

    protected virtual void RemoveLinks(Item item)
    {
      Assert.ArgumentNotNull((object) item, "item");
      this.DataApi.Execute(" DELETE\r\n                      FROM {0}Links{1}\r\n                      WHERE {0}SourceItemID{1} = {2}itemID{3}\r\n                      AND {0}SourceDatabase{1} = {2}database{3}", (object) "itemID", (object) item.ID.ToGuid(), (object) "database", (object) this.GetString(item.Database.Name, 50));
    }

    protected virtual void RemoveItemVersionLinks(Item item)
    {
      Assert.ArgumentNotNull((object) item, "item");
      this.DataApi.Execute(" DELETE\r\n                      FROM {0}Links{1}\r\n                      WHERE {0}SourceItemID{1} = {2}itemID{3} \r\n                      AND (({0}SourceLanguage{1} = {2}sourceLanguage{3} \r\n                      AND {0}SourceVersion{1}  = {2}sourceVersion{3})\r\n                      OR\r\n                      ({0}SourceLanguage{1} = {2}invariantLanguage{3} \r\n                      AND {0}SourceVersion{1}  = {2}latestVersion{3})\r\n                      OR\r\n                      ({0}SourceLanguage{1} = {2}sourceLanguage{3} \r\n                      AND {0}SourceVersion{1}  = {2}latestVersion{3}))\r\n                      AND {0}SourceDatabase{1} = {2}database{3}", (object) "itemID", (object) item.ID.ToGuid(), (object) "sourceLanguage", (object) item.Language.ToString(), (object) "sourceVersion", (object) item.Version.Number, (object) "invariantLanguage", (object) Language.Invariant.ToString(), (object) "latestVersion", (object) Sitecore.Data.Version.Latest.Number, (object) "database", (object) this.GetString(item.Database.Name, 50));
    }

    protected override void UpdateLinks(Item item, ItemLink[] links)
    {
      Assert.ArgumentNotNull((object) item, "item");
      Assert.ArgumentNotNull((object) links, "links");
      lock (this.locks.GetLock((object) item.ID))
        Factory.GetRetryer().ExecuteNoResult((Action) (() =>
        {
          using (DataProviderTransaction resource_0 = this.DataApi.CreateTransaction())
          {
            this.RemoveLinks(item);
            for (int local_1 = 0; local_1 < links.Length; ++local_1)
            {
              ItemLink local_2 = links[local_1];
              if (!local_2.SourceItemID.IsNull)
              {
                this.AddLink(item, local_2);
                LinkCounters.DataUpdated.Increment();
              }
            }
            resource_0.Complete();
          }
        }));
    }

    protected override void UpdateItemVersionLinks(Item item, ItemLink[] links)
    {
      Assert.ArgumentNotNull((object) item, "item");
      Assert.ArgumentNotNull((object) links, "links");
      lock (this.locks.GetLock((object) item.ID))
        Factory.GetRetryer().ExecuteNoResult((Action) (() =>
        {
          using (DataProviderTransaction resource_0 = this.DataApi.CreateTransaction())
          {
            this.RemoveItemVersionLinks(item);
            for (int local_1 = 0; local_1 < links.Length; ++local_1)
            {
              ItemLink local_2 = links[local_1];
              if (!local_2.SourceItemID.IsNull && (local_2.SourceItemLanguage == Language.Invariant && local_2.SourceItemVersion == Sitecore.Data.Version.Latest || local_2.SourceItemLanguage == item.Language && local_2.SourceItemVersion == Sitecore.Data.Version.Latest || local_2.SourceItemLanguage == item.Language && local_2.SourceItemVersion == item.Version))
              {
                this.AddLink(item, local_2);
                LinkCounters.DataUpdated.Increment();
              }
            }
            resource_0.Complete();
          }
        }));
    }

    private void AddBrokenLinks(IDataReader reader, List<ItemLink> links, Database database)
    {
      Assert.ArgumentNotNull((object) reader, "reader");
      Assert.ArgumentNotNull((object) links, "links");
      Assert.ArgumentNotNull((object) database, "database");
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
      Assert.ArgumentNotNull((object) item, "item");
      string str = "/sitecore/content";
      return this.GetLinks(" SELECT {0}SourceDatabase{1}, {0}SourceItemID{1}, {0}SourceLanguage{1}, {0}SourceVersion{1}, {0}SourceFieldID{1}, {0}TargetDatabase{1}, {0}TargetItemID{1}, {0}TargetLanguage{1}, {0}TargetVersion{1}, {0}TargetPath{1} FROM {0}Links{1} WHERE {0}TargetDatabase{1} = {2}database{3} AND ({0}TargetPath{1} LIKE '" + StringUtil.Mid(item.Paths.Path, str.Length + 1) + "{5}')", item);
    }

    private ItemLink[] GetLinks(string sql, Item item)
    {
      return this.GetLinks(sql, item, new object[4]
      {
        (object) "database",
        (object) item.Database.Name,
        (object) "itemID",
        (object) item.ID.ToGuid()
      });
    }

    private ItemLink[] GetLinks(string sql, Item item, ID sourceFieldId)
    {
      Assert.ArgumentNotNull((object) sql, "sql");
      Assert.ArgumentNotNull((object) item, "item");
      Assert.ArgumentNotNull((object) sourceFieldId, "sourceFieldId");
      return this.GetLinks(sql, item, new object[6]
      {
        (object) "database",
        (object) item.Database.Name,
        (object) "itemID",
        (object) item.ID.ToGuid(),
        (object) "sourceFieldID",
        (object) sourceFieldId.ToGuid()
      });
    }

    private ItemLink[] GetLinks(string sql, Item item, object[] parameters)
    {
      List<ItemLink> list = new List<ItemLink>();
      lock (this.locks.GetLock((object) item.ID))
      {
        using (DataProviderReader resource_0 = this.DataApi.CreateReader(sql, parameters))
        {
          while (resource_0.Read())
          {
            string local_2 = resource_0.InnerReader.GetString(0);
            ID local_3 = ID.Parse(resource_0.InnerReader.GetGuid(1));
            Language local_4 = Language.Parse(resource_0.InnerReader.GetString(2));
            Sitecore.Data.Version local_5 = Sitecore.Data.Version.Parse(resource_0.InnerReader.GetInt32(3));
            ID local_6 = ID.Parse(resource_0.InnerReader.GetGuid(4));
            string local_7 = resource_0.InnerReader.GetString(5);
            ID local_8 = ID.Parse(resource_0.InnerReader.GetGuid(6));
            Language local_9 = Language.Parse(resource_0.InnerReader.GetString(7));
            Sitecore.Data.Version local_10 = Sitecore.Data.Version.Parse(resource_0.InnerReader.GetInt32(8));
            string local_11 = resource_0.InnerReader.GetString(9);
            list.Add(new ItemLink(local_2, local_3, local_4, local_5, local_6, local_7, local_8, local_9, local_10, local_11));
            LinkCounters.DataRead.Increment();
          }
        }
      }
      return list.ToArray();
    }
  }
}