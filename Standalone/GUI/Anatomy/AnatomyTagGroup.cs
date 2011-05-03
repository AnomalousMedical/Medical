using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace Medical.GUI
{
    class AnatomyTagGroup : Anatomy, IDisposable
    {
        private List<AnatomyCommand> groupCommands = new List<AnatomyCommand>();
        private List<Anatomy> groupAnatomy = new List<Anatomy>();

        public AnatomyTagGroup(String anatomicalName)
        {
            this.AnatomicalName = anatomicalName;
        }

        public void Dispose()
        {
            foreach (AnatomyCommand command in groupCommands)
            {
                command.Dispose();
            }
        }

        public void addAnatomy(Anatomy anatomy)
        {
            groupAnatomy.Add(anatomy);
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

        public Vector3 Center
        {
            get
            {
                Vector3 locationSum = Vector3.Zero;
                int locationCount = 0;
                foreach (Anatomy anatomy in groupAnatomy)
                {
                    locationSum += anatomy.Center;
                    ++locationCount;
                }
                return locationSum / locationCount;
            }
        }

        public TransparencyChanger TransparencyChanger
        {
            get
            {
                foreach (AnatomyCommand command in Commands)
                {
                    if (command is TransparencyChanger)
                    {
                        return (TransparencyChanger)command;
                    }
                }
                return null;
            }
        }
    }
}
