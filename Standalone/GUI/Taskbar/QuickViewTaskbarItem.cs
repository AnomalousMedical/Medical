using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Medical.Controller;

namespace Medical.GUI
{
    public class QuickViewTaskbarItem : TaskbarItem
    {
        private QuickViewPopup quickViewPopup;

        public QuickViewTaskbarItem(NavigationController navigationController, SceneViewController sceneViewController, LayerController layerController)
            :base("Quick View", "Camera")
        {
            quickViewPopup = new QuickViewPopup(navigationController, sceneViewController, layerController);
        }

        public override void Dispose()
        {
            quickViewPopup.Dispose();
            base.Dispose();
        }

        public override void clicked(Widget source, EventArgs e)
        {
            quickViewPopup.show(source.AbsoluteLeft, source.AbsoluteTop + source.Height);
        }
    }
}
