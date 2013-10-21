using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller.AnomalousMvc;
using Medical.GUI.AnomalousMvc;
using Medical.GUI;
using System.IO;
using MyGUIPlugin;
using Medical.Platform;
using Engine.Platform;
using Medical.Editor;
using Medical;
using System.Drawing;
using System.Drawing.Imaging;
using Lecture.GUI;
using Engine;

namespace Lecture
{
    class SlideEditorContext
    {
        public event Action<SlideEditorContext> Focus;
        public event Action<SlideEditorContext> Blur;

        enum Events
        {
            Save,
            Undo,
            Redo
        }

        private Dictionary<String, Pair<RawRmlWysiwygView, RmlWysiwygComponent>> rmlEditors = new Dictionary<string, Pair<RawRmlWysiwygView, RmlWysiwygComponent>>();
        private String currentRmlEditor;
        private AnomalousMvcContext mvcContext;
        private EventContext eventContext;
        private MedicalRmlSlide slide;
        private EditorUICallback uiCallback;
        private UndoRedoBuffer undoBuffer;
        private SlideshowEditController slideEditorController;
        private ImageRenderer imageRenderer;
        private MedicalSlideItemTemplate itemTemplate;
        private RunCommandsAction setupScene;
        private SlideTaskbarView taskbar;
        private RunCommandsAction showCommand;
        private Action<String, String> wysiwygUndoCallback;

        public SlideEditorContext(MedicalRmlSlide slide, String slideName, SlideshowEditController editorController, EditorUICallback uiCallback, UndoRedoBuffer undoBuffer, ImageRenderer imageRenderer, MedicalSlideItemTemplate itemTemplate, Action<String, String> wysiwygUndoCallback)
        {
            this.slide = slide;
            this.uiCallback = uiCallback;
            this.slideEditorController = editorController;
            this.undoBuffer = undoBuffer;
            this.imageRenderer = imageRenderer;
            this.itemTemplate = itemTemplate;
            this.wysiwygUndoCallback = wysiwygUndoCallback;

            mvcContext = new AnomalousMvcContext();
            mvcContext.StartupAction = "Common/Start";
            mvcContext.FocusAction = "Common/Focus";
            mvcContext.BlurAction = "Common/Blur";
            mvcContext.SuspendAction = "Common/Suspended";
            mvcContext.ResumeAction = "Common/Resumed";

            showCommand = new RunCommandsAction("Show",
                    new ShowViewCommand("InfoBar"),
                    new RunActionCommand("Editor/SetupScene")
                    );

            foreach (RmlSlidePanel panel in slide.Panels)
            {
                addSlidePanelEditor(slide, panel);
            }

            DragAndDropTaskManager<WysiwygDragDropItem> htmlDragDrop = new DragAndDropTaskManager<WysiwygDragDropItem>(
                new WysiwygDragDropItem("Heading", "Editor/HeaderIcon", "<h1>Heading</h1>"),
                new WysiwygDragDropItem("Paragraph", "Editor/ParagraphsIcon", "<p>Add paragraph text here.</p>"),
                new WysiwygDragDropItem("Image", "Editor/ImageIcon", String.Format("<img src=\"{0}\" scale=\"true\"></img>", RmlWysiwygComponent.DefaultImage))
                );
            htmlDragDrop.Dragging += (item, position) =>
                {
                    foreach (var editor in rmlEditors.Values)
                    {
                        editor.Second.setPreviewElement(position, item.Markup, item.PreviewTagType);
                    }
                };
            htmlDragDrop.DragEnded += (item, position) =>
                {
                    foreach (var editor in rmlEditors.Values)
                    {
                        if (editor.Second.insertRml(item.Markup, position))
                        {
                            currentRmlEditor = editor.First.Name;
                        }
                    }
                };
            htmlDragDrop.ItemActivated += (item) =>
                {
                    rmlEditors[currentRmlEditor].Second.insertRml(item.Markup);
                };

            taskbar = new SlideTaskbarView("InfoBar", slideName);
            taskbar.addTask(new CallbackTask("Save", "Save", "CommonToolstrip/Save", "", 0, true, item =>
            {
                saveAll();
            }));
            taskbar.addTask(new CallbackTask("Undo", "Undo", "Lecture.Icon.Undo", "Edit", 0, true, item =>
            {
                undoBuffer.undo();
            }));
            taskbar.addTask(new CallbackTask("Redo", "Redo", "Lecture.Icon.Redo", "Edit", 0, true, item =>
            {
                undoBuffer.execute();
            }));
            foreach (Task htmlDragDropTask in htmlDragDrop.Tasks)
            {
                taskbar.addTask(htmlDragDropTask);
            }
            taskbar.addTask(new CallbackTask("AddSlide", "Add Slide", "Lecture.Icon.AddSlide", "Edit", 0, true, item =>
            {
                slideEditorController.createSlide();
            }));
            taskbar.addTask(new CallbackTask("DuplicateSlide", "Duplicate Slide", "Lecture.Icon.DuplicateSlide", "Edit", 0, true, item =>
            {
                slideEditorController.duplicateSlide(slide);
            }));
            taskbar.addTask(new CallbackTask("RemoveSlide", "Remove Slide", "Lecture.Icon.RemoveSlide", "Edit", 0, true, item =>
            {
                editorController.removeSelectedSlides();
            }));
            taskbar.addTask(new CallbackTask("Capture", "Capture", "Lecture.Icon.Capture", "Edit", 0, true, item =>
            {
                editorController.capture();
            }));
            taskbar.addTask(new CallbackTask("EditTimeline", "Edit Timeline", "Lecture.Icon.EditTimeline", "Edit", 0, true, item =>
            {
                editorController.editTimeline(slide);
            }));
            taskbar.addTask(new CallbackTask("Present", "Present", "Lecture.Icon.Present", "Edit", 0, true, item =>
            {
                editorController.runSlideshow(slide);
            }));
            taskbar.addTask(new CallbackTask("PresentFromBeginning", "Present From Beginning", "Lecture.Icon.PresentBeginning", "Edit", 0, true, item =>
            {
                editorController.runSlideshow(0);
            }));
            taskbar.addTask(new CallbackTask("AddRightPanel", "Add Right Panel", CommonResources.NoIcon, "Edit", 0, true, item =>
            {
                RmlSlidePanel panel = new RmlSlidePanel()
                {
                    ViewLocation = ViewLocations.Right,
                    SizeStrategy = ViewSizeStrategy.Auto,
                    Size = 30,
                    Rml = @"<rml>
	<head>
		<link type=""text/template"" href=""/MasterTemplate.trml"" />
	</head>
	<body template=""MasterTemplate"">
        <h1>Click to Change Title</h1>
        <p>Click to change text.</p>
    </body>
</rml>
"                };
                slide.addPanel(panel);
                var view = addSlidePanelEditor(slide, panel);
                mvcContext.Controllers.add(new MvcController(view.Name + "Controller",
                    new RunCommandsAction("Show",
                        new ShowViewCommand(view.Name))
                        ));
                mvcContext.runAction(String.Format("{0}Controller/Show", view.Name));
            }));
            mvcContext.Views.add(taskbar);

            setupScene = new RunCommandsAction("SetupScene");
            slide.populateCommand(setupScene);

            mvcContext.Controllers.add(new MvcController("Editor",
                setupScene,
                showCommand,
                new RunCommandsAction("Close", new CloseAllViewsCommand())
                ));

            mvcContext.Controllers.add(new MvcController("Common",
                new RunCommandsAction("Start", new RunActionCommand("Editor/Show")),
                new CallbackAction("Focus", context =>
                {
                    htmlDragDrop.CreateIconPreview();
                    GlobalContextEventHandler.setEventContext(eventContext);
                    if (Focus != null)
                    {
                        Focus.Invoke(this);
                    }
                }),
                new CallbackAction("Blur", context =>
                {
                    commitText();
                    foreach(var panel in slide.Panels)
                    {
                        String editorName = panel.createViewName("RmlView");
                        var editor = rmlEditors[editorName];
                        editor.First.Rml = panel.Rml = getCurrentText(editor.Second);
                    }
                    GlobalContextEventHandler.disableEventContext(eventContext);
                    htmlDragDrop.DestroyIconPreview();
                    if (Blur != null)
                    {
                        Blur.Invoke(this);
                    }
                }),
                new RunCommandsAction("Suspended", new SaveViewLayoutCommand()),
                new RunCommandsAction("Resumed", new RestoreViewLayoutCommand())));

            eventContext = new EventContext();
            MessageEvent saveEvent = new MessageEvent(Events.Save);
            saveEvent.addButton(KeyboardButtonCode.KC_LCONTROL);
            saveEvent.addButton(KeyboardButtonCode.KC_S);
            saveEvent.FirstFrameUpEvent += eventManager =>
            {
                saveAll();
            };
            eventContext.addEvent(saveEvent);

            MessageEvent undoEvent = new MessageEvent(Events.Undo);
            undoEvent.addButton(KeyboardButtonCode.KC_LCONTROL);
            undoEvent.addButton(KeyboardButtonCode.KC_Z);
            undoEvent.FirstFrameUpEvent += eventManager =>
            {
                undoBuffer.undo();
            };
            eventContext.addEvent(undoEvent);

            MessageEvent redoEvent = new MessageEvent(Events.Redo);
            redoEvent.addButton(KeyboardButtonCode.KC_LCONTROL);
            redoEvent.addButton(KeyboardButtonCode.KC_Y);
            redoEvent.FirstFrameUpEvent += eventManager =>
            {
                undoBuffer.execute();
            };
            eventContext.addEvent(redoEvent);
        }

        public void close()
        {
            mvcContext.runAction("Editor/Close");
        }

        public void addTask(Task task)
        {
            taskbar.addTask(task);
        }

        public void setWysiwygRml(String panelName, String rml, bool keepScrollPosition)
        {
            Pair<RawRmlWysiwygView, RmlWysiwygComponent> editor;
            if (rmlEditors.TryGetValue(panelName, out editor) && editor.Second != null)
            {
                editor.Second.cancelAndHideEditor();
                editor.Second.setRml(rml, keepScrollPosition, true);
            }
        }

        public SlideSceneInfo getCurrentSceneInfo()
        {
            return new SlideSceneInfo(slide);
        }

        public void slideNameChanged(string slideName)
        {
            taskbar.DisplayName = slideName;
        }

        public void applySceneInfo(SlideSceneInfo info)
        {
            setupScene.clear();
            info.applyToSlide(slide);
            slide.populateCommand(setupScene);
            mvcContext.runAction("Editor/SetupScene");
            updateThumbnail();
        }

        internal void capture()
        {
            itemTemplate.applySceneStateToSlide(slide);
            updateThumbnail();
        }

        public AnomalousMvcContext MvcContext
        {
            get
            {
                return mvcContext;
            }
        }

        private String getCurrentText(RmlWysiwygComponent rmlComponent)
        {
            if (rmlComponent != null)
            {
                return rmlComponent.CurrentRml;
            }
            return null;
        }

        private void saveAll()
        {
            commitText();
            slideEditorController.save();
        }

        public void commitText()
        {
            foreach (var editor in rmlEditors.Values)
            {
                if (editor.Second != null)
                {
                    editor.Second.aboutToSaveRml();
                    if (editor.Second.ChangesMade)
                    {
                        updateThumbnail();
                    }
                }
            }
        }

        public void updateThumbnail()
        {
            if (slideEditorController.ResourceProvider != null)
            {
                IntSize2 sceneThumbSize = new IntSize2(SlideImageManager.ThumbWidth, SlideImageManager.ThumbHeight);
                IntVector2 sceneThumbPosition = new IntVector2(0, 0);
                Bitmap thumb = slideEditorController.SlideImageManager.createThumbBitmap(slide);
                using (Graphics g = Graphics.FromImage(thumb))
                {
                    foreach (var editor in rmlEditors.Values)
                    {
                        if (editor.Second != null)
                        {
                            IntVector2 location = editor.Second.ViewHost.Container.Location - editor.Second.ViewHost.Container.RigidParent.Location;
                            IntSize2 size = editor.Second.ViewHost.Container.WorkingSize;
                            float sizeRatio = (float)SlideImageManager.ThumbWidth / editor.Second.ViewHost.Container.RigidParentWorkingSize.Width;
                            
                            Rectangle panelThumbPos = new Rectangle(0, 0, SlideImageManager.ThumbWidth, SlideImageManager.ThumbHeight);
                            switch (editor.First.ViewLocation)
                            {
                                case ViewLocations.Left:
                                    panelThumbPos.Width = (int)(size.Width * sizeRatio);
                                    sceneThumbPosition.x = panelThumbPos.Width;
                                    sceneThumbSize.Width -= panelThumbPos.Width;
                                    break;
                                case ViewLocations.Right:
                                    panelThumbPos.Width = (int)(size.Width * sizeRatio);
                                    panelThumbPos.X = SlideImageManager.ThumbWidth - panelThumbPos.Width;
                                    sceneThumbSize.Width -= panelThumbPos.Width;
                                    break;
                            }
                            editor.Second.writeToGraphics(g, panelThumbPos);
                        }
                    }

                    ImageRendererProperties imageProperties = new ImageRendererProperties();
                    imageProperties.Width = sceneThumbSize.Width;
                    imageProperties.Height = sceneThumbSize.Height;
                    imageProperties.AntiAliasingMode = 2;
                    imageProperties.TransparentBackground = false;
                    imageProperties.UseActiveViewportLocation = false;
                    imageProperties.OverrideLayers = true;
                    imageProperties.ShowBackground = true;
                    imageProperties.ShowWatermark = false;
                    imageProperties.ShowUIUpdates = false;
                    imageProperties.LayerState = slide.Layers;
                    imageProperties.CameraPosition = slide.CameraPosition.Translation;
                    imageProperties.CameraLookAt = slide.CameraPosition.LookAt;

                    using (Bitmap sceneThumb = imageRenderer.renderImage(imageProperties))
                    {
                        g.DrawImage(sceneThumb, sceneThumbPosition.x, sceneThumbPosition.y);
                    }
                }
                
                slideEditorController.SlideImageManager.thumbnailUpdated(slide);
            }
        }

        private RawRmlWysiwygView addSlidePanelEditor(MedicalRmlSlide slide, RmlSlidePanel panel)
        {
            String editorViewName = panel.createViewName("RmlView");
            RawRmlWysiwygView rmlView = new RawRmlWysiwygView(editorViewName, this.uiCallback, this.uiCallback, this.undoBuffer);
            rmlView.ViewLocation = panel.ViewLocation;
            rmlView.IsWindow = false;
            rmlView.EditPreviewContent = true;
            rmlView.Size = new IntSize2(panel.Size, panel.Size);
            rmlView.WidthSizeStrategy = panel.SizeStrategy;
            rmlView.Rml = panel.Rml;
            rmlView.FakePath = slide.UniqueName + "/index.rml";
            rmlView.ComponentCreated += (view, component) =>
            {
                rmlEditors[view.Name].Second = component;
                component.RmlEdited += rmlEditor =>
                {
                    panel.Rml = rmlEditor.CurrentRml;
                };
            };
            rmlView.UndoRedoCallback = (rml) =>
            {
                this.wysiwygUndoCallback(editorViewName, rml);
            };
            rmlView.RequestFocus += (view) =>
            {
                currentRmlEditor = view.Name;
            };
            rmlView.addCustomStrategy(new SlideImageStrategy("img", this.slideEditorController.ResourceProvider, slide.UniqueName));
            mvcContext.Views.add(rmlView);
            rmlEditors.Add(rmlView.Name, new Pair<RawRmlWysiwygView, RmlWysiwygComponent>(rmlView, null));
            showCommand.addCommand(new ShowViewCommand(rmlView.Name));
            if (currentRmlEditor == null)
            {
                currentRmlEditor = rmlView.Name;
            }

            return rmlView;
        }
    }
}
