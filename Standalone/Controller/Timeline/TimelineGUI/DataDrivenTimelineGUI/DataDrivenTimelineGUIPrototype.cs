using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    class DataDrivenTimelineGUIPrototype : TimelineGUIFactoryPrototype
    {
        /// <summary>
        /// Get the GUI to use for the ShowGUI action. Here we create a new
        /// instance of ExampleGUI and return it.
        /// </summary>
        /// <returns></returns>
        public TimelineGUI getGUI()
        {
            return new DataDrivenTimelineGUI();
        }

        /// <summary>
        /// This will return the GUIData class for this gui. Right now this
        /// returns null since we are not using any data.
        /// </summary>
        /// <returns></returns>
        public TimelineGUIData getGUIData()
        {
            return new DataDrivenTimelineGUIData();
        }

        /// <summary>
        /// This is the name of the Prototype. It should be namespaced with the
        /// name of the plugin. So here we are using the ExamExample namespace
        /// and then calling this ExampleGUI. By doing it this way we prevent
        /// name collisions between plugins.
        /// </summary>
        public string Name
        {
            get
            {
                return "DataDrivenGUI";
            }
        }
    }
}
