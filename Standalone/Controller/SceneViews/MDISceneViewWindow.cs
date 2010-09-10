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
        private Button transparencyMode;

        public MDISceneViewWindow(OgreRenderManager rm, SceneViewController controller, UpdateTimer mainTimer, CameraMover cameraMover, String name)
            :base(controller, mainTimer, cameraMover, name)
        {
            this.rm = rm;

            //MDI Window
            mdiWindow = new MDIWindow(Name);
            mdiWindow.SuppressLayout = true;
            mdiWindow.Content = this;
            mdiWindow.SuppressLayout = false;
            mdiWindow.Caption = Name;
            mdiWindow.Closed += new EventHandler(mdiWindow_Closed);
            mdiWindow.ActiveStatusChanged += new EventHandler(mdiWindow_ActiveStatusChanged);

            transparencyMode = mdiWindow.findChildWidget("TransparencyModeButton") as Button;
            transparencyMode.MouseButtonClick += new MyGUIEvent(transparencyMode_MouseButtonClick);
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

        void transparencyMode_MouseButtonClick(Widget source, EventArgs e)
        {
            UseDefaultTransparency = !UseDefaultTransparency;
            if (UseDefaultTransparency)
            {
                transparencyMode.Caption = "Global";
            }
            else
            {
                transparencyMode.Caption = "Local";
            }
            TransparencyController.ActiveTransparencyState = CurrentTransparencyState;
        }
    }
}
