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
        private EventLayer currentEventLayer = null;
        private TravelTracker travelTracker = new TravelTracker();
        private IEnumerator<Anatomy> currentClickAnatomy = null;
        private IntVector3 lastMousePos;

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
        public void setNewResults(IEnumerable<Anatomy> matches, EventLayer eventLayer)
        {
            stopListeningToEvents();
            this.currentEventLayer = eventLayer;
            currentClickAnatomy = matches.GetEnumerator();
            PreviousMatch = null;
            if(currentClickAnatomy.MoveNext())
            {
                lastMousePos = currentEventLayer.Mouse.AbsolutePosition;
                currentEventLayer.Mouse.Moved += Mouse_Moved;
                travelTracker.reset();
            }
            else
            {
                currentClickAnatomy = null;
            }
        }

        /// <summary>
        /// Advance the iterator.
        /// </summary>
        public void moveNext()
        {
            if(currentClickAnatomy != null)
            {
                PreviousMatch = CurrentMatch;
                if(!currentClickAnatomy.MoveNext())
                {
                    stopListeningToEvents();
                    currentClickAnatomy = null;
                    PreviousMatch = null;
                }
            }
        }

        /// <summary>
        /// Clear out any current matches.
        /// </summary>
        public void clear()
        {
            stopListeningToEvents();
            currentClickAnatomy = null;
            PreviousMatch = null;
        }

        /// <summary>
        /// Will be true if a new click search should be done and false if this iterator
        /// still has more results.
        /// </summary>
        public bool DoNewClickSearch
        {
            get
            {
                return currentClickAnatomy == null;
            }
        }

        /// <summary>
        /// The current match, will be null if there is no results.
        /// </summary>
        public Anatomy CurrentMatch
        {
            get
            {
                return currentClickAnatomy != null ? currentClickAnatomy.Current : null;
            }
        }

        /// <summary>
        /// The anatomy that was selected previously.
        /// </summary>
        public Anatomy PreviousMatch { get; private set; }

        void Mouse_Moved(Mouse mouse)
        {
            travelTracker.traveled(mouse.AbsolutePosition - lastMousePos); //Have to use absolute position since RelativePosition has not been calculated
            lastMousePos = mouse.AbsolutePosition;
            if(travelTracker.TraveledOverLimit)
            {
                stopListeningToEvents();
                currentClickAnatomy = null;
                PreviousMatch = null;
            }
        }

        private void stopListeningToEvents()
        {
            if (currentEventLayer != null)
            {
                currentEventLayer.Mouse.Moved -= Mouse_Moved;
                currentEventLayer = null;
            }
        }
    }
}
