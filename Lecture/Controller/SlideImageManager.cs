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

        public static readonly int SceneThumbWidth = ScaleHelper.Scaled(250);
        public static readonly int SceneThumbHeight = ScaleHelper.Scaled(250);

        private ImageAtlas imageAtlas = new ImageAtlas("SlideThumbs", new IntSize2(ThumbWidth, ThumbHeight));
        private Dictionary<Slide, Bitmap> unsavedThumbs = new Dictionary<Slide, Bitmap>();
        private Dictionary<Slide, Bitmap> unsavedSceneThumbs = new Dictionary<Slide, Bitmap>();
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
        /// Call this function to make a new scene thumbnail bitmap for a slide.
        /// </summary>
        /// <param name="slide"></param>
        /// <param name="fileNotFoundCallback">This function is called if the bitmap file is not found. It should return a bitmap that will have control taken by this class, it will be treated as an unsaved scene thumb, and will be disposed by this class.</param>
        /// <returns></returns>
        public Bitmap loadThumbSceneBitmap(Slide slide, Func<Bitmap> fileNotFoundCallback = null)
        {
            Bitmap thumb;
            if (!unsavedSceneThumbs.TryGetValue(slide, out thumb))
            {
                String sceneThumbFile = slide.SceneThumbName;
                if (slideEditController.ResourceProvider.fileExists(sceneThumbFile))
                {
                    using (Stream stream = slideEditController.ResourceProvider.openFile(sceneThumbFile))
                    {
                        thumb = (Bitmap)Bitmap.FromStream(stream);
                        unsavedSceneThumbs.Add(slide, thumb); //THIS IS DUMB, PUT IT IN ANOTHER DICTIONARY, these already exist, need a dictionary for the existing ones so we don't keep loading from file
                        //limit the size of it to the last 10 loaded, and make sure when adding or replacing an unsaved one that you remove any entries in this dictionary associated with that slide.
                    }
                }
                else
                {
                    if (fileNotFoundCallback != null)
                    {
                        thumb = fileNotFoundCallback();
                        unsavedSceneThumbs.Add(slide, thumb); //Since we had to generate it, consider this an unsaved thumb
                    }
                    else
                    {
                        throw new FileNotFoundException(sceneThumbFile);
                    }
                }
            }
            return thumb;
        }

        /// <summary>
        /// Add a bitmap to act as a scene thumb for a slide. This class will take control of the 
        /// bitmap and dispose it when it is done.
        /// </summary>
        /// <param name="slide"></param>
        /// <param name="thumb"></param>
        public void addUnsavedSceneThumb(Slide slide, Bitmap thumb)
        {
            Bitmap oldThumb;
            if (unsavedSceneThumbs.TryGetValue(slide, out oldThumb))
            {
                oldThumb.Dispose();
                unsavedSceneThumbs[slide] = thumb;
            }
            else
            {
                unsavedSceneThumbs.Add(slide, thumb);
            }
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
                saveThumbnail(Path.Combine(slide.UniqueName, Slideshow.SlideThumbName), thumb);
            }
            unsavedThumbs.Clear();
            foreach (Slide slide in unsavedSceneThumbs.Keys)
            {
                Bitmap thumb = unsavedSceneThumbs[slide];
                saveThumbnail(slide.SceneThumbName, thumb);
            }
            unsavedSceneThumbs.Clear();
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
            if (unsavedSceneThumbs.TryGetValue(slide, out oldThumb))
            {
                oldThumb.Dispose();
                unsavedSceneThumbs.Remove(slide);
            }
        }

        private void clearUnsavedThumbs()
        {
            foreach (Bitmap thumb in unsavedThumbs.Values)
            {
                thumb.Dispose();
            }
            unsavedThumbs.Clear();
            foreach (Bitmap thumb in unsavedSceneThumbs.Values)
            {
                thumb.Dispose();
            }
            unsavedSceneThumbs.Clear();
        }

        /// <summary>
        /// Save a thumbnail, will dispose the bitmap passed in.
        /// </summary>
        /// <param name="slide"></param>
        /// <param name="thumb"></param>
        private void saveThumbnail(String fileName, Bitmap thumb)
        {
            try
            {
                using (Stream stream = slideEditController.ResourceProvider.openWriteStream(fileName))
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
    }
}
