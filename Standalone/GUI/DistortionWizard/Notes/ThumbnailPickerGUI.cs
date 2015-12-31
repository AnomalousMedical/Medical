using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;
using Medical.Controller;
using FreeImageAPI;

namespace Medical.GUI.AnomalousMvc
{
    delegate void ThumbnailPickerGUIEvent(ThumbnailPickerGUI thumbPicker);

    class ThumbnailPickerGUI : IDisposable
    {
        private ImageAtlas thumbnailImages;
        private List<ThumbnailPickerInfo> thumbnailProperties = new List<ThumbnailPickerInfo>();
        private ImageRenderer imageRenderer;
        private SingleSelectButtonGrid imageGrid;

        public event ThumbnailPickerGUIEvent SelectedThumbnailChanged;

        public ThumbnailPickerGUI(ImageRenderer imageRenderer, ScrollView thumbnailScroll)
        {
            this.imageRenderer = imageRenderer;
            imageGrid = new SingleSelectButtonGrid(thumbnailScroll);
            imageGrid.SelectedValueChanged += new EventHandler(imageGrid_SelectedValueChanged);
            thumbnailImages = new ImageAtlas("ThumbnailPicker", new IntSize2(imageGrid.ItemWidth, imageGrid.ItemHeight));
        }

        public void Dispose()
        {
            imageGrid.Dispose();
            imageGrid = null;
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
                Coroutine.Start(doUpdateThumbnails());
            }
            else
            {
                throw new Exception("Cannot generate thumbnails for the Thumbnail Picker. Image Renderer is null.");
            }
        }

        private IEnumerator<YieldAction> doUpdateThumbnails()
        {
            imageGrid.clear();
            yield return Coroutine.WaitSeconds(0.1f);
            foreach (ThumbnailPickerInfo thumbProp in thumbnailProperties)
            {
                if (imageGrid == null)
                {
                    yield break;
                }

                String imageId;
                using (FreeImageBitmap thumb = imageRenderer.renderImage(thumbProp.ImageProperties))
                {
                    imageId = thumbnailImages.addImage(thumb, thumb);
                }

                imageGrid.SuppressLayout = true;
                ButtonGridItem item = imageGrid.addItem("Main", "", imageId);
                item.UserObject = thumbProp;

                imageGrid.SuppressLayout = false;
                imageGrid.layout();

                if (imageGrid.Count == 1) //Select first item
                {
                    imageGrid.SelectedItem = imageGrid.getItem(0);
                }

                yield return Coroutine.WaitSeconds(0.1f);
            }
        }

        public ImageRendererProperties SelectedThumbnailProperties
        {
            get
            {
                ThumbnailPickerInfo thumbInfo = (ThumbnailPickerInfo)imageGrid.SelectedItem.UserObject;
                if (thumbInfo != null)
                {
                    return thumbInfo.ImageProperties;
                }
                else if (imageGrid.Count > 0)
                {
                    thumbInfo = (ThumbnailPickerInfo)imageGrid.getItem(0).UserObject;
                    return thumbInfo.ImageProperties;
                }
                else
                {
                    return null;
                }
            }
        }

        void imageGrid_SelectedValueChanged(object sender, EventArgs e)
        {
            if (SelectedThumbnailChanged != null)
            {
                SelectedThumbnailChanged.Invoke(this);
            }
        }
    }
}
