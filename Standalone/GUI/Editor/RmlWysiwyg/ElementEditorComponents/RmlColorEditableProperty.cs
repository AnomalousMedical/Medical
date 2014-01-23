using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.GUI.RmlWysiwyg.ElementEditorComponents
{
    class RmlColorEditableProperty : RmlEditableProperty
    {
        public RmlColorEditableProperty(String name, Color value, CreateBrowser browserBuildCallback = null)
            : base(name, String.Format("#{0:X6}", value.toRGB()), browserBuildCallback)
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
                return Color.FromRGBAString(Value);
            }
            return base.getRealValue(column);
        }

        public override void setValue(int column, object value)
        {
            if (column == 1)
            {
                this.Value = String.Format("#{0:X6}", ((Color)value).toRGB());
                fireValueChanged();
            }
            else
            {
                base.setValue(column, value);
            }
        }
    }
}
