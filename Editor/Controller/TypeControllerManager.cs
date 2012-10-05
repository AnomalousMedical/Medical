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
        //Editor contexts
        private MvcEditorContext mvcEditorContext;
        private MovementSequenceEditorContext movementSequenceEditorContext;
        private PluginEditorContext pluginEditorContext;
        private TimelineEditorContext timelineEditorContext;
        private RmlEditorContext rmlEditorContext;
        private RcssEditorContext rcssEditorContext;
        private TRmlEditorContext trmlEditorContext;

        private PropEditController propEditController;
        private StandaloneController standaloneController;

        public TypeControllerManager(StandaloneController standaloneController, EditorPlugin plugin)
        {
            this.standaloneController = standaloneController;
            propEditController = plugin.PropEditController;
            EditorController editorController = plugin.EditorController;
            editorController.ProjectChanged += editorController_ProjectChanged;

            //MVC Type Controller
            MvcTypeController mvcTypeController = new MvcTypeController(editorController);
            mvcTypeController.OpenEditor += (file, editingMvcContex) =>
            {
                mvcEditorContext = new MvcEditorContext(editingMvcContex, file, mvcTypeController, plugin.UICallback);
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

            //Rml type controller
            RmlTypeController rmlTypeController = new RmlTypeController(editorController);
            rmlTypeController.OpenEditor += (file) =>
                {
                    rmlEditorContext = new RmlEditorContext(file, rmlTypeController, plugin.UICallback, mvcTypeController.CurrentObject);
                    rmlEditorContext.Focus += (obj) =>
                        {
                            rmlEditorContext = obj;
                        };
                    rmlEditorContext.Blur += obj =>
                        {
                            rmlTypeController.updateCachedText(obj.CurrentFile, obj.CurrentText);
                            if (rmlEditorContext == obj)
                            {
                                rmlEditorContext = null;
                            }
                        };
                    editorController.runEditorContext(rmlEditorContext.MvcContext);
                };

            //Rcss Type Controller
            RcssTypeController rcssTypeController = new RcssTypeController(editorController);
            rcssTypeController.OpenEditor += (file) =>
                {
                    rcssEditorContext = new RcssEditorContext(file, rmlTypeController.LastRmlFile, rcssTypeController);
                    rcssEditorContext.Focus += (obj) =>
                        {
                            rcssEditorContext = obj;
                        };
                    rcssEditorContext.Blur += (obj) =>
                        {
                            rcssTypeController.updateCachedText(obj.CurrentFile, obj.CurrentText);
                            if (rcssEditorContext == obj)
                            {
                                rcssEditorContext = null;
                            }
                        };
                    editorController.runEditorContext(rcssEditorContext.MvcContext);
                };
            
            //Plugin type controller
            PluginTypeController pluginTypeController = new PluginTypeController(editorController);
            pluginTypeController.OpenEditor += (file, ddPlugin) =>
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

            //Movement Sequence type controller
            MovementSequenceTypeController movementSequenceTypeController = new MovementSequenceTypeController(editorController);
            movementSequenceTypeController.OpenEditor += (file, movementSequence) =>
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

            //TRML Type controller
            TRmlTypeController trmlTypeController = new TRmlTypeController(editorController);
            trmlTypeController.OpenEditor += (file) =>
            {
                trmlEditorContext = new TRmlEditorContext(file, rmlTypeController.LastRmlFile, trmlTypeController);
                trmlEditorContext.Focus += obj =>
                    {
                        trmlEditorContext = obj;
                    };
                trmlEditorContext.Blur += obj =>
                    {
                        trmlTypeController.updateCachedText(obj.CurrentFile, obj.CurrentText);
                        if (trmlEditorContext == obj)
                        {
                            trmlEditorContext = null;
                        }
                    };
                editorController.runEditorContext(trmlEditorContext.MvcContext);
            };

            //Timeline type controller
            TimelineTypeController timelineTypeController = new TimelineTypeController(editorController);
            timelineTypeController.OpenEditor += (file, timeline) =>
                {
                    propEditController.removeAllOpenProps();
                    timelineEditorContext = new TimelineEditorContext(timeline, file, timelineTypeController, propEditController);
                    timelineEditorContext.Focus += obj =>
                        {
                            timelineEditorContext = obj;
                        };
                    timelineEditorContext.Blur += obj =>
                        {
                            if (obj == timelineEditorContext)
                            {
                                timelineEditorContext = null;
                            }
                        };
                    editorController.runEditorContext(timelineEditorContext.MvcContext);
                };

            //Add item templates
            editorController.addItemTemplate(new EmptyViewItemTemplate(rmlTypeController, mvcTypeController));
            editorController.addItemTemplate(new ViewWithTimelineItemTemplate(rmlTypeController, mvcTypeController, timelineTypeController));

            //Add type controllers to editor controller
            editorController.addTypeController(timelineTypeController);
            editorController.addTypeController(movementSequenceTypeController);
            editorController.addTypeController(rmlTypeController);
            editorController.addTypeController(trmlTypeController);
            editorController.addTypeController(rcssTypeController);
            editorController.addTypeController(mvcTypeController);
            editorController.addTypeController(pluginTypeController);

            editorController.addItemTemplate(new PluginBrandingResourceItemTemplate());
        }

        void editorController_ProjectChanged(EditorController editorController, String defaultFile)
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
            if (timelineEditorContext != null)
            {
                timelineEditorContext.close();
            }
            if (rmlEditorContext != null)
            {
                rmlEditorContext.close();
            }
            if (rcssEditorContext != null)
            {
                rcssEditorContext.close();
            } 
            if (trmlEditorContext != null)
            {
                trmlEditorContext.close();
            }

            if (editorController.ResourceProvider != null)
            {
                standaloneController.DocumentController.addToRecentDocuments(editorController.ResourceProvider.BackingLocation);
                //Try to open a default mvc context
                String mvcFile = "MvcContext.mvc";
                if (editorController.ResourceProvider.exists(mvcFile))
                {
                    editorController.openEditor(mvcFile);
                }
                else
                {
                    IEnumerable<String> files = editorController.ResourceProvider.listFiles("*.mvc", "", true);
                    String firstMvcFile = files.FirstOrDefault();
                    if (firstMvcFile != null)
                    {
                        editorController.openEditor(firstMvcFile);
                    }
                }
            }
        }
    }
}
