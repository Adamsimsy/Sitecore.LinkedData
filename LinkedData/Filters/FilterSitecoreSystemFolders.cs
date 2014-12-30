using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinkedData.Helpers;
using VDS.RDF;

namespace LinkedData.Filters
{
    public class FilterSitecoreSystemFolders : IFilter
    {
        readonly List<string> _systemFolderPaths = new List<string>() { "/sitecore/templates", "/sitecore/layout", "/sitecore/system" };

        public bool ShouldFilter(Triple triple)
        {
            var subjectItem = SitecoreTripleHelper.UriToItem(triple.Subject.ToString());

            bool shouldFilter = false;

            if (subjectItem != null)
            {
                var subjectPath = subjectItem.Paths.FullPath.ToLower();

                shouldFilter = _systemFolderPaths.Any(subjectPath.StartsWith);
            }

            if (shouldFilter)
                return true;

            var objectItem = SitecoreTripleHelper.UriToItem(triple.Object.ToString());

            if (objectItem != null)
            {
                var objectPath = objectItem.Paths.FullPath.ToLower();

                shouldFilter = _systemFolderPaths.Any(objectPath.StartsWith);
            }

            return shouldFilter;
        }
    }
}
