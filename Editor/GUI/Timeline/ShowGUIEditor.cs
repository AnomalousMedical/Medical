using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;
using Engine.Editing;

namespace Medical.GUI
{
    class ShowGUIEditor : Dialog
    {
        private TimelineFileBrowserDialog fileBrowser;
        private TimelineController timelineController;
        private ShowGUIPropertiesTable propertiesTable;

        private Edit nextTimelineEdit;
        private ComboBox guiCombo;

        private String nextTimeline;
        private String guiName;
        private TimelineGUIData storeGUIData;
        private TimelineGUIData displayedGUIData;

        public ShowGUIEditor(TimelineFileBrowserDialog fileBrowser, TimelineController timelineController)
            : base("Medical.GUI.Timeline.ShowGUIEditor.layout")
        {
            this.fileBrowser = fileBrowser;
            this.timelineController = timelineController;

            nextTimelineEdit = window.findWidget("NextTimelineEdit") as Edit;
            guiCombo = window.findWidget("GUIChoiceCombo") as ComboBox;
            guiCombo.EventComboChangePosition += new MyGUIEvent(guiCombo_EventComboChangePosition);

            Button browseTimelineButton = window.findWidget("BrowseTimelineButton") as Button;
            browseTimelineButton.MouseButtonClick += new MyGUIEvent(browseTimelineButton_MouseButtonClick);

            Button applyButton = window.findWidget("ApplyButton") as Button;
            applyButton.MouseButtonClick += new MyGUIEvent(applyButton_MouseButtonClick);

            Button cancelButton = window.findWidget("CancelButton") as Button;
            cancelButton.MouseButtonClick += new MyGUIEvent(cancelButton_MouseButtonClick);

            propertiesTable = new ShowGUIPropertiesTable(new Table(window.findWidget("PropertiesTable")));
        }

        public void setProperties(ShowTimelineGUIAction action)
        {
            nextTimeline = action.NextTimeline;
            guiName = action.GUIName;

            //Create a copy of the GUIData to use for editing.
            storeGUIData = action.GUIData;
            if (storeGUIData != null)
            {
                storeGUIData = storeGUIData.createCopy();
            }
        }

        public void clear()
        {
            nextTimeline = null;
            guiName = null;
            storeGUIData = null;
            propertiesTable.clear();
        }

        public ShowTimelineGUIAction createAction()
        {
            ShowTimelineGUIAction action = new ShowTimelineGUIAction();
            action.GUIName = guiName;
            action.NextTimeline = nextTimeline;
            action.GUIData = storeGUIData != null ? storeGUIData.createCopy() : null;
            return action;
        }

        protected override void onShown(EventArgs args)
        {
            guiCombo.removeAllItems();
            base.onShown(args);
            foreach (String prototype in timelineController.GUIFactory.Prototypes)
            {
                guiCombo.addItem(prototype);
            }
            displayStoredValues();
        }

        void browseTimelineButton_MouseButtonClick(Widget source, EventArgs e)
        {
            fileBrowser.Position = new Vector2(source.AbsoluteLeft, source.AbsoluteTop);
            fileBrowser.ensureVisible();
            fileBrowser.promptForFile("*.tl", delegate(String filename)
            {
                nextTimelineEdit.Caption = filename;
            });
        }

        void applyButton_MouseButtonClick(Widget source, EventArgs e)
        {
            storeGUIValues();
            this.close();
        }

        void cancelButton_MouseButtonClick(Widget source, EventArgs e)
        {
            this.close();
        }

        void storeGUIValues()
        {
            nextTimeline = nextTimelineEdit.Caption;
            guiName = guiCombo.SelectedItemName;
            storeGUIData = displayedGUIData != null ? displayedGUIData.createCopy() : null;
        }

        void displayStoredValues()
        {
            nextTimelineEdit.Caption = nextTimeline;

            if (guiName != null)
            {
                uint index = guiCombo.findItemIndexWith(guiName);
                if (index != uint.MaxValue)
                {
                    guiCombo.SelectedIndex = index;
                }
                else
                {
                    guiCombo.addItem(guiName);
                    guiCombo.SelectedIndex = guiCombo.ItemCount - 1;
                }
            }
            else
            {
                guiCombo.clearIndexSelected();
            }

            displayedGUIData = storeGUIData != null ? storeGUIData.createCopy() : null;
            showGUIDataOnTable();
        }

        void guiCombo_EventComboChangePosition(Widget source, EventArgs e)
        {
            displayedGUIData = timelineController.GUIFactory.getGUIData(guiCombo.SelectedItemName);
            showGUIDataOnTable();
        }

        private void showGUIDataOnTable()
        {
            if (displayedGUIData != null)
            {
                propertiesTable.setEditInterface(displayedGUIData.getEditInterface());
            }
            else
            {
                propertiesTable.setEditInterface(null);
            }
        }
    }
}
