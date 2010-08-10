using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;
using Engine.Platform;

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

        private LayerController layerController;
        private ImageAtlas predefinedImageAtlas;
        private ButtonGrid predefinedLayerGallery;

        private Button showContacts;

        public LayerGUIController(Gui ribbonLayout, LayerController layerController)
        {
            //Predefined layers
            this.layerController = layerController;
            layerController.LayerStateSetChanged += new LayerControllerEvent(layerController_LayerStateSetChanged);
            predefinedLayerGallery = new ButtonGrid(ribbonLayout.findWidgetT("Layers/Predefined") as ScrollView);
            predefinedLayerGallery.SelectedValueChanged += new EventHandler(predefinedLayerGallery_SelectedValueChanged);
            predefinedImageAtlas = new ImageAtlas("PredefinedLayers", new Size2(100, 100), new Size2(512, 512));

            showContacts = ribbonLayout.findWidgetT("Layers/ShowContacts") as Button;
            showContacts.MouseButtonClick += new MyGUIEvent(showContacts_MouseButtonClick);

            //if (UserPermissions.Instance.allowFeature(Features.PIPER_JBO_FEATURE_CUSTOM_LAYERS))
            {
                layerController.CurrentLayerStateChanged += new LayerControllerEvent(synchronizeLayerMenus);

                skinMenu = new LayerGUIMenu(ribbonLayout.findWidgetT("Layers/Skin") as Button, ribbonLayout.findWidgetT("Layers/SkinMenu") as Button);
                skinMenu.createShortcuts(KeyboardButtonCode.KC_F1);
                skinMenu.TransparencyChanged += changeSkinTransparency;

                musclesMenu = new LayerGUIMenu(ribbonLayout.findWidgetT("Layers/Muscles") as Button, ribbonLayout.findWidgetT("Layers/MusclesMenu") as Button);
                musclesMenu.createShortcuts(KeyboardButtonCode.KC_F2);
                musclesMenu.TransparencyChanged += changeMuscleTransparency;

                skullMenu = new LayerGUISkullMenu(ribbonLayout.findWidgetT("Layers/Skull") as Button, ribbonLayout.findWidgetT("Layers/SkullMenu") as Button);
                skullMenu.createShortcuts(KeyboardButtonCode.KC_F3);
                skullMenu.createEminanceShortcut(KeyboardButtonCode.KC_F4);
                skullMenu.TransparencyChanged += changeSkullTransparency;

                mandibleMenu = new LayerGUIMenu(ribbonLayout.findWidgetT("Layers/Mandible") as Button, ribbonLayout.findWidgetT("Layers/MandibleMenu") as Button);
                mandibleMenu.createShortcuts(KeyboardButtonCode.KC_F5);
                mandibleMenu.TransparencyChanged += changeMandibleTransparency;

                discsMenu = new LayerGUIMenu(ribbonLayout.findWidgetT("Layers/Discs") as Button, ribbonLayout.findWidgetT("Layers/DiscsMenu") as Button);
                discsMenu.createShortcuts(KeyboardButtonCode.KC_F6);
                discsMenu.TransparencyChanged += changeDiscTransparency;

                spineMenu = new LayerGUIMenu(ribbonLayout.findWidgetT("Layers/Spine") as Button, ribbonLayout.findWidgetT("Layers/SpineMenu") as Button);
                spineMenu.createShortcuts(KeyboardButtonCode.KC_F7);
                spineMenu.TransparencyChanged += changeSpineTransparency;

                hyoidMenu = new LayerGUIMenu(ribbonLayout.findWidgetT("Layers/Hyoid") as Button, ribbonLayout.findWidgetT("Layers/HyoidMenu") as Button);
                hyoidMenu.createShortcuts(KeyboardButtonCode.KC_F8);
                hyoidMenu.TransparencyChanged += changeHyoidTransparency;

                topTeethMenu = new LayerGUIMenu(ribbonLayout.findWidgetT("Layers/MaxillaryTeeth") as Button, ribbonLayout.findWidgetT("Layers/MaxillaryTeethMenu") as Button);
                topTeethMenu.createShortcuts(KeyboardButtonCode.KC_F9);
                topTeethMenu.TransparencyChanged += changeTopToothTransparency;

                bottomTeethMenu = new LayerGUIMenu(ribbonLayout.findWidgetT("Layers/MandibularTeeth") as Button, ribbonLayout.findWidgetT("Layers/MandibularTeethMenu") as Button);
                bottomTeethMenu.createShortcuts(KeyboardButtonCode.KC_F10);
                bottomTeethMenu.TransparencyChanged += changeBottomToothTransparency;
            }
            //else
            //{
            //    basicForm.customLayersGroup.Visible = false;
            //}
        }

        public void resetMenus()
        {
            skinMenu.setAlpha(1.0f);
            musclesMenu.setAlpha(1.0f);
            skullMenu.setAlpha(1.0f);
            mandibleMenu.setAlpha(1.0f);
            hyoidMenu.setAlpha(1.0f);
            spineMenu.setAlpha(1.0f);
            discsMenu.setAlpha(1.0f);
            topTeethMenu.setAlpha(1.0f);
            bottomTeethMenu.setAlpha(1.0f);
        }

        void synchronizeLayerMenus(LayerController controller)
        {
            foreach (LayerEntry layerEntry in controller.CurrentLayerState.Entries)
            {
                if (layerEntry.RenderGroup == RenderGroup.Skin && layerEntry.TransparencyObject == "Skin")
                {
                    skinMenu.setAlpha(layerEntry.AlphaValue);
                }
                else if (layerEntry.RenderGroup == RenderGroup.Muscles && layerEntry.TransparencyObject == "Left Masseter")
                {
                    musclesMenu.setAlpha(layerEntry.AlphaValue);
                }
                else if (layerEntry.RenderGroup == RenderGroup.Bones)
                {
                    if (layerEntry.TransparencyObject == "Skull")
                    {
                        skullMenu.setAlpha(layerEntry.AlphaValue);
                    }
                    else if (layerEntry.TransparencyObject == "Mandible")
                    {
                        mandibleMenu.setAlpha(layerEntry.AlphaValue);
                    }
                    else if (layerEntry.TransparencyObject == "Hyoid")
                    {
                        hyoidMenu.setAlpha(layerEntry.AlphaValue);
                    }
                }
                else if (layerEntry.RenderGroup == RenderGroup.Spine)
                {
                    if (layerEntry.TransparencyObject == "C1")
                    {
                        spineMenu.setAlpha(layerEntry.AlphaValue);
                    }
                }
                else if (layerEntry.RenderGroup == RenderGroup.TMJ)
                {
                    if (layerEntry.TransparencyObject == "Left TMJ Disc")
                    {
                        discsMenu.setAlpha(layerEntry.AlphaValue);
                    }
                }
                else if (layerEntry.RenderGroup == RenderGroup.Teeth)
                {
                    if (layerEntry.TransparencyObject == "Tooth 10")
                    {
                        topTeethMenu.setAlpha(layerEntry.AlphaValue);
                    }
                    else if (layerEntry.TransparencyObject == "Tooth 25")
                    {
                        bottomTeethMenu.setAlpha(layerEntry.AlphaValue);
                    }
                }
            }
        }

        void showContacts_MouseButtonClick(Widget source, EventArgs e)
        {
            TeethController.HighlightContacts = !TeethController.HighlightContacts;
            showContacts.StateCheck = TeethController.HighlightContacts;
        }

        void predefinedLayerGallery_SelectedValueChanged(object sender, EventArgs e)
        {
            ButtonGridItem selectedItem = predefinedLayerGallery.SelectedItem;
            if (selectedItem != null)
            {
                layerController.applyLayerState(selectedItem.UserObject.ToString());
            }
        }

        void layerController_LayerStateSetChanged(LayerController controller)
        {
            predefinedImageAtlas.clear();
            predefinedLayerGallery.clear();
            foreach (LayerState state in controller.CurrentLayers.LayerStates)
            {
                if (!state.Hidden && state.Thumbnail != null)
                {
                    String imageKey = predefinedImageAtlas.addImage(state, state.Thumbnail);
                    ButtonGridItem item = predefinedLayerGallery.addItem("Main", state.Name, imageKey);
                    item.UserObject = state.Name;
                }
            }
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
