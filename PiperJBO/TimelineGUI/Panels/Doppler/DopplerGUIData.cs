using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Engine;
using Engine.Editing;

namespace Medical.GUI
{
    class DopplerGUIData : AbstractTimelineGUIData
    {
        public DopplerGUIData()
        {

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
        public Vector3 LateralJointCameraPosition { get; set; }

        [Editable]
        public Vector3 LateralJointCameraLookAt { get; set; }

        [Editable]
        public Vector3 SuperiorJointCameraPosition { get; set; }

        [Editable]
        public Vector3 SuperiorJointCameraLookAt { get; set; }

        [Editable]
        public Vector3 BothJointsCameraPosition { get; set; }

        [Editable]
        public Vector3 BothJointsCameraLookAt { get; set; }

        [Editable]
        public LayerState JointMenuLayers { get; set; }

        [Editable]
        public LayerState DiscLayers { get; set; }

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
