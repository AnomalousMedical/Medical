using Engine;
using Engine.Attributes;
using Engine.Editing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine.Renderer;

namespace Medical
{
    public class DynamicSplint : BehaviorInterface
    {
        private String multiPropName;

        [DoNotCopy]
        [DoNotSave]
        private MultiProp multiProp;

        protected override void constructed()
        {
            this.addToDebugDrawing();

            multiProp = Owner.getElement(multiPropName) as MultiProp;

            if (multiProp == null)
            {
                blacklist("Cannot find MultiProp '{0}'", multiPropName);
            }

            using (var stream = VirtualFileSystem.Instance.openStream("Plugins/SplintProps/PartModels/SplintSpace.positions", Engine.Resources.FileMode.Open))
            {
                PositionCollection positions = new PositionCollection(stream);
                
                for(int i = StartIndex; i < EndIndex; ++i)
                {
                    var position = positions.getPosition(String.Format("BaseSplint{0}", i));
                    multiProp.addSection(new MultiPropSection(String.Format("SplintSection{0}", i), String.Format("BaseSplint{0}.mesh", i), String.Format("BaseSplintCol{0}", i), position.Translation, position.Rotation, Vector3.ScaleIdentity));
                }
                
                //for (int i = 17; i < 33; ++i)
                //{
                //    var position = positions.getPosition(String.Format("BaseSplint{0}", i));
                //    multiProp.addSection(new MultiPropSection(String.Format("SplintSection{0}", i), String.Format("BaseSplint{0}.mesh", i), String.Format("BaseSplintCol{0}", i), position.Translation, position.Rotation, Vector3.ScaleIdentity));
                //}
            }

            base.constructed();
        }

        [Editable]
        public String MultiPropName
        {
            get
            {
                return multiPropName;
            }
            set
            {
                multiPropName = value;
            }
        }

        public int StartIndex { get; set; }

        public int EndIndex { get; set; }

        public override void drawDebugInfo(DebugDrawingSurface debugDrawing)
        {
            debugDrawing.begin(Owner.Name, DrawingType.LineList);
            debugDrawing.drawAxes(Owner.Translation, Vector3.UnitX, Vector3.UnitY, Vector3.UnitZ, 3);
            debugDrawing.end();

            base.drawDebugInfo(debugDrawing);
        }
    }
}
