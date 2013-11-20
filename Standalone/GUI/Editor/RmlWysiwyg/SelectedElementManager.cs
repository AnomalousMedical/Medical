using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libRocketPlugin;
using MyGUIPlugin;
using Engine;

namespace Medical.GUI
{
    class SelectedElementManager
    {
        private ElementStrategy elementStrategy;
        private Element selectedElement;
        private Element highlightElement;
        private Widget selectionWidget;
        private Widget widthAdjust;
        private Widget heightAdjust;
        private Widget bothAdjust;

        private Size2 elementStartSize;
        private IntVector2 mouseStartPosition;

        public SelectedElementManager(Widget parentWidget)
        {
            this.selectionWidget = parentWidget.findWidget("SelectionWidget");

            widthAdjust = parentWidget.findWidget("WidthAdjust");
            widthAdjust.MouseDrag += widthAdjust_MouseDrag;
            widthAdjust.MouseButtonPressed += dragHandle_Pressed;
            heightAdjust = parentWidget.findWidget("HeightAdjust");
            heightAdjust.MouseDrag += heightAdjust_MouseDrag;
            heightAdjust.MouseButtonPressed += dragHandle_Pressed;
            bothAdjust = parentWidget.findWidget("BothAdjust");
            bothAdjust.MouseDrag += bothAdjust_MouseDrag;
            bothAdjust.MouseButtonPressed += dragHandle_Pressed;
        }

        public void clearSelectedAndHighlightedElement()
        {
            SelectedElement = null;
            HighlightElement = null;
            ElementStrategy = null;
        }

        public void updateHighlightPosition()
        {
            if (highlightElement != null)
            {
                int selectionLeft = (int)highlightElement.AbsoluteLeft;
                int selectionTop = (int)highlightElement.AbsoluteTop;
                int selectionWidth = (int)highlightElement.OffsetWidth;
                int selectionHeight = (int)highlightElement.OffsetHeight;
                int selectionRight = selectionLeft + selectionWidth;
                int selectionBottom = selectionTop + selectionHeight;

                selectionWidget.setCoord(selectionLeft, selectionTop, selectionWidth, selectionHeight);
                widthAdjust.setPosition(selectionRight - widthAdjust.Width / 2, selectionTop + selectionHeight / 2 - widthAdjust.Height / 2);
                heightAdjust.setPosition(selectionLeft + selectionWidth / 2 - heightAdjust.Width / 2, selectionBottom - heightAdjust.Width / 2);
                bothAdjust.setPosition(selectionRight - bothAdjust.Width / 2, selectionBottom - bothAdjust.Height / 2);
            }
        }

        public bool HasSelection
        {
            get
            {
                return selectedElement != null;
            }
        }

        public ElementStrategy ElementStrategy
        {
            get
            {
                return elementStrategy;
            }
            set
            {
                elementStrategy = value;
                showResizeHandles(elementStrategy != null && elementStrategy.Resizable);
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
                    updateHighlightPosition();
                }
                else
                {
                    selectionWidget.Visible = false;
                }
            }
        }

        private void showResizeHandles(bool show)
        {
            widthAdjust.Visible = show;
            heightAdjust.Visible = show;
            bothAdjust.Visible = show;
        }

        void bothAdjust_MouseDrag(Widget source, EventArgs e)
        {
            if (elementStrategy != null && selectedElement != null)
            {
                MouseEventArgs me = (MouseEventArgs)e;
                IntVector2 mouseOffset = me.Position - mouseStartPosition;
                Size2 newSize = new Size2(elementStartSize.Width + mouseOffset.x, elementStartSize.Height + mouseOffset.y);
                float ratio = selectedElement.Context.ZoomLevel * ScaleHelper.ScaleFactor;
                newSize = newSize / ratio;

                elementStrategy.changeSize(selectedElement, (IntSize2)newSize);
                updateHighlightPosition();
            }
        }

        void heightAdjust_MouseDrag(Widget source, EventArgs e)
        {
            if (elementStrategy != null && selectedElement != null)
            {
                MouseEventArgs me = (MouseEventArgs)e;
                IntVector2 mouseOffset = me.Position - mouseStartPosition;
                Size2 newSize = new Size2(elementStartSize.Width, elementStartSize.Height + mouseOffset.y);
                float ratio = selectedElement.Context.ZoomLevel * ScaleHelper.ScaleFactor;
                newSize = newSize / ratio;

                elementStrategy.changeSize(selectedElement, (IntSize2)newSize);
                updateHighlightPosition();
            }
        }

        void widthAdjust_MouseDrag(Widget source, EventArgs e)
        {
            if (elementStrategy != null && selectedElement != null)
            {
                MouseEventArgs me = (MouseEventArgs)e;
                IntVector2 mouseOffset = me.Position - mouseStartPosition;
                Size2 newSize = new Size2(elementStartSize.Width + mouseOffset.x, elementStartSize.Height);
                float ratio = selectedElement.Context.ZoomLevel * ScaleHelper.ScaleFactor;
                newSize = newSize / ratio;

                elementStrategy.changeSize(selectedElement, (IntSize2)newSize);
                updateHighlightPosition();
            }
        }

        void dragHandle_Pressed(Widget source, EventArgs e)
        {
            if (selectedElement != null)
            {
                MouseEventArgs me = (MouseEventArgs)e;
                mouseStartPosition = me.Position;
                elementStartSize = new Size2(selectedElement.OffsetWidth, selectedElement.OffsetHeight);
            }
        }
    }
}
