using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    /// <summary>
    /// The state for an individual tooth.
    /// </summary>
    public class ToothState
    {
        private bool extracted;

        public ToothState(bool extracted)
        {
            this.extracted = extracted;
        }

        public bool Extracted
        {
            get
            {
                return extracted;
            }
            set
            {
                extracted = value;
            }
        }
    }
}
