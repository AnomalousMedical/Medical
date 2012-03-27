﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.GUI
{
    public class ExamViewer : MDIDialog
    {
        private ExamController examController;
        private MultiListBox examList;
        private MyGUIAnalysisDisplayProvider displayProvider = new MyGUIAnalysisDisplayProvider();

        public ExamViewer(ExamController examController)
            :base("Medical.GUI.ExamViewer.ExamViewer.layout")
        {
            this.examController = examController;
            examController.ExamAdded += new ExamController.ExamControllerEvent(examController_ExamAdded);
            examController.ExamRemoved += new ExamController.ExamControllerEvent(examController_ExamRemoved);
            examController.ExamsCleared += new ExamController.ExamControllerEvent(examController_ExamsCleared);

            examList = (MultiListBox)window.findWidget("ExamList");
            examList.addColumn("Exam", 50);
            examList.setColumnResizingPolicyAt(0, ResizingPolicy.Fill);
            examList.addColumn("Date", 50);
            examList.setColumnResizingPolicyAt(1, ResizingPolicy.Fill);
            examList.ListSelectAccept += new MyGUIEvent(examList_ListSelectAccept);
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        void examController_ExamsCleared(ExamController examController, Exam exam)
        {
            examList.removeAllItems();
        }

        void examController_ExamRemoved(ExamController examController, Exam exam)
        {
            uint index = examList.findItemWithData(exam);
            if (index != uint.MaxValue)
            {
                examList.removeItemAt(index);
            }
        }

        void examController_ExamAdded(ExamController examController, Exam exam)
        {
            examList.addItem(exam.PrettyName, exam);
            examList.setSubItemNameAt(1, examList.getItemCount() - 1, exam.Date.ToString());
        }

        void examList_ListSelectAccept(Widget source, EventArgs e)
        {
            uint selectedIndex = examList.getIndexSelected();
            if (selectedIndex != uint.MaxValue)
            {
                Exam exam = (Exam)examList.getItemDataAt(selectedIndex);
                ExamAnalyzerMenu menu = new ExamAnalyzerMenu(exam.Analyzers);
                menu.addHistory(exam);
                menu.Closed += new Engine.EventDelegate<ExamAnalyzerMenu>(menu_Closed);
                menu.RunExamAnalyzer += new Engine.EventDelegate<ExamAnalyzerMenu, ExamAnalyzer>(menu_RunExamAnalyzer);
                menu.show(source.AbsoluteLeft, source.AbsoluteTop);
            }
        }

        void menu_Closed(ExamAnalyzerMenu source)
        {
            source.Dispose();
        }

        void menu_RunExamAnalyzer(ExamAnalyzerMenu source, ExamAnalyzer arg)
        {
            uint selectedIndex = examList.getIndexSelected();
            if (selectedIndex != uint.MaxValue)
            {
                Exam exam = (Exam)examList.getItemDataAt(selectedIndex);
                arg.analyzeExam(exam, displayProvider);
            }
        }
    }
}
