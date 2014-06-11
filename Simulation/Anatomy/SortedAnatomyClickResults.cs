using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical
{
    public class SortedAnatomyClickResults
    {
        private List<AnatomyClickResult> matchList = new List<AnatomyClickResult>();

        public void add(AnatomyClickResult clickResult)
        {
            matchList.Add(clickResult);
        }

        internal void sort()
        {
            matchList.Sort((x, y) =>
            {
                if (x.Distance < y.Distance)
                {
                    return -1;
                }
                else if (x.Distance > y.Distance)
                {
                    return 1;
                }
                return 0;
            });
        }

        public IEnumerable<AnatomyIdentifier> Anatomy
        {
            get
            {
                return matchList.Select(m => m.AnatomyIdentifier);
            }
        }

        public IEnumerable<AnatomyClickResult> AnatomyWithDistances
        {
            get
            {
                return matchList;
            }
        }

        public int Count
        {
            get
            {
                return matchList.Count;
            }
        }

        public AnatomyIdentifier Closest
        {
            get
            {
                return matchList[0].AnatomyIdentifier;
            }
        }
    }
}
