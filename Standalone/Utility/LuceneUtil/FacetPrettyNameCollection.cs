using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Medical.Utility.LuceneUtil
{
    /// <summary>
    /// This collection manages facet names to pretty names if you wish to display a facet with different
    /// text than you use to search. It is case sensitive.
    /// </summary>
    public class FacetPrettyNameCollection
    {
        class FacetFieldNames
        {
            Dictionary<String, String> prettyNames = new Dictionary<string,string>();

            public String PrettyName { get; set; }

            public void setPrettyName(String value, String prettyName)
            {
                if (prettyNames.ContainsKey(value))
                {
                    prettyNames[value] = prettyName;
                }
                else
                {
                    prettyNames.Add(value, prettyName);
                }
            }

            public String getPrettyName(String value)
            {
                String prettyName;
                if (prettyNames.TryGetValue(value, out prettyName))
                {
                    return prettyName;
                }
                return value;
            }
        }

        private Dictionary<String, FacetFieldNames> fieldNames = new Dictionary<string, FacetFieldNames>();

        /// <summary>
        /// Set the pretty name of a field. You do not have to do this to add value pretty names for the field.
        /// </summary>
        /// <param name="field"></param>
        /// <param name="prettyName"></param>
        public void setFieldPrettyName(String field, String prettyName)
        {
            FacetFieldNames names = getFieldNames(field);
            names.PrettyName = prettyName;
        }

        /// <summary>
        /// Set the pretty name of a field value.
        /// </summary>
        /// <param name="field"></param>
        /// <param name="value"></param>
        /// <param name="prettyName"></param>
        public void setValuePrettyName(String field, String value, String prettyName)
        {
            FacetFieldNames names = getFieldNames(field);
            names.setPrettyName(value, prettyName);
        }

        /// <summary>
        /// Get the pretty name of a field or just return the passed field value if it doesn't exist.
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public String getFieldPrettyName(String field)
        {
            FacetFieldNames names;
            if (fieldNames.TryGetValue(field, out names))
            {
                if (names.PrettyName != null)
                {
                    return names.PrettyName;
                }
            }
            return field;
        }

        /// <summary>
        /// Get the pretty name of a field value or just the passed value if it doesn't exist.
        /// </summary>
        /// <param name="field"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public String getValuePrettyName(String field, String value)
        {
            FacetFieldNames names;
            if (fieldNames.TryGetValue(field, out names))
            {
                return names.getPrettyName(value);
            }
            return value;
        }

        private FacetFieldNames getFieldNames(String group)
        {
            FacetFieldNames names;
            if (!fieldNames.TryGetValue(group, out names))
            {
                names = new FacetFieldNames();
                fieldNames.Add(group, names);
            }
            return names;
        }
    }
}