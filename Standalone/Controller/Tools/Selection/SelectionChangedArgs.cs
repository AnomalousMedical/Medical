using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    /// <summary>
    /// This is a class that is dispatched with the SelectionChangedEvent to help figure
    /// out what was changed about the selection.  This is pooled by the SelectedObjectManager
    /// so it is only valid during the callback.
    /// </summary>
    public class SelectionChangedArgs
    {
        private List<SelectableObject> objectsAdded = new List<SelectableObject>();
        private List<SelectableObject> objectsRemoved = new List<SelectableObject>();

        /// <summary>
        /// Constructor, internal use only.
        /// </summary>
        /// <param name="owner">The SelectedObjectManager that owns this args.</param>
        internal SelectionChangedArgs(SelectionController owner)
        {
            this.Owner = owner;
        }

        /// <summary>
        /// Get the owner of these args.
        /// </summary>
        public SelectionController Owner { get; private set; }

        /// <summary>
        /// An enum of all objects added for this event.
        /// </summary>
        public IEnumerable<SelectableObject> ObjectsAdded
        {
            get
            {
                return objectsAdded;
            }
        }

        /// <summary>
        /// An enum of all objects removed for this event.
        /// </summary>
        public IEnumerable<SelectableObject> ObjectsRemoved
        {
            get
            {
                return objectsRemoved;
            }
        }

        /// <summary>
        /// Reset the args to be used again.
        /// </summary>
        internal void reset()
        {
            objectsAdded.Clear();
            objectsRemoved.Clear();
        }

        /// <summary>
        /// Set multiple objects added at once.
        /// </summary>
        /// <param name="added">An enum of objects that were added.</param>
        internal void setObjectsAdded(IEnumerable<SelectableObject> added)
        {
            objectsAdded.AddRange(added);
        }

        /// <summary>
        /// Set a single object added.
        /// </summary>
        /// <param name="added">The object added.</param>
        internal void setObjectAdded(SelectableObject added)
        {
            objectsAdded.Add(added);
        }

        /// <summary>
        /// Set multiple objects removed.
        /// </summary>
        /// <param name="removed">An enum of objects removed.</param>
        internal void setObjectsRemoved(IEnumerable<SelectableObject> removed)
        {
            objectsRemoved.AddRange(removed);
        }

        /// <summary>
        /// Set a single object as removed.
        /// </summary>
        /// <param name="removed">The object that was removed.</param>
        internal void setObjectRemoved(SelectableObject removed)
        {
            objectsRemoved.Add(removed);
        }
    }
}
