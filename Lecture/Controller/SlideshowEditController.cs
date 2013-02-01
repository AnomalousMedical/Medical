using Medical;
using Medical.Controller;
using Medical.Controller.AnomalousMvc;
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
        public Action<AnomalousMvcContext> RunContext;

        public delegate void SelectSlides(Slide primary, IEnumerable<Slide> secondary);
        public event SelectSlides SlideSelected;

        private UndoRedoBuffer undoBuffer = new UndoRedoBuffer(50);

        //Editor Contexts
        private SlideEditorContext slideEditorContext;

        private StandaloneController standaloneController;
        private PropEditController propEditController;
        private EditorController editorController;
        private ShowTypeController showTypeController;
        private EditorUICallback uiCallback;
        private Slideshow slideshow;
        private ImageRenderer imageRenderer;
        private MedicalSlideItemTemplate medicalSlideTemplate;
        private SlideImageManager slideImageManager;
        private TimelineTypeController timelineTypeController;
        private TimelineEditorContext timelineEditorContext;
        private TimelineController timelineController;

        private bool allowUndoCreation = true;
        private Slide lastEditSlide = null;

        public SlideshowEditController(StandaloneController standaloneController, EditorUICallback uiCallback, PropEditController propEditController, EditorController editorController, TimelineController timelineController)
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
            }
        }

        private bool openEditorContextForSlide(Slide slide)
        {
            bool openedEditContext = false;
            //This is done like this so we could have multiple slide types besides MedicalRmlSlide
            if (slide is MedicalRmlSlide)
            {
                MedicalRmlSlide medicalSlide = (MedicalRmlSlide)slide;
                slideEditorContext = new SlideEditorContext(medicalSlide, "Slide " + (slideshow.indexOf(slide) + 1), this, uiCallback, undoBuffer, imageRenderer, medicalSlideTemplate, (rml) =>
                {
                    slideEditorContext.setWysiwygRml(rml, true);
                });
                slideEditorContext.Focus += (obj) =>
                {
                    slideEditorContext = obj;
                };
                slideEditorContext.Blur += obj =>
                {
                    medicalSlide.Rml = obj.CurrentText;
                    if (slideEditorContext == obj)
                    {
                        slideEditorContext = null;
                    }
                };
                editorController.runEditorContext(slideEditorContext.MvcContext);
                openedEditContext = true;
            }
            return openedEditContext;
        }

        public void editTimeline(Slide slide)
        {
            try
            {
                Timeline timeline = null;
                String timelineFilePath = Path.Combine(slide.UniqueName, "Timeline.tl");
                if (!ResourceProvider.exists(timelineFilePath))
                {
                    timelineTypeController.createNewTimeline(timelineFilePath);
                }
                timeline = editorController.loadFile<Timeline>(timelineFilePath); //By loading after creating we ensure this is in the cached resources

                propEditController.removeAllOpenProps();
                timelineEditorContext = new TimelineEditorContext(timeline, slide, String.Format("Slide {0} - Timeline", slideshow.indexOf(slide) + 1), this, propEditController, editorController, uiCallback, timelineController);
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
                    else if (slideIndex - 1 > 0)
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
                    else if (slideIndex - 1 > 0)
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
            undoBuffer.clear(); //Can't really recover from this one, so just erase all undo
            List<Guid> cleanupSlides = new List<Guid>(projectGuidDirectories());
            foreach (Slide slide in slideshow.Slides)
            {
                Guid guid;
                if (Guid.TryParse(slide.UniqueName, out guid))
                {
                    cleanupSlides.Remove(guid);
                }
            }
            foreach (Guid dir in cleanupSlides)
            {
                editorController.ResourceProvider.delete(dir.ToString("D"));
            }
        }

        public void save()
        {
            editorController.saveAllCachedResources();
            slideImageManager.saveThumbnails();
        }

        public void runSlideshow(int startIndex)
        {
            AnomalousMvcContext context = slideshow.createContext(ResourceProvider, startIndex);
            context.RuntimeName = editorController.EditorContextRuntimeName;
            context.setResourceProvider(ResourceProvider);
            context.BlurAction = "Common/Blur";
            CallbackAction blurAction = new CallbackAction("Blur", blurContext =>
                {
                    NavigationModel model = blurContext.getModel<NavigationModel>(Medical.SlideshowProps.BaseContextProperties.NavigationModel);
                    Slide slide = slideshow.get(model.CurrentIndex);
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
            if (RunContext != null)
            {
                RunContext.Invoke(context);
            }
        }

        public void runSlideshow(MedicalRmlSlide slide)
        {
            runSlideshow(slideshow.indexOf(slide));
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

        void editorController_ProjectChanged(EditorController editorController, string fullFilePath)
        {
            if (slideEditorContext != null)
            {
                slideEditorContext.close();
            }
            if (timelineEditorContext != null)
            {
                timelineEditorContext.close();
            }
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
                    IEnumerable<String> files = editorController.ResourceProvider.listFiles("*.show", "", true);
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

        void loadSlideshow(String file)
        {
            standaloneController.DocumentController.addToRecentDocuments(editorController.ResourceProvider.getFullFilePath(file));
            slideshow = editorController.loadFile<Slideshow>(file);
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
            }
        }

        public void capture()
        {
            if (slideEditorContext != null)
            {
                SlideSceneInfo undoInfoStack = slideEditorContext.getCurrentSceneInfo();
                slideEditorContext.capture();
                SlideSceneInfo redoInfoStack = slideEditorContext.getCurrentSceneInfo();
                undoBuffer.pushAndSkip(new TwoWayDelegateCommand<SlideSceneInfo, SlideSceneInfo>(applySceneInfo, redoInfoStack, applySceneInfo, undoInfoStack));
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

        internal void openProject(string projectPath, string fullFilePath)
        {
            editorController.openProject(projectPath, fullFilePath);
        }

        public IEnumerable<AddItemTemplate> ItemTemplates
        {
            get
            {
                return editorController.ItemTemplates;
            }
        }

        public void createItem(AddItemTemplate itemTemplate)
        {
            try
            {
                ((ProjectItemTemplate)itemTemplate).createItem("", editorController);
            }
            catch (Exception ex)
            {
                MessageBox.show(String.Format("Error creating item.\n{0}", ex.Message), "Error Creating Item", MessageBoxStyle.IconError | MessageBoxStyle.Ok);
            }
        }

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
    }
}
