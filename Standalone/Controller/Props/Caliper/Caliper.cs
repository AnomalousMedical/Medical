using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.ObjectManagement;
using OgrePlugin;
using Engine;
using OgreWrapper;

namespace Medical
{
    public class Caliper : Interface
    {
        public const String DefinitionName = "Caliper";

        public static void createPropDefinition(PropFactory propFactory)
        {
            GenericSimObjectDefinition pen = new GenericSimObjectDefinition(DefinitionName);
            pen.Enabled = true;

            EntityDefinition entityDefinition = new EntityDefinition(PropFactory.EntityName);
            entityDefinition.MeshName = "Caliper.mesh";

            SceneNodeDefinition nodeDefinition = new SceneNodeDefinition(PropFactory.NodeName);
            nodeDefinition.addMovableObjectDefinition(entityDefinition);
            pen.addElement(nodeDefinition);

            PropFadeBehavior propFadeBehavior = new PropFadeBehavior();
            BehaviorDefinition propFadeBehaviorDef = new BehaviorDefinition(PropFactory.FadeBehaviorName, propFadeBehavior);
            pen.addElement(propFadeBehaviorDef);

            propFactory.addDefinition(DefinitionName, pen);
        }

        private Entity entity;
        private SubEntity subEntity;
        private Color color = Color.Red;

        private Bone leverBone;
        private Bone sliderBone;

        protected override void constructed()
        {
            base.constructed();

            SceneNodeElement sceneNode = Owner.getElement(PropFactory.NodeName) as SceneNodeElement;
            if (sceneNode == null)
            {
                blacklist("Could not find SceneNode {0} in Caliper SimObject", PropFactory.NodeName);
            }
            
            entity = sceneNode.getNodeObject(PropFactory.EntityName) as Entity;
            if (entity == null)
            {
                blacklist("Could not find Entity {0} in Caliper SimObject", PropFactory.EntityName);
            }
            
            subEntity = entity.getSubEntity(0);
            if (subEntity == null)
            {
                blacklist("Could not find sub entity with index 0 in Caliper SimObject");
            }
            
            Skeleton skeleton = entity.getSkeleton();
            if (skeleton == null)
            {
                blacklist("Could not find skeleton for Caliper SimObject");
            }

            leverBone = skeleton.getBone("BCalLever");
            if (leverBone == null)
            {
                blacklist("Could not find 'BCalLever' for Caliper SimObject");
            }
            leverBone.setManuallyControlled(true);

            sliderBone = skeleton.getBone("BCalSlider");
            if (sliderBone == null)
            {
                blacklist("Could not find 'BCalSlider' for Caliper SimObject");
            }
            sliderBone.setManuallyControlled(true);
        }
    }
}
