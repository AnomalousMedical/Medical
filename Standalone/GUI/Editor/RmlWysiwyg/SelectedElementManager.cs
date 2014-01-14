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
        private Widget xAdjust;
        private Widget yAdjust;
        private Widget xyAdjust;
        private Widget yWidthAdjust;
        private Widget xHeightAdjust;

        private Rect elementStartRect;
        private IntVector2 mouseStartPosition;
        private Widget parentWidget;

        public SelectedElementManager(Widget parentWidget)
        {
            this.parentWidget = parentWidget;
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

            xAdjust = parentWidget.findWidget("XAdjust");
            xAdjust.MouseDrag += xAdjust_MouseDrag;
            xAdjust.MouseButtonPressed += dragHandle_Pressed;
            xAdjust.MouseButtonReleased += dragHandle_Released;

            yAdjust = parentWidget.findWidget("YAdjust");
            xyAdjust = parentWidget.findWidget("XYAdjust");
            yWidthAdjust = parentWidget.findWidget("YWidthAdjust");
            xHeightAdjust = parentWidget.findWidget("XHeightAdjust");
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
                heightAdjust.setPosition(selectionLeft + selectionWidth / 2 - heightAdjust.Width / 2, selectionBottom - heightAdjust.Height / 2);
                bothAdjust.setPosition(selectionRight - bothAdjust.Width / 2, selectionBottom - bothAdjust.Height / 2);
                xAdjust.setPosition(selectionLeft - xAdjust.Width / 2, selectionTop + selectionHeight / 2 - xAdjust.Height / 2);
                yAdjust.setPosition(selectionLeft + selectionWidth / 2 - yAdjust.Width / 2, selectionTop - yAdjust.Height / 2);
                xyAdjust.setPosition(selectionLeft - xAdjust.Width / 2, selectionTop - yAdjust.Height / 2);
                yWidthAdjust.setPosition(selectionRight - yWidthAdjust.Width / 2, selectionTop - yWidthAdjust.Height / 2);
                xHeightAdjust.setPosition(selectionLeft - xHeightAdjust.Width / 2, selectionBottom - xHeightAdjust.Height / 2);
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
            xAdjust.Visible = show;
            yAdjust.Visible = show;
            xyAdjust.Visible = show;
            yWidthAdjust.Visible = show;
            xHeightAdjust.Visible = show;
        }

        void genericAdjust(Widget source, EventArgs e, Func<IntVector2, Rect> computeSizeCallback)
        {
            if (elementStrategy != null && selectedElement != null)
            {
                MouseEventArgs me = (MouseEventArgs)e;
                IntVector2 mouseOffset = me.Position - mouseStartPosition;
                Rect newSize = computeSizeCallback(mouseOffset);
                sendSizeChange(newSize, ResizeType.WidthHeight, new IntSize2(parentWidget.Width, parentWidget.Height));
            }
        }

        void bothAdjust_MouseDrag(Widget source, EventArgs e)
        {
            genericAdjust(source, e, (mouseOffset) => new Rect(elementStartRect.Left, elementStartRect.Top, elementStartRect.Width + mouseOffset.x, elementStartRect.Height + mouseOffset.y));
        }

        void heightAdjust_MouseDrag(Widget source, EventArgs e)
        {
            genericAdjust(source, e, (mouseOffset) => new Rect(elementStartRect.Left, elementStartRect.Top, elementStartRect.Width, elementStartRect.Height + mouseOffset.y));
        }

        void widthAdjust_MouseDrag(Widget source, EventArgs e)
        {
            genericAdjust(source, e, (mouseOffset) => new Rect(elementStartRect.Left, elementStartRect.Top, elementStartRect.Width + mouseOffset.x, elementStartRect.Height));
        }

        void xAdjust_MouseDrag(Widget source, EventArgs e)
        {
            genericAdjust(source, e, (mouseOffset) => new Rect(elementStartRect.Left + mouseOffset.x, elementStartRect.Top, elementStartRect.Width, elementStartRect.Height));
        }

        private void sendSizeChange(Rect newRect, ResizeType resizeType, IntSize2 boundsRect)
        {
            float ratio = selectedElement.Context.ZoomLevel * ScaleHelper.ScaleFactor;
            newRect = newRect / ratio;
            boundsRect = (IntSize2)(boundsRect / ratio);

            elementStrategy.changeSizePreview(selectedElement, (IntRect)newRect, resizeType, boundsRect);
            updateHighlightPosition();
        }

        void dragHandle_Pressed(Widget source, EventArgs e)
        {
            if (selectedElement != null)
            {
                float ratio = selectedElement.Context.ZoomLevel * ScaleHelper.ScaleFactor;
                MouseEventArgs me = (MouseEventArgs)e;
                mouseStartPosition = me.Position;
                elementStartRect = elementStrategy.getStartingRect(selectedElement) * ratio;
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
