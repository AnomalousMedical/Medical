using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    delegate void PromptAnswerSelected(PromptAnswer answer);

    interface IQuestionProvider
    {
        void addQuestion(PromptQuestion question);

        void showPrompt(PromptAnswerSelected answerSelectedCallback);

        void clear();
    }
}
