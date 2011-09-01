using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;
using Logging;
using System.IO;
using Engine.Saving;

namespace Medical
{
    partial class ScratchAreaFolder
    {
        protected String folderName;
        private ScratchAreaFolder parentFolder;
        private List<ScratchAreaFolder> children = new List<ScratchAreaFolder>();
        private List<ScratchAreaItem> items = new List<ScratchAreaItem>();

        protected ScratchAreaFolder(String folderName)
        {
            this.folderName = folderName;
        }

        public ScratchAreaFolder(String folderName, ScratchAreaFolder parent)
        {
            this.folderName = folderName;
            this.parentFolder = parent;
        }

        public ScratchAreaFolder addFolder(String name)
        {
            ScratchAreaFolder folder = new ScratchAreaFolder(name, this);
            String path = Path.Combine(FilesystemPath, name);
            try
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                children.Add(folder);
                onFolderAdded(folder);
            }
            catch (Exception e)
            {
                Log.Error("Could not create ScratchAreaFolder {0} in path {1} because {2}", name, path, e.Message);
            }
            return folder;
        }

        public void removeFolder(String name)
        {
            ScratchAreaFolder folder = null;
            foreach (ScratchAreaFolder currentFolder in children)
            {
                if (currentFolder.folderName == name)
                {
                    folder = currentFolder;
                    break;
                }
            }
            removeFolder(folder);
        }

        private void removeFolder(ScratchAreaFolder folder)
        {
            if (folder != null && children.Remove(folder))
            {
                String folderPath = folder.FilesystemPath;
                try
                {
                    if(Directory.Exists(folderPath))
                    {
                        Directory.Delete(folderPath, true);
                    }
                    onFolderRemoved(folder);
                }
                catch (Exception e)
                {
                    Log.Error("Could not delete ScratchAreaFolder {0} at path {1} because {2}", folder.folderName, folderPath, e.Message);
                }
            }
        }

        public bool hasFolder(String name)
        {
            foreach (ScratchAreaFolder currentFolder in children)
            {
                if (currentFolder.folderName == name)
                {
                    return true;
                }
            }
            return false;
        }

        public void addItem(String name, Saveable data)
        {
            ScratchAreaItem item = new ScratchAreaItem(name, this);
            item.serialize(data);
            items.Add(item);
            onItemAdded(item);
        }

        public void removeItem(ScratchAreaItem item)
        {
            if (item != null && items.Remove(item))
            {
                item.deleteFile();
                onItemRemoved(item);
            }
        }

        public bool hasItem(String name)
        {
            foreach (ScratchAreaItem item in items)
            {
                if (item.Name == name)
                {
                    return true;
                }
            }
            return false;
        }

        public void loadInfo()
        {
            String path = FilesystemPath;
            foreach (String directory in Directory.GetDirectories(path, "*", SearchOption.TopDirectoryOnly))
            {
                DirectoryInfo info = new DirectoryInfo(directory);
                if((info.Attributes & FileAttributes.Hidden) == 0)
                {
                    ScratchAreaFolder folder = addFolder(info.Name);
                    folder.loadInfo();
                }
            }

            foreach (String file in Directory.GetFiles(path, "*.sav", SearchOption.TopDirectoryOnly))
            {
                addExistingItem(Path.GetFileNameWithoutExtension(file));
            }
        }

        public String FilesystemPath
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                findPath(sb);
                return sb.ToString();
            }
        }

        protected virtual void findPath(StringBuilder sb)
        {
            if (parentFolder != null)
            {
                parentFolder.findPath(sb);
            }
            sb.Append("//");
            sb.Append(folderName);
        }

        private void addExistingItem(String name)
        {
            ScratchAreaItem item = new ScratchAreaItem(name, this);
            items.Add(item);
            onItemAdded(item);
        }
    }

    partial class ScratchAreaFolder
    {
        private EditInterface editInterface;
        private EditInterfaceManager<ScratchAreaFolder> folderEdits;
        private EditInterfaceManager<ScratchAreaItem> itemEdits;

        public EditInterface EditInterface
        {
            get
            {
                if (editInterface == null)
                {
                    editInterface = new EditInterface(folderName);
                    editInterface.addCommand(new EditInterfaceCommand("Add Folder", addFolderCallback));
                    editInterface.addCommand(new EditInterfaceCommand("Paste", pasteCallback));

                    folderEdits = new EditInterfaceManager<ScratchAreaFolder>(editInterface);
                    folderEdits.addCommand(new EditInterfaceCommand("Remove", removeFolderCallback));

                    itemEdits = new EditInterfaceManager<ScratchAreaItem>(editInterface);
                    itemEdits.addCommand(new EditInterfaceCommand("Remove", removeItemCallback));

                    foreach (ScratchAreaFolder folder in children)
                    {
                        onFolderAdded(folder);
                    }
                    foreach (ScratchAreaItem item in items)
                    {
                        onItemAdded(item);
                    }
                }
                return editInterface;
            }
        }

        private void addFolderCallback(EditUICallback callback, EditInterfaceCommand caller)
        {
            callback.getInputString("Enter a name.", delegate(String input, ref String errorPrompt)
            {
                if (!hasFolder(input))
                {
                    addFolder(input);
                    return true;
                }
                errorPrompt = String.Format("A folder named {0} already exists. Please input another name.", input);
                return false;
            });
        }

        private void removeFolderCallback(EditUICallback callback, EditInterfaceCommand caller)
        {
            ScratchAreaFolder folder = folderEdits.resolveSourceObject(callback.getSelectedEditInterface());
            removeFolder(folder);
        }

        private void pasteCallback(EditUICallback callback, EditInterfaceCommand caller)
        {
            SaveableClipboard clipboard = null;
            callback.runCustomQuery(ScratchAreaCustomQueries.GetClipboard, delegate(Object result, ref String errorMessage)
            {
                clipboard = result as SaveableClipboard;
                return true;
            });
            if (clipboard != null && clipboard.HasSourceObject)
            {
                callback.getInputString("Enter a name.", delegate(String input, ref String errorPrompt)
                {
                    if (!hasItem(input))
                    {
                        addItem(input, clipboard.createCopy<Saveable>());
                        return true;
                    }
                    errorPrompt = String.Format("An item named {0} already exists. Please input another name.", input);
                    return false;
                });
            }
        }

        private void removeItemCallback(EditUICallback callback, EditInterfaceCommand caller)
        {
            ScratchAreaItem item = itemEdits.resolveSourceObject(callback.getSelectedEditInterface());
            removeItem(item);
        }

        private void onFolderAdded(ScratchAreaFolder folder)
        {
            if (editInterface != null)
            {
                folderEdits.addSubInterface(folder, folder.EditInterface);
            }
        }

        private void onFolderRemoved(ScratchAreaFolder folder)
        {
            if (editInterface != null)
            {
                folderEdits.removeSubInterface(folder);
            }
        }

        private void onItemAdded(ScratchAreaItem item)
        {
            if (editInterface != null)
            {
                itemEdits.addSubInterface(item, item.EditInterface);
            }
        }

        private void onItemRemoved(ScratchAreaItem item)
        {
            if (editInterface != null)
            {
                itemEdits.removeSubInterface(item);
            }
        }
    }
}
