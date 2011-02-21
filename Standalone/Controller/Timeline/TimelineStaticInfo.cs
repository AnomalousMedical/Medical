using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    public class TimelineMatchInfo
    {
        internal TimelineMatchInfo(Type actionType, String comment, String file)
        {
            ActionType = actionType;
            Comment = comment;
            File = file;
        }

        public Type ActionType { get; private set; }

        public String Comment { get; private set; }

        public String File { get; private set; }
    }

    /// <summary>
    /// This class is used in static analysis of timelines.
    /// </summary>
    public abstract class TimelineStaticInfo
    {
        private List<TimelineMatchInfo> matches = new List<TimelineMatchInfo>();

        public void addMatch(Type actionType, String comment, String file)
        {
            matches.Add(new TimelineMatchInfo(actionType, comment, file));
        }

        public void clearMatches()
        {
            matches.Clear();
        }

        public abstract bool matchesPattern(String check);

        public bool HasMatches
        {
            get
            {
                return matches.Count > 0;
            }
        }

        public IEnumerable<TimelineMatchInfo> Matches
        {
            get
            {
                return matches;
            }
        }
    }

    public class ExactMatchStaticInfo : TimelineStaticInfo
    {
        private String searchPattern;

        public ExactMatchStaticInfo(String searchPattern)
        {
            this.searchPattern = searchPattern;
        }

        public override bool matchesPattern(String check)
        {
            if (check != null)
            {
                return check.Contains(searchPattern);
            }
            else
            {
                return check == searchPattern;
            }
        }
    }

    public class EndsWithStaticInfo : TimelineStaticInfo
    {
        private String searchPattern;

        public EndsWithStaticInfo(String searchPattern)
        {
            this.searchPattern = searchPattern;
        }

        public override bool matchesPattern(String check)
        {
            if (check != null)
            {
                return check.EndsWith(searchPattern);
            }
            else
            {
                return check == searchPattern;
            }
        }
    }
}
