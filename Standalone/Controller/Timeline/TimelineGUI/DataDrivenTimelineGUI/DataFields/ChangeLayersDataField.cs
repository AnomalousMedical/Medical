using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;
using Engine.Saving;

namespace Medical
{
    public class ChangeLayersDataField : DataField
    {
        public ChangeLayersDataField(String name)
            :base(name)
        {
            Layers = new EditableLayerState("ChangeLayers");
        }

        public override void createControl(DataControlFactory factory)
        {
            factory.addField(this);
        }

        [Editable]
        public EditableLayerState Layers { get; set; }

        public override string Type
        {
            get
            {
                return "Change Layers";
            }
        }

        protected ChangeLayersDataField(LoadInfo info)
            :base(info)
        {

        }
    }
}
