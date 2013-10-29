using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    class NullTransparencyChanger : TransparencyChanger
    {
        private static NullTransparencyChanger instance;

        public static NullTransparencyChanger Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new NullTransparencyChanger();
                }
                return instance;
            }
        }

        private NullTransparencyChanger()
        {

        }

        public void smoothBlend(float alpha, float duration, EasingFunction easingFunction)
        {
            
        }

        public float CurrentAlpha
        {
            get
            {
                return 0.0f;
            }
            set
            {
                
            }
        }
    }
}
