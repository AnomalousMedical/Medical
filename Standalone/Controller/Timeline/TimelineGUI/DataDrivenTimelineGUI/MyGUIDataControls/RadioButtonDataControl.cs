using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;

namespace Medical
{
    public class RadioButtonDataControl : DataControl
    {
        private TextBox text;
        private ButtonGrid optionGrid;
        private ScrollView optionScrollView;
        private String valueName;
        private String defaultValue;

        public RadioButtonDataControl(Widget parentWidget, MultipleChoiceField field)
        {
            text = (TextBox)parentWidget.createWidgetT("StaticText", "StaticText", 0, 0, 10, 15, Align.Default, "");
            text.Caption = field.Name;
            text.setSize(text.getTextRegion().width, text.Height);
            text.NeedMouseFocus = false;

            optionScrollView = (ScrollView)parentWidget.createWidgetT("ScrollView", "ScrollViewEmpty", 0, 0, 10, 15, Align.Default, "");
            optionScrollView.CanvasAlign = Align.Left | Align.Top;
            optionScrollView.VisibleHScroll = false;
            optionScrollView.VisibleVScroll = false;
            optionGrid = new ButtonGrid(optionScrollView, new ButtonGridTextAdjustedGridLayout(25));
            optionGrid.ButtonSkin = "RadioBox";
            optionGrid.ShowGroupCaptions = false;
            optionGrid.HighlightSelectedButton = true;
            optionGrid.ItemWidth = 10;
            optionGrid.ItemHeight = 20;
            optionGrid.SuppressLayout = true;
            foreach (MultipleChoiceOption option in field.Options)
            {
                optionGrid.addItem("", option.OptionText);
            }
            optionGrid.SuppressLayout = false;

            valueName = field.Name;
            defaultValue = field.DefaultValue;
        }

        public override void Dispose()
        {
            optionGrid.Dispose();
            Gui.Instance.destroyWidget(optionScrollView);
            Gui.Instance.destroyWidget(text);
        }

        public override void captureData(DataDrivenExamSection examSection)
        {
            if (optionGrid.Count > 0)
            {
                examSection.setValue(valueName, optionGrid.SelectedItem.Caption);
            }
        }

        public override void displayData(DataDrivenExamSection examSection)
        {
            String value = examSection.getValue(valueName, defaultValue);
            if (value != null)
            {
                ButtonGridItem item = optionGrid.findItemByCaption(value);
                if (item != null)
                {
                    optionGrid.SelectedItem = item;
                }
            }
        }

        public override void bringToFront()
        {
            
        }

        public override void setAlpha(float alpha)
        {
            
        }

        public override void layout()
        {
            text.setPosition((int)Location.x, (int)Location.y);
            text.setSize((int)WorkingSize.Width, text.Height);

            optionScrollView.setPosition((int)Location.x, (int)(Location.y + text.Height));
            optionGrid.resizeAndLayout((int)WorkingSize.Width);
            optionScrollView.setSize((int)WorkingSize.Width, (int)optionScrollView.CanvasSize.Height);
            Height = optionScrollView.Height + text.Height;
        }

        public override Size2 DesiredSize
        {
            get 
            {  
                return new Size2(TopmostWorkingSize.Width, 50);
            }
        }

        public override bool Visible
        {
            get
            {
                return text.Visible;
            }
            set
            {
                text.Visible = value;
            }
        }
    }
}
