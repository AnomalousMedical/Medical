using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;
using Engine.Reflection;

namespace Medical
{
    public class LayerStateEditableProperty : EditableProperty
    {
        private Object owner;
        private MemberWrapper memberWrapper;

        public LayerStateEditableProperty(Object owner, MemberWrapper memberWrapper)
        {
            this.owner = owner;
            this.memberWrapper = memberWrapper;
        }

        public bool canParseString(int column, string value, out string errorMessage)
        {
            errorMessage = "Does not take string values, but returns true anyway.";
            return true;
        }

        public Type getPropertyType(int column)
        {
            switch (column)
            {
                case 0:
                    return typeof(String);
                case 1:
                    return typeof(LayerState);
            }
            return null;
        }

        public string getValue(int column)
        {
            switch (column)
            {
                case 0:
                    return memberWrapper.getWrappedName();
                case 1:
                    LayerState layerState = LayerState;
                    if (layerState != null)
                    {
                        return LayerState.ToString();
                    }
                    return null;
            }
            return null;
        }

        public Object getRealValue(int column)
        {
            switch (column)
            {
                case 0:
                    return memberWrapper.getWrappedName();
                case 1:
                    LayerState layerState = LayerState;
                    if (layerState != null)
                    {
                        return LayerState;
                    }
                    return null;
            }
            return null;
        }

        public void setValue(int column, Object value)
        {

        }

        public void setValueStr(int column, string value)
        {
            
        }

        public LayerState LayerState
        {
            get
            {
                return (LayerState)memberWrapper.getValue(owner, null);
            }
            set
            {
                memberWrapper.setValue(owner, value, null);
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
        public bool Advanced { get; set; }
    }
}
