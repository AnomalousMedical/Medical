using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.GUI
{
    public class AnatomyFinder : Dialog
    {
        private MultiList anatomyList;
        private Edit searchBox;

        private AnatomyContextWindowManager anatomyWindowManager = new AnatomyContextWindowManager();
        private AnatomyTagManager anatomyTagManager = new AnatomyTagManager();
        private AnatomySearchList anatomySearchList = new AnatomySearchList();
        private List<AnatomyTagGroup> topLevelGroups = new List<AnatomyTagGroup>();

        public AnatomyFinder()
            :base("Medical.GUI.Anatomy.AnatomyFinder.layout")
        {
            anatomyList = (MultiList)window.findWidget("AnatomyList");
            anatomyList.addColumn("Anatomy", anatomyList.Width);
            anatomyList.ListChangePosition += new MyGUIEvent(anatomyList_ListChangePosition);
            anatomyList.ListSelectAccept += new MyGUIEvent(anatomyList_ListSelectAccept);

            searchBox = (Edit)window.findWidget("SearchBox");
            searchBox.EventEditTextChange += new MyGUIEvent(searchBox_EventEditTextChange);
        }

        public void sceneLoaded()
        {
            anatomyTagManager.clear();
            anatomySearchList.clear();
            topLevelGroups.Clear();
            foreach (AnatomyIdentifier anatomy in AnatomyManager.AnatomyList)
            {
                anatomySearchList.addAnatomy(anatomy);
                anatomyTagManager.addAnatomyIdentifier(anatomy);
            }
            foreach (AnatomyTagGroup tagGroup in anatomyTagManager.Groups)
            {
                anatomyList.addItem(tagGroup.AnatomicalName, tagGroup);
                topLevelGroups.Add(tagGroup);
                anatomySearchList.addAnatomy(tagGroup);
            }
        }

        public void sceneUnloading()
        {
            anatomyList.removeAllItems();
        }

        void searchBox_EventEditTextChange(Widget source, EventArgs e)
        {
            String searchTerm = searchBox.Caption;
            anatomyList.removeAllItems();
            if (searchTerm.Length == 0)
            {
                foreach (AnatomyTagGroup tagGroup in topLevelGroups)
                {
                    anatomyList.addItem(tagGroup.AnatomicalName, tagGroup);
                }
            }
            else
            {
                foreach (Anatomy anatomy in anatomySearchList.findMatchingAnatomy(searchTerm, 35))
                {
                    anatomyList.addItem(anatomy.AnatomicalName, anatomy);
                }
            }
        }

        void anatomyList_ListChangePosition(Widget source, EventArgs e)
        {
            if (anatomyList.hasItemSelected())
            {
                anatomyWindowManager.showWindow((Anatomy)anatomyList.getItemDataAt(anatomyList.getIndexSelected()));
            }
            else
            {
                anatomyWindowManager.closeUnpinnedWindow();
            }
        }

        void anatomyList_ListSelectAccept(Widget source, EventArgs e)
        {
            if (anatomyList.hasItemSelected())
            {
                TransparencyController.smoothSetAllAlphas(0.0f, MedicalConfig.TransparencyChangeMultiplier);
                Anatomy selectedAnatomy = (Anatomy)anatomyList.getItemDataAt(anatomyList.getIndexSelected());
                foreach (AnatomyCommand command in selectedAnatomy.Commands)
                {
                    if (command.UIText == "Transparency")
                    {
                        command.NumericValue = 1.0f;
                    }
                }
            }
        }
    }
}
