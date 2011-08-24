using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine.Saving;
using Engine.Editing;
using Engine.Attributes;

namespace Medical
{
    class NumericDataField : DataField
    {
        public NumericDataField(String name)
            :base(name)
        {
            AllowDecimalPlaces = true;
            MinValue = 0;
            MaxValue = 100;
            CurrentValue = 0;
        }

        public override DataControl createControl(Widget parentWidget, DataDrivenTimelineGUI gui)
        {
            return new NumericEditDataControl(parentWidget, this);
        }

        [Editable]
        public bool AllowDecimalPlaces { get; set; }

        [Editable]
        public decimal MinValue { get; set; }

        [Editable]
        public decimal MaxValue { get; set; }

        [Editable]
        public decimal CurrentValue { get; set; }

        public override string Type
        {
            get
            {
                return "Numeric";
            }
        }

        protected NumericDataField(LoadInfo info)
            :base(info)
        {

        }
    }
}
