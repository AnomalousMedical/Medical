using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    class BehaviorErrorManager
    {
        private LinkedList<BehaviorBlacklistEventArgs> blacklistEvents = new LinkedList<BehaviorBlacklistEventArgs>();

        public BehaviorErrorManager()
        {
            BehaviorInterface.Instance.BehaviorBlacklisted += Instance_BehaviorBlacklisted;
        }

        public void clear()
        {
            blacklistEvents.Clear();
        }

        void Instance_BehaviorBlacklisted(BehaviorBlacklistEventArgs blacklistInfo)
        {
            blacklistEvents.AddLast(blacklistInfo);
        }

        public bool HasErrors
        {
            get
            {
                return blacklistEvents.First != null;
            }
        }

        public IEnumerable<BehaviorBlacklistEventArgs> BlacklistEvents
        {
            get
            {
                return blacklistEvents;
            }
        }
    }
}
