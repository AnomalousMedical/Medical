using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;
using Engine.Saving;

namespace Medical.Model
{
    public class WizardStateInfo : Saveable
    {
        public WizardStateInfo()
        {
            ProcedureDate = DateTime.Now;
            DataSource = "Anomalous Medical";
            Name = "Custom Distortion";
            Notes = "";
        }

        public DateTime ProcedureDate { get; set; }

        [Editable]
        public String DataSource { get; set; }

        [Editable]
        public String Notes { get; set; }

        [Editable]
        public String Name { get; set; }

        protected WizardStateInfo(LoadInfo info)
        {
            DataSource = info.GetString("DataSource");
            Notes = info.GetString("Notes");
            Name = info.GetString("Name");
            ProcedureDate = new DateTime(info.GetInt64("ProcedureDate"));
        }

        public void getInfo(SaveInfo info)
        {
            info.AddValue("DataSource", DataSource);
            info.AddValue("Notes", Notes);
            info.AddValue("Name", Name);
            info.AddValue("ProcedureDate", ProcedureDate.Ticks);
        }
    }
}
