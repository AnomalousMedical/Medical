using Engine.Saving;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller.AnomalousMvc;

namespace Medical
{
    public interface Slide : Saveable
    {
        void setupContext(AnomalousMvcContext context, String name, bool allowPrevious, bool allowNext, ResourceProvider resourceProvider);

        /// <summary>
        /// Make a new unique name for the slide, should only need to be done when duplicating a slide for some reason.
        /// </summary>
        void generateNewUniqueName();

        void cleanup(CleanupFileInfo info, ResourceProvider resourceProvider);

        String UniqueName { get; }
    }
}
