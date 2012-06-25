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
        private Widget selectionWidget;

        public SelectedElementManager(Widget selectionWidget)
        {
            this.selectionWidget = selectionWidget;
        }

        public void clearSelectedElement()
        {
            SelectedElement = null;
        }

        public void updateSelectionPosition()
        {
            if (selectedElement != null)
            {
                selectionWidget.setCoord((int)selectedElement.AbsoluteLeft, (int)selectedElement.AbsoluteTop, (int)selectedElement.ClientWidth, (int)selectedElement.ClientHeight);
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
                if (selectedElement != null)
                {
                    selectionWidget.Visible = true;
                    selectionWidget.setCoord((int)selectedElement.AbsoluteLeft, (int)selectedElement.AbsoluteTop, (int)selectedElement.ClientWidth, (int)selectedElement.ClientHeight);
                }
                else
                {
                    selectionWidget.Visible = false;
                }
            }
        }
    }
}
