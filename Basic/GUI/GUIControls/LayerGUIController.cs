using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ComponentFactory.Krypton.Ribbon;
using ComponentFactory.Krypton.Toolkit;
using Medical.Controller;

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

        public LayerGUIController(BasicForm basicForm, BasicController basicController)
        {
            skinMenu = new LayerGUIMenu(basicForm.layersSkinButton);
            skinMenu.TransparencyChanged += changeSkinTransparency;

            musclesMenu = new LayerGUIMenu(basicForm.layersMusclesButton);
            musclesMenu.TransparencyChanged += changeMuscleTransparency;

            skullMenu = new LayerGUISkullMenu(basicForm.layersSkullButton);
            skullMenu.TransparencyChanged += changeSkullTransparency;

            mandibleMenu = new LayerGUIMenu(basicForm.layersMandibleButton);
            mandibleMenu.TransparencyChanged += changeMandibleTransparency;

            discsButton = new LayerGUIMenu(basicForm.layersDiscsButton);
            discsButton.TransparencyChanged += changeDiscTransparency;

            spineMenu = new LayerGUIMenu(basicForm.layersSpineButton);
            spineMenu.TransparencyChanged += changeSpineTransparency;

            hyoidMenu = new LayerGUIMenu(basicForm.layersHyoidButton);
            hyoidMenu.TransparencyChanged += changeHyoidTransparency;

            topTeethMenu = new LayerGUIMenu(basicForm.layersTopTeethButton);
            topTeethMenu.TransparencyChanged += changeTopToothTransparency;

            bottomTeethMenu = new LayerGUIMenu(basicForm.layersBottomTeethButton);
            bottomTeethMenu.TransparencyChanged += changeBottomToothTransparency;

            showTeethCollisionCommand = basicForm.showTeethCollisionCommand;
            showTeethCollisionCommand.Execute += new EventHandler(showTeethCollisionCommand_Execute);
        }

        public void Dispose()
        {
            skinMenu.Dispose();
        }

        void showTeethCollisionCommand_Execute(object sender, EventArgs e)
        {
            TeethController.HighlightContacts = showTeethCollisionCommand.Checked;
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
