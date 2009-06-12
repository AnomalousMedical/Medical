using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OgrePlugin;
using Engine.Editing;
using OgreWrapper;
using Engine.ObjectManagement;
using Engine.Platform;
using Engine;
using Engine.Saving;
using Engine.Attributes;

namespace Medical
{
    class InteractiveCurve : Behavior
    {
        [Editable]
        String targetSimObject;

        [Editable]
        String targetNode;

        [Editable]
        String targetManualObject;

        [Editable]
        uint lineDetail = 50;

        [Editable]
        Color color = Color.White;

        ManualObject manualObject;
        SimObject targetObject;
        NaturalSplineSet curve = new NaturalSplineSet();

        public void addControlPoint(Vector3 controlPoint)
        {
            curve.addControlPoint(controlPoint);
        }

        public int getNumControlPoints()
        {
            return curve.getNumControlPoints();
        }

        public void recomputeCurve()
        {
            curve.computeSplines();
            //manualObject.clear();
            //manualObject.begin("colorVertexNoDepth", OperationType.OT_LINE_STRIP);
            //manualObject.position(curve.interpolate(0.0f));
            //manualObject.color(color.r, color.g, color.b, color.a);
            //for (uint i = 1; i <= lineDetail; ++i)
            //{
            //    manualObject.position(curve.interpolate(i / (float)lineDetail));
            //    manualObject.color(color.r, color.g, color.b, color.a);
            //}
            //manualObject.end();
        }

        public Vector3 interpolate(float percentage)
        {
            return curve.interpolate(percentage);
        }

        protected override void constructed()
        {
            targetObject = Owner.getOtherSimObject(targetSimObject);
            if (targetObject != null)
            {
                SceneNodeElement node = targetObject.getElement(targetNode) as SceneNodeElement;
                if (node != null)
                {
                    manualObject = node.getNodeObject(targetManualObject) as ManualObject;
                    if (manualObject == null)
                    {
                        blacklist("Could not find target ManualObject {0}.", targetManualObject);
                    }
                    else
                    {
                        //Initialize the manual object so it can be easily updated.
                        //manualObject.begin("colorVertexNoDepth", OperationType.OT_LINE_STRIP);
                        //manualObject.estimateVertexCount(lineDetail);
                        //manualObject.position(ref Vector3.Zero);
                        //manualObject.position(ref Vector3.Zero);
                        //manualObject.end();
                    }
                }
                else
                {
                    blacklist("Could not find target SceneNodeElement {0}.", targetNode);
                }
            }
            else
            {
                blacklist("Could not find Target SimObject {0}.", targetSimObject);
            }
        }

        public override void update(Clock clock, EventManager eventManager)
        {
            
        }

        private const String CTRL_PT_BASE = "CtrlPt";

        protected override void customSave(SaveInfo info)
        {
            int numCtrlPts = curve.getNumControlPoints();
            for (int i = 0; i < numCtrlPts; ++i)
            {
                info.AddValue(CTRL_PT_BASE + i, curve.getControlPoint(i));
            }
        }

        protected override void customLoad(LoadInfo info)
        {
            for (int i = 0; info.hasValue(CTRL_PT_BASE + i); ++i )
            {
                curve.addControlPoint(info.GetVector3(CTRL_PT_BASE + i));
            }
            curve.computeSplines();
        }
    }
}
