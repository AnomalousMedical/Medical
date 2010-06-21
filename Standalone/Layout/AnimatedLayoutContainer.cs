using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Engine.Platform;

namespace Medical
{
    public class AnimatedLayoutContainer : ScreenLayoutContainer, UpdateListener
    {
        private UpdateTimer mainTimer;
        private ScreenLayoutContainer childContainer;

        private Vector2 animatedLocation;
        //private Size animatedSize = new Size();
        private float animationLength;
        private float currentTime;
        private Size fullSize;
        private bool animating = false;

        public AnimatedLayoutContainer(UpdateTimer mainTimer)
        {
            this.mainTimer = mainTimer;
            mainTimer.addFixedUpdateListener(this);
        }

        public override void layout()
        {
            if (childContainer != null)
            {
                childContainer.Location = animatedLocation;
                childContainer.WorkingSize = new Size(fullSize.Width, WorkingSize.Height);
                childContainer.layout();
            }
        }

        public void slideOutContainerLeft(ScreenLayoutContainer childContainer, float slideDuration)
        {
            this.childContainer = childContainer;
            childContainer._setParent(this);
            currentTime = 0.0f;
            animationLength = slideDuration;
            fullSize = childContainer.DesiredSize;
            animatedLocation = new Vector2(Location.x - fullSize.Width, Location.y);
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
                }
                animatedLocation = new Vector2(Location.x - fullSize.Width * ((animationLength - currentTime) / animationLength), Location.y);
                invalidate();
            }
        }
    }
}
