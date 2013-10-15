using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libRocketPlugin;
using OgreWrapper;

namespace Medical.GUI
{
    public static class RocketOgreTextureManager
    {
        private const String RocketImageOgreGroup = "RocketImages";

        public static void startup()
        {
            OgreResourceGroupManager.getInstance().createResourceGroup("Rocket");
            OgreResourceGroupManager.getInstance().addResourceLocation("/", "EngineArchive", "Rocket", false);

            OgreResourceGroupManager.getInstance().addResourceLocation("__RmlViewerFilesystem__", RocketRawOgreFilesystemArchive.ArchiveName, RocketImageOgreGroup, false);
        }

        public static void shutdown()
        {
            TextureDatabase.ReleaseTextures();
            OgreResourceGroupManager.getInstance().removeResourceLocation("__RmlViewerFilesystem__", RocketImageOgreGroup);
            OgreResourceGroupManager.getInstance().destroyResourceGroup(RocketImageOgreGroup);
        }
    }
}
