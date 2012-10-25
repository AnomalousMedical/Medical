using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace Medical.GUI
{
    interface TextHighlighter
    {
        void colorString(StringBuilder input);

        Color BackgroundColor { get; }
    }
}
