using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WeifenLuo.WinFormsUI.Docking;

namespace Medical
{
    /// <summary>
    /// A medical interface is a set of specific guis and controls to work on a
    /// specific part of the body. These can be swapped out by the main
    /// interface as required depending on what is being shown.
    /// </summary>
    public interface MedicalInterface
    {
        /// <summary>
        /// Initialise the interface.
        /// </summary>
        /// <param name="controller">The MedicalController.</param>
        void initialize(MedicalController controller);

        /// <summary>
        /// Called when the scene has changed.
        /// </summary>
        void sceneChanged();

        /// <summary>
        /// Used when restoring window positions. Return the window matching the
        /// persistString or null if no match is found.
        /// </summary>
        /// <param name="persistString">A string describing the window.</param>
        /// <returns>The matching DockContent or null if none is found.</returns>
        DockContent getDockContent(String persistString);

        /// <summary>
        /// Destroy the interface and all ui's.
        /// </summary>
        void destroy();
    }
}
