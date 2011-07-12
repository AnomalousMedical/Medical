﻿using System;
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
    public class AbstractExam<ExamType> : Exam
        where ExamType : Exam, new()
    {

        #region Static

        static ExamType currentExam;

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
        /// Save this exam and clear the current exam.
        /// </summary>
        public static void saveAndClear()
        {
            ExamController.Instance.addExam(Current);
            clear();
        }

        /// <summary>
        /// Clear the current exam without saving it.
        /// </summary>
        public static void clear()
        {
            currentExam = default(ExamType);
        }

        #endregion Static

        [DoNotSave]
        private DateTime date;

        [DoNotSave]
        private String prettyName;

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

        public virtual void showBreakdownGUI()
        {
            GenericExamOverview viewer = new GenericExamOverview(this);
            viewer.open(false);
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

        #region Saveable Members

        protected AbstractExam(LoadInfo info)
        {
            date = new DateTime(info.GetInt64("ExamReserved_Date"));
            prettyName = info.GetString("ExamReserved_PrettyName");
            ReflectedSaver.RestoreObject(this, info, ExamSaveMemberScanner.Scanner);
        }

        public void getInfo(SaveInfo info)
        {
            info.AddValue("ExamReserved_Date", date.Ticks);
            info.AddValue("ExamReserved_PrettyName", prettyName);
            ReflectedSaver.SaveObject(this, info, ExamSaveMemberScanner.Scanner);
        }

        #endregion
    }
}
