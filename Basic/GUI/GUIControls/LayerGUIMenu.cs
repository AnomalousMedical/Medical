using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ComponentFactory.Krypton.Ribbon;
using ComponentFactory.Krypton.Toolkit;

namespace Medical.GUI
{
    public delegate void ChangeTransparency(float value);

    class LayerGUIMenu : IDisposable
    {
        public event ChangeTransparency TransparencyChanged;

        protected KryptonRibbonGroupButton mainButton;
        protected KryptonContextMenu contextMenu;
        protected KryptonContextMenuRadioButton opaqueButton;
        protected KryptonContextMenuRadioButton transparentButton;
        protected KryptonContextMenuRadioButton hiddenButton;

        public LayerGUIMenu(KryptonRibbonGroupButton mainButton)
        {
            contextMenu = new KryptonContextMenu();
            
            KryptonContextMenuHeading visibilityHeading = new KryptonContextMenuHeading("Visibility");
            contextMenu.Items.Add(visibilityHeading);
            
            opaqueButton = new KryptonContextMenuRadioButton("Opaque");
            opaqueButton.Checked = true;
            opaqueButton.AutoClose = true;
            opaqueButton.CheckedChanged += new EventHandler(opaqueButton_CheckedChanged);
            contextMenu.Items.Add(opaqueButton);

            transparentButton = new KryptonContextMenuRadioButton("Transparent");
            transparentButton.AutoClose = true;
            transparentButton.CheckedChanged += new EventHandler(transparentButton_CheckedChanged);
            contextMenu.Items.Add(transparentButton);

            hiddenButton = new KryptonContextMenuRadioButton("Hidden");
            hiddenButton.AutoClose = true;
            hiddenButton.CheckedChanged += new EventHandler(hiddenButton_CheckedChanged);
            contextMenu.Items.Add(hiddenButton);

            this.mainButton = mainButton;
            mainButton.Click += new EventHandler(mainButton_Click);
            mainButton.KryptonContextMenu = contextMenu;
        }

        public void Dispose()
        {
            contextMenu.Dispose();
        }

        void mainButton_Click(object sender, EventArgs e)
        {
            if (opaqueButton.Checked)
            {
                transparentButton.Checked = true;
            }
            else if (transparentButton.Checked)
            {
                hiddenButton.Checked = true;
            }
            else if (hiddenButton.Checked)
            {
                opaqueButton.Checked = true;
            }
        }

        void opaqueButton_CheckedChanged(object sender, EventArgs e)
        {
            if (opaqueButton.Checked && TransparencyChanged != null)
            {
                TransparencyChanged(1.0f);
            }
        }

        void transparentButton_CheckedChanged(object sender, EventArgs e)
        {
            if (transparentButton.Checked && TransparencyChanged != null)
            {
                TransparencyChanged(0.7f);
            }
        }

        void hiddenButton_CheckedChanged(object sender, EventArgs e)
        {
            if (hiddenButton.Checked && TransparencyChanged != null)
            {
                TransparencyChanged(0.0f);
            }
        }
    }
}
