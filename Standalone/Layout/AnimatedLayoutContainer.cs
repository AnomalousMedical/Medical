using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Engine.Platform;

namespace Medical
{
    public delegate void AnimationCompletedDelegate(ScreenLayoutContainer oldChild);

    public class AnimatedLayoutContainer : ScreenLayoutContainer, UpdateListener
    {
        private UpdateTimer mainTimer;
        private ScreenLayoutContainer childContainer;
        private ScreenLayoutContainer oldChildContainer;

        private Vector2 animatedLocation;
        private AnimationCompletedDelegate animationComplete;
        private float animationLength;
        private float currentTime;
        private Size fullSize;
        private bool animating = false;
        private float alpha = 1.0f;

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
                childContainer.Location = animatedLocation;
                childContainer.WorkingSize = new Size(fullSize.Width, WorkingSize.Height);
                childContainer.layout();
            }
            if (oldChildContainer != null)
            {
                oldChildContainer.Location = animatedLocation;
                oldChildContainer.WorkingSize = new Size(fullSize.Width, WorkingSize.Height);
                oldChildContainer.layout();
            }
        }

        public void changePanel(ScreenLayoutContainer childContainer, float animDuration, AnimationCompletedDelegate animationComplete)
        {
            currentTime = 0.0f;
            animationLength = animDuration;
            this.animationComplete = animationComplete;

            oldChildContainer = this.childContainer;
            this.childContainer = childContainer;
            if (childContainer != null)
            {
                childContainer._setParent(this);
                fullSize = childContainer.DesiredSize;
                animatedLocation = new Vector2(Location.x - fullSize.Width, Location.y);
            }
            animating = true;
        }

        public override Size DesiredSize
        {
            get 
            {
                return new Size(fullSize.Width + animatedLocation.x, fullSize.Height);
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
                    if (animationComplete != null)
                    {
                        animationComplete(oldChildContainer);
                    }
                    oldChildContainer = null;
                }
                alpha = ((animationLength - currentTime) / animationLength);
                if (oldChildContainer != null && childContainer != null)
                {
                    oldChildContainer.setAlpha(1.0f - alpha);
                    childContainer.setAlpha(alpha);
                }
                else
                {
                    if (childContainer != null)
                    {
                        animatedLocation = new Vector2(Location.x - fullSize.Width * alpha, Location.y);
                    }
                    else
                    {
                        animatedLocation = new Vector2(Location.x - fullSize.Width * (1.0f - alpha), Location.y);
                    }
                }
                invalidate();
            }
        }
    }
}
