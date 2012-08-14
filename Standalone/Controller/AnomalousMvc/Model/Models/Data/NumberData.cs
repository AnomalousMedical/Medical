using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Engine.Editing;
using Engine;

namespace Medical.Controller.AnomalousMvc
{
    public class NumberData : DataModelItem
    {
        public NumberData(String name)
            :base(name)
        {
            
        }

        [Editable]
        public decimal Value { get; set; }

        public override string StringValue
        {
            get
            {
                return Value.ToString();
            }
            set
            {
                decimal result;
                if (NumberParser.TryParse(value, out result))
                {
                    Value = result;
                }
            }
        }

        protected NumberData(LoadInfo info)
            :base(info)
        {

        }
    }
}
