using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace Medical.Controller
{
    public class HeadController : MedicalInterface
    {
        private MedicalController controller;
        private HeadToolStrip headToolStrip;
        private HeadPlaybackState startHeadState;

        public void initialize(MedicalController controller)
        {
            headToolStrip = new HeadToolStrip(this);
            this.controller = controller;
            controller.addToolStrip(headToolStrip);
            headToolStrip.MandibleSizeControl.initialize(controller);
        }

        /// <summary>
        /// Called when the scene has changed.
        /// </summary>
        public void sceneChanged()
        {
            headToolStrip.sceneChanged();
            startHeadState = new HeadPlaybackState(0.0f);
            startHeadState.update();
        }

        public void sceneUnloading()
        {
            headToolStrip.sceneUnloading();
        }

        /// <summary>
        /// Used when restoring window positions. Return the window matching the
        /// persistString or null if no match is found.
        /// </summary>
        /// <param name="persistString">A string describing the window.</param>
        /// <returns>The matching DockContent or null if none is found.</returns>
        public DockContent getDockContent(String persistString)
        {
            return headToolStrip.getDockContent(persistString);
        }

        public void destroy()
        {
            controller.removeToolStrip(headToolStrip);
            headToolStrip.Dispose();
        }

        public void addControlToUI(DockContent control)
        {
            controller.showDockContent(control);
        }

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
            HeadPlaybackState newState = new HeadPlaybackState(startTime);
            newState.update();
            startHeadState.Last.HeadNext = newState;
            return newState;
        }

        public PlaybackState getStartPlaybackState()
        {
            return startHeadState;
        }
    }
}
