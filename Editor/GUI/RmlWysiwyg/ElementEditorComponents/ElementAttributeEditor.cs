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
    class ElementAttributeEditor : ElementEditorComponent
    {
        private ScrollView propertiesScroll;
        private ScrollablePropertiesForm propertiesForm;

        public ElementAttributeEditor(Element element, MedicalUICallback uiCallback, RmlWysiwygBrowserProvider browserProvider)
            : base("Medical.GUI.RmlWysiwyg.ElementEditorComponents.ElementAttributeEditor.layout", "Attributes")
        {
            propertiesScroll = (ScrollView)widget.findWidget("PropertiesScroll");
            propertiesForm = new ScrollablePropertiesForm(propertiesScroll, uiCallback);

            EditInterface editInterface = new EditInterface(element.TagName);
            int index = 0;
            String name = null;
            String value = null;
            while (element.IterateAttributes(ref index, ref name, ref value))
            {
                switch (name.ToLowerInvariant())
                {
                    case "onclick":
                        editInterface.addEditableProperty(new RmlEditableProperty(name, value, element, callback =>
                        {
                            return browserProvider.createActionBrowser();
                        }));
                        break;
                    case "src":
                        if (element.TagName == "img")
                        {
                            editInterface.addEditableProperty(new RmlEditableProperty(name, value, element, callback =>
                            {
                                return browserProvider.createFileBrowser(new String[] { "*.png", "*.jpg", "*jpeg", "*.gif", "*.bmp" }, "Images", "/");
                            }));
                        }
                        else
                        {
                            editInterface.addEditableProperty(new RmlEditableProperty(name, value, element));
                        }
                        break;
                    default:
                        editInterface.addEditableProperty(new RmlEditableProperty(name, value, element));
                        break;
                }
            }
            propertiesForm.EditInterface = editInterface;
        }

        public override void Dispose()
        {
            propertiesForm.Dispose();
            base.Dispose();
        }
    }
}
