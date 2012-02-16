using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    public class EditQuestionsResults
    {
        public EditQuestionsResults(PromptQuestion question, String soundFile)
        {
            this.Question = question;
            this.SoundFile = soundFile;
        }

        public PromptQuestion Question { get; set; }

        public String SoundFile { get; set; }
    }
}
