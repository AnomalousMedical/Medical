using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.ObjectManagement;

namespace Medical
{
    public class MandibleController
    {
        static Mandible mandible = null;

        internal static void setMandible(Mandible mandibleInstance)
        {
            if (mandible == null)
            {
                mandible = mandibleInstance;
            }
            else
            {
                throw new Exception("Only one mandible can exist per scene.");
            }
        }

        internal static void clearMandible(Mandible mandibleInstance)
        {
            if (mandible == mandibleInstance)
            {
                mandible = null;
            }
        }

        public static AnimationManipulatorState createMandibleState()
        {
            if (mandible != null)
            {
                return mandible.createMandibleState();
            }
            else
            {
                return null;
            }
        }

        public static Mandible Mandible
        {
            get
            {
                return mandible;
            }
        }
    }
}
