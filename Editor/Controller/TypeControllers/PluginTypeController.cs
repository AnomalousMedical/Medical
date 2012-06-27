using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI;
using MyGUIPlugin;
using System.IO;

namespace Medical
{
    class PluginTypeController : SaveableTypeController<DDAtlasPlugin>
    {
        private EditorController editorController;
        private PluginEditorContext editorContext;
        private const String Icon = "EditorFileIcon/.ddp";

        public PluginTypeController(EditorController editorController)
            :base(".ddp", editorController)
        {
            this.editorController = editorController;
            editorController.ProjectChanged += new EditorControllerEvent(editorController_ProjectChanged);
        }

        public override void openFile(string file)
        {
            DDAtlasPlugin plugin = (DDAtlasPlugin)loadObject(file);

            editorContext = new PluginEditorContext(plugin, file, this);
            editorContext.Focus += new Action<PluginEditorContext>(editorContext_Focus);
            editorContext.Blur += new Action<PluginEditorContext>(editorContext_Blur);
            editorController.runEditorContext(editorContext.MvcContext);
        }

        public void saveFile(DDAtlasPlugin plugin, string file)
        {
            saveObject(file, plugin);
            editorController.NotificationManager.showNotification(String.Format("{0} saved.", file), Icon, 2);
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
            creatingNewFile(filePath);
            saveObject(filePath, newPlugin);
            openFile(filePath);
        }

        void editorController_ProjectChanged(EditorController editorController)
        {
            close();
        }

        private void close()
        {
            if (editorContext != null)
            {
                editorContext.close();
            }
            closeCurrentCachedResource();
        }

        void editorContext_Focus(PluginEditorContext obj)
        {
            editorContext = obj;
        }

        void editorContext_Blur(PluginEditorContext obj)
        {
            if (editorContext == obj)
            {
                editorContext = null;
            }
        }
    }
}
