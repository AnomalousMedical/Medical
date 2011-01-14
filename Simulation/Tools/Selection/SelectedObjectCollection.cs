using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace Medical
{
    /// <summary>
    /// This class manages the selected objects and can perform rotation and translation
    /// on them.
    /// </summary>
    class SelectedObjectCollection
    {
        private List<SelectableObject> selectedObjects = new List<SelectableObject>();
        private List<Vector3> translationOffsets = new List<Vector3>();
        private List<Quaternion> rotationOffsets = new List<Quaternion>();
        private Vector3 translation = Vector3.Zero;
        private Quaternion rotation = Quaternion.Identity;

        /// <summary>
        /// Constructor, restricted to internal use only.
        /// </summary>
        internal SelectedObjectCollection()
        {

        }

        /// <summary>
        /// Add an object to the collection.
        /// </summary>
        /// <param name="toAdd">The object to add.</param>
        internal void _addObject(SelectableObject toAdd)
        {
            if (!selectedObjects.Contains(toAdd))
            {
                selectedObjects.Add(toAdd);
                computeOffsets();
            }
        }

        /// <summary>
        /// Add several objects to the collection.
        /// </summary>
        /// <param name="range">An enumerable with the objects to add.</param>
        internal void _addRange(IEnumerable<SelectableObject> range)
        {
            selectedObjects.AddRange(range);
        }

        /// <summary>
        /// Remove an object from the collection.
        /// </summary>
        /// <param name="toRemove">The object to remove.</param>
        internal void _removeObject(SelectableObject toRemove)
        {
            selectedObjects.Remove(toRemove);
            computeOffsets();
        }

        /// <summary>
        /// Clear the selection.
        /// </summary>
        internal void _clear()
        {
            selectedObjects.Clear();
            translation = Vector3.Zero;
            rotation = Quaternion.Identity;
        }

        /// <summary>
        /// Determine if the given object is part of the selection.
        /// </summary>
        /// <param name="obj">The object to check for.</param>
        /// <returns>True if the object is in the collection, false if it is not.</returns>
        public bool contains(SelectableObject obj)
        {
            return selectedObjects.Contains(obj);
        }

        /// <summary>
        /// Get an Enumerable over all the selected objects.
        /// </summary>
        /// <returns>An enumerable with all selected objects.</returns>
        public IEnumerable<SelectableObject> getSelectedObjects()
        {
            return selectedObjects;
        }

        /// <summary>
        /// Determine if this collection has some items selected.
        /// </summary>
        /// <returns>True if items are selected, false if they are not.</returns>
        public bool hasEntries()
        {
            return selectedObjects.Count != 0;
        }

        /// <summary>
        /// Get the current translation of this collection.
        /// </summary>
        /// <returns>The current translation of this collection.</returns>
        public Vector3 getTranslation()
        {
            return translation;
        }

        /// <summary>
        /// Get the current rotation of this collection.
        /// </summary>
        /// <returns>The current rotation of this collection.</returns>
        public Quaternion getRotation()
        {
            return rotation;
        }

        /// <summary>
        /// Translate the elements in the collection to the new location relative to their
        /// positions when they were selected.
        /// </summary>
        /// <param name="newLoc">The new location to translate relative to.</param>
        public void translate(ref Vector3 newLoc)
        {
            for (int i = 0; i < selectedObjects.Count; i++)
            {
                Vector3 localTrans = newLoc + Quaternion.quatRotate(rotation, translationOffsets[i]);
                selectedObjects[i].editTranslation(ref localTrans);
            }
            translation = newLoc;
        }

        /// <summary>
        /// Rotate the elements in this collection relative to the center point of the collection.
        /// </summary>
        /// <param name="newRot">The rotation to rotate relative to.</param>
        public void rotate(ref Quaternion newRot)
        {
            //For many selected objects rotate relative to the original rotation.
            if (selectedObjects.Count > 1)
            {
                for (int i = 0; i < selectedObjects.Count; i++)
                {
                    Vector3 localTrans = translationOffsets[i];
                    localTrans = Quaternion.quatRotate(ref newRot, ref localTrans) + this.getTranslation();
                    Quaternion localRot = newRot * rotationOffsets[i];
                    selectedObjects[i].editPosition(ref localTrans, ref localRot);
                }
            }
            //If only one object is selected set its rotation absolutely.
            else if (selectedObjects.Count == 1)
            {
                selectedObjects[0].editRotation(ref newRot);
            }
            rotation = newRot;
        }

        /// <summary>
        /// Get the number of items selected.
        /// </summary>
        public int Count
        {
            get
            {
                return selectedObjects.Count;
            }
        }

        /// <summary>
        /// Helper function to get the average translation for the pivot, the relative
        /// translations from the pivot and starting rotations for all objects.
        /// </summary>
        private void computeOffsets()
        {
            translation = Vector3.Zero;

            if (selectedObjects.Count == 1)
            {
                rotation = selectedObjects[0].getRotation();
            }
            else
            {
                rotation = Quaternion.Identity;
            }

            translationOffsets.Clear();
            rotationOffsets.Clear();
            if (selectedObjects.Count != 0)
            {
                foreach (SelectableObject selected in selectedObjects)
                {
                    Vector3 trans = selected.getTranslation();
                    translation += trans;
                    translationOffsets.Add(trans);
                    rotationOffsets.Add(selected.getRotation());
                }
                translation /= (float)selectedObjects.Count;
                for (int i = 0; i < translationOffsets.Count; i++)
                {
                    translationOffsets[i] = translationOffsets[i] - translation;
                }
            }
        }
    }
}
