using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace Medical
{
    /// <summary>
    /// This interface allows an object to be selected in the
    /// SelectionController and be updated by it.
    /// </summary>
    public interface SelectableObject
    {
        /// <summary>
        /// Edit the position.
        /// </summary>
        /// <param name="translation">The new translation.</param>
        /// <param name="rotation">The ne rotation.</param>
        void editPosition(ref Vector3 translation, ref Quaternion rotation);

        /// <summary>
        /// Edit the translation.
        /// </summary>
        /// <param name="translation">The new translation to set.</param>
        void editTranslation(ref Vector3 translation);

        /// <summary>
        /// Edit the rotation.
        /// </summary>
        /// <param name="rotation">The new rotation to set.</param>
        void editRotation(ref Quaternion rotation);

        /// <summary>
        /// Get the rotation of the object.
        /// </summary>
        /// <returns>The rotation.</returns>
        Quaternion getRotation();

        /// <summary>
        /// Get the translation of the object.
        /// </summary>
        /// <returns>The translation.</returns>
        Vector3 getTranslation();
    }
}
