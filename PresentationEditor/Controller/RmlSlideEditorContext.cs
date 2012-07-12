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

namespace PresentationEditor
{
    class RmlSlideEditorContext
    {
        public event Action<RmlSlideEditorContext> Focus;
        public event Action<RmlSlideEditorContext> Blur;

        enum Events
        {
            Save
        }

        private RmlWysiwygComponent rmlComponent;
        private String currentFile;
        private AnomalousMvcContext mvcContext;
        private EventContext eventContext;
        private RmlTypeController rmlTypeController;
        private MedicalUICallback uiCallback;

        public RmlSlideEditorContext(String file, RmlTypeController rmlTypeController, MedicalUICallback uiCallback)
        {
            this.rmlTypeController = rmlTypeController;
            this.currentFile = file;
            this.uiCallback = uiCallback;

            rmlTypeController.loadText(file);

            mvcContext = new AnomalousMvcContext();
            mvcContext.StartupAction = "Common/Start";
            mvcContext.FocusAction = "Common/Focus";
            mvcContext.BlurAction = "Common/Blur";
            mvcContext.SuspendAction = "Common/Suspended";
            mvcContext.ResumeAction = "Common/Resumed";

            RmlWysiwygView rmlView = new RmlWysiwygView("RmlView", uiCallback, null);
            rmlView.ViewLocation = ViewLocations.Left;
            rmlView.IsWindow = true;
            rmlView.RmlFile = file;
            rmlView.ComponentCreated += (view, component) =>
            {
                rmlComponent = component;
                //rmlComponent.RmlEdited += rmlEditor =>
                //{
                    
                //};
            };
            mvcContext.Views.add(rmlView);

            //EditorTaskbarView taskbar = new EditorTaskbarView("InfoBar", currentFile, "Editor/Close");
            //taskbar.addTask(new RunMvcContextActionTask("Save", "Save Rml File", "FileToolstrip/Save", "File", "Editor/Save", mvcContext));
            //taskbar.addTask(new RunMvcContextActionTask("Cut", "Cut", "Editor/CutIcon", "Edit", "Editor/Cut", mvcContext));
            //taskbar.addTask(new RunMvcContextActionTask("Copy", "Copy", "Editor/CopyIcon", "Edit", "Editor/Copy", mvcContext));
            //taskbar.addTask(new RunMvcContextActionTask("Paste", "Paste", "Editor/PasteIcon", "Edit", "Editor/Paste", mvcContext));
            //taskbar.addTask(new RunMvcContextActionTask("SelectAll", "Select All", "Editor/SelectAllIcon", "Edit", "Editor/SelectAll", mvcContext));
            //taskbar.addTask(new RunMvcContextActionTask("Paragraph", "Paragraph", "Editor/ParagraphsIcon", "Edit", "Editor/Paragraph", mvcContext));
            //taskbar.addTask(new RunMvcContextActionTask("Header", "Header", "Editor/HeaderIcon", "Edit", "Editor/Header", mvcContext));
            //taskbar.addTask(new RunMvcContextActionTask("ActionLink", "Action Link", "Editor/LinksIcon", "Edit", "Editor/ActionLink", mvcContext));
            //taskbar.addTask(new RunMvcContextActionTask("Button", "Button", "Editor/AddButtonIcon", "Edit", "Editor/Button", mvcContext));
            //taskbar.addTask(new RunMvcContextActionTask("Image", "Image", "Editor/ImageIcon", "Edit", "Editor/Image", mvcContext));
            //mvcContext.Views.add(taskbar);

            mvcContext.Controllers.add(new MvcController("Editor",
                new RunCommandsAction("Show",
                    new ShowViewCommand("RmlView")
                    //new ShowViewCommand("RmlEditor"),
                    //new ShowViewCommand("InfoBar")
                    ),
                new RunCommandsAction("Close", new CloseAllViewsCommand()),
                new CallbackAction("Save", context =>
                    {
                        save();
                    }),
                new CallbackAction("Cut", context =>
                    {
                        //textEditorComponent.cut();
                    }),
                new CallbackAction("Copy", context =>
                    {
                        //textEditorComponent.copy();
                    }),
                new CallbackAction("Paste", context =>
                    {
                        //textEditorComponent.paste();
                    }),
                new CallbackAction("SelectAll", context =>
                    {
                        //textEditorComponent.selectAll();
                    }),
                new CallbackAction("Paragraph", context =>
                    {
                        rmlComponent.insertParagraph();
                    }),
                new CallbackAction("Header", context =>
                    {
                        rmlComponent.insertHeader1();
                    }),
                new CallbackAction("ActionLink", context =>
                    {
                        //BrowserWindow<String>.GetInput(uiCallback.createActionBrowser(), true, delegate(String result, ref string errorPrompt)
                        //{
                        //    rmlComponent.insertLink(result);
                        //    return true;
                        //});
                    }),
                new CallbackAction("Button", context =>
                    {
                        //BrowserWindow<String>.GetInput(uiCallback.createActionBrowser(), true, delegate(String result, ref string errorPrompt)
                        //{
                        //    //textEditorComponent.insertText(String.Format("<input type=\"submit\" onclick=\"{0}\">Empty Button</input>", result));
                        //    rmlComponent.insertButton(result);
                        //    return true;
                        //});
                    }),
                new CallbackAction("Image", context =>
                    {
                        //BrowserWindow<String>.GetInput(uiCallback.createFileBrowser("*.png", "Image Files"), true, delegate(String result, ref string errorPrompt)
                        //{
                        //    //textEditorComponent.insertText(String.Format("<input type=\"submit\" onclick=\"{0}\">Empty Button</input>", result));
                        //    rmlComponent.insertImage(result);
                        //    return true;
                        //});
                    })));

            mvcContext.Controllers.add(new MvcController("Common",
                new RunCommandsAction("Start", new RunActionCommand("Editor/Show")),
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
                save();
            };
            eventContext.addEvent(saveEvent);
        }

        public void close()
        {
            mvcContext.runAction("Editor/Close");
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

        public string CurrentFile
        {
            get
            {
                return currentFile;
            }
        }

        private void save()
        {
            if (rmlComponent != null)
            {
                rmlComponent.aboutToSaveRml();
            }
            if (rmlComponent != null)
            {
                rmlTypeController.saveFile(rmlComponent.CurrentRml, currentFile);
                if (rmlComponent != null)
                {
                    rmlComponent.reloadDocument(currentFile);
                }
            }
        }
    }
}
