using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;
using Engine.Saving;
using Medical.Controller.AnomalousMvc;
using Engine.Attributes;

namespace Medical.Model
{
    public class MedicalStateInfoModel : MvcModel
    {
        public const String DefaultName = "DefaultMedicalStateInfoModel";

        public MedicalStateInfoModel(String name)
            :base(name)
        {
            DataSource = "Anomalous Medical";
            StateName = "Custom Distortion";
            Notes = "";
        }

        public override void reset()
        {
            procedureDate = DateTime.Now;
        }

        [DoNotSave]
        private DateTime procedureDate;
        public DateTime ProcedureDate
        {
            get
            {
                return procedureDate;
            }
            set
            {
                procedureDate = value;
            }
        }

        [Editable]
        public String DataSource { get; set; }

        [Editable]
        public String Notes { get; set; }

        [Editable]
        public String StateName { get; set; }

        protected MedicalStateInfoModel(LoadInfo info)
            :base(info)
        {

        }
    }
}
