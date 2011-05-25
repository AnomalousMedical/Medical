using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    /// <summary>
    /// This is a prototype in the TimelineGUIFactory. It will create the gui.
    /// </summary>
    public interface TimelineGUIFactoryPrototype
    {
        /// <summary>
        /// Create the gui for this entry.
        /// </summary>
        /// <returns>The gui for this entry.</returns>
        TimelineGUI createGUI();

        /// <summary>
        /// The name of this prototype. Should make a resonable effort to make
        /// this unique. Namespace it to the plugin you are writing.
        /// </summary>
        String Name { get; }
    }
}
