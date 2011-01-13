using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Logging;
using SoundPlugin;

namespace Medical
{
    public class ShowPromptAction : TimelineInstantAction
    {
        private List<PromptQuestion> questions = new List<PromptQuestion>();
        private String soundFile = null;
        private Source source;

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
            if (soundFile != null)
            {
                source = TimelineController.playSound(soundFile);
                source.PlaybackFinished += new SourceFinishedDelegate(source_PlaybackFinished);
            }
        }

        public void addQuestion(PromptQuestion question)
        {
            questions.Add(question);
        }

        public void removeQuestion(PromptQuestion question)
        {
            questions.Remove(question);
        }

        public IEnumerable<PromptQuestion> Questions
        {
            get
            {
                return questions;
            }
        }

        public String SoundFile
        {
            get
            {
                return soundFile;
            }
            set
            {
                soundFile = value;
            }
        }

        private void answerSelected(PromptAnswer answer)
        {
            if (answer.Action != null)
            {
                answer.Action.execute(TimelineController);
            }
            else
            {
                Log.Warning("Answer {0} does not have an action. Nothing will be done", answer.Text);
            }
            if (source != null)
            {
                source.stop();
            }
        }

        void source_PlaybackFinished(Source source)
        {
            this.source = null;
        }

        #region Saving

        private const String QUESTIONS = "Questions";
        private const String SOUND = "Sound";

        protected ShowPromptAction(LoadInfo info)
            :base(info)
        {
            info.RebuildList<PromptQuestion>(QUESTIONS, questions);
            soundFile = info.GetString(SOUND, null);
        }

        public override void getInfo(SaveInfo info)
        {
            base.getInfo(info);
            info.ExtractList<PromptQuestion>(QUESTIONS, questions);
            info.AddValue(SOUND, soundFile);
        }

        #endregion
    }
}
