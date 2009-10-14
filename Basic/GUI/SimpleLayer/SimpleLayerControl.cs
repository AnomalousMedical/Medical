using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using Engine.ObjectManagement;
using Medical.Controller;

namespace Medical.GUI
{
    public partial class SimpleLayerControl : GUIElement
    {
        private List<TransparencyInterface> currentAlphaBlendInterfaces = new List<TransparencyInterface>();

        public SimpleLayerControl()
        {
            InitializeComponent();
            skinSlider.TransparencyChanged += changeSkinTransparency;
            skullSlider.TransparencyChanged += changeSkullTransparency;
            tmjSlider.TransparencyChanged += changeDiscTransparency;
            mandibleSlider.TransparencyChanged += changeMandibleTransparency;
            topTeethSlider.TransparencyChanged += changeTopToothTransparency;
            bottomTeethSlider.TransparencyChanged += changeBottomToothTransparency;
            muscleSlider.TransparencyChanged += changeMuscleTransparency;
            spineSlider.TransparencyChanged += changeSpineTransparency;
            hyoidSlider.TransparencyChanged += changeHyoidTransparency;
        }

        public void setupShortcuts(ShortcutController shortcutController)
        {
            ShortcutGroup group = shortcutController.createOrRetrieveGroup("LayerShortcuts");
            ShortcutEventCommand skinToggle = new ShortcutEventCommand("SkinToggle", Keys.Q, true);
            skinToggle.Execute += new ShortcutEventCommand.ExecuteEvent(skinToggle_Execute);
            group.addShortcut(skinToggle);

            ShortcutEventCommand skullToggle = new ShortcutEventCommand("SkullToggle", Keys.T, true);
            skullToggle.Execute += new ShortcutEventCommand.ExecuteEvent(skullToggle_Execute);
            group.addShortcut(skullToggle);

            ShortcutEventCommand eminenceToggle = new ShortcutEventCommand("EminenceToggle", Keys.E, true);
            eminenceToggle.Execute += new ShortcutEventCommand.ExecuteEvent(eminenceToggle_Execute);
            group.addShortcut(eminenceToggle);

            ShortcutEventCommand tmjToggle = new ShortcutEventCommand("TmjToggle", Keys.R, true);
            tmjToggle.Execute += new ShortcutEventCommand.ExecuteEvent(tmjToggle_Execute);
            group.addShortcut(tmjToggle);

            ShortcutEventCommand mandibleToggle = new ShortcutEventCommand("TmjToggle", Keys.Y, true);
            mandibleToggle.Execute += new ShortcutEventCommand.ExecuteEvent(mandibleToggle_Execute);
            group.addShortcut(mandibleToggle);

            ShortcutEventCommand topTeethToggle = new ShortcutEventCommand("TopTeethToggle", Keys.O, true);
            topTeethToggle.Execute += new ShortcutEventCommand.ExecuteEvent(topTeethToggle_Execute);
            group.addShortcut(topTeethToggle);

            ShortcutEventCommand bottomTeethToggle = new ShortcutEventCommand("BottomTeethToggle", Keys.P, true);
            bottomTeethToggle.Execute += new ShortcutEventCommand.ExecuteEvent(bottomTeethToggle_Execute);
            group.addShortcut(bottomTeethToggle);

            ShortcutEventCommand muscleToggle = new ShortcutEventCommand("MuscleToggle", Keys.W, true);
            muscleToggle.Execute += new ShortcutEventCommand.ExecuteEvent(muscleToggle_Execute);
            group.addShortcut(muscleToggle);

            ShortcutEventCommand spineToggle = new ShortcutEventCommand("SpineToggle", Keys.I, true);
            spineToggle.Execute += new ShortcutEventCommand.ExecuteEvent(spineToggle_Execute);
            group.addShortcut(spineToggle);

            ShortcutEventCommand hyoidToggle = new ShortcutEventCommand("HyoidToggle", Keys.U, true);
            hyoidToggle.Execute += new ShortcutEventCommand.ExecuteEvent(hyoidToggle_Execute);
            group.addShortcut(hyoidToggle);
        }

        void eminenceToggle_Execute()
        {
            showEminence.Checked = !showEminence.Checked;
        }

        void hyoidToggle_Execute()
        {
            hyoidSlider.toggle();
        }

        void spineToggle_Execute()
        {
            spineSlider.toggle();
        }

        void muscleToggle_Execute()
        {
            muscleSlider.toggle();
        }

        void bottomTeethToggle_Execute()
        {
            bottomTeethSlider.toggle();
        }

        void topTeethToggle_Execute()
        {
            topTeethSlider.toggle();
        }

        void mandibleToggle_Execute()
        {
            mandibleSlider.toggle();
        }

        void tmjToggle_Execute()
        {
            tmjSlider.toggle();
        }

        void skullToggle_Execute()
        {
            skullSlider.toggle();
        }

        void skinToggle_Execute()
        {
            skinSlider.toggle();
        }

        protected override void sceneLoaded(SimScene scene)
        {

        }

        void changeHyoidTransparency(float alpha)
        {
            TransparencyGroup group = TransparencyController.getTransparencyGroup(RenderGroup.Bones);
            TransparencyInterface skull = group.getTransparencyObject("Hyoid");
            skull.smoothBlend(alpha);
        }

        private void changeSkullTransparency(float alpha)
        {
            TransparencyGroup group = TransparencyController.getTransparencyGroup(RenderGroup.Bones);
            TransparencyInterface skull = group.getTransparencyObject("Skull");
            skull.smoothBlend(alpha);
            TransparencyInterface skullInterior = group.getTransparencyObject("Skull Interior");
            skullInterior.smoothBlend(alpha);
            if (showEminence.Checked)
            {
                TransparencyInterface leftEminence = group.getTransparencyObject("Left Eminence");
                TransparencyInterface rightEminence = group.getTransparencyObject("Right Eminence");
                leftEminence.smoothBlend(alpha);
                rightEminence.smoothBlend(alpha);
            }
            TransparencyInterface maxillarySinus = group.getTransparencyObject("Maxillary Sinus");
            maxillarySinus.smoothBlend(alpha);
        }

        private void changeDiscTransparency(float alpha)
        {
            TransparencyGroup group = TransparencyController.getTransparencyGroup(RenderGroup.TMJ);
            TransparencyInterface leftDisc = group.getTransparencyObject("Left TMJ Disc");
            leftDisc.smoothBlend(alpha);
            TransparencyInterface rightDisc = group.getTransparencyObject("Right TMJ Disc");
            rightDisc.smoothBlend(alpha);
        }

        private void changeMandibleTransparency(float alpha)
        {
            TransparencyGroup group = TransparencyController.getTransparencyGroup(RenderGroup.Bones);
            TransparencyInterface skull = group.getTransparencyObject("Mandible");
            skull.smoothBlend(alpha);
        }

        private void changeTopToothTransparency(float alpha)
        {
            TransparencyGroup group = TransparencyController.getTransparencyGroup(RenderGroup.Teeth);
            for (int i = 1; i < 17; ++i)
            {
                group.getTransparencyObject("Tooth " + i).smoothBlend(alpha);
            }
        }

        private void changeBottomToothTransparency(float alpha)
        {
            TransparencyGroup group = TransparencyController.getTransparencyGroup(RenderGroup.Teeth);
            for (int i = 17; i < 33; ++i)
            {
                group.getTransparencyObject("Tooth " + i).smoothBlend(alpha);
            }
        }

        private void changeSkinTransparency(float alpha)
        {
            TransparencyGroup group = TransparencyController.getTransparencyGroup(RenderGroup.Skin);
            TransparencyInterface skin = group.getTransparencyObject("Skin");
            skin.smoothBlend(alpha);
            TransparencyInterface leftEye = group.getTransparencyObject("Left Eye");
            leftEye.smoothBlend(alpha);
            TransparencyInterface rightEye = group.getTransparencyObject("Right Eye");
            rightEye.smoothBlend(alpha);
            TransparencyInterface eyebrowsAndEyelashes = group.getTransparencyObject("Eyebrows and Eyelashes");
            eyebrowsAndEyelashes.smoothBlend(alpha);
        }

        private void changeMuscleTransparency(float alpha)
        {
            TransparencyGroup group = TransparencyController.getTransparencyGroup(RenderGroup.Muscles);
            foreach (TransparencyInterface item in group.getTransparencyObjectIter())
            {
                item.smoothBlend(alpha);
            }
        }

        private void changeSpineTransparency(float alpha)
        {
            TransparencyGroup group = TransparencyController.getTransparencyGroup(RenderGroup.Spine);
            foreach (TransparencyInterface item in group.getTransparencyObjectIter())
            {
                item.smoothBlend(alpha);
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

        //private void eminenceVisible_CheckedChanged(object sender, EventArgs e)
        //{
        //    
        //}

        //private void eminenceHidden_CheckedChanged(object sender, EventArgs e)
        //{
        //    
        //}
    }
}
