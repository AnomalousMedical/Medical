using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using System.IO;

namespace Medical.GUI
{
    class TimelineIndexEditor : Dialog
    {
        private TimelineFileBrowserDialog fileBrowser;
        private MultiList startupTimelinesList;
        private Button removeButton;
        private Edit nameEdit;
        private Edit descriptionEdit;
        private TimelineIndexItem currentItem = null;

        public event EventHandler SaveIndexData;

        public TimelineIndexEditor(TimelineFileBrowserDialog fileBrowser)
            : base("Medical.GUI.Timeline.TimelineIndexEditor.layout")
        {
            this.fileBrowser = fileBrowser;

            startupTimelinesList = window.findWidget("StartupTimelinesList") as MultiList;
            startupTimelinesList.addColumn("Timelines", startupTimelinesList.Width);
            startupTimelinesList.ListChangePosition += new MyGUIEvent(startupTimelinesList_ListChangePosition);

            Button addButton = window.findWidget("AddButton") as Button;
            addButton.MouseButtonClick += new MyGUIEvent(addButton_MouseButtonClick);

            removeButton = window.findWidget("RemoveButton") as Button;
            removeButton.Enabled = false;
            removeButton.MouseButtonClick += new MyGUIEvent(removeButton_MouseButtonClick);

            nameEdit = window.findWidget("NameEdit") as Edit;
            nameEdit.Enabled = false;

            descriptionEdit = window.findWidget("DescriptionEdit") as Edit;
            descriptionEdit.Enabled = false;

            Button applyButton = window.findWidget("ApplyButton") as Button;
            applyButton.MouseButtonClick += new MyGUIEvent(applyButton_MouseButtonClick);

            Button cancelButton = window.findWidget("CancelButton") as Button;
            cancelButton.MouseButtonClick += new MyGUIEvent(cancelButton_MouseButtonClick);
        }

        public void setData(TimelineIndex index)
        {
            foreach (TimelineIndexItem item in index.Items)
            {
                TimelineIndexItem addItem = new TimelineIndexItem(item);
                startupTimelinesList.addItem(addItem.TimelineName, addItem);
            }
        }

        public TimelineIndex createIndex()
        {
            TimelineIndex index = new TimelineIndex();
            uint numItems = startupTimelinesList.getItemCount();
            for (uint i = 0; i < numItems; ++i)
            {
                index.addItem((TimelineIndexItem)startupTimelinesList.getItemDataAt(i));
            }
            return index;
        }

        public void clear()
        {
            currentItem = null;
            nameEdit.Caption = "";
            descriptionEdit.Caption = "";
            startupTimelinesList.removeAllItems();
            removeButton.Enabled = false;
            nameEdit.Enabled = false;
            descriptionEdit.Enabled = false;
        }

        void addButton_MouseButtonClick(Widget source, EventArgs e)
        {
            fileBrowser.promptForFile("*.tl", fileChosen);
        }

        public void fileChosen(String file)
        {
            TimelineIndexItem item = new TimelineIndexItem(file);
            item.Name = Path.GetFileNameWithoutExtension(file);
            item.Description = "";
            startupTimelinesList.addItem(file, item);
        }

        void removeButton_MouseButtonClick(Widget source, EventArgs e)
        {
            uint index = startupTimelinesList.getIndexSelected();
            startupTimelinesList.removeItemAt(index);
            removeButton.Enabled = startupTimelinesList.getIndexSelected() != uint.MaxValue;
        }

        void startupTimelinesList_ListChangePosition(Widget source, EventArgs e)
        {
            synchronizeCurrentItem();

            uint index = startupTimelinesList.getIndexSelected();
            bool validIndexSelected = index != uint.MaxValue;
            if (validIndexSelected)
            {
                currentItem = startupTimelinesList.getItemDataAt(index) as TimelineIndexItem;
                nameEdit.Caption = currentItem.Name;
                descriptionEdit.Caption = currentItem.Description;
            }
            removeButton.Enabled = validIndexSelected;
            nameEdit.Enabled = validIndexSelected;
            descriptionEdit.Enabled = validIndexSelected;
        }

        private void synchronizeCurrentItem()
        {
            if (currentItem != null)
            {
                currentItem.Name = nameEdit.Caption;
                currentItem.Description = descriptionEdit.Caption;
            }
        }

        void applyButton_MouseButtonClick(Widget source, EventArgs e)
        {
            synchronizeCurrentItem();
            if (SaveIndexData != null)
            {
                SaveIndexData.Invoke(this, EventArgs.Empty);
            }
            this.clear();
            this.close();
        }

        void cancelButton_MouseButtonClick(Widget source, EventArgs e)
        {
            this.clear();
            this.close();
        }
    }
}
