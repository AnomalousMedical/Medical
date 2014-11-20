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
        Classification,
        Structure
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
            luceneSearch.setAnatomy(AnatomyManager.AnatomyList, AnatomyManager.AnatomyOrganizer);
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
            fireSearchStarted(SuggestedDisplaySortMode.Alphabetical);
            fireClearDisplayedAnatomy();

            Anatomy bestMatchAnatomy = null;

            var matches = AnatomyManager.findAnatomy(ray);

            if (matches.Count > 0)
            {
                //Display found anatomy and related groups
                HashSet<String> displayedGroups = new HashSet<String>();
                foreach (AnatomyIdentifier anatomy in matches.Anatomy)
                {
                    fireDisplayAnatomy(anatomy);
                    foreach (var group in luceneSearch.relatedGroupsFor(anatomy))
                    {
                        if (group.ShowInClickSearch && displayedGroups.Add(group.AnatomicalName))
                        {
                            fireDisplayAnatomy(group);
                        }
                    }
                }

                //Choose which anatomy to select, start with the closest match
                AnatomyIdentifier firstMatch = matches.Closest;
                bestMatchAnatomy = firstMatch;
                if (PickingMode == AnatomyPickingMode.Group || !showPremiumAnatomy)
                {
                    try
                    {
                        Anatomy groupSelection = currentClickGroupSelectionFor(firstMatch);
                        if (groupSelection != null)
                        {
                            bestMatchAnatomy = groupSelection;
                        }
                    }
                    catch(Exception)
                    {
                        //Ignore exceptions, in this case we just use the selected anatomy.
                    }
                }
            }
            else
            {
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

        public TopLevelMode TopLevelMode
        {
            get
            {
                return currentTopLevelMode;
            }
            set
            {
                currentTopLevelMode = value;
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

        public IEnumerable<AnatomyGroup> Structures
        {
            get
            {
                return luceneSearch.Structures;
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
                case TopLevelMode.Structure:
                    return luceneSearch.Structures;
                default:
                    throw new NotImplementedException();
            }
        }

        private Anatomy currentClickGroupSelectionFor(AnatomyIdentifier anatomy)
        {
            if (showPremiumAnatomy)
            {
                String system = anatomy.Systems.FirstOrDefault();
                switch (currentTopLevelMode)
                {
                    case TopLevelMode.Classification:
                        return luceneSearch.buildGroupFromFacets(String.Format("{0} of the {1}", anatomy.Classification, anatomy.Region), IEnumerableUtil<AnatomyFacet>.Iter(new AnatomyFacet("Classification", anatomy.Classification), new AnatomyFacet("Region", anatomy.Region)));
                    case TopLevelMode.Region:
                        return luceneSearch.buildGroupFromFacets(String.Format("{0} of the {1}", system, anatomy.Region), IEnumerableUtil<AnatomyFacet>.Iter(new AnatomyFacet("System", system), new AnatomyFacet("Region", anatomy.Region)));
                    case TopLevelMode.System:
                        return luceneSearch.buildGroupFromFacets(String.Format("{0} of the {1}", system, anatomy.Region), IEnumerableUtil<AnatomyFacet>.Iter(new AnatomyFacet("System", system), new AnatomyFacet("Region", anatomy.Region)));
                    case TopLevelMode.Structure:
                        return luceneSearch.buildGroupFromFacets(String.Format("{0} of the {1}", system, anatomy.Structure), IEnumerableUtil<AnatomyFacet>.Iter(new AnatomyFacet("System", system), new AnatomyFacet("Structure", anatomy.Structure)));
                    default:
                        throw new NotImplementedException();
                }
            }
            else
            {
                AnatomyGroup groupSelection;
                luceneSearch.tryGetSystem(anatomy.Systems.FirstOrDefault(), out groupSelection);
                return groupSelection;
            }
        }
    }
}
