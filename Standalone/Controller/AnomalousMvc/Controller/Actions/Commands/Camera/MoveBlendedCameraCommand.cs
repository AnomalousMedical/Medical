using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;
using Engine.Saving;
using Engine;
using Anomalous.GuiFramework.Cameras;

namespace Medical.Controller.AnomalousMvc
{
    public class MoveBlendedCameraCommand : ActionCommand
    {
        public MoveBlendedCameraCommand()
        {
            CameraStartPosition = new CameraPosition();
            CameraStartPosition.LookAt = new Vector3(0f, 0f, 0f);
            CameraStartPosition.Translation = new Vector3(0f, 0f, 150f);

            CameraEndPosition = new CameraPosition();
            CameraEndPosition.LookAt = new Vector3(0f, 0f, 0f);
            CameraEndPosition.Translation = new Vector3(0f, 0f, 150f);
        }

        public override void execute(AnomalousMvcContext context)
        {
            float blend;
            float.TryParse(context.getActionArgument("value"), out blend);
            var blendedPosition = new CameraPosition(CameraStartPosition);
            blendedPosition.LookAt = CameraStartPosition.LookAt.lerp(CameraEndPosition.LookAt, blend);
            blendedPosition.Translation = CameraStartPosition.Translation.lerp(CameraEndPosition.Translation, blend);
            blendedPosition.IncludePoint = CameraStartPosition.IncludePoint.lerp(CameraEndPosition.IncludePoint, blend);
            context.applyCameraPosition(blendedPosition, 0.0f);
        }

        protected override void createEditInterface()
        {
            editInterface = new EditInterface(Type);
            editInterface.addSubInterface(CameraStartPosition.getEditInterface("Start Position", ReflectedEditInterface.DefaultScanner));
            editInterface.addSubInterface(CameraEndPosition.getEditInterface("End Position", ReflectedEditInterface.DefaultScanner));
            editInterface.IconReferenceTag = Icon;
        }

        public CameraPosition CameraStartPosition { get; set; }

        public CameraPosition CameraEndPosition { get; set; }

        public override string Type
        {
            get
            {
                return "Move Blended Camera";
            }
        }

        public override string Icon
        {
            get
            {
                return CommonResources.NoIcon;
            }
        }

        protected MoveBlendedCameraCommand(LoadInfo info)
            : base(info)
        {

        }
    }
}
