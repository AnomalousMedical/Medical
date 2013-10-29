using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.ObjectManagement;
using Engine.Saving;
using Engine;

namespace Medical
{
    class CompoundTransparencyAnatomyCommand : AbstractNumericAnatomyCommand, TransparencyChanger
    {
        private List<TransparencyAnatomyCommand> subCommands = new List<TransparencyAnatomyCommand>();

        private bool valueChanging = false;

        public CompoundTransparencyAnatomyCommand()
        {

        }

        public override void Dispose()
        {
            foreach (AnatomyCommand command in subCommands)
            {
                command.NumericValueChanged -= command_NumericValueChanged;
            }
            subCommands.Clear();
        }

        public override bool link(SimObject owner, AnatomyIdentifier parentAnatomy, ref String errorMessage)
        {
            errorMessage = "Cannot link CompoundTransparencyAnatomyCommands";
            return false;
        }

        public void smoothBlend(float alpha, float duration, EasingFunction easingFunction)
        {
            foreach (TransparencyAnatomyCommand command in subCommands)
            {
                command.smoothBlend(alpha, duration, easingFunction);
            }
        }

        public float CurrentAlpha
        {
            get
            {
                return NumericValue;
            }
            set
            {
                foreach (TransparencyAnatomyCommand command in subCommands)
                {
                    command.NumericValue = value;
                }
            }
        }

        public void addSubCommand(TransparencyAnatomyCommand command)
        {
            command.NumericValueChanged += command_NumericValueChanged;
            subCommands.Add(command);
        }

        public override AnatomyCommandUIType UIType
        {
            get
            {
                return AnatomyCommandUIType.Numeric;
            }
        }

        public override float NumericValue
        {
            get
            {
                if (subCommands.Count > 0)
                {
                    return subCommands[0].NumericValue;
                }
                else
                {
                    return 0;
                }
            }
            set
            {
                if (value != NumericValue)
                {
                    valueChanging = true;
                    foreach (AnatomyCommand command in subCommands)
                    {
                        command.NumericValue = value;
                    }
                    valueChanging = false;
                    fireNumericValueChanged(value);
                }
            }
        }

        public override float NumericValueMin
        {
            get { return 0.0f; }
        }

        public override float NumericValueMax
        {
            get { return 1.0f; }
        }

        public override string UIText
        {
            get { return "Transparency"; }
        }

        public override AnatomyCommand createTagGroupCommand()
        {
            throw new NotSupportedException();
        }

        public override void addToTagGroupCommand(AnatomyCommand tagGroupCommand)
        {
            throw new NotSupportedException();
        }

        public override void getInfo(SaveInfo info)
        {
            throw new NotSupportedException();
        }

        void command_NumericValueChanged(AnatomyCommand command, float value)
        {
            if (!valueChanging)
            {
                fireNumericValueChanged(value);
            }
        }
    }
}
