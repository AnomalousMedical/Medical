using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace libRocketPlugin
{
    class TestEventListenerInstancer : EventListenerInstancer
    {
        public override EventListener InstanceEventListener(string name)
        {
            return new TestEventListener(name);
        }
    }
}
