﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Engine.Editing;
using Medical.Controller.AnomalousMvc;

namespace Medical.GUI.AnomalousMvc
{
    class PresetStateView : WizardView
    {
        public PresetStateView(String name)
            :base(name)
        {
            AttachToScrollView = false;
        }

        public override ViewHost createViewHost(AnomalousMvcContext context)
        {
            return new PresetStateGUI(this, context);
        }

        [Editable]
        public String PresetDirectory { get; set; }

        protected PresetStateView(LoadInfo info)
            :base(info)
        {

        }
    }
}