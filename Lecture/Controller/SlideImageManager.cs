using Engine;
using Medical;
using Medical.Controller;
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
    /// <summary>
    /// This class manages the lifecycle for slideshow images.
    /// </summary>
    public class SlideImageManager : IDisposable
    {
        /// <summary>
        /// Called before a thumbnail is updated in the image atlas (This is mostly a mygui hack as it gets unstable if images are removed from it while they are still being used.)
        /// </summary>
        public event Action<Slide> ThumbUpdating;

        /// <summary>
        /// Called when the thumbnail for a slide is updated. Passes the name of the image along.
        /// </summary>
        public event Action<Slide, String> ThumbUpdated;

        public static readonly int ThumbWidth = ScaleHelper.Scaled(183);
        public static readonly int ThumbHeight = ScaleHelper.Scaled(101);

        private ImageAtlas imageAtlas = new ImageAtlas("SlideThumbs", new IntSize2(ThumbWidth, ThumbHeight));
        private Dictionary<Slide, Bitmap> unsavedThumbs = new Dictionary<Slide, Bitmap>();
        private SlideshowEditController slideEditController;
        private WorkQueue workQueue = new WorkQueue();

        public SlideImageManager(SlideshowEditController slideEditController)
        {
            this.slideEditController = slideEditController;
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

        public void loadThumbnail(Slide slide, Action<Slide, String> loadedCallback)
        {
            String id = imageAtlas.getImageId(slide.UniqueName);
            if (id != null)
            {
                loadedCallback(slide, id);
            }
            else
            {
                workQueue.enqueue(() =>
                    {
                        String thumbPath = Path.Combine(slide.UniqueName, Slideshow.SlideThumbName);
                        try
                        {
                            if (slideEditController.ResourceProvider.exists(thumbPath))
                            {
                                using (Stream stream = slideEditController.ResourceProvider.openFile(thumbPath))
                                {
                                    Image thumb = Bitmap.FromStream(stream);
                                    ThreadManager.invoke(new Action(() =>
                                        {
                                            try
                                            {
                                                if (!imageAtlas.containsImage(slide.UniqueName))
                                                {
                                                    loadedCallback(slide, imageAtlas.addImage(slide.UniqueName, thumb));
                                                }
                                            }
                                            finally
                                            {
                                                thumb.Dispose();
                                            }
                                        }));
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Logging.Log.Error("Could not load thumbnail because of {0} exception.\nReason: {1}", ex.GetType(), ex.Message);
                        }
                    });
            }
        }

        public String getThumbnailId(Slide slide)
        {
            return imageAtlas.getImageId(slide.UniqueName);
        }

        /// <summary>
        /// Call this function to make a new thumbnail bitmap for a slide.
        /// </summary>
        /// <param name="slide"></param>
        /// <returns></returns>
        public Bitmap createThumbBitmap(Slide slide)
        {
            Bitmap thumb = new Bitmap(ThumbWidth, ThumbHeight);
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
            return thumb;
        }

        /// <summary>
        /// Call this function if you updated the thumbnail for a slide.
        /// </summary>
        /// <param name="slide"></param>
        public void thumbnailUpdated(Slide slide)
        {
            if (ThumbUpdating != null)
            {
                ThumbUpdating.Invoke(slide);
            }
            imageAtlas.removeImage(slide.UniqueName);
            String imageKey = imageAtlas.addImage(slide.UniqueName, unsavedThumbs[slide]);
            if (ThumbUpdated != null)
            {
                ThumbUpdated.Invoke(slide, imageKey);
            }
        }

        /// <summary>
        /// This will save all outstanding thumbnails to the disk.
        /// </summary>
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

        /// <summary>
        /// Remove an image for a given slide.
        /// </summary>
        /// <param name="slide"></param>
        public void removeImage(Slide slide)
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
    }
}
