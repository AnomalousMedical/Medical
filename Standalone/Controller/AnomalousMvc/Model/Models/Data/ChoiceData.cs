using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Engine.Editing;

namespace Medical.Controller.AnomalousMvc
{
    public class ChoiceData : DataModelItem
    {
        public ChoiceData(String name)
            :base(name)
        {

        }

        [Editable]
        public String Value { get; set; }

        public override string StringValue
        {
            get
            {
                return Value;
            }
            set
            {
                Value = value;
            }
        }

        protected ChoiceData(LoadInfo info)
            :base(info)
        {

        }
    }
}
