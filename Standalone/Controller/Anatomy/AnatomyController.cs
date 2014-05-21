using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Medical.Controller;
using MyGUIPlugin;
using System.Drawing;
using System.Reflection;

namespace Medical
{
    public enum AnatomyPickingMode
    {
        Group,
        Individual,
        None,
    }

    public class AnatomyController
    {
        public event EventHandler AnatomyChanged;

        private AnatomyTagManager anatomyTagManager = new AnatomyTagManager();
        private AnatomySearchList anatomySearchList = new AnatomySearchList();

        private AnatomyPickingMode pickingMode;
        public event EventDelegate<AnatomyController, AnatomyPickingMode> PickingModeChanged;
        public event EventDelegate<AnatomyController, bool> ShowPremiumAnatomyChanged;
        private bool showPremiumAnatomy = true;

        /// <summary>
        /// Called when a piece of anatomy has been searched for and should be displayed.
        /// </summary>
        public event Action<Anatomy> DisplayAnatomy;

        /// <summary>
        /// Called when the display of anatomy should be cleared.
        /// </summary>
        public event Action ClearDisplayedAnatomy;

        /// <summary>
        /// Fired when a search is started. This fires for all types of searches.
        /// </summary>
        public event Action SearchStarted;

        /// <summary>
        /// Fired when a search is ended. This fires for all types of searches.
        /// </summary>
        public event Action SearchEnded;

        public AnatomyController()
        {

        }

        public void sceneLoaded()
        {
            AnatomyOrganizer organizer = AnatomyManager.AnatomyOrganizer;
            if (organizer != null)
            {
                anatomyTagManager.setupPropertyGroups(organizer.TagProperties);
            }
            foreach (AnatomyIdentifier anatomy in AnatomyManager.AnatomyList)
            {
                anatomySearchList.addAnatomy(anatomy);
                anatomyTagManager.addAnatomyIdentifier(anatomy);
            }
            foreach (AnatomyTagGroup tagGroup in anatomyTagManager.Groups)
            {
                anatomySearchList.addAnatomy(tagGroup);
            }
            if (AnatomyChanged != null)
            {
                AnatomyChanged.Invoke(this, EventArgs.Empty);
            }
        }

        public void sceneUnloading()
        {
            anatomyTagManager.clear();
            anatomySearchList.clear();
        }

        /// <summary>
        /// Find the anatomy along a given ray. Will fire search events. Returns the best match anatomy based
        /// on position and selection mode (group or individual). Note that this will still return non null anatomy
        /// if the picking mode is none, the caller must deal with that case. Returns null if no anatomy was found
        /// along the ray.
        /// </summary>
        /// <param name="ray"></param>
        /// <returns></returns>
        public Anatomy findAnatomy(Ray3 ray)
        {
            Anatomy bestMatchAnatomy = null;
            fireSearchStarted();
            fireClearDisplayedAnatomy();

            var matches = AnatomyManager.findAnatomy(ray);

            HashSet<String> anatomyTags = new HashSet<String>();
            if (matches.Count > 0)
            {
                AnatomyIdentifier firstMatch = matches[0];
                bestMatchAnatomy = firstMatch;
                foreach (AnatomyIdentifier anatomy in matches)
                {
                    fireDisplayAnatomy(anatomy);
                    foreach (AnatomyTag tag in anatomy.Tags)
                    {
                        anatomyTags.Add(tag.Tag);
                    }
                }
                foreach (AnatomyTagGroup tagGroup in TagManager.Groups)
                {
                    if (tagGroup.ShowInClickSearch && anatomyTags.Contains(tagGroup.AnatomicalName))
                    {
                        fireDisplayAnatomy(tagGroup);
                    }
                }

                if (PickingMode == AnatomyPickingMode.Group && firstMatch.AllowGroupSelection || !showPremiumAnatomy)
                {
                    AnatomyTagGroup tagGroup;
                    foreach (AnatomyTag tag in firstMatch.Tags)
                    {
                        if (anatomyTagManager.tryGetTagGroup(tag.Tag, out tagGroup) && tagGroup.ShowInClickSearch && (showPremiumAnatomy || tagGroup.ShowInBasicVersion))
                        {
                            bestMatchAnatomy = tagGroup;
                            break;
                        }
                    }
                }
            }
            else
            {
                foreach (var anatomy in SearchList.TopLevelAnatomy)
                {
                    fireDisplayAnatomy(anatomy);
                }
            }
            fireSearchEnded();

            return bestMatchAnatomy;
        }

        public void findAnatomy(String searchTerm)
        {
            fireSearchStarted();
            fireClearDisplayedAnatomy();
            if (String.IsNullOrEmpty(searchTerm))
            {
                foreach (Anatomy anatomy in SearchList.TopLevelAnatomy)
                {
                    fireDisplayAnatomy(anatomy);
                }
            }
            else
            {
                foreach (Anatomy anatomy in SearchList.findMatchingAnatomy(searchTerm, 35))
                {
                    fireDisplayAnatomy(anatomy);
                }
            }
            fireSearchEnded();
        }

        public void displayAnatomy(IEnumerable<Anatomy> anatomyToDisplay)
        {
            fireSearchStarted();
            fireClearDisplayedAnatomy();
            foreach (Anatomy relatedAnatomy in anatomyToDisplay)
            {
                fireDisplayAnatomy(relatedAnatomy);
            }
            fireSearchEnded();
        }

        public AnatomyTagManager TagManager
        {
            get
            {
                return anatomyTagManager;
            }
        }

        public AnatomySearchList SearchList
        {
            get
            {
                return anatomySearchList;
            }
        }

        public AnatomyPickingMode PickingMode
        {
            get
            {
                return pickingMode;
            }
            set
            {
                if (value != pickingMode)
                {
                    pickingMode = value;
                    if (PickingModeChanged != null)
                    {
                        PickingModeChanged.Invoke(this, value);
                    }
                }
            }
        }

        public bool ShowPremiumAnatomy
        {
            get
            {
                return showPremiumAnatomy;
            }
            set
            {
                if (showPremiumAnatomy != value)
                {
                    showPremiumAnatomy = value;
                    if (ShowPremiumAnatomyChanged != null)
                    {
                        ShowPremiumAnatomyChanged.Invoke(this, showPremiumAnatomy);
                    }
                }
            }
        }

        private AnatomySelection selectedAnatomy = new AnatomySelection();
        public AnatomySelection SelectedAnatomy
        {
            get
            {
                return selectedAnatomy;
            }
        }

        private void fireDisplayAnatomy(Anatomy anatomy)
        {
            if (DisplayAnatomy != null)
            {
                DisplayAnatomy.Invoke(anatomy);
            }
        }

        private void fireClearDisplayedAnatomy()
        {
            if (ClearDisplayedAnatomy != null)
            {
                ClearDisplayedAnatomy.Invoke();
            }
        }

        private void fireSearchStarted()
        {
            if (SearchStarted != null)
            {
                SearchStarted.Invoke();
            }
        }

        private void fireSearchEnded()
        {
            if (SearchEnded != null)
            {
                SearchEnded.Invoke();
            }
        }
    }
}
