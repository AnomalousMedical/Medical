using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ComponentFactory.Krypton.Ribbon;

namespace Medical.GUI
{
    public class StatePickerRibbon : IDisposable
    {
        private StatePickerWizard stateController;
        private ImageList imageList;
        private KryptonRibbonTab statePickerTab;
        private KryptonRibbonGroup ribbonGroup;
        private KryptonRibbonGroupTriple currentTriple;
        private int numModes = 0;
        private Dictionary<StatePickerPanel, KryptonRibbonGroupButton> panelButtons = new Dictionary<StatePickerPanel,KryptonRibbonGroupButton>();
        private int selectedIndex = 0;
        private KryptonRibbonGroupButton lastButton;

        public StatePickerRibbon(ImageList imageList, StatePickerWizard stateController)
        {
            statePickerTab = new KryptonRibbonTab();
            statePickerTab.Text = "State Wizard";
            ribbonGroup = new KryptonRibbonGroup();
            ribbonGroup.TextLine1 = " ";
            statePickerTab.Groups.Add(ribbonGroup);
            ribbonGroup.AllowCollapsed = false;

            this.imageList = imageList;
            this.stateController = stateController;
        }

        public void Dispose()
        {
            statePickerTab.Dispose();
        }

        public void addMode(StatePickerPanel panel)
        {
            if (currentTriple == null || currentTriple.Items.Count == 3)
            {
                currentTriple = new KryptonRibbonGroupTriple();
                ribbonGroup.Items.Add(currentTriple);
                currentTriple.MinimumSize = GroupItemSize.Large;
            }
            KryptonRibbonGroupButton button = new KryptonRibbonGroupButton();
            button.TextLine1 = panel.Text;
            button.Tag = numModes++;
            button.ButtonType = GroupButtonType.Check;
            button.Click += new EventHandler(button_Click);
            panelButtons.Add(panel, button);
            currentTriple.Items.Add(button);
        }

        public void updateImage(StatePickerPanel panel)
        {
            KryptonRibbonGroupButton button = panelButtons[panel];
            if (button.ImageLarge != null)
            {
                button.ImageLarge.Dispose();
            }
            button.ImageLarge = imageList.Images[panel.NavigationImageKey];
        }

        public int SelectedIndex
        {
            get
            {
                return selectedIndex;
            }
            set
            {
                selectedIndex = value;
                foreach (KryptonRibbonGroupButton button in panelButtons.Values)
                {
                    if ((int)button.Tag == selectedIndex)
                    {
                        button.Checked = true;
                        synchronizeButtons(button);
                        break;
                    }
                }
            }
        }

        public KryptonRibbonTab RibbonTab
        {
            get
            {
                return statePickerTab;
            }
        }

        void button_Click(object sender, EventArgs e)
        {
            KryptonRibbonGroupButton button = sender as KryptonRibbonGroupButton;
            synchronizeButtons(button);
            selectedIndex = (int)button.Tag;
            stateController.modeChanged((int)button.Tag);
        }

        void synchronizeButtons(KryptonRibbonGroupButton newButton)
        {
            if (newButton != lastButton)
            {
                if (lastButton != null)
                {
                    lastButton.Checked = false;
                }
                lastButton = newButton;
            }
        }
    }
}
