using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace Medical.GUI
{
    public partial class SimpleLayerControl : GUIElement
    {
        private List<TransparencyInterface> currentAlphaBlendInterfaces = new List<TransparencyInterface>();

        public SimpleLayerControl()
        {
            InitializeComponent();
        }

        

        protected override void sceneLoaded()
        {
            base.sceneLoaded();
            skullOpaque.Checked = true;
            mandibleOpaque.Checked = true;
            topTeethOpaque.Checked = true;
            bottomTeethOpaque.Checked = true;
        }

        private void skullOpaque_CheckedChanged(object sender, EventArgs e)
        {
            if (skullOpaque.Checked)
            {
                TransparencyGroup group = TransparencyController.getTransparencyGroup(RenderGroup.Bones);
                TransparencyInterface skull = group.getTransparencyObject("Skull");
                skull.smoothBlend(1.0f);
            }
        }

        private void skullTransparent_CheckedChanged(object sender, EventArgs e)
        {
            if (skullTransparent.Checked)
            {
                TransparencyGroup group = TransparencyController.getTransparencyGroup(RenderGroup.Bones);
                TransparencyInterface skull = group.getTransparencyObject("Skull");
                skull.smoothBlend(0.7f);
            }
        }

        private void skullHidden_CheckedChanged(object sender, EventArgs e)
        {
            if (skullHidden.Checked)
            {
                TransparencyGroup group = TransparencyController.getTransparencyGroup(RenderGroup.Bones);
                TransparencyInterface skull = group.getTransparencyObject("Skull");
                skull.smoothBlend(0.0f);
            }
        }

        private void mandibleOpaque_CheckedChanged(object sender, EventArgs e)
        {
            if (mandibleOpaque.Checked)
            {
                TransparencyGroup group = TransparencyController.getTransparencyGroup(RenderGroup.Bones);
                TransparencyInterface skull = group.getTransparencyObject("Mandible");
                skull.smoothBlend(1.0f);
            }
        }

        private void mandibleTransparent_CheckedChanged(object sender, EventArgs e)
        {
            if (mandibleTransparent.Checked)
            {
                TransparencyGroup group = TransparencyController.getTransparencyGroup(RenderGroup.Bones);
                TransparencyInterface skull = group.getTransparencyObject("Mandible");
                skull.smoothBlend(0.7f);
            }
        }

        private void mandibleHidden_CheckedChanged(object sender, EventArgs e)
        {
            if (mandibleHidden.Checked)
            {
                TransparencyGroup group = TransparencyController.getTransparencyGroup(RenderGroup.Bones);
                TransparencyInterface skull = group.getTransparencyObject("Mandible");
                skull.smoothBlend(0.0f);
            }
        }

        private void teethOpaque_CheckedChanged(object sender, EventArgs e)
        {
            if (topTeethOpaque.Checked)
            {
                TransparencyGroup group = TransparencyController.getTransparencyGroup(RenderGroup.Teeth);
                for( int i = 1; i < 17; ++i)
                {
                    group.getTransparencyObject("Tooth " + i).smoothBlend(1.0f);
                }
            }
        }

        private void teethTransparent_CheckedChanged(object sender, EventArgs e)
        {
            if (topTeethTransparent.Checked)
            {
                TransparencyGroup group = TransparencyController.getTransparencyGroup(RenderGroup.Teeth);
                for (int i = 1; i < 17; ++i)
                {
                    group.getTransparencyObject("Tooth " + i).smoothBlend(0.7f);
                }
            }
        }

        private void teethHidden_CheckedChanged(object sender, EventArgs e)
        {
            if (topTeethHidden.Checked)
            {
                TransparencyGroup group = TransparencyController.getTransparencyGroup(RenderGroup.Teeth);
                for (int i = 1; i < 17; ++i)
                {
                    group.getTransparencyObject("Tooth " + i).smoothBlend(0.0f);
                }
            }
        }

        private void bottomTeethOpaque_CheckedChanged(object sender, EventArgs e)
        {
            if (bottomTeethOpaque.Checked)
            {
                TransparencyGroup group = TransparencyController.getTransparencyGroup(RenderGroup.Teeth);
                for (int i = 17; i < 33; ++i)
                {
                    group.getTransparencyObject("Tooth " + i).smoothBlend(1.0f);
                }
            }
        }

        private void bottomTeethTransparent_CheckedChanged(object sender, EventArgs e)
        {
            if (bottomTeethTransparent.Checked)
            {
                TransparencyGroup group = TransparencyController.getTransparencyGroup(RenderGroup.Teeth);
                for (int i = 17; i < 33; ++i)
                {
                    group.getTransparencyObject("Tooth " + i).smoothBlend(0.7f);
                }
            }
        }

        private void bottomTeethHidden_CheckedChanged(object sender, EventArgs e)
        {
            if (bottomTeethHidden.Checked)
            {
                TransparencyGroup group = TransparencyController.getTransparencyGroup(RenderGroup.Teeth);
                for (int i = 17; i < 33; ++i)
                {
                    group.getTransparencyObject("Tooth " + i).smoothBlend(0.0f);
                }
            }
        }
    }
}
