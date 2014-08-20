using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI;
using MyGUIPlugin;
using System.IO;

namespace Medical
{
    public class DependencyTypeController : SaveableTypeController<DDAtlasDependency>
    {
        private const String Icon = CommonResources.NoIcon;

        public DependencyTypeController(EditorController editorController)
            :base(".ddd", editorController)
        {
            
        }

        public void saveFile(DDAtlasDependency dependency, string file)
        {
            saveObject(file, dependency);
            EditorController.NotificationManager.showNotification(String.Format("{0} saved.", file), Icon, 2);
        }

        public override ProjectItemTemplate createItemTemplate()
        {
            return new ProjectItemTemplateFixedNameDelegate("Dependency Definition", Icon, "File", delegate(String path, EditorController editorController)
            {
                String filePath = Path.Combine(path, "Dependency.ddd");
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
            });
        }

        private void createNewPlugin(String filePath)
        {
            DDAtlasDependency newDep = new DDAtlasDependency()
                {
                    VersionString = "1.0.0.0",
                    PropDefinitionDirectory = "PropDefinitions",
                };
            creatingNewFile(filePath);
            saveObject(filePath, newDep);
            openEditor(filePath);
        }
    }
}
