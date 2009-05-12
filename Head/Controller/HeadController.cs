using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI;
using System.Windows.Forms;

namespace Medical.Controller
{
    public class HeadController : MedicalInterface
    {
        private MedicalController controller;
        private HeadToolStrip headToolStrip;

        public void initialize(MedicalController controller)
        {
            headToolStrip = new HeadToolStrip(this);
            this.controller = controller;
            controller.MedicalForm.addToolStrip(headToolStrip);
        }

        public void destroy()
        {
            controller.MedicalForm.removeToolStrip(headToolStrip);
        }

        public void addControlToUI(Control control)
        {
            controller.MedicalForm.addLeftControl(control);
        }

        public void removeControl(Control control)
        {
            controller.MedicalForm.removeControl(control);
        }
    }
}
