﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller.AnomalousMvc;
using Engine.Saving;
using Engine.Editing;

namespace Medical.GUI.AnomalousMvc
{
    public class MyGUIView : View
    {
        private ButtonCollection buttons = new ButtonCollection();

        public MyGUIView(String name)
            : base(name)
        {

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
