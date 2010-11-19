using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;

namespace Medical.GUI
{
    class ShowImageProperties : TimelineDataPanel
    {
        private ShowImageAction showImage;
        private TimelineData timelineData;
        private Edit imageFileEdit;

        private NumericEdit xPosition;
        private NumericEdit yPosition;
        private NumericEdit width;
        private NumericEdit height;

        public ShowImageProperties(Widget parentWidget)
            :base(parentWidget, "Medical.GUI.Timeline.ActionProperties.ShowImageProperties.layout")
        {
            imageFileEdit = mainWidget.findWidget("ImageFileEdit") as Edit;

            Button browseButton = mainWidget.findWidget("BrowseButton") as Button;
            browseButton.MouseButtonClick += new MyGUIEvent(browseButton_MouseButtonClick);

            xPosition = new NumericEdit(mainWidget.findWidget("XPositionEdit") as Edit);
            xPosition.ValueChanged += position_ValueChanged;
            xPosition.MinValue = 0.0f;
            xPosition.MaxValue = 1.0f;
            xPosition.Increment = 0.05f;
            
            yPosition = new NumericEdit(mainWidget.findWidget("YPositionEdit") as Edit);
            yPosition.ValueChanged += position_ValueChanged;
            yPosition.MinValue = 0.0f;
            yPosition.MaxValue = 1.0f;
            yPosition.Increment = 0.05f;
            
            width = new NumericEdit(mainWidget.findWidget("WidthEdit") as Edit);
            width.ValueChanged += size_ValueChanged;
            width.MinValue = 0.0f;
            width.MaxValue = 1.0f;
            width.Increment = 0.05f;
            
            height = new NumericEdit(mainWidget.findWidget("HeightEdit") as Edit);
            height.ValueChanged += size_ValueChanged;
            height.MinValue = 0.0f;
            height.MaxValue = 1.0f;
            height.Increment = 0.05f;
        }

        public override void setCurrentData(TimelineData data)
        {
            timelineData = data;
            showImage = (ShowImageAction)((TimelineActionData)data).Action;
            imageFileEdit.Caption = showImage.ImageFile;
            Vector2 position = showImage.Position;
            xPosition.FloatValue = position.x;
            yPosition.FloatValue = position.y;
            Size2 size = showImage.Size;
            width.FloatValue = size.Width;
            height.FloatValue = size.Height;
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

        void position_ValueChanged(Widget source, EventArgs e)
        {
            showImage.Position = new Vector2(xPosition.FloatValue, yPosition.FloatValue);
        }

        void size_ValueChanged(Widget source, EventArgs e)
        {
            showImage.Size = new Size2(width.FloatValue, height.FloatValue);
        }
    }
}
