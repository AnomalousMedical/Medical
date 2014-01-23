using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;
using libRocketPlugin;
using Engine;
using ExCSS;

namespace Medical.GUI.RmlWysiwyg.ElementEditorComponents
{
    class StyleChoiceEditableProperty : StyleEditableProperty
    {
        private ChoiceObject choiceObject = new ChoiceObject();

        public StyleChoiceEditableProperty(String name, StyleDeclaration declaration, UnitType unitType, CreateBrowser browserBuildCallback = null)
            :base(name, declaration, unitType, browserBuildCallback)
        {
            
        }

        public StyleChoiceEditableProperty(String name, StyleDeclaration declaration, UnitType unitType, IEnumerable<Pair<String, Object>> choices, CreateBrowser browserBuildCallback = null)
            : base(name, declaration, unitType, browserBuildCallback)
        {
            addChoices(choices);
        }

        public void addChoice(String display, String value)
        {
            choiceObject.addChoice(display, value);
        }

        public void addChoices(IEnumerable<Pair<String, Object>> choices)
        {
            choiceObject.addChoices(choices);
        }

        public void removeChoice(String value)
        {
            choiceObject.removeChoice(value);
        }

        public override Type getPropertyType(int column)
        {
            if (column == 1)
            {
                return typeof(ChoiceObject);
            }
            return base.getPropertyType(column);
        }

        public override object getRealValue(int column)
        {
            if (column == 1)
            {
                return choiceObject;
            }
            return base.getRealValue(column);
        }
    }
}
