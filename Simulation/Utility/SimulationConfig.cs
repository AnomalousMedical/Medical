using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    public class SimulationConfig
    {
        public const float UnitsToMM = 8.467f;

        public const float MMToUnits = 1 / UnitsToMM;

        private SimulationConfig()
        {

        }

        public static float GetCm(float engineUnit)
        {
            return engineUnit * UnitsToMM / 10;
        }

        public static float GetMm(float engineUnit)
        {
            return engineUnit * UnitsToMM;
        }
    }
}
