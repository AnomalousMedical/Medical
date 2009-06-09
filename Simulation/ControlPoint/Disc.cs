using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Engine.Platform;
using Engine.Editing;
using OgreWrapper;
using Engine.ObjectManagement;
using OgrePlugin;

namespace Medical
{
    class Disc : Interface
    {
        protected override void constructed()
        {
            
        }

        public Vector3 getOffset(float position)
        {
            return Vector3.UnitY * -0.302f;
        }
    }
}
