using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;

namespace Medical
{
    class ShowPromptAction : TimelineInstantAction
    {
        private List<PromptQuestion> questions = new List<PromptQuestion>();

        public ShowPromptAction()
        {

        }

        public override void doAction()
        {
            IQuestionProvider questionProvider = TimelineController.QuestionProvider;
            questionProvider.clear();
            foreach (PromptQuestion question in questions)
            {
                questionProvider.addQuestion(question);
            }
            questionProvider.showPrompt(answerSelected);
        }

        public void addQuestion(PromptQuestion question)
        {
            questions.Add(question);
        }

        public void removeQuestion(PromptQuestion question)
        {
            questions.Remove(question);
        }

        private void answerSelected(PromptAnswer answer)
        {
            answer.Action.execute(TimelineController);
        }

        #region Saving

        private const String QUESTIONS = "Questions";

        protected ShowPromptAction(LoadInfo info)
            :base(info)
        {
            info.RebuildList<PromptQuestion>(QUESTIONS, questions);
        }

        public override void getInfo(SaveInfo info)
        {
            base.getInfo(info);
            info.ExtractList<PromptQuestion>(QUESTIONS, questions);
        }

        #endregion
    }
}
