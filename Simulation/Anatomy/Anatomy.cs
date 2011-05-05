using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace Medical
{
    /// <summary>
    /// This interface allows for the manipulation of anatomy.
    /// </summary>
    public interface Anatomy
    {
        /// <summary>
        /// The name of this anatomical structure.
        /// </summary>
        String AnatomicalName { get; }

        /// <summary>
        /// The list of UI commands for this anatomical structure.
        /// </summary>
        IEnumerable<AnatomyCommand> Commands { get; }

        /// <summary>
        /// The location of the center of this anatomy.
        /// </summary>
        Vector3 Center { get; }

        /// <summary>
        /// The bounding radius of this object.
        /// </summary>
        float BoundingRadius { get; }

        /// <summary>
        /// The class that changes transparency for this Anatomy.
        /// </summary>
        TransparencyChanger TransparencyChanger { get; }
    }
}
