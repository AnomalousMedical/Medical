using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Engine.ObjectManagement;
using OgrePlugin;
using OgreWrapper;

namespace Medical
{
    public class LineProp : Interface
    {
        public const String DefinitionName = "Line";
        public const String BehaviorName = "Behavior";

        public static void createPropDefinition(PropFactory propFactory)
        {
            GenericSimObjectDefinition simObject = new GenericSimObjectDefinition("LinePrototype");
            simObject.Enabled = true;
            ManualObjectDefinition manualObjectDefinition = new ManualObjectDefinition(PropFactory.ManualObjectName);
            SceneNodeDefinition nodeDefinition = new SceneNodeDefinition(PropFactory.NodeName);
            nodeDefinition.addMovableObjectDefinition(manualObjectDefinition);
            simObject.addElement(nodeDefinition);
            LineProp behavior = new LineProp();
            BehaviorDefinition behaviorDef = new BehaviorDefinition(BehaviorName, behavior);
            simObject.addElement(behaviorDef);
            PropFadeBehavior propFadeBehavior = new ManualObjectPropFadeBehavior();
            BehaviorDefinition propFadeBehaviorDef = new BehaviorDefinition(PropFactory.FadeBehaviorName, propFadeBehavior);
            simObject.addElement(propFadeBehaviorDef);
            propFactory.addDefinition(DefinitionName, simObject);
        }

        private ManualObject manualObject;
        private float length = 6.0f;
        private Color color = Color.Red;

        public LineProp()
        {
            
        }

        protected override void constructed()
        {
            base.constructed();
            SceneNodeElement sceneNode = Owner.getElement(PropFactory.NodeName) as SceneNodeElement;
            if (sceneNode == null)
            {
                blacklist("Cannot find scene node {0} in LineProp.", PropFactory.NodeName);
            }
            manualObject = sceneNode.getNodeObject("ManualObject") as ManualObject;
            if (manualObject == null)
            {
                blacklist("Cannot find 'ManualObject' in node {0}.", PropFactory.NodeName);
            }
            createLine();
        }

        public void createLine()
        {
            manualObject.clear();
            manualObject.begin("ManualTwoSidedPropShapeMaterial", OperationType.OT_LINE_LIST);
            manualObject.position(0, 0, 0);
            manualObject.color(color.r, color.g, color.b, color.a);
            manualObject.position(0, length, 0);
            manualObject.color(color.r, color.g, color.b, color.a);
            manualObject.end();
        }

        public float Length
        {
            get
            {
                return length;
            }
            set
            {
                length = value;
            }
        }

        public Color Color
        {
            get
            {
                return color;
            }
            set
            {
                color = value;
            }
        }
    }
}
