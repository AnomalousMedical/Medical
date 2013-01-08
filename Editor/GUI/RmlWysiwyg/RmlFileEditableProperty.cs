using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;
using libRocketPlugin;

namespace Medical.GUI
{
    class RmlFileEditableProperty : EditableProperty
    {
        private RmlEditableProperty rmlEditableProperty;

        public RmlFileEditableProperty(String name, String value, Element element, RmlEditableProperty.CreateBrowser browserBuildCallback = null)
        {
            rmlEditableProperty = new RmlEditableProperty(name, value, element, browserBuildCallback);
        }

        public bool canParseString(int column, string value, out string errorMessage)
        {
            if (rmlEditableProperty.canParseString(column, value, out errorMessage))
            {
                if (Core.GetFileInterface().Exists(value, rmlEditableProperty.ElementDocumentSourcePath))
                {
                    return true;
                }
                else
                {
                    errorMessage = String.Format("Cannot find file {0}", value);
                }
            }
            return false;
        }

        public Browser getBrowser(int column, EditUICallback uiCallback)
        {
            return rmlEditableProperty.getBrowser(column, uiCallback);
        }

        public Type getPropertyType(int column)
        {
            return rmlEditableProperty.getPropertyType(column);
        }

        public object getRealValue(int column)
        {
            return rmlEditableProperty.getRealValue(column);
        }

        public string getValue(int column)
        {
            return rmlEditableProperty.getValue(column);
        }

        public bool hasBrowser(int column)
        {
            return rmlEditableProperty.hasBrowser(column);
        }

        public void setValue(int column, object value)
        {
            rmlEditableProperty.setValue(column, value);
        }

        public void setValueStr(int column, string value)
        {
            rmlEditableProperty.setValueStr(column, value);
        }
    }
}
