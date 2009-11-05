using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ComponentFactory.Krypton.Ribbon;
using ComponentFactory.Krypton.Toolkit;

namespace Medical.GUI
{
    class LayerGUISkullMenu : LayerGUIMenu
    {
        KryptonContextMenuCheckBox showEminence;

        public LayerGUISkullMenu(KryptonRibbonGroupButton mainButton)
            :base(mainButton)
        {
            KryptonContextMenuSeparator separator = new KryptonContextMenuSeparator();
            separator.Horizontal = false;
            contextMenu.Items.Add(separator);

            KryptonContextMenuHeading header = new KryptonContextMenuHeading("Cutouts");
            contextMenu.Items.Add(header);

            showEminence = new KryptonContextMenuCheckBox("Show Eminence");
            showEminence.Checked = true;
            showEminence.CheckedChanged += new EventHandler(showEminence_CheckedChanged);
            contextMenu.Items.Add(showEminence);
        }

        public bool ShowEminance
        {
            get
            {
                return showEminence.Checked;
            }
            set
            {
                showEminence.Checked = value;
            }
        }

        private void showEminence_CheckedChanged(object sender, EventArgs e)
        {
            if (showEminence.Checked)
            {
                TransparencyGroup group = TransparencyController.getTransparencyGroup(RenderGroup.Bones);
                TransparencyInterface skull = group.getTransparencyObject("Skull");
                TransparencyInterface leftEminence = group.getTransparencyObject("Left Eminence");
                TransparencyInterface rightEminence = group.getTransparencyObject("Right Eminence");
                leftEminence.smoothBlend(skull.CurrentAlpha);
                rightEminence.smoothBlend(skull.CurrentAlpha);
            }
            else
            {
                TransparencyGroup group = TransparencyController.getTransparencyGroup(RenderGroup.Bones);
                TransparencyInterface leftEminence = group.getTransparencyObject("Left Eminence");
                TransparencyInterface rightEminence = group.getTransparencyObject("Right Eminence");
                leftEminence.smoothBlend(0.0f);
                rightEminence.smoothBlend(0.0f);
            }
        }
    }
}
