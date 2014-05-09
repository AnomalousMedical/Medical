using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libRocketPlugin;

namespace Medical.GUI
{
    public class RocketEventListenerInstancer : EventListenerInstancer
    {
        private static RocketEventController rocketEventController;

        /// <summary>
        /// Set an event controller to generate custom events when loading a document or other work.
        /// </summary>
        /// <param name="eventController">The event controller to generate custom events.</param>
        public static void setEventController(RocketEventController eventController)
        {
            rocketEventController = eventController;
        }

        /// <summary>
        /// Reset the event controller back to generating debug events. This
        /// should be done when you are finished with what you needed to load
        /// with setEventController.
        /// </summary>
        public static void resetEventController()
        {
            rocketEventController = null;
        }

        public override EventListener InstanceEventListener(string name, Element element)
        {
            if (rocketEventController != null)
            {
                return rocketEventController.createEventListener(name);
            }
            return new DebugEventListener(name);
        }
    }
}
