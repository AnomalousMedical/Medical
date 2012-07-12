using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Presentation;
using Medical.Controller.AnomalousMvc;
using Medical.Platform;
using Medical.GUI;
using Engine.Platform;

namespace Medical
{
    class PresentationEditorContext
    {
        public event Action<PresentationEditorContext> Focused;
        public event Action<PresentationEditorContext> Blured;

        enum Events
        {
            Save
        }

        private String currentFile;
        private AnomalousMvcContext mvcContext;
        private EventContext eventContext;
        private PresentationTypeController presentationTypeController;

        private PresentationIndex presentation;

        public PresentationEditorContext(PresentationIndex presentation, String file, PresentationTypeController presentationTypeController)
        {
            this.presentationTypeController = presentationTypeController;
            this.currentFile = file;
            this.presentation = presentation;

            mvcContext = new AnomalousMvcContext();
            mvcContext.StartupAction = "Common/Start";
            mvcContext.FocusAction = "Common/Focus";
            mvcContext.BlurAction = "Common/Blur";
            mvcContext.SuspendAction = "Common/Suspended";
            mvcContext.ResumeAction = "Common/Resumed";

            mvcContext.Models.add(new EditMenuManager());

            SlideIndexView slideIndexView = new SlideIndexView("SlideIndex", presentation);
            slideIndexView.ViewLocation = ViewLocations.Right;
            slideIndexView.IsWindow = true;
            mvcContext.Views.add(slideIndexView);

            EditorTaskbarView taskbar = new EditorTaskbarView("InfoBar", currentFile, "Editor/Close");
            taskbar.addTask(new RunMvcContextActionTask("Save", "Save Rml File", "FileToolstrip/Save", "File", "Editor/Save", mvcContext));
            taskbar.addTask(new RunMvcContextActionTask("AddSlide", "Add Slide", "StandaloneIcons/NoIcon", "File", "Editor/AddSlide", mvcContext));
            taskbar.addTask(new RunMvcContextActionTask("Preview", "Preview", "StandaloneIcons/NoIcon", "File", "Editor/Preview", mvcContext));
            mvcContext.Views.add(taskbar);

            mvcContext.Controllers.add(new MvcController("Editor", 
                new RunCommandsAction("Show",
                    new ShowViewCommand("SlideIndex"),
                    new ShowViewCommand("InfoBar")),
                new RunCommandsAction("Close", 
                    new CloseAllViewsCommand()),
                new CallbackAction("Save", context =>
                {
                    save();
                }),
                new CallbackAction("AddSlide", context =>
                {
                    SlideEntry slideEntry = new SlideEntry();
                    presentation.addEntry(slideEntry);
                    slideEntry.createFile(presentationTypeController.EditorController.ResourceProvider);
                }),
                new CallbackAction("Preview", context =>
                {
                    presentationTypeController.previewPresentation(presentation);
                })
                ));

            mvcContext.Controllers.add(new MvcController("Common", 
                new RunCommandsAction("Start", new RunActionCommand("Editor/Show")),
                new CallbackAction("Focus", context =>
                {
                    GlobalContextEventHandler.setEventContext(eventContext);
                    if (Focused != null)
                    {
                        Focused.Invoke(this);
                    }
                }),
                new CallbackAction("Blur", context =>
                {
                    GlobalContextEventHandler.disableEventContext(eventContext);
                    if (Blured != null)
                    {
                        Blured.Invoke(this);
                    }
                }),
                new RunCommandsAction("Suspended", new SaveViewLayoutCommand()),
                new RunCommandsAction("Resumed", new RestoreViewLayoutCommand())));

            eventContext = new EventContext();
            eventContext.addEvent(new MessageEvent(Events.Save, 
                frameUp: eventManager =>
                {
                    save();
                }, 
                keys: new KeyboardButtonCode[] { KeyboardButtonCode.KC_LCONTROL, KeyboardButtonCode.KC_S }));
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

        private void save()
        {
            presentationTypeController.saveFile(presentation, currentFile);
        }
    }
}
