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
    public class NumericDataField : DataField
    {
        public NumericDataField(String name)
            :base(name)
        {
            AllowDecimalPlaces = true;
            MinValue = 0;
            MaxValue = 100;
            StartingValue = 0;
        }

        public override void createControl(DataControlFactory factory)
        {
            factory.addField(this);
        }

        [Editable]
        public bool AllowDecimalPlaces { get; set; }

        [Editable]
        public decimal MinValue { get; set; }

        [Editable]
        public decimal MaxValue { get; set; }

        [Editable]
        public decimal StartingValue { get; set; }

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
