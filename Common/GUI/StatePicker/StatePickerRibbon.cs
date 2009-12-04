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
        public event EventHandler SelectedIndexChanged;

        private KryptonRibbonTab statePickerTab;
        private KryptonRibbonGroup ribbonGroup;
        private KryptonRibbonGroupTriple currentTriple;
        private int numModes = 0;
        private Dictionary<StatePickerPanel, KryptonRibbonGroupButton> panelButtons = new Dictionary<StatePickerPanel,KryptonRibbonGroupButton>();
        private int selectedIndex = 0;
        private KryptonRibbonGroupButton lastButton;

        public StatePickerRibbon()
        {
            statePickerTab = new KryptonRibbonTab();
            statePickerTab.Text = "State Wizard";
            ribbonGroup = new KryptonRibbonGroup();
            ribbonGroup.TextLine1 = " ";
            ribbonGroup.DialogBoxLauncher = false;
            statePickerTab.Groups.Add(ribbonGroup);
            ribbonGroup.AllowCollapsed = false;
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
            button.TextLine1 = panel.TextLine1;
            button.TextLine2 = panel.TextLine2;
            button.ImageLarge = panel.LargeIcon;
            button.Tag = numModes++;
            button.ButtonType = GroupButtonType.Check;
            button.Click += new EventHandler(button_Click);
            panelButtons.Add(panel, button);
            currentTriple.Items.Add(button);
        }

        public void clearModes()
        {
            currentTriple = null;
            ribbonGroup.Items.Clear();
            panelButtons.Clear();
            numModes = 0;
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
            if (SelectedIndexChanged != null)
            {
                SelectedIndexChanged.Invoke(this, EventArgs.Empty);
            }
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
