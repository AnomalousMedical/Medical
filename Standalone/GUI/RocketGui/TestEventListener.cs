using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace libRocketPlugin
{
    class TestEventListener : EventListener
    {
        private string name;

        public TestEventListener(String name)
        {
            this.name = name;
        }

        public override void ProcessEvent(Event evt)
        {
            Logging.Log.Debug("{0} phase {1} | type {2}", name, evt.Phase, evt.Type);
            foreach (RktEntry param in evt.Parameters)
            {
                Logging.Log.Debug("key {0} | type {1} | value {2}", param.Key, param.Value.VariantType, param.Value.StringValue);
            }
        }
    }
}
