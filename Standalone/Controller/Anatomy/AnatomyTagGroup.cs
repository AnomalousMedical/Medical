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
        bool showInBasicVersion = true;
        bool showInTextSearch = true;
        bool showInClickSearch = true;
        bool showInTree = true;

        public AnatomyTagGroup(String anatomicalName)
        {
            this.AnatomicalName = anatomicalName;
        }

        public AnatomyTagGroup(String anatomicalName, bool showInBasicVersion, bool showInTextSearch, bool showInClickSearch, bool showInTree)
        {
            this.AnatomicalName = anatomicalName;
            this.showInBasicVersion = showInBasicVersion;
            this.showInTextSearch = showInTextSearch;
            this.showInClickSearch = showInClickSearch;
            this.showInTree = showInTree;
        }

        public void Dispose()
        {
            foreach (AnatomyCommand command in groupCommands)
            {
                command.Dispose();
            }
        }

        public void addAnatomy(AnatomyIdentifier anatomy)
        {
            if (showInTree)
            {
                anatomy.addRelatedAnatomy(this);
            }
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

        public IEnumerable<Anatomy> RelatedAnatomy
        {
            get
            {
                return groupAnatomy;
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
                return NullTransparencyChanger.Instance;
            }
        }

        public Vector3 PreviewCameraDirection
        {
            get
            {
                return Vector3.Backward;
            }
        }

        public bool ShowInTextSearch
        {
            get
            {
                return showInTextSearch;
            }
        }

        public bool ShowInClickSearch
        {
            get
            {
                return showInClickSearch;
            }
        }

        public bool ShowInTree
        {
            get
            {
                return showInTree;
            }
        }

        public bool ShowInBasicVersion
        {
            get
            {
                return showInBasicVersion;
            }
        }
    }
}
