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
            procedureDate = DateTime.Now;
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
            //Check to see if the notes look like rtf.
            if (notes.StartsWith("{\\rtf1"))
            {
                //Do some really basic rtf parsing.
                parseRTF();
            }
        }

        public void getInfo(SaveInfo info)
        {
            info.AddValue(PROCEDURE_DATE, procedureDate.Ticks);
            info.AddValue(DATA_SOURCE, dataSource);
            info.AddValue(NOTES, notes);
        }

        /// <summary>
        /// This method does really basic rtf parsing to the notes.
        /// </summary>
        private void parseRTF()
        {
            //Find the first line and eliminate it.
            int firstLineIndex = notes.IndexOf('\n');
            if (firstLineIndex >= 0)
            {
                notes = notes.Substring(firstLineIndex + 1);
            }

            //See if the next line starts with a \ meaning it has some instructions that need parsing out.
            if (notes.StartsWith("\\"))
            {
                int firstSpaceIndex = notes.IndexOf(' ');
                notes = notes.Substring(firstSpaceIndex + 1);
            }

            //Find the last brace and remove it.
            int lastBraceIndex = notes.LastIndexOf('}');
            if (lastBraceIndex >= 0)
            {
                //Count backwards removing whitespace
                char currentChar = notes[lastBraceIndex - 1];
                while (currentChar == '\r' || currentChar == '\n')
                {
                    currentChar = notes[--lastBraceIndex - 1];
                }
                notes = notes.Substring(0, lastBraceIndex);
            }
            //Replace all special characters.
            notes = notes.Replace("\\par", "")
                .Replace("&amp;", "&")
                .Replace("&lt;", "<")
                .Replace("&gt;", ">")
                .Replace("\\tab", "\t")
                .Replace("\\{", "{")
                .Replace("\\}", "}")
                .Replace("\\\\", "\\");
        }

        #endregion
    }
}
