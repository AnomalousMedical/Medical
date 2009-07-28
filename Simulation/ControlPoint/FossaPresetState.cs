using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    public class FossaPresetState : PresetState
    {
        private Dictionary<String, float> positions = new Dictionary<string, float>();

        public FossaPresetState(String name, String category, String imageName)
            : base(name, category, imageName)
        {

        }

        public void addPosition(String fossa, float position)
        {
            positions.Add(fossa, position);
        }

        public override void applyToState(MedicalState state)
        {
            foreach (String position in positions.Keys)
            {
                state.Fossa.addPosition(position, positions[position]);
            }
        }
    }
}
