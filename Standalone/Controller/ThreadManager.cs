using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Medical.Controller
{
    class TargetEntry
    {
        private Delegate func;
        private Object[] args;
        private AutoResetEvent threadEvent = new AutoResetEvent(false);

        public TargetEntry(Delegate func, object[] args)
        {
            this.func = func;
            this.args = args;
        }

        public void invoke()
        {
            func.DynamicInvoke(args);
            threadEvent.Set();
            Finished = true;
        }

        public void wait()
        {
            threadEvent.WaitOne();
        }

        public bool Finished { get; set; }
    }

    class ThreadManager
    {
        private static List<TargetEntry> targets = new List<TargetEntry>();

        private ThreadManager()
        {

        }

        /// <summary>
        /// Exectue a Delegate when doInvoke is called on the thread that calls doInvoke
        /// </summary>
        /// <param name="target"></param>
        public static void invoke(Delegate target, params object[] args)
        {
            TargetEntry entry = new TargetEntry(target, args);
            lock (targets)
            {
                targets.Add(entry);
            }
            if (!entry.Finished)
            {
                entry.wait();
            }
        }

        public static void doInvoke()
        {
            lock (targets)
            {
                foreach (TargetEntry target in targets)
                {
                    target.invoke();
                }
                targets.Clear();
            }
        }
    }
}
