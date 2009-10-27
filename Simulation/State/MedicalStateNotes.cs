using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;

namespace Medical
{
    public class MedicalStateNotes : Saveable
    {
        private DateTime procedureDate;
        private String dataSource;
        private String notes;

        public MedicalStateNotes()
        {

        }

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

        public String DataSource
        {
            get
            {
                return dataSource;
            }
            set
            {
                dataSource = value;
            }
        }

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

        #region Saveable Members

        private const string PROCEDURE_DATE = "ProcedureDate";
        private const string DATA_SOURCE = "DataSource";
        private const string NOTES = "Notes";

        protected MedicalStateNotes(LoadInfo info)
        {
            procedureDate = new DateTime(info.GetInt64(PROCEDURE_DATE));
            dataSource = info.GetString(DATA_SOURCE);
            notes = info.GetString(NOTES);
        }

        public void getInfo(SaveInfo info)
        {
            info.AddValue(PROCEDURE_DATE, procedureDate.Ticks);
            info.AddValue(DATA_SOURCE, dataSource);
            info.AddValue(NOTES, notes);
        }

        #endregion
    }
}
