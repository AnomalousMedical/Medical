using Engine.Threads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Medical.Controller
{
    public partial class BookmarksController //Note that this class is a private class in the BookmarksController since we don't want to expose it outside that class.
    {
        class LoadBookmarksBgTask : CancelableBackgroundWorkTask<ObjectBuffer<Bookmark>>
        {
            private BookmarksController controller;
            private CancelableBackgroundWorker<ObjectBuffer<Bookmark>> bgWorker;
            private bool processNewDirectoryOnCancel = false;
            private BookmarkPath currentPath;

            public LoadBookmarksBgTask(BookmarksController controller)
            {
                this.controller = controller;
                this.bgWorker = new CancelableBackgroundWorker<ObjectBuffer<Bookmark>>(this);
            }

            public void loadBookmarks(BookmarkPath path)
            {
                this.currentPath = path;
                this.CanDoWork = !controller.bookmarksResourceProvider.CanWrite || controller.bookmarksResourceProvider.directoryExists(currentPath.Path);
                bgWorker.startWork();
            }

            public void cancel()
            {
                bgWorker.cancel();
            }

            public bool CanDoWork { get; private set; }

            public IEnumerable<ObjectBuffer<Bookmark>> WorkUnits
            {
                get
                {
                    ObjectBuffer<Bookmark> bookmarks = new ObjectBuffer<Bookmark>(5);
                    foreach (String file in controller.bookmarksResourceProvider.listFiles("*.bmk", currentPath.Path, false))
                    {
                        Bookmark bookmark;
                        using (XmlTextReader xmlReader = new XmlTextReader(controller.bookmarksResourceProvider.openFile(file)))
                        {
                            try
                            {
                                bookmark = xmlSaver.restoreObject(xmlReader) as Bookmark;
                            }
                            catch (Exception ex)
                            {
                                bookmark = null;
                                Logging.Log.Error("{0} loading bookmark '{1}'. Skipping this bookmark. Message: {2}", ex.GetType().Name, file, ex.Message);
                            }
                        }
                        if (bookmark != null)
                        {
                            bookmark.BackingFile = file;
                            if(bookmarks.addItem(bookmark))
                            {
                                yield return bookmarks;
                            }
                        }
                    }
                    if(bookmarks.HasItems)
                    {
                        yield return bookmarks;
                    }
                }
            }

            public int Progress { get; private set; }

            public void resetDisplay()
            {
                processNewDirectoryOnCancel = false;
                controller.clearBookmarks();
            }

            public void processingStarted()
            {
                
            }

            public void workProcessed(ObjectBuffer<Bookmark> item)
            {
                foreach(Bookmark bookmark in item.Items)
                {
                    controller.fireBookmarkAdded(bookmark);
                }
                item.reset();
            }

            public void updateProgress(int progressPercentage)
            {
                
            }

            public void runWorkCompleted()
            {
                
            }

            public bool canceled()
            {
                return processNewDirectoryOnCancel;
            }

            public void startedWhileBusy()
            {
                processNewDirectoryOnCancel = true;
            }
        }
    }
}
