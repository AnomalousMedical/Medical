using Engine;
using libRocketPlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.GUI
{
    /// <summary>
    /// A simple interface for a class that can provide highlight info.
    /// </summary>
    public interface HighlightProvider
    {
        /// <summary>
        /// Get a rect of additional offsets to apply to an element's size
        /// when showing a highlight.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        IntRect getAdditionalHighlightAreaRect(Element element);
    }
}
