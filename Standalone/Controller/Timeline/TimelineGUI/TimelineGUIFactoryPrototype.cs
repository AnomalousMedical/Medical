using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    /// <summary>
    /// This is a prototype in the TimelineGUIFactory.
    /// </summary>
    public interface TimelineGUIFactoryPrototype
    {
        /// <summary>
        /// Get the gui for this entry. This may or may not be created at this
        /// time. The timeline system does not care about when the GUIs are
        /// created/destroyed. That is up to client code.
        /// </summary>
        /// <returns>The gui for this entry.</returns>
        TimelineGUI getGUI();

        /// <summary>
        /// The name of this prototype. Should make a resonable effort to make
        /// this unique. Namespace it to the plugin you are writing.
        /// </summary>
        String Name { get; }
    }
}
