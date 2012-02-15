using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;
using Engine.Saving;

namespace Medical
{
    class ChangeLayersDoAction : DoActionsDataFieldCommand
    {
        public ChangeLayersDoAction()
        {
            Layers = new EditableLayerState("ChangeLayers");
        }

        public override void doAction(DataDrivenTimelineGUI gui)
        {
            gui.applyLayers(Layers);
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

        protected ChangeLayersDoAction(LoadInfo info)
            :base(info)
        {

        }
    }
}
