using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.ObjectManagement;
using Engine;
using Engine.Saving;
using Engine.Editing;

namespace Medical
{
    public class SimulationScene : SimElementManager
    {
        private String name;
        private SimulationSceneFactory factory = new SimulationSceneFactory();

        private String presetDirectory;
        private String sequenceDirectory;
        private SceneViewWindowPresetController windowPresets;

        public SimulationScene(String name)
        {
            this.name = name;
        }

        public void Dispose()
        {

        }

        public SimElementManagerDefinition createDefinition()
        {
            SimulationSceneDefinition definition = new SimulationSceneDefinition(name);
            definition.PresetDirectory = presetDirectory;
            definition.SequenceDirectory = sequenceDirectory;
            definition.WindowPresets = CopySaver.Default.copy(windowPresets);
            definition.Version = Version;
            definition.AllowIK = AllowIK;
            return definition;
        }

        public SimElementFactory getFactory()
        {
            return factory;
        }

        public string getName()
        {
            return name;
        }

        public Type getSimElementManagerType()
        {
            return typeof(SimulationScene);
        }

        public String PresetDirectory
        {
            get
            {
                return presetDirectory;
            }
            set
            {
                presetDirectory = value;
            }
        }

        public String SequenceDirectory
        {
            get
            {
                return sequenceDirectory;
            }
            set
            {
                sequenceDirectory = value;
            }
        }

        public SceneViewWindowPresetController WindowPresets
        {
            get
            {
                return windowPresets;
            }
            internal set
            {
                windowPresets = value;
            }
        }

        public int Version { get; internal set; }

        public bool AllowIK { get; internal set; }
    }
}
