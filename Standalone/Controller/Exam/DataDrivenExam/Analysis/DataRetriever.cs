using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;

namespace Medical.Controller.Exam
{
    public class DataRetriever : Saveable
    {
        private List<String> examSections = new List<string>();

        public DataRetriever()
        {

        }

        public DataRetriever(String dataPoint, params String[] sections)
        {
            DataPoint = dataPoint;
            examSections.AddRange(sections);
        }

        public void addExamSection(String section)
        {
            examSections.Add(section);
        }

        public void removeExamSection(String section)
        {
            examSections.Remove(section);
        }

        public T getData<T>(DataDrivenExam exam, T defaultValue)
        {
            DataDrivenExamSection dataSection = exam;
            foreach (String examSectionName in examSections)
            {
                dataSection = dataSection.getSectionNoCreate(examSectionName);
                if (dataSection == null)
                {
                    return defaultValue;
                }
            }
            return dataSection.getValue<T>(DataPoint, defaultValue);
        }

        public String DataPoint { get; set; }

        public IEnumerable<String> ExamSections
        {
            get
            {
                return examSections;
            }
        }

        protected DataRetriever(LoadInfo info)
        {
            DataPoint = info.GetString("DataPoint");
            info.RebuildList<String>("Section", examSections);
        }

        public void getInfo(SaveInfo info)
        {
            info.AddValue("DataPoint", DataPoint);
            info.ExtractList<String>("Section", examSections);
        }
    }
}
