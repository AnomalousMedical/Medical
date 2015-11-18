﻿using Anomalous.GuiFramework.Editor;
using Engine.Editing;
using libRocketPlugin;
using Medical.GUI.RmlWysiwyg.ElementEditorComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical.GUI.RmlWysiwyg.Elements
{
    class DataStrategy : ElementStrategy
    {
        enum DataDisplayType
        {
            volume,
            measurement
        }

        private EditInterfaceEditor editInterfaceEditor;
        private DataElementEditor dataElementEditor;

        public DataStrategy(String tag, String previewIconName = CommonResources.NoIcon)
            : base(tag, previewIconName, true)
        {

        }

        public override RmlElementEditor openEditor(Element element, GuiFrameworkUICallback uiCallback, int left, int top)
        {
            dataElementEditor = new DataElementEditor(element);
            EditInterface editInterface = dataElementEditor.EditInterface;
            editInterfaceEditor = new EditInterfaceEditor("Data Display Properties", editInterface, uiCallback);
            dataElementEditor.EditInterfaceEditor = editInterfaceEditor;
            RmlElementEditor editor = RmlElementEditor.openEditor(element, left, top, this);
            editor.addElementEditor(editInterfaceEditor);
            return editor;
        }

        public override bool applyChanges(Element element, RmlElementEditor editor, RmlWysiwygComponent component)
        {
            dataElementEditor.applyToElement(element);
            return true;
        }

        private class DataElementEditor
        {
            private DataDisplayType dataType;
            private String target;
            private EditInterface editInterface;

            public DataElementEditor(Element element)
            {
                DataDisplayType type;
                if (!Enum.TryParse<DataDisplayType>(element.GetAttributeString("type"), out type))
                {
                    type = DataDisplayType.volume;
                }
                dataType = type;

                target = element.GetAttributeString("target");
            }

            public void applyToElement(Element element)
            {
                element.SetAttribute("type", DataType.ToString());
                element.SetAttribute("target", Target);
            }

            [Editable]
            public DataDisplayType DataType
            {
                get
                {
                    return dataType;
                }
                set
                {
                    dataType = value;
                    EditInterfaceEditor.alertChangesMade();
                }
            }

            [Editable]
            public String Target
            {
                get
                {
                    return target;
                }
                set
                {
                    target = value;
                    EditInterfaceEditor.alertChangesMade();
                }
            }

            public EditInterfaceEditor EditInterfaceEditor { get; set; }

            public EditInterface EditInterface
            {
                get
                {
                    if (editInterface == null)
                    {
                        editInterface = ReflectedEditInterface.createEditInterface(this, "Data Display");
                    }
                    return editInterface;
                }
            }
        }
    }
}
