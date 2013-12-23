using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    public class SingleChildConcreteLayoutContainer : SingleChildLayoutContainer
    {
        private LayoutContainer child;
        private float alpha = 1.0f;
        private bool visible = true;

        public SingleChildConcreteLayoutContainer()
        {

        }

        public override LayoutContainer Child
        {
            get
            {
                return child;
            }
            set
            {
                if (child != null)
                {
                    child._setParent(null);
                }
                child = value;
                if (child != null)
                {
                    child._setParent(this);
                    child.setAlpha(alpha);
                    child.Visible = visible;
                }
            }
        }

        public override void bringToFront()
        {
            if (child != null)
            {
                child.bringToFront();
            }
        }

        public override void setAlpha(float alpha)
        {
            this.alpha = alpha;
            if (child != null)
            {
                child.setAlpha(alpha);
            }
        }

        public override void layout()
        {
            if (child != null)
            {
                child.Location = Location;
                child.WorkingSize = WorkingSize;
                child.layout();
            }
        }

        public override Engine.IntSize2 DesiredSize
        {
            get
            {
                if (child != null)
                {
                    return child.DesiredSize;
                }
                return new IntSize2(0, 0);
            }
        }

        public override bool Visible
        {
            get
            {
                if (child != null)
                {
                    return child.Visible;
                }
                return visible;
            }
            set
            {
                visible = value;
                if (child != null)
                {
                    child.Visible = visible;
                }
            }
        }
    }
}
