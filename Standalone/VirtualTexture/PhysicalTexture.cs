//#define DRAW_MIP_MARKERS
//#define DRAW_SKIPS

using Engine;
using OgrePlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical
{
    class PhysicalTexture : IDisposable
    {
        static FreeImageAPI.Color[] mipColors = new FreeImageAPI.Color[17];
        static PhysicalTexture()
        {
            //Colors from https://en.wikipedia.org/wiki/List_of_Crayola_crayon_colors#24_pack_Mini_Twistables

            mipColors[0] =  new FreeImageAPI.Color() { R = 28, G = 172, B = 120, A = 255 }; //Green
            mipColors[1] =  new FreeImageAPI.Color() { R = 238, G = 32, B = 77, A = 255 }; //Red
            mipColors[2] =  new FreeImageAPI.Color() { R = 31, G = 117, B = 254, A = 255 }; //Blue
            mipColors[3] =  new FreeImageAPI.Color() { R = 115, G = 102, B = 189, A = 255 }; //Blue Violet
            mipColors[4] =  new FreeImageAPI.Color() { R = 180, G = 103, B = 77, A = 255 }; //Brown
            mipColors[5] =  new FreeImageAPI.Color() { R = 255, G = 170, B = 204, A = 255 }; //Carnation Pink
            mipColors[6] =  new FreeImageAPI.Color() { R = 253, G = 219, B = 109, A = 255 }; //Dandelion
            mipColors[7] =  new FreeImageAPI.Color() { R = 149, G = 145, B = 140, A = 255 }; //Gray
            mipColors[8] =  new FreeImageAPI.Color() { R = 255, G = 117, B = 56, A = 255 }; //Orange
            mipColors[9] =  new FreeImageAPI.Color() { R = 0, G = 0, B = 0, A = 255 }; //Black
            mipColors[10] = new FreeImageAPI.Color() { R = 253, G = 217, B = 181, A = 255 }; //Apricot
            mipColors[11] = new FreeImageAPI.Color() { R = 192, G = 68, B = 143, A = 255 }; //Red Violet
            mipColors[12] = new FreeImageAPI.Color() { R = 255, G = 255, B = 255, A = 255 }; //White
            mipColors[13] = new FreeImageAPI.Color() { R = 252, G = 232, B = 131, A = 255 }; //Yellow
            mipColors[14] = new FreeImageAPI.Color() { R = 197, G = 227, B = 132, A = 255 }; //Yellow Green
            mipColors[15] = new FreeImageAPI.Color() { R = 255, G = 174, B = 66, A = 255 }; //Yellow Orange
            mipColors[16] = new FreeImageAPI.Color() { R = 246, G = 83, B = 166, A = 255 }; //Permanent Magenta
        }

        private TexturePtr physicalTexture;
        private HardwarePixelBufferSharedPtr buffer;
        private String textureName;
        private VirtualTextureManager virtualTextureManager;
        private String name;
        private int texelsPerPage;
        private IntSize2 size;

        private FreeImageAPI.FreeImageBitmap blitBitmap;
        private PixelBox blitBitmapBox;

        public PhysicalTexture(String name, IntSize2 size, VirtualTextureManager virtualTextureManager, int texelsPerPage)
        {
            this.name = name;
            this.texelsPerPage = texelsPerPage;
            this.size = size;
            this.virtualTextureManager = virtualTextureManager;
            this.textureName = "PhysicalTexture" + name;
            physicalTexture = TextureManager.getInstance().createManual(textureName, VirtualTextureManager.ResourceGroup, TextureType.TEX_TYPE_2D, 
                (uint)size.Width, (uint)size.Height, 1, 0, PixelFormat.PF_A8R8G8B8, TextureUsage.TU_RENDERTARGET, null, false, 0); //Got as a render target for now so we can save the output.
            buffer = physicalTexture.Value.getBuffer();

            blitBitmap = new FreeImageAPI.FreeImageBitmap(texelsPerPage, texelsPerPage, FreeImageAPI.PixelFormat.Format32bppArgb);
            unsafe
            {
                blitBitmapBox = new PixelBox(0, 0, texelsPerPage, texelsPerPage, OgreDrawingUtility.getOgreFormat(blitBitmap.PixelFormat), blitBitmap.GetScanlinePointer(0).ToPointer());
            }
        }

        public void Dispose()
        {
            blitBitmapBox.Dispose();
            blitBitmap.Dispose();
            buffer.Dispose();
            physicalTexture.Dispose();
        }

        public unsafe void color(Color color)
        {
            using (var fiBitmap = new FreeImageAPI.FreeImageBitmap((int)physicalTexture.Value.Width, (int)physicalTexture.Value.Height, FreeImageAPI.PixelFormat.Format32bppArgb))
            {
                var fiColor = new FreeImageAPI.Color();
                fiColor.R = (byte)(color.r * 255);
                fiColor.G = (byte)(color.g * 255);
                fiColor.B = (byte)(color.b * 255);
                fiColor.A = (byte)(color.a * 255);
                fiBitmap.FillBackground(new FreeImageAPI.RGBQUAD(fiColor));

                using (var buffer = physicalTexture.Value.getBuffer())
                {
                    using (PixelBox pixelBox = new PixelBox(0, 0, fiBitmap.Width, fiBitmap.Height, OgreDrawingUtility.getOgreFormat(fiBitmap.PixelFormat), fiBitmap.GetScanlinePointer(0).ToPointer()))
                    {
                        buffer.Value.blitFromMemory(pixelBox);
                    }
                }
            }
        }

        public void addPage(HardwarePixelBufferSharedPtr source, IntRect srcRect, IntRect destRect)
        {
            buffer.Value.blit(source, srcRect, destRect);
        }

//        /// <summary>
//        /// Determine which pages are active and load them.
//        /// Will probably have to separate this out 
//        /// </summary>
//        public unsafe void loadPages()
//        {
//            //temporary
//            //color(Color.Black);

//            using (var buffer = physicalTexture.Value.getBuffer())
//            {
//                int pageCount = 0;
//                int x = 0;
//                int y = 0;
//                foreach (var indirectionTex in virtualTextureManager.IndirectionTextures)
//                {
//                    //Just load the first pages we come across until we run out of space, will implement caching later
//                    String originalTextureName;
//                    if (indirectionTex.OriginalTextures.TryGetValue(name, out originalTextureName))
//                    {
//                        pageCount += indirectionTex.ActivePages.Count;
//                        using (var originalTexture = TextureManager.getInstance().getByName(originalTextureName))
//                        {
//                            int mipCount = originalTexture.Value.NumMipmaps;
//                            int currentMip = -1;
//                            HardwarePixelBufferSharedPtr sourceBuffer = null;
//                            try
//                            {
//                                foreach (var page in indirectionTex.ActivePages)
//                                {
//                                    if (page.mip < mipCount)
//                                    {
//                                        IntRect src = new IntRect(page.x * texelsPerPage, page.y * texelsPerPage, texelsPerPage, texelsPerPage);
//                                        IntRect dest = new IntRect(x, y, texelsPerPage, texelsPerPage);
//                                        if (src.Right <= originalTexture.Value.Width >> page.mip && src.Bottom <= originalTexture.Value.Height >> page.mip) //If statement hacks around too small textures
//                                        {
//                                            if (page.mip != currentMip)
//                                            {
//                                                IDisposableUtil.DisposeIfNotNull(sourceBuffer);
//                                                currentMip = page.mip;
//                                                sourceBuffer = originalTexture.Value.getBuffer(0, (uint)page.mip);
//#if DRAW_MIP_MARKERS
//                                                //Temp mip marker drawing
//                                                blitBitmap.FillBackground(new FreeImageAPI.RGBQUAD(mipColors[currentMip]));
//                                                buffer.Value.blitFromMemory(blitBitmapBox, dest);

//                                                x += texelsPerPage;
//                                                if (x >= size.Width)
//                                                {
//                                                    y += texelsPerPage;
//                                                    x = 0;
//                                                    if (y >= size.Height)
//                                                    {
//                                                        return; //ran out of space, stop copying
//                                                    }
//                                                }

//                                                //Have to update dest too, since we drew something
//                                                dest = new IntRect(x, y, texelsPerPage, texelsPerPage);
//#endif
//                                            }

//                                            //Logging.Log.Debug("Drawing {0}", page);
//                                            //This is shit and relies on the textures already being loaded in ogre.
//                                            buffer.Value.blit(sourceBuffer, src, dest);

//                                            //Even crappier way copying from the textures in memory to main memory and then back
//                                            //originalBuffer.Value.blitToMemory(src, blitBitmapBox);
//                                            //buffer.Value.blitFromMemory(blitBitmapBox, dest);
//                                        }
//#if DRAW_SKIPS
//                                        else
//                                        {
//                                            blitBitmap.FillBackground(new FreeImageAPI.RGBQUAD(mipColors[12])); //White
//                                            buffer.Value.blitFromMemory(blitBitmapBox, new IntRect(x, y, texelsPerPage, texelsPerPage));
//                                            //Logging.Log.Debug("Can't fit {0}", page);
//                                        }
//#endif
//                                    }
//#if DRAW_SKIPS
//                                    else
//                                    {
//                                        blitBitmap.FillBackground(new FreeImageAPI.RGBQUAD(mipColors[9])); //Black
//                                        buffer.Value.blitFromMemory(blitBitmapBox, new IntRect(x, y, texelsPerPage, texelsPerPage));
//                                        //Logging.Log.Debug("Can't handle mip map level for {0}", page);
//                                    }
//#endif
                                    
//                                    //Always increment even if we couldn't draw, likely some other texure did draw.
//                                    x += texelsPerPage;
//                                    if (x >= size.Width)
//                                    {
//                                        y += texelsPerPage;
//                                        x = 0;
//                                        if (y >= size.Height)
//                                        {
//                                            Logging.Log.Debug("Ran out of space");
//                                            return; //ran out of space, stop copying
//                                        }
//                                    }
//                                }
//                            }
//                            finally
//                            {
//                                IDisposableUtil.DisposeIfNotNull(sourceBuffer);
//                            }
//                        }
//                    }

//#if DRAW_MIP_MARKERS
//                    blitBitmap.FillBackground(new FreeImageAPI.RGBQUAD(mipColors[16]));
//                    buffer.Value.blitFromMemory(blitBitmapBox, new IntRect(x, y, texelsPerPage, texelsPerPage));
//#endif
//                }
//            }
//        }

        public String TextureName
        {
            get
            {
                return textureName;
            }
        }

        private void saveBlitBitmap()
        {
            using (var stream = System.IO.File.Open(name + "blit.bmp", System.IO.FileMode.Create, System.IO.FileAccess.ReadWrite))
            {
                blitBitmap.Save(stream, FreeImageAPI.FREE_IMAGE_FORMAT.FIF_BMP);
            }
        }
    }
}
