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

        public void setData(PixelBox sourceBox, PhysicalTexture physicalTexture, int padding)
        {
            if (sourceBox.getWidth() == pixelBox.getWidth())
            {
                Image.Scale(sourceBox, pixelBox, Image.Filter.FILTER_NEAREST);
            }
            else
            {
                //source is too small, pad it out
                IntRect sourceRect = sourceBox.Rect;
                using (PixelBox partialPixels = texture.getPixelBox())
                {
                    //Center
                    partialPixels.Rect = new IntRect(padding, padding, sourceRect.Width, sourceRect.Height);
                    Image.Scale(sourceBox, partialPixels, Image.Filter.FILTER_NEAREST);

                    ////Left
                    partialPixels.Rect = new IntRect(0, padding, padding, sourceRect.Height);
                    sourceBox.Rect = new IntRect(sourceRect.Left, sourceRect.Top, padding, sourceRect.Height);
                    Image.Scale(sourceBox, partialPixels, Image.Filter.FILTER_NEAREST);
                    
                    ////Top
                    partialPixels.Rect = new IntRect(padding, 0, sourceRect.Width, padding);
                    sourceBox.Rect = new IntRect(sourceRect.Left, sourceRect.Top, sourceRect.Width, padding);
                    Image.Scale(sourceBox, partialPixels, Image.Filter.FILTER_NEAREST);

                    ////Right
                    partialPixels.Rect = new IntRect(sourceRect.Width + padding, padding, padding, sourceRect.Height);
                    sourceBox.Rect = new IntRect(sourceRect.Right - padding, sourceRect.Top, padding, sourceRect.Height);
                    Image.Scale(sourceBox, partialPixels, Image.Filter.FILTER_NEAREST);

                    ////Bottom
                    partialPixels.Rect = new IntRect(padding, sourceRect.Height + padding, sourceRect.Width, padding);
                    sourceBox.Rect = new IntRect(sourceRect.Left, sourceRect.Bottom - padding, sourceRect.Width, padding);
                    Image.Scale(sourceBox, partialPixels, Image.Filter.FILTER_NEAREST);
                }
            }
            this.physicalTexture = physicalTexture;
        }

        public void copyToGpu(IntRect destination)
        {
            physicalTexture.addPage(pixelBox, destination);
        }
    }
}
