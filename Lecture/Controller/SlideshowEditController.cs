using Engine.Saving;
using Medical;
using Medical.Controller;
using Medical.Controller.AnomalousMvc;
using Medical.GUI;
using MyGUIPlugin;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace Lecture
{
    public class SlideshowEditController
    {
        public event Action<Slideshow> SlideshowLoaded;
        public event Action SlideshowClosed;
        public event Action<Slide, int> SlideAdded;
        public event Action<Slide> SlideRemoved;
        public event Action RequestRemoveSelected;
        public event Action<AnomalousMvcContext> SlideshowContextStarting;
        public event Action<AnomalousMvcContext> SlideshowContextBlurred;

        public delegate void SelectSlides(Slide primary, IEnumerable<Slide> secondary);
        public event SelectSlides SlideSelected;

        private UndoRedoBuffer undoBuffer = new UndoRedoBuffer(50);

        //Editor Contexts
        private SlideEditorContext slideEditorContext;

        private StandaloneController standaloneController;
        private PropEditController propEditController;
        private EditorController editorController;
        private ShowTypeController showTypeController;
        private LectureUICallback uiCallback;
        private Slideshow slideshow;
        private ImageRenderer imageRenderer;
        private MedicalSlideItemTemplate medicalSlideTemplate;
        private SlideImageManager slideImageManager;
        private TimelineTypeController timelineTypeController;
        private TimelineEditorContext timelineEditorContext;
        private TimelineController timelineController;

        private bool allowUndoCreation = true;
        private Slide lastEditSlide = null;

        public SlideshowEditController(StandaloneController standaloneController, LectureUICallback uiCallback, PropEditController propEditController, EditorController editorController, TimelineController timelineController)
        {
            this.standaloneController = standaloneController;
            this.uiCallback = uiCallback;
            this.propEditController = propEditController;
            this.editorController = editorController;
            this.imageRenderer = standaloneController.ImageRenderer;
            this.timelineController = timelineController;
            editorController.ProjectChanged += editorController_ProjectChanged;
            slideImageManager = new SlideImageManager(this);

            //Show Type Controller
            showTypeController = new ShowTypeController(editorController);
            editorController.addTypeController(showTypeController);
            timelineTypeController = new TimelineTypeController(editorController);
            editorController.addTypeController(timelineTypeController);

            medicalSlideTemplate = new MedicalSlideItemTemplate(standaloneController.SceneViewController, standaloneController.MedicalStateController);
            medicalSlideTemplate.SlideCreated += (slide) =>
                {
                    if (lastEditSlide != null)
                    {
                        int insertIndex = slideshow.indexOf(lastEditSlide);
                        if (insertIndex != -1)
                        {
                            ++insertIndex;
                        }
                        addSlide(slide, insertIndex);
                    }
                    else
                    {
                        addSlide(slide);
                    }
                };
            editorController.addItemTemplate(medicalSlideTemplate);
        }

        public void editSlide(Slide slide)
        {
            bool openedEditContext = false;
            if (slide != lastEditSlide)
            {
                openedEditContext = openEditorContextForSlide(slide);
            }
            else
            {
                if (slideEditorContext != null)
                {
                    slideEditorContext.slideNameChanged("Slide " + (slideshow.indexOf(slide) + 1));
                }
                else
                {
                    openEditorContextForSlide(slide); //If the slide context is null the timeline editor is open, switch back to slide editing.
                    //We ignore the context open result, because we do not care about undo in this situation
                }
            }

            if (openedEditContext)
            {
                if (lastEditSlide != null && allowUndoCreation)
                {
                    undoBuffer.pushAndSkip(new TwoWayDelegateCommand<Slide, Slide>(
                        (redoItem) =>
                        {
                            //Hacky, but we cannot modify the active slide without messing up the classes that triggered this.
                            ThreadManager.invoke(new Action(delegate()
                            {
                                allowUndoCreation = false;
                                if (SlideSelected != null)
                                {
                                    SlideSelected.Invoke(redoItem, IEnumerableUtil<Slide>.EmptyIterator);
                                }
                                allowUndoCreation = true;
                            }));
                        },
                        slide,
                        (undoItem) =>
                        {
                            //Hacky, but we cannot modify the active slide without messing up the classes that triggered this.
                            ThreadManager.invoke(new Action(delegate()
                            {
                                allowUndoCreation = false;
                                if (SlideSelected != null)
                                {
                                    SlideSelected.Invoke(undoItem, IEnumerableUtil<Slide>.EmptyIterator);
                                }
                                allowUndoCreation = true;
                            }));
                        },
                        lastEditSlide)
                    );
                }
                lastEditSlide = slide;
                uiCallback.CurrentDirectory = slide.UniqueName;
            }
        }

        private bool openEditorContextForSlide(Slide slide)
        {
            bool openedEditContext = false;
            //This is done like this so we could have multiple slide types besides MedicalRmlSlide
            if (slide is MedicalRmlSlide)
            {
                MedicalRmlSlide medicalSlide = (MedicalRmlSlide)slide;
                slideEditorContext = new SlideEditorContext(medicalSlide, "Slide " + (slideshow.indexOf(slide) + 1), this, uiCallback, undoBuffer, imageRenderer, medicalSlideTemplate, (panelName, rml) =>
                {
                    slideEditorContext.setWysiwygRml(panelName, rml, true);
                });
                if (standaloneController.SharePluginController != null)
                {
                    CallbackTask cleanupBeforeShareTask = new CallbackTask("Lecture.SharePluginTask", standaloneController.SharePluginController.Name, standaloneController.SharePluginController.IconName, standaloneController.SharePluginController.Category, 0, false, (item) =>
                    {
                        shareSlideshow();
                    });
                    slideEditorContext.addTask(cleanupBeforeShareTask);
                }
                slideEditorContext.Focus += (obj) =>
                {
                    slideEditorContext = obj;
                };
                slideEditorContext.Blur += obj =>
                {
                    if (slideEditorContext == obj)
                    {
                        slideEditorContext.RecordResizeUndo -= slideEditorContext_RecordResizeUndo;
                        slideEditorContext = null;
                    }
                };
                slideEditorContext.RecordResizeUndo += slideEditorContext_RecordResizeUndo;
                editorController.runEditorContext(slideEditorContext.MvcContext);
                openedEditContext = true;
            }
            return openedEditContext;
        }

        void slideEditorContext_RecordResizeUndo(RmlEditorViewInfo view, int oldSize, int newSize)
        {
            String panelName = view.View.Name;
            Action<int> changeSize = (size) =>
                {
                    slideEditorContext.resizePanel(panelName, size);
                };

            undoBuffer.pushAndSkip(new TwoWayDelegateCommand<int, int>(changeSize, newSize, changeSize, oldSize));
        }

        public void editTimeline(Slide slide, String fileName, String text)
        {
            try
            {
                Timeline timeline = null;
                String timelineFilePath = Path.Combine(slide.UniqueName, fileName);
                if (!ResourceProvider.exists(timelineFilePath))
                {
                    timelineTypeController.createNewTimeline(timelineFilePath);
                }
                timeline = editorController.loadFile<Timeline>(timelineFilePath); //By loading after creating we ensure this is in the cached resources

                propEditController.removeAllOpenProps();
                timelineEditorContext = new TimelineEditorContext(timeline, slide, String.Format("Slide {0} - {1}", slideshow.indexOf(slide) + 1, text), this, propEditController, editorController, uiCallback, timelineController);
                timelineEditorContext.Focus += obj =>
                {
                    timelineEditorContext = obj;
                };
                timelineEditorContext.Blur += obj =>
                {
                    if (obj == timelineEditorContext)
                    {
                        timelineEditorContext = null;
                    }
                };
                editorController.runEditorContext(timelineEditorContext.MvcContext);
            }
            catch (Exception ex)
            {
                MessageBox.show(String.Format("Error opening timeline for editing.\n{0}", ex.Message), "Load Error", MessageBoxStyle.IconError | MessageBoxStyle.Ok);
            }
        }

        class SlideInfo
        {
            public SlideInfo(Slide slide, int index)
            {
                this.Slide = slide;
                this.Index = index;
            }

            public Slide Slide { get; set; }

            public int Index { get; set; }

            public static int Sort(SlideInfo left, SlideInfo right)
            {
                return left.Index - right.Index;
            }
        }

        class RemoveSlideInfo : SlideInfo
        {
            public RemoveSlideInfo(Slide slide, int index, Slide changeToSlide)
                :base(slide, index)
            {
                this.ChangeToSlide = changeToSlide;
            }

            public Slide ChangeToSlide { get; set; }
        }

        public void removeSelectedSlides()
        {
            if (RequestRemoveSelected != null)
            {
                RequestRemoveSelected.Invoke();
            }
        }

        public void removeSlides(IEnumerable<Slide> slides, Slide primarySelection)
        {
            int slideIndex = slideshow.indexOf(primarySelection);
            List<SlideInfo> removedSlides = new List<SlideInfo>(from slide in slides select new SlideInfo(slide, slideshow.indexOf(slide)));
            if (removedSlides.Count > 0)
            {
                removedSlides.Sort(SlideInfo.Sort);

                doRemoveSlides(removedSlides);

                Slide changeToSlide = null;
                if (primarySelection == lastEditSlide)
                {
                    if (slideIndex < slideshow.Count)
                    {
                        changeToSlide = slideshow.get(slideIndex);
                    }
                    else if (slideIndex - 1 >= 0)
                    {
                        changeToSlide = slideshow.get(slideIndex - 1);
                    }
                }

                bool wasAllowingUndo = allowUndoCreation;
                allowUndoCreation = false;
                if (changeToSlide != null && SlideSelected != null)
                {
                    SlideSelected.Invoke(changeToSlide, IEnumerableUtil<Slide>.EmptyIterator);
                }
                allowUndoCreation = wasAllowingUndo;

                if (allowUndoCreation)
                {
                    undoBuffer.pushAndSkip(new TwoWayDelegateCommand(
                        () => //Execute
                        {
                            ThreadManager.invoke(new Action(() =>
                            {
                                allowUndoCreation = false;
                                doRemoveSlides(removedSlides);
                                if (changeToSlide != null && SlideSelected != null)
                                {
                                    SlideSelected.Invoke(changeToSlide, IEnumerableUtil<Slide>.EmptyIterator);
                                }
                                allowUndoCreation = true;
                            }));
                        },
                        () => //Undo
                        {
                            ThreadManager.invoke(new Action(() =>
                            {
                                allowUndoCreation = false;
                                foreach (SlideInfo slide in removedSlides)
                                {
                                    slideshow.insertSlide(slide.Index, slide.Slide);
                                    if (SlideAdded != null)
                                    {
                                        SlideAdded.Invoke(slide.Slide, slide.Index);
                                    }
                                }
                                if (SlideSelected != null)
                                {
                                    SlideSelected.Invoke(primarySelection, secondarySlideSelections(removedSlides, primarySelection));
                                }
                                allowUndoCreation = true;
                            }));
                        },
                        poppedFrontFunc: () =>
                        {
                            foreach (SlideInfo slideInfo in removedSlides)
                            {
                                cleanupThumbnail(slideInfo);
                            }
                        }));
                }
            }
        }

        private void doRemoveSlides(List<SlideInfo> removedSlides)
        {
            bool wasAllowingUndo = allowUndoCreation;
            allowUndoCreation = false;
            foreach (SlideInfo slideInfo in removedSlides)
            {
                slideshow.removeSlide(slideInfo.Slide);
                if (SlideRemoved != null)
                {
                    SlideRemoved.Invoke(slideInfo.Slide);
                }
            }
            allowUndoCreation = wasAllowingUndo;
        }

        public void removeSlide(Slide slide)
        {
            int slideIndex = slideshow.indexOf(slide);
            if (slideIndex != -1)
            {
                slideshow.removeAt(slideIndex);
                if (SlideRemoved != null)
                {
                    SlideRemoved.Invoke(slide);
                }

                Slide changeToSlide = null;
                if (slide == lastEditSlide)
                {
                    if (slideIndex < slideshow.Count)
                    {
                        changeToSlide = slideshow.get(slideIndex);
                    }
                    else if (slideIndex - 1 >= 0)
                    {
                        changeToSlide = slideshow.get(slideIndex - 1);
                    }
                }

                if (changeToSlide != null)
                {
                    bool wasAllowingUndo = allowUndoCreation;
                    allowUndoCreation = false;
                    if (SlideSelected != null)
                    {
                        SlideSelected.Invoke(changeToSlide, IEnumerableUtil<Slide>.EmptyIterator);
                    }
                    allowUndoCreation = wasAllowingUndo;
                }

                if (allowUndoCreation)
                {
                    if (changeToSlide == null)
                    {
                        undoBuffer.pushAndSkip(new TwoWayDelegateCommand<SlideInfo>(
                        (executeSlide) =>
                        {
                            allowUndoCreation = false;
                            removeSlide(executeSlide.Slide);
                            allowUndoCreation = true;
                        },
                        (undoSlide) =>
                        {
                            allowUndoCreation = false;
                            addSlide(undoSlide.Slide, undoSlide.Index);
                            allowUndoCreation = true;
                        },
                        new SlideInfo(slide, slideIndex)));
                    }
                    else
                    {
                        undoBuffer.pushAndSkip(new TwoWayDelegateCommand<RemoveSlideInfo>((executeSlide) =>
                        {
                            //Hacky, but we cannot modify the active slide without messing up the classes that triggered this.
                            ThreadManager.invoke(new Action(delegate()
                            {
                                allowUndoCreation = false;
                                removeSlide(executeSlide.Slide);
                                if (SlideSelected != null)
                                {
                                    SlideSelected.Invoke(executeSlide.ChangeToSlide, IEnumerableUtil<Slide>.EmptyIterator);
                                }
                                allowUndoCreation = true;
                            }));
                        },
                        (undoSlide) =>
                        {
                            //Hacky, but we cannot modify the active slide without messing up the classes that triggered this.
                            ThreadManager.invoke(new Action(delegate()
                            {
                                allowUndoCreation = false;
                                addSlide(undoSlide.Slide, undoSlide.Index);
                                if (SlideSelected != null)
                                {
                                    SlideSelected.Invoke(undoSlide.Slide, IEnumerableUtil<Slide>.EmptyIterator);
                                }
                                allowUndoCreation = true;
                            }));
                        },
                        new RemoveSlideInfo(slide, slideIndex, changeToSlide),
                        poppedFrontFunc: cleanupThumbnail
                        ));
                    }
                }
            }
        }

        public void addSlide(Slide slide, int index = -1)
        {
            if (index == -1)
            {
                slideshow.addSlide(slide);
            }
            else
            {
                slideshow.insertSlide(index, slide);
            }

            if (!editorController.ResourceProvider.exists(slide.UniqueName))
            {
                editorController.ResourceProvider.createDirectory("", slide.UniqueName);
            }

            if (SlideAdded != null)
            {
                SlideAdded.Invoke(slide, index);
            }

            //Delay this till the next frame, so the rml has actually been rendererd
            ThreadManager.invoke(new Action(delegate()
            {
                if (slideEditorContext != null)
                {
                    slideEditorContext.updateThumbnail();
                }
            }));

            if (allowUndoCreation)
            {
                if (lastEditSlide == null)
                {
                    undoBuffer.pushAndSkip(new TwoWayDelegateCommand<SlideInfo>((executeSlide) =>
                    {
                        allowUndoCreation = false;
                        addSlide(executeSlide.Slide, executeSlide.Index);
                        allowUndoCreation = true;
                    },
                    (undoSlide) =>
                    {
                        allowUndoCreation = false;
                        removeSlide(undoSlide.Slide);
                        allowUndoCreation = true;
                    },
                    new SlideInfo(slide, slideshow.indexOf(slide)),
                    trimmedFunc: cleanupThumbnail
                    ));
                }
                else
                {
                    undoBuffer.pushAndSkip(new TwoWayDelegateCommand<RemoveSlideInfo>(
                    (executeSlide) =>
                    {
                        //Hacky, but we cannot modify the active slide without messing up the classes that triggered this.
                        ThreadManager.invoke(new Action(delegate()
                        {
                            allowUndoCreation = false;
                            addSlide(executeSlide.Slide, executeSlide.Index);
                            if (SlideSelected != null)
                            {
                                SlideSelected.Invoke(executeSlide.Slide, IEnumerableUtil<Slide>.EmptyIterator);
                            }
                            allowUndoCreation = true;
                        }));
                    },
                    (undoSlide) =>
                    {
                        //Hacky, but we cannot modify the active slide without messing up the classes that triggered this.
                        ThreadManager.invoke(new Action(delegate()
                        {
                            allowUndoCreation = false;
                            if (SlideSelected != null)
                            {
                                SlideSelected.Invoke(undoSlide.ChangeToSlide, IEnumerableUtil<Slide>.EmptyIterator);
                            }
                            removeSlide(undoSlide.Slide);
                            allowUndoCreation = true;
                        }));
                    },
                    new RemoveSlideInfo(slide, index, lastEditSlide),
                    trimmedFunc: cleanupThumbnail
                    ));
                }
            }

            bool wasAllowingUndo = allowUndoCreation;
            allowUndoCreation = false;
            if (SlideSelected != null)
            {
                SlideSelected.Invoke(slide, IEnumerableUtil<Slide>.EmptyIterator);
            }
            allowUndoCreation = wasAllowingUndo;
        }

        public void moveSlides(IEnumerable<Slide> slides, int index)
        {
            List<SlideInfo> sortedSlides = new List<SlideInfo>(from slide in slides select new SlideInfo(slide, slideshow.indexOf(slide)));
            sortedSlides.Sort(SlideInfo.Sort);

            bool wasAllowingUndo = allowUndoCreation;
            allowUndoCreation = false;
            doMoveSlides(index, sortedSlides);
            allowUndoCreation = wasAllowingUndo;

            if (allowUndoCreation)
            {
                undoBuffer.pushAndSkip(new TwoWayDelegateCommand(
                    () => //Execute
                    {
                        allowUndoCreation = false;
                        doMoveSlides(index, sortedSlides);
                        allowUndoCreation = true;
                    },
                    () => //Undo
                    {
                        allowUndoCreation = false;
                        foreach (SlideInfo info in sortedSlides)
                        {
                            int formerIndex = slideshow.indexOf(info.Slide);
                            slideshow.removeAt(formerIndex);
                            if (SlideRemoved != null)
                            {
                                SlideRemoved.Invoke(info.Slide);
                            }
                        }
                        //Can't think of how to do this without two loops, have to compensate for other things
                        //that need to be undone or else this won't put things back, two loops makes sure
                        //all items are removed and we can just insert back to original indices.
                        foreach (SlideInfo info in sortedSlides)
                        {
                            slideshow.insertSlide(info.Index, info.Slide);
                            if (SlideAdded != null)
                            {
                                SlideAdded.Invoke(info.Slide, info.Index);
                            }
                        }
                        if (SlideSelected != null)
                        {
                            SlideSelected.Invoke(lastEditSlide, secondarySlideSelections(sortedSlides, lastEditSlide));
                        }
                        allowUndoCreation = true;
                    }));
            }
        }

        private void doMoveSlides(int index, List<SlideInfo> sortedSlides)
        {
            int actualInsertIndex = index;
            foreach (SlideInfo slideInfo in sortedSlides)
            {
                if (slideInfo.Index <= index && actualInsertIndex > 0)
                {
                    --actualInsertIndex;
                }
                slideshow.removeSlide(slideInfo.Slide);
                if (SlideRemoved != null)
                {
                    SlideRemoved.Invoke(slideInfo.Slide);
                }

                slideshow.insertSlide(actualInsertIndex, slideInfo.Slide);
                if (SlideAdded != null)
                {
                    SlideAdded.Invoke(slideInfo.Slide, actualInsertIndex);
                }
                ++actualInsertIndex;
            }
            if (SlideSelected != null)
            {
                Slide primarySelection = lastEditSlide;
                if (primarySelection == null)
                {
                    //Double check the last edit slide if the user moved a slide without actually clicking and releasing that slide will be null,
                    //in this case use the slide that was moved.
                    primarySelection = sortedSlides[0].Slide;
                }
                SlideSelected.Invoke(primarySelection, secondarySlideSelections(sortedSlides, primarySelection));
            }
        }

        private static IEnumerable<Slide> secondarySlideSelections(IEnumerable<SlideInfo> movedSlides, Slide skipSlide)
        {
            foreach (SlideInfo info in movedSlides)
            {
                if (info.Slide != skipSlide)
                {
                    yield return info.Slide;
                }
            }
        }

        public void cleanup()
        {
            //Cleanup slide trash
            CleanupInfo cleanupInfo = new CleanupInfo();
            slideshow.cleanup(cleanupInfo, ResourceProvider);
            
            undoBuffer.clear(); //Can't really recover from this one, so just erase all undo
            List<Guid> cleanupSlides = new List<Guid>(projectGuidDirectories());
            foreach (Slide slide in slideshow.Slides)
            {
                Guid guid;
                if (Guid.TryParse(slide.UniqueName, out guid))
                {
                    cleanupSlides.Remove(guid);
                }
                foreach (String file in ResourceProvider.listFiles("*", slide.UniqueName, true))
                {
                    if (!cleanupInfo.isClaimed(file))
                    {
                        try
                        {
                            editorController.ResourceProvider.delete(file);
                        }
                        catch (Exception ex)
                        {
                            Logging.Log.Error("Cleanup -- Failed to delete file '{0}'. Reason: {1}", file, ex.Message);
                        }
                    }
                }
            }
            foreach (Guid dir in cleanupSlides)
            {
                try
                {
                    editorController.ResourceProvider.delete(dir.ToString("D"));
                }
                catch (Exception ex)
                {
                    Logging.Log.Error("Cleanup -- Failed to delete directory '{0}'. Reason: {1}", dir, ex.Message);
                }
            }
        }

        /// <summary>
        /// This save function handles any exceptions, use it from keyboard shortcuts or anywhere that you don't care
        /// about any exceptions that might be thrown, otherwise use unsafeSave.
        /// </summary>
        public void safeSave()
        {
            try
            {
                unsafeSave();
            }
            catch (Exception ex)
            {
                MessageBox.show(String.Format("There was an error saving your smart lecture.\nException type: {0}\n{1}", ex.GetType().Name, ex.Message), "Save Error", MessageBoxStyle.Ok | MessageBoxStyle.IconError);
            }
        }

        /// <summary>
        /// This function saves the slideshow, it will not handle any exceptions related to this.
        /// </summary>
        public void unsafeSave()
        {
            editorController.saveAllCachedResources();
            slideImageManager.saveThumbnails();
        }

        public void saveAs(String destination)
        {
            destination = editorController.ProjectTypes.getProjectBasePath(destination);
            ResourceProvider clonedProvider;
            if (editorController.ProjectTypes.areSameProjectType(editorController.ResourceProvider.BackingProvider.BackingLocation, destination))
            {
                editorController.ResourceProvider.cloneProviderTo(destination);
                clonedProvider = editorController.ProjectTypes.openResourceProvider(destination);
            }
            else
            {
                editorController.ProjectTypes.deleteProject(destination);
                editorController.ProjectTypes.ensureProjectExists(destination);
                clonedProvider = editorController.ProjectTypes.openResourceProvider(destination);
                ResourceProviderExtensions.cloneTo(editorController.ResourceProvider, clonedProvider);
            }
            libRocketPlugin.RocketInterface.Instance.SystemInterface.RemoveRootPath(editorController.ResourceProvider.BackingLocation); //Have to remove old backing location
            editorController.ProjectTypes.resourceProviderClosed(editorController.ResourceProvider.BackingProvider);
            editorController.changeActiveResourceProvider(clonedProvider);

            //Do the actual save
            unsafeSave();

            //Reload the current slide
            openEditorContextForSlide(lastEditSlide);
            standaloneController.DocumentController.addToRecentDocuments(editorController.ResourceProvider.BackingLocation);
        }

        public void runSlideshow(int startIndex)
        {
            if (startIndex == -1)
            {
                startIndex = 0;
            }
            AnomalousMvcContext context = slideshow.createContext(ResourceProvider, standaloneController.GUIManager, startIndex);
            context.RuntimeName = editorController.EditorContextRuntimeName;
            context.setResourceProvider(ResourceProvider);
            context.BlurAction = "Common/Blur";
            CallbackAction blurAction = new CallbackAction("Blur", blurContext =>
                {
                    NavigationModel model = blurContext.getModel<NavigationModel>(Medical.SlideshowProps.BaseContextProperties.NavigationModel);
                    Slide slide = slideshow.get(model.CurrentIndex);
                    if (SlideshowContextBlurred != null)
                    {
                        SlideshowContextBlurred.Invoke(blurContext);
                    }
                    ThreadManager.invoke(new Action(() =>
                        {
                            if (lastEditSlide == slide)
                            {
                                if (SlideSelected != null)
                                {
                                    SlideSelected.Invoke(null, IEnumerableUtil<Slide>.EmptyIterator);
                                }
                            }
                            if (SlideSelected != null)
                            {
                                SlideSelected.Invoke(slide, IEnumerableUtil<Slide>.EmptyIterator);
                            }
                        }));
                });
            context.Controllers["Common"].Actions.add(blurAction);
            if (SlideshowContextStarting != null)
            {
                SlideshowContextStarting.Invoke(context);
            }
            standaloneController.TimelineController.setResourceProvider(editorController.ResourceProvider);
            standaloneController.MvcCore.startRunningContext(context);
        }

        public void runSlideshow(MedicalRmlSlide slide)
        {
            runSlideshow(slideshow.indexOf(slide));
        }

        public void applySlideLayout(TemplateSlide template)
        {
            if (slideEditorContext != null)
            {
                slideEditorContext.applySlideLayout(template);
            }
        }

        private IEnumerable<Guid> projectGuidDirectories()
        {
            Guid guid;
            foreach (String file in editorController.ResourceProvider.listDirectories("*", "", false))
            {
                if (Guid.TryParse(file, out guid))
                {
                    yield return guid;
                }
            }
        }

        public UndoRedoBuffer UndoBuffer
        {
            get
            {
                return undoBuffer;
            }
        }

        public EditorResourceProvider ResourceProvider
        {
            get
            {
                return editorController.ResourceProvider;
            }
        }

        public SlideImageManager SlideImageManager
        {
            get
            {
                return slideImageManager;
            }
        }

        void editorController_ProjectChanged(EditorController editorController)
        {
            closeEditors();
            lastEditSlide = null;
            undoBuffer.clear();

            if (editorController.ResourceProvider != null)
            {
                //Try to open a default slideshow
                String file = "Slides.show";
                if (editorController.ResourceProvider.exists(file))
                {
                    loadSlideshow(file);
                }
                else
                {
                    IEnumerable<String> files = editorController.ResourceProvider.listFiles("*.show", "", false);
                    String firstFile = files.FirstOrDefault();
                    if (firstFile != null)
                    {
                        loadSlideshow(firstFile);
                    }
                }
            }
            else if (SlideshowClosed != null)
            {
                SlideshowClosed.Invoke();
            }
        }

        /// <summary>
        /// Load a slideshow. The SlideshowLoaded event will fire when it is done.
        /// </summary>
        /// <param name="file"></param>
        void loadSlideshow(String file)
        {
            standaloneController.DocumentController.addToRecentDocuments(editorController.ResourceProvider.BackingLocation);
            slideshow = editorController.loadFile<Slideshow>(file);
            if (updateSmartLecture())
            {
                finishLoadingSlideshow();
            }
        }

        public void capture()
        {
            if (slideEditorContext != null)
            {
                SlideSceneInfo undoInfoStack = slideEditorContext.getCurrentSceneInfo();
                slideEditorContext.capture();
                SlideSceneInfo redoInfoStack = slideEditorContext.getCurrentSceneInfo();
                undoBuffer.pushAndSkip(new TwoWayDelegateCommand<SlideSceneInfo, SlideSceneInfo>(applySceneInfo, redoInfoStack, applySceneInfo, undoInfoStack,
                    poppedFrontFunc: IDisposableUtil.DisposeIfNotNull, clearedFunc: IDisposableUtil.DisposeIfNotNull, trimmedFunc: IDisposableUtil.DisposeIfNotNull));
            }
        }

        private void applySceneInfo(SlideSceneInfo info)
        {
            slideEditorContext.applySceneInfo(info);
        }

        internal void stopPlayingTimelines()
        {
            editorController.stopPlayingTimelines();
        }

        internal void createNewProject(string projectDirectory, bool deleteOld, ProjectTemplate projectTemplate)
        {
            editorController.createNewProject(projectDirectory, deleteOld, projectTemplate);
        }

        internal void closeProject()
        {
            editorController.closeProject();
        }

        internal void openProject(string fullFilePath)
        {
            editorController.openProject(editorController.ProjectTypes.getProjectBasePath(fullFilePath));
        }

        /// <summary>
        /// Check a loaded slideshow to see if it needs updates or if the program does.
        /// Returns true if it is safe to finish loading or false if this function will
        /// handle that step somehow later.
        /// </summary>
        /// <returns></returns>
        private bool updateSmartLecture()
        {
            if (slideshow.Version < Slideshow.CurrentVersion)
            {
                MessageBox.show("This Smart Lecture is out of date, would you like to update it now?\nUpdating this Smart Lecture will allow you to edit it, however, it will be incompatible with older versions of Anomalous Medical.\nIt is reccomended that you do this update.", "Update Required", MessageBoxStyle.Yes | MessageBoxStyle.No | MessageBoxStyle.IconQuest, (result) =>
                    {
                        if (result == MessageBoxStyle.Yes)
                        {
                            try
                            {
                                //First do the save as if needed to zip up old project
                                String backingLoc = editorController.ResourceProvider.BackingProvider.BackingLocation;
                                if (!backingLoc.EndsWith(".sl"))
                                {
                                    saveAs(backingLoc + ".sl");
                                }
                                if (slideshow.Version < 2)
                                {
                                    EmbeddedResourceHelpers.CopyResourceToStream(EmbeddedTemplateNames.MasterTemplate_trml, "MasterTemplate.trml", editorController.ResourceProvider, EmbeddedTemplateNames.Assembly);
                                    EmbeddedResourceHelpers.CopyResourceToStream(EmbeddedTemplateNames.SlideMasterStyles_rcss, "SlideMasterStyles.rcss", editorController.ResourceProvider, EmbeddedTemplateNames.Assembly);
                                }
                                slideshow.updateToVersion(Slideshow.CurrentVersion, editorController.ResourceProvider);
                                unsafeSave();
                                finishLoadingSlideshow();
                            }
                            catch (Exception ex)
                            {
                                MessageBox.show(String.Format("There was an error updating your smart lecture.\nException type: {0}\n{1}", ex.GetType().Name, ex.Message), "Update Error", MessageBoxStyle.Ok | MessageBoxStyle.IconError);
                            }
                        }
                        else
                        {
                            closeProject();
                        }
                        InlineRmlUpgradeCache.removeSlideshowPanels(slideshow);
                    });
                return false;
            }
            else if (slideshow.Version > Slideshow.CurrentVersion)
            {
                MessageBox.show("This Smart Lecture was created in a newer version of Anomalous Medical.\nPlease update Anomalous Medical to be able to edit this file.", "Update Required", MessageBoxStyle.Ok | MessageBoxStyle.IconWarning);
                closeProject();
                return false;
            }
            return true;
        }

        /// <summary>
        /// Complete the sequence of loading a slideshow. Called after the slideshow is loaded, or if it needed upgrading after its upgraded.
        /// </summary>
        private void finishLoadingSlideshow()
        {
            if (SlideshowLoaded != null)
            {
                SlideshowLoaded.Invoke(slideshow);
            }
            if (slideshow.Count > 0)
            {
                if (SlideSelected != null)
                {
                    SlideSelected(slideshow.get(0), IEnumerableUtil<Slide>.EmptyIterator);
                }
            }
            else
            {
                medicalSlideTemplate.createItem("", editorController);
                //If we got here its because there were no slides in the slideshow, adding the slide on the
                //line above will cause an undo state to be created, since we know we have just loaded a fresh
                //slideshow with no changes, just clear the undo buffer so the user cannot undo to having no slides
                undoBuffer.clear();
            }
        }

        internal void closeEditors()
        {
            if (slideEditorContext != null)
            {
                slideEditorContext.close();
            }
            if (timelineEditorContext != null)
            {
                timelineEditorContext.close();
            }
        }

        public void duplicateSlide(Slide slide)
        {
            try
            {
                Slide copied = CopySaver.Default.copy(slide);
                String oldName = copied.UniqueName;
                copied.generateNewUniqueName();
                editorController.ResourceProvider.copyDirectory(oldName, copied.UniqueName);
                int index = slideshow.indexOf(slide) + 1;
                if (index >= slideshow.Count)
                {
                    index = -1;
                }
                addSlide(copied, index);
            }
            catch (Exception ex)
            {
                MessageBox.show(String.Format("Error duplicating your slide.\nReason: {0} occured. {1}", ex.GetType().Name, ex.Message), "Duplicate Error", MessageBoxStyle.IconError | MessageBoxStyle.Ok);
            }
        }

        public void createSlide()
        {
            //Commented code will bring up the template dialog
            //AddItemDialog.AddItem(editorController.ItemTemplates, createItem);
            try
            {
                medicalSlideTemplate.createItem("", editorController);
            }
            catch (Exception ex)
            {
                MessageBox.show(String.Format("Error creating slide.\n{0}", ex.Message), "Error Creating Slide", MessageBoxStyle.IconError | MessageBoxStyle.Ok);
            }
        }

        //private void createItem(AddItemTemplate itemTemplate)
        //{
        //    try
        //    {
        //        ((ProjectItemTemplate)itemTemplate).createItem("", editorController);
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.show(String.Format("Error creating item.\n{0}", ex.Message), "Error Creating Item", MessageBoxStyle.IconError | MessageBoxStyle.Ok);
        //    }
        //}

        private void cleanupThumbnail(SlideInfo slideInfo)
        {
            //Double check the slideshow to make sure the slide isn't in use. This was causing
            //big problems, but the double check fixed it. The root cause was actually that the add
            //slide undos were removing their thumbnails incorrectly, however, I will leave this guard here
            //to make this function a bit more robust anyway. Checking the slideshow quick before removing the
            //thumbnail won't take that long.
            //
            //Call it paranoia, we were stumped by this for days
            if (slideshow.indexOf(slideInfo.Slide) == -1)
            {
                slideImageManager.removeImage(slideInfo.Slide);
            }
        }

        internal bool projectExists(string fullProjectName)
        {
            return editorController.ProjectTypes.doesProjectExist(fullProjectName);
        }

        private void shareSlideshow()
        {
            MessageBox.show("Before sharing your Smart Lecture it will be cleaned and saved. Do you wish to continue?", "Share Smart Lecture", MessageBoxStyle.IconQuest | MessageBoxStyle.Yes | MessageBoxStyle.No, (result) =>
            {
                if (result == MessageBoxStyle.Yes)
                {
                    try
                    {
                        slideEditorContext.commitText();
                        this.unsafeSave();
                        this.cleanup();
                        standaloneController.SharePluginController.sharePlugin(editorController.ResourceProvider.BackingProvider, PluginCreationTool.SmartLectureTools);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.show(String.Format("There was an error cleaning your smart lecture.\nException type: {0}\n{1}", ex.GetType().Name, ex.Message), "Cleaning Error", MessageBoxStyle.Ok | MessageBoxStyle.IconError);
                    }
                }
            });
        }

        /// <summary>
        /// This will refresh the rml and thumbnail for the current slide editor context.
        /// </summary>
        public void refreshRmlAndThumbnail()
        {
            if (slideEditorContext != null)
            {
                slideEditorContext.refreshAllRml();
                slideEditorContext.updateThumbnail();
            }
        }
    }
}
