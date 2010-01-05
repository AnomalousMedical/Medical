using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Engine.ObjectManagement;

namespace Medical
{
    public class Mandible : Interface
    {
        protected override void constructed()
        {
            MandibleController.setMandible(this);
        }

        protected override void destroy()
        {
            MandibleController.clearMandible(this);
        }

        public AnimationManipulatorState createMandibleState()
        {
            AnimationManipulatorState state = new AnimationManipulatorState();
            foreach (SimElement element in Owner.getElementIter())
            {
                AnimationManipulator manipulator = element as AnimationManipulator;
                if (manipulator != null)
                {
                    state.addPosition(manipulator.createStateEntry());
                }
            }
            return state;
        }
    }
}
