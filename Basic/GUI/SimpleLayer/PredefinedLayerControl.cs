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
    public partial class PredefinedLayerControl : GUIElement
    {
        public PredefinedLayerControl()
        {
            InitializeComponent();
        }

        private void skinButton_CheckedChanged(object sender, EventArgs e)
        {
            if (skinButton.Checked)
            {
                setAlphas(1.0f, 1.0f, 1.0f, 1.0f, 1.0f);
            }
        }

        private void transparentSkinButton_CheckedChanged(object sender, EventArgs e)
        {
            if (transparentSkinButton.Checked)
            {
                setAlphas(0.7f, 1.0f, 1.0f, 1.0f, 1.0f);
            }
        }

        private void bonesButton_CheckedChanged(object sender, EventArgs e)
        {
            if (bonesButton.Checked)
            {
                setAlphas(0.0f, 1.0f, 1.0f, 1.0f, 1.0f);
            }
        }

        private void fossaCutAwayButton_CheckedChanged(object sender, EventArgs e)
        {
            if (fossaCutAwayButton.Checked)
            {
                setAlphas(0.0f, 1.0f, 0.0f, 1.0f, 1.0f);
            }
        }

        private void transparentBonesButton_CheckedChanged(object sender, EventArgs e)
        {
            if (transparentBonesButton.Checked)
            {
                setAlphas(0.0f, 0.7f, 0.7f, 1.0f, 1.0f);
            }
        }

        private void teethButton_CheckedChanged(object sender, EventArgs e)
        {
            if (teethButton.Checked)
            {
                setAlphas(0.0f, 0.0f, 0.0f, 1.0f, 1.0f);
            }
        }

        private void topTeethButton_CheckedChanged(object sender, EventArgs e)
        {
            if (topTeethButton.Checked)
            {
                setAlphas(0.0f, 0.0f, 0.0f, 1.0f, 0.0f);
            }
        }

        private void bottomTeethButton_CheckedChanged(object sender, EventArgs e)
        {
            if (bottomTeethButton.Checked)
            {
                setAlphas(0.0f, 0.0f, 0.0f, 0.0f, 1.0f);
            }
        }

        private void setAlphas(float skinAlpha, float bonesAlpha, float eminanceAlpha, float topTeethAlpha, float bottomTeethAlpha)
        {
            TransparencyGroup group = TransparencyController.getTransparencyGroup(RenderGroup.Bones);
            TransparencyInterface skull = group.getTransparencyObject("Skull");
            skull.smoothBlend(bonesAlpha);
            TransparencyInterface leftEminence = group.getTransparencyObject("Left Eminence");
            TransparencyInterface rightEminence = group.getTransparencyObject("Right Eminence");
            leftEminence.smoothBlend(eminanceAlpha);
            rightEminence.smoothBlend(eminanceAlpha);
            TransparencyInterface skullInterior = group.getTransparencyObject("Skull Interior");
            skullInterior.smoothBlend(bonesAlpha);
            TransparencyInterface mandible = group.getTransparencyObject("Mandible");
            mandible.smoothBlend(bonesAlpha);
            group = TransparencyController.getTransparencyGroup(RenderGroup.Teeth);
            for (int i = 1; i < 17; ++i)
            {
                group.getTransparencyObject("Tooth " + i).smoothBlend(topTeethAlpha);
            }
            for (int i = 17; i < 33; ++i)
            {
                group.getTransparencyObject("Tooth " + i).smoothBlend(bottomTeethAlpha);
            }
            group = TransparencyController.getTransparencyGroup(RenderGroup.Skin);
            TransparencyInterface skin = group.getTransparencyObject("Skin");
            skin.smoothBlend(skinAlpha);
            TransparencyInterface leftEye = group.getTransparencyObject("Left Eye");
            leftEye.smoothBlend(skinAlpha);
            TransparencyInterface rightEye = group.getTransparencyObject("Right Eye");
            rightEye.smoothBlend(skinAlpha);
            TransparencyInterface eyebrowsAndEyelashes = group.getTransparencyObject("Eyebrows and Eyelashes");
            eyebrowsAndEyelashes.smoothBlend(skinAlpha);
        }
    }
}
