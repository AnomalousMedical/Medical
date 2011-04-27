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

        public event AnatomyNumericValueChanged NumericValueChanged;

        public event AnatomyBooleanValueChanged BooleanValueChanged;

        public CompoundAnatomyCommand(AnatomyCommandUIType uiType, String uiText)
        {
            UIType = uiType;
            UIText = uiText;
        }

        public void link(SimObject owner)
        {
            
        }

        public void addSubCommand(AnatomyCommand command)
        {
            if (subCommands.Count == 0 && command.UIType == AnatomyCommandUIType.Numeric)
            {
                NumericValueMin = command.NumericValueMin;
                NumericValueMax = command.NumericValueMax;
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
                foreach (AnatomyCommand command in subCommands)
                {
                    command.NumericValue = value;
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
                foreach (AnatomyCommand command in subCommands)
                {
                    command.BooleanValue = value;
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

        public bool Valid
        {
            get
            {
                return subCommands.Count > 0;
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
