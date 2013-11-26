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
using Engine.Editing;
using Medical.SlideshowActions;

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

        private Dictionary<String, RmlEditorViewInfo> rmlEditors = new Dictionary<string, RmlEditorViewInfo>();
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
        private RunCommandsAction showEditorWindowsCommand;
        private RunCommandsAction closeEditorWindowsCommand;
        private Action<String, String> wysiwygUndoCallback;
        private SlideLayoutPickerTask slideLayoutPicker;
        private SlideshowEditController editorController;
        private PanelResizeWidget panelResizeWidget;
        private bool forceUpdateThumbOnBlur = false;

        SlideImageStrategy imageStrategy;
        SlideTriggerStrategy triggerStrategy;

        public SlideEditorContext(MedicalRmlSlide slide, String slideName, SlideshowEditController editorController, EditorUICallback uiCallback, UndoRedoBuffer undoBuffer, ImageRenderer imageRenderer, MedicalSlideItemTemplate itemTemplate, Action<String, String> wysiwygUndoCallback)
        {
            this.slide = slide;
            this.uiCallback = uiCallback;
            if (uiCallback.hasCustomQuery(PlayTimelineAction.CustomActions.EditTimeline))
            {
                uiCallback.removeCustomQuery(PlayTimelineAction.CustomActions.EditTimeline);
            }
            uiCallback.addOneWayCustomQuery(PlayTimelineAction.CustomActions.EditTimeline, new Action<PlayTimelineAction>(action_EditTimeline));
            this.slideEditorController = editorController;
            this.undoBuffer = undoBuffer;
            this.imageRenderer = imageRenderer;
            this.itemTemplate = itemTemplate;
            this.wysiwygUndoCallback = wysiwygUndoCallback;
            this.editorController = editorController;
            panelResizeWidget = new PanelResizeWidget();
            panelResizeWidget.RecordResizeUndo += panelResizeWidget_RecordResizeUndo;

            imageStrategy = new SlideImageStrategy("img", this.slideEditorController.ResourceProvider, slide.UniqueName);
            triggerStrategy = new SlideTriggerStrategy(slide, createTriggerActionBrowser(), undoBuffer, "a", "Lecture.Icon.TriggerIcon");

            mvcContext = new AnomalousMvcContext();
            mvcContext.StartupAction = "Common/Start";
            mvcContext.FocusAction = "Common/Focus";
            mvcContext.BlurAction = "Common/Blur";
            mvcContext.SuspendAction = "Common/Suspended";
            mvcContext.ResumeAction = "Common/Resumed";

            showEditorWindowsCommand = new RunCommandsAction("ShowEditors");
            closeEditorWindowsCommand = new RunCommandsAction("CloseEditors");

            RunCommandsAction showCommand = new RunCommandsAction("Show",
                    new ShowViewCommand("InfoBar"),
                    new RunActionCommand("Editor/SetupScene"),
                    new RunActionCommand("Editor/ShowEditors")
                    );

            refreshPanelEditors(false);

            DragAndDropTaskManager<WysiwygDragDropItem> htmlDragDrop = new DragAndDropTaskManager<WysiwygDragDropItem>(
                new WysiwygDragDropItem("Heading", "Editor/HeaderIcon", "<h1>Heading</h1>"),
                new WysiwygDragDropItem("Paragraph", "Editor/ParagraphsIcon", "<p>Add paragraph text here.</p>"),
                new WysiwygDragDropItem("Image", "Editor/ImageIcon", String.Format("<img src=\"{0}\" scale=\"true\"></img>", RmlWysiwygComponent.DefaultImage)),
                new WysiwygDragDropItem("Trigger", "Lecture.Icon.TriggerIcon", "<a class=\"TriggerLink\" onclick=\"\">Add trigger text here.</a>")
                );
            htmlDragDrop.Dragging += (item, position) =>
                {
                    foreach (var editor in rmlEditors.Values)
                    {
                        editor.Component.setPreviewElement(position, item.Markup, item.PreviewTagType);
                    }
                };
            htmlDragDrop.DragEnded += (item, position) =>
                {
                    foreach (var editor in rmlEditors.Values)
                    {
                        if (editor.Component.insertRml(item.Markup, position))
                        {
                            currentRmlEditor = editor.View.Name;
                        }
                    }
                };
            htmlDragDrop.ItemActivated += (item) =>
                {
                    rmlEditors[currentRmlEditor].Component.insertRml(item.Markup);
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
            slideLayoutPicker = new SlideLayoutPickerTask();

            makeTempPresets();

            slideLayoutPicker.ChangeSlideLayout += slideLayoutPicker_ChangeSlideLayout;
            taskbar.addTask(slideLayoutPicker);
            mvcContext.Views.add(taskbar);

            setupScene = new RunCommandsAction("SetupScene");
            slide.populateCommand(setupScene);

            mvcContext.Controllers.add(new MvcController("Editor",
                setupScene,
                showCommand,
                showEditorWindowsCommand,
                closeEditorWindowsCommand,
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
                    slideLayoutPicker.createLayoutPicker();
                }),
                new CallbackAction("Blur", context =>
                {
                    commitText(forceUpdateThumbOnBlur);
                    foreach (RmlSlidePanel panel in slide.Panels.Where(p => p is RmlSlidePanel))
                    {
                        String editorName = panel.createViewName("RmlView");
                        var editor = rmlEditors[editorName];
                        editor.View.Rml = panel.Rml = editor.getCurrentComponentText();
                    }
                    slideLayoutPicker.destroyLayoutPicker();
                    GlobalContextEventHandler.disableEventContext(eventContext);
                    htmlDragDrop.DestroyIconPreview();
                    panelResizeWidget.Dispose();
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

        private void makeTempPresets()
        {
            //Couple simple presets
            TemplateSlide presetSlide = new TemplateSlide()
            {
                Name = "Left Panel",
                IconName = "Lecture.SlideLayouts.OnePanel"
            };
            presetSlide.addPanel(new RmlSlidePanel()
            {
                Rml = MedicalSlideItemTemplate.defaultSlide,
                Size = 480,
                ViewLocation = ViewLocations.Left,
            });
            slideLayoutPicker.addPresetSlide(presetSlide);

            presetSlide = new TemplateSlide()
            {
                Name = "Left and Right Panel",
                IconName = "Lecture.SlideLayouts.TwoPanel"
            };
            presetSlide.addPanel(new RmlSlidePanel()
            {
                Rml = MedicalSlideItemTemplate.defaultSlide,
                Size = 480,
                ViewLocation = ViewLocations.Left,
            });
            presetSlide.addPanel(new RmlSlidePanel()
            {
                Rml = MedicalSlideItemTemplate.defaultSlide,
                Size = 480,
                ViewLocation = ViewLocations.Right,
            });
            slideLayoutPicker.addPresetSlide(presetSlide);

            presetSlide = new TemplateSlide()
            {
                Name = "Three Panel",
                IconName = "Lecture.SlideLayouts.ThreePanel"
            };
            presetSlide.addPanel(new RmlSlidePanel()
            {
                Rml = MedicalSlideItemTemplate.defaultSlide,
                Size = 480,
                ViewLocation = ViewLocations.Left,
            });
            presetSlide.addPanel(new RmlSlidePanel()
            {
                Rml = MedicalSlideItemTemplate.defaultSlide,
                Size = 480,
                ViewLocation = ViewLocations.Right,
            });
            presetSlide.addPanel(new RmlSlidePanel()
            {
                Rml = MedicalSlideItemTemplate.defaultSlide,
                Size = 288,
                ViewLocation = ViewLocations.Top,
            });
            slideLayoutPicker.addPresetSlide(presetSlide);

            presetSlide = new TemplateSlide()
            {
                Name = "Four Panel",
                IconName = "Lecture.SlideLayouts.FourPanel"
            };
            presetSlide.addPanel(new RmlSlidePanel()
            {
                Rml = MedicalSlideItemTemplate.defaultSlide,
                Size = 480,
                ViewLocation = ViewLocations.Left,
            });
            presetSlide.addPanel(new RmlSlidePanel()
            {
                Rml = MedicalSlideItemTemplate.defaultSlide,
                Size = 480,
                ViewLocation = ViewLocations.Right,
            });
            presetSlide.addPanel(new RmlSlidePanel()
            {
                Rml = MedicalSlideItemTemplate.defaultSlide,
                Size = 288,
                ViewLocation = ViewLocations.Top,
            });
            presetSlide.addPanel(new RmlSlidePanel()
            {
                Rml = MedicalSlideItemTemplate.defaultSlide,
                Size = 288,
                ViewLocation = ViewLocations.Bottom,
            });
            slideLayoutPicker.addPresetSlide(presetSlide);

            presetSlide = new TemplateSlide(new FullScreenSlideLayoutStrategy())
            {
                Name = "50/50",
                IconName = "Lecture.SlideLayouts.TwoColumn"
            };
            presetSlide.addPanel(new RmlSlidePanel()
            {
                Rml = MedicalSlideItemTemplate.defaultSlide,
                Size = 50,
                ViewLocation = ViewLocations.Left,
            });
            presetSlide.addPanel(new RmlSlidePanel()
            {
                Rml = MedicalSlideItemTemplate.defaultSlide,
                Size = 50,
                ViewLocation = ViewLocations.Right,
            });
            slideLayoutPicker.addPresetSlide(presetSlide);

            presetSlide = new TemplateSlide(new FullScreenSlideLayoutStrategy())
            {
                Name = "Full Screen",
                IconName = "Lecture.SlideLayouts.Full",
            };
            presetSlide.addPanel(new RmlSlidePanel()
            {
                Rml = MedicalSlideItemTemplate.defaultSlide,
                Size = 100,
                ViewLocation = ViewLocations.Left,
            });
            slideLayoutPicker.addPresetSlide(presetSlide);

            presetSlide = new TemplateSlide()
            {
                Name = "Left and Top",
                IconName = "Lecture.SlideLayouts.LeftTop"
            };
            presetSlide.addPanel(new RmlSlidePanel()
            {
                Rml = MedicalSlideItemTemplate.defaultSlide,
                Size = 480,
                ViewLocation = ViewLocations.Left,
            });
            presetSlide.addPanel(new RmlSlidePanel()
            {
                Rml = MedicalSlideItemTemplate.defaultSlide,
                Size = 288,
                ViewLocation = ViewLocations.Top,
            });
            slideLayoutPicker.addPresetSlide(presetSlide);
        }

        void slideLayoutPicker_ChangeSlideLayout(Slide newSlideLayout)
        {
            undoBuffer.pushAndExecute(new TwoWayDelegateCommand<Slide, Slide>(
                (execSlide) =>
                    {
                        forceUpdateThumbOnBlur = true;
                        execSlide.copyLayoutToSlide(slide, false);
                        refreshPanelEditors(true);
                    },
                newSlideLayout,
                (undoSlide) =>
                    {
                        forceUpdateThumbOnBlur = true;
                        undoSlide.copyLayoutToSlide(slide, true);
                        refreshPanelEditors(true);
                    },
                slide.createTemplateSlide()));
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
            RmlEditorViewInfo editor;
            if (rmlEditors.TryGetValue(panelName, out editor) && editor.Component != null)
            {
                editor.Component.cancelAndHideEditor();
                editor.Component.setRml(rml, keepScrollPosition, true);
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

        private void saveAll()
        {
            commitText();
            slideEditorController.save();
        }

        public void commitText(bool forceUpdateThumb = false)
        {
            bool updateThumb = forceUpdateThumb;
            foreach (var editor in rmlEditors.Values)
            {
                updateThumb |= editor.commitText();
            }
            if (updateThumb)
            {
                updateThumbnail();
            }
        }

        public void updateThumbnail()
        {
            forceUpdateThumbOnBlur = false;
            if (slideEditorController.ResourceProvider != null)
            {
                IntSize2 sceneThumbSize = new IntSize2(SlideImageManager.ThumbWidth, SlideImageManager.ThumbHeight);
                IntVector2 sceneThumbPosition = new IntVector2(0, 0);
                Bitmap thumb = slideEditorController.SlideImageManager.createThumbBitmap(slide);
                using (Graphics g = Graphics.FromImage(thumb))
                {
                    foreach (var editor in rmlEditors.Values)
                    {
                        if (editor.Component != null)
                        {
                            IntVector2 location = editor.Component.ViewHost.Container.Location - editor.Component.ViewHost.Container.RigidParent.Location;
                            IntSize2 size = editor.Component.ViewHost.Container.WorkingSize;
                            float sizeRatio = (float)SlideImageManager.ThumbWidth / editor.Component.ViewHost.Container.RigidParentWorkingSize.Width;

                            Rectangle panelThumbPos = new Rectangle((int)Math.Round(location.x * sizeRatio), (int)Math.Round(location.y * sizeRatio), (int)Math.Round(size.Width * sizeRatio), (int)Math.Round(size.Height * sizeRatio));
                            switch (editor.View.ViewLocation)
                            {
                                case ViewLocations.Left:
                                    sceneThumbPosition.x = panelThumbPos.Width;
                                    sceneThumbSize.Width -= panelThumbPos.Width;
                                    break;
                                case ViewLocations.Right:
                                    sceneThumbSize.Width -= panelThumbPos.Width;
                                    break;
                                case ViewLocations.Top:
                                    sceneThumbPosition.y = panelThumbPos.Height;
                                    sceneThumbSize.Height -= panelThumbPos.Height;
                                    break;
                                case ViewLocations.Bottom:
                                    sceneThumbSize.Height -= panelThumbPos.Height;
                                    break;
                            }
                            editor.Component.writeToGraphics(g, panelThumbPos);
                        }
                    }

                    if (sceneThumbSize.Width > 0 && sceneThumbSize.Height > 0)
                    {
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
                        imageProperties.UseIncludePoint = slide.CameraPosition.UseIncludePoint;
                        imageProperties.IncludePoint = slide.CameraPosition.IncludePoint;

                        using (Bitmap sceneThumb = imageRenderer.renderImage(imageProperties))
                        {
                            g.DrawImage(sceneThumb, sceneThumbPosition.x, sceneThumbPosition.y);
                        }
                    }
                }
                
                slideEditorController.SlideImageManager.thumbnailUpdated(slide);
            }
        }

        private void refreshPanelEditors(bool replaceExistingEditors)
        {
            if (replaceExistingEditors)
            {
                mvcContext.runAction("Editor/CloseEditors");
            }
            closeEditorWindowsCommand.clear();
            showEditorWindowsCommand.clear();
            foreach (var editor in rmlEditors.Values)
            {
                mvcContext.Views.remove(editor.View);
                editor.Component.ElementDraggedOffDocument -= RmlWysiwyg_ElementDraggedOffDocument;
                editor.Component.ElementDroppedOffDocument -= RmlWysiwyg_ElementDroppedOffDocument;
                editor.Component.ElementReturnedToDocument -= RmlWysiwyg_ElementReturnedToDocument;
            }
            rmlEditors.Clear();

            foreach (RmlSlidePanel panel in slide.Panels.Where(p => p is RmlSlidePanel))
            {
                SlideInstanceLayoutStrategy instanceLayout = slide.LayoutStrategy.createLayoutStrategy();
                String editorViewName = panel.createViewName("RmlView");
                RawRmlWysiwygView rmlView = new RawRmlWysiwygView(editorViewName, this.uiCallback, this.uiCallback, this.undoBuffer);
                rmlView.ViewLocation = panel.ViewLocation;
                rmlView.IsWindow = false;
                rmlView.EditPreviewContent = true;
                rmlView.Rml = panel.Rml;
                rmlView.FakePath = slide.UniqueName + "/index.rml";
                instanceLayout.addView(rmlView);
                rmlView.ComponentCreated += (view, component) =>
                {
                    rmlEditors[view.Name].Component = component;
                    component.RmlEdited += rmlEditor =>
                    {
                        panel.Rml = rmlEditor.CurrentRml;
                    };
                    component.ElementDraggedOffDocument += RmlWysiwyg_ElementDraggedOffDocument;
                    component.ElementDroppedOffDocument += RmlWysiwyg_ElementDroppedOffDocument;
                    component.ElementReturnedToDocument += RmlWysiwyg_ElementReturnedToDocument;
                };
                rmlView.UndoRedoCallback = (rml) =>
                {
                    this.wysiwygUndoCallback(editorViewName, rml);
                };
                rmlView.RequestFocus += (view) =>
                {
                    setCurrentRmlEditor(view.Name);
                };
                rmlView.addCustomStrategy(imageStrategy);
                rmlView.addCustomStrategy(triggerStrategy);
                mvcContext.Views.add(rmlView);
                rmlEditors.Add(rmlView.Name, new RmlEditorViewInfo(rmlView, panel));
                showEditorWindowsCommand.addCommand(new ShowViewCommand(rmlView.Name));
                closeEditorWindowsCommand.addCommand(new CloseViewIfOpen(rmlView.Name));
                if (currentRmlEditor == null)
                {
                    setCurrentRmlEditor(rmlView.Name);
                }
            }

            if (replaceExistingEditors)
            {
                mvcContext.runAction("Editor/ShowEditors");
            }
        }

        void setCurrentRmlEditor(String name)
        {
            if (currentRmlEditor != null)
            {
                rmlEditors[currentRmlEditor].lostFocus();
            }
            currentRmlEditor = name;
            if (currentRmlEditor != null)
            {
                var editor = rmlEditors[currentRmlEditor];
                panelResizeWidget.setCurrentEditor(editor);
            }
            else
            {
                panelResizeWidget.setCurrentEditor(null);
            }
        }

        void RmlWysiwyg_ElementDraggedOffDocument(RmlWysiwygComponent sender, IntVector2 position, string innerRmlHint, string previewElementTagType)
        {
            foreach (var editor in rmlEditors.Values)
            {
                if (editor.Component != sender)
                {
                    editor.Component.setPreviewElement(position, innerRmlHint, previewElementTagType);
                }
            }
        }

        void RmlWysiwyg_ElementReturnedToDocument(RmlWysiwygComponent sender, IntVector2 position, string innerRmlHint, string previewElementTagType)
        {
            foreach (var editor in rmlEditors.Values)
            {
                if (editor.Component != sender)
                {
                    editor.Component.setPreviewElement(position, innerRmlHint, previewElementTagType);
                }
            }
        }

        void RmlWysiwyg_ElementDroppedOffDocument(RmlWysiwygComponent sender, IntVector2 position, string innerRmlHint, string previewElementTagType)
        {
            foreach (var editor in rmlEditors.Values)
            {
                if (editor.Component != sender)
                {
                    editor.Component.insertRml(innerRmlHint, position);
                }
            }
        }

        private Browser createTriggerActionBrowser()
        {
            Browser browser = new Browser("Types", "Choose Trigger Type");
            BrowserNode rootNode = browser.getTopNode();
            browser.DefaultSelection = new BrowserNode("Setup Scene", new Func<String, SlideAction>((name) =>
            {
                SetupSceneAction setupScene = new SetupSceneAction(name);
                setupScene.captureSceneState(uiCallback);
                return setupScene;
            }));
            rootNode.addChild(browser.DefaultSelection);

            rootNode.addChild(new BrowserNode("Play Timeline", new Func<String, SlideAction>((name) =>
            {
                return new PlayTimelineAction(name);
            })));

            return browser;
        }

        void action_EditTimeline(PlayTimelineAction action)
        {
            editorController.editTimeline(slide, action.TimelineFileName);
        }

        public event PanelResizeWidget.RecordResizeInfoDelegate RecordResizeUndo
        {
            add
            {
                panelResizeWidget.RecordResizeUndo += value;
            }
            remove
            {
                panelResizeWidget.RecordResizeUndo -= value;
            }
        }

        internal void resizePanel(string panelName, int size)
        {
            var editor = rmlEditors[panelName];
            editor.Panel.Size = size;
            if (editor.Component != null)
            {
                editor.Component.ViewHost.Container.invalidate();
            }
        }

        void panelResizeWidget_RecordResizeUndo(RmlEditorViewInfo view, int oldSize, int newSize)
        {
            forceUpdateThumbOnBlur = true;
        }
    }
}
