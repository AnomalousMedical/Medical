using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using libRocketPlugin;
using Logging;

namespace Medical.GUI
{
    class DebugEventListener : EventListener
    {
        private string name;

        public DebugEventListener(String name)
        {
            this.name = name;
        }

        public override void ProcessEvent(Event evt)
        {
            Log.Debug("--------- {0} | type {1} ----------", name, evt.Type);
            foreach (RktEntry param in evt.Parameters)
            {
                Log.Debug("key {0} | type {1} | value {2}", param.Key, param.Value.VariantType, param.Value.StringValue);
            }
            Log.Debug("-----------------------------------");
        }
    }
}
