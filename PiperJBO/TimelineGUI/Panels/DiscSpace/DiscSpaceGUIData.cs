using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;
using Engine.Saving;

namespace Medical.GUI
{
    class DiscSpaceGUIData : TimelineWizardPanelData
    {
        public DiscSpaceGUIData()
        {
            NormalLayers = new EditableLayerState("NormalLayers");
            ShowDiscLayers = new EditableLayerState("DiscLayers");
        }

        [Editable]
        public EditableLayerState NormalLayers { get; set; }

        [Editable]
        public EditableLayerState ShowDiscLayers { get; set; }

        public override string Name
        {
            get { return "DiscSpaceGUIData"; }
        }

        protected DiscSpaceGUIData(LoadInfo info)
            :base(info)
        {

        }
    }
}
