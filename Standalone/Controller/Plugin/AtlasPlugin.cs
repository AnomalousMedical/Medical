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
        void loadGUIResources();

        void initialize(StandaloneController standaloneController);

        void sceneLoaded(SimScene scene);

        void sceneUnloading(SimScene scene);

        void setMainInterfaceEnabled(bool enabled);

        /// <summary>
        /// Called when the scene has finished loading and has been fully revealed. (i.e. any fading effects etc. have been completed).
        /// This only happens on the first scene load when the program opens.
        /// </summary>
        void sceneRevealed();

        long PluginId { get; }

        String PluginName { get; }

        String BrandingImageKey { get; }

        /// <summary>
        /// The file system location of the plugin.
        /// </summary>
        String Location { get; }

        /// <summary>
        /// The verison of the plugin.
        /// </summary>
        Version Version { get; }

        /// <summary>
        /// Return true to allow the plugin to be uninstalled.
        /// </summary>
        bool AllowUninstall { get; }

        /// <summary>
        /// An enumerator over the plugin ids that this plugin is dependent on.
        /// </summary>
        IEnumerable<long> DependencyPluginIds { get; }
    }
}
