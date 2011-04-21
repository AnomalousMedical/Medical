using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine.Platform;

namespace Medical.GUI
{
    class LayersDialog : Dialog
    {
        private LayerGUIMenu skinMenu;
        private LayerGUIMenu nervesMenu;
        private LayerGUIMenu musclesMenu;
        private LayerGUISkullMenu skullMenu;
        private LayerGUIMenu mandibleMenu;
        private LayerGUIMenu discsMenu;
        private LayerGUIMenu spineMenu;
        private LayerGUIMenu hyoidMenu;
        private LayerGUIMenu topTeethMenu;
        private LayerGUIMenu bottomTeethMenu;

        private LayerController layerController;

        public LayersDialog(LayerController layerController)
            :base("Medical.GUI.Layers.LayersDialog.layout")
        {
            //Predefined layers
            this.layerController = layerController;

            layerController.LayerStateApplied += new LayerControllerEvent(layerStateChanged);

            skinMenu = new LayerGUIMenu(window.findWidget("Layers/Skin") as Button, window.findWidget("Layers/SkinMenu") as Button);
            skinMenu.createShortcuts(KeyboardButtonCode.KC_1);
            skinMenu.TransparencyChanged += changeSkinTransparency;

            nervesMenu = new LayerGUIMenu(window.findWidget("Layers/Nerves") as Button, window.findWidget("Layers/NervesMenu") as Button);
            nervesMenu.createShortcuts(KeyboardButtonCode.KC_2);
            nervesMenu.TransparencyChanged += changeNerveTransparency;

            musclesMenu = new LayerGUIMenu(window.findWidget("Layers/Muscles") as Button, window.findWidget("Layers/MusclesMenu") as Button);
            musclesMenu.createShortcuts(KeyboardButtonCode.KC_3);
            musclesMenu.TransparencyChanged += changeMuscleTransparency;

            skullMenu = new LayerGUISkullMenu(window.findWidget("Layers/Skull") as Button, window.findWidget("Layers/SkullMenu") as Button);
            skullMenu.createShortcuts(KeyboardButtonCode.KC_4);
            skullMenu.createEminanceShortcut(KeyboardButtonCode.KC_5);
            skullMenu.TransparencyChanged += changeSkullTransparency;
            skullMenu.ToggleEminance += toggleShowEminance;

            mandibleMenu = new LayerGUIMenu(window.findWidget("Layers/Mandible") as Button, window.findWidget("Layers/MandibleMenu") as Button);
            mandibleMenu.createShortcuts(KeyboardButtonCode.KC_6);
            mandibleMenu.TransparencyChanged += changeMandibleTransparency;

            discsMenu = new LayerGUIMenu(window.findWidget("Layers/Discs") as Button, window.findWidget("Layers/DiscsMenu") as Button);
            discsMenu.createShortcuts(KeyboardButtonCode.KC_7);
            discsMenu.TransparencyChanged += changeDiscTransparency;

            spineMenu = new LayerGUIMenu(window.findWidget("Layers/Spine") as Button, window.findWidget("Layers/SpineMenu") as Button);
            spineMenu.createShortcuts(KeyboardButtonCode.KC_8);
            spineMenu.TransparencyChanged += changeSpineTransparency;

            hyoidMenu = new LayerGUIMenu(window.findWidget("Layers/Hyoid") as Button, window.findWidget("Layers/HyoidMenu") as Button);
            hyoidMenu.createShortcuts(KeyboardButtonCode.KC_9);
            hyoidMenu.TransparencyChanged += changeHyoidTransparency;

            topTeethMenu = new LayerGUIMenu(window.findWidget("Layers/MaxillaryTeeth") as Button, window.findWidget("Layers/MaxillaryTeethMenu") as Button);
            topTeethMenu.createShortcuts(KeyboardButtonCode.KC_0);
            topTeethMenu.TransparencyChanged += changeTopToothTransparency;

            bottomTeethMenu = new LayerGUIMenu(window.findWidget("Layers/MandibularTeeth") as Button, window.findWidget("Layers/MandibularTeethMenu") as Button);
            bottomTeethMenu.createShortcuts(KeyboardButtonCode.KC_0);
            bottomTeethMenu.TransparencyChanged += changeBottomToothTransparency;

            TransparencyController.ActiveTransparencyStateChanged += new EventHandler(TransparencyController_ActiveTransparencyStateChanged);
        }

        public override void Dispose()
        {
            skinMenu.Dispose();
            nervesMenu.Dispose();
            musclesMenu.Dispose();
            skullMenu.Dispose();
            mandibleMenu.Dispose();
            discsMenu.Dispose();
            spineMenu.Dispose();
            hyoidMenu.Dispose();
            topTeethMenu.Dispose();
            bottomTeethMenu.Dispose();
            base.Dispose();
        }

        public void resetMenus()
        {
            skinMenu.setAlpha(1.0f);
            nervesMenu.setAlpha(1.0f);
            musclesMenu.setAlpha(1.0f);
            skullMenu.setAlpha(1.0f);
            mandibleMenu.setAlpha(1.0f);
            hyoidMenu.setAlpha(1.0f);
            spineMenu.setAlpha(1.0f);
            discsMenu.setAlpha(1.0f);
            topTeethMenu.setAlpha(1.0f);
            bottomTeethMenu.setAlpha(1.0f);
        }

        void layerStateChanged(LayerController controller)
        {
            synchronizeLayerMenus();
        }

        void TransparencyController_ActiveTransparencyStateChanged(object sender, EventArgs e)
        {
            synchronizeLayerMenus();
        }

        void synchronizeLayerMenus()
        {
            TransparencyInterface skull = TransparencyController.getTransparencyObject("Skull");
            if (skull != null)
            {
                skullMenu.setAlpha(skull.CurrentAlpha);
            }
            TransparencyInterface leftEminence = TransparencyController.getTransparencyObject("Left Eminence");
            if (leftEminence != null)
            {
                skullMenu.ShowEminance = leftEminence.CurrentAlpha == skull.CurrentAlpha;
            }

            TransparencyInterface nerve = TransparencyController.getTransparencyObject("Left Auriculo Temporal Nerve");
            if (nerve != null)
            {
                nervesMenu.setAlpha(nerve.CurrentAlpha);
            }

            TransparencyInterface leftDisc = TransparencyController.getTransparencyObject("Left TMJ Disc");
            if (leftDisc != null)
            {
                discsMenu.setAlpha(leftDisc.CurrentAlpha);
            }


            TransparencyInterface mandible = TransparencyController.getTransparencyObject("Mandible");
            if (mandible != null)
            {
                mandibleMenu.setAlpha(mandible.CurrentAlpha);
            }

            TransparencyInterface topTooth = TransparencyController.getTransparencyObject("Tooth 1");
            if (topTooth != null)
            {
                topTeethMenu.setAlpha(topTooth.CurrentAlpha);
            }
            TransparencyInterface bottomTooth = TransparencyController.getTransparencyObject("Tooth 17");
            if (bottomTooth != null)
            {
                bottomTeethMenu.setAlpha(bottomTooth.CurrentAlpha);
            }

            TransparencyInterface skin = TransparencyController.getTransparencyObject("Skin");
            if (skin != null)
            {
                skinMenu.setAlpha(skin.CurrentAlpha);
            }

            TransparencyInterface muscle = TransparencyController.getTransparencyObject("Left Masseter");
            if (muscle != null)
            {
                musclesMenu.setAlpha(muscle.CurrentAlpha);
            }

            TransparencyInterface spine = TransparencyController.getTransparencyObject("C1");
            if (spine != null)
            {
                spineMenu.setAlpha(spine.CurrentAlpha);
            }

            TransparencyInterface hyoid = TransparencyController.getTransparencyObject("Hyoid");
            if (hyoid != null)
            {
                hyoidMenu.setAlpha(hyoid.CurrentAlpha);
            }

        }

        public bool AllowShortcuts
        {
            get
            {
                if (skinMenu != null)
                {
                    return skinMenu.AllowShortcuts;
                }
                return false;
            }
            set
            {
                if (skinMenu != null)
                {
                    skinMenu.AllowShortcuts = value;
                    nervesMenu.AllowShortcuts = value;
                    musclesMenu.AllowShortcuts = value;
                    skullMenu.AllowShortcuts = value;
                    mandibleMenu.AllowShortcuts = value;
                    discsMenu.AllowShortcuts = value;
                    spineMenu.AllowShortcuts = value;
                    hyoidMenu.AllowShortcuts = value;
                    topTeethMenu.AllowShortcuts = value;
                    bottomTeethMenu.AllowShortcuts = value;
                }
            }
        }

        #region Transparency Helper Functions

        void changeHyoidTransparency(float alpha)
        {
            TransparencyInterface hyoid = TransparencyController.getTransparencyObject("Hyoid");
            if (hyoid != null)
            {
                hyoid.smoothBlend(alpha, MedicalConfig.TransparencyChangeMultiplier);
            }
        }

        void changeNerveTransparency(float alpha)
        {
            TransparencyGroup group = TransparencyController.getTransparencyGroup(RenderGroup.Nerves);
            foreach (TransparencyInterface nerve in group.getTransparencyObjectIter())
            {
                nerve.smoothBlend(alpha, MedicalConfig.TransparencyChangeMultiplier);
            }
        }

        private void changeSkullTransparency(float alpha)
        {
            TransparencyInterface skull = TransparencyController.getTransparencyObject("Skull");
            skull.smoothBlend(alpha, MedicalConfig.TransparencyChangeMultiplier);
            TransparencyInterface skullInterior = TransparencyController.getTransparencyObject("Skull Interior");
            skullInterior.smoothBlend(alpha, MedicalConfig.TransparencyChangeMultiplier);
            if (skullMenu.ShowEminance)
            {
                TransparencyInterface leftEminence = TransparencyController.getTransparencyObject("Left Eminence");
                TransparencyInterface rightEminence = TransparencyController.getTransparencyObject("Right Eminence");
                leftEminence.smoothBlend(alpha, MedicalConfig.TransparencyChangeMultiplier);
                rightEminence.smoothBlend(alpha, MedicalConfig.TransparencyChangeMultiplier);
            }
            TransparencyInterface maxillarySinus = TransparencyController.getTransparencyObject("Maxillary Sinus");
            maxillarySinus.smoothBlend(alpha, MedicalConfig.TransparencyChangeMultiplier);
        }

        private void changeDiscTransparency(float alpha)
        {
            TransparencyInterface leftDisc = TransparencyController.getTransparencyObject("Left TMJ Disc");
            leftDisc.smoothBlend(alpha, MedicalConfig.TransparencyChangeMultiplier);
            TransparencyInterface rightDisc = TransparencyController.getTransparencyObject("Right TMJ Disc");
            rightDisc.smoothBlend(alpha, MedicalConfig.TransparencyChangeMultiplier);
        }

        private void changeMandibleTransparency(float alpha)
        {
            TransparencyInterface skull = TransparencyController.getTransparencyObject("Mandible");
            skull.smoothBlend(alpha, MedicalConfig.TransparencyChangeMultiplier);
        }

        private void changeTopToothTransparency(float alpha)
        {
            for (int i = 1; i < 17; ++i)
            {
                TransparencyController.getTransparencyObject("Tooth " + i).smoothBlend(alpha, MedicalConfig.TransparencyChangeMultiplier);
            }
        }

        private void changeBottomToothTransparency(float alpha)
        {
            for (int i = 17; i < 33; ++i)
            {
                TransparencyController.getTransparencyObject("Tooth " + i).smoothBlend(alpha, MedicalConfig.TransparencyChangeMultiplier);
            }
        }

        private void changeSkinTransparency(float alpha)
        {
            TransparencyInterface skin = TransparencyController.getTransparencyObject("Skin");
            skin.smoothBlend(alpha, MedicalConfig.TransparencyChangeMultiplier);
            TransparencyInterface leftEye = TransparencyController.getTransparencyObject("Left Eye");
            leftEye.smoothBlend(alpha, MedicalConfig.TransparencyChangeMultiplier);
            TransparencyInterface rightEye = TransparencyController.getTransparencyObject("Right Eye");
            rightEye.smoothBlend(alpha, MedicalConfig.TransparencyChangeMultiplier);
            TransparencyInterface eyebrowsAndEyelashes = TransparencyController.getTransparencyObject("Eyebrows and Eyelashes");
            eyebrowsAndEyelashes.smoothBlend(alpha, MedicalConfig.TransparencyChangeMultiplier);
        }

        private void changeMuscleTransparency(float alpha)
        {
            TransparencyGroup group = TransparencyController.getTransparencyGroup(RenderGroup.Muscles);
            foreach (TransparencyInterface item in group.getTransparencyObjectIter())
            {
                item.smoothBlend(alpha, MedicalConfig.TransparencyChangeMultiplier);
            }
        }

        private void changeSpineTransparency(float alpha)
        {
            TransparencyGroup group = TransparencyController.getTransparencyGroup(RenderGroup.Spine);
            foreach (TransparencyInterface item in group.getTransparencyObjectIter())
            {
                item.smoothBlend(alpha, MedicalConfig.TransparencyChangeMultiplier);
            }
        }

        private void toggleShowEminance(bool show)
        {
            if (show)
            {
                TransparencyInterface skull = TransparencyController.getTransparencyObject("Skull");
                TransparencyInterface leftEminence = TransparencyController.getTransparencyObject("Left Eminence");
                TransparencyInterface rightEminence = TransparencyController.getTransparencyObject("Right Eminence");
                leftEminence.smoothBlend(skull.CurrentAlpha, MedicalConfig.TransparencyChangeMultiplier);
                rightEminence.smoothBlend(skull.CurrentAlpha, MedicalConfig.TransparencyChangeMultiplier);
            }
            else
            {
                TransparencyInterface leftEminence = TransparencyController.getTransparencyObject("Left Eminence");
                TransparencyInterface rightEminence = TransparencyController.getTransparencyObject("Right Eminence");
                leftEminence.smoothBlend(0.0f, MedicalConfig.TransparencyChangeMultiplier);
                rightEminence.smoothBlend(0.0f, MedicalConfig.TransparencyChangeMultiplier);
            }
        }

        #endregion
    }
}
