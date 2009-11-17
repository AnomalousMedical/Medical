using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ComponentFactory.Krypton.Navigator;
using ComponentFactory.Krypton.Toolkit;

namespace Medical.GUI
{
    public class BasicStateWizardHost : StatePickerUIHost
    {
        private BasicForm form;
        private KryptonPanel topInformationPanel;
        private KryptonPanel leftInformationPanel;

        public BasicStateWizardHost(BasicForm form)
        {
            this.form = form;
            topInformationPanel = form.topInformationPanel;
            leftInformationPanel = form.leftInformationPanel;
        }

        public void setLeftInformation(Control control)
        {
            leftInformationPanel.Controls.Clear();
            if (control != null)
            {
                leftInformationPanel.Controls.Add(control);
                leftInformationPanel.Size = control.Size;
                control.Dock = DockStyle.Fill;
                leftInformationPanel.Visible = true;
            }
            else
            {
                leftInformationPanel.Visible = false;
            }
        }

        public void setRightInformation(Control control)
        {
            
        }

        public void setTopInformation(Control control)
        {
            topInformationPanel.Controls.Clear();
            if (control != null)
            {
                topInformationPanel.Controls.Add(control);
                topInformationPanel.Size = control.Size;
                control.Dock = DockStyle.Fill;
                topInformationPanel.Visible = true;
            }
            else
            {
                topInformationPanel.Visible = false;
            }
        }

        public void setBottomInformation(Control control)
        {
            
        }
    }
}
