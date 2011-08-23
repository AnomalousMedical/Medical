using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Engine.Editing;

namespace Medical
{
    public class DataDrivenExamSection : Saveable
    {
        private String prettyName;
        private EditInterface editInterface;
        private Dictionary<String, Saveable> values;

        public DataDrivenExamSection(String prettyName)
        {
            this.prettyName = prettyName;
            values = new Dictionary<String, Saveable>();
        }

        public string PrettyName
        {
            get
            {
                return prettyName;
            }
        }

        public EditInterface EditInterface
        {
            get
            {
                if (editInterface == null)
                {
                    editInterface = new EditInterface(prettyName);
                }
                return editInterface;
            }
        }

        protected DataDrivenExamSection(LoadInfo info)
        {
            prettyName = info.GetValue("PrettyName", prettyName);
            info.RebuildDictionary<String, Saveable>("ExamValue", values);
        }

        public void getInfo(SaveInfo info)
        {
            info.AddValue("PrettyName", prettyName);
            info.ExtractDictionary<String, Saveable>("ExamValue", values);
        }
    }
}
