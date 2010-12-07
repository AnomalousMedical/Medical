using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.GUI
{
    class MyGUIQuestionProvider : Dialog, IQuestionProvider
    {
        private ScrollView questionScroll;

        public MyGUIQuestionProvider()
            :base("Medical.GUI.Timeline.QuestionProvider.MyGUIQuestionProvider.layout")
        {

        }

        public void addQuestion(PromptQuestion question)
        {
            
        }

        public void showPrompt(PromptAnswerSelected answerSelectedCallback)
        {
            
        }

        public void clear()
        {
            
        }
    }
}
