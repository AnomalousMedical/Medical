using Engine;
using FreeImageAPI;
using Medical;
using Medical.Controller;
using MyGUIPlugin;
using System;
using System.Collections.Generic;
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
        private Dictionary<Slide, FreeImageBitmap> unsavedThumbs = new Dictionary<Slide, FreeImageBitmap>();
        private Dictionary<Slide, SceneThumbInfo> unsavedSceneThumbs = new Dictionary<Slide, SceneThumbInfo>();
        private MemoryCache<Slide, SceneThumbInfo> savedSceneThumbCache = new MemoryCache<Slide, SceneThumbInfo>();
        private SlideshowEditController slideEditController;
        private WorkQueue workQueue = new WorkQueue();

        public SlideImageManager(SlideshowEditController slideEditController)
        {
            this.slideEditController = slideEditController;
        }

        public void Dispose()
        {
            imageAtlas.Dispose();
            clearAllThumbs();
        }

        public void clear()
        {
            imageAtlas.clear();
            clearAllThumbs();
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
                                    var thumb = new FreeImageBitmap(stream);
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
        public FreeImageBitmap createThumbBitmap(Slide slide)
        {
            FreeImageBitmap thumb = new FreeImageBitmap(ThumbWidth, ThumbHeight, PixelFormat.Format32bppArgb);
            FreeImageBitmap oldThumb;
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
        public SceneThumbInfo loadThumbSceneBitmap(Slide slide, Func<SceneThumbInfo> fileNotFoundCallback = null)
        {
            SceneThumbInfo thumb;
            if (!unsavedSceneThumbs.TryGetValue(slide, out thumb) && !savedSceneThumbCache.TryGetValue(slide, out thumb))
            {
                String sceneThumbFile = slide.SceneThumbName;
                String sceneThumbInfoFile = slide.SceneThumbInfoName;
                if (slideEditController.ResourceProvider.fileExists(sceneThumbFile) && slideEditController.ResourceProvider.fileExists(sceneThumbInfoFile))
                {
                    using (Stream stream = slideEditController.ResourceProvider.openFile(sceneThumbInfoFile))
                    {
                        thumb = SharedXmlSaver.Load<SceneThumbInfo>(stream);
                    }

                    using (Stream stream = slideEditController.ResourceProvider.openFile(sceneThumbFile))
                    {
                        thumb.SceneThumb = new FreeImageBitmap(stream);
                    }
                    savedSceneThumbCache[slide] = thumb;
                }
                else
                {
                    if (fileNotFoundCallback != null)
                    {
                        thumb = fileNotFoundCallback();
                        unsavedSceneThumbs.Add(slide, thumb); //Since we had to generate it, consider this an unsaved thumb, we also know for sure here that the thumb does not already exist
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
        public void addUnsavedSceneThumb(Slide slide, SceneThumbInfo thumb)
        {
            SceneThumbInfo oldThumb;
            //Make sure we haven't loaded some other saved version of this thumb
            if (savedSceneThumbCache.TryGetValue(slide, out oldThumb))
            {
                savedSceneThumbCache.Remove(slide);
                oldThumb.Dispose();
            }

            //Make sure it wasn't already in unsavedSceneThumbs
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
                FreeImageBitmap thumb = unsavedThumbs[slide];
                saveThumbnail(Path.Combine(slide.UniqueName, Slideshow.SlideThumbName), thumb);
                thumb.Dispose();
            }
            unsavedThumbs.Clear();
            foreach (Slide slide in unsavedSceneThumbs.Keys)
            {
                var thumb = unsavedSceneThumbs[slide];
                saveSceneThumbInfo(slide, thumb);
                thumb.Dispose();
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
            FreeImageBitmap oldThumb;
            if (unsavedThumbs.TryGetValue(slide, out oldThumb))
            {
                oldThumb.Dispose();
                unsavedThumbs.Remove(slide);
            }
            SceneThumbInfo oldThumbInfo;
            if (unsavedSceneThumbs.TryGetValue(slide, out oldThumbInfo))
            {
                oldThumb.Dispose();
                unsavedSceneThumbs.Remove(slide);
            }
            if (savedSceneThumbCache.TryGetValue(slide, out oldThumbInfo))
            {
                oldThumb.Dispose();
                savedSceneThumbCache.Remove(slide);
            }
        }

        private void clearAllThumbs()
        {
            foreach (var thumb in unsavedThumbs.Values)
            {
                thumb.Dispose();
            }
            unsavedThumbs.Clear();
            foreach (SceneThumbInfo thumb in unsavedSceneThumbs.Values)
            {
                thumb.Dispose();
            }
            unsavedSceneThumbs.Clear();
            foreach (SceneThumbInfo thumb in savedSceneThumbCache.Values)
            {
                thumb.Dispose();
            }
            savedSceneThumbCache.Clear();
        }

        /// <summary>
        /// Save a thumbnail file
        /// </summary>
        /// <param name="slide"></param>
        /// <param name="thumb"></param>
        private void saveThumbnail(String fileName, FreeImageBitmap thumb)
        {
            try
            {
                using (Stream stream = slideEditController.ResourceProvider.openWriteStream(fileName))
                {
                    thumb.Save(stream, FREE_IMAGE_FORMAT.FIF_PNG);
                }
            }
            catch (Exception ex)
            {
                Logging.Log.Error("{0} exception updating thumbnail. Message: {1}", ex.GetType().Name, ex.Message);
            }
        }

        private void saveSceneThumbInfo(Slide slide, SceneThumbInfo thumbInfo)
        {
            try
            {
                using (Stream stream = slideEditController.ResourceProvider.openWriteStream(slide.SceneThumbInfoName))
                {
                    SharedXmlSaver.Save(thumbInfo, stream);
                }
                saveThumbnail(slide.SceneThumbName, thumbInfo.SceneThumb);
            }
            catch (Exception ex)
            {
                Logging.Log.Error("{0} exception updating thumbnail info. Message: {1}", ex.GetType().Name, ex.Message);
            }
        }
    }
}
