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

        private float animationLength;
        private float currentTime;
        private bool animating = false;
        private float alpha = 1.0f;
        private IntSize2 oldSize;
        private IntSize2 newSize;
        private IntSize2 sizeDelta;
        private IntSize2 currentSize;
        private EasingFunction currentEasing = EasingFunction.None;

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

        public override void changePanel(LayoutContainer childContainer, float animDuration)
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

            oldChildContainer = this.childContainer;
            if (oldChildContainer != null)
            {
                oldSize = oldChildContainer.DesiredSize;
                oldChildContainer.animatedResizeStarted(new IntSize2(WorkingSize.Width, oldSize.Height));
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
                childContainer.animatedResizeStarted(new IntSize2(WorkingSize.Width, newSize.Height));
                //Force the child container to fit in the current alloted space
                childContainer.Location = Location;
                childContainer.WorkingSize = new IntSize2(WorkingSize.Width, oldSize.Height);
                childContainer.layout();
            }
            else
            {
                newSize = new IntSize2(0, 0);
            }

            sizeDelta = newSize - oldSize;

            if (oldSize.Height == 0)
            {
                currentEasing = EasingFunction.EaseOutQuadratic;
            }
            else if (newSize.Height == 0)
            {
                currentEasing = EasingFunction.EaseInQuadratic;
            }
            else
            {
                currentEasing = EasingFunction.EaseInOutQuadratic;
            }

            //Make sure we start with no alpha if blending
            if (childContainer != null && oldChildContainer != null)
            {
                childContainer.setAlpha(0.0f);
            }

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
                currentTime += clock.DeltaSeconds;
                if (currentTime < animationLength)
                {
                    alpha = EasingFunctions.Ease(currentEasing, 0, 1.0f, currentTime, animationLength);
                    currentSize = new IntSize2(WorkingSize.Width, (int)(oldSize.Height + sizeDelta.Height * alpha));
                }
                else
                {
                    currentTime = animationLength;
                    alpha = 1.0f;
                    currentSize = new IntSize2(WorkingSize.Width, oldSize.Height + sizeDelta.Height);

                    finishAnimation();
                    oldChildContainer = null;
                }
                if (childContainer != null && oldChildContainer != null)
                {
                    childContainer.setAlpha(alpha);
                }
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
                oldChildContainer.animatedResizeCompleted(oldSize);
                oldChildContainer.layout();
            }
            fireAnimationComplete(oldChildContainer);
            if (childContainer != null)
            {
                childContainer.animatedResizeCompleted(currentSize);
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
            mainTimer.addUpdateListener(this);
        }

        private void unsubscribeFromUpdates()
        {
            animating = false;
            mainTimer.removeUpdateListener(this);
        }
    }
}
