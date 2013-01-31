using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.Controller.AnomalousMvc
{
    /// <summary>
    /// This class will maintain a stack of MvcContexts. This stack is based on
    /// the RuntimeName of the item. The RuntimeName must be unique in order for
    /// the context to be kept in the stack. If the RuntimeName of a newly added
    /// context matches one that already exists. The old one will be removed
    /// from the stack. This can be taken advantage of to keep several similar
    /// types of contexts from stacking up indefinitely.
    /// </summary>
    class ActiveContextManager
    {
        private List<AnomalousMvcContext> activeContexts = new List<AnomalousMvcContext>();

        public ActiveContextManager()
        {

        }

        public void pushContext(AnomalousMvcContext context)
        {
            //Remove any existing contexts with the same runtime name
            for(int i = 0; i < activeContexts.Count; ++i)
            {
                if (activeContexts[i].RuntimeName == context.RuntimeName)
                {
                    activeContexts[i].removedFromStack();
                    activeContexts.RemoveAt(i--);
                }
            }
            activeContexts.Add(context);
        }

        public void removeContext(AnomalousMvcContext context)
        {
            context.removedFromStack();
            activeContexts.Remove(context);
        }

        public AnomalousMvcContext CurrentContext
        {
            get
            {
                if (activeContexts.Count > 0)
                {
                    return activeContexts[activeContexts.Count - 1];
                }
                return null;
            }
        }

        public int Count
        {
            get
            {
                return activeContexts.Count;
            }
        }
    }
}
