using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    public class NullLayoutContainer : LayoutContainer
    {
        private float alpha = 1.0f;
        private bool visible = true;
        private IntSize2 desiredSize;

        public NullLayoutContainer()
        {

        }

        public NullLayoutContainer(IntSize2 desiredSize)
        {
            this.desiredSize = desiredSize;
        }

        public override void bringToFront()
        {
            
        }

        public override void setAlpha(float alpha)
        {
            this.alpha = alpha;
        }

        public override void layout()
        {
            
        }

        public void setDesiredSize(IntSize2 desiredSize)
        {
            this.desiredSize = desiredSize;
        }

        public override IntSize2 DesiredSize
        {
            get
            {
                return desiredSize;
            }
        }        

        public override bool Visible
        {
            get
            {
                return visible;
            }
            set
            {
                visible = value;
            }
        }
    }
}
