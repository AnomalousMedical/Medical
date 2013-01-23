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

namespace Lecture
{
    class SlideEditorContext
    {
        public event Action<SlideEditorContext> Focus;
        public event Action<SlideEditorContext> Blur;
        public event Action<Slide, Bitmap> ThumbnailUpdated;

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
        private EditorController editorController;
        private ImageRenderer imageRenderer;

        public SlideEditorContext(MedicalRmlSlide slide, EditorController editorController, EditorUICallback uiCallback, UndoRedoBuffer undoBuffer, ImageRenderer imageRenderer, Action<String> wysiwygUndoCallback)
        {
            this.slide = slide;
            this.uiCallback = uiCallback;
            this.editorController = editorController;
            this.undoBuffer = undoBuffer;
            this.imageRenderer = imageRenderer;

            mvcContext = new AnomalousMvcContext();
            mvcContext.StartupAction = "Common/Start";
            mvcContext.FocusAction = "Common/Focus";
            mvcContext.BlurAction = "Common/Blur";
            mvcContext.SuspendAction = "Common/Suspended";
            mvcContext.ResumeAction = "Common/Resumed";

            //ExpandingGenericEditorView genericEditor = new ExpandingGenericEditorView("TimelinePropertiesEditor", showTypeController.CurrentObject.getEditInterface());
            //genericEditor.IsWindow = true;
            //genericEditor.ViewLocation = ViewLocations.Left;
            //mvcContext.Views.add(genericEditor);
            
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
                new WysiwygDragDropItem("Image", "Editor/ImageIcon", String.Format("<img src=\"{0}\"></img>", RmlWysiwygComponent.DefaultImage)),
                new WysiwygDragDropItem("Link", "Editor/LinksIcon", "<a onclick=\"None\">Link</a>"),
                new WysiwygDragDropItem("Button", "Editor/AddButtonIcon", "<input type=\"submit\" onclick=\"None\">Button</input>"),
                new WysiwygDragDropItem("Separator", CommonResources.NoIcon, "<x-separator/>"),
                new WysiwygDragDropItem("Two Columns", CommonResources.NoIcon, "<div class=\"TwoColumn\"><div class=\"Column\"><p>Column 1 text goes here.</p></div><div class=\"Column\"><p>Column 2 text goes here.</p></div></div>"),
                new WysiwygDragDropItem("Heading and Paragraph", CommonResources.NoIcon, "<h1>Heading For Paragraph.</h1><p>Paragraph for heading.</p>", "div"),
                new WysiwygDragDropItem("Left Image and Paragraph", CommonResources.NoIcon, String.Format("<div class=\"ImageParagraphLeft\"><img src=\"{0}\" /><p>Add paragraph text here.</p></div>", RmlWysiwygComponent.DefaultImage)),
                new WysiwygDragDropItem("Right Image and Paragraph", CommonResources.NoIcon, String.Format("<div class=\"ImageParagraphRight\"><img src=\"{0}\" /><p>Add paragraph text here.</p></div>", RmlWysiwygComponent.DefaultImage))
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

            //EditorTaskbarView taskbar = new EditorTaskbarView("InfoBar", "NOT DEFINED", "Editor/Close");
            //taskbar.addTask(new CallbackTask("SaveAll", "Save All", "Editor/SaveAllIcon", "", 0, true, item =>
            //{
            //    saveAll();
            //}));
            //taskbar.addTask(new RunMvcContextActionTask("Save", "Save Rml File", "FileToolstrip/Save", "File", "Editor/Save", mvcContext));
            //taskbar.addTask(new RunMvcContextActionTask("Undo", "Undo", CommonResources.NoIcon, "Edit", "Editor/Undo", mvcContext));
            //taskbar.addTask(new RunMvcContextActionTask("Redo", "Redo", CommonResources.NoIcon, "Edit", "Editor/Redo", mvcContext));
            //taskbar.addTask(new RunMvcContextActionTask("Cut", "Cut", "Editor/CutIcon", "Edit", "Editor/Cut", mvcContext));
            //taskbar.addTask(new RunMvcContextActionTask("Copy", "Copy", "Editor/CopyIcon", "Edit", "Editor/Copy", mvcContext));
            //taskbar.addTask(new RunMvcContextActionTask("Paste", "Paste", "Editor/PasteIcon", "Edit", "Editor/Paste", mvcContext));
            //taskbar.addTask(new RunMvcContextActionTask("SelectAll", "Select All", "Editor/SelectAllIcon", "Edit", "Editor/SelectAll", mvcContext));
            //taskbar.addTask(new RunMvcContextActionTask("RmlEditor", "Edit Rml", RmlTypeController.Icon, "Edit", "RmlTextEditor/Show", mvcContext));
            //mvcContext.Views.add(taskbar);

            mvcContext.Controllers.add(new MvcController("HtmlDragDrop",
                new RunCommandsAction("Show",
                    new ShowViewIfNotOpenCommand("HtmlDragDrop")),
                new RunCommandsAction("Close",
                    new CloseViewCommand())
                    ));

            mvcContext.Controllers.add(new MvcController("Editor",
                new RunCommandsAction("Show",
                    new ShowViewCommand("RmlView")
                    //,new ShowViewCommand("InfoBar")
                    ),
                new RunCommandsAction("Close", new CloseAllViewsCommand()),
                new CallbackAction("Save", context =>
                    {
                        saveAll();
                    }),
                new CallbackAction("Cut", context =>
                    {
                        
                    }),
                new CallbackAction("Copy", context =>
                    {
                        
                    }),
                new CallbackAction("Paste", context =>
                    {
                        
                    }),
                new CallbackAction("SelectAll", context =>
                    {
                        
                    }),
                new CallbackAction("Undo", context =>
                    {
                        undoBuffer.undo();
                    }),
                new CallbackAction("Redo", context =>
                    {
                        undoBuffer.execute();
                    })
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
                rmlComponent.setRml(rml, keepScrollPosition);
            }
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
            editorController.saveAllCachedResources();
        }

        private void commitText()
        {
            if (rmlComponent != null)
            {
                rmlComponent.aboutToSaveRml();
                updateThumbnail();
            }
        }

        private const int thumbWidth = 183;
        private const int thumbHeight = 101;
        private const int slideWidth = thumbWidth / 3;
        private const int sceneWidth = thumbWidth - slideWidth;

        private void updateThumbnail()
        {
            if (editorController.ResourceProvider != null)
            {
                ImageRendererProperties imageProperties = new ImageRendererProperties();
                imageProperties.Width = sceneWidth;
                imageProperties.Height = thumbHeight;
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

                using (Bitmap thumb = new Bitmap(thumbWidth, thumbHeight))
                {
                    using (Graphics g = Graphics.FromImage(thumb))
                    {
                        rmlComponent.writeToGraphics(g, new Rectangle(0, 0, slideWidth, thumbHeight));
                        using (Bitmap sceneThumb = imageRenderer.renderImage(imageProperties))
                        {
                            g.DrawImage(sceneThumb, slideWidth, 0);
                        }

                        if (ThumbnailUpdated != null)
                        {
                            ThumbnailUpdated.Invoke(slide, thumb);
                        }
                    }
                }
            }
        }
    }
}
