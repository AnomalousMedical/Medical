using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using Engine.Renderer;
using OgrePlugin;
using System.Drawing;
using Medical.GUI;
using Engine;

namespace Medical.Controller
{
    class SingleViewCloneWindow : SceneViewWindow
    {
        public event EventHandler Closed;

        private OgreWindow rendererWindow;
        private NativeOSWindow osWindow;

        public SingleViewCloneWindow(WindowInfo windowInfo, SceneViewController controller, UpdateTimer mainTimer, CameraMover cameraMover, String name, BackgroundScene background, int zIndexStart, bool floatOnParent)
            :base(controller, mainTimer, cameraMover, name, background, zIndexStart)
        {
            IntVector2 location = SystemInfo.getDisplayLocation(windowInfo.MonitorIndex);
            location.y = -1;
            osWindow = new NativeOSWindow(MainWindow.Instance, "Clone Window", location, new IntSize2(windowInfo.Width, windowInfo.Height), floatOnParent);
            this.rendererWindow = (OgreWindow)OgreInterface.Instance.createRendererWindow(new WindowInfo(osWindow, "CloneWindow"));
            this.createBackground(rendererWindow.OgreRenderTarget, true);
            this.listenForCameraMoverUpdates();
            this.RendererWindow = rendererWindow;
            osWindow.show();
            osWindow.Closed += osWindow_Closed;

            transparencyStateName = controller.ActiveWindow.CurrentTransparencyState;
            controller.ActiveWindowChanged += controller_ActiveWindowChanged;
        }

        public override void Dispose()
        {
            //Reset the transparencyStateName so that it does not wipe out an existing state
            transparencyStateName = Name;
            base.Dispose();
        }

        public override void close()
        {
            osWindow.close();
        }

        void osWindow_Closed(OSWindow sender)
        {
            controller.destroyWindow(this);
            if (Closed != null)
            {
                Closed.Invoke(this, EventArgs.Empty);
            }
            OgreInterface.Instance.destroyRendererWindow(rendererWindow);
            controller.ActiveWindowChanged -= controller_ActiveWindowChanged;
        }

        void controller_ActiveWindowChanged(SceneViewWindow window)
        {
            transparencyStateName = window.CurrentTransparencyState;
        }
    }
}
