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

        public SlideLayoutPickerTask()
            :base("SlideLayoutPicker", "Change Slide Layout", CommonResources.NoIcon, "Edit")
        {

        }

        public void createLayoutPicker()
        {
            slideLayoutPicker = new SlideLayoutPicker();
        }

        public void destroyLayoutPicker()
        {
            slideLayoutPicker.Dispose();
            slideLayoutPicker = null;
        }

        public override void clicked(TaskPositioner taskPositioner)
        {
            IntVector2 position = taskPositioner.findGoodWindowPosition(slideLayoutPicker.Width, slideLayoutPicker.Height);
            slideLayoutPicker.setPosition(position.x, position.y);
        }

        public override bool Active
        {
            get
            {
                return false;
            }
        }
    }
}
