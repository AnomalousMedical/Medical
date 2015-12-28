﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller.AnomalousMvc;
using Medical.GUI.AnomalousMvc;
using Medical.GUI;
using System.IO;
using MyGUIPlugin;
using Engine.Platform;
using Medical.Editor;
using Medical;
using Lecture.GUI;
using Engine;
using Engine.Editing;
using Medical.SlideshowActions;
using Medical.Controller;
using OgrePlugin;
using FreeImageAPI;
using Engine.Threads;
using Anomalous.libRocketWidget;
using Anomalous.GuiFramework;
using Anomalous.GuiFramework.Cameras;

namespace Lecture
{
    class SlideEditorContext
    {
        public event Action<SlideEditorContext> Focus;
        public event Action<SlideEditorContext> Blur;

        private Dictionary<String, RmlEditorViewInfo> rmlEditors = new Dictionary<string, RmlEditorViewInfo>();
        private String currentRmlEditor;
        private AnomalousMvcContext mvcContext;
        private EventContext eventContext;
        private Slide slide;
        private LectureUICallback uiCallback;
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
        private DragAndDropTaskManager<WysiwygDragDropItem> htmlDragDrop;
        private SlideshowStyleManager styleManager;
        private SlideDisplayManager displayManager;
        private LayerState undoState = null;
        private CameraPosition undoCamera = null;
        private LayerController layerController;
        private SceneViewController sceneViewController;

        SlideImageStrategy imageStrategy;
        SlideTriggerStrategy triggerStrategy;
        SlideInputStrategy inputStrategy;

        public SlideEditorContext(Slide slide, String slideName, SlideshowEditController editorController, StandaloneController standaloneController, LectureUICallback uiCallback, UndoRedoBuffer undoBuffer, MedicalSlideItemTemplate itemTemplate, bool autoSetupScene, Action<String, String> wysiwygUndoCallback)
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
            this.imageRenderer = standaloneController.ImageRenderer;
            this.itemTemplate = itemTemplate;
            this.wysiwygUndoCallback = wysiwygUndoCallback;
            this.editorController = editorController;
            this.layerController = standaloneController.LayerController;
            this.sceneViewController = standaloneController.SceneViewController;
            panelResizeWidget = new PanelResizeWidget();
            panelResizeWidget.RecordResizeUndo += panelResizeWidget_RecordResizeUndo;

            displayManager = new SlideDisplayManager(editorController.VectorMode);

            RunCommandsAction previewTriggerAction = new RunCommandsAction("PreviewTrigger");

            imageStrategy = new SlideImageStrategy(slide, undoBuffer, this.slideEditorController.ResourceProvider, slide.UniqueName);
            triggerStrategy = new SlideTriggerStrategy(slide, createTriggerActionBrowser(), undoBuffer, "a", "Lecture.Icon.TriggerIcon", standaloneController.NotificationManager, previewTriggerAction);
            triggerStrategy.PreviewTrigger += triggerStrategy_PreviewTrigger;
            inputStrategy = new SlideInputStrategy(slide, undoBuffer, standaloneController.NotificationManager, previewTriggerAction, "input", CommonResources.NoIcon);
            inputStrategy.PreviewTrigger += triggerStrategy_PreviewTrigger;

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
                new WysiwygDragDropItem("Heading", "Editor/HeaderIcon", "<h1>Add Heading Here</h1>"),
                new WysiwygDragDropItem("Paragraph", "Editor/ParagraphsIcon", "<p>Add paragraph text here.</p>"),
                new WysiwygCallbackDragDropItem("Image", "Editor/ImageIcon", String.Format("<img src=\"{0}\" style=\"width:200px;\"></img>", RmlWysiwygComponent.DefaultImage), 
                    () => //Markup Callback
                    {
                        String actionName = Guid.NewGuid().ToString();
                        ShowPopupImageAction action = new ShowPopupImageAction(actionName)
                        {
                            ImageName = RmlWysiwygComponent.DefaultImage
                        };
                        slide.addAction(action);
                        return String.Format("<img src=\"{0}\" style=\"width:200px;\" onclick=\"{1}\"></img>", RmlWysiwygComponent.DefaultImage, actionName);
                    }),
                new WysiwygDragDropItem("Data Dispaly", CommonResources.NoIcon, "<data type=\"volume\" target=\"\">Data Display</data>"),
                new WysiwygCallbackDragDropItem("Trigger", "Lecture.Icon.TriggerIcon", "<a class=\"TriggerLink\" onclick=\"\">Add trigger text here.</a>",
                    () => //Markup Callback
                    {
                        String actionName = Guid.NewGuid().ToString();
                        SetupSceneAction action = new SetupSceneAction(actionName);
                        action.captureSceneState(uiCallback);
                        slide.addAction(action);
                        return String.Format("<a class=\"TriggerLink\" onclick=\"{0}\">Add trigger text here.</a>", actionName);
                    }),
                new WysiwygCallbackDragDropItem("Slider", CommonResources.NoIcon, "<input type=\"range\" min=\"0\" max=\"100\" value=\"0\" change=\"\"/>",
                    () => //Markup Callback
                    {
                        String actionName = Guid.NewGuid().ToString();
                        BlendSceneAction action = new BlendSceneAction(actionName);
                        action.captureSceneToStartAndEnd(uiCallback);
                        slide.addAction(action);
                        return String.Format("<input type=\"range\" min=\"0\" max=\"100\" value=\"0\" onchange=\"{0}\"/>", actionName);
                    })
                );
            htmlDragDrop.Dragging += (item, position) =>
                {
                    foreach (var editor in rmlEditors.Values)
                    {
                        editor.Component.setPreviewElement(position, item.PreviewMarkup, item.PreviewTagType);
                    }
                };
            htmlDragDrop.DragEnded += (item, position) =>
                {
                    bool allowAdd = true;
                    foreach (var editor in rmlEditors.Values)
                    {
                        if (allowAdd && editor.Component.contains(position))
                        {
                            editor.Component.insertRml(item.createDocumentMarkup());
                            setCurrentRmlEditor(editor.View.Name);
                            allowAdd = false;
                        }
                        else
                        {
                            editor.Component.cancelAndHideEditor();
                            editor.Component.clearPreviewElement(false);
                        }
                    }
                };
            htmlDragDrop.ItemActivated += (item) =>
                {
                    rmlEditors[currentRmlEditor].Component.insertRml(item.createDocumentMarkup());
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
                editorController.editTimeline(slide, "Timeline.tl", "Timeline");
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
            taskbar.addTask(new CallbackTask("EditSlideshowTheme", "Edit Slideshow Theme", "Lecture.Icon.EditStyle", "Edit", 0, true, item =>
            {
                IntVector2 taskPosition = item.CurrentTaskPositioner.findGoodWindowPosition(SlideshowStyleManager.Width, SlideshowStyleManager.Height);
                styleManager.showEditor(taskPosition.x, taskPosition.y);
            }));

            taskbar.addTask(new CallbackTask("ResetSlide", "Reset Slide", "Lecture.Icon.RevertIcon", "Edit", 0, true, item =>
            {
                resetSlide();
            }));

            mvcContext.Views.add(taskbar);

            setupScene = new RunCommandsAction("SetupScene");
            if (autoSetupScene)
            {
                setupScene.addCommand(new CallbackCommand(context =>
                    {
                        undoState = LayerState.CreateAndCapture();
                        undoCamera = sceneViewController.ActiveWindow != null ? sceneViewController.ActiveWindow.createCameraPosition() : null;
                    }));
                slide.populateCommand(setupScene);
                setupScene.addCommand(new CallbackCommand(context =>
                {
                    if (undoState != null)
                    {
                        layerController.pushUndoState(undoState);
                        undoState = null;
                    }
                    if (undoCamera != null)
                    {
                        if (sceneViewController.ActiveWindow != null)
                        {
                            sceneViewController.ActiveWindow.pushUndoState(undoCamera);
                        }
                        undoCamera = null;
                    }
                }));
            }

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
                    if (currentRmlEditor != null) //Make sure we have an active editor
                    {
                        String current = currentRmlEditor;
                        currentRmlEditor = null;
                        setCurrentRmlEditor(current);
                    }
                    this.slideEditorController.VectorModeChanged += slideEditorController_VectorModeChanged;
                }),
                new CallbackAction("Blur", blur),
                new RunCommandsAction("Suspended", new SaveViewLayoutCommand()),
                new RunCommandsAction("Resumed", new RestoreViewLayoutCommand()),
                previewTriggerAction));

            eventContext = new EventContext();
            ButtonEvent saveEvent = new ButtonEvent(EventLayers.Gui);
            saveEvent.addButton(KeyboardButtonCode.KC_LCONTROL);
            saveEvent.addButton(KeyboardButtonCode.KC_S);
            saveEvent.FirstFrameUpEvent += eventManager =>
            {
                saveAll();
            };
            eventContext.addEvent(saveEvent);

            ButtonEvent undoEvent = new ButtonEvent(EventLayers.Gui);
            undoEvent.addButton(KeyboardButtonCode.KC_LCONTROL);
            undoEvent.addButton(KeyboardButtonCode.KC_Z);
            undoEvent.FirstFrameUpEvent += eventManager =>
            {
                undoBuffer.undo();
            };
            eventContext.addEvent(undoEvent);

            ButtonEvent redoEvent = new ButtonEvent(EventLayers.Gui);
            redoEvent.addButton(KeyboardButtonCode.KC_LCONTROL);
            redoEvent.addButton(KeyboardButtonCode.KC_Y);
            redoEvent.FirstFrameUpEvent += eventManager =>
            {
                undoBuffer.execute();
            };
            eventContext.addEvent(redoEvent);

            ButtonEvent runEvent = new ButtonEvent(EventLayers.Gui);
            runEvent.addButton(KeyboardButtonCode.KC_F5);
            runEvent.FirstFrameUpEvent += eventManager =>
            {
                ThreadManager.invoke(() =>
                {
                    editorController.runSlideshow(0);
                });
            };
            eventContext.addEvent(runEvent);

            ButtonEvent captureEvent = new ButtonEvent(EventLayers.Gui);
            captureEvent.addButton(KeyboardButtonCode.KC_LCONTROL);
            captureEvent.addButton(KeyboardButtonCode.KC_SPACE);
            captureEvent.FirstFrameUpEvent += eventManager =>
            {
                editorController.capture();
            };
            eventContext.addEvent(captureEvent);
        }

        public void resetSlide()
        {
            mvcContext.runAction("Editor/SetupScene");
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
            template.copyLayoutToSlide(slide, editorController.ResourceProvider, false);
            refreshPanelEditors(true);
            updateThumbnail();
        }

        public SlideSceneInfo getCurrentSceneInfo()
        {
            SceneThumbInfo sceneThumbInfo = slideEditorController.SlideImageManager.loadThumbSceneBitmap(slide, renderSceneThumbnail);
            return new SlideSceneInfo(slide, sceneThumbInfo.copy());
        }

        public void slideNameChanged(string slideName)
        {
            taskbar.DisplayName = slideName;
        }

        public void applySceneInfo(SlideSceneInfo info)
        {
            if (info.SceneThumbInfo != null)
            {
                slideEditorController.SlideImageManager.addUnsavedSceneThumb(slide, info.SceneThumbInfo.copy());
            }
            setupScene.clear();
            info.applyToSlide(slide);
            slide.populateCommand(setupScene);
            mvcContext.runAction("Editor/SetupScene");
            updateThumbnail();
        }

        internal void capture()
        {
            itemTemplate.applySceneStateToSlide(slide);
            slide.populateCommand(setupScene);
            mvcContext.runAction("Editor/SetupScene");
            updateThumbnail(true);
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

        public void commitText()
        {
            bool updateThumb = false;
            foreach (var editor in rmlEditors.Values)
            {
                updateThumb = editor.commitText() | updateThumb;
            }
            if (updateThumb)
            {
                updateThumbnail();
            }
        }

        public void updateThumbnail(bool forceUpdateSceneThumb = false)
        {
            Dictionary<RmlEditorViewInfo, LayoutContainer> layoutPositions = new Dictionary<RmlEditorViewInfo, LayoutContainer>();
            if (slideEditorController.ResourceProvider != null)
            {
                //Setup a LayoutChain to mimic the main one.
                LayoutChain layoutChain = new LayoutChain();
                layoutChain.addLink(new PopupAreaChainLink(GUILocationNames.ContentAreaPopup), true);
                layoutChain.addLink(new BorderLayoutNoAnimationChainLink(GUILocationNames.ContentArea), true);

                IntSize2 thumbTotalSize = new IntSize2(SlideImageManager.ThumbWidth, SlideImageManager.ThumbHeight);
                FreeImageBitmap thumb = slideEditorController.SlideImageManager.createThumbBitmap(slide);
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

                //Render thumbnail, Start with the scene
                IntVector2 sceneThumbPosition = sceneContainer.Location;
                String sceneThumbFile = slide.SceneThumbName;
                if (forceUpdateSceneThumb)
                {
                    slideEditorController.SlideImageManager.addUnsavedSceneThumb(slide, renderSceneThumbnail());
                }

                SceneThumbInfo sceneThumbInfo = slideEditorController.SlideImageManager.loadThumbSceneBitmap(slide, renderSceneThumbnail);
                IntSize2 centerSize = sceneContainer.WorkingSize;
                RectangleF destRect = new RectangleF(sceneThumbPosition.x, sceneThumbPosition.y, centerSize.Width, centerSize.Height);

                thumb.FillBackground(new RGBQUAD(FreeImageAPI.Color.FromArgb(sceneThumbInfo.Color.toARGB())));

                int requiredWidth = (sceneThumbInfo.SceneThumb.Width - ((sceneThumbInfo.SceneThumb.Width - sceneThumbInfo.IncludeX) * 2));
                int requiredHeight = (sceneThumbInfo.SceneThumb.Height - (sceneThumbInfo.IncludeY * 2));

                int sceneThumbWidth = sceneThumbInfo.SceneThumb.Width;
                int sceneThumbHeight = sceneThumbInfo.SceneThumb.Height;
                int sceneThumbHalfWidth = sceneThumbWidth / 2;
                int sceneThumbHalfHeight = sceneThumbHeight / 2;
                float requiredHeightWidthRatio = (float)requiredHeight / requiredWidth;
                float centerHeightWidthRatio = (float)centerSize.Height / centerSize.Width;

                float srcWidth = requiredWidth;
                float srcHeight = requiredHeight;

                if (requiredHeightWidthRatio < centerHeightWidthRatio) //Compare ratios between our source required area and the destination
                {
                    //Use the full required width, add height from source image
                    //Convert the center size to the same size ratio as the required size
                    float sizeRatio = (float)requiredWidth / centerSize.Width;
                    srcHeight = centerSize.Height * sizeRatio;
                    if (srcHeight > sceneThumbHeight) //Stretch out the image as much as possible, limiting by the size of the scene thumb if needed.
                    {
                        srcHeight = sceneThumbHeight;
                    }
                    float destHeight = srcHeight / sizeRatio;
                    destRect = new RectangleF(destRect.Left, destRect.Height / 2 + destRect.Top - destHeight / 2, destRect.Width, destHeight); //Make a dest rect that takes as much image as it can
                }
                else
                {
                    //Use the full required height, add width from source image
                    float sizeRatio = (float)requiredHeight / centerSize.Height;
                    srcWidth = centerSize.Width * sizeRatio;
                    if (srcWidth > sceneThumbWidth)
                    {
                        srcWidth = sceneThumbWidth;
                    }
                    float destWidth = srcWidth / sizeRatio;
                    destRect = new RectangleF(destRect.Width / 2 + destRect.Left - destWidth / 2, destRect.Top, destWidth, destRect.Height);
                }

                RectangleF srcRect = new RectangleF(sceneThumbHalfWidth - srcWidth / 2, sceneThumbHalfHeight - srcHeight / 2, srcWidth, srcHeight);

                using (FreeImageBitmap resizedSceneThumb = sceneThumbInfo.SceneThumb.Copy((int)srcRect.X, (int)srcRect.Y, (int)srcRect.Right, (int)srcRect.Bottom))
                {
                    resizedSceneThumb.Rescale((int)destRect.Width, (int)destRect.Height, FREE_IMAGE_FILTER.FILTER_BILINEAR);
                    thumb.Paste(resizedSceneThumb, (int)destRect.X, (int)destRect.Y, int.MaxValue);
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
                            editor.Component.writeToGraphics(thumb, panelThumbPos);
                        }
                    }
                }

                slideEditorController.SlideImageManager.thumbnailUpdated(slide);
            }
        }

        private SceneThumbInfo renderSceneThumbnail()
        {
            SceneThumbInfo sceneThumbInfo = new SceneThumbInfo();
            ImageRendererProperties imageProperties = new ImageRendererProperties();
            imageProperties.Width = SlideImageManager.SceneThumbWidth;
            imageProperties.Height = SlideImageManager.SceneThumbHeight;
            imageProperties.AntiAliasingMode = 2;
            imageProperties.TransparentBackground = false;
            imageProperties.UseActiveViewportLocation = false;
            imageProperties.OverrideLayers = true;
            imageProperties.ShowBackground = false;
            imageProperties.ShowWatermark = false;
            imageProperties.ShowUIUpdates = false;
            slide.StartupAction.configureThumbnailProperties(imageProperties);
            imageProperties.CustomizeCameraPosition = (camera, viewport) =>
            {
                SceneNode node = camera.getParentSceneNode();
                Vector3 position = node.getDerivedPosition();
                Vector3 lookAt = imageProperties.CameraLookAt;
                Vector3 topLeft = SceneViewWindow.Unproject(0, 0, camera.getViewMatrix(), camera.getProjectionMatrix());
                Vector3 bottomRight = SceneViewWindow.Unproject(1, 1, camera.getViewMatrix(), camera.getProjectionMatrix());
                Vector3 include = imageProperties.IncludePoint;

                //Move camera back more
                float distance = -60;
                Vector3 direction = (position - lookAt).normalized();
                node.setPosition(position - (direction * distance));
                camera.lookAt(lookAt);

                Vector2 includeLoc = SceneViewWindow.Project(include, camera.getViewMatrix(), camera.getProjectionMatrix(), imageProperties.Width, imageProperties.Height);
                sceneThumbInfo.IncludeX = (int)includeLoc.x;
                sceneThumbInfo.IncludeY = (int)includeLoc.y;
                sceneThumbInfo.Color = viewport.getBackgroundColor();
            };

            sceneThumbInfo.SceneThumb = imageRenderer.renderImage(imageProperties);
            return sceneThumbInfo;
        }

        /// <summary>
        /// This function will reload all open editors with their text as well as clearing the css cache.
        /// This can be used when larger sweeping changes are made and you just need to reload everything.
        /// </summary>
        public void refreshAllRml()
        {
            RocketWidgetInterface.clearAllCaches();
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
            this.slideEditorController.VectorModeChanged -= slideEditorController_VectorModeChanged;
            commitText();
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

            foreach (RmlSlidePanel panel in slide.Panels.Where(p => p is RmlSlidePanel))
            {
                SlideInstanceLayoutStrategy instanceLayout = slide.LayoutStrategy.createLayoutStrategy(displayManager);
                String editorViewName = panel.createViewName("RmlView");
                RmlWysiwygView rmlView = new RmlWysiwygView(editorViewName, this.uiCallback, this.undoBuffer);
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
                        updateThumbnail();
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
                rmlView.GetMissingRmlCallback = getDefaultMissingRml;
                rmlView.addCustomStrategy(imageStrategy);
                rmlView.addCustomStrategy(triggerStrategy);
                rmlView.addCustomStrategy(inputStrategy);
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
                    if (editor.Component.contains(position))
                    {
                        editor.Component.insertRml(innerRmlHint);
                        setCurrentRmlEditor(editor.View.Name);
                    }
                    else
                    {
                        editor.Component.cancelAndHideEditor();
                        editor.Component.clearPreviewElement(false);
                    }
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

            rootNode.addChild(new BrowserNode("Stop Timeline", new Func<String, SlideAction>((name) =>
            {
                return new StopTimelineAction(name);
            })));

            return browser;
        }

        void action_EditTimeline(PlayTimelineAction action)
        {
            String text = "Trigger Timeline";
            String actionName = action.Name;
            if (currentRmlEditor != null)
            {
                var editor = rmlEditors[currentRmlEditor];
                var element = editor.Component.TopContentElement.GetElementsWithAttribute("onclick").FirstOrDefault(e => e.GetAttributeString("onclick").Equals(actionName));
                if (element != null)
                {
                    text = element.InnerRml;
                    if (text.Length > 33)
                    {
                        text = text.Substring(0, 30) + "...";
                    }
                    text = String.Format("Trigger Timeline - \"{0}\"", text);
                }
            }
            editorController.editTimeline(slide, action.TimelineFileName, text);
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
            updateThumbnail();
        }

        void panelResizeWidget_RecordResizeUndo(RmlEditorViewInfo view, int oldSize, int newSize)
        {
            updateThumbnail();
        }

        void slideLayoutPicker_ChangeSlideLayout(TemplateSlide newSlideLayout)
        {
            undoBuffer.pushAndExecute(new TwoWayDelegateCommand<TemplateSlide, TemplateSlide>(newSlideLayout, slide.createTemplateSlide(editorController.ResourceProvider),
                new TwoWayDelegateCommand<TemplateSlide, TemplateSlide>.Funcs()
                {
                    ExecuteFunc = editorController.applySlideLayout,
                    UndoFunc = editorController.applySlideLayout, 
                }));
        }

        void slideEditorController_VectorModeChanged(SlideshowEditController obj)
        {
            displayManager.VectorMode = obj.VectorMode;
            if (rmlEditors.Count > 0)
            {
                rmlEditors.First().Value.Component.ViewHost.Container.invalidate();
            }
        }

        private string getDefaultMissingRml(string file, AnomalousMvcContext context)
        {
            using (StreamReader stream = new StreamReader(this.GetType().Assembly.GetManifestResourceStream("Lecture.Controller.MissingFile.rml")))
            {
                return String.Format(stream.ReadToEnd(), file, context.ResourceProvider.BackingLocation);
            }
        }

        void triggerStrategy_PreviewTrigger()
        {
            mvcContext.runAction("Common/PreviewTrigger");
        }

        private void makeTempPresets()
        {
            const int TitleSize = 150;
            const int SideSize = 480;
            const int FooterSize = 200;

            String defaultRml = EmbeddedResourceHelpers.ReadResourceContents(EmbeddedTemplateNames.SimpleSlide_rml, EmbeddedTemplateNames.Assembly);
            //Couple simple presets
            TemplateSlide presetSlide = new TemplateSlide()
            {
                Name = "Text",
                IconName = "Lecture.SlideLayouts.OnePanel"
            };
            presetSlide.addPanel(new RmlSlidePanelTemplate()
            {
                Rml = defaultRml,
                Size = SideSize,
                ElementName = new BorderLayoutElementName(GUILocationNames.ContentArea, BorderLayoutLocations.Left),
            });
            slideLayoutPicker.addPresetSlide(presetSlide);

            presetSlide = new TemplateSlide()
            {
                Name = "Title and Text",
                IconName = "Lecture.SlideLayouts.LeftTop"
            };
            presetSlide.addPanel(new RmlSlidePanelTemplate()
            {
                Rml = defaultRml,
                Size = SideSize,
                ElementName = new BorderLayoutElementName(GUILocationNames.ContentArea, BorderLayoutLocations.Left),
            });
            presetSlide.addPanel(new RmlSlidePanelTemplate()
            {
                Rml = defaultRml,
                Size = TitleSize,
                ElementName = new BorderLayoutElementName(GUILocationNames.ContentArea, BorderLayoutLocations.Top),
            });
            slideLayoutPicker.addPresetSlide(presetSlide);

            presetSlide = new TemplateSlide()
            {
                Name = "Comparison",
                IconName = "Lecture.SlideLayouts.TwoPanel"
            };
            presetSlide.addPanel(new RmlSlidePanelTemplate()
            {
                Rml = defaultRml,
                Size = SideSize,
                ElementName = new BorderLayoutElementName(GUILocationNames.ContentArea, BorderLayoutLocations.Left),
            });
            presetSlide.addPanel(new RmlSlidePanelTemplate()
            {
                Rml = defaultRml,
                Size = SideSize,
                ElementName = new BorderLayoutElementName(GUILocationNames.ContentArea, BorderLayoutLocations.Right),
            });
            slideLayoutPicker.addPresetSlide(presetSlide);

            presetSlide = new TemplateSlide()
            {
                Name = "Title and\nComparison",
                IconName = "Lecture.SlideLayouts.ThreePanel"
            };
            presetSlide.addPanel(new RmlSlidePanelTemplate()
            {
                Rml = defaultRml,
                Size = SideSize,
                ElementName = new BorderLayoutElementName(GUILocationNames.ContentArea, BorderLayoutLocations.Left),
            });
            presetSlide.addPanel(new RmlSlidePanelTemplate()
            {
                Rml = defaultRml,
                Size = SideSize,
                ElementName = new BorderLayoutElementName(GUILocationNames.ContentArea, BorderLayoutLocations.Right),
            });
            presetSlide.addPanel(new RmlSlidePanelTemplate()
            {
                Rml = defaultRml,
                Size = TitleSize,
                ElementName = new BorderLayoutElementName(GUILocationNames.ContentArea, BorderLayoutLocations.Top),
            });
            slideLayoutPicker.addPresetSlide(presetSlide);

            presetSlide = new TemplateSlide()
            {
                Name = "Comparison\nand Footer",
                IconName = "Lecture.SlideLayouts.FourPanel"
            };
            presetSlide.addPanel(new RmlSlidePanelTemplate()
            {
                Rml = defaultRml,
                Size = SideSize,
                ElementName = new BorderLayoutElementName(GUILocationNames.ContentArea, BorderLayoutLocations.Left),
            });
            presetSlide.addPanel(new RmlSlidePanelTemplate()
            {
                Rml = defaultRml,
                Size = SideSize,
                ElementName = new BorderLayoutElementName(GUILocationNames.ContentArea, BorderLayoutLocations.Right),
            });
            presetSlide.addPanel(new RmlSlidePanelTemplate()
            {
                Rml = defaultRml,
                Size = FooterSize,
                ElementName = new BorderLayoutElementName(GUILocationNames.ContentArea, BorderLayoutLocations.Bottom),
            });
            slideLayoutPicker.addPresetSlide(presetSlide);

            presetSlide = new TemplateSlide()
            {
                Name = "Everything",
                IconName = "Lecture.SlideLayouts.FourPanel"
            };
            presetSlide.addPanel(new RmlSlidePanelTemplate()
            {
                Rml = defaultRml,
                Size = SideSize,
                ElementName = new BorderLayoutElementName(GUILocationNames.ContentArea, BorderLayoutLocations.Left),
            });
            presetSlide.addPanel(new RmlSlidePanelTemplate()
            {
                Rml = defaultRml,
                Size = SideSize,
                ElementName = new BorderLayoutElementName(GUILocationNames.ContentArea, BorderLayoutLocations.Right),
            });
            presetSlide.addPanel(new RmlSlidePanelTemplate()
            {
                Rml = defaultRml,
                Size = TitleSize,
                ElementName = new BorderLayoutElementName(GUILocationNames.ContentArea, BorderLayoutLocations.Top),
            });
            presetSlide.addPanel(new RmlSlidePanelTemplate()
            {
                Rml = defaultRml,
                Size = FooterSize,
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
        }
    }
}
