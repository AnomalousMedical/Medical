using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical
{
    /// <summary>
    /// A dependency is a shared resource for plugins. It could be art and definition for a prop or a shared
    /// coding resource that can be used in multiple plugins. This is designed to reduce duplication, for example
    /// an arrow prop used in several plugins can be loaded as a dependency so we don't have to duplicate it on the
    /// server, the user's computer or waste bandwidth downloading it for every plugin that needs it.
    /// </summary>
    public interface AtlasDependency : IDisposable
    {
        /// <summary>
        /// Load the gui resources for a dependency. This will be called every time the dependecy is loaded and
        /// should be used to load informational resources only.
        /// </summary>
        void loadGUIResources();

        /// <summary>
        /// Initialize the dependency and load whatever resources it contains.
        /// </summary>
        /// <param name="standaloneController"></param>
        void initialize(StandaloneController standaloneController, AtlasDependencyManager dependencyManager);

        /// <summary>
        /// The id of the dependency
        /// </summary>
        long DependencyId { get; }

        /// <summary>
        /// A friendly name for the dependency. Could be displayed to the user.
        /// </summary>
        String Name { get; }

        /// <summary>
        /// The key for a branding image for this dependency.
        /// </summary>
        String BrandingImageKey { get; }

        /// <summary>
        /// The file system location of the dependency.
        /// </summary>
        String Location { get; }

        /// <summary>
        /// The verison of the dependency.
        /// </summary>
        Version Version { get; }

        /// <summary>
        /// Return true when this dependency has been initialized.
        /// </summary>
        bool Initialized { get; }
    }
}
