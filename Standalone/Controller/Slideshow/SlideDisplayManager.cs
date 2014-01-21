using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    public class SlideDisplayManager
    {
        public event Action<SlideDisplayManager> DisplayModeChanged;

        private float additionalZoomLevel = 1.0f;
        private bool vectorMode = true;

        public SlideDisplayManager()
        {
            
        }

        public float AdditionalZoomMultiple
        {
            get
            {
                return additionalZoomLevel;
            }
            set
            {
                if (additionalZoomLevel != value)
                {
                    additionalZoomLevel = value;
                    fireDisplayModeChanged();
                }
            }
        }

        public bool VectorMode
        {
            get
            {
                return vectorMode;
            }
            set
            {
                if (vectorMode != value)
                {
                    vectorMode = value;
                    fireDisplayModeChanged();
                }
            }
        }

        private void fireDisplayModeChanged()
        {
            if (DisplayModeChanged != null)
            {
                DisplayModeChanged.Invoke(this);
            }
        }
    }
}
