using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Medical.Controller
{
    public delegate void StatusUpdater(String status);

    public class StatusController
    {
        public static event StatusUpdater StatusChanged;

        private StatusController()
        {

        }

        public static void SetStatus(String status)
        {
            if (StatusChanged != null)
            {
                StatusChanged.Invoke(status);
            }
        }

        public static void TaskCompleted()
        {
            SetStatus("Ready");
        }
    }
}
