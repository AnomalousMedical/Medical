﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;
using Logging;
using Engine.Saving;

namespace Medical.Controller.AnomalousMvc
{
    public class RestoreMusclePositionCommand : ActionCommand
    {
        public RestoreMusclePositionCommand()
        {
            Name = "DefaultMusclePosition";
        }

        public override void execute(AnomalousMvcContext context)
        {
            if (Name != null)
            {
                MusclePosition musclePosition = context.getModel<MusclePosition>(Name);
                if (musclePosition != null)
                {
                    context.applyMusclePosition(musclePosition, MedicalConfig.CameraTransitionTime);
                }
                else
                {
                    Log.Warning("Cannot find muscle position {0}.", Name);
                }
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
                return "Restore Muscle Position";
            }
        }

        public override string Icon
        {
            get
            {
                return "MvcContextEditor/MusclePositionRestoreIcon";
            }
        }

        protected RestoreMusclePositionCommand(LoadInfo info)
            : base(info)
        {

        }
    }
}
