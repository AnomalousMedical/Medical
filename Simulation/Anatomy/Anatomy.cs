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
        /// Get an AxisAlignedBox bounding volume.
        /// </summary>
        AxisAlignedBox WorldBoundingBox { get; }

        /// <summary>
        /// The class that changes transparency for this Anatomy.
        /// </summary>
        TransparencyChanger TransparencyChanger { get; }

        /// <summary>
        /// The direction to move the camera when generating a preview for this anatomy.
        /// </summary>
        Vector3 PreviewCameraDirection { get; }

        /// <summary>
        /// True if the anatomy is a group, false if it is an individual item.
        /// </summary>
        bool IsGroup { get; }
    }
}
