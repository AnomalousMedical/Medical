using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Engine.Editing;
using Engine.Attributes;

namespace Medical
{
    public class AbstractExamSection : ExamSection, EditInterfaceOverride
    {
        private String prettyName;

        [DoNotCopy]
        private EditInterface editInterface;

        public AbstractExamSection(String prettyName)
        {
            this.prettyName = prettyName;
        }

        public EditInterface getEditInterface(string memberName, Engine.Reflection.MemberScanner scanner)
        {
            if (editInterface == null)
            {
                editInterface = ReflectedEditInterface.createEditInterface(this, AbstractExamMemberScanner.MemberScanner, prettyName, null);
            }
            return editInterface;
        }

        [Hidden]
        public string PrettyName
        {
            get
            {
                return prettyName;
            }
        }

        #region Saveable Members

        protected AbstractExamSection(LoadInfo info)
        {
            prettyName = info.GetString("ExamSectionReserved_PrettyName");
            ReflectedSaver.RestoreObject(this, info, ExamSaveMemberScanner.Scanner);
        }

        public void getInfo(SaveInfo info)
        {
            info.AddValue("ExamSectionReserved_PrettyName", prettyName);
            ReflectedSaver.SaveObject(this, info, ExamSaveMemberScanner.Scanner);
        }

        #endregion
    }
}
