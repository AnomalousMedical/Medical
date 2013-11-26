using Engine;
using Medical;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lecture.GUI
{
    class SlideLayoutPickerTask : Task
    {
        private SlideLayoutPicker slideLayoutPicker;

        private List<TemplateSlide> presetSlides = new List<TemplateSlide>();

        public event Action<TemplateSlide> ChangeSlideLayout;

        public SlideLayoutPickerTask()
            :base("SlideLayoutPicker", "Change Slide Layout", "Lecture.Icon.LayoutIcon", "Edit")
        {

        }

        public void addPresetSlide(TemplateSlide preset)
        {
            presetSlides.Add(preset);
        }

        public void createLayoutPicker()
        {
            slideLayoutPicker = new SlideLayoutPicker(presetSlides);
            slideLayoutPicker.SlidePresetChosen += slideLayoutPicker_SlidePresetChosen;
        }

        public void destroyLayoutPicker()
        {
            slideLayoutPicker.SlidePresetChosen -= slideLayoutPicker_SlidePresetChosen;
            slideLayoutPicker.Dispose();
            slideLayoutPicker = null;
        }

        public override void clicked(TaskPositioner taskPositioner)
        {
            IntVector2 position = taskPositioner.findGoodWindowPosition(slideLayoutPicker.Width, slideLayoutPicker.Height);
            slideLayoutPicker.show(position.x, position.y);
        }

        public override bool Active
        {
            get
            {
                return false;
            }
        }

        void slideLayoutPicker_SlidePresetChosen(TemplateSlide obj)
        {
            if (ChangeSlideLayout != null)
            {
                ChangeSlideLayout.Invoke(obj);
            }
        }
    }
}
