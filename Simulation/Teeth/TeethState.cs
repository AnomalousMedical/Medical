using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    public class TeethState
    {
        private Dictionary<String, ToothState> teeth = new Dictionary<string, ToothState>();

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
    }
}
