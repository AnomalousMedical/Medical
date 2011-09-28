﻿using System;
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

        public void cancel()
        {
            threadEvent.Set();
            Finished = true;
        }

        public bool Finished { get; set; }
    }

    public class ThreadManager
    {
        private static List<TargetEntry> targets = new List<TargetEntry>();

        private ThreadManager()
        {

        }

        /// <summary>
        /// Exectue a Delegate when doInvoke is called on the thread that calls doInvoke
        /// </summary>
        /// <param name="target"></param>
        public static void invokeAndWait(Delegate target, params object[] args)
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

        public static void invoke(Delegate target, params object[] args)
        {
            TargetEntry entry = new TargetEntry(target, args);
            lock (targets)
            {
                targets.Add(entry);
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

        /// <summary>
        /// This will cancel all targets and return control back to any waiting
        /// threads. Should be called only on shutdown.
        /// </summary>
        public static void cancelAll()
        {
            lock (targets)
            {
                foreach (TargetEntry target in targets)
                {
                    target.cancel();
                }
                targets.Clear();
            }
        }
    }
}
