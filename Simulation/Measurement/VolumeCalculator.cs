using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine.Platform;
using Engine.Editing;
using OgrePlugin;

namespace Medical
{
    public class VolumeCalculator : BehaviorInterface
    {
        [Editable]
        private String nodeName = "Node";
        [Editable]
        private String entityName = "Entity";
        [Editable]
        private String name;

        Entity entity;

        protected override void constructed()
        {
            SceneNodeElement sceneNode = Owner.getElement(nodeName) as SceneNodeElement;
            if (sceneNode == null)
            {
                blacklist("Cannot find Node '{0}'", nodeName);
            }

            entity = sceneNode.getNodeObject(entityName) as Entity;

            if (entity == null)
            {
                if (entityName == null)
                {
                    blacklist("entityName is Null");
                }
                else
                {
                    blacklist("Cannot find entity named '{0}' in node '{1}'.", entityName, nodeName);
                }
            }

            if(String.IsNullOrEmpty(name))
            {
                blacklist("Name is empty");
            }

            VolumeController.addVolume(this);

            base.constructed();
        }

        protected override void destroy()
        {
            VolumeController.removeVolume(this);
            base.destroy();
        }

        public float CurrentVolume
        {
            get
            {
                return entity.calculateVolume();
            }
        }

        public String Name
        {
            get
            {
                return name;
            }
        }
    }
}
