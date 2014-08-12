using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Engine.Platform;
using Engine.Command;
using Engine.Behaviors.Animation;

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
            pluginManager.addCreateSimElementManagerCommand(new AddSimElementManagerCommand("Create Simulation Scene Definition", SimulationSceneDefinition.Create));
        }

        public void link(PluginManager pluginManager)
        {

        }

        public void setPlatformInfo(UpdateTimer mainTimer, EventManager eventManager)
        {
            
        }

        public void Dispose()
        {

        }

        public void createDebugCommands(List<CommandManager> commands)
        {

        }

        public void setupRenamedSaveableTypes(RenamedTypeMap renamedTypeMap)
        {
            renamedTypeMap.addRenamedType("Medical.PositionBroadcaster", typeof(PositionBroadcaster));
            renamedTypeMap.addRenamedType("Medical.PoseableByEntity", typeof(PoseableByAnatomyIdentifier));
        }

        #endregion
    }
}
