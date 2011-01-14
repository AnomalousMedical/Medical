using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace Medical
{
    public enum SelectionMode
    {
        AddElement,
        RemoveElement,
        SingleElement
    }

    public delegate void ObjectSelected(SelectionChangedArgs args);
    public delegate void SelectionTranslated(Vector3 newPosition);
    public delegate void SelectionRotated(Quaternion newRotation);

    /// <summary>
    /// This class helps mediate what object is selected at a given time.
    /// </summary>
    public class SelectionController
    {
        public event ObjectSelected OnSelectionChanged;
        public event SelectionTranslated OnSelectionTranslated;
        public event SelectionRotated OnSelectionRotated;

        private SelectedObjectCollection selectedObjects = new SelectedObjectCollection();
        private SelectionChangedArgs changedArgs;

        /// <summary>
        /// Constructor.
        /// </summary>
        public SelectionController()
        {
            changedArgs = new SelectionChangedArgs(this);
        }

        /// <summary>
        /// Call this funciton to set a single selected object.
        /// </summary>
        /// <param name="toSelect">The sim object to set as selected.</param>
        public void setSelectedObject(SelectableObject toSelect)
        {
            changedArgs.reset();
            changedArgs.setObjectsRemoved(selectedObjects.getSelectedObjects());

            selectedObjects._clear();

            changedArgs.setObjectAdded(toSelect);
            selectedObjects._addObject(toSelect);
            fireSelectionChanged();
        }

        /// <summary>
        /// Add a SimObject to the selection.
        /// </summary>
        /// <param name="toSelect">The SelectableObject to add to the selection.</param>
        public void addSelectedObject(SelectableObject toSelect)
        {
            changedArgs.reset();
            changedArgs.setObjectAdded(toSelect);
            selectedObjects._addObject(toSelect);
            fireSelectionChanged();
        }

        /// <summary>
        /// Remove a SimObject from the selection.
        /// </summary>
        /// <param name="toRemove">The SelectableObject to remove.</param>
        public void removeSelectedObject(SelectableObject toRemove)
        {
            changedArgs.reset();
            changedArgs.setObjectRemoved(toRemove);
            selectedObjects._removeObject(toRemove);
            fireSelectionChanged();
        }

        /// <summary>
        /// Set several objects as the selected objects.
        /// </summary>
        /// <param name="objects">A LinkedList of objects to add.</param>
        public void setSelectedObjects(LinkedList<SelectableObject> objects)
        {
            changedArgs.reset();
            changedArgs.setObjectsRemoved(selectedObjects.getSelectedObjects());
            selectedObjects._clear();
            foreach (SelectableObject obj in objects)
            {
                changedArgs.setObjectAdded(obj);
                selectedObjects._addObject(obj);
            }
            fireSelectionChanged();
        }

        /// <summary>
        /// Add several objects to the selection.
        /// </summary>
        /// <param name="objects">A LinkedList of objects to add.</param>
        public void addSelectedObjects(LinkedList<SelectableObject> objects)
        {
            changedArgs.reset();
            foreach (SelectableObject obj in objects)
            {
                changedArgs.setObjectAdded(obj);
                selectedObjects._addObject(obj);
            }
            fireSelectionChanged();
        }

        /// <summary>
        /// Remove several objects from the selection.
        /// </summary>
        /// <param name="objects">A LinkedList of objects to remove.</param>
        public void removeSelectedObjects(LinkedList<SelectableObject> objects)
        {
            changedArgs.reset();
            foreach (SelectableObject obj in objects)
            {
                changedArgs.setObjectRemoved(obj);
                selectedObjects._removeObject(obj);
            }
            fireSelectionChanged();
        }

        /// <summary>
        /// Clear the current selection leaving nothing selected.
        /// </summary>
        public void clearSelection()
        {
            changedArgs.reset();
            changedArgs.setObjectsRemoved(selectedObjects.getSelectedObjects());
            selectedObjects._clear();
            fireSelectionChanged();
        }

        /// <summary>
        /// Move the selected object and alert all listeners.
        /// </summary>
        /// <param name="newTrans">The new translation to set.</param>
        public void translateSelectedObject(ref Vector3 newTrans)
        {
            selectedObjects.translate(ref newTrans);
            if (OnSelectionTranslated != null)
            {
                OnSelectionTranslated.Invoke(selectedObjects.getTranslation());
            }
        }

        /// <summary>
        /// Rotate the selected object and alert all listeners.
        /// </summary>
        /// <param name="newRot">The new rotation to set.</param>
        public void rotateSelectedObject(ref Quaternion newRot)
        {
            selectedObjects.rotate(ref newRot);
            if (OnSelectionRotated != null)
            {
                OnSelectionRotated.Invoke(selectedObjects.getRotation());
            }
        }

        /// <summary>
        /// Determine if any objects are selected.
        /// </summary>
        /// <returns>True if at least one object is selected.</returns>
        public bool hasSelection()
        {
            return selectedObjects.hasEntries();
        }

        /// <summary>
        /// Get an enum over all objects that are selected.
        /// </summary>
        /// <returns>An enum over all selected objects.</returns>
        public IEnumerable<SelectableObject> getSelectedObjects()
        {
            return selectedObjects.getSelectedObjects();
        }

        /// <summary>
        /// Get the translation of the selected object(s).
        /// </summary>
        /// <returns>The translation or pivot for multiple objects.</returns>
        public Vector3 getSelectionTranslation()
        {
            return selectedObjects.getTranslation();
        }

        /// <summary>
        /// Get the rotation of the selected object(s).
        /// </summary>
        /// <returns>The rotation of a single selected object or the current relative rotation of multiple objects.</returns>
        public Quaternion getSelectionRotation()
        {
            return selectedObjects.getRotation();
        }

        /// <summary>
        /// Helper function to to fire the OnSelectionChanged event.
        /// </summary>
        void fireSelectionChanged()
        {
            if (OnSelectionChanged != null)
            {
                OnSelectionChanged.Invoke(changedArgs);
            }
        }
    }
}
