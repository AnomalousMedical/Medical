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
    public class ElementAttributeEditor : ElementEditorComponent
    {
        private ScrollView propertiesScroll;
        private ScrollablePropertiesForm propertiesForm;
        private List<RmlEditableProperty> originalProperties;

        public ElementAttributeEditor(Element element, MedicalUICallback uiCallback, RmlWysiwygBrowserProvider browserProvider)
            : base("Medical.GUI.Editor.RmlWysiwyg.ElementEditorComponents.ElementAttributeEditor.layout", "Attributes")
        {
            propertiesScroll = (ScrollView)widget.findWidget("PropertiesScroll");
            propertiesForm = new ScrollablePropertiesForm(propertiesScroll, uiCallback);
            originalProperties = new List<RmlEditableProperty>(element.NumAttributes);

            EditInterface editInterface = new EditInterface(element.TagName);
            int index = 0;
            String name;
            String value;
            while (element.IterateAttributes(ref index, out name, out value))
            {
                RmlEditableProperty property;
                switch (name.ToLowerInvariant())
                {
                    case "onclick":
                        property = new RmlEditableProperty(name, value, callback =>
                        {
                            return browserProvider.createActionBrowser();
                        });
                        break;
                    case "src":
                        if (element.TagName == "img")
                        {
                            property = new RmlEditableProperty(name, value, callback =>
                            {
                                return browserProvider.createFileBrowser(new String[] { "*.png", "*.jpg", "*jpeg", "*.gif", "*.bmp" }, "Images", "/");
                            });
                        }
                        else
                        {
                            property = new RmlEditableProperty(name, value);
                        }
                        break;
                    default:
                        property = new RmlEditableProperty(name, value);
                        break;
                }
                property.ValueChanged += sender =>
                    {
                        fireChangesMade();
                        fireApplyChanges();
                    };
                editInterface.addEditableProperty(property);
                originalProperties.Add(property);
            }
            propertiesForm.EditInterface = editInterface;
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

        public bool applyToElement(Element element)
        {
            if (HasChanges)
            {
                foreach (RmlEditableProperty property in originalProperties)
                {
                    element.SetAttribute(property.Name, property.Value);
                }
                return true;
            }
            return false;
        }
    }
}
