using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical
{
    /// <summary>
    /// This interface mostly exists to differentiate the Dependencies from the Plugins. Dependencies that
    /// are code based can extend this interface and will be able to be added through addDependency in
    /// the AtlasPluginManager.
    /// </summary>
    public interface AtlasDependency : AtlasPlugin
    {
    }
}
