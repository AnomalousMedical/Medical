using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Engine.ObjectManagement;
using Engine.Editing;

namespace Medical
{
    /// <summary>
    /// This is a special case command that has several sub commands. It is not
    /// possible to build this through the editor and instead it is built when
    /// the tag groups are built.
    /// </summary>
    public class CompoundAnatomyCommand : AnatomyCommand
    {
        private List<AnatomyCommand> subCommands = new List<AnatomyCommand>();

        private bool valueChanging = false;

        public event AnatomyNumericValueChanged NumericValueChanged;
        public event AnatomyBooleanValueChanged BooleanValueChanged;

        public CompoundAnatomyCommand(AnatomyCommandUIType uiType, String uiText)
        {
            UIType = uiType;
            UIText = uiText;
        }

        public void Dispose()
        {
            foreach (AnatomyCommand command in subCommands)
            {
                command.NumericValueChanged -= command_NumericValueChanged;
                command.BooleanValueChanged -= command_BooleanValueChanged;
            }
            subCommands.Clear();
        }

        public bool link(SimObject owner, AnatomyIdentifier parentAnatomy, ref String errorMessage)
        {
            errorMessage = "Cannot link CompoundAnatomyCommands";
            return false;
        }

        public void addSubCommand(AnatomyCommand command)
        {
            switch (command.UIType)
            {
                case AnatomyCommandUIType.Numeric:
                    command.NumericValueChanged += command_NumericValueChanged;
                    if (subCommands.Count == 0)
                    {
                        NumericValueMin = command.NumericValueMin;
                        NumericValueMax = command.NumericValueMax;
                    }
                    break;
                case AnatomyCommandUIType.Boolean:
                    command.BooleanValueChanged += command_BooleanValueChanged;
                    break;
            }
            
            subCommands.Add(command);
        }

        public AnatomyCommandUIType UIType { get; private set; }

        public float NumericValue
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
                    if (NumericValueChanged != null)
                    {
                        NumericValueChanged.Invoke(this, value);
                    }
                }
            }
        }

        public float NumericValueMin { get; private set; }

        public float NumericValueMax { get; private set; }

        public bool BooleanValue
        {
            get
            {
                if (subCommands.Count > 0)
                {
                    return subCommands[0].BooleanValue;
                }
                else
                {
                    return false;
                }
            }
            set
            {
                if (value != BooleanValue)
                {
                    valueChanging = true;
                    foreach (AnatomyCommand command in subCommands)
                    {
                        command.BooleanValue = value;
                    }
                    valueChanging = false;
                    if (BooleanValueChanged != null)
                    {
                        BooleanValueChanged.Invoke(this, value);
                    }
                }
            }
        }

        public string UIText { get; private set; }

        public void execute()
        {
            foreach (AnatomyCommand command in subCommands)
            {
                command.execute();
            }
        }

        public bool allowDisplay(AnatomyCommandPermissions permissions)
        {
            if (subCommands.Count > 0)
            {
                return subCommands[0].allowDisplay(permissions);
            }
            else
            {
                return true;
            }
        }

        void command_BooleanValueChanged(AnatomyCommand command, bool value)
        {
            if (!valueChanging && BooleanValueChanged != null)
            {
                BooleanValueChanged.Invoke(this, value);
            }
        }

        void command_NumericValueChanged(AnatomyCommand command, float value)
        {
            if (!valueChanging && NumericValueChanged != null)
            {
                NumericValueChanged.Invoke(this, value);
            }
        }

        public EditInterface createEditInterface()
        {
            throw new NotSupportedException();
        }

        public AnatomyCommand createTagGroupCommand()
        {
            throw new NotSupportedException();
        }

        public void addToTagGroupCommand(AnatomyCommand tagGroupCommand)
        {
            throw new NotSupportedException();
        }

        #region Saving

        public void getInfo(SaveInfo info)
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}
