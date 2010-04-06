using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    public class TopLipSection : LipSection
    {
        protected override void link()
        {
            LipController.addTopCollisionSection(this);
        }

        protected override void destroy()
        {
            LipController.removeTopCollisionSection(this);
        }
    }
}
