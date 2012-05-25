using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Engine.Editing;
using Logging;

namespace Medical.Controller.AnomalousMvc
{
    public class SaveMusclePositionCommand: ActionCommand
    {
        public SaveMusclePositionCommand()
        {
            Name = "DefaultMusclePosition";
        }

        public override void execute(AnomalousMvcContext context)
        {
            if (Name != null)
            {
                MusclePosition musclePosition = new MusclePosition();
                musclePosition.captureState();
                context.addModel(Name, musclePosition);
            }
            else
            {
                Log.Warning("No name defined.");
            }
        }

        [Editable]
        public String Name { get; set; }

        public override string Type
        {
            get
            {
                return "Save Layers";
            }
        }

        protected SaveMusclePositionCommand(LoadInfo info)
            : base(info)
        {

        }
    }
}
