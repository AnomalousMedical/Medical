using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine.Editing;
using Engine.Saving.XMLSaver;
using System.Xml;
using System.IO;
using Engine.Saving;

namespace Medical.GUI
{
    class GenericEditor : MDIDialog
    {
        private MedicalUICallback uiCallback;
        private Tree tree;
        private EditInterfaceTreeView editTreeView;

        private ResizingTable table;
        private PropertiesTable propTable;

        private ObjectEditor objectEditor;

        private String currentFile = null;
        private String defaultDirectory = "";

        private GenericEditorObject editorObject;

        public GenericEditor(BrowserWindow browserWindow, String persistName, GenericEditorObject editorObject)
            : base("Medical.GUI.GenericEditor.GenericEditor.layout", persistName)
        {
            this.editorObject = editorObject;
            window.Caption = String.Format("{0} Editor", editorObject.ObjectTypeName);

            uiCallback = new MedicalUICallback(browserWindow);

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
            editorObject.createNew();
            currentDefinitionChanged(null);
        }

        public void loadExamDefinition()
        {
            using (FileOpenDialog fileDialog = new FileOpenDialog(MainWindow.Instance, String.Format("Open a {0} definition.", editorObject.ObjectTypeName), defaultDirectory, "", editorObject.FileWildcard, false))
            {
                if (fileDialog.showModal() == NativeDialogResult.OK)
                {
                    try
                    {
                        using (Stream stream = File.Open(fileDialog.Path, FileMode.Open, FileAccess.Read))
                        {
                            if (editorObject.load(stream))
                            {
                                currentDefinitionChanged(fileDialog.Path);
                            }
                            else
                            {
                                MessageBox.show("Load error", String.Format("There was an error loading this {0}.", editorObject.ObjectTypeName), MessageBoxStyle.Ok | MessageBoxStyle.IconError);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        MessageBox.show("Load error", String.Format("Exception loading {0}:\n{1}.", editorObject.ObjectTypeName, e.Message), MessageBoxStyle.Ok | MessageBoxStyle.IconError);
                    }
                }
            }
        }

        public void saveExamDefinition()
        {
            if (currentFile != null)
            {
                using (Stream stream = File.OpenWrite(currentFile))
                {
                    editorObject.save(stream);
                }
            }
            else
            {
                saveExamDefinitionAs();
            }
        }

        public void saveExamDefinitionAs()
        {
            using (FileSaveDialog fileDialog = new FileSaveDialog(MainWindow.Instance, String.Format("Save a {0} definition", editorObject.ObjectTypeName), defaultDirectory, "", editorObject.FileWildcard))
            {
                if (fileDialog.showModal() == NativeDialogResult.OK)
                {
                    try
                    {
                        using (Stream stream = File.OpenWrite(fileDialog.Path))
                        {
                            editorObject.save(stream);
                        }
                        fileChanged(fileDialog.Path);
                    }
                    catch (Exception e)
                    {
                        MessageBox.show("Load error", String.Format("Exception saving {0}:\n{1}.", editorObject.ObjectTypeName, e.Message), MessageBoxStyle.Ok | MessageBoxStyle.IconError);
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
            editTreeView.EditInterface = editorObject.getEditInterface();
            fileChanged(file);
        }

        private void fileChanged(String file)
        {
            currentFile = file;
            if (currentFile != null)
            {
                window.Caption = String.Format("{0} Editor - {1}", editorObject.ObjectTypeName, currentFile);
            }
            else
            {
                window.Caption = String.Format("{0} Editor", editorObject.ObjectTypeName);
            }
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
