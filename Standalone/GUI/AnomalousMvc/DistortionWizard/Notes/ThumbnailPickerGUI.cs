﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using MyGUIPlugin;
using Engine;
using Medical.Controller;

namespace Medical.GUI.AnomalousMvc
{
    class ThumbnailPickerGUI : IDisposable
    {
        private ImageAtlas thumbnailImages;
        private List<ThumbnailPickerInfo> thumbnailProperties = new List<ThumbnailPickerInfo>();
        private ImageRenderer imageRenderer;
        private ButtonGrid imageGrid;

        public ThumbnailPickerGUI(ImageRenderer imageRenderer, ScrollView thumbnailScroll)
        {
            this.imageRenderer = imageRenderer;
            imageGrid = new ButtonGrid(thumbnailScroll);
            thumbnailImages = new ImageAtlas("ThumbnailPicker", new Size2(imageGrid.ItemWidth, imageGrid.ItemHeight), new Size2(512, 512));
        }

        public void Dispose()
        {
            thumbnailImages.Dispose();
        }

        /// <summary>
        /// Add a thumbnail to the picker.
        /// </summary>
        public void addThumbnail(NotesThumbnail thumbnail)
        {
            thumbnailProperties.Add(new ThumbnailPickerInfo((int)thumbnailImages.ImageSize.Width, (int)thumbnailImages.ImageSize.Height, thumbnail));
        }

        /// <summary>
        /// Update all thumbnails in the picker.
        /// </summary>
        public void updateThumbnails()
        {
            if (imageRenderer != null)
            {
                thumbnailImages.clear();
                imageGrid.SuppressLayout = true;
                imageGrid.clear();
                foreach (ThumbnailPickerInfo thumbProp in thumbnailProperties)
                {
                    using (Bitmap thumb = imageRenderer.renderImage(thumbProp.ImageProperties))
                    {
                        String imageId = thumbnailImages.addImage(thumb, thumb);
                        ButtonGridItem item = imageGrid.addItem("Main", "", imageId);
                        item.UserObject = thumbProp;
                    }
                }
                if (imageGrid.Count > 0)
                {
                    imageGrid.SelectedItem = imageGrid.getItem(0);
                }
                imageGrid.SuppressLayout = false;
                imageGrid.layout();
            }
            else
            {
                throw new Exception("Cannot generate thumbnails for the Thumbnail Picker. Image Renderer is null.");
            }
        }
    }
}
