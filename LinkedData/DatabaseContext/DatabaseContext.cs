using System.Collections.Generic;

namespace LinkedData.DatabaseContext
{
    public class DatabaseContext
    {
        public string DatabaseName { get; set; }
        public IEnumerable<GraphConfiguration> GraphConfigurations { get; set; } 
    }
}
