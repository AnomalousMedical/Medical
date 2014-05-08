using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical
{
    /// <summary>
    /// This class is an Anatomy subclass that can have anatomy dynamically added and removed from it.
    /// This is used as a way to make selections.
    /// </summary>
    class AnatomySelectionGroup : Anatomy
    {
        private HashSet<Anatomy> groupAnatomy = new HashSet<Anatomy>();
        private List<AnatomyCommand> groupCommands = new List<AnatomyCommand>();

        public AnatomySelectionGroup()
        {

        }

        public AnatomySelectionGroup(IEnumerable<Anatomy> items)
        {
            foreach(var item in items)
            {
                addAnatomy(item);
            }
        }

        public void addAnatomy(Anatomy anatomy)
        {
            if (anatomy != null)
            {
                foreach (var selectable in anatomy.SelectableAnatomy)
                {
                    addSingleAnatomy(selectable);
                }
            }
        }

        private void addSingleAnatomy(Anatomy anatomy)
        {
            //Only add the anatomy if we haven't yet.
            if (!groupAnatomy.Contains(anatomy))
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
        }

        public string AnatomicalName
        {
            get
            {
                return "Selection";
            }
        }

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
                yield break;
            }
        }

        public Engine.AxisAlignedBox WorldBoundingBox
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

        public IEnumerable<Anatomy> SelectableAnatomy
        {
            get
            {
                yield break;
            }
        }

        public bool ShowInTextSearch
        {
            get
            {
                return false;
            }
        }

        public bool ShowInClickSearch
        {
            get
            {
                return false;
            }
        }

        public bool ShowInTree
        {
            get
            {
                return false;
            }
        }

        public bool ShowInBasicVersion
        {
            get
            {
                return true;
            }
        }
    }
}
