using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using Engine.Renderer;
using OgrePlugin;
using System.Drawing;
using Medical.GUI;

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
            Point location = SystemInfo.getDisplayLocation(windowInfo.MonitorIndex);
            location.Y = -1;
            osWindow = new NativeOSWindow(MainWindow.Instance, "Clone Window", location, new Size(windowInfo.Width, windowInfo.Height), floatOnParent);
            this.rendererWindow = (OgreWindow)OgreInterface.Instance.createRendererWindow(new WindowInfo(osWindow, "CloneWindow"));
            this.createBackground(rendererWindow.OgreRenderWindow, true);
            osWindow.show();
            osWindow.Closed += new EventHandler(osWindow_Closed);

            transparencyStateName = controller.ActiveWindow.CurrentTransparencyState;
            controller.ActiveWindowChanged += controller_ActiveWindowChanged;
        }

        public override void Dispose()
        {
            //Reset the transparencyStateName so that it does not wipe out an existing state
            transparencyStateName = Name;
            base.Dispose();
        }

        public override void createSceneView(RendererWindow window, Engine.ObjectManagement.SimScene scene)
        {
            //Ignore the window passed in and use the member one instead
            base.createSceneView(rendererWindow, scene);
        }

        public override void close()
        {
            osWindow.close();
        }

        public RendererWindow RendererWindow
        {
            get
            {
                return rendererWindow;
            }
        }

        void osWindow_Closed(object sender, EventArgs e)
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
