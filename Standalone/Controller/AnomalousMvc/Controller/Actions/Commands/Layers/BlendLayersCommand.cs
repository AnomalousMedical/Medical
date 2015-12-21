using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;
using Engine.Saving;

namespace Medical.Controller.AnomalousMvc
{
    public class BlendLayersCommand : ActionCommand
    {
        private EditableLayerState startLayers;
        private EditableLayerState endLayers;

        public BlendLayersCommand()
        {
            startLayers = new EditableLayerState();
            endLayers = new EditableLayerState();
        }

        public override void execute(AnomalousMvcContext context)
        {
            float blend;
            float.TryParse(context.getActionArgument("value"), out blend);
            context.applyBlendedLayers(StartLayers, endLayers, blend);
        }

        protected override void createEditInterface()
        {
            EditInterface editInterface = new EditInterface("Blend Layers");
            editInterface.addSubInterface(startLayers.getEditInterface(Type, ReflectedEditInterface.DefaultScanner));
            editInterface.addSubInterface(endLayers.getEditInterface(Type, ReflectedEditInterface.DefaultScanner));
            editInterface.IconReferenceTag = Icon;
        }

        public LayerState StartLayers
        {
            get
            {
                return startLayers;
            }
        }

        public LayerState EndLayers
        {
            get
            {
                return endLayers;
            }
        }

        public override string Type
        {
            get
            {
                return "Blend Layers";
            }
        }

        public override string Icon
        {
            get
            {
                return CommonResources.NoIcon;
            }
        }

        protected BlendLayersCommand(LoadInfo info)
            :base(info)
        {
            
        }
    }
}
