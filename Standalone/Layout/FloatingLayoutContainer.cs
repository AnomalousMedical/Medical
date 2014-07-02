using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;

namespace Medical
{
    class FloatingLayoutContainer : UpdateListener
    {
        private UpdateTimer mainTimer;
        private LayoutContainer childLayout;
        private bool showing = true;
        private float animationDuration;
        private float currentTime;
        private AnimationCompletedDelegate animationComplete;

        public FloatingLayoutContainer(UpdateTimer mainTimer, LayoutContainer childLayout, float animationDuration, AnimationCompletedDelegate animationComplete)
        {
            this.mainTimer = mainTimer;
            this.childLayout = childLayout;
            this.animationDuration = animationDuration;
            this.animationComplete = animationComplete;
        }

        public void show()
        {
            showing = true;
            currentTime = 0.0f;
            subscribeToUpdates();
        }

        public void hide()
        {
            showing = false;
            currentTime = 0.0f;
            subscribeToUpdates();
        }

        public void sendUpdate(Clock clock)
        {
            float blendPercent = currentTime / animationDuration;
            if (showing)
            {
                if (blendPercent > 1.0f)
                {
                    childLayout.setAlpha(1.0f);
                    unsubscribeFromUpdates();
                }
                else
                {
                    childLayout.setAlpha(blendPercent);
                }
            }
            else
            {
                if (blendPercent > 1.0f)
                {
                    childLayout.setAlpha(0.0f);
                    unsubscribeFromUpdates();
                    if (animationComplete != null)
                    {
                        animationComplete.Invoke(null);
                    }
                }
                childLayout.setAlpha(1.0f - blendPercent);
            }
        }

        public void exceededMaxDelta()
        {
            
        }

        public void loopStarting()
        {
            
        }

        private void subscribeToUpdates()
        {
            mainTimer.addUpdateListener(this);
        }

        private void unsubscribeFromUpdates()
        {
            mainTimer.removeUpdateListener(this);
        }
    }
}
