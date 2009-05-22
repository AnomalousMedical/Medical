using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WeifenLuo.WinFormsUI.Docking;

namespace Medical
{
    class CommonController : MedicalInterface
    {
        private MedicalController controller;
        private CommonToolStrip toolStrip;

        public void initialize(MedicalController controller)
        {
            toolStrip = new CommonToolStrip(this);
            this.controller = controller;
            controller.addToolStrip(toolStrip);
        }

        public void sceneChanged()
        {
            toolStrip.sceneChanged();
        }

        public void destroy()
        {
            controller.removeToolStrip(toolStrip);
            toolStrip.Dispose();
        }

        public void addControlToUI(DockContent control)
        {
            controller.showDockContent(control);
        }

        public void removeControl(DockContent control)
        {
            controller.hideDockContent(control);
        }
    }
}
