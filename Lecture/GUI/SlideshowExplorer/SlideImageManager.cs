using Engine;
using Medical;
using MyGUIPlugin;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lecture
{
    class SlideImageManager : IDisposable
    {
        public const int ThumbWidth = 183;
        public const int ThumbHeight = 101;

        private ImageAtlas imageAtlas = new ImageAtlas("SlideThumbs", new Size2(ThumbWidth, ThumbHeight), new Size2(512, 512));
        private Dictionary<Slide, Bitmap> unsavedThumbs = new Dictionary<Slide, Bitmap>();
        private SlideshowEditController slideEditController;

        public SlideImageManager(SlideshowEditController slideEditController)
        {
            this.slideEditController = slideEditController;
            slideEditController.Saved += slideEditController_Saved;
        }

        public void Dispose()
        {
            imageAtlas.Dispose();
            clearUnsavedThumbs();
        }

        public void clear()
        {
            imageAtlas.clear();
            clearUnsavedThumbs();
        }

        public String loadThumbnail(Slide slide)
        {
            String thumbPath = Path.Combine(slide.UniqueName, Slideshow.SlideThumbName);
            try
            {
                if (slideEditController.ResourceProvider.exists(thumbPath))
                {
                    using (Stream stream = slideEditController.ResourceProvider.openFile(thumbPath))
                    {
                        using (Image thumb = Bitmap.FromStream(stream))
                        {
                            return imageAtlas.addImage(slide.UniqueName, thumb);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.Log.Error("Could not load thumbnail because of {0} exception.\nReason: {1}", ex.GetType(), ex.Message);
            }
            return null;
        }

        public String thumbnailUpdated(Slide slide, Bitmap thumb)
        {
            Bitmap oldThumb;
            if (unsavedThumbs.TryGetValue(slide, out oldThumb))
            {
                oldThumb.Dispose();
                unsavedThumbs[slide] = thumb;
            }
            else
            {
                unsavedThumbs.Add(slide, thumb);
            }
            imageAtlas.removeImage(slide.UniqueName);
            return imageAtlas.addImage(slide.UniqueName, thumb);
        }

        public void saveThumbnails()
        {
            foreach (Slide slide in unsavedThumbs.Keys)
            {
                Bitmap thumb = unsavedThumbs[slide];
                try
                {
                    using (Stream stream = slideEditController.ResourceProvider.openWriteStream(Path.Combine(slide.UniqueName, Slideshow.SlideThumbName)))
                    {
                        thumb.Save(stream, ImageFormat.Png);
                    }
                }
                catch (Exception ex)
                {
                    Logging.Log.Error("{0} exception updating thumbnail. Message: {1}", ex.GetType().Name, ex.Message);
                }
                thumb.Dispose();
            }
            unsavedThumbs.Clear();
        }

        internal void removeImage(Slide slide)
        {
            imageAtlas.removeImage(slide.UniqueName);
            Bitmap oldThumb;
            if (unsavedThumbs.TryGetValue(slide, out oldThumb))
            {
                oldThumb.Dispose();
                unsavedThumbs.Remove(slide);
            }
        }

        private void clearUnsavedThumbs()
        {
            foreach (Bitmap thumb in unsavedThumbs.Values)
            {
                thumb.Dispose();
            }
            unsavedThumbs.Clear();
        }

        void slideEditController_Saved()
        {
            //saveThumbnails();
            //This almost works, for now it is commented so slides will never update on disk. Just want to commit the changes made so far.
        }
    }
}
