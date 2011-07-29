using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.ObjectManagement;
using Engine;

namespace Medical
{
    public class MandibleController
    {
        static Mandible mandible = null;

        /// <summary>
        /// Block constructor.
        /// </summary>
        private MandibleController()
        {

        }

        /// <summary>
        /// Set the mandible. Only one can be created per scene. If more than one exists an exception will be thrown.
        /// </summary>
        /// <param name="mandibleInstance">The mandible instance to use.</param>
        internal static void setMandible(Mandible mandibleInstance)
        {
            if (mandible == null)
            {
                mandible = mandibleInstance;
                StartTranslation = mandible.Owner.Translation;
                StartRotation = mandible.Owner.Rotation;
            }
            else
            {
                throw new Exception("Only one mandible can exist per scene.");
            }
        }

        /// <summary>
        /// Clear the mandible.
        /// </summary>
        /// <param name="mandibleInstance">The mandible instance calling the clear function.</param>
        internal static void clearMandible(Mandible mandibleInstance)
        {
            if (mandible == mandibleInstance)
            {
                mandible = null;
            }
        }

        /// <summary>
        /// Create a state for the mandible.
        /// </summary>
        /// <returns>A new state for the mandible.</returns>
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

        /// <summary>
        /// Get the current mandible for the scene.
        /// </summary>
        public static Mandible Mandible
        {
            get
            {
                return mandible;
            }
        }

        public static Vector3 StartTranslation { get; set; }

        public static Quaternion StartRotation { get; set; }
    }
}
