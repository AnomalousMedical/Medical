using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Logging;

namespace Medical
{
    /// <summary>
    /// This class will create TimelineGUIs.
    /// </summary>
    public class TimelineGUIFactory
    {
        private Dictionary<String, TimelineGUIFactoryPrototype> prototypes = new Dictionary<String, TimelineGUIFactoryPrototype>();

        public void addPrototype(TimelineGUIFactoryPrototype prototype)
        {
            prototypes.Add(prototype.Name, prototype);
        }

        public void removePrototype(TimelineGUIFactoryPrototype prototype)
        {
            prototypes.Remove(prototype.Name);
        }

        public TimelineGUI createGUI(String name)
        {
            TimelineGUIFactoryPrototype prototype = null;
            if (prototypes.TryGetValue(name, out prototype))
            {
                return prototype.createGUI();
            }
            Log.Error("Could not find TimelineGUI prototype {0}. Did not create TimelineGUI.", name);
            return null;
        }

        public IEnumerable<String> Prototypes
        {
            get
            {
                return prototypes.Keys;
            }
        }
    }
}
