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
            DefaultStateName = "Custom Distortion";
            DefaultNotes = "";
        }

        public override void reset()
        {
            procedureDate = DateTime.Now;
            Notes = DefaultNotes;
            StateName = DefaultStateName;
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
        public String DefaultNotes { get; set; }

        [Editable]
        public String DefaultStateName { get; set; }

        [DoNotSave]
        private String notes;
        public String Notes
        {
            get
            {
                return notes;
            }
            set
            {
                notes = value;
            }
        }

        [DoNotSave]
        private String stateName;
        public String StateName
        {
            get
            {
                return stateName;
            }
            set
            {
                stateName = value;
            }
        }

        protected MedicalStateInfoModel(LoadInfo info)
            :base(info)
        {

        }
    }
}
