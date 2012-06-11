using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller.AnomalousMvc;
using MyGUIPlugin;
using Engine;

namespace Medical.GUI.AnomalousMvc
{
    class WindowDecorator : ViewHostComponent, ButtonFactory, IDisposable
    {
        private ViewHostComponent child;

        protected Window window;
        private String closeAction = null;

        public WindowDecorator(ViewHostComponent child, ButtonCollection buttons)
        {
            if (buttons.Count > 0)
            {
                //Keep button decorator from being made if there is only one button and it is close
                if (buttons.Count == 1 && buttons.hasItem("Close"))
                {
                    buttons["Close"].createButton(this, 0, 0, 0, 0);
                }
                else
                {
                    child = new ButtonDecorator(child, buttons, this);
                }
            }

            if (closeAction != null)
            {
                window = (Window)Gui.Instance.createWidgetT("Window", "WindowCSX", 300, 150, 260, 440, Align.Default, "Overlapped", "");
                window.WindowButtonPressed += new MyGUIEvent(window_WindowButtonPressed);
            }
            else
            {
                window = (Window)Gui.Instance.createWidgetT("Window", "WindowCS", 300, 150, 260, 440, Align.Default, "Overlapped", "");
            }

            int widthDiffernce = window.ClientCoord.width - window.Width;
            int heightDifference = window.ClientCoord.height - window.Height;

            window.setCoord(child.Widget.Left, child.Widget.Top, child.Widget.Width + widthDiffernce, child.Widget.Height + heightDifference);

            this.child = child;
            child.Widget.attachToWidget(window);
            IntCoord clientCoord = window.ClientCoord;
            child.Widget.setCoord(0, 0, clientCoord.width, clientCoord.height);
            child.Widget.Align = Align.Stretch;

            window.WindowChangedCoord += new MyGUIEvent(window_WindowChangedCoord);
            child.topLevelResized();
        }

        void window_WindowButtonPressed(Widget source, EventArgs e)
        {
            MyGUIViewHost viewHost = child.ViewHost;
            viewHost.Context.runAction(closeAction, viewHost);
        }

        public void Dispose()
        {
            child.Dispose();
            Gui.Instance.destroyWidget(window);
        }

        public void opening()
        {
            ensureVisible();
            LayerManager.Instance.upLayerItem(window);
            window.setVisibleSmooth(true);
            child.opening();
        }

        public void closing()
        {
            child.closing();
            window.setVisibleSmooth(false);
        }

        public void topLevelResized()
        {
            child.topLevelResized();
        }

        public MyGUIViewHost ViewHost
        {
            get
            {
                return child.ViewHost;
            }
        }

        public Widget Widget
        {
            get
            {
                return window;
            }
        }

        public bool _RequestClosed { get; set; }

        void window_WindowChangedCoord(Widget source, EventArgs e)
        {
            child.topLevelResized();
        }

        public void _animationCallback(LayoutContainer oldChild)
        {
            Dispose();
        }

        public void addTextButton(ButtonDefinition buttonDefinition, int x, int y, int width, int height)
        {
            ((ButtonDecorator)child).addTextButton(buttonDefinition, x, y, width, height);
        }

        public void addCloseButton(CloseButtonDefinition buttonDefinition, int x, int y, int width, int height)
        {
            closeAction = buttonDefinition.Action;
        }

        /// <summary>
        /// Have the window compute its position to ensure it is visible in the given screen area.
        /// </summary>
        private void ensureVisible()
        {
            //Adjust the position if needed
            int left = window.Left;
            int top = window.Top;
            int right = left + window.Width;
            int bottom = top + window.Height;

            int guiWidth = RenderManager.Instance.ViewWidth;
            int guiHeight = RenderManager.Instance.ViewHeight;

            if (right > guiWidth)
            {
                left -= right - guiWidth;
                if (left < 0)
                {
                    left = 0;
                }
            }

            if (bottom > guiHeight)
            {
                top -= bottom - guiHeight;
                if (top < 0)
                {
                    top = 0;
                }
            }

            window.setPosition(left, top);
        }
    }
}
