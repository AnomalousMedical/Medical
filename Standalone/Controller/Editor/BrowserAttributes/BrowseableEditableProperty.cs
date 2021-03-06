﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;
using Engine.Reflection;

namespace Medical.Editor
{
    /// <summary>
    /// An editable property for timelines to be used with the standard two column edit table layout.
    /// </summary>
    public abstract class BrowseableEditableProperty : EditableProperty
    {
        private MemberWrapper propertyInfo;
        private Object instance;
        private String name;

        public BrowseableEditableProperty(String name, MemberWrapper propertyInfo, Object instance)
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

        public Browser getBrowser(int column, EditUICallback uiCallback)
        {
            switch(column)
            {
                case 0:
                    return null;
                case 1:
                    return buildBrowser(uiCallback);
                default:
                    return null;
            }
        }

        protected abstract Browser buildBrowser(EditUICallback uiCallback);

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
                    Object value = propertyInfo.getValue(instance, null);
                    if(value != null)
                    {
                        return value.ToString();
                    }
                    return null;
                default:
                    return null;
            }
        }

        public Object getRealValue(int column)
        {
            switch (column)
            {
                case 0:
                    return name;
                case 1:
                    Object value = propertyInfo.getValue(instance, null);
                    if (value != null)
                    {
                        return value;
                    }
                    return null;
                default:
                    return null;
            }
        }

        public void setValue(int column, Object value)
        {
            switch (column)
            {
                case 0:
                    name = (String)value;
                    break;
                case 1:
                    propertyInfo.setValue(instance, value, null);
                    break;
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

        public bool readOnly(int column)
        {
            switch (column)
            {
                case 0:
                    return true;
                case 1:
                    return DataReadOnly;
                default:
                    return true;
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

        /// <summary>
        /// Set this to true to indicate to the ui that this property is advanced.
        /// </summary>
        public bool Advanced { get; set; }

        public bool DataReadOnly { get; set; }
    }
}
