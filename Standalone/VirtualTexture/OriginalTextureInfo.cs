using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical
{
    class OriginalTextureInfo
    {
        public OriginalTextureInfo(String textureUnit, String textureFileName, int mipOffset)
        {
            this.TextureUnit = textureUnit;
            this.TextureFileName = textureFileName;
            this.MipOffset = mipOffset;
        }

        public String TextureUnit { get; private set; }

        public String TextureFileName { get; private set; }

        public int MipOffset { get; private set; }
    }
}
