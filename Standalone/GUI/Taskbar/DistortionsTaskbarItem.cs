using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.GUI
{
    public class DistortionsTaskbarItem : TaskbarItem
    {
        private DistortionsPopup distortionsPopup;

        public DistortionsTaskbarItem(StateWizardController wizardController, PiperJBOGUI piperGUI)
            :base("Distortions", "RigidBody")
        {
            distortionsPopup = new DistortionsPopup(wizardController, piperGUI);
        }

        public override void Dispose()
        {
            distortionsPopup.Dispose();
            base.Dispose();
        }

        public override void clicked(Widget source, EventArgs e)
        {
            distortionsPopup.show(source.AbsoluteLeft, source.AbsoluteTop + source.Height);
        }
    }
}
