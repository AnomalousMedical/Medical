﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    public class NullLayoutContainer : SingleChildLayoutContainer
    {
        private LayoutContainer child;

        public NullLayoutContainer()
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
                }
            }
        }

        public override void bringToFront()
        {
            child.bringToFront();
        }

        public override void setAlpha(float alpha)
        {
            child.setAlpha(alpha);
        }

        public override void layout()
        {
            child.Location = Location;
            child.WorkingSize = WorkingSize;
            child.layout();
        }

        public override Engine.IntSize2 DesiredSize
        {
            get
            {
                return child.DesiredSize;
            }
        }

        public override bool Visible
        {
            get
            {
                return child.Visible;
            }
            set
            {
                child.Visible = value;
            }
        }
    }
}
