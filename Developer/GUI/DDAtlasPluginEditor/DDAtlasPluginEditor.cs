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
    class DDAtlasPluginEditor : MDIDialog
    {
        public const String PLUGIN_WILDCARD = "Data Driven Plugin (*.ddp)|*.ddp;";

        private MedicalUICallback uiCallback;
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

        public DDAtlasPluginEditor(BrowserWindow browserWindow, TimelineController mainTimelineController, AtlasPluginManager pluginManager)
            : base("Developer.GUI.DDAtlasPluginEditor.DDAtlasPluginEditor.layout")
        {
            this.mainTimelineController = mainTimelineController;
            this.pluginManager = pluginManager;

            uiCallback = new MedicalUICallback(browserWindow);
            uiCallback.addCustomQuery(DDAtlasPluginCustomQueries.GetTimelineController, getTimelineController);

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

            createNewExamDefinition();

            this.Resized += new EventHandler(DataDrivenExamEditor_Resized);
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

        public void createNewExamDefinition()
        {
            currentDefinition = new DDAtlasPlugin();
            currentDefinitionChanged(null);
        }

        public void loadExamDefinition()
        {
            using (FileOpenDialog fileDialog = new FileOpenDialog(MainWindow.Instance, "Open a plugin definition.", DeveloperConfig.TimelineProjectDirectory, "", PLUGIN_WILDCARD, false))
            {
                if (fileDialog.showModal() == NativeDialogResult.OK)
                {
                    try
                    {
                        using (Stream pluginStream = File.Open(fileDialog.Path, FileMode.Open, FileAccess.Read))
                        {
                            DDAtlasPlugin loadedPlugin = pluginManager.loadPlugin(pluginStream);
                            if (loadedPlugin != null)
                            {
                                currentDefinition = loadedPlugin;
                                currentDefinitionChanged(fileDialog.Path);
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
            }
        }

        public void saveExamDefinition()
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
                saveExamDefinitionAs();
            }
        }

        public void saveExamDefinitionAs()
        {
            using (FileSaveDialog fileDialog = new FileSaveDialog(MainWindow.Instance, "Save a plugin definition", DeveloperConfig.TimelineProjectDirectory, "", PLUGIN_WILDCARD))
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

        void DataDrivenExamEditor_Resized(object sender, EventArgs e)
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

        private void getTimelineController(SendResult<Object> resultCallback, params Object[] args)
        {
            String errorPrompt = null;
            resultCallback.Invoke(mainTimelineController, ref errorPrompt);
        }

        void fileMenuCtrl_ItemAccept(Widget source, EventArgs e)
        {
            MenuCtrlAcceptEventArgs mcae = (MenuCtrlAcceptEventArgs)e;
            switch (mcae.Item.ItemId)
            {
                case "New":
                    createNewExamDefinition();
                    break;
                case "Open":
                    loadExamDefinition();
                    break;
                case "Save":
                    saveExamDefinition();
                    break;
                case "Save As":
                    saveExamDefinitionAs();
                    break;
            }
        }
    }
}
