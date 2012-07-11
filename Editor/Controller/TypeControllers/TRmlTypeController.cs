using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI;
using System.IO;
using MyGUIPlugin;
using libRocketPlugin;

namespace Medical
{
    class TRmlTypeController : TextTypeController
    {
        private GUIManager guiManager;
        public const String Icon = "EditorFileIcon/.trml";
        private TRmlEditorContext trmlContext;
        private RmlTypeController rmlTypeController;

        public TRmlTypeController(EditorController editorController, GUIManager guiManager, RmlTypeController rmlTypeController)
            : base(".trml", editorController)
        {
            editorController.ProjectChanged += new EditorControllerEvent(editorController_ProjectChanged);
            this.guiManager = guiManager;
            this.rmlTypeController = rmlTypeController;
        }

        public override void openFile(string file)
        {
            if (!EditorController.ResourceProvider.exists(file))
            {
                createNewRcssFile(file);
            }

            trmlContext = new TRmlEditorContext(file, rmlTypeController.LastRmlFile, this);
            trmlContext.Focus += new Action<TRmlEditorContext>(trmlContext_Focus);
            trmlContext.Blur += new Action<TRmlEditorContext>(rcssContext_Blur);
            EditorController.runEditorContext(trmlContext.MvcContext);
        }

        public void saveFile(String rcss, String file)
        {
            saveText(file, rcss);
            Factory.ClearStyleSheetCache();
            EditorController.NotificationManager.showNotification(String.Format("{0} saved.", file), Icon, 2);
        }

        public override void addCreationMethod(ContextMenu contextMenu, string path, bool isDirectory, bool isTopLevel)
        {
            contextMenu.add(new ContextMenuItem("Create Template RML File", path, delegate(ContextMenuItem item)
            {
                InputBox.GetInput("Template RML File Name", "Enter a name for the Template RML file.", true, delegate(String result, ref String errorMessage)
                {
                    String filePath = Path.Combine(path, result);
                    filePath = Path.ChangeExtension(filePath, ".trml");
                    if (EditorController.ResourceProvider.exists(filePath))
                    {
                        MessageBox.show(String.Format("Are you sure you want to override {0}?", filePath), "Override", MessageBoxStyle.IconQuest | MessageBoxStyle.Yes | MessageBoxStyle.No, delegate(MessageBoxStyle overrideResult)
                        {
                            if (overrideResult == MessageBoxStyle.Yes)
                            {
                                createNewRcssFile(filePath);
                            }
                        });
                    }
                    else
                    {
                        createNewRcssFile(filePath);
                    }
                    return true;
                });
            }));
        }

        void createNewRcssFile(String filePath)
        {
            creatingNewFile(filePath);
            saveText(filePath, DefaultMasterPage);
            openFile(filePath);
        }

        void trmlContext_Focus(TRmlEditorContext obj)
        {
            trmlContext = obj;
        }

        void rcssContext_Blur(TRmlEditorContext obj)
        {
            updateCachedText(obj.CurrentFile, obj.CurrentText);
            if (trmlContext == obj)
            {
                trmlContext = null;
            }
        }

        void editorController_ProjectChanged(EditorController editorController)
        {
            closeCurrentCachedResource();
            if (trmlContext != null)
            {
                trmlContext.close();
            }
        }

        public const String DefaultMasterPage = @"<template name=""MasterTemplate"" content=""Content"">
  <head>
    <link type=""text/rcss"" href=""/libRocketPlugin.Resources.rkt.rcss""/>
    <link type=""text/rcss"" href=""/libRocketPlugin.Resources.Anomalous.rcss""/>
  </head>
  <body>
    <div id=""Content"" class=""ScrollArea"">
    </div>
  </body>
</template>";
    }
}
