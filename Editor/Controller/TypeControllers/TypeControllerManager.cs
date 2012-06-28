using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller.AnomalousMvc;
using System.IO;
using Medical.GUI.AnomalousMvc;
using Medical.GUI;

namespace Medical
{
    class TypeControllerManager
    {
        private PropEditController propEditController;

        public TypeControllerManager(StandaloneController standaloneController, EditorPlugin plugin)
        {
            propEditController = plugin.PropEditController;
            EditorController editorController = plugin.EditorController;
            GUIManager guiManager = standaloneController.GUIManager;

            RmlTypeController rmlTypeController = new RmlTypeController(editorController, guiManager, plugin.UICallback);
            editorController.addTypeController(rmlTypeController);
            editorController.addTypeController(new RcssTypeController(editorController, guiManager, rmlTypeController));
            MvcTypeController mvcTypeController = new MvcTypeController(editorController, plugin.UICallback);
            editorController.addTypeController(mvcTypeController);
            editorController.addTypeController(new PluginTypeController(editorController));
            editorController.addTypeController(new MovementSequenceTypeController(editorController));
            editorController.addTypeController(new TRmlTypeController(editorController, guiManager, rmlTypeController));
            TimelineTypeController timelineTypeController = new TimelineTypeController(editorController, propEditController);
            timelineTypeController.TimelineChanged += new TimelineTypeEvent(timelineTypeController_TimelineChanged);
            editorController.addTypeController(timelineTypeController);
            rmlTypeController.FileCreated += (rmlCtrl, file) =>
            {
                AnomalousMvcContext mvcContext = mvcTypeController.CurrentObject;
                if (mvcContext != null)
                {
                    String name = file.Replace(Path.GetExtension(file), "");
                    if (!mvcContext.Views.hasItem(name))
                    {
                        RmlView view = new RmlView(name);
                        view.Buttons.add(new CloseButtonDefinition("Close", name + "/Close"));
                        mvcContext.Views.add(view);
                    }
                    if (!mvcContext.Controllers.hasItem(name))
                    {
                        MvcController controller = new MvcController(name);
                        RunCommandsAction show = new RunCommandsAction("Show");
                        show.addCommand(new ShowViewCommand(name));
                        controller.Actions.add(show);
                        RunCommandsAction close = new RunCommandsAction("Close");
                        close.addCommand(new CloseViewCommand());
                        controller.Actions.add(close);
                        mvcContext.Controllers.add(controller);
                    }
                }
            };
        }

        void timelineTypeController_TimelineChanged(TimelineTypeController typeController, Timeline timeline)
        {
            propEditController.removeAllOpenProps();
        }
    }
}
