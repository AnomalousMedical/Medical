using Anomaly;
using System;

namespace AnomalousMedical.Anomaly
{
    class Program
    {
        static void Main(string[] args)
        {
            Engine.Saving.Saver.DefaultOutputType = Engine.Saving.SaverOutputType.Xml; //Keeping Anomalous Medical on XML for now
            AnomalyProgram.Run(new AnomalousMedicalAnomaly());
        }
    }
}
