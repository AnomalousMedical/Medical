using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using Engine.Attributes;

namespace Medical
{
    /// <summary>
    /// This class defines a specific state of transparency for a TransparencyInterface.
    /// </summary>
    [DoNotCopy]
    [DoNotSave]
    class TransparencyState
    {
        private float workingAlpha = 1.0f;
        private float targetOpacity = 1.0f;
        private bool changingOpacity = false;
        private float opacityChangeMultiplier = 1.0f;

        public void smoothBlend(float targetOpacity, float changeMultiplier)
        {
            changingOpacity = true;
            this.targetOpacity = targetOpacity;
            this.opacityChangeMultiplier = changeMultiplier;
        }

        public void update(Clock clock)
        {
            if (changingOpacity)
            {
                if (workingAlpha > targetOpacity)
                {
                    workingAlpha -= (float)clock.Seconds * opacityChangeMultiplier;
                    if (workingAlpha < targetOpacity)
                    {
                        workingAlpha = targetOpacity;
                        changingOpacity = false;
                    }
                }
                else
                {
                    workingAlpha += (float)clock.Seconds * opacityChangeMultiplier;
                    if (workingAlpha > targetOpacity)
                    {
                        workingAlpha = targetOpacity;
                        changingOpacity = false;
                    }
                }
            }
        }

        /// <summary>
        /// The alpha value for this interface. Note that if it is currently in
        /// transition the target opacity will be returned and not the actual
        /// current value.
        /// </summary>
        public float CurrentAlpha
        {
            get
            {
                if (changingOpacity)
                {
                    return targetOpacity;
                }
                else
                {
                    return workingAlpha;
                }
            }
            set
            {
                workingAlpha = value;
                changingOpacity = false;
            }
        }

        /// <summary>
        /// Get the value that this state wants the alpha to be. This might be
        /// overwritten by overrideAlpha.
        /// </summary>
        internal float WorkingAlpha
        {
            get
            {
                return workingAlpha;
            }
        }

        /// <summary>
        /// Get the WorkingAlpha value without worrying about the alpha override.
        /// </summary>
        internal float WorkingAlphaOnly
        {
            get
            {
                return workingAlpha;
            }
        }
    }
}
