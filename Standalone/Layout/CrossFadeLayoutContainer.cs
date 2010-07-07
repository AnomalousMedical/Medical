using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using Engine;

namespace Medical
{
    class CrossFadeLayoutContainer : LayoutContainer, UpdateListener
    {
        private UpdateTimer mainTimer;
        private LayoutContainer childContainer;
        private LayoutContainer oldChildContainer;

        private AnimationCompletedDelegate animationComplete;
        private float animationLength;
        private float currentTime;
        private bool animating = false;
        private float alpha = 1.0f;

        public CrossFadeLayoutContainer(UpdateTimer mainTimer)
        {
            this.mainTimer = mainTimer;
            mainTimer.addFixedUpdateListener(this);
        }

        public override void setAlpha(float alpha)
        {

        }

        public override void layout()
        {
            if (childContainer != null)
            {
                childContainer.Location = Location;
                childContainer.WorkingSize = WorkingSize;
                childContainer.layout();
            }
            if (oldChildContainer != null)
            {
                oldChildContainer.Location = Location;
                oldChildContainer.WorkingSize = WorkingSize;
                oldChildContainer.layout();
            }
        }

        public void changePanel(LayoutContainer childContainer, float animDuration, AnimationCompletedDelegate animationComplete)
        {
            //If we were animating when a new request comes in clear the old animation first.
            if (animating)
            {
                if (this.childContainer != null)
                {
                    this.childContainer.setAlpha(1.0f);
                }
                finishAnimation();
            }

            currentTime = 0.0f;
            animationLength = animDuration;
            this.animationComplete = animationComplete;

            oldChildContainer = this.childContainer;
            
            this.childContainer = childContainer;
            if (childContainer != null)
            {
                childContainer._setParent(this);
            }
            
            animating = true;
        }

        public override Size2 DesiredSize
        {
            get 
            {
                if (childContainer != null)
                {
                    return childContainer.DesiredSize;
                }
                return new Size2(0.0f, 0.0f);
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
                bool finishAnimatingThisFrame = false;
                currentTime += clock.fSeconds;
                if (currentTime > animationLength)
                {
                    currentTime = animationLength;
                    finishAnimatingThisFrame = true;

                    finishAnimation();
                    oldChildContainer = null;
                }
                alpha = currentTime / animationLength;
                if (childContainer != null && oldChildContainer != null)
                {
                    childContainer.setAlpha(alpha);
                }
                invalidate();
                if (finishAnimatingThisFrame)
                {
                    animating = false;
                }
            }
        }

        private void finishAnimation()
        {
            //reset the old child
            if (oldChildContainer != null)
            {
                oldChildContainer._setParent(null);
                oldChildContainer.setAlpha(1.0f);
                oldChildContainer.layout();
            }
            if (animationComplete != null)
            {
                animationComplete(oldChildContainer);
            }
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
    }
}
