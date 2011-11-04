using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.Controller
{
    public class MovementSequenceSet
    {
        private LinkedList<MovementSequenceGroup> groups = new LinkedList<MovementSequenceGroup>();

        public MovementSequenceSet()
        {

        }

        public void addGroup(MovementSequenceGroup group)
        {
            groups.AddLast(group);
        }

        public void removeGroup(MovementSequenceGroup group)
        {
            groups.Remove(group);
        }

        public MovementSequenceGroup getGroup(String name)
        {
            foreach (MovementSequenceGroup group in groups)
            {
                if (group.Name == name)
                {
                    return group;
                }
            }
            return null;
        }

        public IEnumerable<MovementSequenceGroup> Groups
        {
            get
            {
                return groups;
            }
        }
    }
}
