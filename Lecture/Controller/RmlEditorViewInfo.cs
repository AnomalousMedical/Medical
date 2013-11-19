using Medical.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lecture
{
    class RmlEditorViewInfo
    {
        public RmlEditorViewInfo()
        {

        }

        public RmlEditorViewInfo(RawRmlWysiwygView view)
        {
            this.View = view;
        }

        public RawRmlWysiwygView View { get; set; }

        public RmlWysiwygComponent Component { get; set; }

        public String getCurrentComponentText()
        {
            if (Component != null)
            {
                return Component.CurrentRml;
            }
            return null;
        }
    }
}
