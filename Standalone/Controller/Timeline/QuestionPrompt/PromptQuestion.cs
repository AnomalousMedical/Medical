using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Logging;

namespace Medical
{
    public class PromptQuestion : Saveable
    {
        private List<PromptAnswer> answers = new List<PromptAnswer>();

        public PromptQuestion()
        {

        }

        public PromptQuestion(String text)
        {
            this.Text = text;
        }

        public void addAnswer(PromptAnswer answer)
        {
            answers.Add(answer);
        }

        public void removeAnswer(PromptAnswer answer)
        {
            answers.Remove(answer);
        }

        internal void dumpToLog()
        {
            Log.Debug("|- Question: {0}", Text);
            foreach (PromptAnswer answer in answers)
            {
                answer.dumpToLog();
            }
        }

        public String Text { get; set; }

        public IEnumerable<PromptAnswer> Answers
        {
            get
            {
                return answers;
            }
        }

        #region Saveable Members

        private const String TEXT = "Text";
        private const String ANSWER = "Answer";

        protected PromptQuestion(LoadInfo info)
        {
            Text = info.GetString(TEXT);
            info.RebuildList<PromptAnswer>(ANSWER, answers);
        }

        public void getInfo(SaveInfo info)
        {
            info.AddValue(TEXT, Text);
            info.ExtractList<PromptAnswer>(ANSWER, answers);
        }

        #endregion
    }
}
