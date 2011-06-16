using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Engine;
using Engine.Editing;

namespace Medical.GUI
{
    class DopplerGUIData : TimelineWizardPanelData
    {
        public DopplerGUIData()
        {
            LateralJointCamera = new CameraPosition();
            SuperiorJointCamera = new CameraPosition();
            BothJointsCamera = new CameraPosition();
            NormalLayers = new EditableLayerState("JointMenu");
            LateralJointLayers = new EditableLayerState("DiscLayers");
        }

        protected override void customizeEditInterface(EditInterface editInterface)
        {
            editInterface.addCommand(new EditInterfaceCommand("Test", delegate(EditUICallback callback, EditInterfaceCommand caller)
            {
                callback.getInputString("Type something here", delegate(String input, ref String errorMessage)
                {
                    callback.getSelectedEditInterface().addSubInterface(ReflectedEditInterface.createEditInterface(input, null));
                    return true;
                });
            }));
        }

        [Editable]
        public CameraPosition LateralJointCamera { get; set; }

        [Editable]
        public CameraPosition SuperiorJointCamera { get; set; }

        [Editable]
        public CameraPosition BothJointsCamera { get; set; }

        [Editable]
        public EditableLayerState NormalLayers { get; set; }

        [Editable]
        public EditableLayerState LateralJointLayers { get; set; }

        public override string Name
        {
            get { return "DopplerGUIData"; }
        }

        protected DopplerGUIData(LoadInfo info)
            :base(info)
        {

        }
    }
}
