using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;

namespace Medical.GUI
{
    class ShowTextProperties : TimelineDataPanel
    {
        private ShowTextAction showText;
        private TimelineData timelineData;

        private NumericEdit xPosition;
        private NumericEdit yPosition;
        private NumericEdit width;
        private NumericEdit height;
        private NumericEdit fontHeight;
        private ComboBox fontNameCombo;
        private EnumComboBox<TextualAlignment> alignCombo;

        private StaticText cameraText;

        public ShowTextProperties(Widget parentWidget, ITextDisplayFactory textFactory)
            :base(parentWidget, "Medical.GUI.Timeline.ActionProperties.ShowTextProperties.layout")
        {
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

            fontHeight = new NumericEdit(mainWidget.findWidget("FontHeight") as Edit);
            fontHeight.ValueChanged += new MyGUIEvent(fontHeight_ValueChanged);
            fontHeight.MinValue = 1;
            fontHeight.MaxValue = 1000;
            fontHeight.Increment = 1;
            fontHeight.AllowFloat = false;

            fontNameCombo = (ComboBox)mainWidget.findWidget("FontCombo");
            foreach (String font in textFactory.FontNames)
            {
                fontNameCombo.addItem(font);
            }

            alignCombo = new EnumComboBox<TextualAlignment>((ComboBox)mainWidget.findWidget("AlignCombo"));
            alignCombo.EventComboChangePosition += new MyGUIEvent(alignCombo_EventComboAccept);

            cameraText = mainWidget.findWidget("Camera") as StaticText;
            Button useCurrent = mainWidget.findWidget("UseCurrent") as Button;
            useCurrent.MouseButtonClick += new MyGUIEvent(useCurrent_MouseButtonClick);

            Button colorButton = mainWidget.findWidget("ColorButton") as Button;
            colorButton.MouseButtonClick += new MyGUIEvent(colorButton_MouseButtonClick);
        }

        public override void setCurrentData(TimelineData data)
        {
            timelineData = data;
            showText = (ShowTextAction)((TimelineActionData)data).Action;
            Vector2 position = showText.Position;
            xPosition.FloatValue = position.x;
            yPosition.FloatValue = position.y;
            Size2 size = showText.Size;
            width.FloatValue = size.Width;
            height.FloatValue = size.Height;
            cameraText.Caption = showText.CameraName;
            fontHeight.IntValue = showText.FontHeight;
            uint index = fontNameCombo.findItemIndexWith(showText.FontName);
            //Add the font to the combo if it isnt in there already
            if (index == uint.MaxValue)
            {
                fontNameCombo.addItem(showText.FontName);
                index = fontNameCombo.ItemCount - 1;
            }
            fontNameCombo.SelectedIndex = index;
        }

        void position_ValueChanged(Widget source, EventArgs e)
        {
            showText.Position = new Vector2(xPosition.FloatValue, yPosition.FloatValue);
        }

        void size_ValueChanged(Widget source, EventArgs e)
        {
            showText.Size = new Size2(width.FloatValue, height.FloatValue);
        }

        void useCurrent_MouseButtonClick(Widget source, EventArgs e)
        {
            showText.capture();
            cameraText.Caption = showText.CameraName;
        }

        void fontHeight_ValueChanged(Widget source, EventArgs e)
        {
            showText.FontHeight = fontHeight.IntValue;
        }

        void alignCombo_EventComboAccept(Widget source, EventArgs e)
        {
            showText.TextAlign = alignCombo.Value;
        }

        void colorButton_MouseButtonClick(Widget source, EventArgs e)
        {
            ColorMenu.ShowColorMenu(source.AbsoluteLeft, source.AbsoluteTop + source.Height, delegate(Color color)
            {
                showText.setSelectionColor(color);
            });
        }
    }
}
