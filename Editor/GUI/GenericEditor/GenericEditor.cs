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
    public class GenericEditor : MDIDialog
    {
        private Tree tree;
        private EditInterfaceTreeView editTreeView;

        private ResizingTable table;
        private PropertiesTable propTable;

        private ObjectEditor objectEditor;

        private String currentFile = null;
        private String defaultDirectory = "";

        private GenericEditorObject editorObject;
        private EditorController editorController;
        private ExtensionActionCollection extensionActions = new ExtensionActionCollection();

        public GenericEditor(String persistName, GenericEditorObject editorObject, EditorController editorController)
            : base("Medical.GUI.GenericEditor.GenericEditor.layout", persistName)
        {
            this.editorController = editorController;
            this.editorObject = editorObject;
            window.Caption = String.Format("{0} Editor", editorObject.ObjectTypeName);
            window.RootKeyChangeFocus += new MyGUIEvent(window_RootKeyChangeFocus);

            tree = new Tree((ScrollView)window.findWidget("TreeScroller"));
            editTreeView = new EditInterfaceTreeView(tree, editorObject.UICallback);

            table = new ResizingTable((ScrollView)window.findWidget("TableScroller"));
            propTable = new PropertiesTable(table, editorObject.UICallback);

            objectEditor = new ObjectEditor(editTreeView, propTable, editorObject.UICallback);

            extensionActions.Add(new ExtensionAction(String.Format("Save {0}", editorObject.ObjectTypeName), "File", save));
            extensionActions.Add(new ExtensionAction(String.Format("Save {0} As", editorObject.ObjectTypeName), "File", saveAs));

            createNew();

            this.Resized += new EventHandler(GenericEditor_Resized);
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

        public void createNew()
        {
            editorObject.createNew();
            currentFileChanged(null);
        }

        public void load()
        {
            using (FileOpenDialog fileDialog = new FileOpenDialog(MainWindow.Instance, String.Format("Open a {0} definition.", editorObject.ObjectTypeName), defaultDirectory, "", editorObject.FileWildcard, false))
            {
                if (fileDialog.showModal() == NativeDialogResult.OK)
                {
                    load(fileDialog.Path);
                }
            }
        }

        public void load(String filename)
        {
            try
            {
                using (Stream stream = File.Open(filename, FileMode.Open, FileAccess.Read))
                {
                    if (editorObject.load(stream))
                    {
                        currentFileChanged(filename);
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

        public void save()
        {
            if (currentFile != null)
            {
                using (Stream stream = File.Open(currentFile, FileMode.Create, FileAccess.Write))
                {
                    editorObject.save(stream);
                }
            }
            else
            {
                saveAs();
            }
        }

        public void saveAs()
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

        public void activateExtensionActions()
        {
            editorController.ExtensionActions = extensionActions;
        }

        void window_RootKeyChangeFocus(Widget source, EventArgs e)
        {
            if (((RootFocusEventArgs)e).Focus)
            {
                activateExtensionActions();
            }
        }

        void GenericEditor_Resized(object sender, EventArgs e)
        {
            tree.layout();
            table.layout();
        }

        private void currentFileChanged(String file)
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
    }
}
