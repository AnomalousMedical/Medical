using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace Medical
{
    public interface MovableObject
    {
        /// <summary>
        /// Get/Set the position of this object.
        /// </summary>
        Vector3 ToolTranslation { get; }

        void move(Vector3 offset);

        /// <summary>
        /// Get/Set the rotation of this object.
        /// </summary>
        Quaternion ToolRotation { get; set; }

        /// <summary>
        /// Determine if the tools should be shown. True to show false to hide.
        /// </summary>
        bool ShowTools { get; }
    }
}
