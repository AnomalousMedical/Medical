using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Engine.Editing;
using Engine.Attributes;

namespace Medical
{
    public class StaticTextDataField : DataField
    {
        private int fontHeight = 14;
        private int indentation = 0;

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

        [Editable]
        public int FontHeight
        {
            get
            {
                return fontHeight;
            }
            set
            {
                fontHeight = value;
            }
        }

        [Editable]
        public int Indentation
        {
            get
            {
                return indentation;
            }
            set
            {
                indentation = value;
            }
        }

        protected StaticTextDataField(LoadInfo info)
            :base(info)
        {

        }
    }
}
