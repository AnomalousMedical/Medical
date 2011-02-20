using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    public class TimelineStaticInfo
    {
        private List<String> matches = new List<string>();
        private String searchPattern;

        public void addMatch(Type actionType, String comment)
        {
            matches.Add(String.Format("{0} : {1}", actionType.Name, comment));
        }

        public bool matchesPattern(String check)
        {
            return check.Contains(searchPattern);
        }
    }
}
