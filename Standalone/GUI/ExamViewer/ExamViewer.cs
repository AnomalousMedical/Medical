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
        private MultiList examList;

        public ExamViewer(ExamController examController)
            :base("Medical.GUI.ExamViewer.ExamViewer.layout")
        {
            this.examController = examController;
            examController.ExamAdded += new ExamController.ExamControllerEvent(examController_ExamAdded);
            examController.ExamRemoved += new ExamController.ExamControllerEvent(examController_ExamRemoved);
            examController.ExamsCleared += new ExamController.ExamControllerEvent(examController_ExamsCleared);

            examList = (MultiList)window.findWidget("ExamList");
            examList.addColumn("Exam", examList.Width / 2);
            examList.addColumn("Date", examList.Width / 2);
            examList.ListSelectAccept += new MyGUIEvent(examList_ListSelectAccept);

            this.Resized += new EventHandler(ExamViewer_Resized);
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
                exam.showBreakdownGUI();
            }
        }

        void ExamViewer_Resized(object sender, EventArgs e)
        {
            examList.setColumnWidthAt(0, examList.Width / 2);
            examList.setColumnWidthAt(1, examList.Width / 2);
        }
    }
}
