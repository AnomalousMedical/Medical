using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;
using Engine.Saving;

namespace Medical.Controller.AnomalousMvc
{
    public class ChangeLayersCommand : ActionCommand
    {
        private EditableLayerState layers;

        public ChangeLayersCommand()
        {
            layers = new EditableLayerState();
        }

        public override void execute(AnomalousMvcContext context)
        {
            context.applyLayers(Layers);
        }

        protected override void createEditInterface()
        {
            editInterface = layers.getEditInterface(Type, ReflectedEditInterface.DefaultScanner);
            editInterface.IconReferenceTag = Icon;
        }

        public LayerState Layers
        {
            get
            {
                return layers;
            }
        }

        public override string Type
        {
            get
            {
                return "Change Layers";
            }
        }

        public override string Icon
        {
            get
            {
                return "MvcContextEditor/LayersChangeIcon";
            }
        }

        protected ChangeLayersCommand(LoadInfo info)
            :base(info)
        {
            if (!info.hasValue("layers"))
            {
                layers = info.GetValue<EditableLayerState>("<Layers>k__BackingField");
            }
        }
    }
}
