using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Engine;
using Engine.Attributes;
using Engine.Editing;

namespace Medical
{
    /// <summary>
    /// An exam is an arbitrary set of data that can be added to a patient file.
    /// </summary>
    public interface Exam : Saveable
    {
        /// <summary>
        /// This method will make the exam show a gui that shows a breakdown of the exam.
        /// </summary>
        void showBreakdownGUI();

        /// <summary>
        /// The Date an exam was taken.
        /// </summary>
        DateTime Date { get; }

        /// <summary>
        /// The pretty name of the exam for display on the UI.
        /// </summary>
        String PrettyName { get; }

        /// <summary>
        /// An EditInterface for the exam.
        /// </summary>
        EditInterface EditInterface { get; }
    }
}
