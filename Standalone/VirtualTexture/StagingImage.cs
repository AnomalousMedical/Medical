using Engine;
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
        private PhysicalTexture physicalTexture;

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

        public void setData(PixelBox sourceBox, PhysicalTexture physicalTexture)
        {
            Image.Scale(sourceBox, pixelBox, Image.Filter.FILTER_NEAREST);
            this.physicalTexture = physicalTexture;
        }

        public void copyToGpu(IntRect destination)
        {
            physicalTexture.addPage(pixelBox, destination);
        }
    }
}
