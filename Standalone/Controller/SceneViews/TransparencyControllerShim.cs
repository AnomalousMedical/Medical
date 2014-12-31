using Anomalous.GuiFramework.Cameras;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical
{
    class TransparencyControllerShim : ITransparencyController
    {
        public string ActiveTransparencyState
        {
            get
            {
                return TransparencyController.ActiveTransparencyState;
            }
            set
            {
                TransparencyController.ActiveTransparencyState = value;
            }
        }

        public void applyTransparencyState(string name)
        {
            TransparencyController.applyTransparencyState(name);
        }

        public void createTransparencyState(string name)
        {
            TransparencyController.createTransparencyState(name);
        }

        public void removeTransparencyState(string name)
        {
            TransparencyController.removeTransparencyState(name);
        }
    }
}
