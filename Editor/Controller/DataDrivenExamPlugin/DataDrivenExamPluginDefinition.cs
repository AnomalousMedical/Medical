using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Engine.Editing;

namespace Medical
{
    partial class DataDrivenExamPluginDefinition : Saveable
    {
        public DataDrivenExamPluginDefinition()
        {
            
        }

        [Editable]
        public String OutputFile { get; set; }

        protected DataDrivenExamPluginDefinition(LoadInfo info)
        {
            OutputFile = info.GetString("OutputFile");
        }

        public void getInfo(SaveInfo info)
        {
            info.AddValue("OutputFile", OutputFile);
        }
    }

    partial class DataDrivenExamPluginDefinition
    {
        private EditInterface editInterface;

        public EditInterface EditInterface
        {
            get
            {
                if (editInterface == null)
                {
                    editInterface = ReflectedEditInterface.createEditInterface(this, ReflectedEditInterface.DefaultScanner, "Exam Plugin", null);
                }
                return editInterface;
            }
        }
    }
}
