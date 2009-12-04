#define USE_RIBBON

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
        private StatePickerRibbon stateRibbon;
        private StatePickerWizard currentWizard;

        public BasicStateWizardHost(BasicForm form)
        {
            this.form = form;
            topInformationPanel = form.topInformationPanel;
            leftInformationPanel = form.leftInformationPanel;

            stateRibbon = new StatePickerRibbon();
            form.clinicalRibbon.RibbonTabs.Add(stateRibbon.RibbonTab);
            stateRibbon.RibbonTab.Visible = false;
            stateRibbon.SelectedIndexChanged += new EventHandler(stateRibbon_SelectedIndexChanged);
        }

        void stateRibbon_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentWizard.modeChanged(stateRibbon.SelectedIndex);
        }

        public void Dispose()
        {
            stateRibbon.Dispose();
        }

        public void setCurrentWizard(StatePickerWizard wizard)
        {
            currentWizard = wizard;
            leftInformationPanel.Controls.Clear();
            Control control = wizard.WizardControl;
            if (control != null)
            {
                leftInformationPanel.Controls.Add(control);
                leftInformationPanel.Size = control.Size;
                control.Dock = DockStyle.Fill;
            }
        }

        public void addMode(StatePickerPanel mode)
        {
            stateRibbon.addMode(mode);
        }

        public void clearModes()
        {
            stateRibbon.clearModes();
        }

        public int SelectedIndex
        {
            get
            {
                return stateRibbon.SelectedIndex;
            }
            set
            {
                stateRibbon.SelectedIndex = value;
            }
        }

        public bool Visible
        {
            get
            {
                return leftInformationPanel.Visible;
            }
            set
            {
                leftInformationPanel.Visible = value;
                stateRibbon.RibbonTab.Visible = value;
                if (value)
                {
                    form.clinicalRibbon.SelectedTab = stateRibbon.RibbonTab;
                }
            }
        }
    }
}
