using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Engine.Attributes;
using Engine.Editing;
using Engine.ObjectManagement;
using Engine.Platform;

namespace Medical
{
    public delegate void MovingMuscleTargetOffsetChanged(MovingMuscleTarget source, Vector3 offset);

    public class MovingMuscleTarget : Interface
    {
        [DoNotSave]
        [DoNotCopy]
        public event MovingMuscleTargetOffsetChanged OffsetChanged;

        private Vector3 startingPosition;

        protected override void link()
        {
            MuscleController.MovingTarget = this;
            startingPosition = Owner.Translation;
        }

        protected override void destroy()
        {
            MuscleController.MovingTarget = null;
        }

        [DoNotCopy]
        public Vector3 Offset
        {
            get
            {
                return Owner.Translation - startingPosition;
            }
            set
            {
                Vector3 newPos = startingPosition + value;
                updateTranslation(ref newPos);
                if (OffsetChanged != null)
                {
                    OffsetChanged.Invoke(this, value);
                }
            }
        }
    }
}
