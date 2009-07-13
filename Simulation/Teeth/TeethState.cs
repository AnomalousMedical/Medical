using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;

namespace Medical
{
    public class TeethState : Saveable
    {
        private Dictionary<String, ToothState> teeth = new Dictionary<string, ToothState>();

        public TeethState()
        {

        }

        public void addPosition(String disc, ToothState state)
        {
            teeth.Add(disc, state);
        }

        public void blend(TeethState target, float percent)
        {
            foreach (String key in teeth.Keys)
            {
                ToothState start = teeth[key];
                ToothState end = target.teeth[key];
                TeethController.getTooth(key).Extracted = start.Extracted;
            }
        }

        #region Saveable Members

        private const string TEETH = "Teeth";

        protected TeethState(LoadInfo info)
        {
            info.RebuildDictionary<String, ToothState>(TEETH, teeth);
        }

        public void getInfo(SaveInfo info)
        {
            info.ExtractDictionary<String, ToothState>(TEETH, teeth);
        }

        #endregion
    }
}
