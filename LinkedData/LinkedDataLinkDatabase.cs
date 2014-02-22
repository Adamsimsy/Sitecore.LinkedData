using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinkedData.Repository;
using Sitecore.Collections;
using Sitecore.Data;
using Sitecore.Data.DataProviders.Sql;
using Sitecore.Data.Items;
using Sitecore.Data.SqlServer;
using Sitecore.Diagnostics;
using Sitecore.Diagnostics.PerformanceCounters;
using Sitecore.Globalization;
using Sitecore.Links;

namespace LinkedData
{
    public class LinkedDataLinkDatabase : SqlLinkDatabase
  {
    private readonly string connectionString;

    public virtual string ConnectionString
    {
      get
      {
        return this.connectionString;
      }
    }

    public LinkedDataLinkDatabase(string connectionString)
        : base(new SqlServerDataApi(connectionString))
    {
      Assert.ArgumentNotNullOrEmpty(connectionString, "connectionString");
      this.connectionString = SqlServerDataApi.MapFileNames(connectionString);
    }

    public override ItemLink[] GetReferences(Item item)
    {
        Assert.ArgumentNotNull((object)item, "item");
        return this.GetLinks(" SELECT {0}SourceDatabase{1}, {0}SourceItemID{1}, {0}SourceLanguage{1}, {0}SourceVersion{1}, {0}SourceFieldID{1}, {0}TargetDatabase{1}, {0}TargetItemID{1}, {0}TargetLanguage{1}, {0}TargetVersion{1}, {0}TargetPath{1} FROM {0}Links{1} WHERE {0}SourceItemID{1} = {2}itemID{3} AND {0}SourceDatabase{1} = {2}database{3}", item);
    }

        protected override void AddLink(Item item, ItemLink link)
    {
        Assert.ArgumentNotNull((object)item, "item");
        Assert.ArgumentNotNull((object)link, "link");

        var g = LinkedDataManager.ReadGraph();

        LinkedDataManager.WriteTriple(g, LinkedDataManager.ToTriple(g, item, link));

        //this.DataApi.Execute(" INSERT INTO {0}Links{1}(   {0}SourceDatabase{1}, {0}SourceItemID{1}, {0}SourceLanguage{1}, {0}SourceVersion{1}, {0}SourceFieldID{1}, {0}TargetDatabase{1}, {0}TargetItemID{1}, {0}TargetLanguage{1}, {0}TargetVersion{1}, {0}TargetPath{1} ) VALUES(  {2}database{3}, {2}itemID{3}, {2}sourceLanguage{3}, {2}sourceVersion{3}, {2}fieldID{3}, {2}targetDatabase{3}, {2}targetID{3}, {2}targetLanguage{3}, {2}targetVersion{3}, {2}targetPath{3} )", (object)"itemID", (object)item.ID.ToGuid(), (object)"database", (object)this.GetString(item.Database.Name, 50), (object)"fieldID", (object)link.SourceFieldID.ToGuid(), (object)"sourceLanguage", (object)this.GetString(link.SourceItemLanguage.ToString(), 50), (object)"sourceVersion", (object)link.SourceItemVersion.Number, (object)"targetDatabase", (object)this.GetString(link.TargetDatabaseName, 50), (object)"targetID", (object)link.TargetItemID.ToGuid(), (object)"targetLanguage", (object)this.GetString(link.TargetItemLanguage.ToString(), 50), (object)"targetVersion", (object)link.TargetItemVersion.Number, (object)"targetPath", (object)link.TargetPath);
    }

    private ItemLink[] GetLinks(string sql, Item item, object[] parameters)
    {
        List<ItemLink> list = new List<ItemLink>();
        lock (this.locks.GetLock((object)item.ID))
        {
            var g = LinkedDataManager.ReadGraph();

            var items = g.GetTriplesWithSubject(g.CreateUriNode(LinkedDataManager.ItemToUri(item)));

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
    private readonly LockSet locks = new LockSet();
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
  }
}
