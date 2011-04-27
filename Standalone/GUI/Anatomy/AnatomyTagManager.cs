using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.GUI
{
    class AnatomyTagManager
    {
        private Dictionary<String, AnatomyTagGroup> anatomyTagGroups = new Dictionary<string, AnatomyTagGroup>();

        public AnatomyTagManager()
        {

        }

        public void addAnatomyIdentifier(AnatomyIdentifier anatomyIdentifier)
        {
            foreach (AnatomyTag tag in anatomyIdentifier.Tags)
            {
                AnatomyTagGroup tagGroup;
                if (!anatomyTagGroups.TryGetValue(tag.Tag, out tagGroup))
                {
                    tagGroup = new AnatomyTagGroup(tag.Tag);
                    anatomyTagGroups.Add(tag.Tag, tagGroup);
                }
                tagGroup.addAnatomy(anatomyIdentifier);
            }
        }

        public void clear()
        {
            anatomyTagGroups.Clear();
        }

        public IEnumerable<AnatomyTagGroup> Groups
        {
            get
            {
                return anatomyTagGroups.Values;
            }
        }
    }
}
