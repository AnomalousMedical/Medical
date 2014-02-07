using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.Controller.AnomalousMvc
{
    class TeethHighlightState
    {
        private bool highlighted = false;

        public bool Highlighted
        {
            get
            {
                return highlighted;
            }
            set
            {
                highlighted = value;
            }
        }
    }
}
