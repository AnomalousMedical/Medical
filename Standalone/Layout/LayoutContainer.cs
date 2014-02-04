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
        private bool rigid = true;

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

        public virtual void animatedResizeStarted(IntSize2 finalSize)
        {

        }

        public virtual void animatedResizeCompleted(IntSize2 finalSize)
        {

        }

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

        /// <summary>
        /// Find the size of the first parent container that is rigid up the chain.
        /// 
        /// If the container has no parent the current working size will be returned.
        /// </summary>
        public IntSize2 RigidParentWorkingSize
        {
            get
            {
                if (parent != null)
                {
                    if (parent.Rigid)
                    {
                        return parent.WorkingSize;
                    }
                    else
                    {
                        return parent.RigidParentWorkingSize;
                    }

                }
                else
                {
                    return WorkingSize;
                }
            }
        }

        /// <summary>
        /// Find the first rigid parent container up the chain.
        /// 
        /// If the container has no parent the current container will be returned.
        /// </summary>
        public LayoutContainer RigidParent
        {
            get
            {
                if (parent != null)
                {
                    if (parent.Rigid)
                    {
                        return parent;
                    }
                    else
                    {
                        return parent.RigidParent;
                    }

                }
                else
                {
                    return this;
                }
            }
        }

        public bool SuppressLayout { get; set; }

        /// <summary>
        /// Determine if this container is rigid or animated. Animated means that its size will
        /// be recalculated over time when the content is changed instead of snapping to position.
        /// </summary>
        protected bool Rigid
        {
            get
            {
                return rigid;
            }
            set
            {
                rigid = value;
            }
        }

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
