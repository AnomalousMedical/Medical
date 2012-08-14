using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Engine.Editing;

namespace Medical.Controller.AnomalousMvc
{
    public class TrueFalseData : DataModelItem
    {
        public TrueFalseData(String name)
            :base(name)
        {

        }

        [Editable]
        public bool Value { get; set; }

        public override string StringValue
        {
            get
            {
                return Value.ToString();
            }
            set
            {
                bool result;
                if (bool.TryParse(value, out result))
                {
                    Value = result;
                }
            }
        }

        protected TrueFalseData(LoadInfo info)
            :base(info)
        {

        }
    }
}
