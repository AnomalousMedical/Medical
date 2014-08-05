using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller.AnomalousMvc;
using System.IO;
using Medical.GUI.AnomalousMvc;
using Medical.GUI;
using MyGUIPlugin;

namespace Medical
{
    public class TypeControllerManager
    {
        public event Action<EditorFilesystemWatcher> FilesystemWatcherCreated;
        public event Action<EditorFilesystemWatcher> FilesystemWatcherDisposing;

        //Editor contexts
        private MvcEditorContext mvcEditorContext;
        private MovementSequenceEditorContext movementSequenceEditorContext;
        private PluginEditorContext pluginEditorContext;
        private DependencyEditorContext dependencyEditorContext;
        private TimelineEditorContext timelineEditorContext;
        private RmlEditorContext rmlEditorContext;
        private RcssEditorContext rcssEditorContext;
        private TRmlEditorContext trmlEditorContext;
        private XmlEditorContext xmlEditorContext;
        private PropEditorContext propEditorContext;

        private PropEditController propEditController;
        private StandaloneController standaloneController;
        private EditorFilesystemWatcher editorFilesystemWatcher;

        public TypeControllerManager(StandaloneController standaloneController, EditorPlugin plugin)
        {
            this.standaloneController = standaloneController;
            propEditController = plugin.PropEditController;
            EditorController editorController = plugin.EditorController;
            editorController.ProjectChanged += editorController_ProjectChanged;
            editorController.ResourceProviderClosing += editorController_ResourceProviderClosing;
            editorController.ResourceProviderOpened += editorController_ResourceProviderOpened;

            //MVC Type Controller
            MvcTypeController mvcTypeController = new MvcTypeController(editorController);
            mvcTypeController.OpenEditor += (file, editingMvcContex) =>
            {
                mvcEditorContext = new MvcEditorContext(editingMvcContex, file, mvcTypeController, plugin.EditorController, plugin.UICallback);
                plugin.UICallback.CurrentEditingMvcContext = editingMvcContex;
                if (standaloneController.SharePluginController != null)
                {
                    CallbackTask cleanupBeforeShareTask = new CallbackTask("Lecture.SharePluginTask", standaloneController.SharePluginController.Name, standaloneController.SharePluginController.IconName, standaloneController.SharePluginController.Category, 0, false, (item) =>
                    {
                        MessageBox.show("Before sharing your Editor Project it will be saved. Do you wish to continue?", "Share Editor Project", MessageBoxStyle.IconQuest | MessageBoxStyle.Yes | MessageBoxStyle.No, (result) =>
                        {
                            if (result == MessageBoxStyle.Yes)
                            {
                                editorController.saveAllCachedResources();
                                standaloneController.SharePluginController.sharePlugin(editorController.ResourceProvider.BackingProvider, PluginCreationTool.EditorTools);
                            }
                        });
                    });
                    mvcEditorContext.addTask(cleanupBeforeShareTask);
                }
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
                    rmlEditorContext = new RmlEditorContext(file, rmlTypeController, mvcTypeController.CurrentObject, plugin.EditorController, plugin.UICallback);
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
                    pluginEditorContext = new PluginEditorContext(ddPlugin, file, pluginTypeController, plugin.EditorController, plugin.UICallback);
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

            //Dependency type controller
            DependencyTypeController dependencyTypeController = new DependencyTypeController(editorController);
            dependencyTypeController.OpenEditor += (file, ddDep) =>
                {
                    dependencyEditorContext = new DependencyEditorContext(ddDep, file, dependencyTypeController, plugin.EditorController, plugin.UICallback);
                    dependencyEditorContext.Focus += obj =>
                    {
                        dependencyEditorContext = obj;
                    };
                    dependencyEditorContext.Blur += obj =>
                    {
                        if (dependencyEditorContext == obj)
                        {
                            dependencyEditorContext = null;
                        }
                    };
                    editorController.runEditorContext(dependencyEditorContext.MvcContext);
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
                    timelineEditorContext = new TimelineEditorContext(timeline, file, timelineTypeController, propEditController, standaloneController.PropFactory, plugin.EditorController, plugin.UICallback, plugin.TimelineController);
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

            //Xml Type Controller
            XmlTypeController xmlTypeController = new XmlTypeController(editorController);
            xmlTypeController.OpenEditor += (file) =>
            {
                xmlEditorContext = new XmlEditorContext(file, rmlTypeController.LastRmlFile, xmlTypeController);
                xmlEditorContext.Focus += (obj) =>
                {
                    xmlEditorContext = obj;
                };
                xmlEditorContext.Blur += (obj) =>
                {
                    xmlTypeController.updateCachedText(obj.CurrentFile, obj.CurrentText);
                    if (xmlEditorContext == obj)
                    {
                        xmlEditorContext = null;
                    }
                };
                editorController.runEditorContext(xmlEditorContext.MvcContext);
            };

            //Prop type controller
            PropTypeController propTypeController = new PropTypeController(editorController);
            propTypeController.OpenEditor += (file, propDefinition) =>
            {
                propEditorContext = new PropEditorContext(propDefinition, file, propTypeController, plugin.EditorController, plugin.UICallback);
                propEditorContext.Focus += obj =>
                {
                    propEditorContext = obj;
                };
                propEditorContext.Blur += obj =>
                {
                    if (propEditorContext == obj)
                    {
                        propEditorContext = null;
                    }
                };
                editorController.runEditorContext(propEditorContext.MvcContext);
            };

            //Add item templates
            editorController.addItemTemplate(new EmptyViewItemTemplate(rmlTypeController, mvcTypeController));
            editorController.addItemTemplate(new ViewWithTimelineItemTemplate(rmlTypeController, mvcTypeController, timelineTypeController));

            //Add type controllers to editor controller, this also adds some item templates
            editorController.addTypeController(timelineTypeController);
            editorController.addTypeController(movementSequenceTypeController);
            editorController.addTypeController(rmlTypeController);
            editorController.addTypeController(trmlTypeController);
            editorController.addTypeController(rcssTypeController);
            editorController.addTypeController(mvcTypeController);
            editorController.addTypeController(pluginTypeController);
            editorController.addTypeController(dependencyTypeController);
            editorController.addTypeController(xmlTypeController);
            editorController.addTypeController(propTypeController);

            //Add any final item templates
            editorController.addItemTemplate(new PluginBrandingResourceItemTemplate());
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
            if(dependencyEditorContext != null)
            {
                dependencyEditorContext.close();
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
            if(xmlEditorContext != null)
            {
                xmlEditorContext.close();
            }
            if(propEditorContext != null)
            {
                propEditorContext.close();
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

        void editorController_ResourceProviderOpened(EditorResourceProvider obj)
        {
            editorFilesystemWatcher = new EditorFilesystemWatcher(obj);
            if (FilesystemWatcherCreated != null)
            {
                FilesystemWatcherCreated.Invoke(editorFilesystemWatcher);
            }
        }

        void editorController_ResourceProviderClosing(EditorResourceProvider obj)
        {
            if (FilesystemWatcherDisposing != null)
            {
                FilesystemWatcherDisposing.Invoke(editorFilesystemWatcher);
            }
            editorFilesystemWatcher.Dispose();
        }
    }
}
