using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Engine.Editing;

namespace Medical.Controller.AnomalousMvc
{
    public class TextData : DataModelItem
    {
        public TextData(String name)
            :base(name)
        {

        }

        [Editable]
        public String Value { get; set; }

        protected TextData(LoadInfo info)
            :base(info)
        {

        }
    }
}
