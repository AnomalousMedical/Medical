﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Presentation;
using Medical.GUI;
using MyGUIPlugin;
using System.IO;
using Medical.Controller.AnomalousMvc;

namespace Medical
{
    class PresentationTypeController : SaveableTypeController<PresentationIndex>
    {
        private PresentationEditorContext editorContext;
        private StandaloneController standaloneController;
        private const String Icon = "EditorFileIcon/.amp";

        public PresentationTypeController(EditorController editorController, StandaloneController standaloneController)
            :base(".amp", editorController)
        {
            editorController.ProjectChanged += new EditorControllerEvent(editorController_ProjectChanged);
            this.standaloneController = standaloneController;
        }

        public void previewPresentation(PresentationIndex presentation)
        {
            AnomalousMvcContext presentationContext = presentation.buildMvcContext();
            standaloneController.TimelineController.setResourceProvider(EditorController.ResourceProvider);
            presentationContext.setResourceProvider(EditorController.ResourceProvider);
            presentationContext.RuntimeName = "Editor.PreviewMvcContext";
            standaloneController.MvcCore.startRunningContext(presentationContext);
        }

        public override void openFile(string file)
        {
            PresentationIndex presentation = loadObject(file);

            editorContext = new PresentationEditorContext(presentation, file, this);
            editorContext.Focused += new Action<PresentationEditorContext>(editorContext_Focus);
            editorContext.Blured += new Action<PresentationEditorContext>(editorContext_Blur);
            EditorController.runEditorContext(editorContext.MvcContext);
        }

        public void saveFile(PresentationIndex presentation, string file)
        {
            saveObject(file, presentation);
            EditorController.NotificationManager.showNotification(String.Format("{0} saved.", file), Icon, 2);
        }

        public override void addCreationMethod(ContextMenu contextMenu, string path, bool isDirectory, bool isTopLevel)
        {
            contextMenu.add(new ContextMenuItem("Create Presentation", path, delegate(ContextMenuItem item)
            {
                InputBox.GetInput("Presentation Name", "Enter a name for the Presentation.", true, delegate(String result, ref String errorMessage)
                {
                    String filePath = Path.Combine(path, result);
                    filePath = Path.ChangeExtension(filePath, ".amp");
                    if (EditorController.ResourceProvider.exists(filePath))
                    {
                        MessageBox.show(String.Format("Are you sure you want to override {0}?", filePath), "Override", MessageBoxStyle.IconQuest | MessageBoxStyle.Yes | MessageBoxStyle.No, delegate(MessageBoxStyle overrideResult)
                        {
                            if (overrideResult == MessageBoxStyle.Yes)
                            {
                                createNewPresentation(filePath);
                            }
                        });
                    }
                    else
                    {
                        createNewPresentation(filePath);
                    }
                    return true;
                });
            }));
        }

        private void createNewPresentation(String filePath)
        {
            PresentationIndex newPresentation = new PresentationIndex();
            creatingNewFile(filePath);
            saveObject(filePath, newPresentation);
            openFile(filePath);
        }

        void editorController_ProjectChanged(EditorController editorController)
        {
            close();
        }

        private void close()
        {
            if (editorContext != null)
            {
                editorContext.close();
            }
            closeCurrentCachedResource();
        }

        void editorContext_Focus(PresentationEditorContext obj)
        {
            editorContext = obj;
        }

        void editorContext_Blur(PresentationEditorContext obj)
        {
            if (editorContext == obj)
            {
                editorContext = null;
            }
        }
    }
}
