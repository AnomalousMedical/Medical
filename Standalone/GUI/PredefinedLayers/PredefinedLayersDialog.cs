using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;

namespace Medical.GUI
{
    public class PredefinedLayersDialog : Dialog
    {
        private LayerController layerController;
        private ImageAtlas predefinedImageAtlas;
        private ButtonGrid predefinedLayerGallery;

        public PredefinedLayersDialog(LayerController layerController)
            :base("Medical.GUI.PredefinedLayers.PredefinedLayersDialog.layout")
        {
            this.layerController = layerController;
            layerController.LayerStateSetChanged += new LayerControllerEvent(layerController_LayerStateSetChanged);
            predefinedLayerGallery = new ButtonGrid(window.findWidget("PredefinedLayers") as ScrollView);
            predefinedLayerGallery.SelectedValueChanged += new EventHandler(predefinedLayerGallery_SelectedValueChanged);
            predefinedImageAtlas = new ImageAtlas("PredefinedLayers", new Size2(100, 100), new Size2(512, 512));

            window.WindowChangedCoord += new MyGUIEvent(window_WindowChangedCoord);
        }

        public override void deserialize(ConfigFile configFile)
        {
            base.deserialize(configFile);
            predefinedLayerGallery.resizeAndLayout(window.Width - 15);
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

        void predefinedLayerGallery_SelectedValueChanged(object sender, EventArgs e)
        {
            ButtonGridItem selectedItem = predefinedLayerGallery.SelectedItem;
            if (selectedItem != null)
            {
                layerController.applyLayerState(selectedItem.UserObject.ToString());
            }
        }

        void window_WindowChangedCoord(Widget source, EventArgs e)
        {
            predefinedLayerGallery.resizeAndLayout(source.Width - 15);
        }
    }
}
