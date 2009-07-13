using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;

namespace Medical
{
    /// <summary>
    /// The state for an individual tooth.
    /// </summary>
    public class ToothState : Saveable
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

        #region Saveable Members

        private const string EXTRACTED = "Extracted";

        protected ToothState(LoadInfo info)
        {
            extracted = info.GetBoolean(EXTRACTED);
        }

        public void getInfo(SaveInfo info)
        {
            info.AddValue(EXTRACTED, extracted);
        }

        #endregion
    }
}
