using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OgreWrapper;
using OgrePlugin;
using ComponentFactory.Krypton.Toolkit;

namespace Medical.GUI
{
    public partial class Options : KryptonForm
    {
        public Options()
        {
            InitializeComponent();
            this.AllowFormChrome = !WindowsInfo.CompositionEnabled;
            tooltip.SetToolTip(antiAliasingCombo, "This option smooths out jagged edges. However, it comes with a performance penalty so it may cause the program to run more slowly.");
            tooltip.SetToolTip(vsyncCheck, "This option will lock the refresh of the 3d scene to the monitor refresh. This will slow down the program causing it to use less resources.");
            Dictionary<String, ConfigOption> configOptions = Root.getSingleton().getRenderSystem().getConfigOptions();
            if (configOptions.ContainsKey("Anti aliasing"))
            {
                antiAliasingCombo.Items.Add("0");
                foreach (String value in configOptions["Anti aliasing"].PossibleValues)
                {
                    if (value.StartsWith("Level "))
                    {
                        antiAliasingCombo.Items.Add(value.Replace("Level ", ""));
                    }
                }
                antiAliasingCombo.SelectedItem = OgreConfig.FSAA;
            }
            else
            {
                antiAliasingCombo.Enabled = false;
            }
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            antiAliasingCombo.SelectedItem = OgreConfig.FSAA.ToString();
            vsyncCheck.Checked = OgreConfig.VSync;
        }

        private void applyButton_Click(object sender, EventArgs e)
        {
            OgreConfig.VSync = vsyncCheck.Checked;
            int value = 0;
            int.TryParse(antiAliasingCombo.SelectedItem.ToString(), out value);
            OgreConfig.FSAA = value;
            this.DialogResult = DialogResult.OK;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
    }
}
