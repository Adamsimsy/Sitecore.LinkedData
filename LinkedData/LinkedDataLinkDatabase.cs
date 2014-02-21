using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Data.DataProviders.Sql;
using Sitecore.Data.SqlServer;
using Sitecore.Diagnostics;
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
  }
}
