using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical
{
    public class SortedAnatomyClickResults
    {
        private List<Tuple<AnatomyIdentifier, float>> matchList = new List<Tuple<AnatomyIdentifier, float>>();

        public void add(AnatomyIdentifier anatomy, float distance)
        {
            matchList.Add(Tuple.Create(anatomy, distance));
        }

        internal void sort()
        {
            matchList.Sort((x, y) =>
            {
                float xDist = x.Item2;
                float yDist = y.Item2;
                if (xDist < yDist)
                {
                    return -1;
                }
                else if (xDist > yDist)
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
                return matchList.Select(m => m.Item1);
            }
        }

        public IEnumerable<Tuple<AnatomyIdentifier, float>> AnatomyWithDistances
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
                return matchList[0].Item1;
            }
        }

        public float ClosestDistance
        {
            get
            {
                return matchList[0].Item2;
            }
        }
    }
}
