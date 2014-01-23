using Engine;
using ExCSS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.GUI.RmlWysiwyg.ElementEditorComponents
{
    class StyleColorEditableProperty : StyleEditableProperty
    {
        public StyleColorEditableProperty(String name, StyleDeclaration declaration, CreateBrowser browserBuildCallback = null)
            : base(name, declaration, UnitType.RGB, browserBuildCallback)
        {
            
        }

        public override Type getPropertyType(int column)
        {
            if (column == 1)
            {
                return typeof(Color);
            }
            return base.getPropertyType(column);
        }

        public override object getRealValue(int column)
        {
            if (column == 1)
            {
                return getColor(Property);
            }
            return base.getRealValue(column);
        }

        public override void setValue(int column, object value)
        {
            if (column == 1)
            {
                base.setValueStr(column, String.Format("#{0:X6}", ((Color)value).toRGB()));
            }
            else
            {
                base.setValue(column, value);
            }
        }

        private Color getColor(Property property)
        {
            if (property != null)
            {
                Term term = property.Term;
                if (term != null)
                {
                    if (property.Term.RuleValueType == RuleValueType.ValueList)
                    {
                        term = ((TermList)term).Item(0);
                    }
                    if (term.RuleValueType == RuleValueType.PrimitiveValue)
                    {
                        PrimitiveTerm primitiveTerm = (PrimitiveTerm)term;
                        if (primitiveTerm.PrimitiveType == UnitType.RGB)
                        {
                            HtmlColor color = (HtmlColor)primitiveTerm.Value;
                            return Color.FromARGB(color.A, color.R, color.G, color.B);
                        }
                    }
                }
            }
            return new Color(0, 0, 0, 0);
        }

    }
}
