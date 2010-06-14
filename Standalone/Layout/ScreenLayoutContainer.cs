using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace Medical
{
    public abstract class ScreenLayoutContainer
    {
        private ScreenLayoutContainer parent = null;

        public ScreenLayoutContainer()
        {
            
        }

        public ScreenLayoutContainer ParentContainer
        {
            get
            {
                return parent;
            }
        }

        public abstract void layout();

        public Size WorkingSize { get; set; }

        public Vector2 Location { get; set; }

        public abstract Size DesiredSize { get; }

        public Size TopmostWorkingSize
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

        /// <summary>
        /// Internal function to set the parent, should only be called by other ScreenLayoutContainers.
        /// </summary>
        /// <param name="parent"></param>
        internal void _setParent(ScreenLayoutContainer parent)
        {
            this.parent = parent;
        }
    }
}
