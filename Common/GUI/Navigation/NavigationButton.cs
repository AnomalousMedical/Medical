using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OgreWrapper;
using Engine.Platform;
using Engine;
using Logging;

namespace Medical
{
    delegate void NavigationButtonClicked(NavigationButton source);

    /// <summary>
    /// This is a button class using ogre overlays.
    /// </summary>
    class NavigationButton : IDisposable
    {
        public event NavigationButtonClicked Clicked;

        private OverlayRect normalCoords;
        private OverlayRect hoverCoords;
        private OverlayRect pressedCoords;
        private OverlayRect boundsRect;
        private PanelOverlayElement button;

        public NavigationButton(String name, String material, OverlayRect boundsRect, OverlayRect normalCoords, OverlayRect hoverCoords, OverlayRect pressedCoords)
        {
            button = OverlayManager.getInstance().createOverlayElement(PanelOverlayElement.TypeName, name) as PanelOverlayElement;
            button.setUV(normalCoords.X0, normalCoords.Y0, normalCoords.X1, normalCoords.Y1);
            button.setMaterialName("NavigationArrow");
            button.setMetricsMode(GuiMetricsMode.GMM_PIXELS);
            this.normalCoords = normalCoords;
            this.hoverCoords = hoverCoords;
            this.pressedCoords = pressedCoords;
            this.BoundsRect = boundsRect;
        }

        public void Dispose()
        {
            OverlayManager.getInstance().destroyOverlayElement(button);
        }

        internal bool process(Vector3 mousePos, bool leftButtonClicked, float vpWidth, float vpHeight)
        {
            bool clickingOn = false;
            float xPos = 0.0f;
            float yPos = 0.0f;
            switch (button.getHorizontalAlignment())
            {
                case GuiHorizontalAlignment.GHA_CENTER:
                    xPos = vpWidth * 0.5f;
                    break;
                case GuiHorizontalAlignment.GHA_RIGHT:
                    xPos = vpWidth;
                    break;
            }
            switch (button.getVerticalAlignment())
            {
                case GuiVerticalAlignment.GVA_CENTER:
                    yPos = vpHeight * 0.5f;
                    break;
                case GuiVerticalAlignment.GVA_BOTTOM:
                    yPos = vpHeight;
                    break;
            }
            float mouseX = mousePos.x - xPos - button.getLeft();
            float mouseY = mousePos.y - yPos - button.getTop();
            if (mouseX >= 0 && mouseX <= button.getWidth() && mouseY >= 0 && mouseY <= button.getHeight())
            {
                if (leftButtonClicked)
                {
                    button.setUV(pressedCoords.X0, pressedCoords.Y0, pressedCoords.X1, pressedCoords.Y1);
                    clickingOn = true;
                }
                else
                {
                    button.setUV(hoverCoords.X0, hoverCoords.Y0, hoverCoords.X1, hoverCoords.Y1);
                }
            }
            else
            {
                button.setUV(normalCoords.X0, normalCoords.Y0, normalCoords.X1, normalCoords.Y1);
            }
            return clickingOn;
        }

        internal void fireClickEvent()
        {
            if (Clicked != null)
            {
                Clicked.Invoke(this);
            }
        }

        public GuiHorizontalAlignment HorizontalAlignment
        {
            get
            {
                return button.getHorizontalAlignment();
            }
            set
            {
                button.setHorizontalAlignment(value);
            }
        }

        public GuiVerticalAlignment VerticalAlignment
        {
            get
            {
                return button.getVerticalAlignment();
            }
            set
            {
                button.setVerticalAlignment(value);
            }
        }

        public OverlayRect NormalCoords
        {
            get
            {
                return normalCoords;
            }
            set
            {
                normalCoords = value;
            }
        }

        public OverlayRect HoverCoords
        {
            get
            {
                return hoverCoords;
            }
            set
            {
                hoverCoords = value;
            }
        }

        public OverlayRect PressedCoords
        {
            get
            {
                return pressedCoords;
            }
            set
            {
                pressedCoords = value;
            }
        }

        public OverlayRect BoundsRect
        {
            get
            {
                return boundsRect;
            }
            set
            {
                boundsRect = value;
                button.setDimensions(boundsRect.X1, boundsRect.Y1);
                button.setPosition(boundsRect.X0, boundsRect.Y0);
            }
        }

        public NavigationState State { get; set; }

        internal PanelOverlayElement PanelElement
        {
            get
            {
                return button;
            }
        }
    }
}
