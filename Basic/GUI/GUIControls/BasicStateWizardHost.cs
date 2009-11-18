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
        private StatePickerModeList modeList;

        public BasicStateWizardHost(BasicForm form)
        {
            this.form = form;
            topInformationPanel = form.topInformationPanel;
            leftInformationPanel = form.leftInformationPanel;
        }

        public void Dispose()
        {
            modeList.Dispose();
        }

        public void setStateWizardInfo(StatePickerWizard wizard, ImageList imageList)
        {
            this.wizard = wizard;
            this.imageList = imageList;

            //temp
            modeList = new StatePickerModeList(imageList, wizard);
            setTopInformation(modeList);
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
            modeList.addMode(mode);
        }

        public void updateImage(StatePickerPanel panel)
        {
            modeList.updateImage(panel);
        }

        public int SelectedIndex
        {
            get
            {
                return modeList.SelectedIndex;
            }
            set
            {
                modeList.SelectedIndex = value;
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
                topInformationPanel.Visible = value;
            }
        }

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
    }
}
