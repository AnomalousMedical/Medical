using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace Medical
{
    public abstract class LayoutContainer
    {
        private LayoutContainer parent = null;

        public LayoutContainer()
        {
            
        }

        public LayoutContainer ParentContainer
        {
            get
            {
                return parent;
            }
        }

        public void invalidate()
        {
            if (parent != null)
            {
                parent.invalidate();
            }
            else
            {
                if (!SuppressLayout)
                {
                    layout();
                }
            }
        }

        public abstract void bringToFront();

        public abstract void setAlpha(float alpha);

        public abstract void layout();

        public IntSize2 WorkingSize { get; set; }

        public IntVector2 Location { get; set; }

        public abstract IntSize2 DesiredSize { get; }

        public abstract bool Visible { get; set; }

        public IntSize2 TopmostWorkingSize
        {
            get
            {
                if(parent != null)
                {
                    return parent.TopmostWorkingSize;
                }
                else
                {
                    return WorkingSize;
                }
            }
        }

        public bool SuppressLayout { get; set; }

        /// <summary>
        /// Internal function to set the parent, should only be called by other ScreenLayoutContainers.
        /// </summary>
        /// <param name="parent"></param>
        internal void _setParent(LayoutContainer parent)
        {
            this.parent = parent;
        }
    }
}
