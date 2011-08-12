using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using MyGUIPlugin;
using Engine.Renderer;
using Engine.ObjectManagement;

namespace Medical.Controller
{
    public class MDISceneViewWindow : SceneViewWindow
    {
        private MDIDocumentWindow mdiWindow;
        private OgreRenderManager rm;

        public MDISceneViewWindow(OgreRenderManager rm, SceneViewController controller, UpdateTimer mainTimer, CameraMover cameraMover, String name)
            :base(controller, mainTimer, cameraMover, name)
        {
            this.rm = rm;

            //MDI Window
            mdiWindow = new MDIDocumentWindow(Name);
            mdiWindow.AllowedDockLocations = DockLocation.Center;
            mdiWindow.SuppressLayout = true;
            mdiWindow.Content = this;
            mdiWindow.SuppressLayout = false;
            mdiWindow.Caption = Name;
            mdiWindow.Closed += new EventHandler(mdiWindow_Closed);
            mdiWindow.ActiveStatusChanged += new EventHandler(mdiWindow_ActiveStatusChanged);
        }

        public override void Dispose()
        {
            base.Dispose();
            mdiWindow.Dispose();
        }

        public override void createSceneView(RendererWindow window, SimScene scene)
        {
            base.createSceneView(window, scene);
            rm.setActiveViewport(rm.getActiveViewport() + 1);
        }

        public override void destroySceneView()
        {
            base.destroySceneView();
            rm.setActiveViewport(rm.getActiveViewport() - 1);
        }

        public override void close()
        {
            mdiWindow.close();
        }

        public bool AllowClose
        {
            get
            {
                return mdiWindow.AllowClose;
            }
            set
            {
                mdiWindow.AllowClose = value;
            }
        }

        /// <summary>
        /// Get the MDIWindow for this window. Do not touch unless you are SceneViewController.
        /// </summary>
        /// <returns>The MDIWindow for this window.</returns>
        internal MDIWindow _getMDIWindow()
        {
            return mdiWindow;
        }

        void mdiWindow_Closed(object sender, EventArgs e)
        {
            controller.destroyWindow(this);
        }

        void mdiWindow_ActiveStatusChanged(object sender, EventArgs e)
        {
            if (mdiWindow.Active)
            {
                TransparencyController.ActiveTransparencyState = CurrentTransparencyState;
            }
        }
    }
}
