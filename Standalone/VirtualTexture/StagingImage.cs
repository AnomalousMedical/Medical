using OgrePlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical
{
    class StagingImage : IDisposable
    {
        private Image texture;
        private PixelBox pixelBox;

        public StagingImage(int textelsPerPhysicalPage, PixelFormat physicalTextureFormat)
        {
            texture = new Image((uint)textelsPerPhysicalPage, (uint)textelsPerPhysicalPage, 1, physicalTextureFormat, 1, 0);
            pixelBox = texture.getPixelBox();
        }

        public void Dispose()
        {
            pixelBox.Dispose();
            texture.Dispose();
        }

        public void copyData(PixelBox sourceBox)
        {
            Image.Scale(sourceBox, pixelBox, Image.Filter.FILTER_NEAREST);
        }

        public PixelBox PixelBox
        {
            get
            {
                return pixelBox;
            }
        }
    }
}
