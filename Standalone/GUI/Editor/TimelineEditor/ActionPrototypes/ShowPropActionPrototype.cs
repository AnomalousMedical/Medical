using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace Medical.GUI
{
    class ShowPropActionPrototype : TimelineActionPrototype
    {
        private PropEditController propEditController;

        public ShowPropActionPrototype(Color normalColor, Color selectedColor, PropEditController propEditController)
            : base("Show Prop", typeof(ShowPropAction), normalColor, selectedColor)
        {
            this.propEditController = propEditController;
        }

        public override TimelineActionData createData(TimelineAction action)
        {
            return new ShowPropActionData((ShowPropAction)action, propEditController);
        }
    }
}
