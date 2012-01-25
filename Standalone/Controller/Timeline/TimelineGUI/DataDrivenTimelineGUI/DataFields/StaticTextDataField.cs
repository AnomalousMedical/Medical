using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Engine.Editing;

namespace Medical
{
    public class StaticTextDataField : DataField
    {
        public StaticTextDataField(String name)
            :base(name)
        {

        }

        public override void createControl(DataControlFactory factory)
        {
            factory.addField(this);
        }

        public override string Type
        {
            get
            {
                return "Static Text";
            }
        }

        [Editable]
        public String Text { get; set; }

        protected StaticTextDataField(LoadInfo info)
            :base(info)
        {

        }
    }
}
