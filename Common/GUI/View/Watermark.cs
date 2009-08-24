using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OgreWrapper;

namespace Medical
{
    public abstract class Watermark
    {
        public static void CreateResources()
        {
            OgreResourceGroupManager.getInstance().addResourceLocation(Engine.Resources.Resource.ResourceRoot + "/Watermark", "FileSystem", "Watermark", false);
            OgreResourceGroupManager.getInstance().initializeAllResourceGroups();
        }

        public abstract void createOverlays();

        public abstract void sizeChanged(float width, float height);

        public abstract void setVisible(bool visible);

        public abstract void destroyOverlays();
    }
}
