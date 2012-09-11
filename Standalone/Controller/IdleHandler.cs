using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    public enum IdleStatus
    {
        Ok,
    }

    /// <summary>
    /// This class is designed to run the idle callbacks from the os. Idle
    /// occures when no messages are in the queue to be pumped and a frame
    /// should be made. By default this should point to the main timer update
    /// function (or another appropriate idle). You can also feed this class an
    /// IEnumerable&lt;Object&gt; that will be executed instead of the normal
    /// default idle. The default idle will be restored when the enumerator is
    /// finished. This allows long operations to fall back to the message pump
    /// for the os without having to call it explicitly while still preventing
    /// frames from being generated.
    /// </summary>
    public class IdleHandler
    {
        public delegate void IdleDelegate();

        private IdleDelegate defaultIdle;
        private IdleDelegate currentIdleFunc;

        public IEnumerator<IdleStatus> enumIdle;

        public IdleHandler(IdleDelegate defaultIdle)
        {
            this.defaultIdle = defaultIdle;
            this.currentIdleFunc = defaultIdle;
        }

        public IdleDelegate DefaultIdle
        {
            get
            {
                return defaultIdle;
            }
            set
            {
                if (defaultIdle == currentIdleFunc)
                {
                    currentIdleFunc = value;
                }
                defaultIdle = value;
            }
        }

        public void onIdle()
        {
            currentIdleFunc();
        }

        public void runTemporaryIdle(IEnumerable<IdleStatus> idle)
        {
            this.enumIdle = idle.GetEnumerator();
            this.currentIdleFunc = idleEnumerator;
        }

        private void idleEnumerator()
        {
            //Run until the iterator stops, then go back to default idle
            if (!enumIdle.MoveNext())
            {
                currentIdleFunc = defaultIdle;
            }
        }
    }
}
