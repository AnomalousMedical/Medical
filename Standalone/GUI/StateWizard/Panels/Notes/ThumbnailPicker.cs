using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using MyGUIPlugin;
using Engine;

namespace Medical.GUI
{
    class ThumbnailPicker
    {
        private static Engine.Color BACK_COLOR = new Engine.Color(.94f, .94f, .94f);

        private ImageAtlas thumbnailImages;
        private List<ImageRendererProperties> thumbnailProperties = new List<ImageRendererProperties>();
        private Dictionary<ButtonGridItem, Bitmap> currentImages = new Dictionary<ButtonGridItem, Bitmap>();
        private ImageRenderer imageRenderer;
        private ButtonGrid imageGrid;

        public ThumbnailPicker(ImageRenderer imageRenderer, ScrollView thumbnailScroll)
        {
            this.imageRenderer = imageRenderer;
            imageGrid = new ButtonGrid(thumbnailScroll);
            thumbnailImages = new ImageAtlas("ThumbnailPicker", new Size2(imageGrid.ItemWidth, imageGrid.ItemHeight), new Size2(512, 512));
        }

        protected void Dispose()
        {
            thumbnailImages.Dispose();
        }

        /// <summary>
        /// Add a thumbnail to the picker.
        /// </summary>
        /// <param name="navigationState">The navigation state of the thumbnail.</param>
        /// <param name="layerState">The layer state of the thumbnail.</param>
        public void addThumbnail(String navigationState, String layerState)
        {
            ImageRendererProperties imageProp = new ImageRendererProperties();
            imageProp.Width = (int)thumbnailImages.ImageSize.Width;
            imageProp.Height = (int)thumbnailImages.ImageSize.Height;
            imageProp.UseWindowBackgroundColor = false;
            imageProp.CustomBackgroundColor = BACK_COLOR;
            imageProp.AntiAliasingMode = 2;
            imageProp.UseActiveViewportLocation = false;
            imageProp.UseNavigationStatePosition = true;
            imageProp.NavigationStateName = navigationState;
            imageProp.OverrideLayers = true;
            imageProp.LayerState = layerState;
            imageProp.TransparentBackground = true;
            imageProp.ShowBackground = false;
            imageProp.ShowWatermark = false;
            imageProp.ShowUIUpdates = false;

            thumbnailProperties.Add(imageProp);
        }

        /// <summary>
        /// Update all thumbnails in the picker.
        /// </summary>
        public void updateThumbnails()
        {
            if (imageRenderer != null)
            {
                foreach (Bitmap bitmap in currentImages.Values)
                {
                    bitmap.Dispose();
                }
                thumbnailImages.clear();
                currentImages.Clear();
                imageGrid.clear();
                foreach (ImageRendererProperties imageProperties in thumbnailProperties)
                {
                    Bitmap thumb = imageRenderer.renderImage(imageProperties);
                    String imageId = thumbnailImages.addImage(thumb, thumb);
                    ButtonGridItem item = imageGrid.addItem("Main", "", imageId);
                    currentImages.Add(item, thumb);
                }
                if (imageGrid.Count > 0)
                {
                    imageGrid.SelectedItem = imageGrid.getItem(0);
                }
            }
            else
            {
                throw new Exception("Cannot generate thumbnails for the Thumbnail Picker. Image Renderer is null.");
            }
        }

        /// <summary>
        /// Returns the currently selected thumbnail bitmap. This actually
        /// renders a new thumbnail when this function is called, so the
        /// thumbnail will always be up to date. This means that the caller is
        /// responsible for disposing the returned image. This will return null
        /// if no thumbnail is selected.
        /// </summary>
        public Bitmap SelectedThumbnail
        {
            get
            {
                ButtonGridItem selectedItem = imageGrid.SelectedItem;
                if (selectedItem != null)
                {
                    return currentImages[selectedItem];
                }
                //if (SelectedIndex > 0 && SelectedIndex < currentImages.Count)
                //{
                //    return imageRenderer.renderImage(thumbnailProperties[SelectedIndex]);
                //}
                return null;
            }
        }
    }
}
