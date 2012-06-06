using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;
using Engine.Editing;
using Medical.Editor;

namespace Medical.GUI
{
    class ShowImageProperties : TimelineDataPanel
    {
        private ShowImageAction showImage;
        private TimelineData timelineData;
        private EditBox imageFileEdit;
        private MedicalUICallback uiCallback;

        private EnumComboBox<ImageAlignment> alignment;
        private NumericEdit xPosition;
        private NumericEdit yPosition;
        private NumericEdit width;
        private NumericEdit height;

        private CheckButton keepAspectRatio;

        private TextBox cameraText;

        public ShowImageProperties(Widget parentWidget, MedicalUICallback uiCallback)
            :base(parentWidget, "Medical.GUI.TimelineEditor.ActionProperties.ShowImageProperties.layout")
        {
            this.uiCallback = uiCallback;

            imageFileEdit = mainWidget.findWidget("ImageFileEdit") as EditBox;

            Button browseButton = mainWidget.findWidget("BrowseButton") as Button;
            browseButton.MouseButtonClick += new MyGUIEvent(browseButton_MouseButtonClick);

            alignment = new EnumComboBox<ImageAlignment>((ComboBox)mainWidget.findWidget("Alignment"));
            alignment.EventComboChangePosition += new MyGUIEvent(alignment_EventComboChangePosition);

            xPosition = new NumericEdit(mainWidget.findWidget("XPositionEdit") as EditBox);
            xPosition.ValueChanged += position_ValueChanged;
            xPosition.MinValue = 0.0f;
            xPosition.MaxValue = 1.0f;
            xPosition.Increment = 0.05f;
            
            yPosition = new NumericEdit(mainWidget.findWidget("YPositionEdit") as EditBox);
            yPosition.ValueChanged += position_ValueChanged;
            yPosition.MinValue = 0.0f;
            yPosition.MaxValue = 1.0f;
            yPosition.Increment = 0.05f;
            
            width = new NumericEdit(mainWidget.findWidget("WidthEdit") as EditBox);
            width.ValueChanged += size_ValueChanged;
            width.MinValue = 0.0f;
            width.MaxValue = 1.0f;
            width.Increment = 0.05f;
            
            height = new NumericEdit(mainWidget.findWidget("HeightEdit") as EditBox);
            height.ValueChanged += size_ValueChanged;
            height.MinValue = 0.0f;
            height.MaxValue = 1.0f;
            height.Increment = 0.05f;

            keepAspectRatio = new CheckButton(mainWidget.findWidget("KeepAspectCheck") as Button);
            keepAspectRatio.CheckedChanged += new MyGUIEvent(keepAspectRatio_CheckedChanged);

            cameraText = mainWidget.findWidget("Camera") as TextBox;
            Button useCurrent = mainWidget.findWidget("UseCurrent") as Button;
            useCurrent.MouseButtonClick += new MyGUIEvent(useCurrent_MouseButtonClick);
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
            keepAspectRatio.Checked = showImage.KeepAspectRatio;
            cameraText.Caption = showImage.CameraName;
            alignment.Value = showImage.Alignment;
        }

        void browseButton_MouseButtonClick(Widget source, EventArgs e)
        {
            Browser browser = BrowserWindowController.createFileBrowser("*.png", "Image Files");
            uiCallback.showBrowser<String>(browser, fileChosen);
        }

        bool fileChosen(String filename, ref String errorMessage)
        {
            showImage.ImageFile = filename;
            imageFileEdit.Caption = showImage.ImageFile;
            return true;
        }

        void position_ValueChanged(Widget source, EventArgs e)
        {
            showImage.Position = new Vector2(xPosition.FloatValue, yPosition.FloatValue);
        }

        void size_ValueChanged(Widget source, EventArgs e)
        {
            showImage.Size = new Size2(width.FloatValue, height.FloatValue);
        }

        void keepAspectRatio_CheckedChanged(Widget source, EventArgs e)
        {
            showImage.KeepAspectRatio = keepAspectRatio.Checked;
        }

        void useCurrent_MouseButtonClick(Widget source, EventArgs e)
        {
            showImage.capture();
            cameraText.Caption = showImage.CameraName;
        }

        void alignment_EventComboChangePosition(Widget source, EventArgs e)
        {
            showImage.Alignment = alignment.Value;
        }
    }
}
