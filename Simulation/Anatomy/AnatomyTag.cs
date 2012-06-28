using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;
using Engine.Saving;

namespace Medical
{
    public class AnatomyTag : Saveable, EditableProperty
    {
        public AnatomyTag()
        {

        }

        public String Tag { get; set; }

        #region EditableProperty Members

        internal static readonly EditablePropertyInfo Info = new EditablePropertyInfo();

        static AnatomyTag()
        {
            Info.addColumn(new EditablePropertyColumn("Tag", false));
        }

        public bool canParseString(int column, string value, out string errorMessage)
        {
            switch (column)
            {
                case 0:
                    errorMessage = null;
                    return true;
                default:
                    errorMessage = String.Format("Invalid column {0}", column);
                    return false;
            }
        }

        public Type getPropertyType(int column)
        {
            switch (column)
            {
                case 0:
                    return typeof(String);
                default:
                    return null;
            }
        }

        public string getValue(int column)
        {
            switch (column)
            {
                case 0:
                    return Tag;
                default:
                    return null;
            }
        }

        public Object getRealValue(int column)
        {
            switch (column)
            {
                case 0:
                    return Tag;
                default:
                    return null;
            }
        }

        public void setValue(int column, Object value)
        {
            switch (column)
            {
                case 0:
                    Tag = (String)value;
                    break;
            }
        }

        public void setValueStr(int column, string value)
        {
            switch (column)
            {
                case 0:
                    Tag = value;
                    break;
            }
        }

        public bool hasBrowser(int column)
        {
            return false;
        }

        public Browser getBrowser(int column, EditUICallback uiCallback)
        {
            return null;
        }

        #endregion

        #region Saveable Members

        public void getInfo(SaveInfo info)
        {
            info.AddValue("Tag", Tag);
        }

        protected AnatomyTag(LoadInfo info)
        {
            Tag = info.GetString("Tag");
        }

        #endregion
    }
}
