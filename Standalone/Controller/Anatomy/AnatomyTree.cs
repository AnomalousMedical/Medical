using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    public class AnatomyTree
    {
        public List<Anatomy> topLevelAnatomy = new List<Anatomy>();

        public void addAnatomy(Anatomy anatomy, bool allowPremium)
        {
            if (anatomy.ShowInTree && (allowPremium || anatomy.ShowInBasicVersion))
            {
                topLevelAnatomy.Add(anatomy);
            }
        }

        public void clear()
        {
            topLevelAnatomy.Clear();
        }

        public IEnumerable<Anatomy> TopLevelAnatomy
        {
            get
            {
                return topLevelAnatomy;
            }
        }
    }
}
