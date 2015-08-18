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

        /// <summary>
        /// Unload the plugin, only needed if AllowRuntimeUninstall is true. Is not called on program termination
        /// only when plugins are uninstalled or reloading (which uninstalls and then reloads).
        /// </summary>
        /// <param name="willReload">True if this plugin will reload.</param>
        /// <param name="shuttingDown">True if the program is shutting down, can probably skip some items in this case.</param>
        void unload(StandaloneController standaloneController, bool willReload, bool shuttingDown);

        void sceneLoaded(SimScene scene);

        void sceneUnloading(SimScene scene);

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
        /// Will be true if the plugin can be uninstalled at runtime without restarting the program.
        /// </summary>
        bool AllowRuntimeUninstall { get; }

        /// <summary>
        /// An enumerator over the plugin ids that this plugin is dependent on.
        /// </summary>
        IEnumerable<long> DependencyPluginIds { get; }
    }
}
