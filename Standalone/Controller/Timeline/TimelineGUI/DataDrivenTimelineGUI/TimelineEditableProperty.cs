using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;
using Engine.Reflection;

namespace Medical
{
    /// <summary>
    /// An editable property for timelines to be used with the standard two column edit table layout.
    /// </summary>
    class TimelineEditableProperty : EditableProperty
    {
        private MemberWrapper propertyInfo;
        private Object instance;
        private String name;

        public TimelineEditableProperty(String name, MemberWrapper propertyInfo, Object instance)
        {
            this.name = name;
            this.propertyInfo = propertyInfo;
            this.instance = instance;
        }

        public bool canParseString(int column, string value, out string errorMessage)
        {
            errorMessage = "";
            return true;
        }

        public Browser getBrowser(int column)
        {
            switch(column)
            {
                case 0:
                    return null;
                case 1:
                    return TimelineBrowserController.createBrowser();
                default:
                    return null;
            }
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
                    return name;
                case 1:
                    return propertyInfo.getValue(instance, null).ToString();
                default:
                    return null;
            }
        }

        public bool hasBrowser(int column)
        {
            switch (column)
            {
                case 0:
                    return false;
                case 1:
                    return true;
                default:
                    return false;
            }
        }

        public void setValueStr(int column, string value)
        {
            switch (column)
            {
                case 0:
                    name = value;
                    break;
                case 1:
                    propertyInfo.setValue(instance, value, null);
                    break;
            }
        }
    }
}
