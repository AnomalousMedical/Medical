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
        private StatePickerWizard wizard;
        private ImageList imageList;
#if USE_RIBBON
        StatePickerRibbon stateRibbon;
#else
        private StatePickerModeList modeList;
#endif

        public BasicStateWizardHost(BasicForm form)
        {
            this.form = form;
            topInformationPanel = form.topInformationPanel;
            leftInformationPanel = form.leftInformationPanel;
        }

        public void Dispose()
        {
#if USE_RIBBON
            stateRibbon.Dispose();
#else
            modeList.Dispose();
#endif
        }

        public void setStateWizardInfo(StatePickerWizard wizard, ImageList imageList)
        {
            this.wizard = wizard;
            this.imageList = imageList;

#if USE_RIBBON
            stateRibbon = new StatePickerRibbon(imageList, wizard);
            form.clinicalRibbon.RibbonTabs.Add(stateRibbon.RibbonTab);
            stateRibbon.RibbonTab.Visible = false;

#else
            modeList = new StatePickerModeList(imageList, wizard);
            setTopInformation(modeList);
#endif
        }

        public void setDataControl(Control control)
        {
            leftInformationPanel.Controls.Clear();
            if (control != null)
            {
                leftInformationPanel.Controls.Add(control);
                leftInformationPanel.Size = control.Size;
                control.Dock = DockStyle.Fill;
            }
        }

        public void addMode(StatePickerPanel mode)
        {
#if USE_RIBBON
            stateRibbon.addMode(mode);
#else
            modeList.addMode(mode);
#endif
        }

        public void updateImage(StatePickerPanel panel)
        {
#if USE_RIBBON
            stateRibbon.updateImage(panel);
#else
            modeList.updateImage(panel);
#endif
        }

        public int SelectedIndex
        {
            get
            {
#if USE_RIBBON
                return stateRibbon.SelectedIndex;
#else
                return modeList.SelectedIndex;
#endif
            }
            set
            {
#if USE_RIBBON
                stateRibbon.SelectedIndex = value;
#else
                modeList.SelectedIndex = value;
#endif
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
#if USE_RIBBON
                stateRibbon.RibbonTab.Visible = value;
                if (value)
                {
                    form.clinicalRibbon.SelectedTab = stateRibbon.RibbonTab;
                }
#else
                topInformationPanel.Visible = value;
#endif
            }
        }

#if !USE_RIBBON
        void setTopInformation(Control control)
        {
            topInformationPanel.Controls.Clear();
            if (control != null)
            {
                topInformationPanel.Controls.Add(control);
                topInformationPanel.Size = control.Size;
                control.Dock = DockStyle.Fill;
            }
        }
#endif
    }
}
