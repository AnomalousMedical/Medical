using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine.Editing;
using Engine.Saving;

namespace Medical
{
    class BooleanDataField : DataField
    {
        public BooleanDataField(String name)
            :base(name)
        {
            StartingValue = false;
        }

        public override DataControl createControl(Widget parentWidget, DataDrivenTimelineGUI gui)
        {
            return new CheckBoxDataControl(parentWidget, this);
        }

        [Editable]
        public bool StartingValue { get; set; }

        public override string Type
        {
            get
            {
                return "Boolean";
            }
        }

        protected BooleanDataField(LoadInfo info)
            :base(info)
        {

        }
    }
}
