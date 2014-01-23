using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;
using libRocketPlugin;
using ExCSS;

namespace Medical.GUI.RmlWysiwyg.ElementEditorComponents
{
    class StyleEditableProperty : EditableProperty
    {
        public delegate Browser CreateBrowser(EditUICallback uiCallback);

        private String name;
        //private String value;
        private CreateBrowser browserBuildCallback;
        private StyleDeclaration declaration;
        private Property property;
        private UnitType unitType;

        public event Action<StyleEditableProperty> ValueChanged;

        public StyleEditableProperty(String name, StyleDeclaration declaration, UnitType unitType, CreateBrowser browserBuildCallback = null)
        {
            this.name = name;
            this.declaration = declaration;
            this.browserBuildCallback = browserBuildCallback;
            this.unitType = unitType;

            property = declaration.Properties.FirstOrDefault(p => p.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
            if (property == null)
            {
                property = new Property(name);
            }
        }

        public bool canParseString(int column, string value, out string errorMessage)
        {
            errorMessage = null;
            return true;
        }

        public Browser getBrowser(int column, EditUICallback uiCallback)
        {
            switch (column)
            {
                case 0:
                    return null;
                case 1:
                    return browserBuildCallback(uiCallback);
            }
            return null;
        }

        public virtual Type getPropertyType(int column)
        {
            return typeof(String);
        }

        public virtual object getRealValue(int column)
        {
            switch (column)
            {
                case 0:
                    return name;
                case 1:
                    if (property.Term != null)
                    {
                        return property.Term.ToString();
                    }
                    return null;
            }
            return null;
        }

        public string getValue(int column)
        {
            switch (column)
            {
                case 0:
                    return name;
                case 1:
                    if (property.Term != null)
                    {
                        return property.Term.ToString();
                    }
                    return null;
            }
            return null;
        }

        public bool hasBrowser(int column)
        {
            switch (column)
            {
                case 0:
                    return false;
                case 1:
                    return browserBuildCallback != null;
            }
            return false;
        }

        public bool readOnly(int column)
        {
            return column != 0;
        }

        public virtual void setValue(int column, object value)
        {
            String strValue = null;
            if(value != null)
            {
                strValue = value.ToString();
            }
            setValueStr(column, strValue);
        }

        public void setValueStr(int column, string value)
        {
            switch (column)
            {
                case 0:
                    name = value;
                    break;
                case 1:
                    if (String.IsNullOrEmpty(value))
                    {
                        property.Term = null;
                        if (declaration.Contains(property))
                        {
                            declaration.Remove(property);
                        }
                    }
                    else
                    {
                        property.Term = new PrimitiveTerm(unitType, value);
                        if (!declaration.Contains(property))
                        {
                            declaration.Add(property);
                        }                        
                    }
                    break;
            }
            fireValueChanged();
        }

        public String Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }

        /// <summary>
        /// Set this to true to indicate to the ui that this property is advanced.
        /// </summary>
        public bool Advanced { get; set; }

        protected void fireValueChanged()
        {
            if (ValueChanged != null)
            {
                ValueChanged.Invoke(this);
            }
        }
    }
}
