using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;

namespace Medical.GUI
{
    class ShowGUIEditor : Dialog
    {
        private TimelineFileBrowserDialog fileBrowser;
        private TimelineController timelineController;

        private Edit nextTimelineEdit;
        private ComboBox guiCombo;

        public ShowGUIEditor(TimelineFileBrowserDialog fileBrowser, TimelineController timelineController)
            : base("Medical.GUI.Timeline.ShowGUIEditor.layout")
        {
            this.fileBrowser = fileBrowser;
            this.timelineController = timelineController;

            nextTimelineEdit = window.findWidget("NextTimelineEdit") as Edit;
            guiCombo = window.findWidget("GUIChoiceCombo") as ComboBox;

            Button browseTimelineButton = window.findWidget("BrowseTimelineButton") as Button;
            browseTimelineButton.MouseButtonClick += new MyGUIEvent(browseTimelineButton_MouseButtonClick);

            Button applyButton = window.findWidget("ApplyButton") as Button;
            applyButton.MouseButtonClick += new MyGUIEvent(applyButton_MouseButtonClick);

            Button cancelButton = window.findWidget("CancelButton") as Button;
            cancelButton.MouseButtonClick += new MyGUIEvent(cancelButton_MouseButtonClick);
        }

        public void setProperties(ShowTimelineGUIAction action)
        {
            nextTimelineEdit.Caption = action.NextTimeline;
            uint index = guiCombo.findItemIndexWith(action.GUIName);
            if (index != uint.MaxValue)
            {
                guiCombo.SelectedIndex = index;
            }
            else
            {
                guiCombo.addItem(action.GUIName);
                guiCombo.SelectedIndex = guiCombo.ItemCount - 1;
            }
        }

        public void clear()
        {
            nextTimelineEdit.Caption = "";
        }

        public ShowTimelineGUIAction createAction()
        {
            ShowTimelineGUIAction action = new ShowTimelineGUIAction();
            action.GUIName = guiCombo.SelectedItemName;
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
            this.close();
        }

        void cancelButton_MouseButtonClick(Widget source, EventArgs e)
        {
            this.close();
        }
    }
}
