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
        public const String Icon = "EditorFileIcon/.trml";

        public TRmlTypeController(EditorController editorController)
            : base(".trml", editorController)
        {

        }

        public override void openFile(string file)
        {
            if (!EditorController.ResourceProvider.exists(file))
            {
                createNewRcssFile(file);
            }

            base.openFile(file);
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
