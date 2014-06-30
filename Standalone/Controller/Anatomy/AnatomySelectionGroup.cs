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
        private List<Anatomy> relatedAnatomy = new List<Anatomy>(3);

        public AnatomySelectionGroup(IEnumerable<Anatomy> selectedAnatomy)
        {
            foreach (var anatomy in selectedAnatomy)
            {
                relatedAnatomy.Add(anatomy);
                foreach (var selectable in anatomy.SelectableAnatomy)
                {
                    addSingleAnatomy(selectable);
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
                foreach (var anatomy in groupAnatomy)
                {
                    foreach (var command in anatomy.Commands)
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
                return relatedAnatomy;
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
                if (groupAnatomy.Count > 0)
                {
                    return groupAnatomy.First().CurrentAlpha;
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

        private void addSingleAnatomy(Anatomy anatomy)
        {
            //Only add the anatomy if we haven't yet.
            if (!groupAnatomy.Contains(anatomy))
            {
                groupAnatomy.Add(anatomy);
            }
        }
    }
}
