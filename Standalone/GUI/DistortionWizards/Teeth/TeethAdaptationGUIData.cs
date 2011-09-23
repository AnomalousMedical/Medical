using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;
using Engine.Saving;

namespace Medical.GUI
{
    public class TeethAdaptationGUIData : TimelineWizardPanelData
    {
        public TeethAdaptationGUIData()
        {
            TopCamera = new CameraPosition();
            BottomCamera = new CameraPosition();
            LeftLateralCamera = new CameraPosition();
            RightLateralCamera = new CameraPosition();
            MidlineAnteriorCamera = new CameraPosition();
            AllTeethLayers = new EditableLayerState("AllTeethLayers");
            TopCameraLayers = new EditableLayerState("TopCameraLayers");
            BottomCameraLayers = new EditableLayerState("BottomCameraLayers");
        }

        [Editable]
        public CameraPosition TopCamera { get; set; }

        [Editable]
        public CameraPosition BottomCamera { get; set; }

        [Editable]
        public CameraPosition LeftLateralCamera { get; set; }

        [Editable]
        public CameraPosition RightLateralCamera { get; set; }

        [Editable]
        public CameraPosition MidlineAnteriorCamera { get; set; }

        [Editable]
        public EditableLayerState AllTeethLayers { get; set; }

        [Editable]
        public EditableLayerState TopCameraLayers { get; set; }

        [Editable]
        public EditableLayerState BottomCameraLayers { get; set; }

        public override string Name
        {
            get { return "TeethAdaptationGUIData"; }
        }

        protected TeethAdaptationGUIData(LoadInfo info)
            :base(info)
        {
            
        }
    }
}
