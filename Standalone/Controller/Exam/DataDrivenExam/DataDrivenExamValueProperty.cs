using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;

namespace Medical
{
    class DataDrivenExamValueProperty : EditableProperty
    {
        private String key;
        private Object value;

        public DataDrivenExamValueProperty(String key, Object value)
        {
            this.key = key;
            this.value = value;
        }

        public bool canParseString(int column, string value, out string errorMessage)
        {
            errorMessage = "";
            return false;
        }

        public Type getPropertyType(int column)
        {
            return typeof(String);
        }

        public string getValue(int column)
        {
            switch (column)
            {
                case 0:
                    return key;
                case 1:
                    return value.ToString();
                default:
                    return "";
            }
        }

        public Object getRealValue(int column)
        {
            switch (column)
            {
                case 0:
                    return key;
                case 1:
                    return value;
                default:
                    return "";
            }
        }

        public void setValue(int column, Object value)
        {

        }

        public void setValueStr(int column, string value)
        {

        }

        public bool hasBrowser(int column)
        {
            return false;
        }

        public Browser getBrowser(int column, EditUICallback uiCallback)
        {
            return null;
        }

        public bool readOnly(int column)
        {
            return column != 0;
        }

        /// <summary>
        /// Set this to true to indicate to the ui that this property is advanced.
        /// </summary>
        public bool Advanced
        {
            get
            {
                return false;
            }
        }
    }
}
