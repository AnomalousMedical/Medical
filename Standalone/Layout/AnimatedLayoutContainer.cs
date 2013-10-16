using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    public abstract class AnimatedLayoutContainer : LayoutContainer, IDisposable
    {
        public AnimatedLayoutContainer()
        {
            Rigid = false;
        }

        public abstract void Dispose();

        public abstract void changePanel(LayoutContainer childContainer, float animDuration, AnimationCompletedDelegate animationComplete);

        public abstract LayoutContainer CurrentContainer { get; }
    }
}
