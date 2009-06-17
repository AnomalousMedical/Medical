using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    public abstract class MedicalState
    {
        public MedicalState(String name)
        {
            Name = name;
        }

        public String Name { get; set; }

        public abstract void blend(float percent, MedicalState target);

        public abstract void update();
    }
}
