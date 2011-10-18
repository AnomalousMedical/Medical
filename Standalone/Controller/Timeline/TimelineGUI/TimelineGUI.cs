using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI;

namespace Medical
{
    /// <summary>
    /// An interface for a GUI that can be displayed as part of a timeline. How
    /// this works is up to the client code. The Timelines will call for their
    /// GUIs out of the factory, which will return an instance of this
    /// interface. When and where these guis are created/deleted is up to the
    /// client code.
    /// </summary>
    public interface TimelineGUI
    {
        /// <summary>
        /// Set up the GUI.
        /// </summary>
        /// <param name="timelineController">The ShowTimelineGUIAction</param>
        void initialize(ShowTimelineGUIAction showGUIAction);

        /// <summary>
        /// Show the GUI on the screen.
        /// </summary>
        /// <param name="guiManager">The GUIManager to show the gui on.</param>
        void show(GUIManager guiManager);

        /// <summary>
        /// Remove the GUI from the screen. This will only be called if the
        /// timeline being played needs to be canceled on the TimelineController
        /// level. This will only happen if the TimelineController itself is
        /// asked to stop playing timelines while this gui is active. It will
        /// not happen in normal operation, but clients still need to have a way
        /// that they can shut themselves down through this function.
        /// </summary>
        /// <param name="guiManager">The GUIManager the GUI is a part of.</param>
        void forceClose(GUIManager guiManager);
    }
}
