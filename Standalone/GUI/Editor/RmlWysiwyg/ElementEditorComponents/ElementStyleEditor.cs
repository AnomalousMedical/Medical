using Engine;
using Engine.Editing;
using libRocketPlugin;
using MyGUIPlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.GUI.RmlWysiwyg.ElementEditorComponents
{
    public class ElementStyleEditor : ElementEditorComponent
    {
        private ScrollView propertiesScroll;
        private ScrollablePropertiesForm propertiesForm;
        private List<RmlEditableProperty> originalProperties;

        public ElementStyleEditor(Element element, MedicalUICallback uiCallback, RmlWysiwygBrowserProvider browserProvider)
            : base("Medical.GUI.Editor.RmlWysiwyg.ElementEditorComponents.ElementStyleEditor.layout", "Style")
        {
            propertiesScroll = (ScrollView)widget.findWidget("PropertiesScroll");
            propertiesForm = new ScrollablePropertiesForm(propertiesScroll, uiCallback);
            originalProperties = new List<RmlEditableProperty>(element.NumAttributes);

            EditInterface editInterface = new EditInterface(element.TagName);
            addProperty(new RmlEditableProperty("float", element.GetPropertyString("float")), editInterface);
            addProperty(new RmlEditableProperty("clear", element.GetPropertyString("clear")), editInterface);
            addProperty(new RmlEditableProperty("position", element.GetPropertyString("position")), editInterface);
            propertiesForm.EditInterface = editInterface;
        }

        private void addProperty(RmlEditableProperty property, EditInterface editInterface)
        {
            property.ValueChanged += sender =>
            {
                fireChangesMade();
                fireApplyChanges();
            };
            editInterface.addEditableProperty(property);
            originalProperties.Add(property);
        }

        public override void Dispose()
        {
            propertiesForm.Dispose();
            base.Dispose();
        }

        public override void attachToParent(RmlElementEditor parentEditor, Widget parent)
        {
            base.attachToParent(parentEditor, parent);
            propertiesForm.layout();
        }

        public bool buildStyleString(StringBuilder styleString)
        {
            foreach (RmlEditableProperty property in originalProperties)
            {
                if (property.Value != null)
                {
                    styleString.AppendFormat("{0}:{1};", property.Name, property.Value);
                }
            }
            return HasChanges;
        }
    }
}
