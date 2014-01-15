using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;
using libRocketPlugin;
using Engine;

namespace Medical.GUI.RmlWysiwyg.ElementEditorComponents
{
    class RmlChoiceEditableProperty : RmlEditableProperty
    {
        private ChoiceObject choiceObject = new ChoiceObject();

        public RmlChoiceEditableProperty(String name, String value, CreateBrowser browserBuildCallback = null)
            :base(name, value, browserBuildCallback)
        {
            
        }

        public RmlChoiceEditableProperty(String name, String value, IEnumerable<Pair<String, Object>> choices, CreateBrowser browserBuildCallback = null)
            : base(name, value, browserBuildCallback)
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
