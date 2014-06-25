using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical
{
    public class SortedPoseableRaycastResults
    {
        private List<PoseableRaycastResult> matchList = new List<PoseableRaycastResult>();

        public void add(PoseableRaycastResult clickResult)
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

        public IEnumerable<PoseableRaycastResult> Results
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

        public PoseableIdentifier Closest
        {
            get
            {
                return matchList[0].PoseableIdentifier;
            }
        }
    }
}
