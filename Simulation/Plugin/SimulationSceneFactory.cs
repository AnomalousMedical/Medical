using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.ObjectManagement;

namespace Medical
{
    class SimulationSceneFactory : SimElementFactory 
    {
        #region SimElementFactory Members

        public void clearDefinitions()
        {
            
        }

        public IEnumerable<SceneBuildStatus> createProducts(SceneBuildOptions options)
        {
            SceneBuildStatus status = new SceneBuildStatus()
            {
                Subsystem = "Simulation"
            };
            yield return status;
        }

        public void createStaticProducts()
        {
            
        }

        public void linkProducts()
        {
            
        }

        #endregion
    }
}
