using Engine;
using Engine.Editing;
using libRocketPlugin;
using Medical.Editor;
using MyGUIPlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.GUI.RmlWysiwyg.ElementEditorComponents
{
    public class ElementAttributeEditor : ElementEditorComponent
    {
        public enum CustomQueries
        {
            BuildActionBrowser,
            BuildFileBrowser
        }

        private ScrollView propertiesScroll;
        private ScrollablePropertiesForm propertiesForm;
        private List<RmlEditableProperty> originalProperties;

        public ElementAttributeEditor(Element element, MedicalUICallback uiCallback)
            : base("Medical.GUI.Editor.RmlWysiwyg.ElementEditorComponents.ElementAttributeEditor.layout", "Attributes")
        {
            propertiesScroll = (ScrollView)widget.findWidget("PropertiesScroll");
            propertiesForm = new ScrollablePropertiesForm(propertiesScroll, uiCallback);
            originalProperties = new List<RmlEditableProperty>(element.NumAttributes);

            EditInterface editInterface = new EditInterface(element.TagName);
            int index = 0;
            String name;
            String value;
            RmlEditableProperty.CreateBrowser createBrowserCallback = null;
            while (element.IterateAttributes(ref index, out name, out value))
            {
                createBrowserCallback = null;
                RmlEditableProperty property;
                switch (name.ToLowerInvariant())
                {
                    case "onclick":
                        if (uiCallback.hasCustomQuery(CustomQueries.BuildActionBrowser))
                        {
                            createBrowserCallback = callback => uiCallback.runSyncCustomQuery<Browser>(CustomQueries.BuildActionBrowser);
                        }
                        property = new RmlEditableProperty(name, value, createBrowserCallback);
                        break;
                    case "src":
                        if (element.TagName == "img")
                        {
                            if (uiCallback.hasCustomQuery(CustomQueries.BuildFileBrowser))
                            {
                                createBrowserCallback = callback => uiCallback.runSyncCustomQuery<Browser, IEnumerable<String>, String, String>(CustomQueries.BuildFileBrowser, new String[] { "*.png", "*.jpg", "*jpeg", "*.gif", "*.bmp" }, "Images", "/");
                            }
                            property = new RmlEditableProperty(name, value, createBrowserCallback);
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
