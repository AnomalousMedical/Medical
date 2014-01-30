using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    /// <summary>
    /// This class holds old inline rml styles from old slideshows so they can be upgraded.
    /// Anywhere you could load an old slideshow you would want to make sure to clear the cahced
    /// styles for that show from this or they will leak.
    /// 
    /// In the future when we are not worried about version 1 and 2 slideshows (they are pretty much together, version 2 was internal only)
    /// we can remove this class.
    /// </summary>
    public static class InlineRmlUpgradeCache
    {
        private static Dictionary<RmlSlidePanel, String> oldInlineRml = new Dictionary<RmlSlidePanel, string>();

        internal static void setRml(RmlSlidePanel panel, String rml)
        {
            if (oldInlineRml.ContainsKey(panel))
            {
                oldInlineRml[panel] = rml;
            }
            else
            {
                oldInlineRml.Add(panel, rml);
            }

        }

        internal static bool tryGetValue(RmlSlidePanel panel, out String value)
        {
            return oldInlineRml.TryGetValue(panel, out value);
        }

        internal static void removePanel(RmlSlidePanel panel)
        {
            if (panel != null)
            {
                oldInlineRml.Remove(panel);
            }
        }

        public static void removeSlideshowPanels(Slideshow slideshow)
        {
            foreach (var slide in slideshow.Slides)
            {
                foreach (var panel in slide.Panels)
                {
                    removePanel(panel as RmlSlidePanel);
                }
            }
        }
    }
}
