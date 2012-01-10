﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.Controller.Exam
{
    public class ActionBlock : AnalysisAction
    {
        private List<AnalysisAction> actions = new List<AnalysisAction>();

        public void execute(DataDrivenExam exam, DocumentBuilder docBuilder)
        {
            foreach (AnalysisAction action in actions)
            {
                action.execute(exam, docBuilder);
            }
        }

        public void addAction(AnalysisAction action)
        {
            actions.Add(action);
        }

        public void removeAction(AnalysisAction action)
        {
            actions.Remove(action);
        }

        public void insertAction(AnalysisAction action, int index)
        {
            actions.Insert(index, action);
        }
    }
}