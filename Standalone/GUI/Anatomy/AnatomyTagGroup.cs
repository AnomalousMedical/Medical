using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.GUI
{
    class AnatomyTagGroup : Anatomy
    {
        private List<AnatomyCommand> groupCommands = new List<AnatomyCommand>();

        public AnatomyTagGroup(String anatomicalName)
        {
            this.AnatomicalName = anatomicalName;
        }

        public void addAnatomy(Anatomy anatomy)
        {
            foreach (AnatomyCommand command in anatomy.Commands)
            {
                bool foundCommand = false;
                foreach (AnatomyCommand groupCommand in groupCommands)
                {
                    if (groupCommand.UIText == command.UIText)
                    {
                        command.addToTagGroupCommand(groupCommand);
                        foundCommand = true;
                        break;
                    }
                }
                if (!foundCommand)
                {
                    AnatomyCommand compoundCommand = command.createTagGroupCommand();
                    groupCommands.Add(compoundCommand);
                }
            }
        }

        public String AnatomicalName { get; private set; }

        public IEnumerable<AnatomyCommand> Commands
        {
            get
            {
                return groupCommands;
            }
        }
    }
}
