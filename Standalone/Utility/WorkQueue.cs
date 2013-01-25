using Medical.Controller;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Medical
{
    public class WorkQueue
    {
        private BlockingCollection<Action> queue = new BlockingCollection<Action>();

        public WorkQueue()
        {
            Thread t = new Thread(() =>
                {
                    while (true)
                    {
                        Action action = queue.Take();
                        action();
                    }
                });
            t.IsBackground = true;
            t.Start();
        }

        public void enqueue(Action action)
        {
            queue.Add(action);
        }
    }
}
