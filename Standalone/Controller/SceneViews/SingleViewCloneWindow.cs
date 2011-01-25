﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using Engine.Renderer;
using OgrePlugin;
using System.Drawing;

namespace Medical.Controller
{
    class SingleViewCloneWindow : SceneViewWindow
    {
        public event EventHandler Closed;

        private RendererWindow rendererWindow;
        //private Frame frame;
        private WxOSWindow osWindow;

        public SingleViewCloneWindow(WindowInfo windowInfo, SceneViewController controller, UpdateTimer mainTimer, CameraMover cameraMover, String name)
            :base(controller, mainTimer, cameraMover, name)
        {
            throw new NotImplementedException();
            Point location = new Point(-1, -1);
            //wx.Display targetDisplay = wx.Display.GetDisplay(windowInfo.MonitorIndex);
            //location = targetDisplay.Geometry.Location;
            //location.Y = -1;
            //frame = new wx.Frame(Medical.GUI.MainWindow.Instance, "Clone Window", location, new Size(windowInfo.Width, windowInfo.Height));
            //osWindow = new WxOSWindow(frame);
            this.rendererWindow = OgreInterface.Instance.createRendererWindow(new WindowInfo(osWindow, "CloneWindow"));
            AllowNavigation = false;
            //frame.Show();
            //frame.EVT_CLOSE(onClose);

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
            throw new NotImplementedException();
            //frame.Close();   
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

        private void onClose(/*object sender, Event e*/)
        {
            //e.Skip();
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
