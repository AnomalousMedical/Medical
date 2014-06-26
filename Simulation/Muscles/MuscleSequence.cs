using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Engine.Platform;
using Engine.Editing;
using Engine.Attributes;
using Engine.Saving;

namespace Medical
{
    public class MuscleSequence : Behavior
    {
        [Editable]
        private String sequenceName;

        [Editable]
        private String iconName = "";

        [Editable]
        private String groupName = "Group Me";

        [Editable]
        private bool repeat = false;

        private float currentTime = 0.0f;
        private int currentState = 0;

        [DoNotSave]
        private List<MuscleSequenceState> states = new List<MuscleSequenceState>();

        protected override void constructed()
        {
            MuscleController.addMuscleSequence(this);
        }

        protected override void destroy()
        {
            base.destroy();
            MuscleController.removeMuscleSequence(this);
        }

        public override void update(Clock clock, EventManager eventManager)
        {
            currentTime += (float)clock.Seconds;
            if (currentTime > states[currentState].Duration)
            {
                currentState++;
                currentTime = 0.0f;
                if (states.Count > currentState)
                {
                    states[currentState].apply();
                }
                else
                {
                    if (repeat)
                    {
                        currentState = 0;
                        states[currentState].apply();
                    }
                    else
                    {
                        deactivate();
                    }
                }
            }
        }

        public void activate()
        {
            currentTime = 0.0f;
            currentState = 0;
            if (states.Count > currentState)
            {
                MuscleController.setCurrentSequence(this);
                states[currentState].apply();
                Owner.Enabled = true;
            }
        }

        public void deactivate()
        {
            Owner.Enabled = false;
        }

        protected override void customLoad(LoadInfo info)
        {
            base.customLoad(info);
            info.RebuildList<MuscleSequenceState>("MuscleSequenceStates", states);
        }

        protected override void customSave(SaveInfo info)
        {
            base.customSave(info);
            info.ExtractList<MuscleSequenceState>("MuscleSequenceStates", states);
        }

        public String SequenceName
        {
            get
            {
                return sequenceName;
            }
        }

        public String IconName
        {
            get
            {
                return iconName;
            }
        }

        public String GroupName
        {
            get
            {
                return groupName;
            }
        }

        #region EditInterface

        private EditInterfaceCommand removeStateCommand;
        private EditInterfaceManager<MuscleSequenceState> stateEdits;

        protected override void customizeEditInterface(EditInterface editInterface)
        {
            base.customizeEditInterface(editInterface);
            removeStateCommand = new EditInterfaceCommand("Remove", removeState);
            editInterface.addCommand(new EditInterfaceCommand("Add State", addState));
            stateEdits = new EditInterfaceManager<MuscleSequenceState>(editInterface);
            foreach (MuscleSequenceState state in states)
            {
                createStateEditInterface(state);
            }
        }

        private void addState(EditUICallback callback, EditInterfaceCommand command)
        {
            MuscleSequenceState state = new MuscleSequenceState();
            states.Add(state);
            createStateEditInterface(state);
        }

        private void removeState(EditUICallback callback, EditInterfaceCommand command)
        {
            MuscleSequenceState state = stateEdits.resolveSourceObject(callback.getSelectedEditInterface());
            states.Remove(state);
            stateEdits.removeSubInterface(state);
        }

        private void createStateEditInterface(MuscleSequenceState state)
        {
            EditInterface edit = ReflectedEditInterface.createEditInterface(state, ReflectedEditInterface.DefaultScanner, "Muscle Sequence State", null);
            edit.addCommand(removeStateCommand);
            stateEdits.addSubInterface(state, edit);
        }

        #endregion EditInterface
    }
}
