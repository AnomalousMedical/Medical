using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using Engine.Renderer;
using OgrePlugin;

namespace Medical.Controller
{
    class PopupSceneViewWindow : SceneViewWindow, OSWindowListener
    {
        public event EventHandler Closed;

        private RendererWindow rendererWindow;

        public PopupSceneViewWindow(RendererWindow rendererWindow, SceneViewController controller, UpdateTimer mainTimer, CameraMover cameraMover, String name)
            :base(controller, mainTimer, cameraMover, name)
        {
            this.rendererWindow = rendererWindow;
            rendererWindow.Handle.addListener(this);
            AllowNavigation = false;
        }

        public override void createSceneView(RendererWindow window, Engine.ObjectManagement.SimScene scene)
        {
            //Ignore the window passed in and use the member one instead
            base.createSceneView(rendererWindow, scene);
        }

        public override void close()
        {
            
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

        #region OSWindowListener Members

        public void closing(OSWindow window)
        {
            controller.destroyWindow(this);
        }

        public void moved(OSWindow window)
        {
            
        }

        public void resized(OSWindow window)
        {
            
        }

        public void closed(OSWindow window)
        {
            if (Closed != null)
            {
                Closed.Invoke(this, EventArgs.Empty);
            }
            OgreInterface.Instance.destroyRendererWindow(rendererWindow);
        }

        public void focusChanged(OSWindow window)
        {

        }

        #endregion
    }
}
