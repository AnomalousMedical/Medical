using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace Medical.Controller
{
    class SceneViewLayoutContainer : ScreenLayoutContainer
    {
        private SceneViewWindow sceneViewWindow;

        public SceneViewLayoutContainer()
        {
            
        }

        public void setWindow(SceneViewWindow sceneViewWindow)
        {
            this.sceneViewWindow = sceneViewWindow;
            sceneViewWindow._setParent(this);
            invalidate();
        }

        public override void setAlpha(float alpha)
        {

        }

        public override void layout()
        {
            if (sceneViewWindow != null)
            {
                sceneViewWindow.Location = this.Location;
                sceneViewWindow.WorkingSize = this.WorkingSize;
                sceneViewWindow.layout();
            }
        }

        public override Size DesiredSize
        {
            get 
            {
                if (sceneViewWindow != null)
                {
                    return sceneViewWindow.DesiredSize;
                }
                return new Size();
            }
        }
    }
}
