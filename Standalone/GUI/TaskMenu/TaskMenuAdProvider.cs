
using MyGUIPlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical.GUI
{
    public abstract class TaskMenuAdProvider
    {
        public event Action<TaskMenuAdProvider> AdCreated;
        public event Action<TaskMenuAdProvider> AdDestroyed;

        public int Right { get; set; }

        protected internal Widget ParentWidget { get; set; }

        protected void fireAdCreated()
        {
            if (AdCreated != null)
            {
                AdCreated.Invoke(this);
            }
        }

        protected void fireAdDestroyed()
        {
            if (AdDestroyed != null)
            {
                AdDestroyed.Invoke(this);
            }
        }
    }
}
