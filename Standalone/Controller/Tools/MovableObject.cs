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

        /// <summary>
        /// Move the object by offset.
        /// </summary>
        /// <param name="offset">The offset to move.</param>
        void move(Vector3 offset);

        /// <summary>
        /// Get/Set the rotation of this object.
        /// </summary>
        Quaternion ToolRotation { get; }

        /// <summary>
        /// Determine if the tools should be shown. True to show false to hide.
        /// </summary>
        bool ShowTools { get; }

        /// <summary>
        /// Rotate the object to newRot.
        /// </summary>
        /// <param name="newRot">The new rotation.</param>
        void rotate(Quaternion newRot);

        /// <summary>
        /// This is called when the tool gains and looses its highlight. It can
        /// be used to make the object appear to change as well when the tool is
        /// highlighted.
        /// </summary>
        /// <param name="highlighted"></param>
        void alertToolHighlightStatus(bool highlighted);
    }
}
