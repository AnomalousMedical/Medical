using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI;
using MyGUIPlugin;
using System.IO;

namespace Medical
{
    public class PluginTypeController : SaveableTypeController<DDAtlasPlugin>
    {
        private const String Icon = "EditorFileIcon/.ddp";

        public PluginTypeController(EditorController editorController)
            :base(".ddp", editorController)
        {
            
        }

        public void saveFile(DDAtlasPlugin plugin, string file)
        {
            saveObject(file, plugin);
            EditorController.NotificationManager.showNotification(String.Format("{0} saved.", file), Icon, 2);
        }

        public override void addCreationMethod(ContextMenu contextMenu, string path, bool isDirectory, bool isTopLevel)
        {
            if (isTopLevel)
            {
                contextMenu.add(new ContextMenuItem("Create Plugin Definition", path, delegate(ContextMenuItem item)
                {
                    String filePath = Path.Combine(item.UserObject.ToString(), "Plugin.ddp");
                    if (EditorController.ResourceProvider.exists(filePath))
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
            creatingNewFile(filePath);
            saveObject(filePath, newPlugin);
            openFile(filePath);
        }
    }
}
