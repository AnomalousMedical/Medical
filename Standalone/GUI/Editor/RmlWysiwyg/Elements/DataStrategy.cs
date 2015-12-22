using Anomalous.GuiFramework;
using Anomalous.GuiFramework.Editor;
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

        enum DataDisplayUnits
        {
            percent,
            millimeters,
            centimeters,
        }

        private EditInterfaceEditor editInterfaceEditor;
        private DataElementEditor dataElementEditor;

        private EditInterfaceEditor appearanceEditor;
        private TextElementStyle elementStyle;

        public DataStrategy(String tag, String previewIconName = CommonResources.NoIcon)
            : base(tag, previewIconName, true)
        {

        }

        public override RmlElementEditor openEditor(Element element, GuiFrameworkUICallback uiCallback, int left, int top)
        {
            elementStyle = new TextElementStyle(element, true);
            elementStyle.Changed += elementStyle_Changed;
            appearanceEditor = new EditInterfaceEditor("Appearance", elementStyle.getEditInterface(), uiCallback);

            dataElementEditor = new DataElementEditor(element);
            EditInterface editInterface = dataElementEditor.EditInterface;
            editInterfaceEditor = new EditInterfaceEditor("Data Display Properties", editInterface, uiCallback);
            dataElementEditor.EditInterfaceEditor = editInterfaceEditor;
            RmlElementEditor editor = RmlElementEditor.openEditor(element, left, top, this);

            editor.addElementEditor(editInterfaceEditor);
            editor.addElementEditor(appearanceEditor);

            return editor;
        }

        public override bool applyChanges(Element element, RmlElementEditor editor, RmlWysiwygComponent component, out TwoWayCommand additionalUndoOperations)
        {
            additionalUndoOperations = null;
            build(element);
            return true;
        }

        void elementStyle_Changed(StyleDefinition obj)
        {
            appearanceEditor.alertChangesMade();
        }

        public override void applySizeChange(Element element)
        {
            appearanceEditor.alertChangesMade();
        }

        private void build(Element element)
        {
            element.ClearLocalStyles();
            dataElementEditor.applyToElement(element);

            StringBuilder style = new StringBuilder();
            elementStyle.buildStyleAttribute(style);
            if (style.Length > 0)
            {
                element.SetAttribute("style", style.ToString());
            }
            else
            {
                element.RemoveAttribute("style");
            }

            StringBuilder classes = new StringBuilder();
            elementStyle.buildClassList(classes);
            element.ClassNames = classes.ToString();
        }

        private class DataElementEditor
        {
            private DataDisplayType dataType;
            private String target;
            private DataDisplayUnits units;
            private EditInterface editInterface;

            public DataElementEditor(Element element)
            {
                if (!Enum.TryParse<DataDisplayType>(element.GetAttributeString("type"), out dataType))
                {
                    dataType = DataDisplayType.volume;
                }

                target = element.GetAttributeString("target");

                if (!Enum.TryParse<DataDisplayUnits>(element.GetAttributeString("units"), out units))
                {
                    units = DataDisplayUnits.millimeters;
                }
            }

            public void applyToElement(Element element)
            {
                element.SetAttribute("type", DataType.ToString());
                element.SetAttribute("target", Target);
                element.SetAttribute("units", units.ToString());
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

            [Editable]
            public DataDisplayUnits Units
            {
                get
                {
                    return units;
                }
                set
                {
                    units = value;
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
                        editInterface.addEditableProperty(new TargetEditableProperty(this));
                    }
                    return editInterface;
                }
            }

            private class TargetEditableProperty : EditableProperty
            {
                private DataElementEditor dataElementEditor;

                public TargetEditableProperty(DataElementEditor dataElementEditor)
                {
                    this.dataElementEditor = dataElementEditor;
                }

                public bool Advanced
                {
                    get
                    {
                        return false;
                    }
                }

                public bool canParseString(int column, string value, out string errorMessage)
                {
                    errorMessage = null;
                    return true;
                }

                public Browser getBrowser(int column, EditUICallback uiCallback)
                {
                    switch (dataElementEditor.DataType)
                    {
                        case DataDisplayType.volume:
                            return VolumeController.Browser;
                        case DataDisplayType.measurement:
                            return MeasurementController.Browser;
                        default:
                            throw new NotImplementedException();
                    }
                }

                public Type getPropertyType(int column)
                {
                    return typeof(String);
                }

                public object getRealValue(int column)
                {
                    if (column == 1)
                    {
                        return dataElementEditor.Target;
                    }
                    return "Target";
                }

                public string getValue(int column)
                {
                    if (column == 1)
                    {
                        return dataElementEditor.Target;
                    }
                    return "Target";
                }

                public bool hasBrowser(int column)
                {
                    return column == 1;
                }

                public bool readOnly(int column)
                {
                    return false;
                }

                public void setValue(int column, object value)
                {
                    if (column == 1)
                    {
                        dataElementEditor.Target = value.ToString();
                    }
                }

                public void setValueStr(int column, string value)
                {
                    if (column == 1)
                    {
                        dataElementEditor.Target = value;
                    }
                }
            }
        }
    }
}
