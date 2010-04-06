using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    public class BottomLipSection : LipSection
    {
        protected override void link()
        {
            LipController.addBottomCollisionSection(this);
        }

        protected override void destroy()
        {
            LipController.removeBottomCollisionSection(this);
        }
    }
}
