using Engine;
using Engine.Editing;
using ExCSS;
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
        private StyleSheet sheet;
        private StyleDeclaration declarations;

        public ElementStyleEditor(Element element, MedicalUICallback uiCallback, RmlWysiwygBrowserProvider browserProvider)
            : base("Medical.GUI.Editor.RmlWysiwyg.ElementEditorComponents.ElementStyleEditor.layout", "Style")
        {
            propertiesScroll = (ScrollView)widget.findWidget("PropertiesScroll");
            propertiesForm = new ScrollablePropertiesForm(propertiesScroll, uiCallback);

            String style = element.GetAttributeString("style");
            Parser parser = new Parser();
            sheet = parser.Parse(String.Format(".i{{{0}}}", style));
            declarations = sheet.Rulesets[0].Declarations;

            EditInterface editInterface = new EditInterface(element.TagName);
            addProperty(new StyleColorEditableProperty("color", declarations), editInterface);
            addProperty(new StyleColorEditableProperty("background-color", declarations), editInterface);
            addProperty(new StyleColorEditableProperty("border-color", declarations), editInterface);
            addProperty(new StyleEditableProperty("border-width", declarations, UnitType.Pixel), editInterface);
            addProperty(new StyleEditableProperty("font-size", declarations, UnitType.Pixel), editInterface);
            addProperty(new StyleEditableProperty("margin-left", declarations, UnitType.Pixel), editInterface);
            addProperty(new StyleEditableProperty("margin-right", declarations, UnitType.Pixel), editInterface);
            addProperty(new StyleEditableProperty("margin-top", declarations, UnitType.Pixel), editInterface);
            addProperty(new StyleEditableProperty("margin-bottom", declarations, UnitType.Pixel), editInterface);
            addProperty(new StyleEditableProperty("left", declarations, UnitType.Pixel), editInterface);
            addProperty(new StyleEditableProperty("top", declarations, UnitType.Pixel), editInterface);
            addProperty(new StyleEditableProperty("width", declarations, UnitType.Pixel), editInterface);
            addProperty(new StyleEditableProperty("height", declarations, UnitType.Pixel), editInterface);
            addProperty(new StyleChoiceEditableProperty("float", declarations, UnitType.Unknown, floatChoices), editInterface);
            addProperty(new StyleChoiceEditableProperty("clear", declarations, UnitType.Unknown, clearChoices), editInterface);
            addProperty(new StyleChoiceEditableProperty("position", declarations, UnitType.Unknown, positionChoices), editInterface);
            propertiesForm.EditInterface = editInterface;
        }

        private void addProperty(StyleEditableProperty property, EditInterface editInterface)
        {
            property.ValueChanged += sender =>
            {
                fireChangesMade();
                fireApplyChanges();
            };
            editInterface.addEditableProperty(property);
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
            styleString.Append(declarations.ToString());
            return HasChanges;
        }

        private static readonly Pair<String, Object>[] floatChoices = { new Pair<String, Object>("inherit", null),
                                                                        new Pair<String, Object>("none", "none"),
                                                                        new Pair<String, Object>("left", "left"),
                                                                        new Pair<String, Object>("right", "right") };

        private static readonly Pair<String, Object>[] clearChoices = { new Pair<String, Object>("inherit", null),
                                                                        new Pair<String, Object>("none", "none"),
                                                                        new Pair<String, Object>("left", "left"),
                                                                        new Pair<String, Object>("right", "right"),
                                                                        new Pair<String, Object>("both", "both") };

        private static readonly Pair<String, Object>[] positionChoices = { new Pair<String, Object>("inherit", null),
                                                                           new Pair<String, Object>("static", "static"),
                                                                           new Pair<String, Object>("relative", "relative"),
                                                                           new Pair<String, Object>("absolute", "absolute"),
                                                                           new Pair<String, Object>("fixed", "fixed")};
    }
}
