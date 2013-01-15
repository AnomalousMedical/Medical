using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;
using Engine.Saving;
using Engine;

namespace Medical.Controller.AnomalousMvc
{
    public class MoveCameraCommand : ActionCommand
    {
        public MoveCameraCommand()
        {
            CameraPosition = new CameraPosition();
            CameraPosition.LookAt = new Vector3(0f, 0f, 0f);
            CameraPosition.Translation = new Vector3(0f, 0f, 150f);
        }

        public override void execute(AnomalousMvcContext context)
        {
            context.applyCameraPosition(CameraPosition);
        }

        protected override void createEditInterface()
        {
            editInterface = CameraPosition.getEditInterface(Type, ReflectedEditInterface.DefaultScanner);
            editInterface.IconReferenceTag = Icon;
        }

        public CameraPosition CameraPosition { get; set; }

        public override string Type
        {
            get
            {
                return "Move Camera";
            }
        }

        public override string Icon
        {
            get
            {
                return "MvcContextEditor/CameraMoveIcon";
            }
        }

        protected MoveCameraCommand(LoadInfo info)
            :base(info)
        {

        }
    }
}
