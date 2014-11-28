using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace Medical
{
    /// <summary>
    /// A task positioner that just returns 0, 0. Use this instead of null to avoid
    /// having to check for this everywhere.
    /// </summary>
    public class EmptyTaskPositioner : TaskPositioner
    {
        static EmptyTaskPositioner()
        {
            Instance = new EmptyTaskPositioner();
        }

        public static EmptyTaskPositioner Instance { get; private set; }

        private EmptyTaskPositioner()
        {

        }

        public IntVector2 findGoodWindowPosition(int width, int height)
        {
            return new IntVector2(0, 0);
        }
    }
}
