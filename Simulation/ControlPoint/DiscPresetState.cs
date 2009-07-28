using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace Medical
{
    public class DiscPresetState: PresetState
    {
        private Dictionary<String, float> positions = new Dictionary<string, float>();

        public DiscPresetState(String name, String category, String imageName)
            : base(name, category, imageName)
        {

        }

        public void addPosition(String disc, float position)
        {
            positions.Add(disc, position);
        }
    
        public override void applyToState(MedicalState state)
        {
            foreach (String discName in positions.Keys)
            {
                Disc disc = DiscController.getDisc(discName);
                Vector3 offset = disc.getNormalOffset();
                offset *= positions[discName];
                state.Disc.addPosition(discName, offset);
            }
        }
    }
}
