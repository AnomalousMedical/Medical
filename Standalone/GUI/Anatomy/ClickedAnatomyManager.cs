using Engine;
using Engine.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical.GUI
{
    /// <summary>
    /// This class manages the iterator for the currently clicked anatomy. It helps ensure we don't oversubscribe 
    /// to mouse updates and makes handling the match iterator a bit easier to understand.
    /// </summary>
    class ClickedAnatomyManager
    {
        private IEnumerator<Anatomy> currentAnatomy = null;
        private AnatomyIdentifier reclickAnatomy;

        /// <summary>
        /// Constructor.
        /// </summary>
        public ClickedAnatomyManager()
        {

        }

        /// <summary>
        /// Set new click results.
        /// </summary>
        /// <param name="matches">The matches to iterate.</param>
        /// <param name="eventLayer">The event layer to watch for mouse movements.</param>
        public void setNewResults(IEnumerable<Anatomy> matches, AnatomyIdentifier reclickAnatomy)
        {
            currentAnatomy = matches.GetEnumerator();
            PreviousMatch = null;
            if(currentAnatomy.MoveNext())
            {
                this.reclickAnatomy = reclickAnatomy;
            }
            else
            {
                this.reclickAnatomy = null;
                currentAnatomy = null;
            }
        }

        /// <summary>
        /// Advance the iterator.
        /// </summary>
        public void moveNext()
        {
            if(currentAnatomy != null)
            {
                PreviousMatch = CurrentMatch;
                if(!currentAnatomy.MoveNext())
                {
                    currentAnatomy = null;
                    PreviousMatch = null;
                    reclickAnatomy = null;
                }
            }
        }

        /// <summary>
        /// Clear out any current matches.
        /// </summary>
        public void clear()
        {
            currentAnatomy = null;
            reclickAnatomy = null;
            PreviousMatch = null;
        }

        /// <summary>
        /// Determine if the piece of anatomy that was clicked is the same as what was reclicked.
        /// </summary>
        /// <param name="anatomy"></param>
        /// <returns></returns>
        public bool clickedSameAnatomy(Anatomy anatomy)
        {
            if(anatomy == null && reclickAnatomy == null)
            {
                return false;
            }
            return reclickAnatomy == anatomy;
        }

        /// <summary>
        /// The current match, will be null if there is no results.
        /// </summary>
        public Anatomy CurrentMatch
        {
            get
            {
                return currentAnatomy != null ? currentAnatomy.Current : null;
            }
        }

        /// <summary>
        /// The anatomy that was selected previously.
        /// </summary>
        public Anatomy PreviousMatch { get; private set; }
    }
}
