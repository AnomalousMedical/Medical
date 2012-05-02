﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;
using Engine.Saving;
using Engine;

namespace Medical.RmlTimeline.Actions
{
    class MoveCameraCommand : RmlGuiActionCommand
    {
        public MoveCameraCommand()
        {
            CameraPosition = new CameraPosition();
            CameraPosition.LookAt = new Vector3(0f, 0f, 0f);
            CameraPosition.Translation = new Vector3(0f, 0f, 150f);
        }

        public override void execute(RmlTimelineGUI gui)
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

        protected MoveCameraCommand(LoadInfo info)
            :base(info)
        {

        }
    }
}
