﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using Engine.ObjectManagement;

namespace Medical.GUI
{
    public partial class SimpleLayerControl : GUIElement
    {
        private List<TransparencyInterface> currentAlphaBlendInterfaces = new List<TransparencyInterface>();

        public SimpleLayerControl()
        {
            InitializeComponent();
        }



        protected override void sceneLoaded(SimScene scene)
        {
            skullOpaque.Checked = true;
            mandibleOpaque.Checked = true;
            topTeethOpaque.Checked = true;
            bottomTeethOpaque.Checked = true;
            eminenceVisible.Checked = true;
            skullInteriorOpaque.Checked = true;
            skinOpaque.Checked = true;
        }

        private void skullOpaque_CheckedChanged(object sender, EventArgs e)
        {
            if (skullOpaque.Checked)
            {
                TransparencyGroup group = TransparencyController.getTransparencyGroup(RenderGroup.Bones);
                TransparencyInterface skull = group.getTransparencyObject("Skull");
                skull.smoothBlend(1.0f);
                if (eminenceVisible.Checked)
                {
                    TransparencyInterface leftEminence = group.getTransparencyObject("Left Eminence");
                    TransparencyInterface rightEminence = group.getTransparencyObject("Right Eminence");
                    leftEminence.smoothBlend(1.0f);
                    rightEminence.smoothBlend(1.0f);
                }
            }
        }

        private void skullTransparent_CheckedChanged(object sender, EventArgs e)
        {
            if (skullTransparent.Checked)
            {
                TransparencyGroup group = TransparencyController.getTransparencyGroup(RenderGroup.Bones);
                TransparencyInterface skull = group.getTransparencyObject("Skull");
                skull.smoothBlend(0.7f);
                if (eminenceVisible.Checked)
                {
                    TransparencyInterface leftEminence = group.getTransparencyObject("Left Eminence");
                    TransparencyInterface rightEminence = group.getTransparencyObject("Right Eminence");
                    leftEminence.smoothBlend(0.7f);
                    rightEminence.smoothBlend(0.7f);
                }
            }
        }

        private void skullHidden_CheckedChanged(object sender, EventArgs e)
        {
            if (skullHidden.Checked)
            {
                TransparencyGroup group = TransparencyController.getTransparencyGroup(RenderGroup.Bones);
                TransparencyInterface skull = group.getTransparencyObject("Skull");
                skull.smoothBlend(0.0f);
                if (eminenceVisible.Checked)
                {
                    TransparencyInterface leftEminence = group.getTransparencyObject("Left Eminence");
                    TransparencyInterface rightEminence = group.getTransparencyObject("Right Eminence");
                    leftEminence.smoothBlend(0.0f);
                    rightEminence.smoothBlend(0.0f);
                }
            }
        }

        private void skullInteriorOpaque_CheckedChanged(object sender, EventArgs e)
        {
            if (skullInteriorOpaque.Checked)
            {
                TransparencyGroup group = TransparencyController.getTransparencyGroup(RenderGroup.Bones);
                TransparencyInterface skull = group.getTransparencyObject("Skull Interior");
                skull.smoothBlend(1.0f);
            }
        }

        private void skullInteriorTransparent_CheckedChanged(object sender, EventArgs e)
        {
            if (skullInteriorTransparent.Checked)
            {
                TransparencyGroup group = TransparencyController.getTransparencyGroup(RenderGroup.Bones);
                TransparencyInterface skull = group.getTransparencyObject("Skull Interior");
                skull.smoothBlend(0.7f);
            }
        }

        private void skullInteriorHidden_CheckedChanged(object sender, EventArgs e)
        {
            if (skullInteriorHidden.Checked)
            {
                TransparencyGroup group = TransparencyController.getTransparencyGroup(RenderGroup.Bones);
                TransparencyInterface skull = group.getTransparencyObject("Skull Interior");
                skull.smoothBlend(0.0f);
            }
        }

        private void eminenceVisible_CheckedChanged(object sender, EventArgs e)
        {
            if (eminenceVisible.Checked)
            {
                TransparencyGroup group = TransparencyController.getTransparencyGroup(RenderGroup.Bones);
                TransparencyInterface skull = group.getTransparencyObject("Skull");
                TransparencyInterface leftEminence = group.getTransparencyObject("Left Eminence");
                TransparencyInterface rightEminence = group.getTransparencyObject("Right Eminence");
                if (skullOpaque.Checked)
                {
                    leftEminence.smoothBlend(1.0f);
                    rightEminence.smoothBlend(1.0f);
                }
                if (skullTransparent.Checked)
                {
                    leftEminence.smoothBlend(0.7f);
                    rightEminence.smoothBlend(0.7f);
                }
                if (skullHidden.Checked)
                {
                    leftEminence.smoothBlend(0.0f);
                    rightEminence.smoothBlend(0.0f);
                }
            }
        }

        private void eminenceHidden_CheckedChanged(object sender, EventArgs e)
        {
            if (eminenceHidden.Checked)
            {
                TransparencyGroup group = TransparencyController.getTransparencyGroup(RenderGroup.Bones);
                TransparencyInterface leftEminence = group.getTransparencyObject("Left Eminence");
                TransparencyInterface rightEminence = group.getTransparencyObject("Right Eminence");
                leftEminence.smoothBlend(0.0f);
                rightEminence.smoothBlend(0.0f);
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

        private void skinOpaque_CheckedChanged(object sender, EventArgs e)
        {
            if (skinOpaque.Checked)
            {
                TransparencyGroup group = TransparencyController.getTransparencyGroup(RenderGroup.Skin);
                TransparencyInterface skin = group.getTransparencyObject("Skin");
                skin.smoothBlend(1.0f);
                TransparencyInterface leftEye = group.getTransparencyObject("Left Eye");
                leftEye.smoothBlend(1.0f);
                TransparencyInterface rightEye = group.getTransparencyObject("Right Eye");
                rightEye.smoothBlend(1.0f);
            }
        }

        private void skinTransparent_CheckedChanged(object sender, EventArgs e)
        {
            if (skinTransparent.Checked)
            {
                TransparencyGroup group = TransparencyController.getTransparencyGroup(RenderGroup.Skin);
                TransparencyInterface skin = group.getTransparencyObject("Skin");
                skin.smoothBlend(0.7f);
                TransparencyInterface leftEye = group.getTransparencyObject("Left Eye");
                leftEye.smoothBlend(0.7f);
                TransparencyInterface rightEye = group.getTransparencyObject("Right Eye");
                rightEye.smoothBlend(0.7f);
            }
        }

        private void skinHidden_CheckedChanged(object sender, EventArgs e)
        {
            if (skinHidden.Checked)
            {
                TransparencyGroup group = TransparencyController.getTransparencyGroup(RenderGroup.Skin);
                TransparencyInterface skin = group.getTransparencyObject("Skin");
                skin.smoothBlend(0.0f);
                TransparencyInterface leftEye = group.getTransparencyObject("Left Eye");
                leftEye.smoothBlend(0.0f);
                TransparencyInterface rightEye = group.getTransparencyObject("Right Eye");
                rightEye.smoothBlend(0.0f);
            }
        }
    }
}
