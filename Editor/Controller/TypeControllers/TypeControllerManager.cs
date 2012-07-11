﻿using System;
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
        //Editor contexts
        private MvcEditorContext mvcEditorContext;
        private MovementSequenceEditorContext movementSequenceEditorContext;
        private PluginEditorContext pluginEditorContext;
        private PresentationEditorContext presentationEditorContext;


        private PropEditController propEditController;

        public TypeControllerManager(StandaloneController standaloneController, EditorPlugin plugin)
        {
            propEditController = plugin.PropEditController;
            EditorController editorController = plugin.EditorController;
            editorController.ProjectChanged += new EditorControllerEvent(editorController_ProjectChanged);
            GUIManager guiManager = standaloneController.GUIManager;

            RmlTypeController rmlTypeController = new RmlTypeController(editorController, guiManager, plugin.UICallback);
            editorController.addTypeController(rmlTypeController);
            editorController.addTypeController(new RcssTypeController(editorController, guiManager, rmlTypeController));

            //MVC Type Controller
            MvcTypeController mvcTypeController = new MvcTypeController(editorController);
            mvcTypeController.ItemOpened += (file, editingMvcContex) =>
                {
                    mvcEditorContext = new MvcEditorContext(editingMvcContex, file, mvcTypeController);
                    plugin.UICallback.CurrentEditingMvcContext = editingMvcContex;
                    mvcEditorContext.Focused += (obj) =>
                        {
                            mvcEditorContext = obj;
                        };
                    mvcEditorContext.Blured += (obj) =>
                        {
                            if (mvcEditorContext == obj)
                            {
                                mvcEditorContext = null;
                            }
                        };
                    editorController.runEditorContext(mvcEditorContext.MvcContext);
                };
            editorController.addTypeController(mvcTypeController);
            
            //Plugin type controller
            PluginTypeController pluginTypeController = new PluginTypeController(editorController);
            pluginTypeController.ItemOpened += (file, ddPlugin) =>
                {
                    pluginEditorContext = new PluginEditorContext(ddPlugin, file, pluginTypeController);
                    pluginEditorContext.Focus += obj =>
                        {
                            pluginEditorContext = obj;
                        };
                    pluginEditorContext.Blur += obj =>
                        {
                            if (pluginEditorContext == obj)
                            {
                                pluginEditorContext = null;
                            }
                        };
                    editorController.runEditorContext(pluginEditorContext.MvcContext);
                };
            editorController.addTypeController(pluginTypeController);

            //Movement Sequence type controller
            MovementSequenceTypeController movementSequenceTypeController = new MovementSequenceTypeController(editorController);
            movementSequenceTypeController.ItemOpened += (file, movementSequence) =>
                {
                    movementSequenceEditorContext = new MovementSequenceEditorContext(movementSequence, file, movementSequenceTypeController);
                    movementSequenceEditorContext.Focus += obj =>
                        {
                            movementSequenceEditorContext = obj;
                        };
                    movementSequenceEditorContext.Blur += obj =>
                        {
                            if (movementSequenceEditorContext == obj)
                            {
                                movementSequenceEditorContext = null;
                            }
                        };
                    editorController.runEditorContext(movementSequenceEditorContext.MvcContext);
                };
            editorController.addTypeController(movementSequenceTypeController);

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

            //Presentation type controller
            PresentationTypeController presentationTypeController = new PresentationTypeController(editorController, standaloneController);
            presentationTypeController.ItemOpened += (file, presentation) =>
            {
                presentationEditorContext = new PresentationEditorContext(presentation, file, presentationTypeController);
                presentationEditorContext.Focused += obj =>
                    {
                        presentationEditorContext = obj;
                    };
                presentationEditorContext.Blured += obj =>
                    {
                        if (presentationEditorContext == obj)
                        {
                            presentationEditorContext = null;
                        }
                    };
                editorController.runEditorContext(presentationEditorContext.MvcContext);
            };
            editorController.addTypeController(presentationTypeController);
        }

        void editorController_ProjectChanged(EditorController editorController)
        {
            if (mvcEditorContext != null)
            {
                mvcEditorContext.close();
            }
            if (movementSequenceEditorContext != null)
            {
                movementSequenceEditorContext.close();
            }
            if (pluginEditorContext != null)
            {
                pluginEditorContext.close();
            }
            if (presentationEditorContext != null)
            {
                presentationEditorContext.close();
            }

            if (editorController.ResourceProvider != null)
            {
                //Try to open a default mvc context
                String mvcFile = "MvcContext.mvc";
                if (editorController.ResourceProvider.exists(mvcFile))
                {
                    editorController.openFile(mvcFile);
                }
                else
                {
                    String[] files = editorController.ResourceProvider.listFiles("*.mvc", "", true);
                    if (files.Length > 0)
                    {
                        editorController.openFile(files[0]);
                    }
                }
            }
        }

        void timelineTypeController_TimelineChanged(TimelineTypeController typeController, Timeline timeline)
        {
            propEditController.removeAllOpenProps();
        }
    }
}
