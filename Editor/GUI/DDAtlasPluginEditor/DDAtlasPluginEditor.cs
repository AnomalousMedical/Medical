using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine.Editing;
using Engine.Saving.XMLSaver;
using System.Xml;
using System.IO;

namespace Medical.GUI
{
    public class DDAtlasPluginEditor : MDIDialog
    {
        public const String PLUGIN_WILDCARD = "Data Driven Plugin (*.ddp)|*.ddp;";

        private Tree tree;
        private EditInterfaceTreeView editTreeView;

        private ResizingTable table;
        private PropertiesTable propTable;

        private ObjectEditor objectEditor;

        private DDAtlasPlugin currentDefinition;
        private String currentFile = null;

        private AtlasPluginManager pluginManager;
        private EditorController editorController;
        private ExtensionActionCollection extensionActions = new ExtensionActionCollection();

        public DDAtlasPluginEditor(MedicalUICallback uiCallback, AtlasPluginManager pluginManager, EditorController editorController)
            : base("Medical.GUI.DDAtlasPluginEditor.DDAtlasPluginEditor.layout")
        {
            this.pluginManager = pluginManager;
            this.editorController = editorController;

            tree = new Tree((ScrollView)window.findWidget("TreeScroller"));
            editTreeView = new EditInterfaceTreeView(tree, uiCallback);

            table = new ResizingTable((ScrollView)window.findWidget("TableScroller"));
            propTable = new PropertiesTable(table);

            objectEditor = new ObjectEditor(editTreeView, propTable, uiCallback);

            extensionActions.Add(new ExtensionAction("Save Plugin", "File", savePlugin));
            extensionActions.Add(new ExtensionAction("Save Plugin As", "File", savePluginAs));

            createNewPlugin();

            this.Resized += new EventHandler(DDAtlasPluginEditor_Resized);

            window.RootKeyChangeFocus += new MyGUIEvent(window_RootKeyChangeFocus);
        }

        public override void Dispose()
        {
            objectEditor.Dispose();
            propTable.Dispose();
            table.Dispose();
            editTreeView.Dispose();
            tree.Dispose();
            base.Dispose();
        }

        public void createNewPlugin()
        {
            currentDefinition = new DDAtlasPlugin();
            currentDefinitionChanged(null);
        }

        public void loadPlugin()
        {
            using (FileOpenDialog fileDialog = new FileOpenDialog(MainWindow.Instance, "Open a plugin definition.", EditorConfig.TimelineProjectDirectory, "", PLUGIN_WILDCARD, false))
            {
                if (fileDialog.showModal() == NativeDialogResult.OK)
                {
                    loadPlugin(fileDialog.Path);
                }
            }
        }

        public void loadPlugin(String file)
        {
            try
            {
                using (Stream pluginStream = File.Open(file, FileMode.Open, FileAccess.Read))
                {
                    DDAtlasPlugin loadedPlugin = pluginManager.loadPlugin(pluginStream);
                    if (loadedPlugin != null)
                    {
                        currentDefinition = loadedPlugin;
                        currentDefinitionChanged(file);
                    }
                    else
                    {
                        MessageBox.show("Load error", "There was an error loading this plugin.", MessageBoxStyle.Ok | MessageBoxStyle.IconError);
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.show("Load error", "Exception loading plugin:\n." + e.Message, MessageBoxStyle.Ok | MessageBoxStyle.IconError);
            }
        }

        public void savePlugin()
        {
            if (currentFile != null)
            {
                using (XmlTextWriter xmlWriter = new XmlTextWriter(currentFile, Encoding.Default))
                {
                    xmlWriter.Formatting = Formatting.Indented;
                    EditorController.XmlSaver.saveObject(currentDefinition, xmlWriter);
                }
            }
            else
            {
                savePluginAs();
            }
        }

        public void savePluginAs()
        {
            using (FileSaveDialog fileDialog = new FileSaveDialog(MainWindow.Instance, "Save a plugin definition", EditorConfig.TimelineProjectDirectory, "", PLUGIN_WILDCARD))
            {
                if (fileDialog.showModal() == NativeDialogResult.OK)
                {
                    try
                    {
                        using (XmlTextWriter xmlWriter = new XmlTextWriter(fileDialog.Path, Encoding.Default))
                        {
                            xmlWriter.Formatting = Formatting.Indented;
                            EditorController.XmlSaver.saveObject(currentDefinition, xmlWriter);
                            fileChanged(fileDialog.Path);
                        }
                    }
                    catch (Exception e)
                    {
                        MessageBox.show("Load error", "Exception saving plugin:\n." + e.Message, MessageBoxStyle.Ok | MessageBoxStyle.IconError);
                    }
                }
            }
        }

        public void activateExtensionActions()
        {
            editorController.ExtensionActions = extensionActions;
        }

        void window_RootKeyChangeFocus(Widget source, EventArgs e)
        {
            RootFocusEventArgs rfae = (RootFocusEventArgs)e;
            if (rfae.Focus)
            {
                activateExtensionActions();
            }
        }

        void DDAtlasPluginEditor_Resized(object sender, EventArgs e)
        {
            tree.layout();
            table.layout();
        }

        private void currentDefinitionChanged(String file)
        {
            editTreeView.EditInterface = currentDefinition.EditInterface;
            fileChanged(file);
        }

        private void fileChanged(String file)
        {
            currentFile = file;
            if (currentFile != null)
            {
                window.Caption = String.Format("Plugin Editor - {0}", currentFile);
            }
            else
            {
                window.Caption = String.Format("Plugin Editor");
            }
        }
    }
}
