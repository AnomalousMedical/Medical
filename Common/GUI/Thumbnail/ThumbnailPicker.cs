using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ComponentFactory.Krypton.Ribbon;
using System.Windows.Forms;
using System.Drawing;

namespace Medical.GUI
{
    public class ThumbnailPicker : KryptonGallery
    {
        private static Engine.Color BACK_COLOR = new Engine.Color(.94f, .94f, .94f);

        private ImageList thumbnailImages = new ImageList();
        private List<ImageRendererProperties> thumbnailProperties = new List<ImageRendererProperties>();
        private List<Bitmap> currentImages = new List<Bitmap>();
        private ImageRenderer imageRenderer;

        public ThumbnailPicker()
        {
            thumbnailImages.ColorDepth = ColorDepth.Depth32Bit;
            thumbnailImages.ImageSize = new Size(100, 100);
            this.ImageList = thumbnailImages;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
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
            imageProp.Width = thumbnailImages.ImageSize.Width;
            imageProp.Height = thumbnailImages.ImageSize.Height;
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

            thumbnailProperties.Add(imageProp);
        }

        /// <summary>
        /// Update all thumbnails in the picker.
        /// </summary>
        public void updateThumbnails()
        {
            if (imageRenderer != null)
            {
                foreach (Bitmap bitmap in currentImages)
                {
                    bitmap.Dispose();
                }
                thumbnailImages.Images.Clear();
                currentImages.Clear();
                foreach (ImageRendererProperties imageProperties in thumbnailProperties)
                {
                    Bitmap thumb = imageRenderer.renderImage(imageProperties);
                    thumbnailImages.Images.Add(thumb);
                    currentImages.Add(thumb);
                }
                this.ImageList = thumbnailImages;
                if (thumbnailImages.Images.Count > 0)
                {
                    this.SelectedIndex = 0;
                }
            }
            else
            {
                throw new Exception("Cannot generate thumbnails for the Thumbnail Picker. Image Renderer is null.");
            }
        }

        /// <summary>
        /// The ImageRenderer to use to render images.
        /// </summary>
        public ImageRenderer ImageRenderer
        {
            get
            {
                return imageRenderer;
            }
            set
            {
                imageRenderer = value;
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
                if (SelectedIndex > 0 && SelectedIndex < currentImages.Count)
                {
                    return imageRenderer.renderImage(thumbnailProperties[SelectedIndex]);
                }
                return null;
            }
        }
    }
}
