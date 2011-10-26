using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    public class AnatomySearchList
    {
        private List<Anatomy> anatomySearchList = new List<Anatomy>();

        public void addAnatomy(Anatomy anatomy)
        {
            anatomySearchList.Add(anatomy);
        }

        public void removeAnatomy(Anatomy anatomy)
        {
            anatomySearchList.Remove(anatomy);
        }

        public void clear()
        {
            anatomySearchList.Clear();
        }

        public List<Anatomy> findMatchingAnatomy(String text, int searchLimit, bool selectIndividuals)
        {
            text = text.ToLowerInvariant();
            List<Anatomy> results = new List<Anatomy>(searchLimit);
            foreach (Anatomy anatomy in anatomySearchList)
            {
                if ((selectIndividuals || anatomy.IsGroup) && anatomy.AnatomicalName.ToLowerInvariant().Contains(text))
                {
                    if (anatomy.AnatomicalName.Length == text.Length)
                    {
                        results.Insert(0, anatomy);
                    }
                    else
                    {
                        results.Add(anatomy);
                    }
                    if (results.Count == searchLimit)
                    {
                        break;
                    }
                }
            }
            return results;
        }
    }
}
