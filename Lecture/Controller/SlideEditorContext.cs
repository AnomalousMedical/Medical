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
using Medical.Controller;
using OgreWrapper;

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
            Redo,
            Run
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
        private DragAndDropTaskManager<WysiwygDragDropItem> htmlDragDrop;
        private SlideshowStyleManager styleManager;

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

            htmlDragDrop = new DragAndDropTaskManager<WysiwygDragDropItem>(
                new WysiwygDragDropItem("Heading", "Editor/HeaderIcon", "<h1>Heading</h1>"),
                new WysiwygDragDropItem("Paragraph", "Editor/ParagraphsIcon", "<p>Add paragraph text here.</p>"),
                new WysiwygDragDropItem("Image", "Editor/ImageIcon", String.Format("<img src=\"{0}\" style=\"width:200px;\"></img>", RmlWysiwygComponent.DefaultImage)),
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

            styleManager = new SlideshowStyleManager(editorController, uiCallback);
            styleManager.addStyleFile(Path.Combine(slide.UniqueName, Slide.StyleSheetName), "This Slide");
            styleManager.addStyleFile("SlideMasterStyles.rcss", "All Slides");
            taskbar.addTask(new CallbackTask("EditSlideshowTheme", "Edit Slideshow Theme", CommonResources.NoIcon, "Edit", 0, true, item =>
            {
                IntVector2 taskPosition = item.CurrentTaskPositioner.findGoodWindowPosition(SlideshowStyleManager.Width, SlideshowStyleManager.Height);
                styleManager.showEditor(taskPosition.x, taskPosition.y);
            }));


            taskbar.addTask(new CallbackTask("ClearThumb", "Clear Thumb", CommonResources.NoIcon, "Edit", 0, true, item =>
            {
                includeLoc = new IntVector2(0, 0);
            }));

            taskbar.addTask(new CallbackTask("ResetSlide", "Reset Slide", CommonResources.NoIcon, "Edit", 0, true, item =>
            {
                mvcContext.runAction("Editor/SetupScene");
            }));

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
                    panelResizeWidget.createResizeWidget();
                }),
                new CallbackAction("Blur", blur),
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

            MessageEvent runEvent = new MessageEvent(Events.Run);
            runEvent.addButton(KeyboardButtonCode.KC_F5);
            runEvent.FirstFrameUpEvent += eventManager =>
            {
                ThreadManager.invoke(() =>
                {
                    editorController.runSlideshow(0);
                });
            };
            eventContext.addEvent(runEvent);
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

        public void applySlideLayout(TemplateSlide template)
        {
            forceUpdateThumbOnBlur = true;
            template.copyLayoutToSlide(slide, editorController.ResourceProvider, false);
            refreshPanelEditors(true);
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
            slideEditorController.safeSave();
        }

        public void commitText(bool forceUpdateThumb = false)
        {
            bool updateThumb = forceUpdateThumb;
            foreach (var editor in rmlEditors.Values)
            {
                updateThumb = editor.commitText() | updateThumb;
            }
            if (updateThumb)
            {
                updateThumbnail();
            }
        }

        //temporarily here so the info will persist, we will have to save this somewhere
        static IntVector2 includeLoc = new IntVector2();

        public void updateThumbnail(bool forceUpdateSceneThumb = false)
        {
            forceUpdateThumbOnBlur = false;
            Dictionary<RmlEditorViewInfo, LayoutContainer> layoutPositions = new Dictionary<RmlEditorViewInfo, LayoutContainer>();
            if (slideEditorController.ResourceProvider != null)
            {
                //Setup a LayoutChain to mimic the main one.
                LayoutChain layoutChain = new LayoutChain();
                layoutChain.addLink(new PopupAreaChainLink(GUILocationNames.ContentAreaPopup), true);
                layoutChain.addLink(new BorderLayoutNoAnimationChainLink(GUILocationNames.ContentArea), true);

                IntSize2 thumbTotalSize = new IntSize2(SlideImageManager.ThumbWidth, SlideImageManager.ThumbHeight);
                Bitmap thumb = slideEditorController.SlideImageManager.createThumbBitmap(slide);
                layoutChain.SuppressLayout = true;
                LayoutContainer sceneContainer = new NullLayoutContainer(thumbTotalSize);
                layoutChain.addContainer(new BorderLayoutElementName(GUILocationNames.ContentArea, BorderLayoutLocations.Center), sceneContainer, null);
                foreach (var editor in rmlEditors.Values)
                {
                    if (editor.Component != null)
                    {
                        float sizeRatio = (float)SlideImageManager.ThumbHeight / editor.Component.ViewHost.Container.RigidParentWorkingSize.Height;
                        IntSize2 size = (IntSize2)(editor.Component.ViewHost.Container.DesiredSize * sizeRatio);
                        NullLayoutContainer container = new NullLayoutContainer(size);
                        layoutPositions.Add(editor, container);
                        layoutChain.addContainer(editor.View.ElementName, container, null);
                    }
                }
                layoutChain.SuppressLayout = false;
                layoutChain.WorkingSize = thumbTotalSize;
                layoutChain.Location = new IntVector2(0, 0);
                layoutChain.layout();

                //Render thumbnail
                using (Graphics g = Graphics.FromImage(thumb))
                {
                    g.Clear(System.Drawing.Color.HotPink);
                    //Start with the scene
                    IntVector2 sceneThumbPosition = sceneContainer.Location;
                    String sceneThumbFile = Path.Combine(slide.UniqueName, "SceneThumb.png");
                    forceUpdateSceneThumb = includeLoc == new Vector2(0, 0); //Temporary, forces the thumb to regen the first time until we can persist this info
                    if (forceUpdateSceneThumb || !editorController.ResourceProvider.exists(sceneThumbFile))
                    {
                        ImageRendererProperties imageProperties = new ImageRendererProperties();
                        imageProperties.Width = 250;
                        imageProperties.Height = 250;
                        imageProperties.AntiAliasingMode = 2;
                        imageProperties.TransparentBackground = false;
                        imageProperties.UseActiveViewportLocation = false;
                        imageProperties.OverrideLayers = true;
                        imageProperties.ShowBackground = false;
                        imageProperties.ShowWatermark = false;
                        imageProperties.ShowUIUpdates = false;
                        imageProperties.LayerState = slide.Layers;
                        imageProperties.CameraPosition = slide.CameraPosition.Translation;
                        imageProperties.CameraLookAt = slide.CameraPosition.LookAt;
                        imageProperties.UseIncludePoint = slide.CameraPosition.UseIncludePoint;
                        imageProperties.IncludePoint = slide.CameraPosition.IncludePoint;
                        imageProperties.CustomizeCameraPosition = (camera) =>
                        {
                            SceneNode node = camera.getParentSceneNode();
                            Vector3 position = node.getDerivedPosition();
                            Vector3 lookAt = imageProperties.CameraLookAt;
                            Vector3 topLeft = SceneViewWindow.Unproject(0, 0, camera.getViewMatrix(), camera.getProjectionMatrix());
                            Vector3 bottomRight = SceneViewWindow.Unproject(1, 1, camera.getViewMatrix(), camera.getProjectionMatrix());
                            Vector3 include = imageProperties.IncludePoint;

                            //Move camera back more
                            //float distance = -60;
                            //Vector3 direction = (position - lookAt).normalized();
                            //node.setPosition(position - (direction * distance));
                            //camera.lookAt(lookAt);

                            includeLoc = (IntVector2)SceneViewWindow.Project(include, camera.getViewMatrix(), camera.getProjectionMatrix(), imageProperties.Width, imageProperties.Height);
                        };

                        using (Bitmap sceneThumb = imageRenderer.renderImage(imageProperties))
                        {
                            using (Graphics sceneGraph = Graphics.FromImage(sceneThumb))
                            {
                                using (Brush brush = new SolidBrush(System.Drawing.Color.HotPink))
                                {
                                    sceneGraph.FillRectangle(brush, includeLoc.x - 5, includeLoc.y - 5, 10, 10);
                                }
                                //using (Brush brush = new SolidBrush(System.Drawing.Color.FromArgb(0, 255, 0)))
                                //{
                                //    sceneGraph.FillRectangle(brush, topLeftLoc.x - 5, topLeftLoc.y - 5, 10, 10);
                                //    sceneGraph.FillRectangle(brush, bottomRightLoc.x - 5, bottomRightLoc.y - 5, 10, 10);
                                //}
                            }
                            g.DrawImage(sceneThumb, sceneThumbPosition.x, sceneThumbPosition.y);
                            //Cheat for now and just write the file
                            using (Stream stream = editorController.ResourceProvider.openWriteStream(sceneThumbFile))
                            {
                                sceneThumb.Save(stream, ImageFormat.Png);
                            }
                        }
                    }

                    using (Stream stream = editorController.ResourceProvider.openFile(sceneThumbFile))
                    {
                        IntSize2 centerSize = sceneContainer.WorkingSize;
                        RectangleF destRect = new RectangleF(sceneThumbPosition.x, sceneThumbPosition.y, centerSize.Width, centerSize.Height);

                        using (Bitmap sceneThumb = (Bitmap)Bitmap.FromStream(stream))
                        {
                            int requiredWidth = (sceneThumb.Width - ((sceneThumb.Width - includeLoc.x) * 2));
                            int requiredHeight = (sceneThumb.Height - (includeLoc.y * 2));
                            int requiredLeft = includeLoc.x - requiredWidth;
                            int requiredTop = includeLoc.y;

                            int sceneThumbHalfWidth = sceneThumb.Width / 2;
                            int sceneThumbHalfHeight = sceneThumb.Height / 2;
                            float heightWidthRatio = (float)requiredHeight / requiredWidth;

                            if (centerSize.Width < centerSize.Height) //Width is the dest rect limit
                            {
                                if (requiredWidth < requiredHeight) //Height of required area is greater, limit the height and move the top in the dest rectangle
                                {
                                    Logging.Log.Debug("1. Limit Dest {0}, {1} Width && Limit Required {2}, {3} Width", centerSize.Width, centerSize.Height, requiredWidth, requiredHeight);
                                    float limitedWidth = centerSize.Height / heightWidthRatio;
                                    destRect = new RectangleF(destRect.Width / 2 + destRect.Left - limitedWidth / 2, destRect.Top, limitedWidth, destRect.Height);
                                }
                                else //Width of required area is greater, limit the width and move the left in the dest rectangle
                                {
                                    Logging.Log.Debug("2. Limit Dest {0}, {1} Width && Limit Required {2}, {3} Height", centerSize.Width, centerSize.Height, requiredWidth, requiredHeight);
                                    float limitedHeight = centerSize.Width * heightWidthRatio;
                                    destRect = new RectangleF(destRect.Left, destRect.Height / 2 + destRect.Top - limitedHeight / 2, destRect.Width, limitedHeight); 
                                }
                            }
                            else //Height is the dest rect limit
                            {
                                if (requiredWidth < requiredHeight) //Height of required area is greater, limit the width and move the left in the dest rectangle
                                {
                                    Logging.Log.Debug("3. Limit Dest {0}, {1} Height && Limit Required {2}, {3} Width", centerSize.Width, centerSize.Height, requiredWidth, requiredHeight);
                                    float limitedWidth = centerSize.Height / heightWidthRatio;
                                    destRect = new RectangleF(destRect.Width / 2 + destRect.Left - limitedWidth / 2, destRect.Top, limitedWidth, destRect.Height);
                                }
                                else //Width of required area is greater, limit the height and move the top in the dest rectangle
                                {
                                    Logging.Log.Debug("4. Limit Dest {0}, {1} Height && Limit Required {2}, {3} Height", centerSize.Width, centerSize.Height, requiredWidth, requiredHeight);
                                    float limitedHeight = centerSize.Width * heightWidthRatio;
                                    destRect = new RectangleF(destRect.Left, destRect.Height / 2 + destRect.Top - limitedHeight / 2, destRect.Width, limitedHeight);
                                }
                            }

                            RectangleF srcRect = new RectangleF(sceneThumbHalfWidth - requiredWidth / 2, sceneThumbHalfHeight - requiredHeight / 2, requiredWidth, requiredHeight);

                            g.DrawImage(sceneThumb, destRect, srcRect, GraphicsUnit.Pixel);
                        }
                    }

                    //Render all panels
                    foreach (var editor in rmlEditors.Values)
                    {
                        if (editor.Component != null)
                        {
                            LayoutContainer container;
                            if (layoutPositions.TryGetValue(editor, out container))
                            {
                                Rectangle panelThumbPos = new Rectangle(container.Location.x, container.Location.y, container.WorkingSize.Width, container.WorkingSize.Height);
                                editor.Component.writeToGraphics(g, panelThumbPos);
                            }
                        }
                    }
                }
                
                slideEditorController.SlideImageManager.thumbnailUpdated(slide);
            }
        }

        /// <summary>
        /// This function will reload all open editors with their text as well as clearing the css cache.
        /// This can be used when larger sweeping changes are made and you just need to reload everything.
        /// </summary>
        public void refreshAllRml()
        {
            RocketGuiManager.clearAllCaches();
            foreach (var editor in rmlEditors.Values)
            {
                if (editor.Component != null)
                {
                    editor.Component.setRml(editor.Component.UnformattedRml, true, false);
                }
            }
        }

        private void blur(AnomalousMvcContext context)
        {
            commitText(forceUpdateThumbOnBlur);
            if (editorController.ResourceProvider != null) //If this is null the project is closed, no reason to try to save the text
            {
                foreach (RmlSlidePanel panel in slide.Panels.Where(p => p is RmlSlidePanel))
                {
                    String editorName = panel.createViewName("RmlView");
                    var editor = rmlEditors[editorName];
                    if (editor.Component != null && editor.Component.ChangesMade)
                    {
                        String rml = editor.getCurrentComponentText();
                        editor.CachedResource.CachedString = rml;
                        editorController.ResourceProvider.ResourceCache.add(editor.CachedResource);
                    }
                }
            }
            slideLayoutPicker.destroyLayoutPicker();
            GlobalContextEventHandler.disableEventContext(eventContext);
            htmlDragDrop.DestroyIconPreview();
            panelResizeWidget.destroyResizeWidget();
            if (Blur != null)
            {
                Blur.Invoke(this);
            }
        }

        private void refreshPanelEditors(bool replaceExistingEditors)
        {
            int oldEditorCount = rmlEditors.Count;
            if (replaceExistingEditors)
            {
                mvcContext.runAction("Editor/CloseEditors");
            }
            currentRmlEditor = null;
            closeEditorWindowsCommand.clear();
            showEditorWindowsCommand.clear();
            foreach (var editor in rmlEditors.Values)
            {
                mvcContext.Views.remove(editor.View);
                if (editor.Component != null)
                {
                    editor.Component.ElementDraggedOffDocument -= RmlWysiwyg_ElementDraggedOffDocument;
                    editor.Component.ElementDroppedOffDocument -= RmlWysiwyg_ElementDroppedOffDocument;
                    editor.Component.ElementReturnedToDocument -= RmlWysiwyg_ElementReturnedToDocument;
                    editor.Component.cancelAndHideEditor();
                }
            }
            rmlEditors.Clear();

            SlideDisplayManager displayManager = new SlideDisplayManager();
            foreach (RmlSlidePanel panel in slide.Panels.Where(p => p is RmlSlidePanel))
            {
                SlideInstanceLayoutStrategy instanceLayout = slide.LayoutStrategy.createLayoutStrategy(displayManager);
                String editorViewName = panel.createViewName("RmlView");
                RmlWysiwygView rmlView = new RmlWysiwygView(editorViewName, this.uiCallback, this.uiCallback, this.undoBuffer);
                rmlView.ElementName = panel.ElementName;
                rmlView.RmlFile = panel.getRmlFilePath(slide);
                rmlView.ContentId = "Content";
                instanceLayout.addView(rmlView);
                rmlView.ComponentCreated += (view, component) =>
                {
                    var editor = rmlEditors[view.Name];
                    editor.Component = component;
                    component.RmlEdited += rmlEditor =>
                    {
                        String rml = rmlEditor.CurrentRml;
                        editor.CachedResource.CachedString = rml;
                        editorController.ResourceProvider.ResourceCache.add(editor.CachedResource);
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
                rmlEditors.Add(rmlView.Name, new RmlEditorViewInfo(rmlView, panel, editorController.ResourceProvider));
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
            if (currentRmlEditor != name)
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
            updateThumbnail();
        }

        void slideLayoutPicker_ChangeSlideLayout(TemplateSlide newSlideLayout)
        {
            undoBuffer.pushAndExecute(new TwoWayDelegateCommand<TemplateSlide, TemplateSlide>(
                editorController.applySlideLayout, newSlideLayout,
                editorController.applySlideLayout, slide.createTemplateSlide(editorController.ResourceProvider)));
        }

        private void makeTempPresets()
        {
            String defaultRml = EmbeddedResourceHelpers.ReadResourceContents(EmbeddedTemplateNames.SimpleSlide_rml, EmbeddedTemplateNames.Assembly);
            //Couple simple presets
            TemplateSlide presetSlide = new TemplateSlide()
            {
                Name = "Left Panel",
                IconName = "Lecture.SlideLayouts.OnePanel"
            };
            presetSlide.addPanel(new RmlSlidePanelTemplate()
            {
                Rml = defaultRml,
                Size = 480,
                ElementName = new BorderLayoutElementName(GUILocationNames.ContentArea, BorderLayoutLocations.Left),
            });
            slideLayoutPicker.addPresetSlide(presetSlide);

            presetSlide = new TemplateSlide()
            {
                Name = "Left and Right Panel",
                IconName = "Lecture.SlideLayouts.TwoPanel"
            };
            presetSlide.addPanel(new RmlSlidePanelTemplate()
            {
                Rml = defaultRml,
                Size = 480,
                ElementName = new BorderLayoutElementName(GUILocationNames.ContentArea, BorderLayoutLocations.Left),
            });
            presetSlide.addPanel(new RmlSlidePanelTemplate()
            {
                Rml = defaultRml,
                Size = 480,
                ElementName = new BorderLayoutElementName(GUILocationNames.ContentArea, BorderLayoutLocations.Right),
            });
            slideLayoutPicker.addPresetSlide(presetSlide);

            presetSlide = new TemplateSlide()
            {
                Name = "Sides and Top",
                IconName = "Lecture.SlideLayouts.ThreePanel"
            };
            presetSlide.addPanel(new RmlSlidePanelTemplate()
            {
                Rml = defaultRml,
                Size = 480,
                ElementName = new BorderLayoutElementName(GUILocationNames.ContentArea, BorderLayoutLocations.Left),
            });
            presetSlide.addPanel(new RmlSlidePanelTemplate()
            {
                Rml = defaultRml,
                Size = 480,
                ElementName = new BorderLayoutElementName(GUILocationNames.ContentArea, BorderLayoutLocations.Right),
            });
            presetSlide.addPanel(new RmlSlidePanelTemplate()
            {
                Rml = defaultRml,
                Size = 288,
                ElementName = new BorderLayoutElementName(GUILocationNames.ContentArea, BorderLayoutLocations.Top),
            });
            slideLayoutPicker.addPresetSlide(presetSlide);

            presetSlide = new TemplateSlide()
            {
                Name = "Four Panel",
                IconName = "Lecture.SlideLayouts.FourPanel"
            };
            presetSlide.addPanel(new RmlSlidePanelTemplate()
            {
                Rml = defaultRml,
                Size = 480,
                ElementName = new BorderLayoutElementName(GUILocationNames.ContentArea, BorderLayoutLocations.Left),
            });
            presetSlide.addPanel(new RmlSlidePanelTemplate()
            {
                Rml = defaultRml,
                Size = 480,
                ElementName = new BorderLayoutElementName(GUILocationNames.ContentArea, BorderLayoutLocations.Right),
            });
            presetSlide.addPanel(new RmlSlidePanelTemplate()
            {
                Rml = defaultRml,
                Size = 288,
                ElementName = new BorderLayoutElementName(GUILocationNames.ContentArea, BorderLayoutLocations.Top),
            });
            presetSlide.addPanel(new RmlSlidePanelTemplate()
            {
                Rml = defaultRml,
                Size = 288,
                ElementName = new BorderLayoutElementName(GUILocationNames.ContentArea, BorderLayoutLocations.Bottom),
            });
            slideLayoutPicker.addPresetSlide(presetSlide);

            presetSlide = new TemplateSlide()
            {
                Name = "Sides and Bottom",
                IconName = "Lecture.SlideLayouts.FourPanel"
            };
            presetSlide.addPanel(new RmlSlidePanelTemplate()
            {
                Rml = defaultRml,
                Size = 480,
                ElementName = new BorderLayoutElementName(GUILocationNames.ContentArea, BorderLayoutLocations.Left),
            });
            presetSlide.addPanel(new RmlSlidePanelTemplate()
            {
                Rml = defaultRml,
                Size = 480,
                ElementName = new BorderLayoutElementName(GUILocationNames.ContentArea, BorderLayoutLocations.Right),
            });
            presetSlide.addPanel(new RmlSlidePanelTemplate()
            {
                Rml = defaultRml,
                Size = 288,
                ElementName = new BorderLayoutElementName(GUILocationNames.ContentArea, BorderLayoutLocations.Bottom),
            });
            slideLayoutPicker.addPresetSlide(presetSlide);

            presetSlide = new TemplateSlide(new FullScreenSlideLayoutStrategy())
            {
                Name = "Full Screen",
                IconName = "Lecture.SlideLayouts.Full",
            };
            presetSlide.addPanel(new RmlSlidePanelTemplate()
            {
                Rml = defaultRml,
                Size = 100,
                ElementName = new LayoutElementName(GUILocationNames.ContentAreaPopup),
            });
            slideLayoutPicker.addPresetSlide(presetSlide);

            presetSlide = new TemplateSlide()
            {
                Name = "Left and Top",
                IconName = "Lecture.SlideLayouts.LeftTop"
            };
            presetSlide.addPanel(new RmlSlidePanelTemplate()
            {
                Rml = defaultRml,
                Size = 480,
                ElementName = new BorderLayoutElementName(GUILocationNames.ContentArea, BorderLayoutLocations.Left),
            });
            presetSlide.addPanel(new RmlSlidePanelTemplate()
            {
                Rml = defaultRml,
                Size = 288,
                ElementName = new BorderLayoutElementName(GUILocationNames.ContentArea, BorderLayoutLocations.Top),
            });
            slideLayoutPicker.addPresetSlide(presetSlide);
        }
    }
}
