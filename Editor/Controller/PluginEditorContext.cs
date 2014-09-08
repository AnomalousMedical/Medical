using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI;
using Medical.Platform;
using Medical.Controller.AnomalousMvc;
using Engine.Platform;
using Medical.Controller;
using System.IO;

namespace Medical
{
    class PluginEditorContext
    {
        public event Action<PluginEditorContext> Focus;
        public event Action<PluginEditorContext> Blur;

        private String currentFile;
        private AnomalousMvcContext mvcContext;
        private EventContext eventContext;
        private PluginTypeController pluginTypeController;
        private EditorController editorController;
        private StandaloneController standaloneController;

        private DDAtlasPlugin plugin;

        public PluginEditorContext(DDAtlasPlugin plugin, String file, PluginTypeController pluginTypeController, EditorController editorController, MedicalUICallback uiCallback, StandaloneController standaloneController)
        {
            this.pluginTypeController = pluginTypeController;
            this.currentFile = file;
            this.plugin = plugin;
            this.editorController = editorController;
            this.standaloneController = standaloneController;

            mvcContext = new AnomalousMvcContext();
            mvcContext.StartupAction = "Common/Start";
            mvcContext.FocusAction = "Common/Focus";
            mvcContext.BlurAction = "Common/Blur";
            mvcContext.SuspendAction = "Common/Suspended";
            mvcContext.ResumeAction = "Common/Resumed";

            mvcContext.Models.add(new EditMenuManager());

            GenericPropertiesFormView genericPropertiesView = new GenericPropertiesFormView("MvcContext", plugin.EditInterface, editorController, uiCallback, true);
            genericPropertiesView.ElementName = new MDILayoutElementName(GUILocationNames.MDI, DockLocation.Left);
            mvcContext.Views.add(genericPropertiesView);

            EditorTaskbarView taskbar = new EditorTaskbarView("InfoBar", currentFile, "Editor/Close");
            taskbar.addTask(new CallbackTask("SaveAll", "Save All", "Editor/SaveAllIcon", "", 0, true, item =>
            {
                saveAll();
            }));
            taskbar.addTask(new RunMvcContextActionTask("Save", "Save Plugin Definition File", "CommonToolstrip/Save", "File", "Editor/Save", mvcContext));
            taskbar.addTask(new CallbackTask("FindDependencies", "Find Dependencies", "EditorFileIcon/.ddp", "", item =>
            {
                findDependencies();
            }));
            mvcContext.Views.add(taskbar);

            mvcContext.Controllers.add(new MvcController("Editor", 
                new RunCommandsAction("Show",
                    new ShowViewCommand("MvcContext"),
                    new ShowViewCommand("InfoBar")),
                new RunCommandsAction("Close", new CloseAllViewsCommand()),
                new CallbackAction("Save", context =>
                    {
                        save();
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
            ButtonEvent saveEvent = new ButtonEvent(EventLayers.Gui);
            saveEvent.addButton(KeyboardButtonCode.KC_LCONTROL);
            saveEvent.addButton(KeyboardButtonCode.KC_S);
            saveEvent.FirstFrameUpEvent += eventManager =>
            {
                saveAll();
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

        private void saveAll()
        {
            pluginTypeController.EditorController.saveAllCachedResources();
        }

        private void save()
        {
            pluginTypeController.saveFile(plugin, currentFile);
        }

        private void findDependencies()
        {
            CleanupInfo resourceInfo = new CleanupInfo();
            resourceInfo.defineObjectClass(ShowPropAction.PropClass);
            foreach(String file in editorController.ResourceProvider.listFiles("*.tl", "", true))
            {
                Timeline timeline;
                using(Stream fileStream = editorController.ResourceProvider.openFile(file))
                {
                    timeline = SharedXmlSaver.Load<Timeline>(fileStream);
                }
                timeline.cleanup(resourceInfo);
            }

            plugin.setDependencyIds(resourceInfo.getObjects<String>(ShowPropAction.PropClass)
                    .Select(n => standaloneController.PropFactory.getDependencyIdForProp(n))
                    .Where(id => id.HasValue)
                    .Select(id => id.Value));
        }
    }
}
