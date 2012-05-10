﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller.AnomalousMvc;
using Engine.Saving;
using Engine.Editing;

namespace Medical.GUI.AnomalousMvc
{
    class NavigationView : View
    {
        public NavigationView(String name)
            :base(name)
        {
            NavigationModelName = NavigationModel.DefaultName;
        }

        [Editable]
        public String NavigationModelName { get; set; }

        protected NavigationView(LoadInfo info)
            :base(info)
        {

        }
    }
}