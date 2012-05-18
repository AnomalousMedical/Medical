using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI;
using MyGUIPlugin;

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
                editor.open(false);
            }
            editorController.ExtensionActions = extensionActions;
            editor.bringToFront();
        }

        //public void createNewPlugin()
        //{
        //    currentDefinition = new DDAtlasPlugin();
        //    currentDefinitionChanged(null);
        //}

        //public void loadPlugin()
        //{
        //    using (FileOpenDialog fileDialog = new FileOpenDialog(MainWindow.Instance, "Open a plugin definition.", EditorConfig.TimelineProjectDirectory, "", PLUGIN_WILDCARD, false))
        //    {
        //        if (fileDialog.showModal() == NativeDialogResult.OK)
        //        {
        //            loadPlugin(fileDialog.Path);
        //        }
        //    }
        //}

        //public void loadPlugin(String file)
        //{
        //    try
        //    {
        //        using (Stream pluginStream = File.Open(file, FileMode.Open, FileAccess.Read))
        //        {
        //            DDAtlasPlugin loadedPlugin = pluginManager.loadPlugin(pluginStream);
        //            if (loadedPlugin != null)
        //            {
        //                currentDefinition = loadedPlugin;
        //                currentDefinitionChanged(file);
        //            }
        //            else
        //            {
        //                MessageBox.show("Load error", "There was an error loading this plugin.", MessageBoxStyle.Ok | MessageBoxStyle.IconError);
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        MessageBox.show("Load error", "Exception loading plugin:\n." + e.Message, MessageBoxStyle.Ok | MessageBoxStyle.IconError);
        //    }
        //}

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
