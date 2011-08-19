using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Attributes;
using Engine.Saving;
using Engine;
using Medical.GUI;
using Engine.Editing;
using Engine.Reflection;

namespace Medical
{
    /// <summary>
    /// This class provides a helpful default implementation for exams. To use
    /// it inherit this class and add your properties as needed. This class will
    /// handle saving them.
    /// 
    /// It also provides helper methods to easily create a new instance and to
    /// commit this instance to the ExamController and delete the local
    /// reference.
    /// </summary>
    /// <typeparam name="ExamType"></typeparam>
    public abstract class AbstractExam<ExamType> : Exam
        where ExamType : AbstractExam<ExamType>, new()
    {

        #region Static

        static ExamType currentExam;
        static ExamType previousExamVersion;
        static CopySaver copySaver = new CopySaver();

        /// <summary>
        /// The current working instance of this exam.
        /// </summary>
        public static ExamType Current
        {
            get
            {
                if (currentExam == null)
                {
                    currentExam = new ExamType();
                }
                return currentExam;
            }
        }

        /// <summary>
        /// Open the given exam for review. Will create a copy of the exam. Upon calling saveAndClear this copy will replace the original.
        /// </summary>
        /// <param name="exam">The exam to review.</param>
        public static void openForReview(ExamType exam)
        {
            previousExamVersion = exam;
            exam.savePreviousExams = false;
            currentExam = copySaver.copy<ExamType>(exam);
            exam.savePreviousExams = true;
            currentExam.date = DateTime.Now;
        }

        /// <summary>
        /// Save this exam and clear the current exam.
        /// </summary>
        public static void saveAndClear()
        {
            if (previousExamVersion != null)
            {
                ExamController.Instance.replaceExam(previousExamVersion, Current);
                Current.previousExam = previousExamVersion;
            }
            else
            {
                ExamController.Instance.addExam(Current);
            }
            clear();
        }

        /// <summary>
        /// Clear the current exam without saving it.
        /// </summary>
        public static void clear()
        {
            currentExam = default(ExamType);
            previousExamVersion = default(ExamType);
        }

        #endregion Static

        [DoNotSave]
        private DateTime date;

        [DoNotSave]
        private String prettyName;

        [DoNotSave]
        private AbstractExam<ExamType> previousExam;

        [DoNotSave]
        private bool savePreviousExams = true; //This can be turned off when the exam is being copied to save pointlessly copying the previous editions with the copy saver.

        [DoNotCopy]
        private EditInterface editInterface;

        /// <summary>
        /// Constructor, takes a pretty name for the exam.
        /// </summary>
        /// <param name="prettyName">The name of the exam.</param>
        public AbstractExam(String prettyName)
        {
            date = DateTime.Now;
            this.prettyName = prettyName;
        }

        [Hidden]
        public DateTime Date
        {
            get
            {
                return date;
            }
        }

        [Hidden]
        public String PrettyName
        {
            get
            {
                return prettyName;
            }
        }

        [Hidden]
        public virtual EditInterface EditInterface
        {
            get
            {
                if (editInterface == null)
                {
                    editInterface = ReflectedEditInterface.createEditInterface(this, AbstractExamMemberScanner.MemberScanner, prettyName, null);
                }
                return editInterface;
            }
        }

        public Exam PreviousExam
        {
            get
            {
                return previousExam;
            }
        }

        [Hidden]
        public abstract ExamAnalyzerCollection Analyzers { get; }

        #region Saveable Members

        protected AbstractExam(LoadInfo info)
        {
            date = new DateTime(info.GetInt64("ExamReserved_Date"));
            prettyName = info.GetString("ExamReserved_PrettyName");
            previousExam = info.GetValue<AbstractExam<ExamType>>("ExamReserved_Previous", previousExam);
            ReflectedSaver.RestoreObject(this, info, ExamSaveMemberScanner.Scanner);
        }

        public void getInfo(SaveInfo info)
        {
            info.AddValue("ExamReserved_Date", date.Ticks);
            info.AddValue("ExamReserved_PrettyName", prettyName);
            if (savePreviousExams)
            {
                info.AddValue("ExamReserved_Previous", previousExam);
            }
            ReflectedSaver.SaveObject(this, info, ExamSaveMemberScanner.Scanner);
        }

        #endregion
    }
}
