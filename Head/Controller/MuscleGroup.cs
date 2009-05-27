using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    /// <summary>
    /// This class contains true and false values for various muscle names that can be
    /// turned on and off as a unit.  All muscles in a group are by default to be turned
    /// off so only the ones that need to be turned on have to be added.  Values can be
    /// added both on and off, however.
    /// </summary>
    class MuscleGroup
    {
        Dictionary<String, bool> activations = new Dictionary<string, bool>();
        private String name;

        public MuscleGroup(String name)
        {
            this.name = name;
        }

        public void setActivation(String muscle, bool activated)
        {
            if (!activations.ContainsKey(muscle))
            {
                activations.Add(muscle, activated);
            }
            else
            {
                activations[muscle] = activated;
            }
        }

        public bool getActivation(String muscle)
        {
            if (activations.ContainsKey(muscle))
            {
                return activations[muscle];
            }
            return false;
        }

        public override string ToString()
        {
            return name;
        }
    }
}
