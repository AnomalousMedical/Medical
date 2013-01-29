using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace Medical.GUI
{
    class MovePropPrototype : ShowPropSubActionPrototype
    {
        private PropEditController propEditController;

        public MovePropPrototype(Type type, Color color, String typeName, PropEditController propEditController)
            :base(type, color, typeName)
        {
            this.propEditController = propEditController;
        }

        public override PropTimelineData createData(ShowPropSubAction action)
        {
            return new MovePropTimelineData((MovePropAction)action, propEditController);
        }
    }
}
