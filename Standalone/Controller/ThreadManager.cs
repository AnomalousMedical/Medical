using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;

namespace Medical.Controller
{
    public class ThreadManagerSynchronizeInvoke : ISynchronizeInvoke
    {
        private AutoResetEvent autoReset = new AutoResetEvent(false);

        public IAsyncResult BeginInvoke(Delegate method, object[] args)
        {
            autoReset.Reset();
            ThreadManager.invokeAndWait(new Action(() =>
                {
                    method.DynamicInvoke(args);
                    autoReset.Set();
                }));
            return null;
        }

        public object EndInvoke(IAsyncResult result)
        {
            autoReset.WaitOne();
            return null;
        }

        public object Invoke(Delegate method, object[] args)
        {
            ThreadManager.invokeAndWait(method, args);
            return null;
        }

        public bool InvokeRequired
        {
            get { return true; }
        }
    }

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
        private static bool active = true;

        private ThreadManager()
        {

        }

        public static void invokeAndWait(Action target)
        {
            doStartInvokeAndWait(target);
        }

        /// <summary>
        /// Exectue a Delegate when doInvoke is called on the thread that calls doInvoke
        /// </summary>
        /// <param name="target"></param>
        public static void invokeAndWait(Delegate target, params object[] args)
        {
            doStartInvokeAndWait(target, args);
        }

        private static void doStartInvokeAndWait(Delegate target, params object[] args)
        {
            TargetEntry entry = new TargetEntry(target, args);
            lock (targets)
            {
                if (active)
                {
                    targets.Add(entry);
                }
                else
                {
                    entry.cancel();
                }
            }
            if (!entry.Finished)
            {
                entry.wait();
            }
        }

        public static void invoke(Action target)
        {
            doStartInvoke(target);
        }

        public static void invoke(Delegate target, params object[] args)
        {
            doStartInvoke(target, args);
        }

        private static void doStartInvoke(Delegate target, params object[] args)
        {
            TargetEntry entry = new TargetEntry(target, args);
            lock (targets)
            {
                if (active)
                {
                    targets.Add(entry);
                }
                else
                {
                    entry.cancel();
                }
            }
        }

        public static void doInvoke()
        {
            lock (targets)
            {
                foreach (TargetEntry target in activeTargets(targets.Count))
                {
                    target.invoke();
                }
            }
        }

        private static IEnumerable<TargetEntry> activeTargets(int targetCount)
        {
            for (int i = 0; i < targetCount; ++i)
            {
                TargetEntry target = targets[0];
                targets.RemoveAt(0);
                yield return target;
            }
        }

        /// <summary>
        /// This will cancel all targets and return control back to any waiting
        /// threads. Should be called only on shutdown. After this method any
        /// invoke calls will automatically cancel.
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
                active = false;
            }
        }
    }
}
