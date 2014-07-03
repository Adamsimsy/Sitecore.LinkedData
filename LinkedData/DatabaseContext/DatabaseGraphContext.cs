using System.Collections.Generic;

namespace LinkedData.DatabaseContext
{
    public class DatabaseGraphContext
    {
        public string DatabaseName { get; set; }
        public IEnumerable<GraphConfiguration> GraphConfigurations { get; set; } 
    }
}
