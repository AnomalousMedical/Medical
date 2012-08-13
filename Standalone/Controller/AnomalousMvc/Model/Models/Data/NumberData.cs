using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Engine.Editing;

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

        protected NumberData(LoadInfo info)
            :base(info)
        {

        }
    }
}
