using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WeifenLuo.WinFormsUI.Docking;

namespace Medical
{
    class CommonController : MedicalInterface
    {
        private MedicalController controller;
        private CommonToolStrip toolStrip;

        /// <summary>
        /// Initialise the interface.
        /// </summary>
        /// <param name="controller">The MedicalController.</param>
        public void initialize(MedicalController controller)
        {
            toolStrip = new CommonToolStrip(this, controller);
            this.controller = controller;
            controller.addToolStrip(toolStrip);
        }

        /// <summary>
        /// Used when restoring window positions. Return the window matching the
        /// persistString or null if no match is found.
        /// </summary>
        /// <param name="persistString">A string describing the window.</param>
        /// <returns>The matching DockContent or null if none is found.</returns>
        public DockContent getDockContent(String persistString)
        {
            return toolStrip.getDockContent(persistString);
        }

        /// <summary>
        /// Destroy the interface and all ui's.
        /// </summary>
        public void destroy()
        {
            controller.removeToolStrip(toolStrip);
            toolStrip.Dispose();
        }

        /// <summary>
        /// Call when the scene changes to update the UI.
        /// </summary>
        public void sceneChanged()
        {
            toolStrip.sceneChanged();
        }

        public void sceneUnloading()
        {
            toolStrip.sceneUnloading();
        }

        /// <summary>
        /// Add some DockContent to the UI.
        /// </summary>
        /// <param name="control"></param>
        public void addControlToUI(DockContent control)
        {
            controller.showDockContent(control);
        }

        /// <summary>
        /// Remove DockContent from the UI.
        /// </summary>
        /// <param name="control"></param>
        public void removeControl(DockContent control)
        {
            controller.hideDockContent(control);
        }

        /// <summary>
        /// Create a new PlaybackState and return it.
        /// </summary>
        /// <returns>A new playback state with the current info in it.</returns>
        public PlaybackState createPlaybackState(float startTime)
        {
            return null;
        }

        public PlaybackState getStartPlaybackState()
        {
            return null;
        }
    }
}
