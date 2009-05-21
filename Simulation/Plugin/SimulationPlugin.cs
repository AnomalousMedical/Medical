using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Engine.Platform;

namespace Medical
{
    public class SimulationPlugin : PluginInterface
    {
        #region PluginInterface Members

        public DebugInterface getDebugInterface()
        {
            return null;
        }

        public string getName()
        {
            return "Simulation";
        }

        public void initialize(PluginManager pluginManager)
        {
            
        }

        public void setPlatformInfo(UpdateTimer mainTimer, EventManager eventManager)
        {
            
        }

        public void Dispose()
        {
            
        }

        #endregion
    }
}
