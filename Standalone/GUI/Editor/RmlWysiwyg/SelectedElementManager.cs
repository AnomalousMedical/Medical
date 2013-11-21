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
        private const int minSize = 15; //Note that this is applied after the value is scaled and does not need to be scaled further

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
            widthAdjust.MouseButtonReleased += dragHandle_Released;
            
            heightAdjust = parentWidget.findWidget("HeightAdjust");
            heightAdjust.MouseDrag += heightAdjust_MouseDrag;
            heightAdjust.MouseButtonPressed += dragHandle_Pressed;
            heightAdjust.MouseButtonReleased += dragHandle_Released;

            bothAdjust = parentWidget.findWidget("BothAdjust");
            bothAdjust.MouseDrag += bothAdjust_MouseDrag;
            bothAdjust.MouseButtonPressed += dragHandle_Pressed;
            bothAdjust.MouseButtonReleased += dragHandle_Released;
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
                sendSizeChange(newSize, true, true);
            }
        }

        void heightAdjust_MouseDrag(Widget source, EventArgs e)
        {
            if (elementStrategy != null && selectedElement != null)
            {
                MouseEventArgs me = (MouseEventArgs)e;
                IntVector2 mouseOffset = me.Position - mouseStartPosition;
                Size2 newSize = new Size2(elementStartSize.Width, elementStartSize.Height + mouseOffset.y);
                sendSizeChange(newSize, false, true);
            }
        }

        void widthAdjust_MouseDrag(Widget source, EventArgs e)
        {
            if (elementStrategy != null && selectedElement != null)
            {
                MouseEventArgs me = (MouseEventArgs)e;
                IntVector2 mouseOffset = me.Position - mouseStartPosition;
                Size2 newSize = new Size2(elementStartSize.Width + mouseOffset.x, elementStartSize.Height);
                sendSizeChange(newSize, true, false);
            }
        }

        private void sendSizeChange(Size2 newSize, bool adjustWidth, bool adjustHeight)
        {
            if (newSize.Width < minSize)
            {
                newSize.Width = minSize;
            }
            if (newSize.Height < minSize)
            {
                newSize.Height = minSize;
            }
            float ratio = selectedElement.Context.ZoomLevel * ScaleHelper.ScaleFactor;
            newSize = newSize / ratio;

            elementStrategy.changeSizePreview(selectedElement, (IntSize2)newSize, adjustWidth, adjustHeight);
            updateHighlightPosition();
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

        void dragHandle_Released(Widget source, EventArgs e)
        {
            if (elementStrategy != null && selectedElement != null)
            {
                elementStrategy.applySizeChange(selectedElement);
                updateHighlightPosition();
            }
        }
    }
}
