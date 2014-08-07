using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;
using Engine.Saving;

namespace Medical
{
    public class AnatomyTag : Saveable
    {
        public AnatomyTag()
        {

        }

        public String Tag { get; set; }

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

        public class Property : EditableProperty
        {
            internal static readonly EditablePropertyInfo Info = new EditablePropertyInfo();

            static Property()
            {
                Info.addColumn(new EditablePropertyColumn("Tag", false));
            }

            private AnatomyTag tag;
            public Property(AnatomyTag tag)
            {
                this.tag = tag;
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
                        return tag.Tag;
                    default:
                        return null;
                }
            }

            public Object getRealValue(int column)
            {
                switch (column)
                {
                    case 0:
                        return tag.Tag;
                    default:
                        return null;
                }
            }

            public void setValue(int column, Object value)
            {
                switch (column)
                {
                    case 0:
                        tag.Tag = (String)value;
                        break;
                }
            }

            public void setValueStr(int column, string value)
            {
                switch (column)
                {
                    case 0:
                        tag.Tag = value;
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

            public bool readOnly(int column)
            {
                return false;
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
}
