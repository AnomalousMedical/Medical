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
        public const String Icon = CommonResources.NoIcon;

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
            return new OffsetModifierSequenceTemplate(this);
        }

        public void createNew(String filePath, OffsetModiferSequenceType type)
        {
            OffsetModifierSequence sequence;

            switch(type)
            {
                case OffsetModiferSequenceType.Orbit:
                    sequence = new OrbitOffsetModifierSequence();
                    break;
                default:
                    sequence = new SimpleOffsetModifierSequence();
                    break;
            }

            creatingNewFile(filePath);
            saveObject(filePath, sequence);
            openEditor(filePath);
        }
    }
}
