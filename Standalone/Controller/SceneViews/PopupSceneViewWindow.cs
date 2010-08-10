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
            OgreInterface.Instance.destroyRendererWindow(rendererWindow);
        }

        public void focusChanged(OSWindow window)
        {

        }

        #endregion
    }
}
