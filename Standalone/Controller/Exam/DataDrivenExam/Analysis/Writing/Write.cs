using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;

namespace Medical.Controller.Exam
{
    public class Write : WritingAction
    {
        private List<DataRetriever> printData = new List<DataRetriever>();

        public Write()
        {

        }

        public Write(String text)
        {
            this.Text = text;
        }

        public Write(String text, params DataRetriever[] dataRetrievers)
        {
            this.Text = text;
            printData.AddRange(dataRetrievers);
        }

        public override void execute(DataDrivenExam exam, DocumentBuilder docBuilder)
        {
            Object[] writeArgs = new Object[printData.Count];
            for (int i = 0; i < printData.Count; ++i)
            {
                writeArgs[i] = printData[i].getData<Object>(exam, "").ToString();
            }
            docBuilder.write(Text, writeArgs);
        }

        public void addData(DataRetriever retriver)
        {
            printData.Add(retriver);
        }

        public void removeData(DataRetriever retriever)
        {
            printData.Remove(retriever);
        }

        public String Text { get; set; }

        protected Write(LoadInfo info)
            :base(info)
        {
            Text = info.GetString("Text");
            info.RebuildList<DataRetriever>("Data", printData);
        }

        public override void getInfo(SaveInfo info)
        {
            info.AddValue("Text", Text);
            info.ExtractList<DataRetriever>("Data", printData);
        }
    }
}
