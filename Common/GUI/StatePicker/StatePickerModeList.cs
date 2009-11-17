using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Medical.GUI
{
    public partial class StatePickerModeList : UserControl
    {
        private StatePickerWizard stateController;

        public StatePickerModeList(ImageList imageList, StatePickerWizard stateController)
        {
            InitializeComponent();
            navigatorList.SelectedIndexChanged += new EventHandler(navigatorList_SelectedIndexChanged);
            navigatorList.LargeImageList = imageList;

            this.stateController = stateController;
        }

        public void addMode(StatePickerPanel panel)
        {
            ListViewItem item = navigatorList.Items.Add(panel.Text, panel.Text, panel.Text);
            item.Tag = panel;
        }

        public void updateImage(StatePickerPanel panel)
        {
            ListViewItem item = navigatorList.Items[panel.Text];
            item.ImageKey = panel.NavigationImageKey;
        }

        public int SelectedIndex
        {
            get
            {
                if (navigatorList.SelectedIndices.Count > 0)
                {
                    return navigatorList.SelectedIndices[0];
                }
                else
                {
                    return 0;
                }
            }
            set
            {
                if (value < navigatorList.Items.Count)
                {
                    navigatorList.SelectedIndices.Clear();
                    navigatorList.SelectedIndices.Add(value);
                }
            }
        }

        void navigatorList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (navigatorList.SelectedIndices.Count > 0)
            {
                stateController.modeChanged(navigatorList.SelectedIndices[0]);
            }
        }
    }
}
