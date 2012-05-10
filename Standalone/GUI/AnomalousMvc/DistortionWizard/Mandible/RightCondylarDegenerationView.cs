﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Editor;
using Medical.Controller.AnomalousMvc;
using Engine.Saving;

namespace Medical.GUI.AnomalousMvc
{
    class RightCondylarDegenerationView : WizardView
    {
        public RightCondylarDegenerationView(String name)
            : base(name)
        {

        }

        public override ViewHost createViewHost(AnomalousMvcContext context)
        {
            return new RightCondylarDegenerationGUI(this, context);
        }

        [EditableAction]
        public String NormalAction { get; set; }

        [EditableAction]
        public String ShowOsteophyteAction { get; set; }

        protected RightCondylarDegenerationView(LoadInfo info)
            :base(info)
        {

        }
    }
}