using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

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
            controller.addToolStrip(headToolStrip);
        }

        /// <summary>
        /// Used when restoring window positions. Return the window matching the
        /// persistString or null if no match is found.
        /// </summary>
        /// <param name="persistString">A string describing the window.</param>
        /// <returns>The matching DockContent or null if none is found.</returns>
        public DockContent getDockContent(String persistString)
        {
            return null;
        }

        public void destroy()
        {
            controller.removeToolStrip(headToolStrip);
            headToolStrip.Dispose();
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
