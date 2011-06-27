using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.GUI
{
    class GenericExamOverview : Dialog
    {
        private PropertiesTable propertiesTable;
        private ResizingTable table;
        private int lastWidth;
        private int lastHeight;

        public GenericExamOverview(Exam exam)
            :base("Medical.GUI.ExamViewer.GenericExamOverview.layout")
        {
            table = new ResizingTable((ScrollView)window.findWidget("Table"));
            propertiesTable = new PropertiesTable(table);
            propertiesTable.EditInterface = exam.EditInterface;
            window.Caption = String.Format("{0} - {1}", exam.PrettyName, exam.Date);
            window.WindowChangedCoord += new MyGUIEvent(window_WindowChangedCoord);
        }

        void window_WindowChangedCoord(Widget source, EventArgs e)
        {
            if (lastWidth != source.Width || lastHeight != source.Height)
            {
                lastWidth = source.Width;
                lastHeight = source.Height;
                table.layout();
            }
        }

        public override void Dispose()
        {
            propertiesTable.Dispose();
            table.Dispose();
            base.Dispose();
        }

        protected override void onClosed(EventArgs args)
        {
            base.onClosed(args);
            Dispose();
        }
    }
}
