using Engine.Attributes;
using Engine.Editing;
using MyGUIPlugin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical
{
    [SingleEnum]
    public enum OffsetModiferSequenceType
    {
        Simple,
        Orbit
    }

    class OffsetModifierSequenceTemplate : ProjectItemTemplate
    {
        OffsetSequenceTypeController typeController;

        public OffsetModifierSequenceTemplate(OffsetSequenceTypeController typeController)
        {
            this.typeController = typeController;
        }

        public void createItem(string path, EditorController editorController)
        {
            String filePath = Path.Combine(path, Path.ChangeExtension(Name, ".oms"));
            filePath = Path.ChangeExtension(filePath, ".oms");
            if (editorController.ResourceProvider.exists(filePath))
            {
                MessageBox.show(String.Format("Are you sure you want to override {0}?", filePath), "Override", MessageBoxStyle.IconQuest | MessageBoxStyle.Yes | MessageBoxStyle.No, delegate(MessageBoxStyle overrideResult)
                {
                    if (overrideResult == MessageBoxStyle.Yes)
                    {
                        typeController.createNew(filePath, Type);
                    }
                });
            }
            else
            {
                typeController.createNew(filePath, Type);
            }
        }

        public string TypeName
        {
            get
            {
                return "Offset Modifier Sequence";
            }
        }

        public string ImageName
        {
            get
            {
                return OffsetSequenceTypeController.Icon;
            }
        }

        public string Group
        {
            get
            {
                return "File";
            }
        }

        public bool isValid(out string errorMessage)
        {
            if (String.IsNullOrEmpty(Name))
            {
                errorMessage = "Please enter a name.";
                return false;
            }
            else
            {
                errorMessage = null;
                return true;
            }
        }

        private EditInterface editInterface;

        public EditInterface EditInterface
        {
            get
            {
                if (editInterface == null)
                {
                    editInterface = ReflectedEditInterface.createEditInterface(this, ReflectedEditInterface.DefaultScanner, TypeName, null);
                }
                return editInterface;
            }
        }

        [Editable]
        public String Name { get; set; }

        [Editable]
        public OffsetModiferSequenceType Type { get; set; }

        public void reset()
        {
            Name = null;
            Type = OffsetModiferSequenceType.Simple;
        }
    }
}
