using Medical.Controller;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Medical
{
    public class WorkQueue : IDisposable
    {
        private BlockingCollection<Action> queue = new BlockingCollection<Action>();
        private bool running = true;

        public WorkQueue()
        {
            Thread t = new Thread(() =>
                {
                    while (running)
                    {
                        Action action = queue.Take();
                        action();
                    }
                });
            t.IsBackground = true;
            t.Start();
        }

        public void Dispose()
        {
            running = false;
            enqueue(() =>
            {

            });
        }

        public void enqueue(Action action)
        {
            queue.Add(action);
        }
    }
}
