using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using Engine;

namespace Medical
{
    class VerticalPopoutLayoutContainer : LayoutContainer, UpdateListener
    {
        private UpdateTimer mainTimer;
        private LayoutContainer childContainer;
        private LayoutContainer oldChildContainer;

        private AnimationCompletedDelegate animationComplete;
        private float animationLength;
        private float currentTime;
        private bool animating = false;
        private float alpha = 1.0f;
        private Size2 oldSize;
        private Size2 newSize;
        private Size2 sizeDelta;
        private Size2 currentSize;

        public VerticalPopoutLayoutContainer(UpdateTimer mainTimer)
        {
            this.mainTimer = mainTimer;
        }

        public override void setAlpha(float alpha)
        {

        }

        public override void layout()
        {
            if (animating)
            {
                if (childContainer != null)
                {
                    childContainer.Location = Location;
                    childContainer.WorkingSize = currentSize;
                    childContainer.layout();
                }
                if (oldChildContainer != null)
                {
                    oldChildContainer.Location = Location;
                    oldChildContainer.WorkingSize = currentSize;
                    oldChildContainer.layout();
                }
            }
            else
            {
                if (childContainer != null)
                {
                    childContainer.Location = Location;
                    childContainer.WorkingSize = WorkingSize;
                    childContainer.layout();
                }
            }
        }

        /// <summary>
        /// Set the panel that will be displayed when this container is first shown
        /// </summary>
        /// <param name="childContainer"></param>
        public void setInitialPanel(LayoutContainer childContainer)
        {
            currentSize = childContainer.DesiredSize;
            this.childContainer = childContainer;
            invalidate();
        }

        public void changePanel(LayoutContainer childContainer, float animDuration, AnimationCompletedDelegate animationComplete)
        {
            //If we were animating when a new request comes in clear the old animation first.
            if (animating)
            {
                if (this.childContainer != null)
                {
                    this.childContainer.setAlpha(1.0f);
                    this.childContainer.WorkingSize = newSize;
                    this.childContainer.layout();
                }
                finishAnimation();
            }

            currentTime = 0.0f;
            animationLength = animDuration;
            this.animationComplete = animationComplete;

            oldChildContainer = this.childContainer;
            if (oldChildContainer != null)
            {
                oldSize = oldChildContainer.DesiredSize;
            }
            else
            {
                oldSize = new Size2(0.0f, 0.0f);
            }

            this.childContainer = childContainer;
            if (childContainer != null)
            {
                childContainer._setParent(this);
                newSize = childContainer.DesiredSize;
            }
            else
            {
                newSize = new Size2(0.0f, 0.0f);
            }

            sizeDelta = newSize - oldSize;
            subscribeToUpdates();
        }

        public override Size2 DesiredSize
        {
            get 
            {
                return currentSize;
            }
        }

        public void exceededMaxDelta()
        {
            
        }

        public void loopStarting()
        {
            
        }

        public void sendUpdate(Clock clock)
        {
            if (animating)
            {
                currentTime += clock.fSeconds;
                alpha = currentTime / animationLength;
                if (alpha > 1.0f)
                {
                    alpha = 1.0f;

                    finishAnimation();
                    oldChildContainer = null;
                }
                if (childContainer != null && oldChildContainer != null)
                {
                    childContainer.setAlpha(alpha);
                }
                currentSize = new Size2(WorkingSize.Width, oldSize.Height + sizeDelta.Height * alpha);
                invalidate();
            }
        }

        private void finishAnimation()
        {
            //reset the old child
            if (oldChildContainer != null)
            {
                oldChildContainer._setParent(null);
                oldChildContainer.setAlpha(1.0f);
                oldChildContainer.WorkingSize = oldSize;
                oldChildContainer.layout();
            }
            if (animationComplete != null)
            {
                animationComplete(oldChildContainer);
            }
            unsubscribeFromUpdates();
        }

        public override void bringToFront()
        {
            if (childContainer != null)
            {
                childContainer.bringToFront();
            }
        }

        public override bool Visible
        {
            get
            {
                if (childContainer != null)
                {
                    return childContainer.Visible;
                }
                return false;
            }
            set
            {
                if (childContainer != null)
                {
                    childContainer.Visible = value;
                }
            }
        }

        private void subscribeToUpdates()
        {
            animating = true;
            mainTimer.addFixedUpdateListener(this);
        }

        private void unsubscribeFromUpdates()
        {
            animating = false;
            mainTimer.removeFixedUpdateListener(this);
        }
    }
}
