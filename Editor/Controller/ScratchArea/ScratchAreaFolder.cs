using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;
using Logging;
using System.IO;

namespace Medical
{
    partial class ScratchAreaFolder
    {
        protected String folderName;
        private ScratchAreaFolder parentFolder;
        private List<ScratchAreaFolder> children = new List<ScratchAreaFolder>();

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
    }

    partial class ScratchAreaFolder
    {
        private EditInterface editInterface;
        private EditInterfaceManager<ScratchAreaFolder> dataFieldEdits;

        public EditInterface EditInterface
        {
            get
            {
                if (editInterface == null)
                {
                    editInterface = new EditInterface(folderName);
                    editInterface.addCommand(new EditInterfaceCommand("Add Folder", addFolderCallback));

                    dataFieldEdits = new EditInterfaceManager<ScratchAreaFolder>(editInterface);
                    dataFieldEdits.addCommand(new EditInterfaceCommand("Remove", removeFolderCallback));

                    foreach (ScratchAreaFolder folder in children)
                    {
                        onFolderAdded(folder);
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
            ScratchAreaFolder folder = dataFieldEdits.resolveSourceObject(callback.getSelectedEditInterface());
            removeFolder(folder);
        }

        private void onFolderAdded(ScratchAreaFolder folder)
        {
            if (editInterface != null)
            {
                dataFieldEdits.addSubInterface(folder, folder.EditInterface);
            }
        }

        private void onFolderRemoved(ScratchAreaFolder folder)
        {
            if (editInterface != null)
            {
                dataFieldEdits.removeSubInterface(folder);
            }
        }
    }
}
