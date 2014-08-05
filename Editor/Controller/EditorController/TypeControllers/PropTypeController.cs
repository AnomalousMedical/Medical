using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI;
using MyGUIPlugin;
using System.IO;
using Engine.ObjectManagement;

namespace Medical
{
    public class PropTypeController : SaveableTypeController<PropDefinition>
    {
        private const String Icon = CommonResources.NoIcon;

        public PropTypeController(EditorController editorController)
            :base(".prop", editorController)
        {
            
        }

        public void saveFile(PropDefinition prop, string file)
        {
            saveObject(file, prop);
            EditorController.NotificationManager.showNotification(String.Format("{0} saved.", file), Icon, 2);
        }

        public override ProjectItemTemplate createItemTemplate()
        {
            return new ProjectItemTemplateDelegate("Prop Definition", Icon, "File", delegate(String path, String fileName, EditorController editorController)
            {
                String filePath = Path.Combine(path, fileName);
                filePath = Path.ChangeExtension(filePath, ".prop");
                String propName = Path.GetFileNameWithoutExtension(filePath);
                if (EditorController.ResourceProvider.exists(filePath))
                {
                    MessageBox.show(String.Format("Are you sure you want to override {0}?", filePath), "Override", MessageBoxStyle.IconQuest | MessageBoxStyle.Yes | MessageBoxStyle.No, delegate(MessageBoxStyle overrideResult)
                    {
                        if (overrideResult == MessageBoxStyle.Yes)
                        {
                            createNewProp(filePath, propName);
                        }
                    });
                }
                else
                {
                    createNewProp(filePath, propName);
                }
            });
        }

        private void createNewProp(String filePath, String propName)
        {
            PropDefinition prop = new PropDefinition(new GenericSimObjectDefinition(propName))
            {
                Name = propName
            };
            creatingNewFile(filePath);
            saveObject(filePath, prop);
            openEditor(filePath);
        }
    }
}
