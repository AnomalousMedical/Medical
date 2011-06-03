using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Logging;

namespace Medical
{
    /// <summary>
    /// This class will provide access to TimelineGUIs.
    /// </summary>
    public class TimelineGUIFactory
    {
        private Dictionary<String, TimelineGUIFactoryPrototype> prototypes = new Dictionary<String, TimelineGUIFactoryPrototype>();

        /// <summary>
        /// Add a prototype that can be used with a given name.
        /// </summary>
        /// <param name="prototype">The prototype to add.</param>
        public void addPrototype(TimelineGUIFactoryPrototype prototype)
        {
            prototypes.Add(prototype.Name, prototype);
        }

        /// <summary>
        /// Remove a prototype when it is no longer needed.
        /// </summary>
        /// <param name="prototype">The prototype to remove.</param>
        public void removePrototype(TimelineGUIFactoryPrototype prototype)
        {
            prototypes.Remove(prototype.Name);
        }

        /// <summary>
        /// Get the TimelineGUI associated with name from its prototype.
        /// </summary>
        /// <param name="name">The name of the prototype.</param>
        /// <returns>The TimelineGUI returned by that prototype.</returns>
        public TimelineGUI getGUI(String name)
        {
            TimelineGUIFactoryPrototype prototype = null;
            if (prototypes.TryGetValue(name, out prototype))
            {
                return prototype.getGUI();
            }
            Log.Error("Could not find TimelineGUI prototype {0}. Did not create TimelineGUI.", name);
            return null;
        }

        public TimelineGUIData getGUIData(String name)
        {
            TimelineGUIFactoryPrototype prototype = null;
            if (prototypes.TryGetValue(name, out prototype))
            {
                return prototype.getGUIData();
            }
            Log.Error("Could not find TimelineGUI prototype {0}. Did not create TimelineGUIData.", name);
            return null;
        }

        /// <summary>
        /// A list of all prototypes.
        /// </summary>
        public IEnumerable<String> Prototypes
        {
            get
            {
                return prototypes.Keys;
            }
        }
    }
}
