using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace Medical
{
    public class DiscPresetState: PresetState
    {
        private Dictionary<String, Vector3> positions = new Dictionary<string, Vector3>();

        public DiscPresetState(String name, String category, String imageName)
            : base(name, category, imageName)
        {

        }

        public void addPosition(String disc, Vector3 position)
        {
            positions.Add(disc, position);
        }
    
        public override void applyToState(MedicalState state)
        {
            foreach (String position in positions.Keys)
            {
                state.Disc.addPosition(position, positions[position]);
            }
        }
    }
}
