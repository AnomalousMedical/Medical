using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;

namespace Medical
{
    public interface ExamSection : Saveable
    {
        /// <summary>
        /// The pretty name of the exam for display on the UI.
        /// </summary>
        String PrettyName { get; }
    }
}
