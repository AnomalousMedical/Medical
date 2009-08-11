using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    class TeethSelectionGroup
    {
        private HashSet<String> selectedTeeth = new HashSet<string>();
        private String name;

        public TeethSelectionGroup(String name)
        {
            this.name = name;
        }

        public void addTooth(String name)
        {
            selectedTeeth.Add(name);
        }

        public bool isSelected(String name)
        {
            return selectedTeeth.Contains(name);
        }

        public override string ToString()
        {
            return name;
        }
    }
}
