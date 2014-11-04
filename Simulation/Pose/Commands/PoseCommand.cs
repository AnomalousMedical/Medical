using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical.Pose.Commands
{
    class PoseCommand
    {
        private List<PoseCommandAction> actions = new List<PoseCommandAction>();

        public void posingStarted()
        {
            foreach(var action in actions)
            {
                action.posingStarted();
            }
        }

        public void posingEnded()
        {
            foreach (var action in actions)
            {
                action.posingEnded();
            }
        }

        internal void addAction(PoseCommandAction action)
        {
            actions.Add(action);
        }

        internal void removeAction(PoseCommandAction action)
        {
            actions.Remove(action);
        }

        internal bool IsEmpty
        {
            get
            {
                return actions.Count == 0;
            }
        }
    }
}
