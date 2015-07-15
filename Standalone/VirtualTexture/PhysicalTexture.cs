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
    public enum PixelFormatUsageHint
    {
        NotSpecial,
        NormalMap,
        OpacityMap,
    }

    public class PhysicalTexture : IDisposable
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

        public PhysicalTexture(String name, IntSize2 size, VirtualTextureManager virtualTextureManager, int texelsPerPage, CompressedTextureSupport texFormat, PixelFormatUsageHint textureUsage)
        {
            this.name = name;
            this.texelsPerPage = texelsPerPage;
            this.size = size;
            this.virtualTextureManager = virtualTextureManager;
            this.textureName = "PhysicalTexture" + name;
            switch (texFormat)
            {
                case CompressedTextureSupport.None:
                    physicalTexture = TextureManager.getInstance().createManual(textureName, VirtualTextureManager.ResourceGroup, TextureType.TEX_TYPE_2D, 
                        (uint)size.Width, (uint)size.Height, 1, 0, PixelFormat.PF_A8R8G8B8, TextureUsage.TU_RENDERTARGET, null, false, 0); //Got as a render target for now so we can save the output.
                    break;
                case CompressedTextureSupport.DXT:
                    PixelFormat pixelFormat = PixelFormat.PF_DXT5;
                    //switch(textureUsage)
                    //{
                    //    case PixelFormatUsageHint.OpacityMap:
                    //        pixelFormat = PixelFormat.PF_DXT1;
                    //        break;
                    //}
                    physicalTexture = TextureManager.getInstance().createManual(textureName, VirtualTextureManager.ResourceGroup, TextureType.TEX_TYPE_2D,
                        (uint)size.Width, (uint)size.Height, 1, 0, pixelFormat, TextureUsage.TU_DEFAULT, null, false, 0); //Got as a render target for now so we can save the output.
                    break;
            }

            buffer = physicalTexture.Value.getBuffer();
        }

        public void Dispose()
        {
            buffer.Dispose();
            physicalTexture.Dispose();
        }

        public unsafe void color(Color color)
        {
            if (physicalTexture.Value.Format == PixelFormat.PF_A8R8G8B8)
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
        }

        public void addPage(PixelBox source, IntRect destRect)
        {
            buffer.Value.blitFromMemory(source, destRect);
        }

        public String TextureName
        {
            get
            {
                return textureName;
            }
        }

        public PixelFormat TextureFormat
        {
            get
            {
                return physicalTexture.Value.Format;
            }
        }
    }
}
