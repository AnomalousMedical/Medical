using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libRocketPlugin;
using MyGUIPlugin;

namespace Medical.GUI
{
    class SelectedElementManager
    {
        private Element selectedElement;
        private Element highlightElement;
        private Widget selectionWidget;

        public SelectedElementManager(Widget selectionWidget)
        {
            this.selectionWidget = selectionWidget;
        }

        public void clearSelectedAndHighlightedElement()
        {
            SelectedElement = null;
            HighlightElement = null;
        }

        public void updateHighlightPosition()
        {
            if (selectedElement != null)
            {
                selectionWidget.setCoord((int)highlightElement.AbsoluteLeft, (int)highlightElement.AbsoluteTop, (int)highlightElement.OffsetWidth, (int)highlightElement.OffsetHeight);
            }
        }

        public bool HasSelection
        {
            get
            {
                return selectedElement != null;
            }
        }

        public Element SelectedElement
        {
            get
            {
                return selectedElement;
            }
            set
            {
                selectedElement = value;
            }
        }

        public Element HighlightElement
        {
            get
            {
                return highlightElement;
            }
            set
            {
                highlightElement = value;
                if (highlightElement != null)
                {
                    selectionWidget.Visible = true;
                    selectionWidget.setCoord((int)highlightElement.AbsoluteLeft, (int)highlightElement.AbsoluteTop, (int)highlightElement.OffsetWidth, (int)highlightElement.OffsetHeight);
                }
                else
                {
                    selectionWidget.Visible = false;
                }
            }
        }
    }
}
