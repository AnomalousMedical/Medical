using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.GUI
{
    class FontEntry
    {
        public FontEntry(String fontName, int maxSize)
        {
            FontName = fontName;
            MaxSize = maxSize;
        }

        public String FontName { get; set; }

        public int MaxSize { get; set; }
    }

    class MyGUIDynamicFontManager
    {
        private List<FontEntry> fontSizes = new List<FontEntry>();

        public MyGUIDynamicFontManager()
        {

        }

        /// <summary>
        /// Add a font, they must be added in increasing order of size for this to work correctly.
        /// </summary>
        /// <param name="fontName">The name of the font.</param>
        /// <param name="maxSize">The maximum size at which to use this font.</param>
        public void addFont(String fontName, int maxSize)
        {
            fontSizes.Add(new FontEntry(fontName, maxSize));
        }

        public String getFont(int size)
        {
            foreach (FontEntry font in fontSizes)
            {
                if (size < font.MaxSize)
                {
                    return font.FontName;
                }
            }
            if (fontSizes.Count > 0)
            {
                return fontSizes[fontSizes.Count - 1].FontName;
            }
            return null;
        }
    }
}
