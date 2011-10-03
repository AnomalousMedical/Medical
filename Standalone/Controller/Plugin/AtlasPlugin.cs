using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.ObjectManagement;
using Medical.GUI;

namespace Medical
{
    public interface AtlasPlugin : IDisposable
    {
        void initialize(StandaloneController standaloneController);

        void sceneLoaded(SimScene scene);

        void sceneUnloading(SimScene scene);

        void setMainInterfaceEnabled(bool enabled);

        void createMenuBar(NativeMenuBar menu);

        /// <summary>
        /// Called when the scene has finished loading and has been fully revealed. (i.e. any fading effects etc. have been completed).
        /// </summary>
        void sceneRevealed();

        long PluginId { get; }

        String PluginName { get; }

        String BrandingImageKey { get; }
    }
}
