using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    public class CameraResolver
    {
        static List<CameraMotionValidator> validators = new List<CameraMotionValidator>();

        /// <summary>
        /// Add a motion validator to the resolver.
        /// </summary>
        /// <param name="validator">The validator to add.</param>
        public static void addMotionValidator(CameraMotionValidator validator)
        {
            validators.Add(validator);
        }

        public static void removeMotionValidator(CameraMotionValidator validator)
        {
            validators.Remove(validator);
        }

        public static CameraMotionValidator getValidatorForLocation(int x, int y)
        {
            foreach (CameraMotionValidator validator in validators)
            {
                if (validator.allowMotion(x, y))
                {
                    return validator;
                }
            }
            return null;
        }
    }
}
