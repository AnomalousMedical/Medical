using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libRocketPlugin;

namespace Medical.GUI
{
    /// <summary>
    /// This interface can be set on the RocketEventListenerInstance to change
    /// the types of events that are generated. This allows the behavior of that
    /// class to change without modifying the native libRocket side.
    /// </summary>
    public interface RocketEventController
    {
        EventListener createEventListener(String name);
    }
}
