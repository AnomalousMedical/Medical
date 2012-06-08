using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;

namespace Medical.Platform
{
    /// <summary>
    /// This class helps manage shortcut keys that are specific to a certian
    /// place in the program. It gives a place for various things to "fight" for
    /// input from the main event manager.
    /// </summary>
    public static class GlobalContextEventHandler
    {
        private static EventManager eventManager = null;
        private static EventContext eventContext = null;

        /// <summary>
        /// Set the eventmanger to use. This should only be called once.
        /// </summary>
        /// <param name="eventManager">The event manager to use.</param>
        internal static void setEventManager(EventManager eventManager)
        {
            if (GlobalContextEventHandler.eventManager != null)
            {
                throw new Exception("Only assign the event manager one time.");
            }
            GlobalContextEventHandler.eventManager = eventManager;
        }

        /// <summary>
        /// Set the current event context. If another one was set it will be disabled.
        /// </summary>
        /// <param name="context">The new context to set.</param>
        public static void setEventContext(EventContext context)
        {
            if (eventContext != null)
            {
                eventContext.removeMessagesFromEventManager(eventManager);
            }
            eventContext = context;
            if (eventContext != null)
            {
                eventContext.addMessagesToEventManager(eventManager);
            }
        }

        /// <summary>
        /// Disable an event context. This will only make changes if the context is the current context.
        /// </summary>
        /// <param name="context">The context to disable.</param>
        public static void disableEventContext(EventContext context)
        {
            if (eventContext == context && context != null)
            {
                eventContext.removeMessagesFromEventManager(eventManager);
                eventContext = null;
            }
        }
    }
}
