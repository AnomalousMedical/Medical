using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine.Editing;

namespace Medical.GUI
{
    class GenericExamOverview : Dialog
    {
        private PropertiesTable propertiesTable;
        private ResizingTable table;
        private Tree indexTree;
        private EditInterfaceTreeView editInterfaceTreeView;
        private int lastWidth;
        private int lastHeight;

        public GenericExamOverview(Exam exam)
            :base("Medical.GUI.ExamViewer.GenericExamOverview.layout")
        {
            EditInterface examEditInterface = exam.EditInterface;

            table = new ResizingTable((ScrollView)window.findWidget("Table"));
            propertiesTable = new PropertiesTable(table);
            propertiesTable.EditInterface = examEditInterface;
            window.Caption = String.Format("{0} - {1}", exam.PrettyName, exam.Date);
            window.WindowChangedCoord += new MyGUIEvent(window_WindowChangedCoord);

            indexTree = new Tree((ScrollView)window.findWidget("Index"));
            editInterfaceTreeView = new EditInterfaceTreeView(indexTree, null);
            editInterfaceTreeView.EditInterface = examEditInterface;
            editInterfaceTreeView.EditInterfaceSelectionChanged += editInterfaceTreeView_EditInterfaceSelectionChanged;
        }

        void editInterfaceTreeView_EditInterfaceSelectionChanged(EditInterfaceViewEvent evt)
        {
            propertiesTable.EditInterface = evt.EditInterface;
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
