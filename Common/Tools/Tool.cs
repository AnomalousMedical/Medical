using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using Engine.ObjectManagement;
using Engine;

namespace Medical
{
    public interface Tool
    {
        void setEnabled(bool enabled);

        void update(EventManager eventManager);

        void createSceneElement(SimSubScene subScene, PluginManager pluginManager);

        void destroySceneElement(SimSubScene subScene, PluginManager pluginManager);

        void setTranslation(Vector3 position);

        String Name { get; }
    }
}
