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
        private HighlightProvider highlightProvider;
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
        private bool leftAdjustAnchor = true;

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
            yAdjust.MouseDrag += yAdjust_MouseDrag;
            yAdjust.MouseButtonPressed += dragHandle_Pressed;
            yAdjust.MouseButtonReleased += dragHandle_Released;

            xyAdjust = parentWidget.findWidget("XYAdjust");
            xyAdjust.MouseDrag += xyAdjust_MouseDrag;
            xyAdjust.MouseButtonPressed += dragHandle_Pressed;
            xyAdjust.MouseButtonReleased += dragHandle_Released;

            yWidthAdjust = parentWidget.findWidget("YWidthAdjust");
            yWidthAdjust.MouseDrag += yWidthAdjust_MouseDrag;
            yWidthAdjust.MouseButtonPressed += dragHandle_Pressed;
            yWidthAdjust.MouseButtonReleased += dragHandle_Released;

            xHeightAdjust = parentWidget.findWidget("XHeightAdjust");
            xHeightAdjust.MouseDrag += xHeightAdjust_MouseDrag;
            xHeightAdjust.MouseButtonPressed += dragHandle_Pressed;
            xHeightAdjust.MouseButtonReleased += dragHandle_Released;
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
                if (highlightProvider != null)
                {
                    IntRect additionalHighlightRect = highlightProvider.getAdditionalHighlightAreaRect(highlightElement);
                    float ratio = selectedElement.Context.ZoomLevel * ScaleHelper.ScaleFactor;
                    additionalHighlightRect = (IntRect)(additionalHighlightRect * ratio);

                    selectionLeft += additionalHighlightRect.Left;
                    selectionTop += additionalHighlightRect.Top;
                    selectionWidth += additionalHighlightRect.Width;
                    selectionHeight += additionalHighlightRect.Height;
                }
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
                showResizeHandles(elementStrategy != null ? elementStrategy.ResizeHandles : ResizeType.None);
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

        public void setHighlightElement(Element highlightElement, HighlightProvider highlightProvider)
        {
            this.highlightElement = highlightElement;
            this.highlightProvider = highlightProvider;
            if (this.highlightElement != null)
            {
                selectionWidget.Visible = true;
                updateHighlightPosition();
            }
            else
            {
                selectionWidget.Visible = false;
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
                setHighlightElement(value, null);
            }
        }

        private void showResizeHandles(ResizeType handles)
        {
            if (elementStrategy != null)
            {
                widthAdjust.Visible = (handles & ResizeType.Width) == ResizeType.Width;
                heightAdjust.Visible = (handles & ResizeType.Height) == ResizeType.Height;
                bothAdjust.Visible = (handles & ResizeType.WidthHeight) == ResizeType.WidthHeight;
                xAdjust.Visible = (handles & ResizeType.Left) == ResizeType.Left;
                yAdjust.Visible = (handles & ResizeType.Top) == ResizeType.Top;
                xyAdjust.Visible = (handles & ResizeType.LeftTop) == ResizeType.LeftTop;
                yWidthAdjust.Visible = (handles & ResizeType.TopWidth) == ResizeType.TopWidth;
                xHeightAdjust.Visible = (handles & ResizeType.LeftHeight) == ResizeType.LeftHeight;
            }
            else
            {
                widthAdjust.Visible = false;
                heightAdjust.Visible = false;
                bothAdjust.Visible = false;
                xAdjust.Visible = false;
                yAdjust.Visible = false;
                xyAdjust.Visible = false;
                yWidthAdjust.Visible = false;
                xHeightAdjust.Visible = false;
            }
        }

        void genericAdjust(Widget source, EventArgs e, ResizeType resizeType, Func<IntVector2, Rect> computeSizeCallback)
        {
            if (elementStrategy != null && selectedElement != null)
            {
                MouseEventArgs me = (MouseEventArgs)e;
                IntVector2 mouseOffset = me.Position - mouseStartPosition;
                Rect newRect = computeSizeCallback(mouseOffset);

                IntSize2 boundsRect = new IntSize2(parentWidget.Width, parentWidget.Height);
                float ratio = selectedElement.Context.ZoomLevel * ScaleHelper.ScaleFactor;
                newRect = newRect / ratio;
                boundsRect = (IntSize2)(boundsRect / ratio);

                elementStrategy.changeSizePreview(selectedElement, (IntRect)newRect, resizeType, boundsRect);
                updateHighlightPosition();
            }
        }

        void bothAdjust_MouseDrag(Widget source, EventArgs e)
        {
            genericAdjust(source, e, ResizeType.WidthHeight, (mouseOffset) => new Rect(elementStartRect.Left, elementStartRect.Top, elementStartRect.Width + mouseOffset.x, elementStartRect.Height + mouseOffset.y));
        }

        void heightAdjust_MouseDrag(Widget source, EventArgs e)
        {
            genericAdjust(source, e, ResizeType.Height, (mouseOffset) => new Rect(elementStartRect.Left, elementStartRect.Top, elementStartRect.Width, elementStartRect.Height + mouseOffset.y));
        }

        void widthAdjust_MouseDrag(Widget source, EventArgs e)
        {
            if (leftAdjustAnchor)
            {
                genericAdjust(source, e, ResizeType.Width, (mouseOffset) => new Rect(elementStartRect.Left, elementStartRect.Top, elementStartRect.Width + mouseOffset.x, elementStartRect.Height));
            }
            else
            {
                genericAdjust(source, e, ResizeType.Left, (mouseOffset) => new Rect(elementStartRect.Left - mouseOffset.x, elementStartRect.Top, elementStartRect.Width, elementStartRect.Height));
            }
        }

        void xAdjust_MouseDrag(Widget source, EventArgs e)
        {
            if (leftAdjustAnchor)
            {
                genericAdjust(source, e, ResizeType.Left, (mouseOffset) => new Rect(elementStartRect.Left + mouseOffset.x, elementStartRect.Top, elementStartRect.Width, elementStartRect.Height));
            }
            else
            {
                genericAdjust(source, e, ResizeType.Width, (mouseOffset) => new Rect(elementStartRect.Left, elementStartRect.Top, elementStartRect.Width - mouseOffset.x, elementStartRect.Height));
            }
        }

        void yAdjust_MouseDrag(Widget source, EventArgs e)
        {
            genericAdjust(source, e, ResizeType.Top, (mouseOffset) => new Rect(elementStartRect.Left, elementStartRect.Top + mouseOffset.y, elementStartRect.Width, elementStartRect.Height));
        }

        void xyAdjust_MouseDrag(Widget source, EventArgs e)
        {
            if (leftAdjustAnchor)
            {
                genericAdjust(source, e, ResizeType.LeftTop, (mouseOffset) => new Rect(elementStartRect.Left + mouseOffset.x, elementStartRect.Top + mouseOffset.y, elementStartRect.Width, elementStartRect.Height));
            }
            else
            {
                genericAdjust(source, e, ResizeType.TopWidth, (mouseOffset) => new Rect(elementStartRect.Left, elementStartRect.Top + mouseOffset.y, elementStartRect.Width - mouseOffset.x, elementStartRect.Height));
            }
        }

        void yWidthAdjust_MouseDrag(Widget source, EventArgs e)
        {
            if (leftAdjustAnchor)
            {
                genericAdjust(source, e, ResizeType.TopWidth, (mouseOffset) => new Rect(elementStartRect.Left, elementStartRect.Top + mouseOffset.y, elementStartRect.Width + mouseOffset.x, elementStartRect.Height));
            }
            else
            {
                genericAdjust(source, e, ResizeType.LeftTop, (mouseOffset) => new Rect(elementStartRect.Left - mouseOffset.x, elementStartRect.Top + mouseOffset.y, elementStartRect.Width, elementStartRect.Height));
            }
        }

        void xHeightAdjust_MouseDrag(Widget source, EventArgs e)
        {
            genericAdjust(source, e, ResizeType.LeftHeight, (mouseOffset) => new Rect(elementStartRect.Left + mouseOffset.x, elementStartRect.Top, elementStartRect.Width, elementStartRect.Height + mouseOffset.y));
        }

        void dragHandle_Pressed(Widget source, EventArgs e)
        {
            if (selectedElement != null)
            {
                float ratio = selectedElement.Context.ZoomLevel * ScaleHelper.ScaleFactor;
                MouseEventArgs me = (MouseEventArgs)e;
                mouseStartPosition = me.Position;
                elementStartRect = elementStrategy.getStartingRect(selectedElement, out leftAdjustAnchor) * ratio;
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
