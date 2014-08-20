using Engine.Editing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical
{
    class AtlasDependencyResourceEditableProperty : EditableProperty
    {
        private const int PATH_COLUMN = 0;
        private const int RECURSIVE_COLUMN = 1;

        internal static readonly EditablePropertyInfo Info = new EditablePropertyInfo();

        static AtlasDependencyResourceEditableProperty()
        {
            Info.addColumn(new EditablePropertyColumn("Location", false));
            Info.addColumn(new EditablePropertyColumn("Recursive", false));
        }

        private AtlasDependencyResource resource;

        public AtlasDependencyResourceEditableProperty(AtlasDependencyResource resource)
        {
            this.resource = resource;
        }

        /// <summary>
        /// Get the value for a given column.
        /// </summary>
        /// <param name="column">The column to get the value for.</param>
        /// <returns></returns>
        public string getValue(int column)
        {
            switch (column)
            {
                case PATH_COLUMN:
                    return resource.Path;
                case RECURSIVE_COLUMN:
                    return resource.Recursive.ToString();
            }
            throw new EditException(String.Format("Attempted to get a column from a Resource {0} that is not valid.", column));
        }

        public Object getRealValue(int column)
        {
            switch (column)
            {
                case PATH_COLUMN:
                    return resource.Path;
                case RECURSIVE_COLUMN:
                    return resource.Recursive;
            }
            throw new EditException(String.Format("Attempted to get a column from a Resource {0} that is not valid.", column));
        }

        public void setValue(int column, Object value)
        {
            switch (column)
            {
                case PATH_COLUMN:
                    resource.Path = (String)value;
                    break;
                case RECURSIVE_COLUMN:
                    resource.Recursive = (bool)value;
                    break;
                default:
                    throw new EditException(String.Format("Attempted to set a column from a Resource {0} that is not valid.", column));
            }
        }

        /// <summary>
        /// Set the value of this property from a string.
        /// </summary>
        /// <param name="column"></param>
        /// <param name="value"></param>
        public void setValueStr(int column, string value)
        {
            switch (column)
            {
                case PATH_COLUMN:
                    resource.Path = value;
                    break;
                case RECURSIVE_COLUMN:
                    resource.Recursive = bool.Parse(value);
                    break;
                default:
                    throw new EditException(String.Format("Attempted to set a column from a Resource {0} that is not valid.", column));
            }
        }

        /// <summary>
        /// Determine if the given string is in the correct format for this
        /// property to parse.
        /// </summary>
        /// <param name="column"></param>
        /// <param name="value">The value to try to parse.</param>
        /// <param name="errorMessage">An error message if the function returns false.</param>
        /// <returns>True if the string can be parsed.</returns>
        public bool canParseString(int column, string value, out string errorMessage)
        {
            switch (column)
            {
                case PATH_COLUMN:
                    errorMessage = null;
                    return true;
                case RECURSIVE_COLUMN:
                    errorMessage = "Cannot evaluate recursive value as a bool";
                    bool result;
                    return bool.TryParse(value, out result);
            }
            throw new EditException(String.Format("Attempted to validate a column from a Resource {0} that is not valid.", column));
        }

        /// <summary>
        /// Get the type of this property's target object.
        /// </summary>
        /// <param name="column"></param>
        /// <returns>The Type of the object this property will set.</returns>
        public Type getPropertyType(int column)
        {
            switch (column)
            {
                case PATH_COLUMN:
                    return typeof(String);
                case RECURSIVE_COLUMN:
                    return typeof(bool);
            }
            throw new EditException(String.Format("Attempted to get a column type from a Resource {0} that is not valid.", column));
        }

        public Browser getBrowser(int column, EditUICallback uiCallback)
        {
            return null;
        }

        public bool hasBrowser(int column)
        {
            return false;
        }

        public bool readOnly(int column)
        {
            return column != PATH_COLUMN;
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
