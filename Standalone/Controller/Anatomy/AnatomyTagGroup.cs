using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace Medical
{
    public class AnatomyTagGroup : Anatomy, IDisposable
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

        public AxisAlignedBox WorldBoundingBox
        {
            get
            {
                AxisAlignedBox box = new AxisAlignedBox();
                foreach (Anatomy anatomy in groupAnatomy)
                {
                    box.merge(anatomy.WorldBoundingBox);
                }
                return box;
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

        public Vector3 PreviewCameraDirection
        {
            get
            {
                return Vector3.Backward;
            }
        }
    }
}
