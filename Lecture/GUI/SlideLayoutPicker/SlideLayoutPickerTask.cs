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

        private List<Slide> presetSlides = new List<Slide>();

        public event Action<Slide> ChangeSlideLayout;

        public SlideLayoutPickerTask()
            :base("SlideLayoutPicker", "Change Slide Layout", CommonResources.NoIcon, "Edit")
        {

        }

        public void addPresetSlide(Slide preset)
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

        void slideLayoutPicker_SlidePresetChosen(Slide obj)
        {
            if (ChangeSlideLayout != null)
            {
                ChangeSlideLayout.Invoke(obj);
            }
        }
    }
}
