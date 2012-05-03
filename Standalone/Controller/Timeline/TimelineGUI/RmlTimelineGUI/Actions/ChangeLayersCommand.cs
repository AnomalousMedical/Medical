﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;
using Engine.Saving;

namespace Medical.RmlTimeline.Actions
{
    class ChangeLayersCommand : RmlGuiActionCommand
    {
        public ChangeLayersCommand()
        {
            Layers = new EditableLayerState("ChangeLayers");
        }

        public override void execute(RmlTimelineGUI gui)
        {
            gui.applyLayers(Layers);
        }

        protected override void createEditInterface()
        {
            editInterface = Layers.getEditInterface(Type, ReflectedEditInterface.DefaultScanner);
        }

        public EditableLayerState Layers { get; set; }

        public override string Type
        {
            get
            {
                return "Change Layers";
            }
        }

        protected ChangeLayersCommand(LoadInfo info)
            :base(info)
        {

        }
    }
}