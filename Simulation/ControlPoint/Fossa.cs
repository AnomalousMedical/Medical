using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Engine.Platform;
using Engine.Editing;

namespace Medical
{
    class Fossa : Interface
    {
        [Editable]
        String interactiveCurveName;

        InteractiveCurve translation;

        protected override void constructed()
        {
            translation = Owner.getElement(interactiveCurveName) as InteractiveCurve;
            if (translation == null)
            {
                blacklist("Could not find an interactive curve named {0} in SimObject {1}.", interactiveCurveName, Owner.Name);
            }
            else
            {
                if (translation.getNumControlPoints() == 0)
                {
                    translation.addControlPoint(new Vector3(Owner.Translation.x, -5.227f, 0.839f) - Owner.Translation);
                    translation.addControlPoint(new Vector3(Owner.Translation.x, -4.887f, 0.983f) - Owner.Translation);
                    translation.addControlPoint(new Vector3(Owner.Translation.x, -4.755f, 1.087f) - Owner.Translation);
                    translation.addControlPoint(new Vector3(Owner.Translation.x, -4.635f, 1.239f) - Owner.Translation);
                    translation.addControlPoint(new Vector3(Owner.Translation.x, -4.523f, 1.478f) - Owner.Translation);
                    translation.addControlPoint(new Vector3(Owner.Translation.x, -4.517f, 1.595f) - Owner.Translation);
                    translation.addControlPoint(new Vector3(Owner.Translation.x, -4.61f, 1.764f) - Owner.Translation);
                    translation.addControlPoint(new Vector3(Owner.Translation.x, -4.765f, 1.938f) - Owner.Translation);
                    translation.addControlPoint(new Vector3(Owner.Translation.x, -4.947f, 2.125f) - Owner.Translation);
                    translation.addControlPoint(new Vector3(Owner.Translation.x, -5.085f, 2.381f) - Owner.Translation);
                    translation.addControlPoint(new Vector3(Owner.Translation.x, -5.127f, 2.542f) - Owner.Translation);
                    translation.addControlPoint(new Vector3(Owner.Translation.x, -5.028f, 2.989f) - Owner.Translation);
                    translation.addControlPoint(new Vector3(Owner.Translation.x, -4.817f, 3.341f) - Owner.Translation);
                }
            }
        }

        protected override void link()
        {
            if (translation != null)
            {
                translation.recomputeCurve();
            }
        }

        public Vector3 getPosition(float position)
        {
            return Owner.Translation + translation.interpolate(position);
        }
    }
}
