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
            ManualObjectDefinition manualObjectDefinition = new ManualObjectDefinition(PropFactory.ManualObjectName);
            SceneNodeDefinition nodeDefinition = new SceneNodeDefinition(PropFactory.NodeName);
            nodeDefinition.addMovableObjectDefinition(manualObjectDefinition);
            simObject.addElement(nodeDefinition);
            CircularHighlight behavior = new CircularHighlight();
            BehaviorDefinition behaviorDef = new BehaviorDefinition(BehaviorName, behavior);
            simObject.addElement(behaviorDef);
            PropFadeBehavior propFadeBehavior = new ManualObjectPropFadeBehavior();
            BehaviorDefinition propFadeBehaviorDef = new BehaviorDefinition(PropFactory.FadeBehaviorName, propFadeBehavior);
            simObject.addElement(propFadeBehaviorDef);
            propFactory.addDefinition(DefinitionName, simObject);
        }

        private Ellipse innerEllipse;
        private Ellipse outerEllipse;
        private ManualObject manualObject;
        private int numSections = 36;
        private Color color = Color.Red;
        private float thickness = 0.2f;

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
            createEllipse();
        }

        public void createEllipse()
        {
            manualObject.clear();
            manualObject.begin("ManualTwoSidedPropShapeMaterial", OperationType.OT_TRIANGLE_STRIP);
            uint vertexCount = (uint)(numSections * 2);
            uint indexCount = vertexCount + 2;
            manualObject.estimateVertexCount(vertexCount);
            manualObject.estimateIndexCount(indexCount);
            float increment = 6.28f / numSections;
            for (uint i = 0; i < numSections; ++i)
            {
                float t = i * increment;
                Vector2 ellipsePosition = innerEllipse.getPoint(t);
                manualObject.position(new Vector3(ellipsePosition.x, ellipsePosition.y, 0));
                manualObject.color(color.r, color.g, color.b, color.a);
                ellipsePosition = outerEllipse.getPoint(t);
                manualObject.position(new Vector3(ellipsePosition.x, ellipsePosition.y, 0));
                manualObject.color(color.r, color.g, color.b, color.a);

                manualObject.index(i * 2);
                manualObject.index(i * 2 + 1);
            }
            //Close the ellipse
            manualObject.index(0);
            manualObject.index(1);
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
                outerEllipse.Theta = value;
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
                outerEllipse.MajorAxis = value + thickness;
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
                outerEllipse.MinorAxis = value + thickness;
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

        public float Thickness
        {
            get
            {
                return thickness;
            }
            set
            {
                thickness = value;
                outerEllipse.MajorAxis = innerEllipse.MajorAxis + thickness;
                outerEllipse.MinorAxis = innerEllipse.MinorAxis + thickness;
            }
        }
    }
}
