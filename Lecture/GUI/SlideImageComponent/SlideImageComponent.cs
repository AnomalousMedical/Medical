using libRocketPlugin;
using Medical.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lecture.GUI
{
    class SlideImageComponent : ElementEditorComponent
    {
        public SlideImageComponent()
            : base("Lecture.GUI.SlideImageComponent.SlideImageComponent.layout", "SlideImage")
        {

        }

        internal bool applyToElement(Element element)
        {
            return false;
        }
    }
}
