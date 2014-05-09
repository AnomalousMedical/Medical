using libRocketPlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical.GUI
{
    class BuyScreenEventListener : EventListener
    {
        public BuyScreenEventListener()
        {

        }

        public BuyScreenEventListener(Action<Event> process)
        {
            this.Process = process;
        }

        public Action<Event> Process { get; set; }

        public override void ProcessEvent(Event evt)
        {
            if (Process != null)
            {
                Process.Invoke(evt);
            }
        }
    }
}
