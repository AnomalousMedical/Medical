using libRocketPlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical.GUI
{
    class DelegateRocketEventListener : EventListener
    {
        public DelegateRocketEventListener()
        {

        }

        public DelegateRocketEventListener(DelegateRocketEventController.Handler deleg)
        {
            this.Delegate = deleg;
        }

        public DelegateRocketEventController.Handler Delegate { get; set; }

        public override void ProcessEvent(Event evt)
        {
            if (Delegate != null)
            {
                Delegate.Invoke(evt);
            }
        }
    }
}
