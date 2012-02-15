using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;
using Engine.Saving;
using Engine;

namespace Medical
{
    class MoveCameraDoAction : DoActionsDataFieldCommand
    {
        public MoveCameraDoAction()
        {
            CameraPosition = new CameraPosition();
            CameraPosition.LookAt = new Vector3(0f, 0f, 0f);
            CameraPosition.Translation = new Vector3(0f, 0f, 150f);
        }

        public override void doAction(Medical.DataDrivenTimelineGUI gui)
        {
            gui.applyCameraPosition(CameraPosition);
        }

        [Editable]
        public CameraPosition CameraPosition { get; set; }

        public override string Type
        {
            get
            {
                return "Move Camera";
            }
        }

        protected MoveCameraDoAction(LoadInfo info)
            :base(info)
        {

        }
    }
}
