using Medical.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    public class SlideshowEditController
    {
        public event Action<Slideshow> SlideshowLoaded;
        public event Action SlideshowClosed;
        public event Action<Slide, int> SlideAdded;
        public event Action<Slide> SlideRemoved;
        public event Action<Slide> SlideSelected;

        private UndoRedoBuffer undoBuffer = new UndoRedoBuffer(50);

        //Editor Contexts
        private SlideEditorContext slideEditorContext;

        private StandaloneController standaloneController;
        private PropEditController propEditController;
        private EditorController editorController;
        private ShowTypeController showTypeController;
        private EditorUICallback uiCallback;
        private Slideshow slideshow;

        private bool allowUndoCreation = true;
        private Slide lastEditSlide = null;

        public SlideshowEditController(StandaloneController standaloneController, EditorUICallback uiCallback, PropEditController propEditController, EditorController editorController)
        {
            this.standaloneController = standaloneController;
            this.uiCallback = uiCallback;
            this.propEditController = propEditController;
            this.editorController = editorController;
            editorController.ProjectChanged += editorController_ProjectChanged;

            //Show Type Controller
            showTypeController = new ShowTypeController(editorController);
            editorController.addTypeController(showTypeController);

            MedicalSlideItemTemplate medicalSlideTemplate = new MedicalSlideItemTemplate(standaloneController.SceneViewController, standaloneController.MedicalStateController);
            medicalSlideTemplate.SlideCreated += (slide) =>
                {
                    addSlide(slide);
                };
            editorController.addItemTemplate(medicalSlideTemplate);
        }

        public void editSlide(Slide slide)
        {
            bool undoNeeded = false;
            if (slide is MedicalRmlSlide)
            {
                MedicalRmlSlide medicalSlide = (MedicalRmlSlide)slide;
                slideEditorContext = new SlideEditorContext(medicalSlide, uiCallback, undoBuffer, (rml) =>
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
                undoNeeded = true;
            }

            if (undoNeeded)
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
                                    SlideSelected.Invoke(redoItem);
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
                                    SlideSelected.Invoke(undoItem);
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

        class SlideInfo
        {
            public SlideInfo(Slide slide, int index)
            {
                this.Slide = slide;
                this.Index = index;
            }

            public Slide Slide { get; set; }

            public int Index { get; set; }
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
                        SlideSelected.Invoke(changeToSlide);
                    }
                    allowUndoCreation = wasAllowingUndo;
                }

                if (allowUndoCreation)
                {
                    if (changeToSlide == null)
                    {
                        undoBuffer.pushAndSkip(new TwoWayDelegateCommand<SlideInfo>((executeSlide) =>
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
                                    SlideSelected.Invoke(executeSlide.ChangeToSlide);
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
                                    SlideSelected.Invoke(undoSlide.Slide);
                                }
                                allowUndoCreation = true;
                            }));
                        },
                        new RemoveSlideInfo(slide, slideIndex, changeToSlide)));
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
            if (SlideAdded != null)
            {
                SlideAdded.Invoke(slide, index);
            }
            if (allowUndoCreation)
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
                new SlideInfo(slide, slideshow.indexOf(slide))));
            }
        }

        public UndoRedoBuffer UndoBuffer
        {
            get
            {
                return undoBuffer;
            }
        }

        void editorController_ProjectChanged(EditorController editorController, string fullFilePath)
        {
            if (slideEditorContext != null)
            {
                slideEditorContext.close();
            }
            lastEditSlide = null;
            undoBuffer.clear();

            if (editorController.ResourceProvider != null)
            {
                //Try to open a default mvc context
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
        }
    }
}
