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
        private MDIWindow mdiWindow;
        private OgreRenderManager rm;

        public MDISceneViewWindow(OgreRenderManager rm, SceneViewController controller, UpdateTimer mainTimer, CameraMover cameraMover, String name)
            :base(controller, mainTimer, cameraMover, name)
        {
            this.rm = rm;

            //MDI Window
            mdiWindow = new MDIWindow("MDIWindow.layout", Name);
            mdiWindow.SuppressLayout = true;
            mdiWindow.Content = this;
            mdiWindow.SuppressLayout = false;
            mdiWindow.Caption = Name;
            mdiWindow.Closed += new EventHandler(mdiWindow_Closed);
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

        public override bool Focused
        {
            get
            {
                return mdiWindow.Active;
            }
            set
            {
                mdiWindow.Active = value;
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
    }
}
