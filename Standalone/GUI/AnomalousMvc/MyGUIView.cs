using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller.AnomalousMvc;
using Engine.Saving;
using Engine.Editing;
using Medical.Editor;

namespace Medical.GUI.AnomalousMvc
{
    public class MyGUIView : View
    {
        private ButtonCollection buttons;

        public MyGUIView(String name)
            : base(name)
        {
            buttons = new ButtonCollection();
        }

        public ButtonCollection Buttons
        {
            get
            {
                return buttons;
            }
        }

        protected override void customizeEditInterface(EditInterface editInterface)
        {
            editInterface.addSubInterface(buttons.getEditInterface("Buttons"));
            base.customizeEditInterface(editInterface);
        }

        protected MyGUIView(LoadInfo info)
            :base(info)
        {
            if (buttons == null)
            {
                buttons = new ButtonCollection();
            }
        }
    }
}
