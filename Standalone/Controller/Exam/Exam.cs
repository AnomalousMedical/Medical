using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Engine;
using Engine.Attributes;

namespace Medical
{
    /// <summary>
    /// An exam is an arbitrary set of data that can be added to a patient file.
    /// </summary>
    public interface Exam : Saveable
    {
        DateTime Date { get; }

        String PrettyName { get; }
    }
}
