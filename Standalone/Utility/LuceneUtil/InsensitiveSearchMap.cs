using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical.Utility.LuceneUtil
{
    class InsensitiveSearchMap
    {
        class FieldInfo
        {
            private Dictionary<String, String> valueMap = new Dictionary<string, string>();

            public void addValue(String value)
            {
                String loweredValue = value.ToLowerInvariant();
                if (!valueMap.ContainsKey(loweredValue))
                {
                    valueMap.Add(loweredValue, value);
                }
            }

            public bool getValue(String value, out String csValue)
            {
                return valueMap.TryGetValue(value.ToLowerInvariant(), out csValue);
            }

            public String Field { get; set; }
        }

        private Dictionary<String, FieldInfo> fieldMap = new Dictionary<String, FieldInfo>();

        public void addFacet(String field, String value)
        {
            FieldInfo fieldInfo;
            String loweredField = field.ToLowerInvariant();
            if (!fieldMap.TryGetValue(loweredField, out fieldInfo))
            {
                fieldInfo = new FieldInfo()
                {
                    Field = field
                };
                fieldMap.Add(loweredField, fieldInfo);
            }
            fieldInfo.addValue(value);
        }

        public bool getCaseSensitiveFacet(String field, String value, out String csField, out String csValue)
        {
            FieldInfo fieldInfo;
            if (fieldMap.TryGetValue(field.ToLowerInvariant(), out fieldInfo))
            {
                csField = fieldInfo.Field;
                if (!fieldInfo.getValue(value, out csValue))
                {
                    csValue = value; //Not found, but search anyway
                }
                return true;
            }
            csField = null;
            csValue = null;
            return false;
        }
    }
}
