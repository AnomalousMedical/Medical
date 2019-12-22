using Anomaly;
using System;

namespace AnomalousMedical.Anomaly
{
    class Program
    {
        static void Main(string[] args)
        {
            AnomalyProgram.Run(new AnomalousMedicalAnomaly());
        }
    }
}
