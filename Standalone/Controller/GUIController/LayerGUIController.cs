using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.GUI
{
    public class LayerGUIController : IDisposable
    {
        private LayerGUIMenu skinMenu;
        private LayerGUIMenu musclesMenu;
        private LayerGUISkullMenu skullMenu;
        private LayerGUIMenu mandibleMenu;
        private LayerGUIMenu discsMenu;
        private LayerGUIMenu spineMenu;
        private LayerGUIMenu hyoidMenu;
        private LayerGUIMenu topTeethMenu;
        private LayerGUIMenu bottomTeethMenu;

        public LayerGUIController(Gui ribbonLayout)
        {
            //if (UserPermissions.Instance.allowFeature(Features.PIPER_JBO_FEATURE_CUSTOM_LAYERS))
            {
                //layerController.CurrentLayerStateChanged += new LayerControllerEvent(synchronizeLayerMenus);

                skinMenu = new LayerGUIMenu(ribbonLayout.findWidgetT("Layers/Skin") as Button, ribbonLayout.findWidgetT("Layers/SkinMenu") as Button);
                //skinMenu.createShortcuts("SkinToggle", group, Keys.F1);
                skinMenu.TransparencyChanged += changeSkinTransparency;

                musclesMenu = new LayerGUIMenu(ribbonLayout.findWidgetT("Layers/Muscles") as Button, ribbonLayout.findWidgetT("Layers/MusclesMenu") as Button);
                //musclesMenu.createShortcuts("MusclesToggle", group, Keys.F2);
                musclesMenu.TransparencyChanged += changeMuscleTransparency;

                skullMenu = new LayerGUISkullMenu(ribbonLayout.findWidgetT("Layers/Skull") as Button, ribbonLayout.findWidgetT("Layers/SkullMenu") as Button);
                //skullMenu.createShortcuts("SkullToggle", group, Keys.F3);
                //skullMenu.createEminanceShortcut("EminanceToggle", group, Keys.F4);
                skullMenu.TransparencyChanged += changeSkullTransparency;

                mandibleMenu = new LayerGUIMenu(ribbonLayout.findWidgetT("Layers/Mandible") as Button, ribbonLayout.findWidgetT("Layers/MandibleMenu") as Button);
                //mandibleMenu.createShortcuts("MandibleToggle", group, Keys.F5);
                mandibleMenu.TransparencyChanged += changeMandibleTransparency;

                discsMenu = new LayerGUIMenu(ribbonLayout.findWidgetT("Layers/Discs") as Button, ribbonLayout.findWidgetT("Layers/DiscsMenu") as Button);
                //discsMenu.createShortcuts("DiscsToggle", group, Keys.F6);
                discsMenu.TransparencyChanged += changeDiscTransparency;

                spineMenu = new LayerGUIMenu(ribbonLayout.findWidgetT("Layers/Spine") as Button, ribbonLayout.findWidgetT("Layers/SpineMenu") as Button);
                //spineMenu.createShortcuts("SpineToggle", group, Keys.F7);
                spineMenu.TransparencyChanged += changeSpineTransparency;

                hyoidMenu = new LayerGUIMenu(ribbonLayout.findWidgetT("Layers/Hyoid") as Button, ribbonLayout.findWidgetT("Layers/HyoidMenu") as Button);
                //hyoidMenu.createShortcuts("HyoidToggle", group, Keys.F8);
                hyoidMenu.TransparencyChanged += changeHyoidTransparency;

                topTeethMenu = new LayerGUIMenu(ribbonLayout.findWidgetT("Layers/MaxillaryTeeth") as Button, ribbonLayout.findWidgetT("Layers/MaxillaryTeethMenu") as Button);
                //topTeethMenu.createShortcuts("TopTeethToggle", group, Keys.F9);
                topTeethMenu.TransparencyChanged += changeTopToothTransparency;

                bottomTeethMenu = new LayerGUIMenu(ribbonLayout.findWidgetT("Layers/MandibularTeeth") as Button, ribbonLayout.findWidgetT("Layers/MandibularTeethMenu") as Button);
                //bottomTeethMenu.createShortcuts("BottomTeethToggle", group, Keys.F10);
                bottomTeethMenu.TransparencyChanged += changeBottomToothTransparency;
            }
            //else
            //{
            //    basicForm.customLayersGroup.Visible = false;
            //}
        }

        public void Dispose()
        {
            skinMenu.Dispose();
            musclesMenu.Dispose();
            skullMenu.Dispose();
            mandibleMenu.Dispose();
            discsMenu.Dispose();
            spineMenu.Dispose();
            hyoidMenu.Dispose();
            topTeethMenu.Dispose();
            bottomTeethMenu.Dispose();
        }

        #region Transparency Helper Functions

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
            if (skullMenu.ShowEminance)
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

        #endregion
    }
}
