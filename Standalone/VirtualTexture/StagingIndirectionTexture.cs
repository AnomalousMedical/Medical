using OgrePlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical
{
    class StagingIndirectionTexture : IDisposable
    {
        private FreeImageAPI.FreeImageBitmap[] fiBitmap; //Can we do this without this bitmap? (might be ok to keep, but will be using 2x as much memory, however, allows for background modification, could even double buffer)
        private PixelBox[] pixelBox;
        private IndirectionTexture indirectionTexture;

        public StagingIndirectionTexture(int numMipLevels)
        {
            fiBitmap = new FreeImageAPI.FreeImageBitmap[numMipLevels];
            pixelBox = new PixelBox[numMipLevels];
            int currentMipLevel = 1;

            for (int i = numMipLevels - 1; i >= 0; --i)
            {
                fiBitmap[i] = new FreeImageAPI.FreeImageBitmap(currentMipLevel, currentMipLevel, FreeImageAPI.PixelFormat.Format32bppArgb);
                unsafe
                {
                    pixelBox[i] = new PixelBox(0, 0, fiBitmap[i].Width, fiBitmap[i].Height, OgreDrawingUtility.getOgreFormat(fiBitmap[i].PixelFormat), fiBitmap[i].GetScanlinePointer(0).ToPointer());
                }
                currentMipLevel <<= 1;
            }
        }

        public void Dispose()
        {
            for (int i = 0; i < fiBitmap.Length; ++i)
            {
                pixelBox[i].Dispose();
                fiBitmap[i].Dispose();
            }
        }

        public void setData(IndirectionTexture indirectionTexture)
        {
            this.indirectionTexture = indirectionTexture;
            indirectionTexture.copyToStaging(pixelBox);
        }

        public void uploadToGpu()
        {
            indirectionTexture.uploadStagingToGpu(pixelBox);
        }
    }
}
