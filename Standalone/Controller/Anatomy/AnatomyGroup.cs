using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace Medical
{
    public class AnatomyGroup : Anatomy
    {
        private List<Anatomy> groupAnatomy = new List<Anatomy>();
        private List<AnatomyCommand> additionalCommands = new List<AnatomyCommand>();
        bool showInBasicVersion = false;
        bool showInTextSearch = true;
        bool showInClickSearch = true;
        bool showInTree = true;

        public AnatomyGroup(String anatomicalName)
        {
            this.AnatomicalName = anatomicalName;
        }

        public AnatomyGroup(String anatomicalName, bool showInBasicVersion, bool showInTextSearch, bool showInClickSearch, bool showInTree)
        {
            this.AnatomicalName = anatomicalName;
            this.showInBasicVersion = showInBasicVersion;
            this.showInTextSearch = showInTextSearch;
            this.showInClickSearch = showInClickSearch;
            this.showInTree = showInTree;
        }

        public void addAnatomy(Anatomy anatomy)
        {
            groupAnatomy.Add(anatomy);
        }

        public void addAnatomy(IEnumerable<Anatomy> anatomy)
        {
            groupAnatomy.AddRange(anatomy);
        }

        public void addCommand(AnatomyCommand command)
        {
            additionalCommands.Add(command);
        }

        public String AnatomicalName { get; private set; }

        public IEnumerable<AnatomyCommand> Commands
        {
            get
            {
                foreach(var command in additionalCommands)
                {
                    if (command.DisplayInGroup)
                    {
                        yield return command;
                    }
                }
                foreach(var anatomy in groupAnatomy)
                {
                    foreach(var command in anatomy.Commands)
                    {
                        if (command.DisplayInGroup)
                        {
                            yield return command;
                        }
                    }
                }
            }
        }

        public IEnumerable<Anatomy> SelectableAnatomy
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
                AxisAlignedBox box = AxisAlignedBox.MinMaxable;
                foreach (Anatomy anatomy in groupAnatomy)
                {
                    box.merge(anatomy.WorldBoundingBox);
                }
                return box;
            }
        }

        public void smoothBlend(float alpha, float duration, EasingFunction easingFunction)
        {
            foreach(var anatomy in groupAnatomy)
            {
                anatomy.smoothBlend(alpha, duration, easingFunction);
            }
        }

        public float CurrentAlpha
        {
            get
            {
                if(groupAnatomy.Count > 0)
                {
                    return groupAnatomy[0].CurrentAlpha; ;
                }
                return 0.0f;
            }
        }

        public IEnumerable<String> TransparencyNames
        {
            get
            {
                foreach (Anatomy anatomy in groupAnatomy)
                {
                    foreach (String name in anatomy.TransparencyNames)
                    {
                        yield return name;
                    }
                }
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
                return showInTextSearch && groupAnatomy.Count > 0;
            }
        }

        public bool ShowInClickSearch
        {
            get
            {
                return showInClickSearch && groupAnatomy.Count > 0;
            }
        }

        public bool ShowInTree
        {
            get
            {
                return showInTree && groupAnatomy.Count > 0;
            }
        }

        public bool ShowInBasicVersion
        {
            get
            {
                return showInBasicVersion;
            }
        }

        /// <summary>
        /// The number of anatomy items in this group.
        /// </summary>
        public int Count
        {
            get
            {
                return groupAnatomy.Count;
            }
        }

        public IEnumerable<AnatomyFacet> Facets { get; set; }
    }
}
