using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Medical.Utility.LuceneUtil
{
    public class ActiveFacetCollection
    {
        private const String format = "{0}|{1}";

        private HashSet<String> activeFacets = new HashSet<string>();

        public void add(String field, String value)
        {
            activeFacets.Add(String.Format(format, field, value));
        }

        public bool isActive(String field, String value)
        {
            return activeFacets.Contains(String.Format(format, field, value));
        }
    }
}