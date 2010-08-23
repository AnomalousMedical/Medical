using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using Engine.Renderer;
using OgrePlugin;
using System.Drawing;

namespace Medical.Controller
{
    class PopupSceneViewWindow : SceneViewWindow
    {
        public event EventHandler Closed;

        private RendererWindow rendererWindow;
        private wx.Frame frame;
        private WxOSWindow osWindow;

        public PopupSceneViewWindow(WindowInfo windowInfo, SceneViewController controller, UpdateTimer mainTimer, CameraMover cameraMover, String name)
            :base(controller, mainTimer, cameraMover, name)
        {
            Point location = new Point(-1, -1);
#if WINDOWS //For some reason this is not working on mac, so for now just ignore it.
            wx.Display targetDisplay = wx.Display.GetDisplay(windowInfo.MonitorIndex);
            location = targetDisplay.Geometry.Location;
#endif
            frame = new wx.Frame(Medical.GUI.MainWindow.Instance, "Clone Window", location, new Size(windowInfo.Width, windowInfo.Height));
            osWindow = new WxOSWindow(frame);
            this.rendererWindow = OgreInterface.Instance.createRendererWindow(new WindowInfo(osWindow, "CloneWindow"));
            AllowNavigation = false;
            frame.Show();
            frame.EVT_CLOSE(onClose);
        }

        public override void createSceneView(RendererWindow window, Engine.ObjectManagement.SimScene scene)
        {
            //Ignore the window passed in and use the member one instead
            base.createSceneView(rendererWindow, scene);
        }

        public override void close()
        {
            frame.Close();   
        }

        public override bool Focused
        {
            get
            {
                return false;
            }
            set
            {
                
            }
        }

        public RendererWindow RendererWindow
        {
            get
            {
                return rendererWindow;
            }
        }

        private void onClose(object sender, wx.Event e)
        {
            e.Skip();
            controller.destroyWindow(this);
            if (Closed != null)
            {
                Closed.Invoke(this, EventArgs.Empty);
            }
            OgreInterface.Instance.destroyRendererWindow(rendererWindow);
        }
    }
}
