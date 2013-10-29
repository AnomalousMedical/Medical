using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    public interface TransparencyChanger
    {
        void smoothBlend(float alpha, float duration, EasingFunction easingFunction);

        float CurrentAlpha { get; set; }
    }
}
