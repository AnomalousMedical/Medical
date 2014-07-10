using Engine.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical
{
    /// <summary>
    /// This class provides an easy way to subscribe / unsubscribe from updates and get the
    /// update when it is fired. It does not allow access to the exceededMaxDelta and loopStarting
    /// functions, since it might not be subscribed when those events fire.
    /// </summary>
    public class SubscribingUpdateListener : UpdateListener
    {
        private UpdateTimer timer;
        private bool isSubscribed = false;

        public delegate void OnUpdateDelegate(Clock clock);
        public event OnUpdateDelegate OnUpdate;

        public SubscribingUpdateListener(UpdateTimer timer)
        {
            this.timer = timer;
        }

        /// <summary>
        /// Subscribe to the update listener if not already subscribed. Safe to call multiple times, this will
        /// only add the listener to the timer once.
        /// </summary>
        public void subscribeToUpdates()
        {
            if(!isSubscribed)
            {
                timer.addUpdateListener(this);
                isSubscribed = true;
            }
        }

        /// <summary>
        /// Unsubscribe from the timer if not subscribed. Safe to call multiple times, will do nothing if not subscribed.
        /// </summary>
        public void unsubscribeFromUpdates()
        {
            if(isSubscribed)
            {
                timer.removeUpdateListener(this);
                isSubscribed = false;
            }
        }

        /// <summary>
        /// This will be true if this class is currently subscribed to the timer.
        /// </summary>
        public bool IsSubscribed
        {
            get
            {
                return isSubscribed;
            }
        }

        public void exceededMaxDelta()
        {
            
        }

        public void loopStarting()
        {
            
        }

        public void sendUpdate(Clock clock)
        {
            if(OnUpdate != null)
            {
                OnUpdate.Invoke(clock);
            }
        }
    }
}
