using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.Model
{
    public class WizardStateInfo
    {
        public WizardStateInfo()
        {
            ProcedureDate = DateTime.Now;
            DataSource = "Anomalous Medical";
            Name = "Custom Distortion";
            Notes = "";
        }

        public DateTime ProcedureDate { get; set; }

        public String DataSource { get; set; }

        public String Notes { get; set; }

        public String Name { get; set; }
    }
}
