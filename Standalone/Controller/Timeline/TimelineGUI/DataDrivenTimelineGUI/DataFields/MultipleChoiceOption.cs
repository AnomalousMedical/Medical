using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Engine.Editing;

namespace Medical
{
    public class MultipleChoiceOption : EditableProperty, Saveable
    {
        private String optionText;

        public MultipleChoiceOption()
        {

        }

        public String OptionText
        {
            get
            {
                return optionText;
            }
            set
            {
                optionText = value;
            }
        }

        public bool canParseString(int column, string value, out string errorMessage)
        {
            errorMessage = "";
            return true;
        }

        public Type getPropertyType(int column)
        {
            return typeof(String);
        }

        public string getValue(int column)
        {
            return optionText;
        }

        public void setValueStr(int column, string value)
        {
            optionText = value;
        }

        protected MultipleChoiceOption(LoadInfo info)
        {
            optionText = info.GetString("OptionText", optionText);
        }

        public void getInfo(SaveInfo info)
        {
            info.AddValue("OptionText", optionText);
        }

        public bool hasBrowser(int column)
        {
            return false;
        }

        public Browser getBrowser(int column)
        {
            return null;
        }
    }
}
