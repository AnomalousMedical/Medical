using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.GUI
{
    class LayersTaskbarItem : TaskbarItem
    {
        private LayersPopup layersPopup;

        public LayersTaskbarItem(LayerController layerController)
            :base("Custom Layers", "ManualObject")
        {
            layersPopup = new LayersPopup(layerController);
        }

        public override void Dispose()
        {
            layersPopup.Dispose();
            base.Dispose();
        }

        public override void clicked(Widget source, EventArgs e)
        {
            layersPopup.show(source.AbsoluteLeft, source.AbsoluteTop + source.Height);
        }

        public bool AllowShortcuts
        {
            get
            {
                return layersPopup.AllowShortcuts;
            }
            set
            {
                layersPopup.AllowShortcuts = value;
            }
        }
    }
}
