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
        private AnatomyContextWindowManager anatomyWindowManager = new AnatomyContextWindowManager();
        private AnatomyTagManager anatomyTagManager = new AnatomyTagManager();

        public AnatomyFinder()
            :base("Medical.GUI.Anatomy.AnatomyFinder.layout")
        {
            anatomyList = (MultiList)window.findWidget("AnatomyList");
            anatomyList.addColumn("Anatomy", anatomyList.Width);
            anatomyList.ListChangePosition += new MyGUIEvent(anatomyList_ListChangePosition);
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

        public void sceneLoaded()
        {
            anatomyTagManager.clear();
            foreach (AnatomyIdentifier anatomy in AnatomyManager.AnatomyList)
            {
                anatomyList.addItem(anatomy.AnatomicalName, anatomy);
                anatomyTagManager.addAnatomyIdentifier(anatomy);
            }
            foreach (AnatomyTagGroup tagGroup in anatomyTagManager.Groups)
            {
                anatomyList.addItem(tagGroup.AnatomicalName, tagGroup);
            }
        }

        public void sceneUnloading()
        {
            anatomyList.removeAllItems();
        }
    }
}
