using Medical;
using MyGUIPlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lecture.GUI
{
    class SlideLayoutPicker : PopupContainer
    {
        private ButtonGrid buttonGrid;

        public event Action<TemplateSlide> SlidePresetChosen;

        public SlideLayoutPicker(IEnumerable<TemplateSlide> presetSlides)
            :base("Lecture.GUI.SlideLayoutPicker.SlideLayoutPicker.layout")
        {
            buttonGrid = new ButtonGrid((ScrollView)widget.findWidget("ButtonGrid"), new SingleSelectionStrategy());

            foreach (TemplateSlide slide in presetSlides)
            {
                ButtonGridItem item = buttonGrid.addItem("Main", slide.Name, slide.IconName);
                item.ItemClicked += item_ItemClicked;
                item.UserObject = slide;
            }
        }

        public override void Dispose()
        {
            buttonGrid.Dispose();
            base.Dispose();
        }

        void item_ItemClicked(object sender, EventArgs e)
        {
            if (SlidePresetChosen != null)
            {
                SlidePresetChosen.Invoke((TemplateSlide)((ButtonGridItem)sender).UserObject);
            }
        }
    }
}
