using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Engine.Platform;
using Logging;

namespace Medical
{
    public delegate void AnimationCompletedDelegate(ScreenLayoutContainer oldChild);

    public class AnimatedLayoutContainer : ScreenLayoutContainer, UpdateListener
    {
        private UpdateTimer mainTimer;
        private ScreenLayoutContainer childContainer;
        private ScreenLayoutContainer oldChildContainer;

        private AnimationCompletedDelegate animationComplete;
        private float animationLength;
        private float currentTime;
        private bool animating = false;
        private float alpha = 1.0f;
        private Size oldSize;
        private Size newSize;
        private Size sizeDelta;
        private Size currentSize;

        public AnimatedLayoutContainer(UpdateTimer mainTimer)
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

        public void changePanel(ScreenLayoutContainer childContainer, float animDuration, AnimationCompletedDelegate animationComplete)
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
                oldSize = new Size(0.0f, 0.0f);
            }

            this.childContainer = childContainer;
            if (childContainer != null)
            {
                childContainer._setParent(this);
                newSize = childContainer.DesiredSize;
            }
            else
            {
                newSize = new Size(0.0f, 0.0f);
            }

            sizeDelta = newSize - oldSize;
            animating = true;
        }

        public override Size DesiredSize
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
                if (currentTime > animationLength)
                {
                    currentTime = animationLength;
                    animating = false;

                    finishAnimation();
                    oldChildContainer = null;
                }
                alpha = currentTime / animationLength;
                if (childContainer != null && oldChildContainer != null)
                {
                    childContainer.setAlpha(alpha);
                }
                currentSize = new Size(oldSize.Width + sizeDelta.Width * alpha, WorkingSize.Height);
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
        }
    }
}
