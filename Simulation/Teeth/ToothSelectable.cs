using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace Medical
{
    class ToothSelectable : SelectableObject
    {
        private Tooth tooth;

        public ToothSelectable(Tooth tooth)
        {
            this.tooth = tooth;
        }

        public void editPosition(ref Vector3 translation, ref Quaternion rotation)
        {
            //Must convert back to an offset
            Vector3 newPos = translation - tooth.ToolTranslation;
            tooth.move(newPos);
            tooth.rotate(ref rotation);
        }

        public void editTranslation(ref Vector3 translation)
        {
            //Must convert back to an offset
            Vector3 newPos = translation - tooth.ToolTranslation;
            tooth.move(newPos);
        }

        public void editRotation(ref Quaternion rotation)
        {
            tooth.rotate(ref rotation);
        }

        public Quaternion getRotation()
        {
            return tooth.ToolRotation;
        }

        public Vector3 getTranslation()
        {
            return tooth.ToolTranslation;
        }
    }
}
