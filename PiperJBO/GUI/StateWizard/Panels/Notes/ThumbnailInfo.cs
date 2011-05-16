using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller;

namespace Medical.GUI
{
    class ThumbnailInfo
    {
        private static Engine.Color BACK_COLOR = new Engine.Color(.94f, .94f, .94f);

        private ImageRendererProperties imageProp;
        private String layerState;
        private String navigationState;

        public ThumbnailInfo(String layerState, String navigationState, int width, int height)
        {
            imageProp = new ImageRendererProperties();
            imageProp.Width = width;
            imageProp.Height = height;
            imageProp.UseWindowBackgroundColor = false;
            imageProp.CustomBackgroundColor = BACK_COLOR;
            imageProp.AntiAliasingMode = 2;
            imageProp.UseActiveViewportLocation = false;
            imageProp.OverrideLayers = true;
            imageProp.TransparentBackground = true;
            imageProp.ShowBackground = false;
            imageProp.ShowWatermark = false;
            imageProp.ShowUIUpdates = false;

            this.layerState = layerState;
            this.navigationState = navigationState;
        }

        public ImageRendererProperties configureProperties(NavigationController navigationController, LayerController layerController)
        {
            //imageProp.NavigationStateName = navigationState;
            //imageProp.LayerStateName = layerState;
            imageProp.LayerState = layerController.CurrentLayers.getState(layerState);
            NavigationState navState = navigationController.getState(navigationState);
            imageProp.CameraLookAt = navState.LookAt;
            imageProp.CameraPosition = navState.Translation;
            return imageProp;
        }
    }
}
