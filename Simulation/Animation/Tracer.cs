using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Engine.Platform;
using OgreWrapper;
using Engine.Editing;
using OgrePlugin;

namespace Medical.Animation
{
    class Tracer : Behavior
    {
        [Editable]
        private String sceneNodeName;

        [Editable]
        private String manualObjectName;

        private ManualObject manualObject;
        private SceneNodeElement node;

        protected override void constructed()
        {
            base.constructed();
            node = Owner.getElement(sceneNodeName) as SceneNodeElement;
            if (node == null)
            {
                blacklist("Could not find scene node named {0}.", sceneNodeName);
            }
            manualObject = node.getNodeObject(manualObjectName) as ManualObject;
            if (manualObject == null)
            {
                blacklist("Could not find manual object named {0}.", manualObjectName);
            }
        }

        public override void update(Clock clock, EventManager eventManager)
        {
            
        }
    }
}
