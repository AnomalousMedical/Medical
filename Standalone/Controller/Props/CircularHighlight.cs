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
    public class CircularHighlight : Interface
    {
        public const String DefinitionName = "CircularHighlight";
        public const String BehaviorName = "Behavior";

        public static void createPropDefinition(PropFactory propFactory)
        {
            GenericSimObjectDefinition simObject = new GenericSimObjectDefinition("CircularHighlightPrototype");
            simObject.Enabled = true;
            ManualObjectDefinition manualObjectDefinition = new ManualObjectDefinition("ManualObject");
            SceneNodeDefinition nodeDefinition = new SceneNodeDefinition(PropFactory.NodeName);
            nodeDefinition.addMovableObjectDefinition(manualObjectDefinition);
            simObject.addElement(nodeDefinition);
            CircularHighlight behavior = new CircularHighlight();
            BehaviorDefinition behaviorDef = new BehaviorDefinition(BehaviorName, behavior);
            simObject.addElement(behaviorDef);
            //PropFadeBehavior propFadeBehavior = new PropFadeBehavior();
            //BehaviorDefinition propFadeBehaviorDef = new BehaviorDefinition(PropFactory.FadeBehaviorName, propFadeBehavior);
            //simObject.addElement(propFadeBehaviorDef);
            propFactory.addDefinition(DefinitionName, simObject);
        }

        private Ellipse innerEllipse;
        private Ellipse outerEllipse;
        private ManualObject manualObject;
        private int numSections = 36;
        private Color color = Color.Red;

        public CircularHighlight()
        {
            innerEllipse = new Ellipse();
            outerEllipse = new Ellipse();
            outerEllipse.MajorAxis = 2.0f;
            outerEllipse.MinorAxis = 2.0f;
        }

        protected override void constructed()
        {
            base.constructed();
            SceneNodeElement sceneNode = Owner.getElement(PropFactory.NodeName) as SceneNodeElement;
            if (sceneNode == null)
            {
                blacklist("Cannot find scene node {0} in CircularHighlight.", PropFactory.NodeName);
            }
            manualObject = sceneNode.getNodeObject("ManualObject") as ManualObject;
            if (manualObject == null)
            {
                blacklist("Cannot find 'ManualObject' in node {0}.", PropFactory.NodeName);
            }
            //Initialize the manual object so it can be easily updated.
            manualObject.begin("colorvertex", OperationType.OT_LINE_STRIP);
            manualObject.estimateVertexCount(128);
            manualObject.position(ref Vector3.Zero);
            manualObject.position(ref Vector3.Zero);
            manualObject.end();
            createEllipse();
        }

        public void createEllipse()
        {
            manualObject.clear();
            manualObject.begin("colorvertex", OperationType.OT_LINE_STRIP);
            for (float i = 0; i <= 6.28; i += (6.28f / numSections))
            {
                Vector2 ellipsePosition = innerEllipse.getPoint(i);
                manualObject.position(new Vector3(ellipsePosition.x, ellipsePosition.y, 0));
                manualObject.color(color.r, color.g, color.b, color.a);
            }
            manualObject.end();
        }

        public Radian Theta
        {
            get
            {
                return innerEllipse.Theta;
            }
            set
            {
                innerEllipse.Theta = value;
            }
        }

        public float MajorAxis
        {
            get
            {
                return innerEllipse.MajorAxis;
            }
            set
            {
                innerEllipse.MajorAxis = value;
            }
        }

        public float MinorAxis
        {
            get
            {
                return innerEllipse.MinorAxis;
            }
            set
            {
                innerEllipse.MinorAxis = value;
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
