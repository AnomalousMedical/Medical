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

        private RmlWysiwygComponent rmlComponent;
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

        public SlideEditorContext(MedicalRmlSlide slide, String slideName, SlideshowEditController editorController, EditorUICallback uiCallback, UndoRedoBuffer undoBuffer, ImageRenderer imageRenderer, MedicalSlideItemTemplate itemTemplate, Action<String> wysiwygUndoCallback)
        {
            this.slide = slide;
            this.uiCallback = uiCallback;
            this.slideEditorController = editorController;
            this.undoBuffer = undoBuffer;
            this.imageRenderer = imageRenderer;
            this.itemTemplate = itemTemplate;

            mvcContext = new AnomalousMvcContext();
            mvcContext.StartupAction = "Common/Start";
            mvcContext.FocusAction = "Common/Focus";
            mvcContext.BlurAction = "Common/Blur";
            mvcContext.SuspendAction = "Common/Suspended";
            mvcContext.ResumeAction = "Common/Resumed";
            
            RawRmlWysiwygView rmlView = new RawRmlWysiwygView("RmlView", uiCallback, uiCallback, undoBuffer);
            rmlView.ViewLocation = ViewLocations.Left;
            rmlView.IsWindow = true;
            rmlView.Rml = slide.Rml;
            rmlView.ComponentCreated += (view, component) =>
            {
                rmlComponent = component;
                rmlComponent.RmlEdited += rmlEditor =>
                {
                    slide.Rml = rmlEditor.CurrentRml;
                };
            };
            rmlView.UndoRedoCallback = wysiwygUndoCallback;
            mvcContext.Views.add(rmlView);

            DragAndDropView<WysiwygDragDropItem> htmlDragDrop = new DragAndDropView<WysiwygDragDropItem>("HtmlDragDrop",
                new WysiwygDragDropItem("Heading", "Editor/HeaderIcon", "<h1>Heading</h1>"),
                new WysiwygDragDropItem("Paragraph", "Editor/ParagraphsIcon", "<p>Add paragraph text here.</p>"),
                new WysiwygDragDropItem("Image", "Editor/ImageIcon", String.Format("<img src=\"{0}\"></img>", RmlWysiwygComponent.DefaultImage))
                //new WysiwygDragDropItem("Link", "Editor/LinksIcon", "<a onclick=\"None\">Link</a>"),
                //new WysiwygDragDropItem("Button", "Editor/AddButtonIcon", "<input type=\"submit\" onclick=\"None\">Button</input>"),
                //new WysiwygDragDropItem("Separator", CommonResources.NoIcon, "<x-separator/>"),
                //new WysiwygDragDropItem("Two Columns", CommonResources.NoIcon, "<div class=\"TwoColumn\"><div class=\"Column\"><p>Column 1 text goes here.</p></div><div class=\"Column\"><p>Column 2 text goes here.</p></div></div>"),
                //new WysiwygDragDropItem("Heading and Paragraph", CommonResources.NoIcon, "<h1>Heading For Paragraph.</h1><p>Paragraph for heading.</p>", "div"),
                //new WysiwygDragDropItem("Left Image and Paragraph", CommonResources.NoIcon, String.Format("<div class=\"ImageParagraphLeft\"><img src=\"{0}\" /><p>Add paragraph text here.</p></div>", RmlWysiwygComponent.DefaultImage)),
                //new WysiwygDragDropItem("Right Image and Paragraph", CommonResources.NoIcon, String.Format("<div class=\"ImageParagraphRight\"><img src=\"{0}\" /><p>Add paragraph text here.</p></div>", RmlWysiwygComponent.DefaultImage))
                );
            htmlDragDrop.Dragging += (item, position) =>
                {
                    rmlComponent.setPreviewElement(position, item.Markup, item.PreviewTagType);
                };
            htmlDragDrop.DragEnded += (item, position) =>
                {
                    rmlComponent.insertRml(item.Markup, position);
                };
            htmlDragDrop.ItemActivated += (item) =>
                {
                    rmlComponent.insertRml(item.Markup);
                };
            htmlDragDrop.ViewLocation = ViewLocations.Left;
            htmlDragDrop.IsWindow = true;
            mvcContext.Views.add(htmlDragDrop);

            taskbar = new SlideTaskbarView("InfoBar", slideName);
            taskbar.addTask(new CallbackTask("Save", "Save", "FileToolstrip/Save", "", 0, true, item =>
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
            taskbar.addTask(new CallbackTask("AddSlide", "Add Slide", "Lecture.Icon.AddSlide", "Edit", 0, true, item =>
            {
                AddItemDialog.AddItem(editorController.ItemTemplates, editorController.createItem);
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
            taskbar.addTask(new CallbackTask("PresentFromBeginning", "Present From Beginning", "Lecture.Icon.PresetBeginning", "Edit", 0, true, item =>
            {
                editorController.runSlideshow(0);
            }));
            mvcContext.Views.add(taskbar);

            mvcContext.Controllers.add(new MvcController("HtmlDragDrop",
                new RunCommandsAction("Show",
                    new ShowViewIfNotOpenCommand("HtmlDragDrop")),
                new RunCommandsAction("Close",
                    new CloseViewCommand())
                    ));

            setupScene = new RunCommandsAction("SetupScene");
            slide.populateCommand(setupScene);

            mvcContext.Controllers.add(new MvcController("Editor",
                setupScene,
                new RunCommandsAction("Show",
                    new ShowViewCommand("RmlView"),
                    new ShowViewCommand("InfoBar"),
                    new RunActionCommand("Editor/SetupScene")
                    ),
                new RunCommandsAction("Close", new CloseAllViewsCommand())
                ));

            mvcContext.Controllers.add(new MvcController("Common",
                new RunCommandsAction("Start", new RunActionCommand("HtmlDragDrop/Show"), new RunActionCommand("Editor/Show")),
                new CallbackAction("Focus", context =>
                {
                    GlobalContextEventHandler.setEventContext(eventContext);
                    if (Focus != null)
                    {
                        Focus.Invoke(this);
                    }
                }),
                new CallbackAction("Blur", context =>
                {
                    commitText();
                    GlobalContextEventHandler.disableEventContext(eventContext);
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

        public void setWysiwygRml(String rml, bool keepScrollPosition)
        {
            if (rmlComponent != null)
            {
                rmlComponent.cancelAndHideEditor();
                rmlComponent.setRml(rml, keepScrollPosition, true);
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

        public string CurrentText
        {
            get
            {
                return rmlComponent.CurrentRml;
            }
        }

        private void saveAll()
        {
            commitText();
            slideEditorController.save();
        }

        private void commitText()
        {
            if (rmlComponent != null)
            {
                rmlComponent.aboutToSaveRml();
                if (rmlComponent.ChangesMade)
                {
                    updateThumbnail();
                }
            }
        }

        private const int slideWidth = SlideImageManager.ThumbWidth / 3;
        private const int sceneWidth = SlideImageManager.ThumbWidth - slideWidth;

        public void updateThumbnail()
        {
            if (slideEditorController.ResourceProvider != null && rmlComponent != null)
            {
                ImageRendererProperties imageProperties = new ImageRendererProperties();
                imageProperties.Width = sceneWidth;
                imageProperties.Height = SlideImageManager.ThumbHeight;
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

                Bitmap thumb = slideEditorController.SlideImageManager.createThumbBitmap(slide);
                using (Graphics g = Graphics.FromImage(thumb))
                {
                    rmlComponent.writeToGraphics(g, new Rectangle(0, 0, slideWidth, SlideImageManager.ThumbHeight));
                    using (Bitmap sceneThumb = imageRenderer.renderImage(imageProperties))
                    {
                        g.DrawImage(sceneThumb, slideWidth, 0);
                    }

                    slideEditorController.SlideImageManager.thumbnailUpdated(slide);
                }
            }
        }
    }
}
