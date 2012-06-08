using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;

namespace Medical.Platform
{
    public class EventContext
    {
        private List<MessageEvent> messageEvents = new List<MessageEvent>();

        public EventContext()
        {

        }

        public void addEvent(MessageEvent evt)
        {
            messageEvents.Add(evt);
        }

        public void removeEvent(MessageEvent evt)
        {
            messageEvents.Remove(evt);
        }

        public void addMessagesToEventManager(EventManager eventManager)
        {
            foreach (MessageEvent evt in messageEvents)
            {
                eventManager.addEvent(evt);
            }
        }

        public void removeMessagesFromEventManager(EventManager eventManager)
        {
            foreach (MessageEvent evt in messageEvents)
            {
                eventManager.removeEvent(evt);
            }
        }
    }
}
