using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.Controller.AnomalousMvc
{
    class ActiveContextManager
    {
        private List<AnomalousMvcContext> activeContexts = new List<AnomalousMvcContext>();

        public ActiveContextManager()
        {

        }

        public void pushContext(AnomalousMvcContext context)
        {
            activeContexts.Add(context);
        }

        public void removeContext(AnomalousMvcContext context)
        {
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
    }
}
