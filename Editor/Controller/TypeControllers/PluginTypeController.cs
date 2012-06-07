using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI;
using MyGUIPlugin;
using System.IO;

namespace Medical
{
    class PluginTypeController : SaveableTypeController
    {
        public const String PLUGIN_WILDCARD = "Data Driven Plugin (*.ddp)|*.ddp;";

        private DDAtlasPluginEditor editor;
        private EditorController editorController;
        private ExtensionActionCollection extensionActions = new ExtensionActionCollection();
        private String currentFile;

        public PluginTypeController(DDAtlasPluginEditor editor, EditorController editorController)
            :base(".ddp", editorController)
        {
            this.editor = editor;
            editor.GotFocus += new EventHandler(editor_GotFocus);
            this.editorController = editorController;
            editorController.ProjectChanged += new EditorControllerEvent(editorController_ProjectChanged);

            extensionActions.Add(new ExtensionAction("Close Plugin", "File", close));
            extensionActions.Add(new ExtensionAction("Save Plugin", "File", savePlugin));
            extensionActions.Add(new ExtensionAction("Save Plugin As", "File", savePluginAs));
        }

        public override void openFile(string file)
        {
            currentFile = file;
            DDAtlasPlugin plugin = (DDAtlasPlugin)loadObject(file);
            editor.CurrentPlugin = plugin;
            editor.updateCaption(file);
            if (!editor.Visible)
            {
                editor.Visible = true;
            }
            editorController.ExtensionActions = extensionActions;
            editor.bringToFront();
        }

        public override void addCreationMethod(ContextMenu contextMenu, string path, bool isDirectory, bool isTopLevel)
        {
            if (isTopLevel)
            {
                contextMenu.add(new ContextMenuItem("Create Plugin Definition", path, delegate(ContextMenuItem item)
                {
                    String filePath = Path.Combine(item.UserObject.ToString(), "Plugin.ddp");
                    if (editorController.ResourceProvider.exists(filePath))
                    {
                        MessageBox.show(String.Format("Are you sure you want to override {0}?", filePath), "Override", MessageBoxStyle.IconQuest | MessageBoxStyle.Yes | MessageBoxStyle.No, delegate(MessageBoxStyle result)
                        {
                            if (result == MessageBoxStyle.Yes)
                            {
                                createNewPlugin(filePath);
                            }
                        });
                    }
                    else
                    {
                        createNewPlugin(filePath);
                    }
                }));
            }
        }

        private void createNewPlugin(String filePath)
        {
            DDAtlasPlugin newPlugin = new DDAtlasPlugin();
            saveObject(filePath, newPlugin);
            openFile(filePath);
        }

        void editor_GotFocus(object sender, EventArgs e)
        {
            editorController.ExtensionActions = extensionActions;
        }

        private void savePlugin()
        {
            if (currentFile != null)
            {
                saveObject(currentFile, editor.CurrentPlugin);
            }
            else
            {
                savePluginAs();
            }
        }

        private void savePluginAs()
        {
            using (FileSaveDialog fileDialog = new FileSaveDialog(MainWindow.Instance, "Save a plugin definition", "", "", PLUGIN_WILDCARD))
            {
                if (fileDialog.showModal() == NativeDialogResult.OK)
                {
                    try
                    {
                        currentFile = fileDialog.Path;
                        saveObject(currentFile, editor.CurrentPlugin);
                        editor.updateCaption(currentFile);
                    }
                    catch (Exception e)
                    {
                        MessageBox.show("Save error", "Exception saving plugin:\n." + e.Message, MessageBoxStyle.Ok | MessageBoxStyle.IconError);
                    }
                }
            }
        }

        void editorController_ProjectChanged(EditorController editorController)
        {
            close();
        }

        private void close()
        {
            editor.CurrentPlugin = null;
            editor.updateCaption(null);
            closeCurrentCachedResource();
        }
    }
}
