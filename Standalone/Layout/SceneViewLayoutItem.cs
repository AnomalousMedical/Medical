using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace Medical
{
    class SceneViewLayoutItem : ScreenLayoutContainer
    {
        private SceneView sceneView;

        public SceneViewLayoutItem(SceneView sceneView)
        {
            this.sceneView = sceneView;
        }

        public override void setAlpha(float alpha)
        {

        }

        public override void layout()
        {
            Size totalSize = TopmostWorkingSize;
            sceneView.setDimensions(Location.x / totalSize.Width, Location.y / totalSize.Height, WorkingSize.Width / totalSize.Width, WorkingSize.Height / totalSize.Height);
        }

        public override Size DesiredSize
        {
            get 
            {
                return new Size(sceneView.RenderWidth, sceneView.RenderHeight);
            }
        }
    }
}
