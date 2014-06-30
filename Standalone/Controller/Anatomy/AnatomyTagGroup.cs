using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace Medical
{
    public class AnatomyTagGroup : Anatomy
    {
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

        public void addAnatomy(AnatomyIdentifier anatomy)
        {
            if (showInTree)
            {
                anatomy.addRelatedAnatomy(this);
            }
            groupAnatomy.Add(anatomy);
        }

        public String AnatomicalName { get; private set; }

        public IEnumerable<AnatomyCommand> Commands
        {
            get
            {
                foreach(var anatomy in groupAnatomy)
                {
                    foreach(var command in anatomy.Commands)
                    {
                        yield return command;
                    }
                }
            }
        }

        public IEnumerable<Anatomy> RelatedAnatomy
        {
            get
            {
                return groupAnatomy;
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
                AxisAlignedBox box = new AxisAlignedBox();
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
    }
}
