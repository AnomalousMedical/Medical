﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Medical.Controller;
using MyGUIPlugin;
using System.Reflection;
using Medical.Utility.LuceneUtil;

namespace Medical
{
    public enum AnatomyPickingMode
    {
        Group,
        Individual,
        None,
    }

    public enum SelectionOperator
    {
        Select,
        Add,
        Remove
    }

    public enum SuggestedDisplaySortMode
    {
        None,
        Alphabetical
    }

    public class AnatomyController : IDisposable
    {
        public event EventHandler AnatomyChanged;

        private AnatomyTagManager anatomyTagManager = new AnatomyTagManager();
        private AnatomySearchList anatomySearchList = new AnatomySearchList();
        private AnatomyLuceneSearch luceneSearch = new AnatomyLuceneSearch();

        private AnatomyPickingMode pickingMode = AnatomyPickingMode.Group;
        private SelectionOperator selectionOperator = SelectionOperator.Select;
        public event EventDelegate<AnatomyController, AnatomyPickingMode> PickingModeChanged;
        public event EventDelegate<AnatomyController, bool> ShowPremiumAnatomyChanged;
        public event EventDelegate<AnatomyController, AnatomyCommandPermissions> CommandPermissionsChanged;
        public event EventDelegate<AnatomyController, SelectionOperator> SelectionOperatorChanged;
        private bool showPremiumAnatomy = true;
        private AnatomyCommandPermissions commandPermissions = AnatomyCommandPermissions.None;

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
        public event Action<SuggestedDisplaySortMode> SearchStarted;

        /// <summary>
        /// Fired when a search is ended. This fires for all types of searches.
        /// </summary>
        public event Action SearchEnded;

        public AnatomyController()
        {

        }

        public void Dispose()
        {
            luceneSearch.Dispose();
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
            luceneSearch.setAnatomy(AnatomyManager.AnatomyList);
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
            fireSearchStarted(SuggestedDisplaySortMode.None);
            fireClearDisplayedAnatomy();

            var matches = AnatomyManager.findAnatomy(ray);

            HashSet<String> anatomyTags = new HashSet<String>();
            if (matches.Count > 0)
            {
                AnatomyIdentifier firstMatch = matches.Closest;
                bestMatchAnatomy = firstMatch;
                foreach (AnatomyIdentifier anatomy in matches.Anatomy)
                {
                    fireDisplayAnatomy(anatomy);
                    foreach (var tag in anatomy.Tags)
                    {
                        anatomyTags.Add(tag);
                    }
                }
                foreach (AnatomyTagGroup tagGroup in anatomyTagManager.Groups)
                {
                    if (tagGroup.ShowInClickSearch && anatomyTags.Contains(tagGroup.AnatomicalName))
                    {
                        fireDisplayAnatomy(tagGroup);
                    }
                }

                if (PickingMode == AnatomyPickingMode.Group && firstMatch.AllowGroupSelection || !showPremiumAnatomy)
                {
                    AnatomyTagGroup tagGroup;
                    foreach (var tag in firstMatch.Tags)
                    {
                        if (anatomyTagManager.tryGetTagGroup(tag, out tagGroup) && tagGroup.ShowInClickSearch && (showPremiumAnatomy || tagGroup.ShowInBasicVersion))
                        {
                            bestMatchAnatomy = tagGroup;
                            break;
                        }
                    }
                }
            }
            else
            {
                foreach (var anatomy in anatomySearchList.TopLevelAnatomy)
                {
                    fireDisplayAnatomy(anatomy);
                }
            }
            fireSearchEnded();

            return bestMatchAnatomy;
        }

        public void findAnatomy(String searchTerm)
        {
            if (String.IsNullOrEmpty(searchTerm))
            {
                fireSearchStarted(SuggestedDisplaySortMode.Alphabetical);
                fireClearDisplayedAnatomy();

                foreach (Anatomy anatomy in anatomySearchList.TopLevelAnatomy)
                {
                    fireDisplayAnatomy(anatomy);
                }
            }
            else
            {
                fireSearchStarted(SuggestedDisplaySortMode.None);
                fireClearDisplayedAnatomy();

                //foreach (Anatomy anatomy in anatomySearchList.findMatchingAnatomy(searchTerm, 35))
                //List<Facet> facets = new List<Facet>();
                //facets.Add(new Facet("Systems", "Muscular System"));
                //facets.Add(new Facet("Systems", "Nervous System"));
                foreach(var anatomy in luceneSearch.search(searchTerm, IEnumerableUtil<Facet>.EmptyIterator, 35))
                {
                    fireDisplayAnatomy(anatomy);
                }
            }
            fireSearchEnded();
        }

        public void displayAnatomy(IEnumerable<Anatomy> anatomyToDisplay, SuggestedDisplaySortMode sortMode)
        {
            fireSearchStarted(sortMode);
            fireClearDisplayedAnatomy();
            foreach (Anatomy relatedAnatomy in anatomyToDisplay)
            {
                fireDisplayAnatomy(relatedAnatomy);
            }
            fireSearchEnded();
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

        public SelectionOperator SelectionOperator
        {
            get
            {
                return selectionOperator;
            }
            set
            {
                if(value != selectionOperator)
                {
                    selectionOperator = value;
                    if(SelectionOperatorChanged != null)
                    {
                        SelectionOperatorChanged.Invoke(this, selectionOperator);
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

        public void setCommandPermission(AnatomyCommandPermissions permission, bool enabled)
        {
            if (enabled)
            {
                CommandPermissions |= permission;
            }
            else
            {
                CommandPermissions &= (~permission);
            }
        }

        public void processSelection(Anatomy anatomy)
        {
            switch (SelectionOperator)
            {
                case SelectionOperator.Select:
                    SelectedAnatomy.setSelection(anatomy);
                    break;
                case SelectionOperator.Add:
                    SelectedAnatomy.addSelection(anatomy);
                    break;
                case SelectionOperator.Remove:
                    SelectedAnatomy.removeSelection(anatomy);
                    break;
            }
        }

        public AnatomyCommandPermissions CommandPermissions
        {
            get
            {
                return commandPermissions;
            }
            private set
            {
                if (commandPermissions != value)
                {
                    commandPermissions = value;
                    if(CommandPermissionsChanged != null)
                    {
                        CommandPermissionsChanged.Invoke(this, commandPermissions);
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

        private void fireSearchStarted(SuggestedDisplaySortMode suggestedSortMode)
        {
            if (SearchStarted != null)
            {
                SearchStarted.Invoke(suggestedSortMode);
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
