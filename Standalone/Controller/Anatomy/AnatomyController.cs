using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Medical.Controller;
using MyGUIPlugin;
using System.Reflection;

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

    public enum TopLevelMode
    {
        System,
        Region,
        Tag,
        Classification
    }

    public class AnatomyController : IDisposable
    {
        private AnatomyLuceneSearch luceneSearch;

        private AnatomyPickingMode pickingMode = AnatomyPickingMode.Group;
        private SelectionOperator selectionOperator = SelectionOperator.Select;
        private bool showPremiumAnatomy = true;
        private AnatomyCommandPermissions commandPermissions = AnatomyCommandPermissions.None;
        private TopLevelMode currentTopLevelMode = TopLevelMode.System;

        public event EventDelegate<AnatomyController> AnatomyChanged;
        public event EventDelegate<AnatomyController, AnatomyPickingMode> PickingModeChanged;
        public event EventDelegate<AnatomyController, bool> ShowPremiumAnatomyChanged;
        public event EventDelegate<AnatomyController, AnatomyCommandPermissions> CommandPermissionsChanged;
        public event EventDelegate<AnatomyController, SelectionOperator> SelectionOperatorChanged;

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
        /// Fired when a search is started that needs to suggest a caption.
        /// </summary>
        public event Action<String> SuggestSearchCaption;

        /// <summary>
        /// Fired when a search is ended. This fires for all types of searches.
        /// </summary>
        public event Action SearchEnded;

        public AnatomyController()
        {
            luceneSearch = new AnatomyLuceneSearch(this);
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
                luceneSearch.setAnatomyOrganizer(organizer);
            }
            luceneSearch.setAnatomy(AnatomyManager.AnatomyList);
            if (AnatomyChanged != null)
            {
                AnatomyChanged.Invoke(this);
            }
        }

        public void sceneUnloading()
        {
            luceneSearch.clear();
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

            var matches = AnatomyManager.findAnatomy(ray);

            HashSet<String> anatomyTags = new HashSet<String>();
            if (matches.Count > 0)
            {
                fireSearchStarted(SuggestedDisplaySortMode.None);
                fireClearDisplayedAnatomy();

                AnatomyIdentifier firstMatch = matches.Closest;
                bestMatchAnatomy = firstMatch;
                foreach (AnatomyIdentifier anatomy in matches.Anatomy)
                {
                    fireDisplayAnatomy(anatomy);
                    foreach (var system in anatomy.Tags)
                    {
                        anatomyTags.Add(system);
                    }
                }
                //Show related tag anatomy
                AnatomyGroup tagAnatomy;
                foreach(String tag in anatomyTags)
                {
                    if(luceneSearch.tryGetTag(tag, out tagAnatomy) && tagAnatomy.ShowInClickSearch)
                    {
                        fireDisplayAnatomy(tagAnatomy);
                    }
                }

                if (PickingMode == AnatomyPickingMode.Group && firstMatch.AllowGroupSelection || !showPremiumAnatomy)
                {
                    AnatomyGroup tagGroup;
                    foreach (var tag in firstMatch.Tags)
                    {
                        if (luceneSearch.tryGetTag(tag, out tagGroup) && tagGroup.ShowInClickSearch && (showPremiumAnatomy || tagGroup.ShowInBasicVersion))
                        {
                            bestMatchAnatomy = tagGroup;
                            break;
                        }
                    }
                }
            }
            else
            {
                fireSearchStarted(SuggestedDisplaySortMode.Alphabetical);
                fireClearDisplayedAnatomy();

                foreach (var anatomy in currentSelectionEnum().Where(i => i.ShowInTree))
                {
                    fireDisplayAnatomy(anatomy);
                }
            }
            fireSearchEnded();

            return bestMatchAnatomy;
        }

        public void findAnatomy(String searchTerm, IEnumerable<AnatomyFacet> facets)
        {
            if (String.IsNullOrEmpty(searchTerm))
            {
                fireSearchStarted(SuggestedDisplaySortMode.Alphabetical);
                fireClearDisplayedAnatomy();

                foreach (Anatomy anatomy in currentSelectionEnum().Where(i => i.ShowInTree))
                {
                    fireDisplayAnatomy(anatomy);
                }
            }
            else
            {
                fireSearchStarted(SuggestedDisplaySortMode.None);
                fireClearDisplayedAnatomy();

                foreach(var anatomy in luceneSearch.search(searchTerm, facets, 35))
                {
                    fireDisplayAnatomy(anatomy);
                }
            }
            fireSearchEnded();
        }

        public void displayAnatomy(String searchCaption, IEnumerable<Anatomy> anatomyToDisplay, SuggestedDisplaySortMode sortMode)
        {
            fireSuggestSearchCaption(searchCaption);
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

        public IEnumerable<AnatomyGroup> Systems
        {
            get
            {
                return luceneSearch.Systems;
            }
        }

        public IEnumerable<AnatomyGroup> Tags
        {
            get
            {
                return luceneSearch.Tags;
            }
        }

        public IEnumerable<AnatomyGroup> Regions
        {
            get
            {
                return luceneSearch.Regions;
            }
        }

        public IEnumerable<AnatomyGroup> Classifications
        {
            get
            {
                return luceneSearch.Classifications;
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

        private void fireSuggestSearchCaption(String caption)
        {
            if(SuggestSearchCaption != null)
            {
                SuggestSearchCaption.Invoke(caption);
            }
        }

        private IEnumerable<AnatomyGroup> currentSelectionEnum()
        {
            switch(currentTopLevelMode)
            {
                case TopLevelMode.Classification:
                    return luceneSearch.Classifications;
                case TopLevelMode.Region:
                    return luceneSearch.Regions;
                case TopLevelMode.System:
                    return luceneSearch.Systems;
                case TopLevelMode.Tag:
                    return luceneSearch.Tags;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
