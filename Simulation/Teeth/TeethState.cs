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

        internal TeethState()
        {

        }

        public void addPosition(ToothState state)
        {
            if (teeth.ContainsKey(state.Name))
            {
                teeth.Remove(state.Name);
            }
            teeth.Add(state.Name, state);
        }

        public void blend(TeethState target, float percent)
        {
            foreach (String key in teeth.Keys)
            {
                ToothState start = teeth[key];
                ToothState end = target.teeth[key];
                start.blend(end, percent);
            }
        }

        public IEnumerable<ToothState> StateEnum
        {
            get
            {
                return teeth.Values;
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
