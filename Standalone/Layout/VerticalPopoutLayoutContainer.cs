using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using Engine;

namespace Medical
{
    class VerticalPopoutLayoutContainer : AnimatedLayoutContainer, UpdateListener
    {
        private UpdateTimer mainTimer;
        private LayoutContainer childContainer;
        private LayoutContainer oldChildContainer;

        private event AnimationCompletedDelegate animationComplete;
        private float animationLength;
        private float currentTime;
        private bool animating = false;
        private float alpha = 1.0f;
        private IntSize2 oldSize;
        private IntSize2 newSize;
        private IntSize2 sizeDelta;
        private IntSize2 currentSize;

        public VerticalPopoutLayoutContainer(UpdateTimer mainTimer)
        {
            this.mainTimer = mainTimer;
        }

        public override void Dispose()
        {
            if (animating)
            {
                finishAnimation();
            }
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

        public override void changePanel(LayoutContainer childContainer, float animDuration, AnimationCompletedDelegate animationComplete)
        {
            //If we were animating when a new request comes in clear the old animation first.
            if (animating)
            {
                if (this.childContainer != null)
                {
                    this.childContainer.setAlpha(1.0f);
                    this.childContainer.WorkingSize = newSize;
                    this.childContainer.layout();
                    finishAnimation();
                }
                else
                {
                    //If we were transitioning to null, but now there is another container use the child that was being transitioned
                    this.childContainer = oldChildContainer;
                    unsubscribeFromUpdates();
                }
            }

            currentTime = 0.0f;
            animationLength = animDuration;
            this.animationComplete += animationComplete;

            oldChildContainer = this.childContainer;
            if (oldChildContainer != null)
            {
                oldSize = oldChildContainer.DesiredSize;
                oldChildContainer.animatedResizeStarted(oldSize);
            }
            else
            {
                oldSize = new IntSize2(0, 0);
            }

            this.childContainer = childContainer;
            if (childContainer != null)
            {
                childContainer._setParent(this);
                newSize = childContainer.DesiredSize;
                childContainer.animatedResizeStarted(newSize);
            }
            else
            {
                newSize = new IntSize2(0, 0);
            }

            sizeDelta = newSize - oldSize;
            subscribeToUpdates();
        }

        public override IntSize2 DesiredSize
        {
            get
            {
                if (animating)
                {
                    return currentSize;
                }
                else
                {
                    if (childContainer != null)
                    {
                        return childContainer.DesiredSize;
                    }
                    else
                    {
                        return new IntSize2();
                    }
                }
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
                if (currentTime < animationLength)
                {
                    alpha = EasingFunctions.EaseOutQuadratic(0, 1.0f, currentTime, animationLength);
                }
                else
                {
                    currentTime = animationLength;
                    alpha = 1.0f;

                    finishAnimation();
                    oldChildContainer = null;
                }
                if (childContainer != null && oldChildContainer != null)
                {
                    childContainer.setAlpha(alpha);
                }
                currentSize = new IntSize2(WorkingSize.Width, (int)(oldSize.Height + sizeDelta.Height * alpha));
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
                oldChildContainer.animatedResizeCompleted();
                oldChildContainer.layout();
            }
            if (animationComplete != null)
            {
                animationComplete.Invoke(oldChildContainer);
                animationComplete = null;
            }
            if (childContainer != null)
            {
                childContainer.animatedResizeCompleted();
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

        public override LayoutContainer CurrentContainer
        {
            get
            {
                return childContainer;
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
