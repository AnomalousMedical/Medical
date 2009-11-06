using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ComponentFactory.Krypton.Ribbon;
using ComponentFactory.Krypton.Toolkit;
using Medical.Controller;
using System.Windows.Forms;
using System.Drawing;

namespace Medical.GUI
{
    public class LayerGUIController : IDisposable
    {
        private LayerGUIMenu skinMenu;
        private LayerGUIMenu musclesMenu;
        private LayerGUISkullMenu skullMenu;
        private LayerGUIMenu mandibleMenu;
        private LayerGUIMenu discsButton;
        private LayerGUIMenu spineMenu;
        private LayerGUIMenu hyoidMenu;
        private LayerGUIMenu topTeethMenu;
        private LayerGUIMenu bottomTeethMenu;

        private KryptonCommand showTeethCollisionCommand;

        private LayerController layerController;
        private KryptonRibbonGroupGallery predefinedLayerGallery;
        private ImageList predefinedImageList;
        private List<String> stateIndexes = new List<string>();

        public LayerGUIController(BasicForm basicForm, BasicController basicController, ShortcutController shortcuts)
        {
            ShortcutGroup group = shortcuts.createOrRetrieveGroup("LayerShortcuts");

            skinMenu = new LayerGUIMenu(basicForm.layersSkinButton);
            skinMenu.createShortcuts("SkinToggle", group, Keys.F1);
            skinMenu.TransparencyChanged += changeSkinTransparency;

            musclesMenu = new LayerGUIMenu(basicForm.layersMusclesButton);
            musclesMenu.createShortcuts("MusclesToggle", group, Keys.F2);
            musclesMenu.TransparencyChanged += changeMuscleTransparency;

            skullMenu = new LayerGUISkullMenu(basicForm.layersSkullButton);
            skullMenu.createShortcuts("SkullToggle", group, Keys.F3);
            skullMenu.createEminanceShortcut("EminanceToggle", group, Keys.F4);
            skullMenu.TransparencyChanged += changeSkullTransparency;

            mandibleMenu = new LayerGUIMenu(basicForm.layersMandibleButton);
            mandibleMenu.createShortcuts("MandibleToggle", group, Keys.F5);
            mandibleMenu.TransparencyChanged += changeMandibleTransparency;

            discsButton = new LayerGUIMenu(basicForm.layersDiscsButton);
            discsButton.createShortcuts("DiscsToggle", group, Keys.F6);
            discsButton.TransparencyChanged += changeDiscTransparency;

            spineMenu = new LayerGUIMenu(basicForm.layersSpineButton);
            spineMenu.createShortcuts("SpineToggle", group, Keys.F7);
            spineMenu.TransparencyChanged += changeSpineTransparency;

            hyoidMenu = new LayerGUIMenu(basicForm.layersHyoidButton);
            hyoidMenu.createShortcuts("HyoidToggle", group, Keys.F8);
            hyoidMenu.TransparencyChanged += changeHyoidTransparency;

            topTeethMenu = new LayerGUIMenu(basicForm.layersTopTeethButton);
            //topTeethMenu.createShortcuts("TopTeethToggle", group, Keys.F9);
            topTeethMenu.TransparencyChanged += changeTopToothTransparency;

            bottomTeethMenu = new LayerGUIMenu(basicForm.layersBottomTeethButton);
            //bottomTeethMenu.createShortcuts("BottomTeethToggle", group, Keys.F10);
            bottomTeethMenu.TransparencyChanged += changeBottomToothTransparency;

            showTeethCollisionCommand = basicForm.showTeethCollisionCommand;
            showTeethCollisionCommand.Execute += new EventHandler(showTeethCollisionCommand_Execute);

            //Predefined layers
            layerController = basicController.LayerController;
            layerController.LayerStateSetChanged += new LayerControllerEvent(LayerController_LayerStateSetChanged);
            predefinedImageList = new ImageList();
            predefinedImageList.ColorDepth = ColorDepth.Depth32Bit;
            predefinedImageList.ImageSize = new Size(48, 48);
            predefinedLayerGallery = basicForm.predefinedLayerGallery;
            predefinedLayerGallery.ImageList = predefinedImageList;
            predefinedLayerGallery.SelectedIndexChanged += new EventHandler(predefinedLayerGallery_SelectedIndexChanged);
        }

        public void Dispose()
        {
            skinMenu.Dispose();
            predefinedImageList.Dispose();
        }

        void showTeethCollisionCommand_Execute(object sender, EventArgs e)
        {
            TeethController.HighlightContacts = showTeethCollisionCommand.Checked;
        }

        void LayerController_LayerStateSetChanged(LayerController controller)
        {
            predefinedImageList.Images.Clear();
            stateIndexes.Clear();
            foreach (LayerState state in controller.CurrentLayers.LayerStates)
            {
                if (!state.Hidden && state.Thumbnail != null)
                {
                    predefinedImageList.Images.Add(state.Thumbnail);
                    stateIndexes.Add(state.Name);
                }
            }
        }

        void predefinedLayerGallery_SelectedIndexChanged(object sender, EventArgs e)
        {
            layerController.applyLayerState(stateIndexes[predefinedLayerGallery.SelectedIndex]);
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
