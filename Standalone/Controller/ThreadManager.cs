using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.Controller
{
    class TargetEntry
    {
        private Delegate func;
        private Object[] args;

        public TargetEntry(Delegate func, object[] args)
        {
            this.func = func;
            this.args = args;
        }

        public void invoke()
        {
            func.DynamicInvoke(args);
        }
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
            lock (targets)
            {
                targets.Add(new TargetEntry(target, args));
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
