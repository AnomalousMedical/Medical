using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OgreWrapper;

namespace Medical
{
    public abstract class Watermark : IDisposable
    {
        public void Dispose()
        {
            destroyOverlays();
        }

        public abstract void createOverlays();

        public abstract void sizeChanged(float width, float height);

        public abstract void setVisible(bool visible);

        public abstract void destroyOverlays();

        public abstract Watermark clone(String newName);

        public void preFindVisibleCallback(bool currentCameraRender)
        {
            this.setVisible(currentCameraRender);
        }
    }
}
