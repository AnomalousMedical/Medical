using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.Controller.Exam
{
    public class DataRetriever
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
    }
}
