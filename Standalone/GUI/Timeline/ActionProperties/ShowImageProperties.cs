using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.GUI
{
    class ShowImageProperties : TimelineDataPanel
    {
        private ShowImageAction showImage;
        private TimelineData timelineData;
        private Edit imageFileEdit;

        public ShowImageProperties(Widget parentWidget)
            :base(parentWidget, "Medical.GUI.Timeline.ActionProperties.ShowImageProperties.layout")
        {
            imageFileEdit = mainWidget.findWidget("ImageFileEdit") as Edit;

            Button browseButton = mainWidget.findWidget("BrowseButton") as Button;
            browseButton.MouseButtonClick += new MyGUIEvent(browseButton_MouseButtonClick);
        }

        public override void setCurrentData(TimelineData data)
        {
            timelineData = data;
            showImage = (ShowImageAction)((TimelineActionData)data).Action;
            imageFileEdit.Caption = showImage.ImageFile;
        }

        void browseButton_MouseButtonClick(Widget source, EventArgs e)
        {
            showImage.TimelineController.promptForFile("*.png", fileChosen);
        }

        void fileChosen(String filename)
        {
            showImage.ImageFile = filename;
            imageFileEdit.Caption = showImage.ImageFile;
        }
    }
}
