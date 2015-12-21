using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Engine.Editing;

namespace Medical.Controller.AnomalousMvc
{
    public class BlendMusclePositionCommand : ActionCommand
    {
        private MusclePosition startPosition;
        private MusclePosition endPosition;

        public BlendMusclePositionCommand()
        {
            startPosition = new MusclePosition();
            startPosition.captureState();

            endPosition = new MusclePosition();
            endPosition.captureState();
        }

        public override void execute(AnomalousMvcContext context)
        {
            float blend;
            float.TryParse(context.getActionArgument("value"), out blend);
            startPosition.incrementalBlend(endPosition, blend);
        }

        protected override void createEditInterface()
        {
            editInterface = new EditInterface(Type);
            editInterface.addSubInterface(startPosition.getEditInterface("Start Position"));
            editInterface.addSubInterface(endPosition.getEditInterface("End Position"));
            editInterface.IconReferenceTag = Icon;
        }

        public MusclePosition StartPosition
        {
            get
            {
                return startPosition;
            }
        }

        public MusclePosition EndPosition
        {
            get
            {
                return endPosition;
            }
        }

        public override string Type
        {
            get
            {
                return "Blend Muscle Position";
            }
        }

        public override string Icon
        {
            get
            {
                return CommonResources.NoIcon;
            }
        }

        protected BlendMusclePositionCommand(LoadInfo info)
            :base(info)
        {

        }
    }
}
