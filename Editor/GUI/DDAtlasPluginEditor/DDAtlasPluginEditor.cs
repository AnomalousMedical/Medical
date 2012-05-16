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
        private TimelineController mainTimelineController;

        private XmlSaver xmlSaver = new XmlSaver();
        private String currentFile = null;

        private AtlasPluginManager pluginManager;

        public DDAtlasPluginEditor(MedicalUICallback uiCallback, TimelineController mainTimelineController, AtlasPluginManager pluginManager)
            : base("Medical.GUI.DDAtlasPluginEditor.DDAtlasPluginEditor.layout")
        {
            this.mainTimelineController = mainTimelineController;
            this.pluginManager = pluginManager;

            tree = new Tree((ScrollView)window.findWidget("TreeScroller"));
            editTreeView = new EditInterfaceTreeView(tree, uiCallback);

            table = new ResizingTable((ScrollView)window.findWidget("TableScroller"));
            propTable = new PropertiesTable(table);

            objectEditor = new ObjectEditor(editTreeView, propTable, uiCallback);

            MenuBar menu = window.findWidget("MenuBar") as MenuBar;
            MenuItem fileMenu = menu.addItem("File", MenuItemType.Popup);
            MenuControl fileMenuCtrl = menu.createItemPopupMenuChild(fileMenu);
            fileMenuCtrl.ItemAccept += new MyGUIEvent(fileMenuCtrl_ItemAccept);
            fileMenuCtrl.addItem("New", MenuItemType.Normal, "New");
            fileMenuCtrl.addItem("Open", MenuItemType.Normal, "Open");
            fileMenuCtrl.addItem("Save", MenuItemType.Normal, "Save");
            fileMenuCtrl.addItem("Save As", MenuItemType.Normal, "Save As");

            createNewPlugin();

            this.Resized += new EventHandler(DDAtlasPluginEditor_Resized);
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
                    xmlSaver.saveObject(currentDefinition, xmlWriter);
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
                            xmlSaver.saveObject(currentDefinition, xmlWriter);
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

        void fileMenuCtrl_ItemAccept(Widget source, EventArgs e)
        {
            MenuCtrlAcceptEventArgs mcae = (MenuCtrlAcceptEventArgs)e;
            switch (mcae.Item.ItemId)
            {
                case "New":
                    createNewPlugin();
                    break;
                case "Open":
                    loadPlugin();
                    break;
                case "Save":
                    savePlugin();
                    break;
                case "Save As":
                    savePluginAs();
                    break;
            }
        }
    }
}
