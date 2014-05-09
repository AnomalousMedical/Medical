using libRocketPlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical.GUI
{
    public class DelegateRocketEventController : RocketEventController
    {
        public delegate void Handler(Event evt);

        private Dictionary<String, Handler> handlers = new Dictionary<string, Handler>();

        public DelegateRocketEventController()
        {
            
        }

        public void addHandler(String name, Handler handler)
        {
            handlers.Add(name, handler);
        }

        public void removeHandler(String name)
        {
            handlers.Remove(name);
        }

        public EventListener createEventListener(string name)
        {
            Handler result;
            if (handlers.TryGetValue(name, out result))
            {
                return new DelegateRocketEventListener(result);
            }
            return null;
        }
    }
}
