using Anomalous.GuiFramework;
using Anomalous.GuiFramework.Cameras;
using Anomalous.GuiFramework.Debugging;
using Anomalous.GuiFramework.Editor;
using Anomalous.libRocketWidget;
using Anomalous.OSPlatform;
using Anomaly;
using BEPUikPlugin;
using BulletPlugin;
using Engine;
using libRocketPlugin;
using Medical;
using MyGUIPlugin;
using OgrePlugin;
using SoundPlugin;
using System;
using System.Collections.Generic;
using System.Text;

namespace AnomalousMedical.Anomaly
{
    class AnomalousMedicalAnomaly : IAnomalyImplementation
    {
        public void AddPlugins(PluginManager pluginManager)
        {
            pluginManager.addPluginAssembly(typeof(OgreInterface).Assembly);
            pluginManager.addPluginAssembly(typeof(BulletInterface).Assembly);
            pluginManager.addPluginAssembly(typeof(NativePlatformPlugin).Assembly);
            pluginManager.addPluginAssembly(typeof(MyGUIInterface).Assembly);
            pluginManager.addPluginAssembly(typeof(RocketInterface).Assembly);
            pluginManager.addPluginAssembly(typeof(SoundPluginInterface).Assembly);
            pluginManager.addPluginAssembly(typeof(BEPUikInterface).Assembly);
            pluginManager.addPluginAssembly(typeof(SimulationPlugin).Assembly);
            pluginManager.addPluginAssembly(typeof(GuiFrameworkInterface).Assembly);
            pluginManager.addPluginAssembly(typeof(RocketWidgetInterface).Assembly);
            pluginManager.addPluginAssembly(typeof(GuiFrameworkCamerasInterface).Assembly);
            pluginManager.addPluginAssembly(typeof(GuiFrameworkEditorInterface).Assembly);
            pluginManager.addPluginAssembly(typeof(GuiFrameworkDebuggingInterface).Assembly);
        }
    }
}
