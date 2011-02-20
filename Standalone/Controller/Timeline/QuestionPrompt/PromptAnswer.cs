using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Logging;

namespace Medical
{
    public class PromptAnswer : Saveable
    {
        public PromptAnswer()
        {

        }

        public PromptAnswer(String text)
        {
            this.Text = text;
        }

        internal void dumpToLog()
        {
            Log.Debug("|-- Answer: {0}", Text);
            if (Action != null)
            {
                Action.dumpToLog();
            }
            else
            {
                Log.Debug("|--- No Action Defined");
            }
            Log.Debug("|");
        }

        internal void findFileReference(TimelineStaticInfo info)
        {
            if (Action != null)
            {
                Action.findFileReference(info, Text);
            }
        }

        public String Text { get; set; }

        public PromptAnswerAction Action { get; set; }

        #region Saveable Members

        private const String TEXT = "Text";
        private const String ACTION = "Action";

        protected PromptAnswer(LoadInfo info)
        {
            Text = info.GetString(TEXT);
            Action = info.GetValue<PromptAnswerAction>(ACTION, null);
        }

        public void getInfo(SaveInfo info)
        {
            info.AddValue(TEXT, Text);
            info.AddValue(ACTION, Action);
        }

        #endregion
    }
}
