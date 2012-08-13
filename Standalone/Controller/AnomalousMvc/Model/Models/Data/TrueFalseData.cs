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

        protected TrueFalseData(LoadInfo info)
            :base(info)
        {

        }
    }
}
