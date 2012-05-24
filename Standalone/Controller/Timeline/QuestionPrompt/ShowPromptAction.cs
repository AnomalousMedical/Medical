﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Logging;
using SoundPlugin;
using Engine.Editing;

namespace Medical
{
    public class ShowPromptAction : TimelineInstantAction
    {
        private List<PromptQuestion> questions = new List<PromptQuestion>();
        private String soundFile = null;
        private Source source;
        private TimelineController timelineControllerAfterDoAction; //This will be stored in doAction because it will go away when the timeline stops

        public ShowPromptAction()
        {

        }

        public override void doAction()
        {
            timelineControllerAfterDoAction = TimelineController;
            IQuestionProvider questionProvider = TimelineController.QuestionProvider;
            questionProvider.clear();
            foreach (PromptQuestion question in questions)
            {
                questionProvider.addQuestion(question);
            }
            questionProvider.showPrompt(answerSelected);
            if (soundFile != null && soundFile != String.Empty)
            {
                source = TimelineController.playSound(soundFile);
                if (source != null)
                {
                    source.PlaybackFinished += new SourceFinishedDelegate(source_PlaybackFinished);
                }
            }
        }

        public override void dumpToLog()
        {
            if (SoundFile != null)
            {
                Log.Debug("| ShowPromptAction - Sound File \"{0}\"", SoundFile);
            }
            else
            {
                Log.Debug("| ShowPromptAction");
            }
            foreach (PromptQuestion question in questions)
            {
                question.dumpToLog();
            }
        }

        public override void findFileReference(TimelineStaticInfo info)
        {
            if (info.matchesPattern(SoundFile))
            {
                info.addMatch(this.GetType(), "Show Prompt sound file.", SoundFile);
            }
            foreach (PromptQuestion question in questions)
            {
                question.findFileReference(info);
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

        public enum CustomEditQueries
        {
            OpenQuestionEditor,
            ConvertToMvc
        }

        protected override void customizeEditInterface(EditInterface editInterface)
        {
            base.customizeEditInterface(editInterface);
            editInterface.addCommand(new EditInterfaceCommand("Edit Questions", showQuestionEditor));
            editInterface.addCommand(new EditInterfaceCommand("Convert to MVC", convertToMvc));
        }

        private void showQuestionEditor(EditUICallback callback, EditInterfaceCommand caller)
        {
            callback.runCustomQuery(CustomEditQueries.OpenQuestionEditor, delegate(Object result, ref String message)
            {
                EditQuestionsResults questionResults = result as EditQuestionsResults;
                if (questionResults != null)
                {
                    questions.Clear();
                    addQuestion(questionResults.Question);
                    this.SoundFile = questionResults.SoundFile;
                    return true;
                }
                else
                {
                    message = "Returned class was not a EditQuestionResults, can not read data.";
                    return false;
                }
            }, this);
        }

        private void convertToMvc(EditUICallback callback, EditInterfaceCommand caller)
        {
            callback.runCustomQuery(CustomEditQueries.ConvertToMvc, delegate(Object result, ref String message)
            {
                return true;
            }, this);
        }

        private void answerSelected(PromptAnswer answer)
        {
            if (answer.Action != null)
            {
                answer.Action.execute(timelineControllerAfterDoAction);
            }
            else
            {
                timelineControllerAfterDoAction._fireMultiTimelineStopEvent();
                //Log.Warning("Answer {0} does not have an action. Nothing will be done", answer.Text);
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
