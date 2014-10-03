using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI;
using System.Xml;
using Medical.Muscles;
using MyGUIPlugin;
using System.IO;

namespace Medical
{
    public class OffsetSequenceTypeController : SaveableTypeController<OffsetModifierSequence>
    {
        private const String Icon = CommonResources.NoIcon;

        public OffsetSequenceTypeController(EditorController editorController)
            :base(".oms", editorController)
        {
            
        }

        public void saveFile(OffsetModifierSequence sequence, String file)
        {
            saveObject(file, sequence);
            EditorController.NotificationManager.showNotification(String.Format("{0} saved.", file), Icon, 2);
        }

        public override ProjectItemTemplate createItemTemplate()
        {
            return new ProjectItemTemplateDelegate("Offset Modifier Sequence", Icon, "File", delegate(String path, String fileName, EditorController editorController)
            {
                String filePath = Path.Combine(path, fileName);
                filePath = Path.ChangeExtension(filePath, ".oms");
                if (EditorController.ResourceProvider.exists(filePath))
                {
                    MessageBox.show(String.Format("Are you sure you want to override {0}?", filePath), "Override", MessageBoxStyle.IconQuest | MessageBoxStyle.Yes | MessageBoxStyle.No, delegate(MessageBoxStyle overrideResult)
                    {
                        if (overrideResult == MessageBoxStyle.Yes)
                        {
                            createNew(filePath);
                        }
                    });
                }
                else
                {
                    createNew(filePath);
                }
            });
        }

        void createNew(String filePath)
        {
            OffsetModifierSequence sequence = new OffsetModifierSequence();
            creatingNewFile(filePath);
            saveObject(filePath, sequence);
            openEditor(filePath);
        }
    }
}
